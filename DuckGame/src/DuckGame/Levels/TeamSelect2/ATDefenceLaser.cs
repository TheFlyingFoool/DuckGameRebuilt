namespace DuckGame
{
    public class ATDefenceLaser : ATLaser
    {
        public ATDefenceLaser()
        {
            accuracy = 1f;
            range = 600f;
            penetration = 1f;
            bulletSpeed = 30f;
            bulletLength = 40f;
            bulletThickness = 0.3f;
            rangeVariation = 50f;
            bulletType = typeof(LaserBulletPurple);
            angleShot = false;
        }
    }
}
