// Decompiled with JetBrains decompiler
// Type: DuckGame.GrenadeCannon
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;

namespace DuckGame
{
    [EditorGroup("Guns|Explosives")]
    public class GrenadeCannon : Gun
    {
        public StateBinding _fireAngleState = new StateBinding(nameof(_fireAngle));
        public StateBinding _aimAngleState = new StateBinding(nameof(_aimAngle));
        public StateBinding _aimWaitState = new StateBinding(nameof(_aimWait));
        public StateBinding _aimingState = new StateBinding(nameof(_aiming));
        public StateBinding _cooldownState = new StateBinding(nameof(_cooldown));
        public bool _doLoad;
        public bool _doneLoad;
        public float _timer = 1.2f;
        public float _fireAngle;
        public float _aimAngle;
        public float _aimWait;
        public bool _aiming;
        public float _cooldown;
        private SpriteMap _sprite;

        public override float angle
        {
            get => base.angle + _aimAngle;
            set => _angle = value;
        }

        public GrenadeCannon(float xval, float yval)
          : base(xval, yval)
        {
            ammo = 4;
            _type = "gun";
            _sprite = new SpriteMap("grenadecannon", 26, 12);
            _sprite.AddAnimation("idle4", 0.4f, false, new int[1]);
            _sprite.AddAnimation("load4", 0.4f, false, 1, 2, 3, 4);
            _sprite.AddAnimation("idle3", 0.4f, false, 5);
            _sprite.AddAnimation("load3", 0.4f, false, 6, 7, 8, 9);
            _sprite.AddAnimation("idle2", 0.4f, false, 10);
            _sprite.AddAnimation("load2", 0.4f, false, 11, 12, 13, 14);
            _sprite.AddAnimation("idle1", 0.4f, false, 15);
            _sprite.AddAnimation("load1", 0.4f, false, 16, 17, 18, 19);
            _sprite.AddAnimation("idle0", 0.4f, false, 20);
            _sprite.SetAnimation("idle4");
            graphic = _sprite;
            center = new Vec2(11f, 7f);
            collisionOffset = new Vec2(-6f, -4f);
            collisionSize = new Vec2(16f, 8f);
            _barrelOffsetTL = new Vec2(22f, 6f);
            _laserOffsetTL = new Vec2(22f, 6f);
            _fireSound = "pistol";
            _kickForce = 3f;
            _fireRumble = RumbleIntensity.Light;
            _holdOffset = new Vec2(2f, 0f);
            _ammoType = new ATGrenade();
            _fireSound = "deepMachineGun";
            _bulletColor = Color.White;
            editorTooltip = "An unstable weapon filled with explosions. Spits fire if the trigger is held too long.";
        }

        public override void Update()
        {
            base.Update();
            if (_doLoad && _sprite.finished)
            {
                GrenadePin grenadePin = new GrenadePin(x, y)
                {
                    hSpeed = -offDir * (1.5f + Rando.Float(0.5f)),
                    vSpeed = -2f
                };
                Level.Add(grenadePin);
                SFX.Play("pullPin");
                _doneLoad = true;
                _doLoad = false;
            }
            if (_doneLoad)
                _timer -= 0.01f;
            if (_timer <= 0.0)
            {
                _timer = 1.2f;
                _doneLoad = false;
                _doLoad = false;
                if (isServerForObject)
                {
                    Vec2 vec2 = Offset(barrelOffset);
                    --ammo;
                    Vec2 vec = Maths.AngleToVec(barrelAngle + Rando.Float(-0.1f, 0.1f));
                    for (int index = 0; index < 12; ++index)
                        Level.Add(SmallFire.New(vec2.x, vec2.y, vec.x * Rando.Float(3.5f, 5f) + Rando.Float(-2f, 2f), vec.y * Rando.Float(3.5f, 5f) + Rando.Float(-2f, 2f)));
                    for (int index = 0; index < 6; ++index)
                        Level.Add(SmallSmoke.New(vec2.x + Rando.Float(-2f, 2f), vec2.y + Rando.Float(-2f, 2f)));
                    _sprite.SetAnimation("idle" + Math.Min(ammo, 4).ToString());
                    kick = 1f;
                    _aiming = false;
                    _cooldown = 1f;
                    _fireAngle = 0f;
                    if (owner != null)
                    {
                        this.owner.hSpeed -= vec.x * 4f;
                        this.owner.vSpeed -= vec.y * 4f;
                        if (this.owner is Duck owner && owner.crouch)
                            owner.sliding = true;
                    }
                    else
                    {
                        hSpeed -= vec.x * 4f;
                        vSpeed -= vec.y * 4f;
                    }
                }
            }
            if (_doneLoad && _aiming)
                laserSight = true;
            if (_aiming && _aimWait <= 0.0 && _fireAngle < 90.0)
                _fireAngle += 3f;
            if (_aimWait > 0.0)
                _aimWait -= 0.9f;
            if (_cooldown > 0.0)
                _cooldown -= 0.1f;
            else
                _cooldown = 0f;
            if (owner != null)
            {
                _aimAngle = -Maths.DegToRad(_fireAngle);
                if (offDir < 0)
                    _aimAngle = -_aimAngle;
            }
            else
            {
                _aimWait = 0f;
                _aiming = false;
                _aimAngle = 0f;
                _fireAngle = 0f;
            }
            if (!_raised)
                return;
            _aimAngle = 0f;
        }

        public override void OnPressAction()
        {
            if (!_doneLoad && !_doLoad)
            {
                _sprite.SetAnimation("load" + Math.Min(ammo, 4).ToString());
                _doLoad = true;
            }
            if (!_doneLoad || _cooldown != 0.0)
                return;
            if (ammo > 0)
            {
                _aiming = true;
                _aimWait = 1f;
            }
            else
                SFX.Play("click");
        }

        public override void OnReleaseAction()
        {
            if (!_doneLoad || _cooldown != 0.0 || !_aiming || ammo <= 0)
                return;
            _aiming = false;
            --ammo;
            kick = 1f;
            if (!receivingPress && isServerForObject)
            {
                Vec2 vec2 = Offset(barrelOffset);
                double radians = barrelAngle + Rando.Float(-0.1f, 0.1f);
                CannonGrenade t = new CannonGrenade(vec2.x, vec2.y)
                {
                    _pin = false,
                    _timer = _timer
                };
                Fondle(t);
                Vec2 vec = Maths.AngleToVec((float)radians);
                t.hSpeed = vec.x * 10f;
                t.vSpeed = vec.y * 10f;
                Level.Add(t);
                _timer = 1.2f;
                _doneLoad = false;
                _doLoad = false;
                _sprite.SetAnimation("idle" + Math.Min(ammo, 4).ToString());
            }
            _cooldown = 1f;
            angle = 0f;
            _fireAngle = 0f;
        }

        public override void Fire()
        {
        }
    }
}
