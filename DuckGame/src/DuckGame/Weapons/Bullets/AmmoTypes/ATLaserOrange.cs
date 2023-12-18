namespace DuckGame
{
    public class ATLaserOrange : ATLaser
    {
        public new bool angleShot = true;

        public ATLaserOrange()
        {
            accuracy = 0.75f;
            range = 250f;
            penetration = 1f;
            bulletSpeed = 20f;
            bulletThickness = 0.3f;
            bulletType = typeof(LaserBulletOrange);
            complexSync = true;
        }
    }
}
