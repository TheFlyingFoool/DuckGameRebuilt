namespace DuckGame
{
    public class ATPhaser : ATLaser
    {
        public ATPhaser()
        {
            accuracy = 0.8f;
            range = 600f;
            penetration = 1f;
            bulletSpeed = 10f;
            bulletThickness = 0.3f;
            bulletLength = 40f;
            rangeVariation = 50f;
            bulletType = typeof(LaserBullet);
            angleShot = false;
        }

        public override void WriteAdditionalData(BitBuffer b)
        {
            base.WriteAdditionalData(b);
            b.Write(bulletThickness);
            b.Write(penetration);
        }

        public override void ReadAdditionalData(BitBuffer b)
        {
            base.ReadAdditionalData(b);
            bulletThickness = b.ReadFloat();
            penetration = b.ReadFloat();
        }
    }
}
