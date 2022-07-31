// Decompiled with JetBrains decompiler
// Type: DuckGame.HugeLaser
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
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
            get => this._chargeAnim == null ? (byte)0 : (byte)this._chargeAnim.animationIndex;
            set
            {
                if (this._chargeAnim == null || this._chargeAnim.animationIndex == value)
                    return;
                this._chargeAnim.animationIndex = value;
            }
        }

        public byte spriteFrame
        {
            get => this._chargeAnim == null ? (byte)0 : (byte)this._chargeAnim._frame;
            set
            {
                if (this._chargeAnim == null)
                    return;
                this._chargeAnim._frame = value;
            }
        }

        public HugeLaser(float xval, float yval)
          : base(xval, yval)
        {
            this.ammo = 30;
            this._type = "gun";
            this.graphic = new Sprite("hugeLaser");
            this.center = new Vec2(32f, 32f);
            this.collisionOffset = new Vec2(-16f, -8f);
            this.collisionSize = new Vec2(32f, 15f);
            this._barrelOffsetTL = new Vec2(47f, 30f);
            this._fireSound = "";
            this._fullAuto = false;
            this._fireWait = 1f;
            this._kickForce = 1f;
            this._fireRumble = RumbleIntensity.Light;
            this._editorName = "Death Laser";
            this._tip = new Sprite("bigLaserTip");
            this._tip.CenterOrigin();
            this._chargeAnim = new SpriteMap("laserCharge", 32, 16);
            this._chargeAnim.AddAnimation("idle", 1f, true, new int[1]);
            this._chargeAnim.AddAnimation("load", 0.05f, false, 0, 1, 2, 3, 4);
            this._chargeAnim.AddAnimation("loaded", 1f, true, 5);
            this._chargeAnim.AddAnimation("charge", 0.38f, false, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 28);
            this._chargeAnim.AddAnimation("uncharge", 1.2f, false, 28, 28, 27, 26, 25, 24, 23, 22, 21, 20, 19, 18, 17, 16, 15, 14, 13, 12, 11, 10, 9, 8, 7, 6);
            this._chargeAnim.AddAnimation("drain", 2f, false, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40);
            this._chargeAnim.SetAnimation("loaded");
            this._chargeAnim.center = new Vec2(16f, 10f);
            this._editorName = "Death Ray";
            this.editorTooltip = "Hold the trigger to charge a beam of pure death and destruction. You know, for kids!";
            this._bio = "Invented by Dr.Death for scanning items at your local super market. Also has some military application.";
        }

        public override void Initialize()
        {
            this._chargeSound = SFX.Get("laserCharge", 0.0f);
            this._chargeSoundShort = SFX.Get("laserChargeShort", 0.0f);
            this._unchargeSound = SFX.Get("laserUncharge", 0.0f);
            this._unchargeSoundShort = SFX.Get("laserUnchargeShort", 0.0f);
        }

        public void PostFireLogic()
        {
            if (this.isServerForObject)
            {
                this._unchargeSound.Stop();
                this._unchargeSound.Volume = 0.0f;
                this._unchargeSoundShort.Stop();
                this._unchargeSoundShort.Volume = 0.0f;
                this._chargeSound.Stop();
                this._chargeSound.Volume = 0.0f;
                this._chargeSoundShort.Stop();
                this._chargeSoundShort.Volume = 0.0f;
            }
            this._chargeAnim.SetAnimation("drain");
            SFX.Play("laserBlast");
        }

        public override void Update()
        {
            if (Network.isActive)
            {
                if (this.isServerForObject)
                {
                    this._chargeVolume = this._chargeSound.State == SoundState.Playing ? this._chargeSound.Volume : 0.0f;
                    this._chargeVolumeShort = this._chargeSoundShort.State == SoundState.Playing ? this._chargeSoundShort.Volume : 0.0f;
                    this._unchargeVolume = this._unchargeSound.State == SoundState.Playing ? this._unchargeSound.Volume : 0.0f;
                    this._unchargeVolumeShort = this._unchargeSoundShort.State == SoundState.Playing ? this._unchargeSoundShort.Volume : 0.0f;
                }
                else
                {
                    this._chargeSound.Volume = this._chargeVolume;
                    this._chargeSoundShort.Volume = this._chargeVolumeShort;
                    this._unchargeSound.Volume = this._unchargeVolume;
                    this._unchargeSoundShort.Volume = this._unchargeVolumeShort;
                    if (_chargeVolume > 0.0 && this._chargeSound.State != SoundState.Playing)
                        this._chargeSound.Play();
                    else if (_chargeVolume <= 0.0)
                        this._chargeSound.Stop();
                    if (_chargeVolumeShort > 0.0 && this._chargeSoundShort.State != SoundState.Playing)
                        this._chargeSoundShort.Play();
                    else if (_chargeVolumeShort <= 0.0)
                        this._chargeSoundShort.Stop();
                    if (_unchargeVolume > 0.0 && this._unchargeSound.State != SoundState.Playing)
                        this._unchargeSound.Play();
                    else if (_unchargeVolume <= 0.0)
                        this._unchargeSound.Stop();
                    if (_unchargeVolumeShort > 0.0 && this._unchargeSoundShort.State != SoundState.Playing)
                        this._unchargeSoundShort.Play();
                    else if (_unchargeVolumeShort <= 0.0)
                        this._unchargeSoundShort.Stop();
                }
            }
            base.Update();
            if (_charge > 0.0)
                this._charge -= 0.1f;
            else
                this._charge = 0.0f;
            if (this._chargeAnim.currentAnimation == "uncharge" && this._chargeAnim.finished)
                this._chargeAnim.SetAnimation("loaded");
            if (this._chargeAnim.currentAnimation == "charge" && this._chargeAnim.finished && this.isServerForObject)
            {
                this.PostFireLogic();
                if (this.owner is Duck owner)
                {
                    RumbleManager.AddRumbleEvent(owner.profile, new RumbleEvent(RumbleIntensity.Medium, RumbleDuration.Pulse, RumbleFalloff.Short));
                    owner.sliding = true;
                    owner.crouch = true;
                    Vec2 vec2 = this.barrelVector * 9f;
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
                    this.hSpeed -= barrelVector.x * 9f;
                    this.vSpeed -= (float)(barrelVector.y * 9.0 + 3.0);
                }
                Vec2 vec2_1 = this.Offset(this.barrelOffset);
                Vec2 vec2_2 = this.Offset(this.barrelOffset + new Vec2(1200f, 0.0f)) - vec2_1;
                if (this.isServerForObject)
                    ++Global.data.laserBulletsFired.valueInt;
                if (Network.isActive)
                    Send.Message(new NMDeathBeam(this, vec2_1, vec2_2));
                DeathBeam deathBeam = new DeathBeam(vec2_1, vec2_2, this.owner)
                {
                    isLocal = this.isServerForObject
                };
                Level.Add(deathBeam);
                this.doBlast = true;
            }
            if (this.doBlast && this.isServerForObject)
            {
                ++this._framesSinceBlast;
                if (this._framesSinceBlast > 10)
                {
                    this._framesSinceBlast = 0;
                    this.doBlast = false;
                }
            }
            if (this._chargeAnim.currentAnimation == "drain" && this._chargeAnim.finished)
                this._chargeAnim.SetAnimation("loaded");
            this._lastDoBlast = this.doBlast;
        }

        public override void Draw()
        {
            base.Draw();
            Material material = Graphics.material;
            Graphics.material = this.material;
            this._tip.depth = this.depth + 1;
            this._tip.alpha = this._charge;
            if (this._chargeAnim.currentAnimation == "charge")
                this._tip.alpha = _chargeAnim.frame / 24f;
            else if (this._chargeAnim.currentAnimation == "uncharge")
                this._tip.alpha = (24 - this._chargeAnim.frame) / 24f;
            else
                this._tip.alpha = 0.0f;
            Graphics.Draw(this._tip, this.barrelPosition.x, this.barrelPosition.y);
            this._chargeAnim.flipH = this.graphic.flipH;
            this._chargeAnim.depth = this.depth + 1;
            this._chargeAnim.angle = this.angle;
            this._chargeAnim.alpha = this.alpha;
            Graphics.Draw(_chargeAnim, this.x, this.y);
            Graphics.material = material;
            float num1 = Maths.NormalizeSection(this._tip.alpha, 0.0f, 0.7f);
            float num2 = Maths.NormalizeSection(this._tip.alpha, 0.6f, 1f);
            float num3 = Maths.NormalizeSection(this._tip.alpha, 0.75f, 1f);
            float num4 = Maths.NormalizeSection(this._tip.alpha, 0.9f, 1f);
            float num5 = Maths.NormalizeSection(this._tip.alpha, 0.8f, 1f) * 0.5f;
            if ((double)num1 <= 0.0)
                return;
            Vec2 p1 = this.Offset(this.barrelOffset);
            Vec2 p2 = this.Offset(this.barrelOffset + new Vec2(num1 * 1200f, 0f));
            Graphics.DrawLine(p1, p2, new Color((this._tip.alpha * 0.7f + 0.3f), this._tip.alpha, this._tip.alpha) * (0.3f + num5), (1f + num2 * 12f));
            Graphics.DrawLine(p1, p2, Color.Red * (0.2f + num5), (1f + num3 * 28f));
            Graphics.DrawLine(p1, p2, Color.Red * (0.1f + num5), (0.2f + num4 * 40f));
        }

        public override void OnPressAction()
        {
            if (this._chargeAnim == null || this._chargeSound == null)
                return;
            if (this._chargeAnim.currentAnimation == "loaded")
            {
                this._chargeAnim.SetAnimation("charge");
                if (!this.isServerForObject)
                    return;
                this._chargeSound.Volume = 1f;
                this._chargeSound.Play();
                this._unchargeSound.Stop();
                this._unchargeSound.Volume = 0f;
                this._unchargeSoundShort.Stop();
                this._unchargeSoundShort.Volume = 0f;
            }
            else
            {
                if (!(this._chargeAnim.currentAnimation == "uncharge"))
                    return;
                if (this.isServerForObject)
                {
                    if (this._chargeAnim.frame > 18)
                    {
                        this._chargeSound.Volume = 1f;
                        this._chargeSound.Play();
                    }
                    else
                    {
                        this._chargeSoundShort.Volume = 1f;
                        this._chargeSoundShort.Play();
                    }
                }
                int frame = this._chargeAnim.frame;
                this._chargeAnim.SetAnimation("charge");
                this._chargeAnim.frame = 22 - frame;
                if (!this.isServerForObject)
                    return;
                this._unchargeSound.Stop();
                this._unchargeSound.Volume = 0.0f;
                this._unchargeSoundShort.Stop();
                this._unchargeSoundShort.Volume = 0.0f;
            }
        }

        public override void OnHoldAction()
        {
        }

        public override void OnReleaseAction()
        {
            if (!(this._chargeAnim.currentAnimation == "charge"))
                return;
            if (this.isServerForObject)
            {
                if (this._chargeAnim.frame > 20)
                {
                    this._unchargeSound.Stop();
                    this._unchargeSound.Volume = 1f;
                    this._unchargeSound.Play();
                }
                else
                {
                    this._unchargeSoundShort.Stop();
                    this._unchargeSoundShort.Volume = 1f;
                    this._unchargeSoundShort.Play();
                }
            }
            int frame = this._chargeAnim.frame;
            this._chargeAnim.SetAnimation("uncharge");
            this._chargeAnim.frame = 22 - frame;
            if (!this.isServerForObject)
                return;
            this._chargeSound.Stop();
            this._chargeSound.Volume = 0.0f;
            this._chargeSoundShort.Stop();
            this._chargeSoundShort.Volume = 0.0f;
        }
    }
}
