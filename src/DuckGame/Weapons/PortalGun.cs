// Decompiled with JetBrains decompiler
// Type: DuckGame.PortalGun
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    [BaggedProperty("canSpawn", false)]
    [BaggedProperty("isOnlineCapable", false)]
    public class PortalGun : Gun
    {
        private bool _curPortal;

        public bool curPortal
        {
            get => this._curPortal;
            set => this._curPortal = value;
        }

        public PortalGun(float xval, float yval)
          : base(xval, yval)
        {
            this.ammo = 99;
            this._ammoType = new ATPortal(this);
            this._ammoType.range = 600f;
            this._ammoType.accuracy = 1f;
            this._ammoType.rebound = false;
            this._ammoType.bulletSpeed = 10f;
            this._ammoType.bulletLength = 40f;
            this._ammoType.rangeVariation = 50f;
            (this._ammoType as ATPortal).angleShot = false;
            this._type = "gun";
            this.graphic = new Sprite("phaser");
            this.center = new Vec2(7f, 4f);
            this.collisionOffset = new Vec2(-7f, -4f);
            this.collisionSize = new Vec2(15f, 9f);
            this._barrelOffsetTL = new Vec2(14f, 3f);
            this._fireSound = "laserRifle";
            this._fullAuto = false;
            this._fireWait = 0.0f;
            this._kickForce = 0.5f;
            this._holdOffset = new Vec2(0.0f, 0.0f);
            this._flare = new SpriteMap("laserFlare", 16, 16)
            {
                center = new Vec2(0.0f, 8f)
            };
        }
    }
}
