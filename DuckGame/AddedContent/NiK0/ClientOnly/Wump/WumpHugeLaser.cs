using Microsoft.Xna.Framework.Audio;

namespace DuckGame
{
    [ClientOnly]
    [EditorGroup("Rebuilt|Wump|Lasers")]
    public class WumpHugeLaser : Gun, IDrawToDifferentLayers
    {
        public byte netAnimationIndex
        {
            get
            {
                if (_chargeAnim == null)
                {
                    return 0;
                }
                return (byte)_chargeAnim.animationIndex;
            }
            set
            {
                if (_chargeAnim != null && _chargeAnim.animationIndex != value)
                {
                    _chargeAnim.animationIndex = value;
                }
            }
        }

        public byte spriteFrame
        {
            get
            {
                if (_chargeAnim == null)
                {
                    return 0;
                }
                return (byte)_chargeAnim._frame;
            }
            set
            {
                if (_chargeAnim != null)
                {
                    _chargeAnim._frame = value;
                }
            }
        }
        public WumpHugeLaser(float xval, float yval) : base(xval, yval)
        {
            ammo = 99;
            _type = "gun";
            graphic = new Sprite("wumphugelaser");
            Content.textures[graphic.Namebase] = graphic.texture;
            center = new Vec2(17.5f, 12f);
            collisionOffset = new Vec2(-16f, -4f);
            collisionSize = new Vec2(32f, 15f);
            _barrelOffsetTL = new Vec2(33, 13);
            _fireSound = "";
            _fullAuto = false;
            _fireWait = 1f;
            _kickForce = 0.4f;
            _holdOffset = new Vec2(1, -4);
            _fireRumble = RumbleIntensity.Light;
            _editorName = "Death Laser";
            _tip = new Sprite("bigLaserTip", 0f, 0f);
            _tip.CenterOrigin();
            _chargeAnim = new SpriteMap("wumphugelasercharge", 35, 24);
            _chargeAnim.AddAnimation("idle", 1f, true, new int[1]);
            _chargeAnim.AddAnimation("load", 0.05f, false, new int[]
            {
                0,
                1,
                2,
                3,
                4
            });
            _chargeAnim.AddAnimation("loaded", 1f, true, new int[]
            {
                0
            });
            _chargeAnim.AddAnimation("charge", 0.2f, false, new int[]
            {
                0,
                1,
                3,
                4,
                5,
                6,
                7,
                8,
                9,
                10,
                11,
                12,
                13,
                14,
                15,
                16,
                17,
                18,
                19,
                20,
                21,
                22,
                23,
                24,
                25,
                26
            });
            _chargeAnim.AddAnimation("uncharge", 1.2f, false, new int[]
            {
                20,
                19,
                18,
                17,
                16,
                15,
                14,
                13,
                12,
                11,
                10,
                9,
                8,
                7,
                6
            });
            _chargeAnim.AddAnimation("drain", 2f, false, new int[]
            {
                20,19,18,17,16,15,14,13,12,11,10,9,8,7,6,5,4,3,1
            });
            _chargeAnim.SetAnimation("loaded");
            _chargeAnim.center = new Vec2(17.5f, 12f);
            _editorName = "Death Ray";
            editorTooltip = "Hold the trigger to charge a beam of pure death and destruction. You know, for kids!";
            _bio = "Invented by Dr.Death for scanning items at your local super market. Also has some military application.";
            weight = 4;
        }
        public bool uncharge;
        public override void Initialize()
        {
            _chargeSound = SFX.Get("chaingunSpinUp", 0f, -2);
            _chargeSoundShort = SFX.Get("chaingunSpinUp", 0f, -2);
            _unchargeSound = SFX.Get("chaingunSpinDown", 0f, -2);
            _unchargeSoundShort = SFX.Get("chaingunSpinDown", 0, -2);
        } 

        public void PostFireLogic()
        {
            if (isServerForObject)
            {
                _unchargeSound.Stop();
                _unchargeSound.Volume = 0f;
                _unchargeSoundShort.Stop();
                _unchargeSoundShort.Volume = 0f;
                _chargeSound.Stop();
                _chargeSound.Volume = 0f;
                _chargeSoundShort.Stop();
                _chargeSoundShort.Volume = 0f;
            }
            _chargeAnim.SetAnimation("drain");
            SFX.PlaySynchronized("laserBlast", 1f, 0f, 0f, false);
        }
        public override void Update()
        {
            //dont question this if statement
            if (prevOwner != null && prevOwner.isServerForObject && prevOwner is Duck d && d.offdirLocked && (d.holdObject == null || (d.holdObject is not WumpHugeLaser wh || wh.weight < 4))) d.offdirLocked = false;

            if (weight > 4)
            {
                raiseSpeed = 0.001f;
                if (duck != null) duck.offdirLocked = true;
            }
            else
            {
                raiseSpeed = 0.15f;
                if (duck != null) duck.offdirLocked = false;
            }
            if (Network.isActive)
            {
                if (isServerForObject)
                {
                    _chargeVolume = (_chargeSound.State == SoundState.Playing) ? _chargeSound.Volume : 0f;
                    _chargeVolumeShort = (_chargeSoundShort.State == SoundState.Playing) ? _chargeSoundShort.Volume : 0f;
                    _unchargeVolume = (_unchargeSound.State == SoundState.Playing) ? _unchargeSound.Volume : 0f;
                    _unchargeVolumeShort = (_unchargeSoundShort.State == SoundState.Playing) ? _unchargeSoundShort.Volume : 0f;
                }
                else
                {
                    _chargeSound.Volume = _chargeVolume;
                    _chargeSoundShort.Volume = _chargeVolumeShort;
                    _unchargeSound.Volume = _unchargeVolume;
                    _unchargeSoundShort.Volume = _unchargeVolumeShort;
                    if (_chargeVolume > 0f && _chargeSound.State != SoundState.Playing)
                    {
                        _chargeSound.Play();
                    }
                    else if (_chargeVolume <= 0f)
                    {
                        _chargeSound.Stop();
                    }
                    if (_chargeVolumeShort > 0f && _chargeSoundShort.State != SoundState.Playing)
                    {
                        _chargeSoundShort.Play();
                    }
                    else if (_chargeVolumeShort <= 0f)
                    {
                        _chargeSoundShort.Stop();
                    }
                    if (_unchargeVolume > 0f && _unchargeSound.State != SoundState.Playing)
                    {
                        _unchargeSound.Play();
                    }
                    else if (_unchargeVolume <= 0f)
                    {
                        _unchargeSound.Stop();
                    }
                    if (_unchargeVolumeShort > 0f && _unchargeSoundShort.State != SoundState.Playing)
                    {
                        _unchargeSoundShort.Play();
                    }
                    else if (_unchargeVolumeShort <= 0f)
                    {
                        _unchargeSoundShort.Stop();
                    }
                }
            }
            base.Update();
            if (_charge > 0f)
            {
                _charge -= 0.1f;
            }
            else
            {
                _charge = 0f;
            }
            if (_chargeAnim.currentAnimation == "uncharge" && _chargeAnim.finished)
            {
                _chargeAnim.SetAnimation("loaded");
            }
            if (_chargeAnim.currentAnimation == "charge" && _chargeAnim.finished && isServerForObject)
            {
                PostFireLogic();
                Duck duck = owner as Duck;
                if (duck != null)
                {
                    RumbleManager.AddRumbleEvent(duck.profile, new RumbleEvent(RumbleIntensity.Medium, RumbleDuration.Pulse, RumbleFalloff.Short, RumbleType.Gameplay));
                    duck.sliding = true;
                    duck.crouch = true;
                    Vec2 vec = barrelVector * 9f;
                    if (duck.ragdoll != null && duck.ragdoll.part2 != null && duck.ragdoll.part1 != null && duck.ragdoll.part3 != null)
                    {
                        duck.ragdoll.part2.hSpeed -= vec.x;
                        duck.ragdoll.part2.vSpeed -= vec.y;
                        duck.ragdoll.part1.hSpeed -= vec.x;
                        duck.ragdoll.part1.vSpeed -= vec.y;
                        duck.ragdoll.part3.hSpeed -= vec.x;
                        duck.ragdoll.part3.vSpeed -= vec.y;
                    }
                    else
                    {
                        duck.hSpeed -= vec.x;
                        duck.vSpeed -= vec.y + 3f;
                        duck.CancelFlapping();
                    }
                }
                else
                {
                    Vec2 barrelVector = base.barrelVector;
                    hSpeed -= barrelVector.x * 9f;
                    vSpeed -= barrelVector.y * 9f + 3f;
                }
                Vec2 vec2 = Offset(barrelOffset);
                Vec2 vec3 = Offset(barrelOffset + new Vec2(1200f, 0f)) - vec2;
                if (isServerForObject)
                {
                    StatBinding laserBulletsFired = Global.data.laserBulletsFired;
                    int valueInt = laserBulletsFired.valueInt;
                    laserBulletsFired.valueInt = valueInt + 1;
                }
                Level.Add(new WumpBeam(vec2, vec3, this)
                {
                    isLocal = isServerForObject
                });
                doBlast = true;
            }
            if (doBlast && isServerForObject)
            {
                _framesSinceBlast++;
                if (_framesSinceBlast > 10)
                {
                    _framesSinceBlast = 0;
                    doBlast = false;
                }
            }
            if (_chargeAnim.currentAnimation == "drain" && _chargeAnim.finished)
            {
                _chargeAnim.SetAnimation("loaded");
            }
            _lastDoBlast = doBlast;
        }

        public override void Draw()
        {
            base.Draw();
            Material material = Graphics.material;
            Graphics.material = base.material;
            _tip.depth = depth + 1;
            _tip.alpha = _charge;
            if (_chargeAnim.currentAnimation == "charge")
            {
                _tip.alpha = _chargeAnim.frame / 27f;
            }
            else if (_chargeAnim.currentAnimation == "uncharge")
            {
                _tip.alpha = (24 - _chargeAnim.frame) / 27f;
            }
            else
            {
                _tip.alpha = 0f;
            }
            Graphics.Draw(_tip, barrelPosition.x, barrelPosition.y);
            _chargeAnim.flipH = graphic.flipH;
            _chargeAnim.depth = depth + 1;
            _chargeAnim.angle = angle;
            _chargeAnim.alpha = alpha;
            Graphics.Draw(_chargeAnim, x, y);
            Graphics.material = material;
            
        }
        public void OnDrawLayer(Layer l)
        {
            if (l == Layer.Foreground && visible) 
            {
                float num = Maths.NormalizeSection(_tip.alpha, 0f, 0.7f);
                float num2 = Maths.NormalizeSection(_tip.alpha, 0.6f, 1f);
                float num3 = Maths.NormalizeSection(_tip.alpha, 0.75f, 1f);
                float num4 = Maths.NormalizeSection(_tip.alpha, 0.9f, 1f);
                float num5 = Maths.NormalizeSection(_tip.alpha, 0.8f, 1f) * 0.5f;
                if (num > 0f)
                {
                    Vec2 p = Offset(barrelOffset);
                    Vec2 p2 = Offset(barrelOffset + new Vec2(num * 1200f, 0f));
                    Graphics.DrawLine(p, p2, new Color(_tip.alpha * 0.7f + 0.3f, _tip.alpha, _tip.alpha) * (0.3f + num5), 1f + num2 * 12f, default);
                    Graphics.DrawLine(p, p2, Color.LightBlue * (0.2f + num5), 1f + num3 * 28f, default);
                    Graphics.DrawLine(p, p2, Color.LightBlue * (0.1f + num5), 0.2f + num4 * 40f, default);
                }
            }
        }

        public override void OnPressAction()
        {
            if (_chargeAnim == null || _chargeSound == null || uncharge)
            {
                return;
            }
            if (_chargeAnim.currentAnimation == "loaded")
            {
                _chargeAnim.SetAnimation("charge");
                if (isServerForObject)
                {
                    _chargeSound.Volume = 1f;
                    _chargeSound.Play();
                    _unchargeSound.Stop();
                    _unchargeSound.Volume = 0f;
                    _unchargeSoundShort.Stop();
                    _unchargeSoundShort.Volume = 0f;
                    return;
                }
            }
            else if (_chargeAnim.currentAnimation == "uncharge")
            {
                if (isServerForObject)
                {
                    if (_chargeAnim.frame > 18)
                    {
                        _chargeSound.Volume = 1f;
                        _chargeSound.Play();
                    }
                    else
                    {
                        _chargeSoundShort.Volume = 1f;
                        _chargeSoundShort.Play();
                    }
                }
                int frame = _chargeAnim.frame;
                _chargeAnim.SetAnimation("charge");
                _chargeAnim.frame = 22 - frame;
                if (isServerForObject)
                {
                    _unchargeSound.Stop();
                    _unchargeSound.Volume = 0f;
                    _unchargeSoundShort.Stop();
                    _unchargeSoundShort.Volume = 0f;
                }
            }
        }

        public override void OnHoldAction()
        {
        }

        public override void OnReleaseAction()
        {
            if (_chargeAnim.currentAnimation == "charge")
            {
                if (isServerForObject)
                {
                    if (_chargeAnim.frame > 20)
                    {
                        _unchargeSound.Stop();
                        _unchargeSound.Volume = 1f;
                        _unchargeSound.Play();
                    }
                    else
                    {
                        _unchargeSoundShort.Stop();
                        _unchargeSoundShort.Volume = 1f;
                        _unchargeSoundShort.Play();
                    }
                }
                int frame = _chargeAnim.frame;
                _chargeAnim.SetAnimation("uncharge");
                _chargeAnim.frame = 22 - frame;
                if (isServerForObject)
                {
                    _chargeSound.Stop();
                    _chargeSound.Volume = 0f;
                    _chargeSoundShort.Stop();
                    _chargeSoundShort.Volume = 0f;
                }
            }
        }

        //	public StateBinding _laserStateBinding = new HugeLaserFlagBinding(GhostPriority.Normal);

        public StateBinding _animationIndexBinding = new StateBinding("netAnimationIndex", 4, false, false);

        public StateBinding _frameBinding = new StateBinding("spriteFrame", -1, false, false);

        public StateBinding _chargeVolumeBinding = new CompressedFloatBinding(GhostPriority.High, "_chargeVolume", 1f, 8, false, false);

        public StateBinding _chargeVolumeShortBinding = new CompressedFloatBinding(GhostPriority.High, "_chargeVolumeShort", 1f, 8, false, false);

        public StateBinding _unchargeVolumeBinding = new CompressedFloatBinding(GhostPriority.High, "_unchargeVolume", 1f, 8, false, false);

        public StateBinding _unchargeVolumeShortBinding = new CompressedFloatBinding(GhostPriority.High, "_unchargeVolumeShort", 1f, 8, false, false);

        private float _chargeVolume;

        private float _chargeVolumeShort;

        private float _unchargeVolume;

        private float _unchargeVolumeShort;

        public bool doBlast;

        private bool _lastDoBlast;

        private Sprite _tip;

        private float _charge;

        public bool _charging;

        public bool _fired;

        private SpriteMap _chargeAnim;

        private Sound _chargeSound;

        private Sound _chargeSoundShort;

        private Sound _unchargeSound;

        private Sound _unchargeSoundShort;

        private int _framesSinceBlast;
    }
}
