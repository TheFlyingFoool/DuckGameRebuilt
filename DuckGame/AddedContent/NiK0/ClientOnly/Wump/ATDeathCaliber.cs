namespace DuckGame
{
    [ClientOnly]
    public class ATDeathCaliber : AmmoType
    {
        public float angle;

        public ATDeathCaliber()
        {
            accuracy = 1f;
            range = 1200;
            penetration = 9f;
            bulletSpeed = 128f;
            impactPower = 9f;
            combustable = true;
            bulletThickness = 3;
            bulletType = typeof(DeathCaliber);
            flawlessPipeTravel = true;
        }

        public override void PopShell(float x, float y, int dir)
        {
            Level.Add(new SniperShell(x, y)
            {
                hSpeed = (float)dir * (1.5f + Rando.Float(1f))
            });
        }
    }
}
