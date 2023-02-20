// Decompiled with JetBrains decompiler
// Type: DuckGame.MysteryGun
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections.Generic;

namespace DuckGame
{
    [EditorGroup("Spawns")]
    [BaggedProperty("isInDemo", true)]
    public class MysteryGun : Thing, IContainPossibleThings
    {
        private SpriteMap _sprite;
        public Type containedType;
        private Thing _addedThing;
        public List<TypeProbPair> contains = new List<TypeProbPair>();

        public MysteryGun(float xpos, float ypos)
          : base(xpos, ypos)
        {
            _sprite = new SpriteMap("mysteryGun", 32, 32);
            graphic = _sprite;
            center = new Vec2(16f, 16f);
            collisionSize = new Vec2(10f, 10f);
            collisionOffset = new Vec2(-5f, -5f);
            depth = (Depth)0.5f;
            _canFlip = false;
            editorTooltip = "Can be configured to spawn a random weapon or item from a specified list.";
            _placementCost += 4;
        }

        public void PreparePossibilities() => containedType = PickType(chanceGroup, contains);

        public static Type PickType(int chanceGroup, List<TypeProbPair> contains)
        {
            ItemBox.GetPhysicsObjects(Editor.Placeables);
            Random random = new Random((int)(Level.GetChanceGroup2(chanceGroup) * 2147483648.0 - 1.0));
            Random generator = Rando.generator;
            Rando.generator = random;
            List<TypeProbPair> typeProbPairList = Utils.Shuffle(contains);
            Type type = null;
            float num = 0f;
            foreach (TypeProbPair typeProbPair in typeProbPairList)
            {
                if (Rando.Float(1f) > 1.0 - typeProbPair.probability)
                {
                    type = typeProbPair.type;
                    break;
                }
                if (typeProbPair.probability > num)
                {
                    num = typeProbPair.probability;
                    type = typeProbPair.type;
                }
            }
            Rando.generator = generator;
            return type;
        }

        public override void Initialize()
        {
            if (Level.current is Editor)
                return;
            ReplaceSelfWithThing();
            if (!Network.isActive || _addedThing == null || loadingLevel == null)
                return;
            _addedThing.PrepareForHost();
        }

        private void ReplaceSelfWithThing()
        {
            if (this.containedType == null)
                PreparePossibilities();
            Type containedType = this.containedType;
            if (containedType != null)
            {
                _addedThing = Editor.CreateObject(containedType) as Thing;
                _addedThing.position = position;
                Level.Add(_addedThing);
            }
            Level.Remove(this);
        }

        public List<TypeProbPair> possible => contains;

        public override ContextMenu GetContextMenu()
        {
            FieldBinding radioBinding = new FieldBinding(this, "contains");
            EditorGroupMenu contextMenu = base.GetContextMenu() as EditorGroupMenu;
            contextMenu.InitializeGroups(new EditorGroup(typeof(PhysicsObject)), radioBinding);
            return contextMenu;
        }

        public override BinaryClassChunk Serialize()
        {
            BinaryClassChunk binaryClassChunk = base.Serialize();
            binaryClassChunk.AddProperty("contains", SerializeTypeProb(contains));
            return binaryClassChunk;
        }

        public override bool Deserialize(BinaryClassChunk node)
        {
            base.Deserialize(node);
            contains = DeserializeTypeProb(node.GetProperty<string>("contains"));
            return true;
        }

        public static string SerializeTypeProb(List<TypeProbPair> list)
        {
            string str = "";
            foreach (TypeProbPair typeProbPair in list)
            {
                str += ModLoader.SmallTypeName(typeProbPair.type);
                str += ":";
                str += typeProbPair.probability.ToString();
                str += "|";
            }
            return str;
        }

        public static List<TypeProbPair> DeserializeTypeProb(string list)
        {
            List<TypeProbPair> typeProbPairList = new List<TypeProbPair>();
            try
            {
                if (list == null)
                    return typeProbPairList;
                string str1 = list;
                char[] chArray = new char[1] { '|' };
                foreach (string str2 in str1.Split(chArray))
                {
                    if (str2.Length > 1)
                    {
                        string[] strArray = str2.Split(':');
                        TypeProbPair typeProbPair = new TypeProbPair()
                        {
                            type = Editor.GetType(strArray[0]),
                            probability = Convert.ToSingle(strArray[1])
                        };
                        typeProbPairList.Add(typeProbPair);
                    }
                }
            }
            catch (Exception)
            {
            }
            return typeProbPairList;
        }

        public override void DrawHoverInfo()
        {
            float num = 0f;
            foreach (TypeProbPair contain in contains)
            {
                if (contain.probability > 0f)
                {
                    Color white = Color.White;
                    Color color = contain.probability != 0f ? (contain.probability >= 0.3f ? (contain.probability >= 0.7f ? Color.Green : Color.Orange) : Colors.DGRed) : Color.DarkGray;
                    string text = contain.type.Name + ": " + contain.probability.ToString("0.000");
                    Graphics.DrawString(text, position + new Vec2((-Graphics.GetStringWidth(text, scale: 0.5f) / 2f), -(16f + num)), color, (Depth)0.9f, scale: 0.5f);
                    num += 4f;
                }
            }
        }
    }
}
