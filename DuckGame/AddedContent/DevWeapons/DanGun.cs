namespace DuckGame
{
    [ClientOnly]
    public class DanGun : Gun
    {
        public SpriteMap sprite;
        public StateBinding _chargeBinding = new StateBinding("charge");
        public DanGun(float xpos, float ypos) : base(xpos, ypos)
        {
            ammo = 1;
            _ammoType = new ATLaser();//place holder cuz dg
            sprite = new SpriteMap("DanGun", 29, 17);
            graphic = sprite;
            center = new Vec2(14.5f, 8.5f);
            _barrelOffsetTL = new Vec2(27, 7f);
            collisionSize = new Vec2(22, 10);
            _collisionOffset = new Vec2(-12.5f, -3.5f);
            wobble = new aWobbleMaterial(this, 0.2f);
        }
        public aWobbleMaterial wobble;
        public float charge;
        public override void Fire()
        {
        }
        public override void OnPressAction()
        {
            if (ammo > 0)
            {
                chargeSound = SFX.Play("danCharge");
                charge = 0;
            }
            else DoAmmoClick();
        }
        public bool glitch;
        public override void OnHoldAction()
        {
            if (ammo > 0)
            {
                charge += 0.03f;
                if (charge > 2) charge = 2;
                base.OnHoldAction();
            }
        }
        public override void OnReleaseAction()
        {
            if (ammo > 0)
            {
                Vec2 v = barrelPosition + barrelVector * charge * 10;

                Level.Add(new GlitchProjectile(v.x, v.y, charge, barrelVector + (barrelVector * charge * 1.5f)));
                _kickForce =  4 * charge;
                charge = 0;
                ammo--;
                ApplyKick();
            }
        }
        public Sound chargeSound;
        public override void Update()
        {
            if (_triggerHeld)
            {
                if (Rando.Int(30) == 0)
                {
                    glitch = true;
                    SFX.Play("glitch" + Rando.Int(1, 2), Rando.Float(0.3f, 0.5f), Rando.Float(-0.2f, 0.2f));
                }
            }
            else if (chargeSound != null && chargeSound.State == Microsoft.Xna.Framework.Audio.SoundState.Playing)
            {
                chargeSound.Stop();
            }
            base.Update();
        }
        public override void Draw()
        {
            if (charge > 0)
            {
                Vec2 v = barrelPosition + barrelVector * charge * 10;
                Graphics.DrawCircle(v, charge * 10, Color.Lime, 1, 1);
            }

            if (glitch)
            {
                Vec2 p = position;
                float a = angle;
                Vec2 s = scale;

                int loops = 1;
                if (Rando.Int(1) == 0) loops = Rando.Int(1, 4);
                for (int i = 0; i < loops; i++)
                {
                    x += Rando.Float(-12, 12);
                    y += Rando.Float(-12, 12);
                    if (Rando.Int(2) == 0) a += Rando.Float(-4, 4);
                    if (Rando.Int(4) == 0) xscale *= Rando.Float(0.5f, 1.5f);
                    if (Rando.Int(4) == 0) yscale *= Rando.Float(0.5f, 1.5f);

                    sprite.imageIndex = 0;
                    base.Draw();
                    sprite.imageIndex = 1;
                    Graphics.material = wobble;
                    depth -= 1;
                    base.Draw();
                    depth += 1;
                    Graphics.material = null;
                }
                if (Rando.Int(2) == 0) glitch = false;
                scale = s;
                angle = a;
                position = p;
            }
            else
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
        }
    }
}
