namespace DuckGame
{
    public class DCGasFire : DeathCrateSetting
    {
        public override void Activate(DeathCrate c, bool server = true)
        {
            if (DGRSettings.ActualParticleMultiplier > 0) Level.Add(new ExplosionPart(c.x, c.y - 2f));
            if (server)
            {
                YellowBarrel yellowBarrel = new YellowBarrel(c.x, c.y)
                {
                    vSpeed = -3f
                };
                Level.Add(yellowBarrel);
                Grenade grenade1 = new Grenade(c.x, c.y);
                grenade1.PressAction();
                grenade1.hSpeed = -1f;
                grenade1.vSpeed = -2f;
                Level.Add(grenade1);
                Grenade grenade2 = new Grenade(c.x, c.y);
                grenade2.PressAction();
                grenade2.hSpeed = 1f;
                grenade2.vSpeed = -2f;
                Level.Add(grenade2);
                Level.Remove(c);
            }
            for (int i = 0; i < Maths.Clamp(DGRSettings.ActualParticleMultiplier, 1, 100000); i++) Level.Add(new MusketSmoke(c.x, c.y));
        }
    }
}
