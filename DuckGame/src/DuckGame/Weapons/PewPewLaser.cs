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
            ammo = 36;
            _ammoType = new ATPewPew();
            _type = "gun";
            graphic = new Sprite("pewpewLaser");
            center = new Vec2(16f, 16f);
            collisionOffset = new Vec2(-8f, -3f);
            collisionSize = new Vec2(16f, 7f);
            _barrelOffsetTL = new Vec2(31f, 15f);
            _fireSound = "laserRifle";
            _fullAuto = true;
            _fireWait = 2f;
            _kickForce = 1f;
            _fireRumble = RumbleIntensity.Kick;
            _holdOffset = new Vec2(0f, 0f);
            _flare = new SpriteMap("laserFlare", 16, 16)
            {
                center = new Vec2(0f, 8f)
            };
            editorTooltip = "Quick-fire laser beam of ULTIMATE DESTRUCTION... with an adorable wittle name.";
        }

        public override void Update()
        {
            if (_bursting)
            {
                _burstWait = Maths.CountDown(_burstWait, 0.16f);
                if (_burstWait <= 0)
                {
                    _burstWait = 1f;
                    if (isServerForObject)
                    {
                        inFire = true;
                        Fire();
                        inFire = false;
                        if (Network.isActive)
                            Send.Message(new NMFireGun(this, firedBullets, bulletFireIndex, false, duck != null ? duck.netProfileIndex : (byte)4, true), NetMessagePriority.Urgent);
                        firedBullets.Clear();
                    }
                    _wait = 0f;
                    ++_burstNum;
                }
                if (_burstNum == 3)
                {
                    _burstNum = 0;
                    _burstWait = 0f;
                    _bursting = false;
                    _wait = _fireWait;
                }
            }
            base.Update();
        }

        public override void OnPressAction()
        {
            if (receivingPress && hasFireEvents && onlyFireAction)
            {
                inFire = true;
                Fire();
                inFire = false;
            }
            if (_bursting || _wait != 0)
                return;
            _bursting = true;
        }

        public override void OnHoldAction()
        {
        }
    }
}
