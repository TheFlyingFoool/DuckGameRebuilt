namespace DuckGame
{
    [FixedNetworkID(30016)]
    public class NMNewPingHost : NMNewPing
    {
        public NetIndex16 hostSynchronizedTime;

        public NMNewPingHost(byte pIndex)
          : base(pIndex)
        {
        }

        public NMNewPingHost()
        {
        }

        protected override void OnSerialize()
        {
            hostSynchronizedTime = Network.TickSync;
            base.OnSerialize();
        }
    }
}
