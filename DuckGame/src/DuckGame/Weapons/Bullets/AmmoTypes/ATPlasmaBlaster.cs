namespace DuckGame
{
    public class ATPlasmaBlaster : ATLaser
    {
        public ATPlasmaBlaster()
        {
            accuracy = 0.75f;
            range = 250f;
            penetration = 1f;
            bulletSpeed = 64f;
            bulletColor = Color.Orange;
            bulletType = typeof(Bullet);
            bulletThickness = 1f;
        }
    }
}
