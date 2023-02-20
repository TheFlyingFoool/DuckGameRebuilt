namespace DuckGame
{
    [ClientOnly]
    public class NMPush : NMEvent
    {
        public Vec2 vector;
        public PhysicsObject hit;

        public NMPush(Vec2 pVector, PhysicsObject pSmack)
        {
            vector = pVector;
            hit = pSmack;
        }

        public NMPush()
        {
        }

        public override void Activate()
        {
            if (Level.current == null || hit == null || !hit.isServerForObject)
                return;
            hit.hSpeed += vector.x;
            hit.vSpeed += vector.y;
        }
    }
}
