namespace DuckGame
{
    public class ATCampingBall : AmmoType
    {
        public ATCampingBall()
        {
            accuracy = 0.6f;
            range = 115f;
            penetration = 1f;
            rangeVariation = 10f;
            combustable = false;
        }

        public override void PopShell(float x, float y, int dir)
        {
            ShotgunShell shotgunShell = new ShotgunShell(x, y)
            {
                hSpeed = dir * (1.5f + Rando.Float(1f))
            };
            Level.Add(shotgunShell);
        }
    }
}
