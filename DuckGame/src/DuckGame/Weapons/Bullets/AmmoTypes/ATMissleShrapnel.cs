namespace DuckGame
{
    public class ATMissileShrapnel : AmmoType
    {
        public ATMissileShrapnel()
        {
            accuracy = 0.75f;
            range = 250f;
            penetration = 0.4f;
            bulletSpeed = 18f;
            combustable = true;
        }

        public override void MakeNetEffect(Vec2 pos, bool fromNetwork = false)
        {
            if (DGRSettings.ExplosionDecals)
            {
                Level.Add(new ExplosionDecal(pos.x - 24, pos.y - 24));
                Level.Add(new ExplosionDecal(pos.x + 24, pos.y - 24));
                Level.Add(new ExplosionDecal(pos.x + 24, pos.y + 24));
                Level.Add(new ExplosionDecal(pos.x - 24, pos.y + 24));
            }

            Level.Add(new ExplosionPart(pos.x + Rando.Float(-2f, 2f), pos.y + Rando.Float(-2f, 2f), false));
            for (int index = 0; index < 4; ++index)
                Level.Add(new ExplosionPart(pos.x + Rando.Float(-11f, 11f), pos.y + Rando.Float(-11f, 11f), false));
            if (fromNetwork)
            {
                foreach (PhysicsObject physicsObject in Level.CheckCircleAll<PhysicsObject>(pos, 70f))
                {
                    if (physicsObject.isServerForObject)
                    {
                        physicsObject.sleeping = false;
                        physicsObject.vSpeed = -2f;
                    }
                }
            }
            SFX.Play("explode");
        }
    }
}
