namespace DuckGame
{
    public class ATLaser : AmmoType
    {
        public bool angleShot = true;

        public ATLaser()
        {
            accuracy = 0.75f;
            range = 250f;
            penetration = 1f;
            bulletSpeed = 20f;
            bulletThickness = 0.3f;
            bulletType = typeof(LaserBullet);
            flawlessPipeTravel = true;
        }
    }
}
