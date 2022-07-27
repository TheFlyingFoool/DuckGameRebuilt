// Decompiled with JetBrains decompiler
// Type: DuckGame.CorkGun
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
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
            this.ammo = 100000;
            this._ammoType = new ATCork();
            this._type = "gun";
            this._sprite = new SpriteMap("corkGun", 13, 10);
            this.graphic = _sprite;
            this.center = new Vec2(6f, 4f);
            this.collisionOffset = new Vec2(-6f, -4f);
            this.collisionSize = new Vec2(12f, 8f);
            this._barrelOffsetTL = new Vec2(10f, 3f);
            this._fireSound = "corkFire";
            this._kickForce = 1f;
            this._fireRumble = RumbleIntensity.Kick;
        }

        public override void Update()
        {
            if (windingVelocity > 1.0)
                this.windingVelocity = 1f;
            this.windingVelocity = Lerp.FloatSmooth(this.windingVelocity, 0.0f, 0.05f);
            if (this.corkObject != null)
            {
                double num = (double)this.corkObject.WindUp(this.windingVelocity);
                if (num < 10.0)
                {
                    Level.Remove(corkObject);
                    this.ammo = 1;
                    this.windingVelocity = 0.0f;
                    this.corkObject = null;
                    this._firedCork = 0;
                    this.scale = new Vec2(1.5f, 1.5f);
                }
                if (num < 16.0)
                    this.windingVelocity = 1f;
            }
            this.scale = Lerp.Vec2Smooth(this.scale, Vec2.One, 0.1f);
            this._sprite.frame = this._firedCork == 0 ? 0 : 1;
            base.Update();
        }

        public override void OnPressAction()
        {
            if (this._firedCork != 0)
                return;
            this._firedCork = 1;
            base.OnPressAction();
        }

        public override void OnHoldAction()
        {
            if (this._firedCork == 2)
                this.windingVelocity += 0.12f;
            base.OnHoldAction();
        }

        public override void OnReleaseAction()
        {
            if (this._firedCork == 1)
                this._firedCork = 2;
            base.OnReleaseAction();
        }
    }
}
