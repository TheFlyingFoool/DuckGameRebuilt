namespace DuckGame
{
    public class ATG18 : AmmoType
    {
        public ATG18()
        {
            accuracy = 0.5f;
            range = 90f;
            penetration = 1f;
            rangeVariation = 40f;
            combustable = true;
        }

        public override void PopShell(float x, float y, int dir)
        {
            if (DGRSettings.S_ParticleMultiplier != 0)
            {
                PistolShell pistolShell = new PistolShell(x, y)
                {
                    hSpeed = dir * (1.5f + Rando.Float(1f))
                };
                Level.Add(pistolShell);
            }
        }
    }
}
