namespace DuckGame
{
    [FixedNetworkID(30014)]
    public class NMNewPing : NMNetworkCoreMessage
    {
        private Timer _pingTimer;
        public byte index;

        public float GetTotalSeconds() => _pingTimer == null ? 1f : (float)_pingTimer.elapsed.TotalSeconds;

        public NMNewPing(byte pIndex) => index = pIndex;

        public NMNewPing()
        {
        }

        protected override void OnSerialize()
        {
            if (_pingTimer == null)
            {
                _pingTimer = new Timer();
                _pingTimer.Start();
            }
            base.OnSerialize();
        }
    }
}
