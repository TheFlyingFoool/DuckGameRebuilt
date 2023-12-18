using System;
using System.Collections.Generic;
using System.Net;

namespace DuckGame
{
    public class NCNetDebug : NCBasic
    {
        public static Dictionary<IPEndPoint, List<NCBasicPacket>> _socketData = new Dictionary<IPEndPoint, List<NCBasicPacket>>();

        public NCNetDebug(Network c, int networkIndex)
          : base(c, networkIndex)
        {
        }

        public override NCError OnSendPacket(byte[] data, int length, object connection)
        {
            byte[] data1 = new byte[length + 8];
            BitBuffer bitBuffer = new BitBuffer(data1, false);
            bitBuffer.Write(2449832521355936907L);
            if (data != null)
                bitBuffer.Write(data, length: length);
            lock (_socketData)
            {
                List<NCBasicPacket> ncBasicPacketList = null;
                if (!_socketData.TryGetValue(connection as IPEndPoint, out ncBasicPacketList))
                    _socketData[connection as IPEndPoint] = ncBasicPacketList = new List<NCBasicPacket>();
                ncBasicPacketList.Add(new NCBasicPacket()
                {
                    data = data1,
                    sender = localEndPoint
                });
                this.bytesThisFrame += length + 8;
                int bytesThisFrame = this.bytesThisFrame;
            }
            return null;
        }

        protected override void ReceivePackets(Queue<NCBasicPacket> packets)
        {
            try
            {
                lock (_socketData)
                {
                    List<NCBasicPacket> ncBasicPacketList = null;
                    if (!_socketData.TryGetValue(localEndPoint, out ncBasicPacketList))
                        return;
                    foreach (NCBasicPacket ncBasicPacket in ncBasicPacketList)
                        packets.Enqueue(ncBasicPacket);
                    ncBasicPacketList.Clear();
                }
            }
            catch (Exception)
            {
            }
        }
    }
}
