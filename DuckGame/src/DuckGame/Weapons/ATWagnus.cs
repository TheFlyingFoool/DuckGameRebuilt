namespace DuckGame
{
    public class ATWagnus : AmmoType
    {
        public ATWagnus()
        {
            accuracy = 1f;
            range = 128f;
            penetration = 2f;
            bulletSpeed = 25f;
            bulletLength = 40f;
            bulletThickness = 0.3f;
            rangeVariation = 0f;
            barrelAngleDegrees = 180f;
            bulletType = typeof(LaserBulletPurple);
            canBeReflected = false;
            canTeleport = false;
        }
    }
}
