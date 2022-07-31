// Decompiled with JetBrains decompiler
// Type: DuckGame.Chaindart
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

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
        private int bulletsTillRemove = 10;
        private int numHanging = 10;
        private bool _spinning;
        private float spinAmount;
        private bool burntOut;

        public Chaindart(float xval, float yval)
          : base(xval, yval)
        {
            this.ammo = 100;
            this._ammoType = new ATDart();
            this._ammoType.range = 170f;
            this._ammoType.accuracy = 0.5f;
            this.wideBarrel = true;
            this.barrelInsertOffset = new Vec2(0f, 0f);
            this._type = "gun";
            this._sprite = new SpriteMap("dartchain", 38, 18);
            this.graphic = _sprite;
            this.center = new Vec2(14f, 9f);
            this.collisionOffset = new Vec2(-8f, -3f);
            this.collisionSize = new Vec2(24f, 10f);
            this._burned = new SpriteMap("dartchain_burned", 38, 18);
            this.graphic = _sprite;
            this._tip = new SpriteMap("dartchain_tip", 38, 18);
            this._barrelOffsetTL = new Vec2(38f, 8f);
            this._fireSound = "pistolFire";
            this._fullAuto = true;
            this._fireWait = 0.7f;
            this._kickForce = 1f;
            this._fireRumble = RumbleIntensity.Kick;
            this.weight = 4f;
            this._spinUp = SFX.Get("chaingunSpinUp");
            this._spinDown = SFX.Get("chaingunSpinDown");
            this._holdOffset = new Vec2(4f, 2f);
            this.flammable = 0.8f;
            this.physicsMaterial = PhysicsMaterial.Plastic;
            this.editorTooltip = "Like a chaingun, but for babies. Fires safety-capped sponge darts.";
        }

        public override void Initialize()
        {
            base.Initialize();
            this._bullets = new ChaingunBullet(this.x, this.y, true)
            {
                parentThing = this
            };
            this._topBullet = this._bullets;
            float num = 0.1f;
            ChaingunBullet chaingunBullet1 = null;
            for (int index = 0; index < 9; ++index)
            {
                ChaingunBullet chaingunBullet2 = new ChaingunBullet(this.x, this.y, true)
                {
                    parentThing = _bullets
                };
                this._bullets = chaingunBullet2;
                chaingunBullet2.waveAdd = num;
                num += 0.4f;
                if (index == 0)
                    this._topBullet.childThing = chaingunBullet2;
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
            if (this.burntOut)
                SFX.Play("dartStick", 0.5f, Rando.Float(0.2f) - 0.1f);
            else
                base.OnPressAction();
        }

        public override void OnHoldAction()
        {
            if (this.burntOut)
                return;
            if (!this._spinning)
            {
                this._spinning = true;
                this._spinDown.Volume = 0f;
                this._spinDown.Stop();
                this._spinUp.Volume = 1f;
                this._spinUp.Play();
            }
            if (_spin < 1.0)
            {
                this._spin += 0.04f;
            }
            else
            {
                this._spin = 1f;
                base.OnHoldAction();
            }
        }

        public override void OnReleaseAction()
        {
            if (!this._spinning)
                return;
            this._spinning = false;
            this._spinUp.Volume = 0f;
            this._spinUp.Stop();
            if (_spin <= 0.9f)
                return;
            this._spinDown.Volume = 1f;
            this._spinDown.Play();
        }

        public override void UpdateOnFire()
        {
            if (!this.onFire)
                return;
            this._burnWait -= 0.01f;
            if (_burnWait < 0.0)
            {
                Level.Add(SmallFire.New(22f, 0f, 0f, 0f, stick: this, canMultiply: false, firedFrom: this));
                this._burnWait = 1f;
            }
            if (burnt >= 1.0)
                return;
            this.burnt += 1f / 1000f;
        }

        public override void Update()
        {
            if (!this.burntOut && burnt >= 1.0)
            {
                Vec2 vec2 = this.Offset(new Vec2(10f, 0f));
                Level.Add(SmallSmoke.New(vec2.x, vec2.y));
                this._onFire = false;
                this.flammable = 0f;
                this.burntOut = true;
            }
            if (this._topBullet != null)
            {
                this._topBullet.DoUpdate();
                int num = (int)(ammo / (double)this.bulletsTillRemove);
                if (num < this.numHanging)
                {
                    this._topBullet = this._topBullet.childThing as ChaingunBullet;
                    if (this._topBullet != null)
                        this._topBullet.parentThing = this;
                }
                this.numHanging = num;
            }
            if (this.isServerForObject)
            {
                FluidPuddle litBy = Level.CheckPoint<FluidPuddle>(this.barrelPosition.x, this.barrelPosition.y);
                if (litBy != null && litBy.data.heat > 0.5)
                    this.OnBurn(this.barrelPosition, litBy);
            }
            this._fireWait = (float)(0.649999976158142 + (double)Maths.NormalizeSection(this._barrelHeat, 5f, 9f) * 3.0) + Rando.Float(0.25f);
            if (_barrelHeat > 10.0)
                this._barrelHeat = 10f;
            this._barrelHeat -= 0.005f;
            if (_barrelHeat < 0.0)
                this._barrelHeat = 0f;
            if (!this.burntOut)
            {
                this._sprite.speed = this._spin;
                this._tip.speed = this._spin;
                if (_spin > 0.0)
                    this._spin -= 0.01f;
                else
                    this._spin = 0f;
                this.spinAmount += this._spin;
                this.barrelInsertOffset = new Vec2(0f, (float)(2.0 + Math.Sin(spinAmount / 9.0 * 3.14000010490417) * 2.0));
            }
            base.Update();
            if (this._topBullet == null)
                return;
            if (!this.graphic.flipH)
                this._topBullet.chainOffset = new Vec2(1f, 5f);
            else
                this._topBullet.chainOffset = new Vec2(-1f, 5f);
        }

        public override void Fire()
        {
            if (burnt >= 1.0 || this.burntOut)
                SFX.Play("dartStick", 0.5f, Rando.Float(0.2f) - 0.1f);
            else
                base.Fire();
        }

        protected override bool OnBurn(Vec2 firePosition, Thing litBy)
        {
            if (!this.onFire)
                SFX.Play("ignite", pitch: (Rando.Float(0.3f) - 0.3f));
            this.onFire = true;
            return true;
        }

        protected override void PlayFireSound() => SFX.PlaySynchronized("dartGunFire", 0.7f, Rando.Float(0.2f) - 0.1f);

        public override void Draw()
        {
            Material material = Graphics.material;
            if (this.burntOut)
            {
                this.graphic = _burned;
                base.Draw();
            }
            else
            {
                base.Draw();
                Graphics.material = this.material;
                this._tip.flipH = this.graphic.flipH;
                this._tip.center = this.graphic.center;
                this._tip.depth = this.depth + 1;
                this._tip.alpha = Math.Min((float)(_barrelHeat * 1.5 / 10.0), 1f);
                this._tip.angle = this.angle;
                Graphics.Draw(_tip, this.x, this.y);
                Graphics.material = material;
            }
            if (this._topBullet == null)
                return;
            this._topBullet.material = this.material;
            this._topBullet.DoDraw();
        }
    }
}
