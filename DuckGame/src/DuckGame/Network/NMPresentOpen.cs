namespace DuckGame
{
    public class NMPresentOpen : NMEvent
    {
        public Vec2 position;
        public byte frame;

        public NMPresentOpen()
        {
        }

        public NMPresentOpen(Vec2 pPosition, byte pFrame)
        {
            position = pPosition;
            frame = pFrame;
        }

        public override void Activate()
        {
            Present.OpenEffect(position, frame, true);
            base.Activate();
        }
    }
}
