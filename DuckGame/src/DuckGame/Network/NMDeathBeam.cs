namespace DuckGame
{
    public class NMDeathBeam : NMEvent
    {
        public HugeLaser laser;
        public Vec2 position;
        public Vec2 target;

        public NMDeathBeam()
        {
        }

        public NMDeathBeam(HugeLaser pLaser, Vec2 pPosition, Vec2 pTarget)
        {
            position = pPosition;
            target = pTarget;
            laser = pLaser;
        }

        public override void Activate()
        {
            DeathBeam deathBeam = new DeathBeam(position, target)
            {
                isLocal = false
            };
            Level.Add(deathBeam);
            if (laser != null)
                laser.PostFireLogic();
            base.Activate();
        }
    }
}
