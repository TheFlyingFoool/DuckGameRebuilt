using System;

namespace DuckGame
{
    [ClientOnly]
    #if DEBUG
    [EditorGroup("Rebuilt|DEV")]
    [BaggedProperty("canSpawn", false)]
    #endif
    public class HyeveGun : Gun
    {
        public SpriteMap sprite;
        public SpriteMap ice;
        public StateBinding _chargeBinding = new StateBinding("charge");
        public StateBinding _heatBinding = new StateBinding("heat");
        public HyeveGun(float xpos, float ypos) : base(xpos, ypos)
        {//15 22
            ice = new SpriteMap("iceBlock", 16, 16);
            ice.center = new Vec2(8);

            _ammoType = new ATMissile();
            sprite = new SpriteMap("HyeveGun", 18, 21);
            center = new Vec2(9, 10.5f);

            _barrelOffsetTL = new Vec2(15, 10);
            collisionSize = new Vec2(8, 13);//15 18
            _collisionOffset = new Vec2(-6f, -4.5f);
            graphic = sprite;
            ammo = 24;
            _holdOffset = new Vec2(0, -2);

            chargeSound = new LoopingSound("chainsawIdle");
            wobble = new aWobbleMaterial(this, 0.2f);
            spawn = new MaterialDev(this, new Color(237, 50, 168));
        }
        public MaterialDev spawn;
        public float spawnSc;
        public aWobbleMaterial wobble;
        public override void OnPressAction()
        {
            if (ammo > 0)
            {
                snapped = 0;
                charge = 0.7f;
            }
            else DoAmmoClick();
        }
        public override void OnHoldAction()
        {
            if (ammo > 0)
            {
                charge += 0.01f;
                if (charge > ammo) charge = ammo;
                if (charge > 4) charge = 4;
            }
        }
        public float charge;
        public LoopingSound chargeSound;
        public override void OnReleaseAction()
        {
            if (ammo > 0)
            {

                Vec2 pos = barrelPosition + (barrelVector * 4 * actualCharge);

                IcerIcyBlock ic = new IcerIcyBlock(pos.x, pos.y, actualCharge / 2f);
                ic.velocity = barrelVector * (8 - actualCharge);
                Level.Add(ic);
                if (actualCharge == 4) ic.ReturnItemToWorld(ic);

                ammo -= actualCharge;
                SFX.PlaySynchronized("pipeOut", 1, -0.1f);
                SFX.PlaySynchronized("netgunFire", 1, -(charge / 6f));
                snapped = 1;
                _kickForce = charge;
                ApplyKick();
                charge = 0.7f;

            }
        }
        public override void Update()
        {
            if (charge > 0.7f)
            {
                chargeSound.lerpVolume = Lerp.Float(chargeSound.lerpVolume, (charge - 0.7f) / 3f, 0.01f);
                chargeSound.pitch = Maths.Clamp((charge - 2) / 2f, -0.3f, 0.6f);
                actualCharge = (int)Math.Round(charge);
                if (actualCharge != snapped)
                {
                    heat -= 0.3f;
                    SFX.Play("glassHit", 1);
                    snapped = actualCharge;
                    Vec2 pos = barrelPosition + (barrelVector * 4 * actualCharge);
                    for (int index = 0; index < DGRSettings.ActualParticleMultiplier * actualCharge * 4; ++index)
                    {
                        Vec2 hitS = Maths.AngleToVec(Rando.Float(6.28f));
                        GlassParticle glassParticle = new GlassParticle(pos.x + Rando.Float(-4f , 4f) * actualCharge, pos.y + Rando.Float(-4f, 4f) * actualCharge, hitS);
                        glassParticle.hSpeed = (hitS.x * 2f * (Rando.Float(1f) + 0.3f));
                        glassParticle.vSpeed = (hitS.y * 2f * (Rando.Float(1f) + 0.3f)) - Rando.Float(2f);
                        glassParticle.hSpeed *= actualCharge / 3f;
                        glassParticle.vSpeed *= actualCharge / 4f;
                        Level.Add(glassParticle);
                    }
                }
            }
            else
            {
                chargeSound.lerpVolume = Lerp.Float(chargeSound.lerpVolume, 0, 0.1f);
                chargeSound.pitch = Lerp.Float(chargeSound.pitch, 0, 0.1f);
            }
            chargeSound.Update();

            base.Update();
        }
        public int actualCharge;
        public int snapped;
        public override void Draw()
        {
            //new Color(0, 150, 249)

            if (!spawn.finished) { Graphics.material = spawn; spawn.Update(); }

            if (charge > 0.7f)
            {
                Vec2 pos = barrelPosition + (barrelVector * 4 * actualCharge);

                ice.scale = new Vec2(0.5f) * actualCharge;
                Graphics.Draw(ice, pos.x, pos.y, depth - 1);
            }


            sprite.imageIndex = 0;
            Vec2 p = position;

            if (charge > 2)
            {
                x += Rando.Float(-charge, charge) / 7f;
                y += Rando.Float(-charge, charge) / 7f;
            }


            int ls = Maths.Clamp(ammo, 0, 24);
            Graphics.DrawRect(Offset(new Vec2(-5f, -3.5f + ((charge - 0.7f) * 0.34f) - ((ls - 24) / 4f))), Offset(new Vec2(1f, 3.1f)), new Color(0, 150, 249), depth - 1);

            base.Draw();

            if (spawn.finished && level != null)  
            {
                if (spawnSc == 0) SFX.Play("laserChargeTeeny", 0.8f, -0.1f);
                spawnSc = Lerp.FloatSmooth(spawnSc, 1, 0.06f);
                sprite.imageIndex = 1;
                Graphics.material = wobble;
                depth -= 2;
                alpha = spawnSc;
                base.Draw();
                alpha = 1;
                depth += 2;
                Graphics.material = null;
                position = p;
            }
        }
    }
}
