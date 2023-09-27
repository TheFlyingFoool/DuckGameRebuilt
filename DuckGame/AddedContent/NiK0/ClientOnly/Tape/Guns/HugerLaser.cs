using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DuckGame
{
    [ClientOnly]
    public class HugerLaser : Gun
    {
        public SpriteMap _chargeAnim;
        public HugerLaser(float xpos, float ypos) : base(xpos, ypos)
        {
            ammo = 99;
            graphic = new Sprite("hugerLaser");
            _chargeAnim = new SpriteMap("hugerLaserAnim", 40, 24);
            //0-42
            _chargeAnim.AddAnimation("nothing", 0, false, 0);
            _chargeAnim.SetAnimation("nothing");
            _chargeAnim.AddAnimation("charge", 0.25f, false, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42);
            _chargeAnim.center = new Vec2(20f, 11.5f);
            _collisionSize = new Vec2(34, 16);
            _collisionOffset = new Vec2(-14, -7.5f);
            center = new Vec2(20f, 12.5f);
            _holdOffset = new Vec2(2, -4);
           
            _barrelOffsetTL = new Vec2(40, 12.5f);
            _kickForce = 20;
            tapeable = false;
        }
        public override void Initialize()
        {
            base.Initialize();
        }
        public float time;

        public List<Duck> adjustingS = new List<Duck>();
        public StateBinding _timeBinding = new StateBinding("time");
        public override void Update()
        {
            for (int i = 0; i < adjustingS.Count; i++)
            {
                Duck d = adjustingS[i];
                if (Math.Abs(d.hSpeed) <= 12)
                {
                    d.hMax = 12;
                }
                if (Math.Abs(d.vSpeed) <= 8)
                {
                    d.vMax = 8;
                }
                if (d.hMax == 12 && d.vMax == 8)
                {
                    adjustingS.RemoveAt(i);
                }
            }
            if (!isServerForObject && time > 0 && _chargeAnim.currentAnimation != "charge") _chargeAnim.SetAnimation("charge");
            if (_chargeAnim.currentAnimation == "charge" && isServerForObject)
            {
                time++;
                if (_chargeAnim.finished)
                {
                    if (duck != null)
                        RumbleManager.AddRumbleEvent(duck.profile, new RumbleEvent(RumbleIntensity.Heavy, RumbleDuration.Long, RumbleFalloff.Medium));
                    time = 0;
                    SFX.PlaySynchronized("laserBlast", 1, 0.15f);
                    SFX.PlaySynchronized("balloonPop", 1.5f, -4.3f);
                    _chargeAnim.SetAnimation("nothing");
                    Send.Message(new NMDeathCone(this, barrelPosition, barrelVector));
                    Level.Add(new LaserConeBlast(barrelPosition.x, barrelPosition.y, barrelVector) { angle = angle });
                    if (duck != null)
                    {
                        duck.vMax = 16;
                        duck.hMax = 24;
                        adjustingS.Add(duck);
                    }
                    ApplyKick();
                }
            }

            if (!_triggerHeld)
            {
                if (s != null && s.State == Microsoft.Xna.Framework.Audio.SoundState.Playing)
                {
                    s.Stop();
                }
                _chargeAnim.SetAnimation("nothing");
            }
            base.Update();
        }
        public override void Draw()
        {
            _chargeAnim.flipH = graphic.flipH;
            _chargeAnim.depth = depth + 1;
            _chargeAnim.angle = angle;
            _chargeAnim.alpha = alpha;
            Graphics.Draw(_chargeAnim, x, y);

            if (_chargeAnim.currentAnimation == "charge")
            {
                float mult = time / 173f;


                float num2 = Maths.NormalizeSection(mult, 0.6f, 1f);
                float num3 = Maths.NormalizeSection(mult, 0.75f, 1f);
                float num4 = Maths.NormalizeSection(mult, 0.9f, 1f);
                float num5 = Maths.NormalizeSection(mult, 0.8f, 1f) * 0.5f;

                Vec2 p1 = barrelPosition;
                Vec2 p2 = barrelPosition + barrelVector * 1500;
                if (num2 > 0)
                {
                    GeometryItem g = new GeometryItem();

                    float umult = num2 * 120f;

                    Vec2 up = barrelVector.Rotate(1.57f, Vec2.Zero);

                    g.AddTriangle(p1, p2 + up * umult, p2 - up * umult, Color.Red * (0.2f + num5));

                    umult *= 0.75f;
                    g.AddTriangle(p1, p2 + up * umult, p2 - up * umult, Color.Red * (0.1f + num5));

                    umult *= 0.5f;
                    g.AddTriangle(p1, p2 + up * umult, p2 - up * umult, new Color((mult * 0.7f + 0.3f), mult, mult) * (0.3f + num5));

                    umult *= 0.25f;
                    g.AddTriangle(p1, p2 + up * umult, p2 - up * umult, new Color((mult * 0.7f + 0.3f), mult, mult) * (0.3f + num5));
                    Graphics.DrawLine(p1, p2, Color.Red * (0.1f + num5), 0.2f);
                    Graphics.DrawLine(p1, p2, new Color((mult * 0.7f + 0.3f), mult, mult) * 0.2f, 1);

                    Graphics.screen.SubmitGeometry(g);
                }
                else
                {
                    Graphics.DrawLine(p1, p2, new Color((mult * 0.7f + 0.3f), mult, mult) * (0.3f + num5), (1f + num2 * 12f));
                    Graphics.DrawLine(p1, p2, Color.Red * (0.2f + num5), (1f + num3 * 28f));
                    Graphics.DrawLine(p1, p2, Color.Red * (0.1f + num5), (0.2f + num4 * 40f));
                }
            }
            base.Draw();
        }

        public Sound s;
        public override void OnPressAction()
        {
            s = SFX.Play("Audio/SFX/DeltaLaserCharge", 1);
            _chargeAnim.SetAnimation("charge");
        }
        public override void OnHoldAction()
        {
        }
        public override void OnReleaseAction()
        {
            time = 0;
            _chargeAnim.SetAnimation("nothing");
        }
    }
}
