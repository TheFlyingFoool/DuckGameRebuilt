namespace DuckGame
{
    public class ATHighCalSniper : AmmoType
    {
        public ATHighCalSniper()
        {
            combustable = true;
            range = 1200f;
            accuracy = 1f;
            penetration = 9f;
            bulletSpeed = 96f;
            impactPower = 6f;
            bulletThickness = 1.5f;
        }

        public override void PopShell(float x, float y, int dir)
        {
            SniperShell sniperShell = new SniperShell(x, y)
            {
                hSpeed = dir * (1.5f + Rando.Float(1f))
            };
            Level.Add(sniperShell);
        }
    }
}
