// Decompiled with JetBrains decompiler
// Type: DuckGame.WireButton
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Linq;

namespace DuckGame
{
    [EditorGroup("Stuff|Wires")]
    [BaggedProperty("isInDemo", true)]
    public class WireButton : Block, IWirePeripheral
    {
        public EditorProperty<bool> offSignal = new EditorProperty<bool>(false);
        public EditorProperty<float> holdTime = new EditorProperty<float>(0.0f);
        public EditorProperty<bool> releaseOnly = new EditorProperty<bool>(false);
        public EditorProperty<bool> invert = new EditorProperty<bool>(false);
        public EditorProperty<int> orientation = new EditorProperty<int>(0, max: 3f, increment: 1f);
        private WireButtonTop _top;
        private SpriteMap _sprite;
        private bool _initializedFrame;
        private float releaseHold;
        private PhysicsObject prevO;

        public WireButton(float xpos, float ypos)
          : base(xpos, ypos)
        {
            this._sprite = new SpriteMap("wireButton", 16, 19);
            this.graphic = _sprite;
            this.center = new Vec2(8f, 11f);
            this.collisionOffset = new Vec2(-8f, -8f);
            this.collisionSize = new Vec2(16f, 16f);
            this.depth = - 0.5f;
            this._editorName = "Wire Button";
            this.editorTooltip = "Stepping on a Button triggers the behavior of connected objects.";
            this.offSignal.name = "Hold Signal";
            this.offSignal._tooltip = "If true, the button continuously send a signal through the wire while pressed.";
            this.holdTime.name = "Hold Time";
            this.holdTime._tooltip = "How long the signal will be held after releasing the button.";
            this.releaseOnly.name = "Release";
            this.releaseOnly._tooltip = "If true, the button will send a signal only when released.";
            this.invert._tooltip = "If true, the button will send signals as long as it's not pressed.";
            this.thickness = 4f;
            this.physicsMaterial = PhysicsMaterial.Metal;
            this.layer = Layer.Foreground;
        }

        public override void TabRotate()
        {
            this.orientation = (EditorProperty<int>)((int)this.orientation + 1);
            if ((int)this.orientation <= 3)
                return;
            this.orientation = (EditorProperty<int>)0;
        }

        public override void Initialize()
        {
            if (this.flipHorizontal)
            {
                if (this.orientation.value == 1)
                    this.orientation.value = 3;
                else if (this.orientation.value == 3)
                    this.orientation.value = 1;
            }
            this.angleDegrees = orientation.value * 90f;
            if (!(Level.current is Editor))
            {
                if (this.orientation.value == 0)
                    this._top = new WireButtonTop(this.x, this.y - 9f, this, this.orientation.value);
                else if (this.orientation.value == 1)
                    this._top = new WireButtonTop(this.x + 9f, this.y, this, this.orientation.value);
                else if (this.orientation.value == 2)
                    this._top = new WireButtonTop(this.x, this.y + 9f, this, this.orientation.value);
                else if (this.orientation.value == 3)
                    this._top = new WireButtonTop(this.x - 9f, this.y, this, this.orientation.value);
                Level.Add(_top);
            }
            base.Initialize();
        }

        public override void Terminate()
        {
            Level.Remove(_top);
            base.Terminate();
        }

        public void Pulse(int type, WireTileset wire)
        {
        }

        public void ButtonPressed(PhysicsObject t)
        {
            if (this._sprite.frame == 0)
            {
                SFX.Play("click");
                this._sprite.frame = 1;
                if (this.invert.value)
                {
                    if (!this.releaseOnly.value && t.isServerForObject)
                        Level.CheckRect<WireTileset>(this.topLeft + new Vec2(2f, 2f), this.bottomRight + new Vec2(-2f, -2f))?.Emit(type: (this.offSignal.value ? 2 : 3));
                }
                else if (!this.releaseOnly.value && t.isServerForObject)
                    Level.CheckRect<WireTileset>(this.topLeft + new Vec2(2f, 2f), this.bottomRight + new Vec2(-2f, -2f))?.Emit(type: (this.offSignal.value ? 1 : 0));
            }
            this.prevO = t;
        }

        public override void Update()
        {
            if (!this._initializedFrame)
            {
                if (Level.CheckRectAll<PhysicsObject>(this._top.topLeft, this._top.bottomRight).FirstOrDefault<PhysicsObject>(x => !(x is TeamHat)) != null)
                    this._sprite.frame = 1;
                this._initializedFrame = true;
            }
            if (this.invert.value)
            {
                if (this._sprite.frame == 0)
                    Level.CheckRect<WireTileset>(this.topLeft + new Vec2(2f, 2f), this.bottomRight + new Vec2(-2f, -2f))?.Emit(type: 1);
                if (this._sprite.frame == 1)
                {
                    PhysicsObject physicsObject = Level.CheckRectAll<PhysicsObject>(this._top.topLeft, this._top.bottomRight).FirstOrDefault<PhysicsObject>(x => !(x is TeamHat));
                    if (physicsObject == null)
                    {
                        SFX.Play("click");
                        this._sprite.frame = 0;
                    }
                    this.prevO = physicsObject;
                }
            }
            else if (this._sprite.frame == 1)
            {
                PhysicsObject physicsObject = Level.CheckRectAll<PhysicsObject>(this._top.topLeft, this._top.bottomRight).FirstOrDefault<PhysicsObject>(x => !(x is TeamHat));
                if (physicsObject == null)
                {
                    this.releaseHold += Maths.IncFrameTimer();
                    if (releaseHold > (double)this.holdTime.value)
                    {
                        SFX.Play("click");
                        this._sprite.frame = 0;
                        if ((this.offSignal.value || this.releaseOnly.value) && (this.prevO == null || this.prevO.isServerForObject))
                            Level.CheckRect<WireTileset>(this.topLeft + new Vec2(2f, 2f), this.bottomRight + new Vec2(-2f, -2f))?.Emit(type: (this.releaseOnly.value ? 0 : 2));
                    }
                }
                this.prevO = physicsObject;
            }
            else
                this.releaseHold = 0.0f;
            base.Update();
        }

        public override void Draw()
        {
            if (Level.current is Editor)
            {
                this.angleDegrees = orientation.value * 90f;
                if (this.flipHorizontal)
                    this.angleDegrees -= 180f;
            }
            else
                this.angleDegrees = orientation.value * 90f;
            base.Draw();
        }
    }
}
