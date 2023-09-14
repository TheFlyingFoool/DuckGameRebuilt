using System;

namespace DuckGame
{
    [ClientOnly]
    public class NiK0Gun : Gun
    {
        public float load;
        public StateBinding _loadBinding = new StateBinding("load");
        public SpriteMap sprite;
        public NiK0Gun(Vec2 pos) : base(pos.x, pos.y)
        {
            ammo = 2;
            _ammoType = new ATFailedPellet();
            sprite = new SpriteMap("nikoGun", 40, 20);
            graphic = sprite;
            _collisionSize = new Vec2(25, 11);
            _collisionOffset = new Vec2(-14, -5);
            _holdOffset = new Vec2(5, 0);
            center = new Vec2(20, 10);
            _barrelOffsetTL = new Vec2(35, 9f);
            wobble = new aWobbleMaterial(this, 0.2f);

            laserSight = true;
            spawn = new MaterialDev(this, new Color(0, 255, 255));
        }
        public MaterialDev spawn;
        public float spawnSc;
        public aWobbleMaterial wobble;
        public Sound charge;
        public override void OnPressAction()
        {

        }
        public override void Update()
        {
            if (_triggerHeld && ammo > 0)
            {
                if (charge == null)
                {
                    charge = SFX.Play("FunnyGunCharge");
                }
            }
            else
            {
                if (charge != null)
                {
                    charge.Kill();
                    charge = null;
                }
            }
            base.Update();
        }
        public override void OnHoldAction()
        {
            if (ammo > 0)
            {
                load = Lerp.Float(load, 6f, 0.023f);
                if (load > 4)
                {
                    load = Lerp.Float(load, 6f, 0.08f);
                }
            }
            else
            {
                if (_wait <= 0)
                {
                    DoAmmoClick();
                    _wait = 7f;
                }
            }
        }
        public override bool Sprung(Thing pSpringer)
        {
            float mult = ((Spring)pSpringer)._mult;
            if (vSpeed > -22f * mult) vSpeed = -22f * mult;
            return false;
        }
        public override void Fire()
        {
        }
        public override void OnReleaseAction()
        {
            if (load > 0.2f)
            {
                if (load >= 4) SFX.PlaySynchronized("FunnyGunSuperShoot");
                else SFX.PlaySynchronized("FunnyGunShoot");
                ammo--;
                Level.Add(new LightBullet(barrelPosition, barrelVector * Maths.Clamp(load * 10, 10, 25), load >= 4, duck));
                _kickForce = load * 2;
                ApplyKick();
                load = 0;

            }
            base.OnReleaseAction();
        }
        public float fUp;
        public float other;
        public float muli;
        public float r;
        public override void Initialize()
        {
            muli = Rando.Float(0.05f, 0.1f);
            r = Rando.Float(1, 3f);
            base.Initialize();
        }
        public override void DrawGlow()
        {
            if (laserSight && held && _laserTex != null && load > 0)
            {
                fUp += muli;
                other += Rando.Float(-0.5f, 0.5f);
                Vec2 norm = barrelVector;

                float lMult = load;

                if (load > 4) lMult = 10 - load;
                float f = 0.5f * lMult / 3;

                Color c = new Color(0, 255, 255);
                for (float z = 0; z < 12; z += 1.3f)
                {
                    Vec2 reNorm = barrelVector.Rotate((float)Math.Sin((fUp + z) * 0.33f) * 3, Vec2.Zero) * lMult / 2;
                    Vec2 vec3 = barrelPosition + reNorm * 8 - (barrelVector * z * 2.6f);


                    for (int i = 1; i < 4; i++)
                    {
                        Graphics.DrawTexturedLine(_laserTex, vec3, vec3 + reNorm * ((float)Math.Cos(i + (float)z + (float)Math.Sin(fUp - other)) * 15), c * (1f - i * 0.2f), f, depth - 3);
                        vec3 += reNorm * ((float)Math.Cos(fUp * norm.x * (Math.Sin(other * z / 12) + 1)) * 3);
                    }
                }
            }
        }
        public override void Draw()
        {
            if (load > 0 && duck != null)
            {
                LightBullet lb = new LightBullet(barrelPosition, barrelVector * Maths.Clamp(load * 10, 10, 25), load >= 4, duck);
                Vec2 last = lb.position;
                for (int i = 0; i < 100; i++)
                {
                    lb.UpdatePosition(true);
                    Graphics.DrawLine(last, lb.position, Color.White * load, 1, 1);
                    last = lb.position;
                }
            }

            if (!spawn.finished) { Graphics.material = spawn; spawn.Update(); }
            sprite.imageIndex = 0;
            base.Draw();
            sprite.imageIndex = 1;


            if (spawn.finished)
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
    }
}
