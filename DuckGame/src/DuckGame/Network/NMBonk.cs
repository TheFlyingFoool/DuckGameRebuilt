namespace DuckGame
{
    public class NMBonk : NMEvent
    {
        public Vec2 position;
        public Vec2 velocity;

        public NMBonk()
        {
        }

        public NMBonk(Vec2 pPosition, Vec2 pVelocity)
        {
            position = pPosition;
            velocity = pVelocity;
        }

        public override void Activate()
        {
            Duck.MakeStars(position, velocity);
            base.Activate();
        }
    }
}
