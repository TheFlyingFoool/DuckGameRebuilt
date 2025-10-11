namespace DuckGame
{
    [ClientOnly]
    public class NMDeathCone : NMEvent
    {
        public HugerLaser laser;
        public Vec2 position;
        public Vec2 target;

        public NMDeathCone()
        {
        }

        public NMDeathCone(HugerLaser pLaser, Vec2 pPosition, Vec2 pTarget)
        {
            position = pPosition;
            target = pTarget;
            laser = pLaser;
        }

        public override void Activate()
        {
            LaserConeBlast deathBeam = new LaserConeBlast(position.x, position.y, target)
            {
                isLocal = false
            };
            Level.Add(deathBeam);
            base.Activate();
        }
    }
}
