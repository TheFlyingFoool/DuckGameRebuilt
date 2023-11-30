using System;

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
            if (DGRSettings.FixBulletPositions)
            {
                bullet.BonusUpdateTicks = Math.Min((int)(connection.manager.ping*60.0), DGRSettings.MaximumCorrectionTicks);
                //DevConsole.Log(((int)(connection.manager.ping * 60.0)).ToString());
            }
            Level.current.AddThing(bullet);
        }
    }
}
