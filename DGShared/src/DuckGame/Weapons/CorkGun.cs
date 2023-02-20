// Decompiled with JetBrains decompiler
// Type: DuckGame.CorkGun
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class CorkGun : Gun
    {
        public CorkObject corkObject;
        private SpriteMap _sprite;
        private int _firedCork;
        public float windingVelocity;

        public CorkGun(float xval, float yval)
          : base(xval, yval)
        {
            ammo = 100000;
            _ammoType = new ATCork();
            _type = "gun";
            _sprite = new SpriteMap("corkGun", 13, 10);
            graphic = _sprite;
            center = new Vec2(6f, 4f);
            collisionOffset = new Vec2(-6f, -4f);
            collisionSize = new Vec2(12f, 8f);
            _barrelOffsetTL = new Vec2(10f, 3f);
            _fireSound = "corkFire";
            _kickForce = 1f;
            _fireRumble = RumbleIntensity.Kick;
        }

        public override void Update()
        {
            if (windingVelocity > 1f)
                windingVelocity = 1f;
            windingVelocity = Lerp.FloatSmooth(windingVelocity, 0f, 0.05f);
            if (corkObject != null)
            {
                double num = corkObject.WindUp(windingVelocity);
                if (num < 10f)
                {
                    Level.Remove(corkObject);
                    ammo = 1;
                    windingVelocity = 0f;
                    corkObject = null;
                    _firedCork = 0;
                    scale = new Vec2(1.5f, 1.5f);
                }
                if (num < 16.0)
                    windingVelocity = 1f;
            }
            scale = Lerp.Vec2Smooth(scale, Vec2.One, 0.1f);
            _sprite.frame = _firedCork == 0 ? 0 : 1;
            base.Update();
        }

        public override void OnPressAction()
        {
            if (_firedCork != 0)
                return;
            _firedCork = 1;
            base.OnPressAction();
        }

        public override void OnHoldAction()
        {
            if (_firedCork == 2)
                windingVelocity += 0.12f;
            base.OnHoldAction();
        }

        public override void OnReleaseAction()
        {
            if (_firedCork == 1)
                _firedCork = 2;
            base.OnReleaseAction();
        }
    }
}
