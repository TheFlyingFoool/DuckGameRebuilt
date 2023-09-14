using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DuckGame
{
    [ClientOnly]
    public class FirebreakGun : Gun
    {
        //26 19
        public SpriteMap sprite;
        private bool @CHARGEFUCKINGDIE = true;
        public FirebreakGun(float xpos, float ypos) : base(xpos, ypos) 
        {
            sprite = new SpriteMap("FirebreakGun", 26, 19);
            graphic = sprite;

            ammo = 5;

            _ammoType = new ATMagicalInk();
            _bulletColor = DGRDevs.Firebreak.Color;

            collisionSize = new Vec2(13, 9.5f);
            _collisionOffset = new Vec2(-7, -4.5f);
            center = new Vec2(13, 9.5f);
            wobble = new aWobbleMaterial(this, 0.2f);
            _holdOffset = new Vec2(3, 4);
            spawn = new MaterialDev(this, new Color(255, 89, 56));
            _barrelOffsetTL = new Vec2(20.5f, 7.5f);
        }
        public MaterialDev spawn;
        public float spawnSc;
        public aWobbleMaterial wobble;
        public override void OnPressAction()
        {
            if (ammo > 0)
            {
                @CHARGEFUCKINGDIE = false;
            }
        }
        public override void OnHoldAction()
        {
            if (ammo > 0)
            {
                if (sprite.imageIndex == 0)
                    return;

                charge = Lerp.Float(charge, 1, 0.01f);
                base.OnHoldAction();
            }
        }

        public override void OnReleaseAction()
        {
            if (ammo > 0)
            {
                if (charge < 0.3f)
                {
                    @CHARGEFUCKINGDIE = true;
                    return;
                }

                SFX.Play("magicFade", charge);

                _ammoType.bulletSpeed = charge * 12;
                _kickForce = charge * 3;
                Fire();
                charge = 0;
            }
        }
        public float charge;
        public override void Update()
        {
            sprite.imageIndex = owner != null && ammo > 0 ? 1 : 0;
            if (@CHARGEFUCKINGDIE)
                charge = Lerp.Float(charge, 0, 0.02f);
            base.Update();
        }
        public StateBinding _chargeBinding = new StateBinding("charge");
        public override void Draw()
        {
            Vec2 previousPosition = position;

            if (!spawn.finished) { Graphics.material = spawn; spawn.Update(); }

            x += Rando.Float(-charge, charge) * 0.5f;
            y += Rando.Float(-charge, charge) * 0.5f;
            base.Draw();

            if (charge > 0)
            {
                float trueAlpha = alpha;
                float trueAngle = angle;
                alpha = charge / 3;

                int loops = Rando.Int(2, 3);
                for (int i = 0; i < loops; i++)
                {
                    if (Rando.Int(2) == 0) angle = Rando.Float(4);
                    x += Rando.Float(12, 20) * Rando.ChooseInt(-1, 1);
                    y += Rando.Float(12, 20) * Rando.ChooseInt(-1, 1);

                    base.Draw();
                    angle = trueAngle;
                }
                alpha = trueAlpha;
                angle = trueAngle;

            }
            
            position = previousPosition;
            
            //Graphics.DrawLine(position, charge * 32f, angle + (Maths.PI / 2f), DGRDevs.Firebreak.Color, 1f, 2f);
            if (spawn.finished)
            {
                if (spawnSc == 0) SFX.Play("laserChargeTeeny", 0.8f, -0.1f);
                spawnSc = Lerp.FloatSmooth(spawnSc, 1, 0.06f);
                sprite.imageIndex += 2;
                Graphics.material = wobble;
                depth -= 2;
                alpha = spawnSc;
                base.Draw();
                alpha = 1;
                depth += 2;
                Graphics.material = null;
                sprite.imageIndex -= 2;
            }
        }
    }
}
