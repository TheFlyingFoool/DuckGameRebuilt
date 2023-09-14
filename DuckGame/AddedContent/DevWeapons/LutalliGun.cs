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
            spawn = new MaterialDev(this, new Color(147, 112, 219));
        }
        public MaterialDev spawn;
        public float spawnSc;
        public aWobbleMaterial wobble;

        public LPortal portal1;
        public LPortal portal2;

        public int img;
        public StateBinding _imgBinding = new StateBinding("img");
        public StateBinding _portal1Binding = new StateBinding("portal1");
        public StateBinding _portal2Binding = new StateBinding("portal2");
        public override void Fire()
        {
            if (_wait == 0)
            {
                ApplyKick();
                _wait = _fireWait;

                if (isServerForObject)
                {
                    PortalProjectile pp = new PortalProjectile(barrelPosition.x, barrelPosition.y);
                    pp.velocity = barrelVector * 14;
                    pp.firedBy = this;
                    pp.frame = img;
                    pp.angle = -barrelAngle;

                    img++;
                    if (img > 1) img = 0;
                    Level.Add(pp);
                }

                for (int i = 0; i < 6 * DGRSettings.ActualParticleMultiplier; i++)
                {
                    PortalParticle pp = new PortalParticle(x, y, img == 0 ? new Color(227, 171, 2) : new Color(2, 222, 206));

                    pp.position = barrelPosition;
                    pp.velocity = barrelVector.Rotate(Rando.Float(-0.6f, 0.6f), Vec2.Zero) * Rando.Float(3);

                    pp.scale = new Vec2(Rando.Float(1.3f, 2.4f));
                    pp.alpha = Rando.Float(0.6f, 0.8f);
                    Level.Add(pp);
                }
            }
        }
        public override void Draw()
        {
            if (!spawn.finished) { Graphics.material = spawn; spawn.Update(); }

            sprite.imageIndex = 0;
            base.Draw();
            if (spawn.finished)
            {
                if (spawnSc == 0) SFX.Play("laserChargeTeeny", 0.8f, -0.1f);
                spawnSc = Lerp.FloatSmooth(spawnSc, 1, 0.06f);
                sprite.imageIndex = 1;
                alpha = spawnSc;
                Graphics.material = wobble;
                depth -= 1;
                base.Draw();
                depth += 1;
                alpha = 1;
                Graphics.material = null;
            }
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
