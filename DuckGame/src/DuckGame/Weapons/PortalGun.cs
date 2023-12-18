namespace DuckGame
{
    [BaggedProperty("canSpawn", false)]
    [BaggedProperty("isOnlineCapable", false)]
    public class PortalGun : Gun
    {
        private bool _curPortal;

        public bool curPortal
        {
            get => _curPortal;
            set => _curPortal = value;
        }

        public PortalGun(float xval, float yval)
          : base(xval, yval)
        {
            ammo = 99;
            _ammoType = new ATPortal(this)
            {
                range = 600f,
                accuracy = 1f,
                rebound = false,
                bulletSpeed = 10f,
                bulletLength = 40f,
                rangeVariation = 50f
            };
            (_ammoType as ATPortal).angleShot = false;
            _type = "gun";
            graphic = new Sprite("phaser");
            center = new Vec2(7f, 4f);
            collisionOffset = new Vec2(-7f, -4f);
            collisionSize = new Vec2(15f, 9f);
            _barrelOffsetTL = new Vec2(14f, 3f);
            _fireSound = "laserRifle";
            _fullAuto = false;
            _fireWait = 0f;
            _kickForce = 0.5f;
            _holdOffset = new Vec2(0f, 0f);
            _flare = new SpriteMap("laserFlare", 16, 16)
            {
                center = new Vec2(0f, 8f)
            };
        }
    }
}
