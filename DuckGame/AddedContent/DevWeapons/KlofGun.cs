namespace DuckGame
{
    [ClientOnly]
    #if DEBUG
    [EditorGroup("Guns|DEV")]
    [BaggedProperty("canSpawn", false)]
    #endif
    public class KlofGun : Gun
    {
        public SpriteMap sprite;
        
        public KlofGun(float xval, float yval) : base(xval, yval)
        {
            _ammoType = new ATLaser();
            sprite = new SpriteMap("brainrotgun", 26, 10);
            graphic = sprite;
            center = new Vec2(7f, 6f);
            collisionSize = new Vec2(23, 5);
            _collisionOffset = new Vec2(-5.5f, -5f);
            _barrelOffsetTL = new Vec2(20, -4.5f);
            ammo = 8;
            _kickForce = 10;
            wobble = new aWobbleMaterial(this, 0.2f);
            spawn = new MaterialDev(this, new Color(0, 250, 154));
            _holdOffset = new Vec2(1, 0);
        }
        public MaterialDev spawn;
        public float spawnSc;

        public override void OnPressAction()
        {
            if (isServerForObject)
            {
                if (ammo > 0)
                {
                    Level.Add(new BrainRotBullet(barrelPosition.x, barrelPosition.y, barrelVector));
                    ApplyKick();
                    SFX.PlaySynchronized("laserBlast");
                    ammo--;
                }
                else
                {
                    DoAmmoClick();
                }
            }
        }

        public aWobbleMaterial wobble;
        public override void Draw()
        {
            if (!spawn.finished) { Graphics.material = spawn; spawn.Update(); }
            sprite.imageIndex = 0;
            base.Draw();
            sprite.imageIndex = 1;

            if (spawn.finished && level != null)  
            {
                if (spawnSc == 0) SFX.Play("laserChargeTeeny", 0.8f, -0.1f);
                spawnSc = Lerp.FloatSmooth(spawnSc, 1, 0.06f);
                Graphics.material = wobble;
                depth -= 1;
                alpha = spawnSc;
                base.Draw();
                alpha = 1;
                depth += 1;
                Graphics.material = null;
            }
        }

        public override void Fire()
        {
            
        }
    }
}