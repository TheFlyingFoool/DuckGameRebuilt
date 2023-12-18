namespace DuckGame
{
    public class ATGrenade : AmmoType
    {
        public ATGrenade()
        {
            accuracy = 1f;
            penetration = 0.35f;
            bulletSpeed = 9f;
            rangeVariation = 0f;
            speedVariation = 0f;
            range = 2000f;
            rebound = true;
            affectedByGravity = true;
            deadly = false;
            weight = 5f;
            ownerSafety = 4;
            bulletThickness = 2f;
            bulletColor = Color.White;
            bulletType = typeof(GrenadeBullet);
            immediatelyDeadly = true;
            sprite = new Sprite("launcherGrenade");
            sprite.CenterOrigin();
            flawlessPipeTravel = true;
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
