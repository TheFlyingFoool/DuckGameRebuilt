namespace DuckGame
{
    public class ATPewPew : ATLaser
    {
        public ATPewPew()
        {
            accuracy = 0.8f;
            range = 600f;
            penetration = 1f;
            bulletSpeed = 10f;
            bulletLength = 40f;
            bulletThickness = 0.3f;
            rangeVariation = 50f;
            bulletType = typeof(LaserBullet);
            angleShot = false;
        }
    }
}
