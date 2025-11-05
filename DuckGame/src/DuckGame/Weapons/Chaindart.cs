using System;

namespace DuckGame
{
    [EditorGroup("Guns|Machine Guns")]
    [BaggedProperty("isSuperWeapon", true)]
    public class Chaindart : Gun
    {
        public StateBinding _fireWaitBinding = new StateBinding("_fireWait");
        public StateBinding _spinBinding = new StateBinding(nameof(_spin));
        public StateBinding _spinningBinding = new StateBinding(nameof(_spinning));
        public StateBinding _burnLifeBinding = new StateBinding(nameof(_burnLife));
        public float _burnLife = 1f;
        public float _burnWait;
        private SpriteMap _burned;
        private SpriteMap _tip;
        private SpriteMap _sprite;
        public float _spin;
        private ChaingunBullet _bullets;
        private ChaingunBullet _topBullet;
        private Sound _spinUp;
        private Sound _spinDown;
        public Sound spinUp
        {
            get { return _spinUp; }
            set { _spinUp = value; }
        }
        public Sound spinDown
        {
            get { return _spinDown; }
            set { _spinDown = value; }
        }
        private int bulletsTillRemove = 10;
        private int numHanging = 10;
        public bool spinning
        {
            get
            {
                return _spinning;
            }
            set
            {
                _spinning = value;
            }
        }
        private bool _spinning;
        private float spinAmount;
        private bool burntOut;

        public Chaindart(float xval, float yval)
          : base(xval, yval)
        {
            ammo = 100;
            _ammoType = new ATDart
            {
                range = 170f,
                accuracy = 0.5f
            };
            wideBarrel = true;
            barrelInsertOffset = new Vec2(0f, 0f);
            _type = "gun";
            _sprite = new SpriteMap("dartchain", 38, 18);
            graphic = _sprite;
            center = new Vec2(14f, 9f);
            collisionOffset = new Vec2(-8f, -3f);
            collisionSize = new Vec2(24f, 10f);
            _burned = new SpriteMap("dartchain_burned", 38, 18);
            graphic = _sprite;
            _tip = new SpriteMap("dartchain_tip", 38, 18);
            _barrelOffsetTL = new Vec2(38f, 8f);
            _fireSound = "pistolFire";
            _fullAuto = true;
            _fireWait = 0.7f;
            _kickForce = 1f;
            _fireRumble = RumbleIntensity.Kick;
            weight = 4f;
            _holdOffset = new Vec2(4f, 2f);
            flammable = 0.8f;
            physicsMaterial = PhysicsMaterial.Plastic;
            editorTooltip = "Like a chaingun, but for babies. Fires safety-capped sponge darts.";
            _editorPreviewOffset.x = -5;
            _editorPreviewWidth = 38;
        }
        public override void OnTeleport()
        {
            if (_bullets != null)
            {
                _bullets.OnTeleport();
            }
            if (_topBullet != null)
            {
                _topBullet.OnTeleport();
            }
            base.OnTeleport();
        }
        public override void Initialize()
        {
            _spinUp = SFX.Get("chaingunSpinUp");
            _spinDown = SFX.Get("chaingunSpinDown");
            _spinUp.saveToRecording = false;
            _spinDown.saveToRecording = false;
            base.Initialize();
            _bullets = new ChaingunBullet(x, y, true)
            {
                parentThing = this
            };
            _topBullet = _bullets;
            float num = 0.1f;
            ChaingunBullet chaingunBullet1 = null;
            for (int index = 0; index < 9; ++index)
            {
                ChaingunBullet chaingunBullet2 = new ChaingunBullet(x, y, true)
                {
                    parentThing = _bullets
                };
                _bullets = chaingunBullet2;
                chaingunBullet2.waveAdd = num;
                num += 0.4f;
                if (index == 0)
                    _topBullet.childThing = chaingunBullet2;
                else
                    chaingunBullet1.childThing = chaingunBullet2;
                chaingunBullet1 = chaingunBullet2;
            }
        }

        public override void Terminate()
        {
        }

        public override void OnPressAction()
        {
            if (burntOut)
                SFX.Play("dartStick", 0.5f, Rando.Float(0.2f) - 0.1f);
            else
                base.OnPressAction();
        }

        public override void OnHoldAction()
        {
            if (burntOut)
                return;
            if (!_spinning)
            {
                _spinning = true;
                if (_spinDown != null)
                {
                    _spinDown.Volume = 0f;
                    _spinDown.Stop();
                }
                if (_spinUp != null)
                {
                    _spinUp.Volume = 1f;
                    _spinUp.Play();
                }
            }
            if (_spin < 1f)
            {
                _spin += 0.04f;
            }
            else
            {
                _spin = 1f;
                base.OnHoldAction();
            }
        }

        public override void OnReleaseAction()
        {
            if (!_spinning)
                return;
            _spinning = false;
            if (_spinUp != null)
            {
                _spinUp.Volume = 0f;
                _spinUp.Stop();
            }
            if (_spin <= 0.9f)
                return;
            if (_spinDown != null)
            {
                _spinDown.Volume = 1f;
                _spinDown.Play();
            }
        }

        public override void UpdateOnFire()
        {
            if (!onFire)
                return;
            _burnWait -= 0.01f;
            if (_burnWait < 0f)
            {
                Level.Add(SmallFire.New(22f, 0f, 0f, 0f, stick: this, canMultiply: false, firedFrom: this));
                _burnWait = 1f;
            }
            if (burnt >= 1f)
                return;
            burnt += 1f / 1000f;
        }

        public override void Update()
        {
            if (!burntOut && burnt >= 1f)
            {
                Vec2 vec2 = Offset(new Vec2(10f, 0f));
                if (DGRSettings.ActualParticleMultiplier >= 1) for (int i = 0; i < DGRSettings.ActualParticleMultiplier; i++) Level.Add(SmallSmoke.New(vec2.x, vec2.y));
                else if (Rando.Float(1) < DGRSettings.ActualParticleMultiplier) Level.Add(SmallSmoke.New(vec2.x, vec2.y));
                _onFire = false;
                flammable = 0f;
                burntOut = true;
            }
            if (_topBullet != null)
            {
                _topBullet.DoUpdate();
                int num = ammo / bulletsTillRemove;
                if (num < numHanging)
                {
                    _topBullet = _topBullet.childThing as ChaingunBullet;
                    if (_topBullet != null)
                        _topBullet.parentThing = this;
                }
                numHanging = num;
            }
            if (isServerForObject)
            {
                FluidPuddle litBy = Level.CheckPoint<FluidPuddle>(barrelPosition.x, barrelPosition.y);
                if (litBy != null && litBy.data.heat > 0.5)
                    OnBurn(barrelPosition, litBy);
            }
            _fireWait = (0.65f + Maths.NormalizeSection(_barrelHeat, 5f, 9f) * 3f) + Rando.Float(0.25f);
            if (_barrelHeat > 10f)
                _barrelHeat = 10f;
            _barrelHeat -= 0.005f;
            if (_barrelHeat < 0f)
                _barrelHeat = 0f;
            if (!burntOut)
            {
                _sprite.speed = _spin;
                _tip.speed = _spin;
                if (_spin > 0f)
                    _spin -= 0.01f;
                else
                    _spin = 0f;
                spinAmount += _spin;
                barrelInsertOffset = new Vec2(0f, (float)(2f + Math.Sin(spinAmount / 9f * 3.14f) * 2f));
            }
            base.Update();
            if (_topBullet != null)
            {
                if (!graphic.flipH)
                    _topBullet.chainOffset = new Vec2(1f, 5f);
                else
                    _topBullet.chainOffset = new Vec2(-1f, 5f);
            }
        }

        public override void Fire()
        {
            if (burnt >= 1f || burntOut)
            {
                SFX.Play("dartStick", 0.5f, -0.1f + Rando.Float(0.2f), 0f, false);
                return;
            }
            base.Fire();
        }

        protected override bool OnBurn(Vec2 firePosition, Thing litBy)
        {
            if (!onFire)
                SFX.Play("ignite", pitch: (Rando.Float(0.3f) - 0.3f));
            onFire = true;
            return true;
        }

        protected override void PlayFireSound() => SFX.PlaySynchronized("dartGunFire", 0.7f, Rando.Float(0.2f) - 0.1f);

        public override void Draw()
        {
            Material material = Graphics.material;
            if (burntOut)
            {
                graphic = _burned;
                base.Draw();
            }
            else
            {
                base.Draw();
                Graphics.material = this.material;
                _tip.flipH = graphic.flipH;
                _tip.center = graphic.center;
                _tip.depth = depth + 1;
                _tip.alpha = Math.Min((float)(_barrelHeat * 1.5f / 10f), 1f);
                _tip.angle = angle;
                Graphics.Draw(ref _tip, x, y);
                Graphics.material = material;
            }
            if (_topBullet != null)
            {
                _topBullet.material = this.material;
                _topBullet.DoDraw();
            }
        }
    }
}
