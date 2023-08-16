using System;

namespace DuckGame
{
    [ClientOnly]
    public class SniperZooka : Gun
    {
        public SniperZooka(float xpos, float ypos) : base(xpos, ypos)
        {
            ammo = 40;
            _ammoType = new ATMissile();
            _ammoType.bulletSpeed = 22;
            laserSight = true;
            _laserOffsetTL = new Vec2(37, 3.5f);
            _barrelOffsetTL = new Vec2(30, 8.5f);
            graphic = new Sprite("snizooka");
            center = new Vec2(18.5f, 8.5f);
            collisionOffset = new Vec2(-18.5f, -2.5f);
            collisionSize = new Vec2(30f, 7);
            _barrelOffsetTL = new Vec2(29f, 4f);
            _fireSound = "missile";
            _kickForce = 4.5f;
            //OH SHIT WE GOTTA CHARGE UP THE SNIZ SNOS
            snizsnos = new Sprite("missile");
            snizsnos.CenterOrigin();
            _manualLoad = true;
            physicsMaterial = PhysicsMaterial.Metal;

            _fireRumble = RumbleIntensity.Medium;

            loseAccuracy = 0.7f;
            maxAccuracyLost = 1.5f;
        }
        public Sprite snizsnos;
        public float load = -1;
        public float mult = 1;

        public StateBinding _loadBinding = new StateBinding("load");
        public StateBinding _multBinding = new StateBinding("mult");
        public StateBinding _rotAngleBinding = new StateBinding("rotangle");
        public override void OnPressAction()
        {
            if (loaded)
            {
                base.OnPressAction();
            }
            else if (isServerForObject)
            {
                if (load == -1)
                {
                    if (duck != null)
                    {
                        duck.runMax = 1.3f;
                    }
                    load = 2.1f;
                    _canRaise = false;
                }
                else mult += 0.15f;
            }
        }
        public override bool HolsterActivate(Holster pHolster)
        {
            if (!loaded)
            {
                pHolster.EjectItem();
                SFX.Play("spring", 1, Rando.Float(-0.6f, -0.4f));
                return false;
            }
            return base.HolsterActivate(pHolster);
        }
        public float rotAngle;
        public override float angle
        {
            get => owner == null?base.angle: base.angle + Maths.DegToRad(-rotAngle * offDir);
            set => _angle = value;
        }
        public bool tech;
        public override void Thrown()
        {
            if (duck != null)
            {
                duck.runMax = 3.1f;
            }
            base.Thrown();
        }
        protected override void PlayFireSound()
        {
            SFX.Play("sniper", 1, Rando.Float(-0.1f, 0.1f));//ERRRRRIIIIIIIIIIK
            base.PlayFireSound();
        }
        public override void Update()
        {
            if (load > 0 && isServerForObject)
            {
                if (owner == null && load > 0.3f)
                {
                    _canRaise = true;
                    load = -1;
                    rotAngle = 0;
                    return;
                }

                mult = Lerp.Float(mult, 1, 0.01f);

                if (load < 0.3f)
                {
                    if (owner != null || tech) //putting this here for funsies :) -NiK0
                    {
                        rotAngle = Lerp.Float(rotAngle, 0, 4 * mult);
                    }
                }
                else
                {
                    tech = owner == null;
                    rotAngle = Lerp.Float(rotAngle, 90, 6 * mult);
                }
                if (owner != null || load < 0.3f) load = Lerp.Float(load, 0, 0.01f * mult);
                if (load <= 0.01f)
                {
                    if (duck != null)
                    {
                        duck.runMax = 3.1f;
                    }
                    _canRaise = true;
                    loaded = true;
                    load = -1;
                    off = new Vec2(-4, 6);
                    ang = 0;
                    _holdOffset = Vec2.Zero;
                    handOffset = Vec2.Zero;
                    ammo--;
                    SFX.Play("loadSniper");
                }
            }
            if (loaded && owner != null)
            {
                handOffset = Vec2.Zero;//online fix
                laserSight = true;
            }
            else laserSight = false;
            base.Update();
        }
        public Sprite armSprite;

        public Vec2 off;
        public float ang;
        public bool lockSniz;
        public override void Draw()
        {
            base.Draw();
            if (load > 0 && duck != null)
            {
                armSprite = duck._spriteArms;
                Depth d = depth + 6;

                if (load < 2.03f)
                {
                    handOffset = new Vec2(0, -99999);
                }
                if (load > 1.9f && load < 2) off = Lerp.Vec2(off, new Vec2(-9, 6), 0.5f * mult);
                else if (load > 1.8f)
                {
                    off = Lerp.Vec2(off, new Vec2(1, 6), 0.5f * mult);
                    d = depth - 1;
                }
                else if (load > 1.7f)
                {
                    off = Lerp.Vec2(off, new Vec2(-9, 5), 0.5f * mult);
                    d = depth - 1;
                }
                else if (load > 1.63f)
                {
                    off = Lerp.Vec2Smooth(off, new Vec2(-7, -8), 0.1f * mult);
                    ang = Lerp.Float(ang, 0.2f, 0.03f * mult);
                }
                else if (load > 1.57f)
                {
                    ang = Lerp.Float(ang, 0.8f, 0.06f * mult);
                    off = Lerp.Vec2Smooth(off, new Vec2(-4, -12), 0.1f * mult);
                }
                else if (load > 1.53f)
                {
                    _holdOffset = Lerp.Vec2Smooth(_holdOffset, new Vec2(0, 6), 0.1f);
                    ang = Lerp.Float(ang, 1.3f, 0.08f * mult);
                    off = Lerp.Vec2Smooth(off, new Vec2(0, -14), 0.1f * mult);
                }
                else if (load > 1.4f)
                {
                    _holdOffset = Lerp.Vec2Smooth(_holdOffset, new Vec2(0, 6), 0.1f);
                    ang = Lerp.Float(ang, 1.8f, 0.08f * mult);
                    off = Lerp.Vec2Smooth(off, new Vec2(2, -12), 0.1f * mult);
                }
                else if (load > 1.1f)
                {
                    ang = Lerp.FloatSmooth(ang, 3.14f, 0.1f * mult);

                    off = Lerp.Vec2Smooth(off, new Vec2(3.5f, -12), 0.1f * mult);
                }
                else if (load > 0.4f)
                {
                    _holdOffset = Lerp.Vec2Smooth(_holdOffset, Vec2.Zero, 0.1f * mult);
                    ang = Lerp.FloatSmooth(ang, 3.14f, 0.1f * mult);

                    off = Lerp.Vec2Smooth(off, new Vec2(3.5f, -7 + (float)Math.Sin(load * 25) * 6), 0.2f * mult);

                    Vec2 v2 = duck.Offset(off);


                    if (off.y > -4) lockSniz = true;
                    armSprite.angle = ang * offDir;
                    if (load < 1.8f)
                    {
                        snizsnos.scale = new Vec2(0.7f);
                        snizsnos.angle = armSprite.angle + 1.57f;
                        if (lockSniz)
                        {
                            Vec2 v3 = duck.Offset(new Vec2(off.x, load * -7 + 1));
                            Graphics.Draw(snizsnos, v3.x + 0.8f * offDir, v3.y, d - 10);
                        }
                        else Graphics.Draw(snizsnos, v2.x + 0.8f * offDir, v2.y, d - 10);
                    }
                    Graphics.Draw(armSprite, v2.x, v2.y, d);
                    return;
                }
                else
                {
                    ang = Lerp.FloatSmooth(ang, 0, 0.15f * mult);
                    off = Lerp.Vec2Smooth(off, new Vec2(-2, 6), 0.15f * mult);
                }

                Vec2 v = duck.Offset(off);

                armSprite.angle = ang * offDir;
                if (load < 1.8f && load > 0.4f)
                {
                    snizsnos.scale = new Vec2(0.7f);
                    snizsnos.angle = armSprite.angle + 1.57f;
                    Graphics.Draw(snizsnos, v.x + 0.8f, v.y, d - 2);
                }
                Graphics.Draw(armSprite, v.x, v.y, d);
                DevConsole.Log("!");
            }
        }
    }
}
