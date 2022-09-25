namespace DuckGame
{
    [ClientOnly]
    public class NMPinkCollision : NMEvent
    {
        public NMPinkCollision(PinkBox box)
        {
            b = box;
        }
        public NMPinkCollision()
        {
        }
        public PinkBox b;

        public override void Activate()
        {
            if (b != null && b.isServerForObject)
            {
                b.Explode();
            }
        }
    }
}
