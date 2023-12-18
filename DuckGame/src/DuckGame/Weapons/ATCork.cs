namespace DuckGame
{
    public class ATCork : AmmoType
    {
        public ATCork()
        {
            accuracy = 0.85f;
            range = 100f;
            penetration = 2f;
            bulletSpeed = 18f;
            bulletType = typeof(CorkBullet);
        }
    }
}
