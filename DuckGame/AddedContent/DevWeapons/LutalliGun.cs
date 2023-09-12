namespace DuckGame
{
    [ClientOnly]
    public class LutalliGun : Gun
    {
        public SpriteMap sprite;
        public LutalliGun(float xpos, float ypos) : base(xpos, ypos)
        {
            ammo = 100;
            _ammoType = new ATLaser();
            sprite = new SpriteMap("LutalliGun", 29, 23);
            graphic = sprite;
            center = new Vec2(14.5f, 11.5f);
            _collisionSize = new Vec2(22, 8);
            _collisionOffset = new Vec2(-12.5f, -3.5f);
            _fireWait = 1.5f;
            _barrelOffsetTL = new Vec2(27, 11);
            wobble = new aWobbleMaterial(this, 0.2f);
            _kickForce = 1.7f;
        }
        public aWobbleMaterial wobble;

        public LPortal portal1;
        public LPortal portal2;

        public int img;
        public override void Fire()
        {
            if (_wait == 0)
            {
                ApplyKick();
                _wait = _fireWait;

                PortalProjectile pp = new PortalProjectile(barrelPosition.x, barrelPosition.y);
                pp.velocity = barrelVector * 14;
                pp.firedBy = this;
                pp.frame = img;
                pp.angle = -barrelAngle;

                img++;
                if (img > 1) img = 0;
                Level.Add(pp);
            }
        }
        public override void Draw()
        {
            sprite.imageIndex = 0;
            base.Draw();

            sprite.imageIndex = 1;
            Graphics.material = wobble;
            depth -= 1;
            base.Draw();
            depth += 1;
            Graphics.material = null;
        }
        public override void Update()
        {
            if (portal1 != null && portal2 != null && !portal1.removeFromLevel && !portal2.removeFromLevel)
            {
                portal1.link = portal2;
                portal2.link = portal1;
            }
            ammo = 100;
            base.Update();
        }
    }
}
