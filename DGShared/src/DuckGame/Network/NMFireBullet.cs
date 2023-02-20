// Decompiled with JetBrains decompiler
// Type: DuckGame.NMFireBullet
//removed for regex reasons Culture=neutral, PublicKeyToken=null
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
            range = varRange;
            speed = varSpeed;
            angle = varAngle;
        }

        public void DoActivate(Vec2 position, Profile owner)
        {
            typeInstance.rangeVariation = 0f;
            typeInstance.accuracy = 1f;
            typeInstance.bulletSpeed = speed;
            typeInstance.speedVariation = 0f;
            Bullet bullet = typeInstance.GetBullet(position.x, position.y, owner?.duck, -angle, distance: range, network: false);
            bullet.isLocal = false;
            bullet.connection = connection;
            Level.current.AddThing(bullet);
        }
    }
}
