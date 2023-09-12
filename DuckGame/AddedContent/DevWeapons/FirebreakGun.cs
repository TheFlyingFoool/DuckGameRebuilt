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
        public FirebreakGun(float xpos, float ypos) : base(xpos, ypos) 
        {
            sprite = new SpriteMap("FirebreakGun", 26, 19);
            graphic = sprite;

            ammo = 3;

            _ammoType = new ATHighCalSniper();

            collisionSize = new Vec2(13, 9.5f);
            _collisionOffset = new Vec2(-7, -4.5f);
            center = new Vec2(13, 9.5f);
            wobble = new aWobbleMaterial(this, 0.2f);
            _holdOffset = new Vec2(3, 4);
        }
        public aWobbleMaterial wobble;
        public override void Fire()
        {
        }
        public override void OnPressAction()
        {
        }
        public override void OnHoldAction()
        {
            charge = Lerp.Float(charge, 1, 0.005f);
            base.OnHoldAction();
        }
        public override void OnReleaseAction()
        {
            SFX.Play("magicFade", charge);

            _kickForce = charge * 3;
            ApplyKick();
            base.OnReleaseAction();
            charge = 0;
        }
        public float charge;
        public override void Update()
        {
            sprite.imageIndex = owner != null ? 1 : 0;
            base.Update();
        }
        public override void Draw()
        {
            Vec2 p = position;

            x += Rando.Float(-charge, charge) * 0.5f;
            y += Rando.Float(-charge, charge) * 0.5f;
            base.Draw();

            if (charge > 0)
            {
                float a = alpha;
                float ang = angle;
                alpha = charge / 3;

                int loops = Rando.Int(2, 3);
                for (int i = 0; i < loops; i++)
                {
                    if (Rando.Int(2) == 0) angle = Rando.Float(4);
                    x += Rando.Float(12, 20) * Rando.ChooseInt(-1, 1);
                    y += Rando.Float(12, 20) * Rando.ChooseInt(-1, 1);

                    base.Draw();
                    angle = ang;
                }
                alpha = a;
                angle = ang;

            }
                position = p;

            sprite.imageIndex += 2;
            Graphics.material = wobble;
            depth -= 2;
            base.Draw();
            depth += 2;
            Graphics.material = null;
            sprite.imageIndex -= 2;
        }
    }
}
