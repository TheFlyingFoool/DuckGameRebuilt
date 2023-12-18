namespace DuckGame
{
    public class ATShrapnel : AmmoType
    {
        public ATShrapnel()
        {
            accuracy = 0.75f;
            range = 250f;
            penetration = 0.4f;
            bulletSpeed = 18f;
            combustable = true;
        }
    }
}
