// Decompiled with JetBrains decompiler
// Type: DuckGame.Chaingun
//removed for regex reasons Culture=neutral, PublicKeyToken=null
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
            ammo = 100;
            _ammoType = new AT9mm
            {
                range = 170f,
                accuracy = 0.5f
            };
            wideBarrel = true;
            barrelInsertOffset = new Vec2(0f, 0f);
            _type = "gun";
            _sprite = new SpriteMap("chaingun", 42, 28);
            graphic = _sprite;
            center = new Vec2(14f, 14f);
            collisionOffset = new Vec2(-8f, -3f);
            collisionSize = new Vec2(24f, 10f);
            _tip = new SpriteMap("chaingunTip", 42, 28);
            _barrelOffsetTL = new Vec2(39f, 14f);
            _fireSound = "pistolFire";
            _fullAuto = true;
            _fireWait = 0.7f;
            _kickForce = 1f;
            _fireRumble = RumbleIntensity.Kick;
            weight = 8f;
            _holdOffset = new Vec2(0f, 2f);
            editorTooltip = "Like a chaingun, but for adults. Fires mean pointy metal things.";
        }

        public override void Initialize()
        {
            _spinUp = SFX.Get("chaingunSpinUp");
            _spinDown = SFX.Get("chaingunSpinDown");
            base.Initialize();
            _bullets = new ChaingunBullet(x, y)
            {
                parentThing = this
            };
            _topBullet = _bullets;
            float num = 0.1f;
            ChaingunBullet chaingunBullet1 = null;
            for (int index = 0; index < 9; ++index)
            {
                ChaingunBullet chaingunBullet2 = new ChaingunBullet(x, y)
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

        public override void OnHoldAction()
        {
            if (!_spinning)
            {
                _spinning = true;
                _spinDown.Volume = 0f;
                _spinDown.Stop();
                _spinUp.Volume = 1f;
                _spinUp.Play();
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
            _spinUp.Volume = 0f;
            _spinUp.Stop();
            if (_spin <= 0.9f)
                return;
            _spinDown.Volume = 1f;
            _spinDown.Play();
        }

        public override void Update()
        {
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
            _fireWait = (0.7f + Maths.NormalizeSection(_barrelHeat, 5f, 9f) * 5f);
            if (_barrelHeat > 11f)
                _barrelHeat = 11f;
            _barrelHeat -= 0.005f;
            if (_barrelHeat < 0f)
                _barrelHeat = 0f;
            _sprite.speed = _spin;
            _tip.speed = _spin;
            spinAmount += _spin;
            barrelInsertOffset = new Vec2(0f, (2f + (float)Math.Sin(spinAmount / 9f * 3.14f) * 2f));
            if (_spin > 0f)
                _spin -= 0.01f;
            else
                _spin = 0f;
            base.Update();
            if (_topBullet == null)
                return;
            if (!graphic.flipH)
                _topBullet.chainOffset = new Vec2(1f, 5f);
            else
                _topBullet.chainOffset = new Vec2(-1f, 5f);
        }

        public override void Draw()
        {
            Material material = Graphics.material;
            base.Draw();
            Graphics.material = this.material;
            _tip.flipH = graphic.flipH;
            _tip.center = graphic.center;
            _tip.depth = depth + 1;
            _tip.alpha = (float)Math.Min((_barrelHeat * 1.5f / 10f), 1f);
            _tip.angle = angle;
            Graphics.Draw(_tip, x, y);
            if (_topBullet != null)
            {
                _topBullet.material = this.material;
                _topBullet.DoDraw();
            }
            Graphics.material = material;
        }
    }
}
