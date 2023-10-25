namespace DuckGame
{
    public class ATReboundLaser : ATLaser
    {
        public ATReboundLaser()
        {
            accuracy = 0.8f;
            range = 220f;
            penetration = 1f;
            bulletSpeed = 20f;
            bulletThickness = 0.3f;
            rebound = true;
            bulletType = typeof(LaserBullet);
            angleShot = true;
        }
    }
}
