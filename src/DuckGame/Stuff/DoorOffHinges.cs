// Decompiled with JetBrains decompiler
// Type: DuckGame.DoorOffHinges
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class DoorOffHinges : PhysicsObject
    {
        public StateBinding _throwSpinBinding = new StateBinding(nameof(_throwSpin));
        public StateBinding _secondaryBinding = new StateBinding(nameof(_secondaryFrame));
        public bool _secondaryFrame;
        public bool _wasSecondaryFrame;
        public float _throwSpin;
        private bool sounded;

        public DoorOffHinges(float xpos, float ypos, bool secondaryFrame)
          : base(xpos, ypos)
        {
            this._secondaryFrame = secondaryFrame;
            this._collisionSize = new Vec2(8f, 8f);
            this._collisionOffset = new Vec2(-4f, -6f);
            this.center = new Vec2(16f, 16f);
            this.collideSounds.Add("rockHitGround");
            this.weight = 2f;
        }

        public override void Initialize()
        {
            this.RefreshSprite();
            base.Initialize();
        }

        public void MakeEffects()
        {
            if (this.sounded)
                return;
            Level.Add(SmallSmoke.New(this.x, this.y + 2f));
            Level.Add(SmallSmoke.New(this.x, this.y - 16f));
            SFX.Play("doorBreak");
            for (int index = 0; index < 8; ++index)
            {
                WoodDebris woodDebris = WoodDebris.New(this.x - 8f + Rando.Float(16f), this.y - 8f + Rando.Float(16f));
                woodDebris.hSpeed = ((double)Rando.Float(1f) > 0.5 ? 1f : -1f) * Rando.Float(3f);
                woodDebris.vSpeed = -Rando.Float(1f);
                Level.Add(woodDebris);
            }
            this.sounded = true;
        }

        private void RefreshSprite()
        {
            this.graphic = new SpriteMap(this._secondaryFrame ? "flimsyDoorDamaged" : "doorFucked", 32, 32);
            this._wasSecondaryFrame = this._secondaryFrame;
        }

        public override void Update()
        {
            if (this._secondaryFrame != this._wasSecondaryFrame)
                this.RefreshSprite();
            if (Network.isActive && !this.sounded && this.visible)
                this.MakeEffects();
            this.angleDegrees = this._throwSpin;
            this.center = new Vec2(16f, 16f);
            this._throwSpin %= 360f;
            this._throwSpin = this.offDir <= 0 ? Lerp.Float(this._throwSpin, -90f, 12f) : Lerp.Float(this._throwSpin, 90f, 12f);
            base.Update();
        }
    }
}
