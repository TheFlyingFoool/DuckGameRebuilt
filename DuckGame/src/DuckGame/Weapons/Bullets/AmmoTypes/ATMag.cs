namespace DuckGame
{
    public class ATMag : AmmoType
    {
        public bool angleShot = true;

        public ATMag()
        {
            accuracy = 0.95f;
            range = 250f;
            penetration = 1f;
            bulletSpeed = 40f;
            bulletThickness = 0.3f;
            bulletType = typeof(MagBullet);
            combustable = true;
        }
    }
}
