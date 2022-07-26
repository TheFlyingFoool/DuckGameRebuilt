// Decompiled with JetBrains decompiler
// Type: DuckGame.NCNetDebug
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

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
            lock (NCNetDebug._socketData)
            {
                List<NCBasicPacket> ncBasicPacketList = (List<NCBasicPacket>)null;
                if (!NCNetDebug._socketData.TryGetValue(connection as IPEndPoint, out ncBasicPacketList))
                    NCNetDebug._socketData[connection as IPEndPoint] = ncBasicPacketList = new List<NCBasicPacket>();
                ncBasicPacketList.Add(new NCBasicPacket()
                {
                    data = data1,
                    sender = this.localEndPoint
                });
                this.bytesThisFrame += length + 8;
                int bytesThisFrame = this.bytesThisFrame;
            }
            return (NCError)null;
        }

        protected override void ReceivePackets(Queue<NCBasicPacket> packets)
        {
            try
            {
                lock (NCNetDebug._socketData)
                {
                    List<NCBasicPacket> ncBasicPacketList = (List<NCBasicPacket>)null;
                    if (!NCNetDebug._socketData.TryGetValue(this.localEndPoint, out ncBasicPacketList))
                        return;
                    foreach (NCBasicPacket ncBasicPacket in ncBasicPacketList)
                        packets.Enqueue(ncBasicPacket);
                    ncBasicPacketList.Clear();
                }
            }
            catch (Exception ex)
            {
            }
        }
    }
}
