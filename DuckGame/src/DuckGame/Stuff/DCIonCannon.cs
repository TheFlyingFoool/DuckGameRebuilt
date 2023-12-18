namespace DuckGame
{
    public class DCIonCannon : DeathCrateSetting
    {
        public override void Activate(DeathCrate c, bool server = true)
        {
            if (DGRSettings.ActualParticleMultiplier > 0) Level.Add(new ExplosionPart(c.x, c.y - 2f));
            Level.Add(new IonCannon(new Vec2(c.x, c.y + 3000f), new Vec2(c.x, c.y - 3000f))
            {
                serverVersion = server
            });
            Graphics.FlashScreen();
            SFX.Play("laserBlast");
            if (!server)
                return;
            Level.Remove(c);
        }
    }
}
