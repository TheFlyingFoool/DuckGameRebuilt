using Microsoft.Xna.Framework.Audio;
using System;

namespace DuckGame
{
    //[EditorGroup("Guns|Lasers")]
    [ClientOnly]
    public class Propaganda : Gun
    {
        public StateBinding _animationIndexBinding = new StateBinding(nameof(netAnimationIndex), 4);
        public StateBinding _frameBinding = new StateBinding(nameof(spriteFrame));
        public StateBinding _chargeBinding = new StateBinding(nameof(charge));
        public StateBinding _cooldownBinding = new StateBinding(nameof(cooldown));
        public StateBinding _chargingBinding = new StateBinding(nameof(charging));
        public StateBinding _blastingBinding = new StateBinding(nameof(blasting));
        public StateBinding _lockedAngleBinding = new StateBinding(nameof(lockedAngle));

        public byte netAnimationIndex
        {
            get => _chargeAnim == null ? (byte)0 : (byte)_chargeAnim.animationIndex;
            set
            {
                if (_chargeAnim == null || _chargeAnim.animationIndex == value)
                    return;
                _chargeAnim.animationIndex = value;
            }
        }

        public byte spriteFrame
        {
            get => _chargeAnim == null ? (byte)0 : (byte)_chargeAnim._frame;
            set
            {
                if (_chargeAnim == null)
                    return;
                _chargeAnim._frame = value;
            }
        }

        public SpriteMap _chargeAnim;
        public Sprite _tip;
        public Sprite _flash;
        private float _charge = 0;
        public float charge { get => _charge; 
            set 
            {
                if ((value >= 2f) && (_charge < 2f) && (_sparkSound.State != SoundState.Playing))
                    _sparkSound.Play();
                _charge = value;
            } 
        }
        public float cooldown = 0;
        public float lockedAngle = 0;
        public int lightPos => (int)((4f - charge) * 400);
        public bool charging = false;
        public bool blasting = false;

        private Sound _chargeSound;
        private Sound _unchargeSound;
        private Sound _sparkSound;

        public Propaganda(float xval, float yval)
          : base(xval, yval)
        {
            ammo = 30;
            _type = "gun";
            graphic = new Sprite("propaganda");
            _chargeAnim = new SpriteMap("propagandaLaserAnim", 17, 8);
            _chargeAnim.center = new Vec2(18f, 6f);
            _chargeAnim.AddAnimation("idle", 0, false, 0);
            _chargeAnim.AddAnimation("charge", 0.25f, false, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16);
            _chargeAnim.AddAnimation("uncharge", 0.5f, false, 16, 15, 14, 13, 12, 11, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1, 0);
            _chargeAnim.AddAnimation("blasting", 0.75f, false, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29);
            _chargeAnim.SetAnimation("nothing");
            _tip = new Sprite("propagandaTip");
            _tip.center = new Vec2(-16f, 4f);
            _flash = new Sprite("holySpark");
            _flash.center = new Vec2(5f, 6f);
            center = new Vec2(20f, 9f);
            collisionOffset = new Vec2(-16f, -8f);
            collisionSize = new Vec2(32f, 15f);
            _holdOffset = new Vec2(3f, -1f);
            _barrelOffsetTL = new Vec2(38f, 7f);
            _fireSound = "";
            _fullAuto = false;
            _fireWait = 1f;
            _kickForce = 5f;
            _fireRumble = RumbleIntensity.Light;
        }

        public override void Initialize()
        {
            _chargeSound = SFX.Get("propagandaCharge", 0f);
            _unchargeSound = SFX.Get("propagandaDischarge", 0f);
            _sparkSound = SFX.Get("propagandaSpark");
            base.Initialize();
        }

        public override void Update()
        {
            _flash.scale = new Vec2(MathHelper.Lerp(_flash.scale.x, 1f, 0.05f), MathHelper.Lerp(_flash.scale.y, 1f, 0.05f));
            _flash.angle += 0.03f;
            if (blasting)
            {
                this._flipHorizontal = false;
                angle = lockedAngle;
            }
            if (isServerForObject)
            {
                if (!blasting)
                {
                    if (cooldown > 0)
                    {
                        cooldown -= 0.05f;
                        if (cooldown <= 0)
                        {
                            cooldown = 0;
                            if (charging)
                            {
                                _chargeAnim.SetAnimation("charge");
                                _chargeSound.Volume = 1f;
                                _chargeSound.Play();
                                _unchargeSound.Stop();
                                _unchargeSound.Volume = 0f;
                            }
                        }
                    }
                    else
                    {
                        if (_chargeAnim.currentAnimation == "charge" && _chargeAnim.frame == 16 && this.charging)
                        {
                            charge = Math.Min(4f, charge + 0.05f);
                        }
                        else
                        {
                            charge = Math.Max(0f, charge - 0.2f);
                        }
                        if (charge >= 4f)
                        {
                            blasting = true;
                            _chargeAnim.SetAnimation("blasting");
                            lockedAngle = angle;
                            _flash.scale = Vec2.One * 3f;
                            SFX.PlaySynchronized("propagandaShoot");
                            this.ApplyKick();
                            _chargeSound.Stop();
                            _chargeSound.Volume = 0f;
                        }
                    }
                }
                else
                {
                    if (charge > 2f)
                    {
                        Vec2 point = barrelPosition;
                        Vec2 vec = barrelVector * lightPos;
                        for (float i = -1f; i <= 1f; i += 0.5f)
                        {
                            Vec2 rotVec = vec.Rotate(i * (charge / 4f) * (float)Math.PI / 40f, Vec2.Zero);
                            foreach (IAmADuck target in Level.CheckLineAll<IAmADuck>(point, point + rotVec))
                            {
                                Duck targetDuck = null;
                                if (target is Duck) targetDuck = (Duck)target;
                                if (target is Ragdoll) targetDuck = ((Ragdoll)target)._duck;
                                if (targetDuck != null)
                                {
                                    if (targetDuck.team != duck.team)
                                    {
                                        targetDuck.ConvertDuck(duck);
                                        if (Network.isActive)
                                        {
                                            Send.Message(new NMConversion(targetDuck, duck));
                                        }
                                        targetDuck.conversionResistance = 50;
                                    }
                                }
                            }
                        }
                    }
                    charge = Math.Max(0f, charge - 0.1f);
                    if (charge <= 0f)
                    {
                        blasting = false;
                        cooldown = 1f;
                        _chargeAnim.SetAnimation("idle");
                        _chargeAnim.alpha = alpha;
                    }
                }
            }
            if (!isServerForObject && !action && _chargeSound.State == SoundState.Playing)
            {
                _unchargeSound.Volume = 1f;
                _unchargeSound.Play();
                _chargeSound.Stop();
                _chargeSound.Volume = 0f;
            }
                base.Update();
        }

        public override void OnPressAction()
        {
            if (cooldown <= 0 && !blasting)
            {
                charging = true;

                _chargeSound.Volume = 1f;
                _chargeSound.Play();
                _unchargeSound.Stop();
                _unchargeSound.Volume = 0f;

                if (_chargeAnim.currentAnimation == "uncharge")
                {
                    int frame = _chargeAnim.frame;
                    _chargeAnim.SetAnimation("charge");
                    _chargeAnim.frame = 16 - frame;
                }
                else
                {
                    _chargeAnim.SetAnimation("charge");
                }
            }
        }

        public override void OnHoldAction()
        {

        }

        public override void OnReleaseAction()
        {
            charging = false;

            if (!blasting)
            {
                _unchargeSound.Volume = 1f;
                _unchargeSound.Play();
                _chargeSound.Stop();
                _chargeSound.Volume = 0f;

                if (_chargeAnim.currentAnimation == "charge")
                {
                    int frame = _chargeAnim.frame;
                    _chargeAnim.SetAnimation("uncharge");
                    _chargeAnim.frame = 16 - frame;
                }
            }
        }

        public override void Draw()
        {
            if (blasting) angle = lockedAngle;

            _chargeAnim.flipH = graphic.flipH;
            _chargeAnim.depth = depth + 1;
            _chargeAnim.angle = angle;
            //_chargeAnim.alpha = alpha * (blasting ? (charge / 4f) : 1f);
            _chargeAnim.alpha = alpha;
            Graphics.Draw(_chargeAnim, x, y);

            _tip.flipH = graphic.flipH;
            _tip.depth = depth + 1;
            _tip.angle = angle;
            _tip.alpha = alpha * Maths.NormalizeSection(charge, 0f, 2f);
            Graphics.Draw(_tip, x, y);

            _flash.depth = depth + 2;
            _flash.alpha = alpha * (blasting ? 1f : Math.Max(Maths.NormalizeSection(charge, 2f, 4f), cooldown));
            Graphics.Draw(_flash, barrelPosition.x, barrelPosition.y);



            if (charge > 0 || cooldown > 0)
            {
                float mult = Maths.NormalizeSection(charge + 1f, 1f, 4f);
                float colorMult = Maths.NormalizeSection(charge, 0f, 4f);

                Vec2 p1 = barrelPosition;
                Vec2 p1ButFurther = barrelPosition + (barrelVector * 32);
                Vec2 p2 = barrelPosition + (barrelVector * lightPos);

                if (blasting || cooldown > 0)
                {
                    GeometryItem g = new GeometryItem();

                    Vec2 up = barrelVector.Rotate(1.57f, Vec2.Zero);

                    float umult = mult * 30f;

                    g.AddTriangle(p1, p2 + up * umult, p2 - up * umult, new Color(247, 224, 90) * colorMult);
                    umult *= 0.5f;
                    g.AddTriangle(p1ButFurther, p2 + up * umult, p2 - up * umult, Color.White * colorMult);

                    Graphics.screen.SubmitGeometry(g);
                }
            }

            base.Draw();
        }
    }
}



