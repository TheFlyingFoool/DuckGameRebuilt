namespace DuckGame
{
    public class ATMagnum : AmmoType
    {
        public float angle;

        public ATMagnum()
        {
            accuracy = 1f;
            range = 300f;
            penetration = 2f;
            bulletSpeed = 36f;
            combustable = true;
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
}
