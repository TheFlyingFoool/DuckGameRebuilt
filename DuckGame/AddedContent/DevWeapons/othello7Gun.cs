using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DuckGame
{
    [ClientOnly]
#if DEBUG
    [EditorGroup("Rebuilt|DEV")]
    [BaggedProperty("canSpawn", false)]
#endif
    public class othello7Gun : Gun
    {
        public Sprite gBaseOut;
        public Sprite gBarrel;
        public Sprite gBarrelOut;
        public Sprite gSpin;
        public Sprite gSpinOut;
        public othello7Gun(float xpos, float ypos) : base(xpos, ypos)
        {
            graphic = new Sprite("othBase");
            gBaseOut = new Sprite("othBaseOut");
            gBarrel = new Sprite("othBarrel");
            gBarrelOut = new Sprite("othBarrelOut");
            gSpin = new Sprite("othSpin");
            gSpinOut = new Sprite("othSpinOut");
            gSpin.center = new Vec2(5);
            gSpinOut.center = new Vec2(5);

            gBarrel.center = new Vec2(0, 5);
            gBarrelOut.center = new Vec2(1, 6);

            center = new Vec2(11, 8.5f);
            gBaseOut.center = center;

            _ammoType = new ATHighCalSniper();

            _kickForce = 5f;
            wobble = new aWobbleMaterial(this, 0.2f);

            collisionSize = new Vec2(18, 10);
            _collisionOffset = new Vec2(-10, -4.5f);

            ammo = 07;
            barrelAng = 6.283f;

            wobble.timeMult = 0.3f;
            _holdOffset = new Vec2(3, -1);
            _barrelOffsetTL = new Vec2(25, 4.5f);


            spinAng = 6.283f;
            spawn = new MaterialDev(this, new Color(160, 0, 255));
        }
        public MaterialDev spawn;
        public float spawnSc;

        public aWobbleMaterial wobble;

        public Vec2 barrelPos;
        public float barrelAng;

        public float spinAng;
        public float spinSpeed;
        public override void OnPressAction() //newLevelBing
        {//dukget 1.4 pitch
            if (loaded)
            {
                if (isServerForObject)
                {
                    ApplyKick();
                    loaded = false;
                    spinSpeed += 0.82f;
                    barrelAng = -0.5f;
                    SFX.PlaySynchronized("rockHitGround", 1, -0.8f);
                    ammo = 07;
                    OthProjectile proj = new OthProjectile(barrelPosition.x, barrelPosition.y);
                    proj.velocity = barrelVector * 2.2f;
                    Level.Add(proj);
                }
            }
        }
        public override void Fire()
        {
        }

        public StateBinding _spinAngBinding = new StateBinding("spinAng");
        public StateBinding _barrelAngBinding = new StateBinding("barrelAng");
        public StateBinding _spinSpeedBinding = new StateBinding("spinSpeed");
        public override void Update()
        {
            if (isServerForObject)
            {
                spinAng += spinSpeed;
                if (spinAng > 6.283f) spinAng -= 6.283f;

                barrelAng = Lerp.FloatSmooth(barrelAng, 0.1f, 0.02f);
                if (barrelAng > 0) barrelAng = 0;

                bool click = spinAng == 6.283f;
                spinAng = Lerp.Float(spinAng, 6.283f, 0.05f);
                if (!click && spinAng == 6.283f && spinSpeed == 0)
                {
                    SFX.PlaySynchronized("click");
                    loaded = true;
                }

                spinSpeed = Lerp.Float(spinSpeed, 0, 0.01f);
            }
            base.Update();
        }
        public override void Draw()
        {
            if (!spawn.finished) { Graphics.material = spawn; spawn.Update(); }

            Vec2 barPos = Offset(new Vec2(-5 + barrelAng * 8, -3.5f) + barrelPos);
            gBarrel.angle = graphic.angle + barrelAng * offDir;
            gBarrel.alpha = graphic.alpha;
            gBarrel.scale = graphic.scale;
            gBarrel.flipH = graphic.flipH;
            Graphics.Draw(gBarrel, barPos.x, barPos.y, depth - 1);

            float ang = angle;
            angle = gBarrel.angle;
            Vec2 spinPos = Offset(new Vec2(4f, 1.5f));
            angle = ang;
            gSpin.angle = graphic.angle + spinAng;
            gSpin.alpha = graphic.alpha;
            gSpin.scale = graphic.scale;
            gSpin.flipH = graphic.flipH;
            gSpin.center = new Vec2(4);
            Graphics.Draw(gSpin, spinPos.x, spinPos.y, depth + 1);
            base.Draw();

            /*_graphic.position = position;
                _graphic.alpha = alpha;
                _graphic.angle = angle;
                _graphic.depth = depth;
                _graphic.scale = scale;
                _graphic.center = center;
             * */

            if (spawn.finished && level != null)  
            {
                if (spawnSc == 0) SFX.Play("laserChargeTeeny", 0.8f, -0.1f);
                spawnSc = Lerp.FloatSmooth(spawnSc, 1, 0.06f);
                Graphics.material = wobble;
                alpha = spawnSc;
                graphic.alpha = spawnSc;


                gBarrelOut.angle = graphic.angle + barrelAng * offDir;
                gBarrelOut.alpha = graphic.alpha;
                gBarrelOut.scale = graphic.scale;
                gBarrelOut.flipH = graphic.flipH;
                Graphics.Draw(gBarrelOut, barPos.x, barPos.y, depth - 2);

                gSpinOut.angle = graphic.angle + spinAng;
                gSpinOut.alpha = graphic.alpha;
                gSpinOut.scale = graphic.scale;
                gSpinOut.flipH = graphic.flipH;
                gSpinOut.center = new Vec2(5);
                Graphics.Draw(gSpinOut, spinPos.x, spinPos.y, depth - 2);

                gBaseOut.angle = graphic.angle;
                gBaseOut.alpha = graphic.alpha;
                gBaseOut.scale = graphic.scale;
                gBaseOut.flipH = graphic.flipH;

                Graphics.Draw(gBaseOut, x, y, depth - 2);

                Graphics.material = null;
                graphic.alpha = 1;
                alpha = 1;
            }
        }
    }
}
