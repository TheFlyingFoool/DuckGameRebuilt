namespace DuckGame
{
    public class NMGetReady : NMEvent
    {
        public override void Activate() => GameMode.getReady = true;
    }
}
