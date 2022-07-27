// Decompiled with JetBrains decompiler
// Type: DuckGame.TriggerVolume
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
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
            this.center = new Vec2(8f, 8f);
            this.collisionSize = new Vec2((int)this.Wide, (int)this.High);
            this.collisionOffset = new Vec2(-((int)this.Wide / 2), -((int)this.High / 2));
        }

        public TriggerVolume(float xpos, float ypos)
          : base(xpos, ypos)
        {
            this.sequence = new SequenceItem(this)
            {
                type = SequenceItemType.Goody
            };
            this.enablePhysics = false;
            this._impactThreshold = 1E-06f;
            this.graphic = new SpriteMap("challenge/goody", 16, 16);
            this.center = new Vec2(8f, 8f);
            (this.graphic as SpriteMap).frame = 3;
            this.collisionOffset = new Vec2(-4f, -4f);
            this.collisionSize = new Vec2(8f, 8f);
            this._contextMenuFilter.Add("Sequence");
            this._editorName = "Trigger Volume";
            this.editorTooltip = "Pretty much an invisible Goody that you can resize.";
            this.Untouch._tooltip = "If enabled, the volume only triggers when a Duck leaves it.";
            this.Ducks_Only._tooltip = "If enabled, only Ducks trigger this volume. Otherwise all physics objects do.";
            this.Is_Goody._tooltip = "If enabled, this volume will count as a collected Goody when triggered.";
        }

        public override void OnSoftImpact(MaterialThing with, ImpactedFrom from)
        {
            if (this._hidden || !(with is PhysicsObject))
                return;
            if (this.Ducks_Only.value)
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
            if (this.Untouch.value)
            {
                this._touching.Add(with as PhysicsObject);
            }
            else
            {
                if (this._touching.Contains(with as PhysicsObject))
                    return;
                this._sequence.Finished();
                if (ChallengeLevel.running && this.Is_Goody.value)
                    ++ChallengeLevel.goodiesGot;
                this._touching.Add(with as PhysicsObject);
            }
        }

        public override void Update()
        {
            bool flag = false;
            foreach (Thing thing in this._touching)
            {
                if (!Collision.Rect(this.rectangle, thing.rectangle))
                {
                    if (this.Untouch.value)
                    {
                        this._sequence.Finished();
                        if (ChallengeLevel.running && this.Is_Goody.value)
                            ++ChallengeLevel.goodiesGot;
                    }
                    flag = true;
                }
            }
            if (flag)
                this._touching.Clear();
            base.Update();
        }

        public override void Initialize()
        {
            if (!(Level.current is Editor))
            {
                this.sequence.order = this.Order.value;
                if (this.sequence.order == -1)
                    this.sequence.order = Rando.Int(256);
                this.sequence.waitTillOrder = true;
                this.center = new Vec2(8f, 8f);
                this.collisionSize = new Vec2((int)this.Wide, (int)this.High);
                this.collisionOffset = new Vec2(-((int)this.Wide / 2), -((int)this.High / 2));
            }
            if (!(Level.current is Editor) && this.sequence.waitTillOrder && this.sequence.order != 0)
            {
                this.visible = false;
                this._hidden = true;
            }
            base.Initialize();
        }

        public override void OnSequenceActivate()
        {
            if (this.sequence.waitTillOrder)
            {
                this.visible = true;
                this._hidden = false;
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
            Graphics.DrawRect(this.position + new Vec2((float)(-(double)num1 / 2.0), (float)(-(double)num2 / 2.0)), this.position + new Vec2(num1 / 2f, num2 / 2f), Colors.DGGreen * 0.5f, (Depth)1f, false);
        }
    }
}
