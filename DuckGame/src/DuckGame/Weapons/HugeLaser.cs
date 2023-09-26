// Decompiled with JetBrains decompiler
// Type: DuckGame.HugeLaser
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using Microsoft.Xna.Framework.Audio;

namespace DuckGame
{
    [EditorGroup("Guns|Lasers")]
    public class HugeLaser : Gun
    {
        public StateBinding _laserStateBinding = new HugeLaserFlagBinding();
        public StateBinding _animationIndexBinding = new StateBinding(nameof(netAnimationIndex), 4);
        public StateBinding _frameBinding = new StateBinding(nameof(spriteFrame));
        public StateBinding _chargeVolumeBinding = new CompressedFloatBinding(GhostPriority.High, nameof(_chargeVolume), bits: 8);
        public StateBinding _chargeVolumeShortBinding = new CompressedFloatBinding(GhostPriority.High, nameof(_chargeVolumeShort), bits: 8);
        public StateBinding _unchargeVolumeBinding = new CompressedFloatBinding(GhostPriority.High, nameof(_unchargeVolume), bits: 8);
        public StateBinding _unchargeVolumeShortBinding = new CompressedFloatBinding(GhostPriority.High, nameof(_unchargeVolumeShort), bits: 8);
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

        public HugeLaser(float xval, float yval)
          : base(xval, yval)
        {
            ammo = 30;
            _type = "gun";
            graphic = new Sprite("hugeLaser");
            center = new Vec2(32f, 32f);
            collisionOffset = new Vec2(-16f, -8f);
            collisionSize = new Vec2(32f, 15f);
            _barrelOffsetTL = new Vec2(47f, 30f);
            _fireSound = "";
            _fullAuto = false;
            _fireWait = 1f;
            _kickForce = 1f;
            _fireRumble = RumbleIntensity.Light;
            _editorName = "Death Laser";
            _tip = new Sprite("bigLaserTip");
            _tip.CenterOrigin();
            _chargeAnim = new SpriteMap("laserCharge", 32, 16);
            _chargeAnim.AddAnimation("idle", 1f, true, new int[1]);
            _chargeAnim.AddAnimation("load", 0.05f, false, 0, 1, 2, 3, 4);
            _chargeAnim.AddAnimation("loaded", 1f, true, 5);
            _chargeAnim.AddAnimation("charge", 0.38f, false, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 28);
            _chargeAnim.AddAnimation("uncharge", 1.2f, false, 28, 28, 27, 26, 25, 24, 23, 22, 21, 20, 19, 18, 17, 16, 15, 14, 13, 12, 11, 10, 9, 8, 7, 6);
            _chargeAnim.AddAnimation("drain", 2f, false, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40);
            _chargeAnim.SetAnimation("loaded");
            _chargeAnim.center = new Vec2(16f, 10f);
            _editorName = "Death Ray";
            editorTooltip = "Hold the trigger to charge a beam of pure death and destruction. You know, for kids!";
            _bio = "Invented by Dr.Death for scanning items at your local super market. Also has some military application.";
            shouldbegraphicculled = false;
        }
        public override Holdable BecomeTapedMonster(TapedGun pTaped)
        {
            if (Editor.clientonlycontent)
            {
                return pTaped.gun1 is HugeLaser && pTaped.gun2 is HugeLaser ? new HugerLaser(x, y) : null;
            }
            return base.BecomeTapedMonster(pTaped);
        }
        public override void Initialize()
        {
            _chargeSound = SFX.Get("laserCharge", 0f);
            _chargeSoundShort = SFX.Get("laserChargeShort", 0f);
            _unchargeSound = SFX.Get("laserUncharge", 0f);
            _unchargeSoundShort = SFX.Get("laserUnchargeShort", 0f);
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
            SFX.Play("laserBlast");
        }

        public override void Update()
        {
            if (Network.isActive)
            {
                if (isServerForObject)
                {
                    _chargeVolume = _chargeSound.State == SoundState.Playing ? _chargeSound.Volume : 0f;
                    _chargeVolumeShort = _chargeSoundShort.State == SoundState.Playing ? _chargeSoundShort.Volume : 0f;
                    _unchargeVolume = _unchargeSound.State == SoundState.Playing ? _unchargeSound.Volume : 0f;
                    _unchargeVolumeShort = _unchargeSoundShort.State == SoundState.Playing ? _unchargeSoundShort.Volume : 0f;
                }
                else
                {
                    _chargeSound.Volume = _chargeVolume;
                    _chargeSoundShort.Volume = _chargeVolumeShort;
                    _unchargeSound.Volume = _unchargeVolume;
                    _unchargeSoundShort.Volume = _unchargeVolumeShort;
                    if (_chargeVolume > 0 && _chargeSound.State != SoundState.Playing)
                        _chargeSound.Play();
                    else if (_chargeVolume <= 0)
                        _chargeSound.Stop();
                    if (_chargeVolumeShort > 0 && _chargeSoundShort.State != SoundState.Playing)
                        _chargeSoundShort.Play();
                    else if (_chargeVolumeShort <= 0)
                        _chargeSoundShort.Stop();
                    if (_unchargeVolume > 0 && _unchargeSound.State != SoundState.Playing)
                        _unchargeSound.Play();
                    else if (_unchargeVolume <= 0)
                        _unchargeSound.Stop();
                    if (_unchargeVolumeShort > 0 && _unchargeSoundShort.State != SoundState.Playing)
                        _unchargeSoundShort.Play();
                    else if (_unchargeVolumeShort <= 0)
                        _unchargeSoundShort.Stop();
                }
            }
            base.Update();
            if (_charge > 0)
                _charge -= 0.1f;
            else
                _charge = 0f;
            if (_chargeAnim.currentAnimation == "uncharge" && _chargeAnim.finished)
                _chargeAnim.SetAnimation("loaded");
            if (_chargeAnim.currentAnimation == "charge" && _chargeAnim.finished && isServerForObject)
            {
                PostFireLogic();
                if (!Recorderator.Playing)
                {
                    if (this.owner is Duck owner)
                    {
                        RumbleManager.AddRumbleEvent(owner.profile, new RumbleEvent(RumbleIntensity.Medium, RumbleDuration.Pulse, RumbleFalloff.Short));
                        owner.sliding = true;
                        owner.crouch = true;
                        Vec2 vec2 = barrelVector * 9f;
                        if (owner.ragdoll != null && owner.ragdoll.part2 != null && owner.ragdoll.part1 != null && owner.ragdoll.part3 != null)
                        {
                            owner.ragdoll.part2.hSpeed -= vec2.x;
                            owner.ragdoll.part2.vSpeed -= vec2.y;
                            owner.ragdoll.part1.hSpeed -= vec2.x;
                            owner.ragdoll.part1.vSpeed -= vec2.y;
                            owner.ragdoll.part3.hSpeed -= vec2.x;
                            owner.ragdoll.part3.vSpeed -= vec2.y;
                        }
                        else
                        {
                            owner.hSpeed -= vec2.x;
                            owner.vSpeed -= vec2.y + 3f;
                            owner.CancelFlapping();
                        }
                    }
                    else
                    {
                        Vec2 barrelVector = this.barrelVector;
                        hSpeed -= barrelVector.x * 9f;
                        vSpeed -= (float)(barrelVector.y * 9 + 3);
                    }
                    Vec2 vec2_1 = Offset(barrelOffset);
                    Vec2 vec2_2 = Offset(barrelOffset + new Vec2(1200f, 0f)) - vec2_1;
                    if (isServerForObject)
                        ++Global.data.laserBulletsFired.valueInt;
                    if (Network.isActive)
                        Send.Message(new NMDeathBeam(this, vec2_1, vec2_2));
                    DeathBeam deathBeam = new DeathBeam(vec2_1, vec2_2, this.owner)
                    {
                        isLocal = isServerForObject
                    };
                    Level.Add(deathBeam);
                    doBlast = true;
                }
            }
            if (doBlast && isServerForObject)
            {
                ++_framesSinceBlast;
                if (_framesSinceBlast > 10)
                {
                    _framesSinceBlast = 0;
                    doBlast = false;
                }
            }
            if (_chargeAnim.currentAnimation == "drain" && _chargeAnim.finished)
                _chargeAnim.SetAnimation("loaded");
            _lastDoBlast = doBlast;
        }

        public override void Draw()
        {
            base.Draw();
            Material material = Graphics.material;
            Graphics.material = this.material;
            _tip.depth = depth + 1;
            _tip.alpha = _charge;
            if (_chargeAnim.currentAnimation == "charge")
                _tip.alpha = _chargeAnim.frame / 24f;
            else if (_chargeAnim.currentAnimation == "uncharge")
                _tip.alpha = (24 - _chargeAnim.frame) / 24f;
            else
                _tip.alpha = 0f;
            Graphics.Draw(_tip, barrelPosition.x, barrelPosition.y);
            _chargeAnim.flipH = graphic.flipH;
            _chargeAnim.depth = depth + 1;
            _chargeAnim.angle = angle;
            _chargeAnim.alpha = alpha;
            Graphics.Draw(_chargeAnim, x, y);
            Graphics.material = material;
            float num1 = Maths.NormalizeSection(_tip.alpha, 0f, 0.7f);
            float num2 = Maths.NormalizeSection(_tip.alpha, 0.6f, 1f);
            float num3 = Maths.NormalizeSection(_tip.alpha, 0.75f, 1f);
            float num4 = Maths.NormalizeSection(_tip.alpha, 0.9f, 1f);
            float num5 = Maths.NormalizeSection(_tip.alpha, 0.8f, 1f) * 0.5f;
            if (num1 <= 0)
                return;
            Vec2 p1 = Offset(barrelOffset);
            Vec2 p2 = Offset(barrelOffset + new Vec2(num1 * 1200f, 0f));
            Graphics.DrawLine(p1, p2, new Color((_tip.alpha * 0.7f + 0.3f), _tip.alpha, _tip.alpha) * (0.3f + num5), (1f + num2 * 12f));
            Graphics.DrawLine(p1, p2, Color.Red * (0.2f + num5), (1f + num3 * 28f));
            Graphics.DrawLine(p1, p2, Color.Red * (0.1f + num5), (0.2f + num4 * 40f));
        }

        public override void OnPressAction()
        {
            if (_chargeAnim == null || _chargeSound == null)
                return;
            if (_chargeAnim.currentAnimation == "loaded")
            {
                _chargeAnim.SetAnimation("charge");
                if (!isServerForObject)
                    return;
                _chargeSound.Volume = 1f;
                _chargeSound.Play();
                _unchargeSound.Stop();
                _unchargeSound.Volume = 0f;
                _unchargeSoundShort.Stop();
                _unchargeSoundShort.Volume = 0f;
            }
            else
            {
                if (!(_chargeAnim.currentAnimation == "uncharge"))
                    return;
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
                if (!isServerForObject)
                    return;
                _unchargeSound.Stop();
                _unchargeSound.Volume = 0f;
                _unchargeSoundShort.Stop();
                _unchargeSoundShort.Volume = 0f;
            }
        }

        public override void OnHoldAction()
        {
        }

        public override void OnReleaseAction()
        {
            if (!(_chargeAnim.currentAnimation == "charge"))
                return;
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
            if (!isServerForObject)
                return;
            _chargeSound.Stop();
            _chargeSound.Volume = 0f;
            _chargeSoundShort.Stop();
            _chargeSoundShort.Volume = 0f;
        }
    }
}
