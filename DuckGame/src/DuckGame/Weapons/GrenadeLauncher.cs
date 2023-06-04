// Decompiled with JetBrains decompiler
// Type: DuckGame.GrenadeLauncher
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    [EditorGroup("Guns|Explosives")]
    public class GrenadeLauncher : Gun
    {
        public StateBinding _fireAngleState = new StateBinding(nameof(_fireAngle));
        public StateBinding _aimAngleState = new StateBinding(nameof(_aimAngle));
        public StateBinding _aimWaitState = new StateBinding(nameof(_aimWait));
        public StateBinding _aimingState = new StateBinding(nameof(_aiming));
        public StateBinding _cooldownState = new StateBinding(nameof(_cooldown));
        public float _fireAngle;
        public float _aimAngle;
        public float _aimWait;
        public bool _aiming;
        public float _cooldown;

        public override float angle
        {
            get => base.angle + _aimAngle;
            set => _angle = value;
        }

        public GrenadeLauncher(float xval, float yval)
          : base(xval, yval)
        {
            wideBarrel = true;
            ammo = 6;
            _type = "gun";
            graphic = new Sprite("grenadeLauncher");
            center = new Vec2(16f, 16f);
            collisionOffset = new Vec2(-6f, -4f);
            collisionSize = new Vec2(16f, 7f);
            _barrelOffsetTL = new Vec2(28f, 14f);
            _fireSound = "pistol";
            _kickForce = 3f;
            _fireRumble = RumbleIntensity.Light;
            _holdOffset = new Vec2(4f, 0f);
            _ammoType = new ATGrenade();
            _fireSound = "deepMachineGun";
            _bulletColor = Color.White;
            editorTooltip = "Delivers a fun & exciting present to a long distance friend. Hold fire to adjust arc.";
        }

        public override void Update()
        {
            base.Update();
            if (_aiming && _aimWait <= 0 && _fireAngle < 90)
                _fireAngle += 3f;
            if (_aimWait > 0)
                _aimWait -= 0.9f;
            if (_cooldown > 0)
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
            if (_cooldown != 0)
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
            if (_cooldown != 0 || ammo <= 0)
                return;
            _aiming = false;
            Fire();
            _cooldown = 1f;
            angle = 0f;
            _fireAngle = 0f;
        }
    }
}
