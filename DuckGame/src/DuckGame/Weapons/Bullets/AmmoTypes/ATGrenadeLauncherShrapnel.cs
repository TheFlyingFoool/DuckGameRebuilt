namespace DuckGame
{
    public class ATGrenadeLauncherShrapnel : ATShrapnel
    {
        public override void MakeNetEffect(Vec2 pos, bool fromNetwork = false)
        {
            for (int index = 0; index < 1; ++index)
            {
                ExplosionPart explosionPart = new ExplosionPart(pos.x - 8f + Rando.Float(16f), pos.y - 8f + Rando.Float(16f));
                explosionPart.xscale *= 0.7f;
                explosionPart.yscale *= 0.7f;
                Level.Add(explosionPart);
            }
            SFX.Play("explode");
        }
    }
}
