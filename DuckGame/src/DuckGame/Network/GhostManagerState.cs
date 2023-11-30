namespace DuckGame
{
    public class GhostManagerState : Thing
    {
        public StateBinding _predictionIndexBinding = new StateBinding(nameof(predictionIndex));
        public NetIndex16 predictionIndex = new NetIndex16(short.MaxValue);

        public GhostManagerState()
          : base()
        {
        }
    }
}
