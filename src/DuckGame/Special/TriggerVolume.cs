// Decompiled with JetBrains decompiler
// Type: DuckGame.TriggerVolume
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System.Collections.Generic;

namespace DuckGame
{
    [EditorGroup("Arcade|Targets", EditorItemType.ArcadeNew)]
    [BaggedProperty("canSpawn", false)]
    public class TriggerVolume : MaterialThing, ISequenceItem
    {
        public EditorProperty<int> Order = new EditorProperty<int>(-1, min: -1f, max: 256f, increment: 1f, minSpecial: "RANDOM");
        public EditorProperty<int> Wide = new EditorProperty<int>(16, min: 16f, max: 1024f, increment: 1f);
        public EditorProperty<int> High = new EditorProperty<int>(16, min: 16f, max: 1024f, increment: 1f);
        public EditorProperty<bool> Untouch = new EditorProperty<bool>(false);
        public EditorProperty<bool> Ducks_Only = new EditorProperty<bool>(true);
        public EditorProperty<bool> Is_Goody = new EditorProperty<bool>(false);
        private HashSet<PhysicsObject> _touching = new HashSet<PhysicsObject>();
        private bool _hidden;

        public override void EditorPropertyChanged(object property)
        {
            if (Level.current is Editor)
                return;
            center = new Vec2(8f, 8f);
            collisionSize = new Vec2((int)Wide, (int)High);
            collisionOffset = new Vec2(-((int)Wide / 2), -((int)High / 2));
        }

        public TriggerVolume(float xpos, float ypos)
          : base(xpos, ypos)
        {
            sequence = new SequenceItem(this)
            {
                type = SequenceItemType.Goody
            };
            enablePhysics = false;
            _impactThreshold = 1E-06f;
            graphic = new SpriteMap("challenge/goody", 16, 16);
            center = new Vec2(8f, 8f);
            (graphic as SpriteMap).frame = 3;
            collisionOffset = new Vec2(-4f, -4f);
            collisionSize = new Vec2(8f, 8f);
            _contextMenuFilter.Add("Sequence");
            _editorName = "Trigger Volume";
            editorTooltip = "Pretty much an invisible Goody that you can resize.";
            Untouch._tooltip = "If enabled, the volume only triggers when a Duck leaves it.";
            Ducks_Only._tooltip = "If enabled, only Ducks trigger this volume. Otherwise all physics objects do.";
            Is_Goody._tooltip = "If enabled, this volume will count as a collected Goody when triggered.";
        }

        public override void OnSoftImpact(MaterialThing with, ImpactedFrom from)
        {
            if (_hidden || !(with is PhysicsObject))
                return;
            if (Ducks_Only.value)
            {
                switch (with)
                {
                    case Duck _:
                    case RagdollPart _:
                    case TrappedDuck _:
                        break;
                    default:
                        return;
                }
            }
            if (with.destroyed || Level.current is Editor)
                return;
            if (Untouch.value)
            {
                _touching.Add(with as PhysicsObject);
            }
            else
            {
                if (_touching.Contains(with as PhysicsObject))
                    return;
                _sequence.Finished();
                if (ChallengeLevel.running && Is_Goody.value)
                    ++ChallengeLevel.goodiesGot;
                _touching.Add(with as PhysicsObject);
            }
        }

        public override void Update()
        {
            bool flag = false;
            foreach (Thing thing in _touching)
            {
                if (!Collision.Rect(rectangle, thing.rectangle))
                {
                    if (Untouch.value)
                    {
                        _sequence.Finished();
                        if (ChallengeLevel.running && Is_Goody.value)
                            ++ChallengeLevel.goodiesGot;
                    }
                    flag = true;
                }
            }
            if (flag)
                _touching.Clear();
            base.Update();
        }

        public override void Initialize()
        {
            if (!(Level.current is Editor))
            {
                sequence.order = Order.value;
                if (sequence.order == -1)
                    sequence.order = Rando.Int(256);
                sequence.waitTillOrder = true;
                center = new Vec2(8f, 8f);
                collisionSize = new Vec2((int)Wide, (int)High);
                collisionOffset = new Vec2(-((int)Wide / 2), -((int)High / 2));
            }
            if (!(Level.current is Editor) && sequence.waitTillOrder && sequence.order != 0)
            {
                visible = false;
                _hidden = true;
            }
            base.Initialize();
        }

        public override void OnSequenceActivate()
        {
            if (sequence.waitTillOrder)
            {
                visible = true;
                _hidden = false;
            }
            base.OnSequenceActivate();
        }

        public override void Draw()
        {
            if (!(Level.current is Editor))
                return;
            base.Draw();
            if (Editor.editorDraw)
                return;
            float num1 = Wide.value;
            float num2 = High.value;
            Graphics.DrawRect(position + new Vec2((float)(-num1 / 2.0), (float)(-num2 / 2.0)), position + new Vec2(num1 / 2f, num2 / 2f), Colors.DGGreen * 0.5f, (Depth)1f, false);
        }
    }
}
