// Decompiled with JetBrains decompiler
// Type: DuckGame.BackgroundTile
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;

namespace DuckGame
{
    public abstract class BackgroundTile : Thing, IStaticRender, IDontUpdate
    {
        protected int _frame;
        public bool cheap;
        public bool isFlipped;
        public bool oppositeSymmetry;

        public override int frame
        {
            get => _frame;
            set
            {
                _frame = value;
                (graphic as SpriteMap).frame = _frame;
            }
        }

        public BackgroundTile(float xpos, float ypos)
          : base(xpos, ypos)
        {
            shouldbeinupdateloop = false;
            layer = Layer.Background;
            _canBeGrouped = true;
            _isStatic = true;
            _opaque = true;
            if (Level.flipH)
                flipHorizontal = true;
            _placementCost = 1;
        }

        public override void Initialize()
        {
            // if (Level.current is Editor)
            //    cheap = false;
            //else
            DoPositioning();
        }

        public virtual void DoPositioning()
        {
            cheap = true;
            graphic.position = position;
            graphic.scale = scale;
            graphic.center = center;
            graphic.depth = depth;
            graphic.alpha = alpha;
            graphic.angle = angle;
            (graphic as SpriteMap).UpdateFrame();
        }

        public override BinaryClassChunk Serialize()
        {
            BinaryClassChunk binaryClassChunk = base.Serialize();
            binaryClassChunk.AddProperty("frame", _frame);
            if (flipHorizontal)
                binaryClassChunk.AddProperty("f", 1);
            return binaryClassChunk;
        }

        public override bool Deserialize(BinaryClassChunk node)
        {
            base.Deserialize(node);
            _frame = node.GetProperty<int>("frame");
            (graphic as SpriteMap).frame = _frame;
            if (node.GetProperty<int>("f") == 1)
                flipHorizontal = true;
            return true;
        }

        public override DXMLNode LegacySerialize()
        {
            DXMLNode dxmlNode = base.LegacySerialize();
            dxmlNode.Add(new DXMLNode("frame", (graphic as SpriteMap).frame));
            return dxmlNode;
        }

        public override bool LegacyDeserialize(DXMLNode node)
        {
            base.LegacyDeserialize(node);
            DXMLNode dxmlNode = node.Element("frame");
            if (dxmlNode != null)
                (graphic as SpriteMap).frame = Convert.ToInt32(dxmlNode.Value);
            return true;
        }

        public override void Draw()
        {
            //graphic.flipH = flipHorizontal;
            //if (cheap && !Editor.editorDraw)
            //    graphic.UltraCheapStaticDraw(flipHorizontal);
            //else
            //    base.Draw();
            if (graphic.position != position)
            {
                (graphic as SpriteMap).ClearCache();
            }
            graphic.position = position;
            graphic.scale = scale;
            graphic.center = center;
            graphic.depth = depth;
            graphic.alpha = alpha;
            graphic.angle = angle;
            graphic.cheapmaterial = material;
            (graphic as SpriteMap).UpdateFrame();
            graphic.UltraCheapStaticDraw(flipHorizontal);
            //  graphic.Draw() FUCK NORMAL DRAWING I AM CHEAP BASTERD 
        }

        public override ContextMenu GetContextMenu() => null;
    }
}
