// Decompiled with JetBrains decompiler
// Type: DuckGame.DataLayerDebug
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System.Collections.Generic;

namespace DuckGame
{
    public class DataLayerDebug : DataLayer
    {
        private bool sendingDuplicate;

        public DataLayerDebug(NCNetworkImplementation pImpl)
          : base(pImpl)
        {
        }

        public override NCError SendPacket(BitBuffer sendData, NetworkConnection connection)
        {
            if (!sendingDuplicate && Rando.Float(1f) < connection.debuggerContext.duplicate)
            {
                sendingDuplicate = true;
                SendPacket(sendData, connection);
                if (connection.debuggerContext.duplicate > 0.4f && Rando.Float(1f) < connection.debuggerContext.duplicate)
                    SendPacket(sendData, connection);
                if (connection.debuggerContext.duplicate > 0.8f && Rando.Float(1f) < connection.debuggerContext.duplicate)
                    SendPacket(sendData, connection);
                sendingDuplicate = false;
            }
            float num = connection.debuggerContext.CalculateLatency();
            if (connection.debuggerContext.lagSpike > 0)
                num = 1f / 1000f;
            if (num == 3.4E+38f)
                return null;
            if (num <= 0f)
                return _impl.OnSendPacket(sendData.buffer, sendData.lengthInBytes, connection.data);
            connection.debuggerContext.packets.Add(new BadConnection.DelayedPacket()
            {
                data = sendData,
                time = num
            });
            return null;
        }

        public override void Update()
        {
        }

        public class BadConnection
        {
            public int lagSpike;
            private float _latency;
            private float _jitter;
            private float _loss;
            private float _duplicate;
            public NetworkConnection connection;
            public List<DelayedPacket> packets = new List<DelayedPacket>();
            //private int i;

            public float latency
            {
                get => _latency == 0 ? DuckNetwork.localConnection.debuggerContext._latency : _latency;
                set => _latency = value;
            }

            public float jitter
            {
                get => _jitter == 0 ? DuckNetwork.localConnection.debuggerContext._jitter : _jitter;
                set => _jitter = value;
            }

            public float loss
            {
                get => _loss == 0 ? DuckNetwork.localConnection.debuggerContext._loss : _loss;
                set => _loss = value;
            }

            public float duplicate
            {
                get => _duplicate == 0 ? DuckNetwork.localConnection.debuggerContext._duplicate : _duplicate;
                set => _duplicate = value;
            }

            public BadConnection(NetworkConnection pContext) => connection = pContext;

            public float CalculateLatency()
            {
                float num = 0f;
                if (CalculateLoss())
                {
                    if (Rando.Int(3) != 0)
                        return float.MaxValue;
                    num += Rando.Float(2f, 4f);
                }
                return (float)(latency + 0f - 0.016f) + Rando.Float(-jitter, jitter) + num;
            }

            public bool CalculateLoss() => loss != 0f && Rando.Float(1f) < loss;

            public bool Update(NCNetworkImplementation pNetwork)
            {
                List<DelayedPacket> delayedPacketList = new List<DelayedPacket>();
                foreach (DelayedPacket packet in packets)
                {
                    packet.time -= Maths.IncFrameTimer();
                    if (packet.time <= 0f && connection.debuggerContext.lagSpike <= 0)
                    {
                        pNetwork.OnSendPacket(packet.data.buffer, packet.data.lengthInBytes, connection.data);
                        delayedPacketList.Add(packet);
                    }
                }
                foreach (DelayedPacket delayedPacket in delayedPacketList)
                {
                    if (Rando.Int(15) == 0)
                        delayedPacket.time = Rando.Float(2f, 5f);
                    else
                        packets.Remove(delayedPacket);
                }
                if (lagSpike > 0)
                    lagSpike -= 9;
                return packets.Count == 0;
            }

            public void Reset() => packets.Clear();

            public class DelayedPacket
            {
                public BitBuffer data;
                public float time;
            }
        }
    }
}
