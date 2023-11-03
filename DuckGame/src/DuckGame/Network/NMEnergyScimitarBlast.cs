namespace DuckGame
{
    public class NMEnergyScimitarBlast : NMEvent
    {
        public Vec2 position;
        public Vec2 target;

        public NMEnergyScimitarBlast(Vec2 pPosition, Vec2 pTarget)
        {
            position = pPosition;
            target = pTarget;
        }

        public NMEnergyScimitarBlast()
        {
        }

        public override void Activate()
        {
            if (Level.current == null)
                return;
            Level.Add(new EnergyScimitarBlast(position, target));
        }
    }
}
