// Decompiled with JetBrains decompiler
// Type: DuckGame.NMPeerInformation
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

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
