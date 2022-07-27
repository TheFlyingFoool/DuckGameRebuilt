// Decompiled with JetBrains decompiler
// Type: DuckGame.PewPewLaser
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    [EditorGroup("Guns|Lasers")]
    public class PewPewLaser : Gun
    {
        public StateBinding _burstingBinding = new StateBinding(nameof(_bursting));
        public StateBinding _burstNumBinding = new StateBinding(nameof(_burstNum));
        public float _burstWait;
        public bool _bursting;
        public int _burstNum;
        public static bool inFire;

        public PewPewLaser(float xval, float yval)
          : base(xval, yval)
        {
            this.ammo = 36;
            this._ammoType = new ATPewPew();
            this._type = "gun";
            this.graphic = new Sprite("pewpewLaser");
            this.center = new Vec2(16f, 16f);
            this.collisionOffset = new Vec2(-8f, -3f);
            this.collisionSize = new Vec2(16f, 7f);
            this._barrelOffsetTL = new Vec2(31f, 15f);
            this._fireSound = "laserRifle";
            this._fullAuto = true;
            this._fireWait = 2f;
            this._kickForce = 1f;
            this._fireRumble = RumbleIntensity.Kick;
            this._holdOffset = new Vec2(0.0f, 0.0f);
            this._flare = new SpriteMap("laserFlare", 16, 16)
            {
                center = new Vec2(0.0f, 8f)
            };
            this.editorTooltip = "Quick-fire laser beam of ULTIMATE DESTRUCTION... with an adorable wittle name.";
        }

        public override void Update()
        {
            if (this._bursting)
            {
                this._burstWait = Maths.CountDown(this._burstWait, 0.16f);
                if (_burstWait <= 0.0)
                {
                    this._burstWait = 1f;
                    if (this.isServerForObject)
                    {
                        PewPewLaser.inFire = true;
                        this.Fire();
                        PewPewLaser.inFire = false;
                        if (Network.isActive)
                            Send.Message(new NMFireGun(this, this.firedBullets, this.bulletFireIndex, false, this.duck != null ? this.duck.netProfileIndex : (byte)4, true), NetMessagePriority.Urgent);
                        this.firedBullets.Clear();
                    }
                    this._wait = 0.0f;
                    ++this._burstNum;
                }
                if (this._burstNum == 3)
                {
                    this._burstNum = 0;
                    this._burstWait = 0.0f;
                    this._bursting = false;
                    this._wait = this._fireWait;
                }
            }
            base.Update();
        }

        public override void OnPressAction()
        {
            if (this.receivingPress && this.hasFireEvents && this.onlyFireAction)
            {
                PewPewLaser.inFire = true;
                this.Fire();
                PewPewLaser.inFire = false;
            }
            if (this._bursting || _wait != 0.0)
                return;
            this._bursting = true;
        }

        public override void OnHoldAction()
        {
        }
    }
}
