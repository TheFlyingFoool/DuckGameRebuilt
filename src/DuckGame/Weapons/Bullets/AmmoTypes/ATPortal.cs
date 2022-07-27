// Decompiled with JetBrains decompiler
// Type: DuckGame.ATPortal
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class ATPortal : AmmoType
    {
        public bool angleShot = true;
        private PortalGun _ownerGun;

        public ATPortal(PortalGun OwnerGun)
        {
            this.accuracy = 0.75f;
            this.range = 250f;
            this.penetration = 1f;
            this.bulletSpeed = 20f;
            this.rebound = true;
            this.bulletThickness = 0.3f;
            this._ownerGun = OwnerGun;
        }

        public override Bullet FireBullet(
          Vec2 position,
          Thing owner = null,
          float angle = 0.0f,
          Thing firedFrom = null)
        {
            angle *= -1f;
            Bullet t = new PortalBullet(position.x, position.y, this, angle, _ownerGun, this.rebound, thick: this.bulletThickness);
            t.firedFrom = firedFrom;
            Level.current.AddThing(t);
            return t;
        }
    }
}
