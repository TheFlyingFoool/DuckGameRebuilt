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
            get => this._chargeAnim == null ? (byte)0 : (byte)this._chargeAnim.animationIndex;
            set
            {
                if (this._chargeAnim == null || this._chargeAnim.animationIndex == value)
                    return;
                this._chargeAnim.animationIndex = value;
            }
        }

        public byte spriteFrame
        {
            get => this._chargeAnim == null ? (byte)0 : (byte)this._chargeAnim._frame;
            set
            {
                if (this._chargeAnim == null)
                    return;
                this._chargeAnim._frame = value;
            }
        }

        public CoilGun(float xval, float yval)
          : base(xval, yval)
        {
            this.ammo = 30;
            this._type = "gun";
            this.center = new Vec2(16f, 16f);
            this.collisionOffset = new Vec2(-11f, -8f);
            this.collisionSize = new Vec2(22f, 12f);
            this._barrelOffsetTL = new Vec2(25f, 13f);
            this._fireSound = "";
            this._fullAuto = false;
            this._fireWait = 1f;
            this._kickForce = 1f;
            this._fireRumble = RumbleIntensity.Kick;
            this._holdOffset = new Vec2(3f, 1f);
            this._editorName = "Death Laser";
            this._chargeAnim = new SpriteMap("coilGun", 32, 32);
            this._chargeAnim.AddAnimation("idle", 1f, true, new int[1]);
            this._chargeAnim.AddAnimation("charge", 0.38f, false, 1, 2, 3, 0, 1, 2, 3, 4, 5, 6, 7, 4, 5, 6, 7);
            this._chargeAnim.AddAnimation("charged", 1f, true, 8, 9, 10, 11);
            this._chargeAnim.AddAnimation("uncharge", 1.2f, false, 7, 6, 5, 4, 7, 6, 5, 4, 3, 2, 1, 0, 3, 2, 1, 0);
            this._chargeAnim.AddAnimation("drain", 2f, false, 7, 6, 5, 4, 7, 6, 5, 4, 3, 2, 1, 0, 3, 2, 1, 0);
            this._chargeAnim.SetAnimation("idle");
            this.graphic = _chargeAnim;
        }

        public override void Initialize()
        {
            this._chargeSound = SFX.Get("laserCharge", 0.0f);
            this._chargeSoundShort = SFX.Get("laserChargeShort", 0.0f);
            this._unchargeSound = SFX.Get("laserUncharge", 0.0f);
            this._unchargeSoundShort = SFX.Get("laserUnchargeShort", 0.0f);
        }

        public override void Update()
        {
            base.Update();
            if (_charge > 0.0)
                this._charge -= 0.1f;
            else
                this._charge = 0.0f;
            if (this._chargeAnim.currentAnimation == "uncharge" && this._chargeAnim.finished)
                this._chargeAnim.SetAnimation("idle");
            if (Network.isActive && this.doBlast && !this._lastDoBlast || this._chargeAnim.currentAnimation == "charge" && this._chargeAnim.finished && this.isServerForObject)
                this._chargeAnim.SetAnimation("charged");
            if (this.doBlast && this.isServerForObject)
            {
                ++this._framesSinceBlast;
                if (this._framesSinceBlast > 10)
                {
                    this._framesSinceBlast = 0;
                    this.doBlast = false;
                }
            }
            if (this._chargeAnim.currentAnimation == "drain" && this._chargeAnim.finished)
                this._chargeAnim.SetAnimation("idle");
            this._lastDoBlast = this.doBlast;
        }

        public override void Draw() => base.Draw();

        public override void OnPressAction()
        {
            if (this._chargeAnim.currentAnimation == "idle")
            {
                this._chargeSound.Volume = 1f;
                this._chargeSound.Play();
                this._chargeAnim.SetAnimation("charge");
                this._unchargeSound.Stop();
                this._unchargeSound.Volume = 0.0f;
                this._unchargeSoundShort.Stop();
                this._unchargeSoundShort.Volume = 0.0f;
            }
            else
            {
                if (!(this._chargeAnim.currentAnimation == "uncharge"))
                    return;
                if (this._chargeAnim.frame > 18)
                {
                    this._chargeSound.Volume = 1f;
                    this._chargeSound.Play();
                }
                else
                {
                    this._chargeSoundShort.Volume = 1f;
                    this._chargeSoundShort.Play();
                }
                int frame = this._chargeAnim.frame;
                this._chargeAnim.SetAnimation("charge");
                this._chargeAnim.frame = 22 - frame;
                this._unchargeSound.Stop();
                this._unchargeSound.Volume = 0.0f;
                this._unchargeSoundShort.Stop();
                this._unchargeSoundShort.Volume = 0.0f;
            }
        }

        public override void OnHoldAction()
        {
        }

        public override void OnReleaseAction()
        {
            if (this._chargeAnim.currentAnimation == "charge")
            {
                if (this._chargeAnim.frame > 20)
                {
                    this._unchargeSound.Volume = 1f;
                    this._unchargeSound.Play();
                }
                else
                {
                    this._unchargeSoundShort.Volume = 1f;
                    this._unchargeSoundShort.Play();
                }
                int frame = this._chargeAnim.frame;
                this._chargeAnim.SetAnimation("uncharge");
                this._chargeAnim.frame = 22 - frame;
                this._chargeSound.Stop();
                this._chargeSound.Volume = 0.0f;
                this._chargeSoundShort.Stop();
                this._chargeSoundShort.Volume = 0.0f;
            }
            if (!(this._chargeAnim.currentAnimation == "charged"))
                return;
            Graphics.FlashScreen();
            this._chargeAnim.SetAnimation("drain");
            SFX.Play("laserBlast");
            for (int index = 0; index < 4; ++index)
                Level.Add(new ElectricalCharge(this.barrelPosition.x, this.barrelPosition.y, offDir, this));
        }
    }
}
