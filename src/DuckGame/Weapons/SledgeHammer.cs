// Decompiled with JetBrains decompiler
// Type: DuckGame.SledgeHammer
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;

namespace DuckGame
{
    [EditorGroup("Guns|Melee")]
    public class SledgeHammer : Gun
    {
        public StateBinding _swingBinding = new StateBinding(nameof(_swing));
        private SpriteMap _sprite;
        private SpriteMap _sledgeSwing;
        private Vec2 _offset;
        private float _swing;
        private float _swingLast;
        private float _swingVelocity;
        private float _swingForce;
        private bool _pressed;
        private float _lastSpeed;
        private int _lastDir;
        private float _fullSwing;
        private float _sparkWait;
        private bool _swung;
        private bool _drawOnce;
        private bool _held;
        private PhysicsObject _lastOwner;
        private float _hPull;

        public SledgeHammer(float xval, float yval)
          : base(xval, yval)
        {
            this.ammo = 4;
            this._ammoType = new ATLaser();
            this._ammoType.range = 170f;
            this._ammoType.accuracy = 0.8f;
            this._type = "gun";
            this._sprite = new SpriteMap("sledgeHammer", 32, 32);
            this._sledgeSwing = new SpriteMap("sledgeSwing", 32, 32);
            this._sledgeSwing.AddAnimation("swing", 0.8f, false, 0, 1, 2, 3, 4, 5);
            this._sledgeSwing.currentAnimation = "swing";
            this._sledgeSwing.speed = 0f;
            this._sledgeSwing.center = new Vec2(16f, 16f);
            this.graphic = _sprite;
            this.center = new Vec2(16f, 14f);
            this.collisionOffset = new Vec2(-2f, 0f);
            this.collisionSize = new Vec2(4f, 18f);
            this._barrelOffsetTL = new Vec2(16f, 28f);
            this._fireSound = "smg";
            this._fullAuto = true;
            this._fireWait = 1f;
            this._kickForce = 3f;
            this.weight = 9f;
            this._dontCrush = false;
            this.collideSounds.Add("rockHitGround2");
            this.holsterAngle = 180f;
            this.holsterOffset = new Vec2(11f, 0f);
            this.editorTooltip = "For big nails.";
        }

        public override void OnSoftImpact(MaterialThing with, ImpactedFrom from)
        {
            if (!(with is IPlatform))
                return;
            for (int index = 0; index < 4; ++index)
                Level.Add(Spark.New(this.barrelPosition.x + Rando.Float(-6f, 6f), this.barrelPosition.y + Rando.Float(-3f, 3f), -MaterialThing.ImpactVector(from)));
        }

        public override void CheckIfHoldObstructed()
        {
            if (!(this.owner is Duck owner))
                return;
            owner.holdObstructed = false;
        }

        public override void Initialize() => base.Initialize();

        public override void ReturnToWorld()
        {
            this.collisionOffset = new Vec2(-2f, 0f);
            this.collisionSize = new Vec2(4f, 18f);
            this._sprite.frame = 0;
            this._swing = 0f;
            this._swingForce = 0f;
            this._pressed = false;
            this._swung = false;
            this._fullSwing = 0f;
            this._swingVelocity = 0f;
        }

        public override void Update()
        {
            if (this._lastOwner != null && this.owner == null)
            {
                this._lastOwner.frictionMod = 0f;
                this._lastOwner = null;
                this._swing = 0f;
                this._swingVelocity = 0f;
            }
            if (this.duck != null)
            {
                if (this.duck.ragdoll != null)
                {
                    this.holsterAngle = 0f;
                    this.holsterOffset = new Vec2(0f, 0f);
                    this.center = new Vec2(16f, 22f);
                    this.collisionOffset = new Vec2(-6f, 6f);
                    this.collisionSize = new Vec2(12f, 12f);
                    return;
                }
                this.holsterAngle = 180f;
                this.center = new Vec2(16f, 14f);
                this.graphic.center = this.center;
                if (this.duck.sliding)
                    this.holsterOffset = new Vec2(4f, 8f);
                else
                    this.holsterOffset = new Vec2(11f, 0f);
            }
            this.collisionOffset = new Vec2(-2f, 0f);
            this.collisionSize = new Vec2(4f, 18f);
            if (_swing > 0.0)
            {
                this.collisionOffset = new Vec2(-9999f, 0f);
                this.collisionSize = new Vec2(4f, 18f);
            }
            this._swingVelocity = Maths.LerpTowards(this._swingVelocity, this._swingForce, 0.1f);
            Duck owner = this.owner as Duck;
            if (this.isServerForObject)
            {
                this._swing += this._swingVelocity;
                float num1 = this._swing - this._swingLast;
                this._swingLast = this._swing;
                if (_swing > 1.0)
                    this._swing = 1f;
                if (_swing < 0.0)
                    this._swing = 0f;
                this._sprite.flipH = false;
                this._sprite.flipV = false;
                if (owner != null && this.held)
                {
                    float hSpeed = owner.hSpeed;
                    this._hPull = Maths.LerpTowards(this._hPull, owner.hSpeed, 0.15f);
                    if ((double)Math.Abs(owner.hSpeed) < 0.100000001490116)
                        this._hPull = 0f;
                    float num2 = Math.Abs(this._hPull) / 2.5f;
                    if ((double)num2 > 1.0)
                        num2 = 1f;
                    this.weight = (float)(8.0 - (double)num2 * 3.0);
                    if ((double)this.weight <= 5.0)
                        this.weight = 5.1f;
                    float num3 = Math.Abs(owner.hSpeed - this._hPull);
                    owner.frictionMod = 0f;
                    if ((double)owner.hSpeed > 0.0 && _hPull > (double)owner.hSpeed)
                        owner.frictionMod = (float)(-(double)num3 * 1.79999995231628);
                    if ((double)owner.hSpeed < 0.0 && _hPull < (double)owner.hSpeed)
                        owner.frictionMod = (float)(-(double)num3 * 1.79999995231628);
                    this._lastDir = owner.offDir;
                    this._lastSpeed = hSpeed;
                    if (_swing != 0.0 && (double)num1 > 0.0)
                    {
                        owner.hSpeed += owner.offDir * (num1 * 3f) * this.weightMultiplier;
                        owner.vSpeed -= num1 * 2f * this.weightMultiplier;
                    }
                }
            }
            if (_sparkWait > 0.0)
                this._sparkWait -= 0.1f;
            else
                this._sparkWait = 0f;
            if (owner != null && this.held && _sparkWait == 0.0 && _swing == 0.0 && owner.Held(this, true))
            {
                if (owner.grounded && owner.offDir > 0 && (double)owner.hSpeed > 1.0)
                {
                    this._sparkWait = 0.25f;
                    Level.Add(Spark.New(this.x - 22f, this.y + 6f, new Vec2(0f, 0.5f)));
                }
                else if (owner.grounded && owner.offDir < 0 && (double)owner.hSpeed < -1.0)
                {
                    this._sparkWait = 0.25f;
                    Level.Add(Spark.New(this.x + 22f, this.y + 6f, new Vec2(0f, 0.5f)));
                }
            }
            if (_swing < 0.5)
            {
                float num = this._swing * 2f;
                this._sprite.imageIndex = (int)((double)num * 10.0);
                this._sprite.angle = (float)(1.20000004768372 - (double)num * 1.5);
                this._sprite.yscale = (float)(1.0 - (double)num * 0.100000001490116);
            }
            else if (_swing >= 0.5)
            {
                float num = (float)((_swing - 0.5) * 2.0);
                this._sprite.imageIndex = 10 - (int)((double)num * 10.0);
                this._sprite.angle = (float)(-0.300000011920929 - (double)num * 1.5);
                this._sprite.yscale = (float)(1.0 - (1.0 - (double)num) * 0.100000001490116);
                this._fullSwing += 0.16f;
                if (!this._swung)
                {
                    this._swung = true;
                    if (this.duck != null && this.isServerForObject)
                        Level.Add(new ForceWave(this.x + offDir * 4f + this.owner.hSpeed, this.y + 8f, offDir, 0.15f, 4f + Math.Abs(this.owner.hSpeed), this.owner.vSpeed, this.duck));
                }
            }
            if (_swing == 1.0)
                this._pressed = false;
            if (_swing == 1.0 && !this._pressed && _fullSwing > 1.0)
            {
                this._swingForce = -0.08f;
                this._fullSwing = 0f;
            }
            if (this._sledgeSwing.finished)
                this._sledgeSwing.speed = 0f;
            this._lastOwner = this.owner as PhysicsObject;
            if (this.duck != null && this.held)
            {
                if ((this.duck.Held(this, true) ? this.duck.action : this.triggerAction) && !this._held && _swing == 0.0)
                {
                    RumbleManager.AddRumbleEvent(this.duck.profile, new RumbleEvent(RumbleIntensity.Kick, RumbleDuration.Pulse, RumbleFalloff.Short));
                    this._fullSwing = 0f;
                    owner._disarmDisable = 30;
                    owner.crippleTimer = 1f;
                    this._sledgeSwing.speed = 1f;
                    this._sledgeSwing.frame = 0;
                    this._swingForce = 0.6f;
                    this._pressed = true;
                    this._swung = false;
                    this._held = true;
                }
                if (!this.duck.action)
                {
                    this._pressed = false;
                    this._held = false;
                }
            }
            this.handOffset = new Vec2(this._swing * 3f, (float)(0.0 - _swing * 4.0));
            this.handAngle = (float)(1.39999997615814 + ((double)this._sprite.angle * 0.5 - 1.0));
            if (owner != null && owner.offDir < 0)
            {
                this._sprite.angle = -this._sprite.angle;
                this.handAngle = -this.handAngle;
            }
            base.Update();
        }

        public override void Draw()
        {
            if (this.duck != null && this.duck.ragdoll != null)
                base.Draw();
            else if (this.owner != null && this._drawOnce)
            {
                this._offset = new Vec2((float)(offDir * -6.0 + _swing * 5.0 * offDir), (float)(_swing * 5.0 - 3.0));
                this.graphic.position = this.position + this._offset;
                this.graphic.depth = this.depth;
                this.graphic.Draw();
                Duck owner = this.owner as Duck;
                if ((double)this._sledgeSwing.speed <= 0.0)
                    return;
                if (owner != null)
                    this._sledgeSwing.flipH = owner.offDir <= 0;
                this._sledgeSwing.position = this.position;
                this._sledgeSwing.depth = this.depth + 1;
                this._sledgeSwing.Draw();
            }
            else
            {
                base.Draw();
                this._drawOnce = true;
            }
        }

        public override void OnPressAction()
        {
        }

        public override void OnReleaseAction()
        {
        }

        public override void Fire()
        {
        }
    }
}
