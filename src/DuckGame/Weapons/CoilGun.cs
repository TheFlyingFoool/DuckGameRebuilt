// Decompiled with JetBrains decompiler
// Type: DuckGame.CoilGun
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    [EditorGroup("Guns|Lasers", EditorItemType.PowerUser)]
    [BaggedProperty("canSpawn", false)]
    [BaggedProperty("isOnlineCapable", false)]
    public class CoilGun : Gun
    {
        public StateBinding _laserStateBinding = new CoilGunFlagBinding();
        public StateBinding _animationIndexBinding = new StateBinding(nameof(netAnimationIndex), 4);
        public StateBinding _frameBinding = new StateBinding(nameof(spriteFrame));
        public bool doBlast;
        private bool _lastDoBlast;
        private float _charge;
        public bool _charging;
        public bool _fired;
        private SpriteMap _chargeAnim;
        private Sound _chargeSound;
        private Sound _chargeSoundShort;
        private Sound _unchargeSound;
        private Sound _unchargeSoundShort;
        private int _framesSinceBlast;

        private byte netAnimationIndex
        {
            get => _chargeAnim == null ? (byte)0 : (byte)_chargeAnim.animationIndex;
            set
            {
                if (_chargeAnim == null || _chargeAnim.animationIndex == value)
                    return;
                _chargeAnim.animationIndex = value;
            }
        }

        public byte spriteFrame
        {
            get => _chargeAnim == null ? (byte)0 : (byte)_chargeAnim._frame;
            set
            {
                if (_chargeAnim == null)
                    return;
                _chargeAnim._frame = value;
            }
        }

        public CoilGun(float xval, float yval)
          : base(xval, yval)
        {
            ammo = 30;
            _type = "gun";
            center = new Vec2(16f, 16f);
            collisionOffset = new Vec2(-11f, -8f);
            collisionSize = new Vec2(22f, 12f);
            _barrelOffsetTL = new Vec2(25f, 13f);
            _fireSound = "";
            _fullAuto = false;
            _fireWait = 1f;
            _kickForce = 1f;
            _fireRumble = RumbleIntensity.Kick;
            _holdOffset = new Vec2(3f, 1f);
            _editorName = "Death Laser";
            _chargeAnim = new SpriteMap("coilGun", 32, 32);
            _chargeAnim.AddAnimation("idle", 1f, true, new int[1]);
            _chargeAnim.AddAnimation("charge", 0.38f, false, 1, 2, 3, 0, 1, 2, 3, 4, 5, 6, 7, 4, 5, 6, 7);
            _chargeAnim.AddAnimation("charged", 1f, true, 8, 9, 10, 11);
            _chargeAnim.AddAnimation("uncharge", 1.2f, false, 7, 6, 5, 4, 7, 6, 5, 4, 3, 2, 1, 0, 3, 2, 1, 0);
            _chargeAnim.AddAnimation("drain", 2f, false, 7, 6, 5, 4, 7, 6, 5, 4, 3, 2, 1, 0, 3, 2, 1, 0);
            _chargeAnim.SetAnimation("idle");
            graphic = _chargeAnim;
        }

        public override void Initialize()
        {
            _chargeSound = SFX.Get("laserCharge", 0f);
            _chargeSoundShort = SFX.Get("laserChargeShort", 0f);
            _unchargeSound = SFX.Get("laserUncharge", 0f);
            _unchargeSoundShort = SFX.Get("laserUnchargeShort", 0f);
        }

        public override void Update()
        {
            base.Update();
            if (_charge > 0.0)
                _charge -= 0.1f;
            else
                _charge = 0f;
            if (_chargeAnim.currentAnimation == "uncharge" && _chargeAnim.finished)
                _chargeAnim.SetAnimation("idle");
            if (Network.isActive && doBlast && !_lastDoBlast || _chargeAnim.currentAnimation == "charge" && _chargeAnim.finished && isServerForObject)
                _chargeAnim.SetAnimation("charged");
            if (doBlast && isServerForObject)
            {
                ++_framesSinceBlast;
                if (_framesSinceBlast > 10)
                {
                    _framesSinceBlast = 0;
                    doBlast = false;
                }
            }
            if (_chargeAnim.currentAnimation == "drain" && _chargeAnim.finished)
                _chargeAnim.SetAnimation("idle");
            _lastDoBlast = doBlast;
        }

        public override void Draw() => base.Draw();

        public override void OnPressAction()
        {
            if (_chargeAnim.currentAnimation == "idle")
            {
                _chargeSound.Volume = 1f;
                _chargeSound.Play();
                _chargeAnim.SetAnimation("charge");
                _unchargeSound.Stop();
                _unchargeSound.Volume = 0f;
                _unchargeSoundShort.Stop();
                _unchargeSoundShort.Volume = 0f;
            }
            else
            {
                if (!(_chargeAnim.currentAnimation == "uncharge"))
                    return;
                if (_chargeAnim.frame > 18)
                {
                    _chargeSound.Volume = 1f;
                    _chargeSound.Play();
                }
                else
                {
                    _chargeSoundShort.Volume = 1f;
                    _chargeSoundShort.Play();
                }
                int frame = _chargeAnim.frame;
                _chargeAnim.SetAnimation("charge");
                _chargeAnim.frame = 22 - frame;
                _unchargeSound.Stop();
                _unchargeSound.Volume = 0f;
                _unchargeSoundShort.Stop();
                _unchargeSoundShort.Volume = 0f;
            }
        }

        public override void OnHoldAction()
        {
        }

        public override void OnReleaseAction()
        {
            if (_chargeAnim.currentAnimation == "charge")
            {
                if (_chargeAnim.frame > 20)
                {
                    _unchargeSound.Volume = 1f;
                    _unchargeSound.Play();
                }
                else
                {
                    _unchargeSoundShort.Volume = 1f;
                    _unchargeSoundShort.Play();
                }
                int frame = _chargeAnim.frame;
                _chargeAnim.SetAnimation("uncharge");
                _chargeAnim.frame = 22 - frame;
                _chargeSound.Stop();
                _chargeSound.Volume = 0f;
                _chargeSoundShort.Stop();
                _chargeSoundShort.Volume = 0f;
            }
            if (!(_chargeAnim.currentAnimation == "charged"))
                return;
            Graphics.FlashScreen();
            _chargeAnim.SetAnimation("drain");
            SFX.Play("laserBlast");
            for (int index = 0; index < 4; ++index)
                Level.Add(new ElectricalCharge(barrelPosition.x, barrelPosition.y, offDir, this));
        }
    }
}
