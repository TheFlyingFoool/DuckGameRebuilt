namespace DuckGame
{
    public class NMAssignDraw : NMEvent
    {
        public override void Activate() => ++Global.data.littleDraws.valueInt;
    }
}
