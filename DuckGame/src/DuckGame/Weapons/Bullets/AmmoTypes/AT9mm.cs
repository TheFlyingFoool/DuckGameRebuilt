namespace DuckGame
{
    public class AT9mm : AmmoType
    {
        public AT9mm()
        {
            accuracy = 0.75f;
            range = 250f;
            penetration = 1f;
            combustable = true;
        }

        public override void PopShell(float x, float y, int dir)
        {
            PistolShell pistolShell = new PistolShell(x, y)
            {
                hSpeed = dir * (1.5f + Rando.Float(1f))
            };
            Level.Add(pistolShell);
        }
    }
}
