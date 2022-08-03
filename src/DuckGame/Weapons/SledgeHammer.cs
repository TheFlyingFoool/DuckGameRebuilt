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
            ammo = 4;
            _ammoType = new ATLaser();
            _ammoType.range = 170f;
            _ammoType.accuracy = 0.8f;
            _type = "gun";
            _sprite = new SpriteMap("sledgeHammer", 32, 32);
            _sledgeSwing = new SpriteMap("sledgeSwing", 32, 32);
            _sledgeSwing.AddAnimation("swing", 0.8f, false, 0, 1, 2, 3, 4, 5);
            _sledgeSwing.currentAnimation = "swing";
            _sledgeSwing.speed = 0f;
            _sledgeSwing.center = new Vec2(16f, 16f);
            graphic = _sprite;
            center = new Vec2(16f, 14f);
            collisionOffset = new Vec2(-2f, 0f);
            collisionSize = new Vec2(4f, 18f);
            _barrelOffsetTL = new Vec2(16f, 28f);
            _fireSound = "smg";
            _fullAuto = true;
            _fireWait = 1f;
            _kickForce = 3f;
            weight = 9f;
            _dontCrush = false;
            collideSounds.Add("rockHitGround2");
            holsterAngle = 180f;
            holsterOffset = new Vec2(11f, 0f);
            editorTooltip = "For big nails.";
        }

        public override void OnSoftImpact(MaterialThing with, ImpactedFrom from)
        {
            if (!(with is IPlatform))
                return;
            for (int index = 0; index < 4; ++index)
                Level.Add(Spark.New(barrelPosition.x + Rando.Float(-6f, 6f), barrelPosition.y + Rando.Float(-3f, 3f), -MaterialThing.ImpactVector(from)));
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
            collisionOffset = new Vec2(-2f, 0f);
            collisionSize = new Vec2(4f, 18f);
            _sprite.frame = 0;
            _swing = 0f;
            _swingForce = 0f;
            _pressed = false;
            _swung = false;
            _fullSwing = 0f;
            _swingVelocity = 0f;
        }

        public override void Update()
        {
            if (_lastOwner != null && this.owner == null)
            {
                _lastOwner.frictionMod = 0f;
                _lastOwner = null;
                _swing = 0f;
                _swingVelocity = 0f;
            }
            if (duck != null)
            {
                if (duck.ragdoll != null)
                {
                    holsterAngle = 0f;
                    holsterOffset = new Vec2(0f, 0f);
                    center = new Vec2(16f, 22f);
                    collisionOffset = new Vec2(-6f, 6f);
                    collisionSize = new Vec2(12f, 12f);
                    return;
                }
                holsterAngle = 180f;
                center = new Vec2(16f, 14f);
                graphic.center = center;
                if (duck.sliding)
                    holsterOffset = new Vec2(4f, 8f);
                else
                    holsterOffset = new Vec2(11f, 0f);
            }
            collisionOffset = new Vec2(-2f, 0f);
            collisionSize = new Vec2(4f, 18f);
            if (_swing > 0.0)
            {
                collisionOffset = new Vec2(-9999f, 0f);
                collisionSize = new Vec2(4f, 18f);
            }
            _swingVelocity = Maths.LerpTowards(_swingVelocity, _swingForce, 0.1f);
            Duck owner = this.owner as Duck;
            if (isServerForObject)
            {
                _swing += _swingVelocity;
                float num1 = _swing - _swingLast;
                _swingLast = _swing;
                if (_swing > 1.0)
                    _swing = 1f;
                if (_swing < 0.0)
                    _swing = 0f;
                _sprite.flipH = false;
                _sprite.flipV = false;
                if (owner != null && held)
                {
                    float hSpeed = owner.hSpeed;
                    _hPull = Maths.LerpTowards(_hPull, owner.hSpeed, 0.15f);
                    if (Math.Abs(owner.hSpeed) < 0.100000001490116)
                        _hPull = 0f;
                    float num2 = Math.Abs(_hPull) / 2.5f;
                    if (num2 > 1.0)
                        num2 = 1f;
                    weight = (float)(8.0 - num2 * 3.0);
                    if (weight <= 5.0)
                        weight = 5.1f;
                    float num3 = Math.Abs(owner.hSpeed - _hPull);
                    owner.frictionMod = 0f;
                    if (owner.hSpeed > 0.0 && _hPull > owner.hSpeed)
                        owner.frictionMod = (float)(-num3 * 1.79999995231628);
                    if (owner.hSpeed < 0.0 && _hPull < owner.hSpeed)
                        owner.frictionMod = (float)(-num3 * 1.79999995231628);
                    _lastDir = owner.offDir;
                    _lastSpeed = hSpeed;
                    if (_swing != 0.0 && num1 > 0.0)
                    {
                        owner.hSpeed += owner.offDir * (num1 * 3f) * weightMultiplier;
                        owner.vSpeed -= num1 * 2f * weightMultiplier;
                    }
                }
            }
            if (_sparkWait > 0.0)
                _sparkWait -= 0.1f;
            else
                _sparkWait = 0f;
            if (owner != null && held && _sparkWait == 0.0 && _swing == 0.0 && owner.Held(this, true))
            {
                if (owner.grounded && owner.offDir > 0 && owner.hSpeed > 1.0)
                {
                    _sparkWait = 0.25f;
                    Level.Add(Spark.New(x - 22f, y + 6f, new Vec2(0f, 0.5f)));
                }
                else if (owner.grounded && owner.offDir < 0 && owner.hSpeed < -1.0)
                {
                    _sparkWait = 0.25f;
                    Level.Add(Spark.New(x + 22f, y + 6f, new Vec2(0f, 0.5f)));
                }
            }
            if (_swing < 0.5)
            {
                float num = _swing * 2f;
                _sprite.imageIndex = (int)(num * 10.0);
                _sprite.angle = (float)(1.20000004768372 - num * 1.5);
                _sprite.yscale = (float)(1.0 - num * 0.100000001490116);
            }
            else if (_swing >= 0.5)
            {
                float num = (float)((_swing - 0.5) * 2.0);
                _sprite.imageIndex = 10 - (int)(num * 10.0);
                _sprite.angle = (float)(-0.300000011920929 - num * 1.5);
                _sprite.yscale = (float)(1.0 - (1.0 - num) * 0.100000001490116);
                _fullSwing += 0.16f;
                if (!_swung)
                {
                    _swung = true;
                    if (duck != null && isServerForObject)
                        Level.Add(new ForceWave(x + offDir * 4f + this.owner.hSpeed, y + 8f, offDir, 0.15f, 4f + Math.Abs(this.owner.hSpeed), this.owner.vSpeed, duck));
                }
            }
            if (_swing == 1.0)
                _pressed = false;
            if (_swing == 1.0 && !_pressed && _fullSwing > 1.0)
            {
                _swingForce = -0.08f;
                _fullSwing = 0f;
            }
            if (_sledgeSwing.finished)
                _sledgeSwing.speed = 0f;
            _lastOwner = this.owner as PhysicsObject;
            if (duck != null && held)
            {
                if ((duck.Held(this, true) ? duck.action : triggerAction) && !_held && _swing == 0.0)
                {
                    RumbleManager.AddRumbleEvent(duck.profile, new RumbleEvent(RumbleIntensity.Kick, RumbleDuration.Pulse, RumbleFalloff.Short));
                    _fullSwing = 0f;
                    owner._disarmDisable = 30;
                    owner.crippleTimer = 1f;
                    _sledgeSwing.speed = 1f;
                    _sledgeSwing.frame = 0;
                    _swingForce = 0.6f;
                    _pressed = true;
                    _swung = false;
                    _held = true;
                }
                if (!duck.action)
                {
                    _pressed = false;
                    _held = false;
                }
            }
            handOffset = new Vec2(_swing * 3f, (float)(0.0 - _swing * 4.0));
            handAngle = (float)(1.39999997615814 + (_sprite.angle * 0.5 - 1.0));
            if (owner != null && owner.offDir < 0)
            {
                _sprite.angle = -_sprite.angle;
                handAngle = -handAngle;
            }
            base.Update();
        }

        public override void Draw()
        {
            if (duck != null && duck.ragdoll != null)
                base.Draw();
            else if (owner != null && _drawOnce)
            {
                _offset = new Vec2((float)(offDir * -6.0 + _swing * 5.0 * offDir), (float)(_swing * 5.0 - 3.0));
                graphic.position = position + _offset;
                graphic.depth = depth;
                graphic.Draw();
                Duck owner = this.owner as Duck;
                if (_sledgeSwing.speed <= 0.0)
                    return;
                if (owner != null)
                    _sledgeSwing.flipH = owner.offDir <= 0;
                _sledgeSwing.position = position;
                _sledgeSwing.depth = depth + 1;
                _sledgeSwing.Draw();
            }
            else
            {
                base.Draw();
                _drawOnce = true;
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
