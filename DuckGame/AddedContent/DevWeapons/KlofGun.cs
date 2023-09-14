namespace DuckGame
{
    [ClientOnly]
    public class KlofGun : Gun
    {
        public SpriteMap sprite;
        
        public KlofGun(float xval, float yval) : base(xval, yval)
        {
            _ammoType = new ATLaser();
            sprite = new SpriteMap("brainrotgun", 26, 10);
            graphic = sprite;
            center = new Vec2(7f, 6f);
            collisionSize = new Vec2(24, 8);
            _collisionOffset = new Vec2(-4f, -3f);
            _barrelOffsetTL = new Vec2(25f, -4f);
            ammo = 8;
            kick = 10f;
            wobble = new aWobbleMaterial(this, 0.2f);
        }

        public override void OnPressAction()
        {
            if (ammo > 0)
            {
                Level.Add(new BrainRotBullet(barrelVector.x + barrelPosition.x, barrelVector.y + barrelPosition.y, barrelVector));
                ApplyKick();
                SFX.Play("laserBlast");
                ammo--;
            }
            else
            {
                DoAmmoClick();
            }

            base.OnPressAction();
        }

        public aWobbleMaterial wobble;
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

        public override void Fire()
        {
            
        }
    }
}