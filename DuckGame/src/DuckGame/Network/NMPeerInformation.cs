using System.Net;

namespace DuckGame
{
    public class NMPeerInformation : NMEvent
    {
        public int port;
        public IPAddress address;

        public NMPeerInformation()
        {
        }

        public NMPeerInformation(IPAddress vaddress, int vport)
        {
            address = vaddress;
            port = vport;
        }

        protected override void OnSerialize()
        {
            byte[] addressBytes = address.GetAddressBytes();
            BitBuffer val = new BitBuffer();
            val.Write(addressBytes, 0, -1);
            _serializedData.Write(val, true);
            _serializedData.Write(port);
        }

        public override void OnDeserialize(BitBuffer d)
        {
            address = new IPAddress(d.ReadBitBuffer().buffer);
            port = d.ReadInt();
        }
    }
}
