using System;

namespace DuckGame
{
    [ClientOnly]
    public class DoubleShotgun : Gun
    {
        public sbyte _loadProgress = 100;
        public float _loadAnimation = 1f;
        public StateBinding _loadProgressBinding = new StateBinding(nameof(_loadProgress));
        protected SpriteMap _loaderSprite;

        public DoubleShotgun(float xval, float yval)
          : base(xval, yval)
        {
            ammo = 3;
            _ammoType = new ATShotgun();
            _ammoType.accuracy = 0;
            _ammoType.range = 140;
            wideBarrel = true;
            _type = "gun";
            graphic = new Sprite("doubleshotgun");
            center = new Vec2(13.5f, 7.5f);
            collisionOffset = new Vec2(-8f, -3.5f);
            collisionSize = new Vec2(17f, 7f);
            _barrelOffsetTL = new Vec2(27f, 7.5f);
            _fireSound = "shotgunFire2";
            _kickForce = 6f;
            _fireRumble = RumbleIntensity.Medium;
            _numBulletsPerFire = 12;
            _manualLoad = true;
            _loaderSprite = new SpriteMap("shotgunLoader", 8, 8)
            {
                center = new Vec2(4f, 4f)
            };
            _holdOffset = new Vec2(1.5f, -1.5f);
        }
        public override bool CanTapeTo(Thing pThing)
        {
            switch (pThing)
            {
                case Shotgun:
                case DoubleShotgun:
                    return false;
                default:
                    return true;
            }
        }
        public override void Update()
        {
            base.Update();
            if (_loadAnimation == -1)
            {
                SFX.Play("shotgunLoad");
                _loadAnimation = 0f;
            }
            if (_loadAnimation >= 0)
            {
                if (_loadAnimation == 0.5 && ammo != 0) PopShell();
                if (_loadAnimation < 1) _loadAnimation += 0.1f;
                else _loadAnimation = 1f;
            }
            if (_loadProgress < 0)
                return;
            if (_loadProgress == 50)
                Reload(false);
            if (_loadProgress < 100)
                _loadProgress += 10;
            else
                _loadProgress = 100;
        }

        public override void OnPressAction()
        {
            if (loaded)
            {
                base.OnPressAction();
                _loadProgress = -1;
                _loadAnimation = -0.01f;
            }
            else
            {
                if (_loadProgress != -1)
                    return;
                _loadProgress = 0;
                _loadAnimation = -1f;
            }
        }

        public override void Draw()
        {
            base.Draw();
            Vec2 vec2 = new Vec2(13f, -2f);
            float num = (float)Math.Sin(_loadAnimation * 3.14f) * 3f;
            _loaderSprite.flipV = false;
            Draw(ref _loaderSprite, new Vec2(vec2.x - 9.5f - num, vec2.y + 6.5f));
            _loaderSprite.flipV = true;
            Draw(ref _loaderSprite, new Vec2(vec2.x - 9.5f - num, vec2.y - 2.5f));
        }
    }
}
