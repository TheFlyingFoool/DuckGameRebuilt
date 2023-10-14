namespace DuckGame
{
    public class ATRCShrapnel : AmmoType
    {
        public ATRCShrapnel()
        {
            accuracy = 0.75f;
            range = 250f;
            penetration = 0.4f;
            bulletSpeed = 18f;
            combustable = true;
        }

        public override void MakeNetEffect(Vec2 pos, bool fromNetwork = false)
        {
            Level.Add(new ExplosionDecal(pos.x - 8, pos.y + 6));
            Level.Add(new ExplosionDecal(pos.x + 8, pos.y + 6));
            Level.Add(new ExplosionDecal(pos.x, pos.y - 6));
            for (int index = 0; index < 1; index = index + 1 + 1)
                Level.Add(new ExplosionPart(pos.x - 20f + Rando.Float(40f), pos.y - 20f + Rando.Float(40f)));
            SFX.Play("explode");
        }
    }
}
