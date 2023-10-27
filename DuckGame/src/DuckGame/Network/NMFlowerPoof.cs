namespace DuckGame
{
    public class NMFlowerPoof : NMEvent
    {
        public Vec2 position;

        public NMFlowerPoof()
        {
        }

        public NMFlowerPoof(Vec2 pPosition) => position = pPosition;

        public override void Activate()
        {
            Flower.PoofEffect(position);
            base.Activate();
        }
    }
}
