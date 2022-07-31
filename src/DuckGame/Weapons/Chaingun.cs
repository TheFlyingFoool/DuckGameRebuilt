// Decompiled with JetBrains decompiler
// Type: DuckGame.Chaingun
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;

namespace DuckGame
{
    [EditorGroup("Guns|Machine Guns")]
    [BaggedProperty("isSuperWeapon", true)]
    public class Chaingun : Gun
    {
        public StateBinding _fireWaitBinding = new StateBinding("_fireWait");
        public StateBinding _spinBinding = new StateBinding(nameof(_spin));
        public StateBinding _spinningBinding = new StateBinding(nameof(_spinning));
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

        public Chaingun(float xval, float yval)
          : base(xval, yval)
        {
            this.ammo = 100;
            this._ammoType = new AT9mm();
            this._ammoType.range = 170f;
            this._ammoType.accuracy = 0.5f;
            this.wideBarrel = true;
            this.barrelInsertOffset = new Vec2(0f, 0f);
            this._type = "gun";
            this._sprite = new SpriteMap("chaingun", 42, 28);
            this.graphic = _sprite;
            this.center = new Vec2(14f, 14f);
            this.collisionOffset = new Vec2(-8f, -3f);
            this.collisionSize = new Vec2(24f, 10f);
            this._tip = new SpriteMap("chaingunTip", 42, 28);
            this._barrelOffsetTL = new Vec2(39f, 14f);
            this._fireSound = "pistolFire";
            this._fullAuto = true;
            this._fireWait = 0.7f;
            this._kickForce = 1f;
            this._fireRumble = RumbleIntensity.Kick;
            this.weight = 8f;
            this._spinUp = SFX.Get("chaingunSpinUp");
            this._spinDown = SFX.Get("chaingunSpinDown");
            this._holdOffset = new Vec2(0f, 2f);
            this.editorTooltip = "Like a chaingun, but for adults. Fires mean pointy metal things.";
        }

        public override void Initialize()
        {
            base.Initialize();
            this._bullets = new ChaingunBullet(this.x, this.y)
            {
                parentThing = this
            };
            this._topBullet = this._bullets;
            float num = 0.1f;
            ChaingunBullet chaingunBullet1 = null;
            for (int index = 0; index < 9; ++index)
            {
                ChaingunBullet chaingunBullet2 = new ChaingunBullet(this.x, this.y)
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

        public override void OnHoldAction()
        {
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

        public override void Update()
        {
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
            this._fireWait = (0.7f + Maths.NormalizeSection(this._barrelHeat, 5f, 9f) * 5f);
            if (_barrelHeat > 11f)
                this._barrelHeat = 11f;
            this._barrelHeat -= 0.005f;
            if (_barrelHeat < 0f)
                this._barrelHeat = 0f;
            this._sprite.speed = this._spin;
            this._tip.speed = this._spin;
            this.spinAmount += this._spin;
            this.barrelInsertOffset = new Vec2(0f, (2f + (float)Math.Sin(spinAmount / 9f * 3.14f) * 2f));
            if (_spin > 0f)
                this._spin -= 0.01f;
            else
                this._spin = 0f;
            base.Update();
            if (this._topBullet == null)
                return;
            if (!this.graphic.flipH)
                this._topBullet.chainOffset = new Vec2(1f, 5f);
            else
                this._topBullet.chainOffset = new Vec2(-1f, 5f);
        }

        public override void Draw()
        {
            Material material = Graphics.material;
            base.Draw();
            Graphics.material = this.material;
            this._tip.flipH = this.graphic.flipH;
            this._tip.center = this.graphic.center;
            this._tip.depth = this.depth + 1;
            this._tip.alpha = (float)Math.Min((_barrelHeat * 1.5 / 10f), 1f);
            this._tip.angle = this.angle;
            Graphics.Draw(_tip, this.x, this.y);
            if (this._topBullet != null)
            {
                this._topBullet.material = this.material;
                this._topBullet.DoDraw();
            }
            Graphics.material = material;
        }
    }
}
