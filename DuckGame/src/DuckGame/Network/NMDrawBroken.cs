namespace DuckGame
{
    public class NMDrawBroken : NMEvent
    {
        public override void Activate() => ++Global.data.drawsPlayed.valueInt;
    }
}
