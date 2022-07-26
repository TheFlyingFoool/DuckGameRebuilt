// Decompiled with JetBrains decompiler
// Type: DuckGame.NMFireBullet
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class NMFireBullet : NMEvent
    {
        public float range;
        public float speed;
        public float angle;
        public AmmoType typeInstance;

        public NMFireBullet()
        {
        }

        public NMFireBullet(float varRange, float varSpeed, float varAngle)
        {
            this.range = varRange;
            this.speed = varSpeed;
            this.angle = varAngle;
        }

        public void DoActivate(Vec2 position, Profile owner)
        {
            this.typeInstance.rangeVariation = 0.0f;
            this.typeInstance.accuracy = 1f;
            this.typeInstance.bulletSpeed = this.speed;
            this.typeInstance.speedVariation = 0.0f;
            Bullet bullet = this.typeInstance.GetBullet(position.x, position.y, (Thing)owner?.duck, -this.angle, distance: this.range, network: false);
            bullet.isLocal = false;
            bullet.connection = this.connection;
            Level.current.AddThing((Thing)bullet);
        }
    }
}
