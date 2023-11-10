namespace DuckGame
{
    public class NMDartSmack : NMEvent
    {
        public Vec2 vector;
        public PhysicsObject hit;

        public NMDartSmack(Vec2 pVector, PhysicsObject pSmack)
        {
            vector = pVector;
            hit = pSmack;
        }

        public NMDartSmack()
        {
        }

        public override void Activate()
        {
            if (Level.current == null || hit == null || !hit.isServerForObject)
                return;
            hit.hSpeed += vector.x;
            hit.vSpeed += vector.y;
            Duck duck = hit as Duck;
            if (!(duck != null) && hit is RagdollPart && (hit as RagdollPart).doll != null && (hit as RagdollPart).doll.captureDuck != null)
                duck = (hit as RagdollPart).doll.captureDuck;
            if (duck == null)
                return;
            if (duck.holdObject is Grenade)
                duck.forceFire = true;
            if (Rando.Float(1f) > 0.6f)
                duck.Swear();
            duck.Disarm(null);
        }
    }
}
