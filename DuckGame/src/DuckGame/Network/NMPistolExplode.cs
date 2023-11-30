namespace DuckGame
{
    public class NMPistolExplode : NMEvent
    {
        public Vec2 position;

        public NMPistolExplode()
        {
        }

        public NMPistolExplode(Vec2 pPosition) => position = pPosition;

        public override void Activate()
        {
            DuelingPistol.ExplodeEffect(position);
            base.Activate();
        }
    }
}
