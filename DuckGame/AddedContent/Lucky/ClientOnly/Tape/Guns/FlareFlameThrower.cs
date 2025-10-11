namespace DuckGame
{
    [ClientOnly]
    public class FlareFlameThrower : Gun
    {
        public StateBinding _firingBinding = new StateBinding(nameof(_firing));
        private SpriteMap _barrelFlame;
        public bool _firing;
        private new float _flameWait;
        private SpriteMap _can;
        private ConstantSound _sound;
        private int _maxAmmo = 80;

        public FlareFlameThrower(float xval, float yval)
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
            graphic = new Sprite("flameflarethrower");
            center = new Vec2(13.5f, 10.5f);
            collisionOffset = new Vec2(-11.5f, -7.5f);
            collisionSize = new Vec2(23, 18f);
            _barrelOffsetTL = new Vec2(28f, 8);
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
            _holdOffset = new Vec2(2, -3.5f);
            _barrelAngleOffset = 8f;
        }
        public override void Initialize()
        {
            _sound = new ConstantSound(this, "flameThrowing");
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
            _sound.lerpVolume = _firing ? 0.5f : 0f;
            if (isServerForObject && _firing && _barrelFlame.imageIndex > 5)
            {
                _flameWait -= 0.25f;
                if (_flameWait > 0)
                    return;
                Vec2 vec = Maths.AngleToVec(barrelAngle);
                Vec2 vec2 = new Vec2(vec.x * 7, vec.y * 7);
                ammo -= 2;
                if (ammo % 20 == 0)
                {
                    SFX.PlaySynchronized("netGunFire", 0.5f, Rando.Float(0.2f) - 0.4f);
                    if (duck != null)
                        RumbleManager.AddRumbleEvent(duck.profile, new RumbleEvent(_fireRumble, RumbleDuration.Pulse, RumbleFalloff.None));
                    ApplyKick();
                    Flare t = new Flare(barrelPosition.x, barrelPosition.y, null);
                    Fondle(t);
                    t.hSpeed = vec.x * 12;
                    t.vSpeed = vec.y * 12;
                    Level.Add(t);
                }
                Level.Add(SmallFire.New(barrelPosition.x, barrelPosition.y, vec2.x, vec2.y, firedFrom: this));
                _flameWait = 1.25f;
            }
            else _flameWait = 0f;
        }

        public override void Draw()
        {
            base.Draw();
            Material material = Graphics.material;
            Graphics.material = null;
            if (_barrelFlame.speed > 0)
            {
                _barrelFlame.alpha = 0.9f;
                Draw(ref _barrelFlame, new Vec2(11f, 1f));
            }
            _can.frame = (int)((1 - ammo / _maxAmmo) * 15);
            Draw(ref _can, new Vec2(barrelOffset.x - 11f, barrelOffset.y + 4f));
            Graphics.material = material;
        }
        public override bool CanTapeTo(Thing pThing)
        {
            switch (pThing)
            {
                case FlareGun:
                case FlameThrower:
                case FlareFlameThrower:
                    return false;
                default:
                    return true;
            }
        }
        public override void OnPressAction()
        {
            if (heat > 1)
            {
                for (int i = 0; i < 3; i++)
                {
                    Vec2 vec = Maths.AngleToVec(Rando.Float(7));
                    Flare t = new Flare(barrelPosition.x, barrelPosition.y, null);
                    Fondle(t);
                    t.hSpeed = vec.x * 12;
                    t.vSpeed = vec.y * 12;
                    Level.Add(t);
                }
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
