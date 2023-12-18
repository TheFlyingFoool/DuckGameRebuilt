namespace DuckGame
{
    public class NMThrowRock : NMEvent
    {
        public byte index;

        public NMThrowRock()
        {
        }

        public NMThrowRock(byte duckIndex) => index = duckIndex;

        public override void Activate() => base.Activate();
    }
}
