/*namespace DuckGame
{
    [ClientOnly]
    public class ATWumpMagnum : AmmoType
    {
        public float angle;

        public ATWumpMagnum()
        {
            forcedIndex = 255;
            accuracy = 1f;
            range = 400f;
            penetration = 2f;
            bulletSpeed = 36f;
            combustable = true;
            rebound = true;
            bulletThickness = 2;
            bulletType = typeof(WumpMagnumbullet);
            flawlessPipeTravel = true;
        }

        public override void PopShell(float x, float y, int dir)
        {
            if (DGRSettings.S_ParticleMultiplier != 0)
            {
                MagnumShell magnumShell = new MagnumShell(x, y)
                {
                    hSpeed = dir * (1.5f + Rando.Float(1f))
                };
                Level.Add(magnumShell);
            }
        }
    }
}*/