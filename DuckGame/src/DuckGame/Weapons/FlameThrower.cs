using System;

namespace DuckGame
{
    [EditorGroup("Guns|Fire")]
    [BaggedProperty("isSuperWeapon", true)]
    public class FlameThrower : Gun
    {
        public StateBinding _firingBinding = new StateBinding(nameof(_firing));
        private SpriteMap _barrelFlame;
        public bool _firing;
        private new float _flameWait;
        private SpriteMap _can;
        private ConstantSound _sound;
        private int _maxAmmo = 100;

        public FlameThrower(float xval, float yval)
          : base(xval, yval)
        {
            barrelInsertOffset = new Vec2(0f, -2f);
            wideBarrel = true;
            ammo = _maxAmmo;
            _ammoType = new AT9mm
            {
                combustable = true
            };
            _type = "gun";
            graphic = new Sprite("flamethrower");
            center = new Vec2(16f, 15f);
            collisionOffset = new Vec2(-8f, -3f);
            collisionSize = new Vec2(16f, 9f);
            _barrelOffsetTL = new Vec2(28f, 16f);
            _fireSound = "smg";
            _fullAuto = true;
            _fireWait = 1f;
            _kickForce = 1f;
            _barrelFlame = new SpriteMap("flameBurst", 20, 21)
            {
                center = new Vec2(0f, 17f)
            };
            _barrelFlame.AddAnimation("idle", 0.4f, true, 0, 1, 2, 3);
            _barrelFlame.AddAnimation("puff", 0.4f, false, 4, 5, 6, 7);
            _barrelFlame.AddAnimation("flame", 0.4f, true, 8, 9, 10, 11);
            _barrelFlame.AddAnimation("puffOut", 0.4f, false, 12, 13, 14, 15);
            _barrelFlame.SetAnimation("idle");
            _can = new SpriteMap("flamethrowerCan", 8, 8)
            {
                center = new Vec2(4f, 4f)
            };
            _holdOffset = new Vec2(2f, 0f);
            _barrelAngleOffset = 8f;
            _editorName = "Flame Thrower";
            editorTooltip = "Some Ducks just want to watch the world burn.";
            _bio = "I have a problem. I want this flame here, to be over there. But I can't pick it up, it's too damn hot. If only there was some way I could throw it.";
        }
        public override Holdable BecomeTapedMonster(TapedGun pTaped)
        {
            if (Editor.clientonlycontent)
            {
                return pTaped.gun1 is FlameThrower && pTaped.gun2 is FlareGun ? new FlareFlameThrower(x, y) : null;
            }
            return base.BecomeTapedMonster(pTaped);
        }
        public override void Initialize()
        {
            _sound = new ConstantSound(this, "flameThrowing");
            _sound.effect.saveToRecording = false;
            base.Initialize();
        }
        public override void Update()
        {
            base.Update();
            if (ammo == 0)
            {
                _firing = false;
                _barrelFlame.speed = 0f;
            }
            if (_firing && _barrelFlame.currentAnimation == "idle")
                _barrelFlame.SetAnimation("puff");
            if (_firing && _barrelFlame.currentAnimation == "puff" && _barrelFlame.finished)
                _barrelFlame.SetAnimation("flame");
            if (!_firing && _barrelFlame.currentAnimation != "idle")
                _barrelFlame.SetAnimation("puffOut");
            if (_barrelFlame.currentAnimation == "puffOut" && _barrelFlame.finished)
                _barrelFlame.SetAnimation("idle");

            if (_sound != null) _sound.lerpVolume = _firing ? 0.5f : 0f;

            if (isServerForObject && _firing && _barrelFlame.imageIndex > 5)
            {
                _flameWait -= 0.25f;
                if (_flameWait > 0)
                    return;
                if (!Recorderator.Playing)
                {
                    Vec2 vec = Maths.AngleToVec(barrelAngle + Rando.Float(-0.5f, 0.5f));
                    Vec2 vec2 = new Vec2(vec.x * Rando.Float(2f, 3.5f), vec.y * Rando.Float(2f, 3.5f));
                    ammo -= 2;
                    Level.Add(SmallFire.New(barrelPosition.x, barrelPosition.y, vec2.x, vec2.y, firedFrom: this));
                }
                _flameWait = 1f;
            }
            else
                _flameWait = 0f;
        }

        public override void Draw()
        {
            base.Draw();
            Material material = Graphics.material;
            Graphics.material = null;
            if (_barrelFlame.speed > 0)
            {
                _barrelFlame.SkipIntraTick = SkipIntratick;
                _barrelFlame.alpha = 0.9f;
                Draw(ref _barrelFlame, new Vec2(11f, 1f));
            }
            _can.SkipIntraTick = SkipIntratick;
            _can.frame = (int)((1f - (float)ammo / (float)_maxAmmo) * 15f);
            Draw(ref _can, new Vec2(barrelOffset.x - 11f, barrelOffset.y + 4f));
            Graphics.material = material;
        }

        public override void OnPressAction()
        {
            if (heat > 1)
            {
                for (int index = 0; index < ammo / 10 + 3; ++index)
                    Level.Add(SmallFire.New(x - 6f + Rando.Float(12f), y - 8f + Rando.Float(4f), Rando.Float(6f) - 3f, 1f - Rando.Float(4.5f), firedFrom: this));
                SFX.Play("explode", pitch: (Rando.Float(0.3f) - 0.3f));
                Level.Remove(this);
                _sound.Kill();
                Level.Add(new ExplosionPart(x, y));
            }
            _firing = true;
        }

        public override void OnReleaseAction() => _firing = false;

        public override void Fire()
        {
        }
    }
}
