// Decompiled with JetBrains decompiler
// Type: DuckGame.StreamManager
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections.Generic;
using System.Linq;

namespace DuckGame
{
    public class StreamManager
    {
        private float _ping = 1f;
        private float _pingPeak;
        private int _losses;
        private int _lossAccumulator;
        private int _lossAccumulatorInc;
        public bool lossThisFrame;
        private int _sent;
        private float _jitter;
        private float _jitterPeak;
        //private int _jitterPeakReset;
        private float[] _previousPings = new float[32];
        private int _currentPing;
        //private float _prevAverage;
        private EventManager _eventManager;
        private List<NetMessage> _unreliableMessages = new List<NetMessage>();
        private List<NetMessage> _unacknowledgedMessages = new List<NetMessage>();
        private List<NetMessage> _freshMessages = new List<NetMessage>();
        private List<NetMessage> _orderedPackets = new List<NetMessage>();
        private NetworkConnection _connection;
        private ushort _expectedReliableOrder;
        private int _lastReceivedAck = -1;
        private HashSet<ushort> _receivedVolatilePackets = new HashSet<ushort>();
        private HashSet<ushort> _receivedUrgentPackets = new HashSet<ushort>();
        private Dictionary<ushort, uint> _previousReliableMessageSizes = new Dictionary<ushort, uint>();
        private List<NMMessageFragment> currentFragmentCollection = new List<NMMessageFragment>();
        private ushort _packetOrder = 1;
        private ushort _reliableOrder;
        private ushort _volatileID = 10000;
        private ushort _urgentID = 10000;
        private NetworkPacket _currentPacketInternal;
        private ConnectionStatus _prevSendStatus = ConnectionStatus.None;
        private int _retransmitCycle;

        public void Reset()
        {
            this._unacknowledgedMessages.RemoveAll(x => x.priority != 0);
            this._unreliableMessages.Clear();
            this._previousReliableMessageSizes.Clear();
            this._receivedUrgentPackets.Clear();
            this._receivedVolatilePackets.Clear();
            this._freshMessages.RemoveAll(x => x.priority != 0);
        }

        public float ping => this._ping;

        public float pingPeak => this._pingPeak;

        public int losses => this._losses;

        public int accumulatedLoss => this._lossAccumulator;

        public void RecordLoss()
        {
            this.lossThisFrame = true;
            ++this._lossAccumulator;
            ++this._losses;
        }

        public int sent => this._sent;

        public float jitter => this._jitter;

        public float jitterPeak => this._jitterPeak;

        public static StreamManager context => NetworkConnection.context != null ? NetworkConnection.context.manager : null;

        public void LogPing(float pingVal)
        {
            if (pingVal < 0.0)
                pingVal = 0f;
            this._previousPings[this._currentPing % 32] = pingVal;
            ++this._currentPing;
            float num = 0f;
            for (int index = 0; index < 32; ++index)
                num += this._previousPings[index];
            this._ping = num / 32f;
            if (_ping <= this._pingPeak)
                return;
            this._pingPeak = this._ping;
        }

        public EventManager eventManager => this._eventManager;

        public long GetPendingStates(GhostObject obj)
        {
            long pendingStates = 0;
            foreach (NetMessage unreliableMessage in this._unreliableMessages)
            {
                if (unreliableMessage is NMGhostData)
                {
                    foreach (NMGhostData.GhostMaskPair ghostMaskPair in (unreliableMessage as NMGhostData).ghostMaskPairs)
                    {
                        if (ghostMaskPair.ghost == obj)
                            pendingStates |= ghostMaskPair.mask;
                    }
                }
            }
            return pendingStates;
        }

        public NetworkConnection connection => this._connection;

        public ushort expectedReliableOrder => this._expectedReliableOrder;

        private void IncrementExpectedOrder() => this._expectedReliableOrder = (ushort)((_expectedReliableOrder + 1) % ushort.MaxValue);

        public int lastReceivedAck => this._lastReceivedAck;

        public StreamManager(NetworkConnection connection)
        {
            this._connection = connection;
            this._eventManager = new EventManager(connection, this);
        }

        public void DoAcks(HashSet<ushort> acksReceived)
        {
            if (acksReceived != null && acksReceived.Count > 0)
            {
                List<NetMessage> netMessageList = new List<NetMessage>();
                lock (this._unacknowledgedMessages)
                {
                    foreach (NetMessage unacknowledgedMessage in this._unacknowledgedMessages)
                    {
                        foreach (ushort num in acksReceived)
                        {
                            if (unacknowledgedMessage.packetsActive.Contains(num))
                            {
                                netMessageList.Add(unacknowledgedMessage);
                                break;
                            }
                        }
                    }
                    foreach (NetMessage netMessage in netMessageList)
                        this._unacknowledgedMessages.Remove(netMessage);
                }
                lock (this._unreliableMessages)
                {
                    foreach (NetMessage unreliableMessage in this._unreliableMessages)
                    {
                        foreach (ushort num in acksReceived)
                        {
                            if (unreliableMessage.packetsActive.Contains(num))
                                netMessageList.Add(unreliableMessage);
                        }
                    }
                    foreach (NetMessage netMessage in netMessageList)
                        this._unreliableMessages.Remove(netMessage);
                }
                acksReceived.Clear();
                int count = this._unacknowledgedMessages.Count;
                foreach (NetMessage m in netMessageList)
                    this.NotifyAfterMessageAck(m, false);
            }
            lock (this._unreliableMessages)
            {
                List<NetMessage> netMessageList = new List<NetMessage>();
                foreach (NetMessage unreliableMessage in this._unreliableMessages)
                {
                    unreliableMessage.timeout -= Maths.IncFrameTimer();
                    if (unreliableMessage.timeout <= 0.0)
                        netMessageList.Add(unreliableMessage);
                }
                foreach (NetMessage m in netMessageList)
                {
                    m.queued = false;
                    m.packet = null;
                    m.packetsActive.Clear();
                    this._unreliableMessages.Remove(m);
                    this.NotifyAfterMessageAck(m, true);
                }
            }
        }

        public void NotifyAfterMessageAck(NetMessage m, bool dropped)
        {
            if (dropped)
                this.RecordLoss();
            else
                m.DoMessageWasReceived();
            if (m.manager != BelongsToManager.GhostManager)
                return;
            GhostManager.context.Notify(this, m, dropped);
        }

        public uint GetExistingReceivedReliableMessageSize(ushort pMessageOrder)
        {
            uint reliableMessageSize;
            this._previousReliableMessageSizes.TryGetValue(pMessageOrder, out reliableMessageSize);
            return reliableMessageSize;
        }

        public void StoreReceivedReliableMessageSize(ushort pOrder, uint pSize) => this._previousReliableMessageSizes[pOrder] = pSize;

        public void MessagesReceived(List<NetMessage> messages)
        {
            foreach (NetMessage message in messages)
            {
                NetMessage m = message;
                if (this.connection.status != ConnectionStatus.Connected && !(m is NMNetworkCoreMessage))
                {
                    if (this.connection.status == ConnectionStatus.Disconnecting || this.connection.status == ConnectionStatus.Disconnected)
                        DevConsole.Log(DCSection.NetCore, "@error Received |WHITE|" + m.ToString() + "|PREV| while disconnecting!!@error");
                    else
                        DevConsole.Log(DCSection.NetCore, "@error Received |WHITE|" + m.ToString() + "|PREV| while connecting!!@error");
                }
                if (m.priority == NetMessagePriority.ReliableOrdered)
                {
                    if (m.order >= _expectedReliableOrder && this._orderedPackets.FirstOrDefault<NetMessage>(x => x.order == m.order) == null)
                    {
                        int index = 0;
                        while (index < this._orderedPackets.Count && _orderedPackets[index].order <= m.order)
                            ++index;
                        this._orderedPackets.Insert(index, m);
                    }
                }
                else if (m.priority == NetMessagePriority.Volatile)
                {
                    if (!this._receivedVolatilePackets.Contains(m.order))
                    {
                        this.NetMessageReceived(m);
                        this._receivedVolatilePackets.Add(m.order);
                        this._receivedVolatilePackets.Remove((ushort)(m.order - 64U));
                    }
                }
                else if (m.priority == NetMessagePriority.Urgent)
                {
                    if (!this._receivedUrgentPackets.Contains(m.order))
                    {
                        this.NetMessageReceived(m);
                        this._receivedUrgentPackets.Add(m.order);
                        this._receivedUrgentPackets.Remove((ushort)(m.order - 64U));
                    }
                }
                else
                    this.NetMessageReceived(m);
            }
        }

        public void NetMessageReceived(NetMessage m)
        {
            if (m.priority == NetMessagePriority.ReliableOrdered)
                return;
            this.ProcessReceivedMessage(m);
        }

        public void ProcessReceivedMessage(NetMessage m)
        {
            NetworkConnection.context = this._connection;
            Main.codeNumber = m.typeIndex;
            if (m.manager == BelongsToManager.GhostManager)
                GhostManager.context.OnMessage(m);
            else if (m.manager == BelongsToManager.EventManager)
                this._eventManager.OnMessage(m);
            else if (m.manager == BelongsToManager.DuckNetwork)
                DuckNetwork.OnMessage(m);
            else if (m.manager == BelongsToManager.None)
                Network.OnMessageStatic(m);
            Main.codeNumber = 12345;
            NetworkConnection.context = null;
        }

        public void QueueMessage(NetMessage msg)
        {
            if (msg.queued)
            {
                DevConsole.Log(DCSection.NetCore, "Message has been queued twice! This shouldn't happen!! (" + msg.GetType().Name + ")");
            }
            else
            {
                if (msg.levelIndex == byte.MaxValue)
                    msg.levelIndex = DuckNetwork.levelIndex;
                msg.queued = true;
                msg.connection = this.connection;
                lock (this._freshMessages)
                    this._freshMessages.Add(msg);
            }
        }

        public void SendImmediatelyUnreliable(NetMessage pMessage)
        {
            if (this.connection == null || this.connection.data == null)
                return;
            pMessage.connection = this.connection;
            BitBuffer bitBuffer = new BitBuffer();
            NetworkPacket packet = new NetworkPacket(bitBuffer, this._connection, this.GetPacketOrder());
            bitBuffer.Write(true);
            this.WriteMessageData(pMessage, bitBuffer);
            pMessage.lastTransmitted = Graphics.frame;
            pMessage.packet = packet;
            bitBuffer.Write(false);
            Network.activeNetwork.core.SendPacket(packet, this._connection);
        }

        public void Update()
        {
            if (this._lossAccumulator > 0)
            {
                ++this._lossAccumulatorInc;
                if (this._lossAccumulatorInc > 8)
                {
                    this._lossAccumulatorInc = 0;
                    --this._lossAccumulator;
                }
                if (this._lossAccumulator > 30)
                    this._lossAccumulator = 30;
            }
            if (this.connection.status == ConnectionStatus.Disconnecting || this.connection.status == ConnectionStatus.Disconnected)
                return;
            this._eventManager.Update();
            bool flag;
            do
            {
                flag = false;
                Queue<NetMessage> netMessageQueue = new Queue<NetMessage>();
                for (int index = 0; index < this._orderedPackets.Count; ++index)
                {
                    NetMessage orderedPacket = this._orderedPackets[index];
                    orderedPacket.timeout += Maths.IncFrameTimer();
                    ushort expectedReliableOrder;
                    if (orderedPacket.timeout > 2.0 && orderedPacket.timeout < 3.0)
                    {
                        orderedPacket.timeout = 1000f;
                        string[] strArray = new string[7]
                        {
              "@disconnect Ordered message |WHITE|",
              orderedPacket.ToString(),
              "|PREV| (",
              orderedPacket.order.ToString(),
              "->",
              null,
              null
                        };
                        expectedReliableOrder = this.expectedReliableOrder;
                        strArray[5] = expectedReliableOrder.ToString();
                        strArray[6] = ") Has been stuck in queue for 2 seconds...";
                        DevConsole.Log(DCSection.DuckNet, string.Concat(strArray), orderedPacket.connection);
                    }
                    if (orderedPacket.order <= _expectedReliableOrder)
                    {
                        if (orderedPacket.order == _expectedReliableOrder)
                            this.IncrementExpectedOrder();
                        if (orderedPacket.serializedData != null)
                        {
                            NetworkConnection.context = orderedPacket.connection;
                            orderedPacket.Deserialize(orderedPacket.serializedData);
                            orderedPacket.ClearSerializedData();
                            NetworkConnection.context = null;
                        }
                        if (orderedPacket is NMMessageFragment)
                        {
                            NMMessageFragment nmMessageFragment = orderedPacket as NMMessageFragment;
                            if (nmMessageFragment.finalFragment)
                            {
                                NetMessage netMessage = nmMessageFragment.Finish(this.currentFragmentCollection);
                                if (netMessage != null)
                                {
                                    this._orderedPackets[index] = netMessage;
                                    DevConsole.Log(DCSection.DuckNet, "@received |DGGREEN|NMMessageFragment assembled (" + netMessage.GetType().ToString() + ")");
                                    --index;
                                    this.currentFragmentCollection.Clear();
                                    continue;
                                }
                                this.currentFragmentCollection.Clear();
                            }
                            else
                                this.currentFragmentCollection.Add(nmMessageFragment);
                        }
                        if (!(orderedPacket is ConditionalMessage) || (orderedPacket as ConditionalMessage).Update())
                        {
                            if (!orderedPacket.activated)
                            {
                                string[] strArray = new string[7]
                                {
                  "@received Activating |WHITE|",
                  orderedPacket.ToString(),
                  "|PREV| (",
                  orderedPacket.order.ToString(),
                  "->",
                  null,
                  null
                                };
                                expectedReliableOrder = this.expectedReliableOrder;
                                strArray[5] = expectedReliableOrder.ToString();
                                strArray[6] = ")";
                                DevConsole.Log(DCSection.DuckNet, string.Concat(strArray), orderedPacket.connection);
                                this.ProcessReceivedMessage(orderedPacket);
                                orderedPacket.activated = true;
                            }
                            if (orderedPacket.MessageIsCompleted())
                            {
                                this._orderedPackets.RemoveAt(index);
                                --index;
                                flag = true;
                            }
                            else
                                break;
                        }
                        else
                            break;
                    }
                }
            }
            while (flag);
        }

        private ushort GetPacketOrder()
        {
            this._packetOrder = (ushort)((_packetOrder + 1) % ushort.MaxValue);
            if (this._packetOrder == 0)
                DevConsole.Log(DCSection.NetCore, "@error !!Packet order index wrapped!!@error");
            return this._packetOrder;
        }

        private ushort GetReliableOrder()
        {
            int reliableOrder = _reliableOrder;
            this._reliableOrder = (ushort)((_reliableOrder + 1) % ushort.MaxValue);
            if (this._reliableOrder != 0)
                return (ushort)reliableOrder;
            DevConsole.Log(DCSection.NetCore, "@error !!Reliable message order wrapped!!@error");
            this._previousReliableMessageSizes.Clear();
            return (ushort)reliableOrder;
        }

        private ushort GetVolatileID()
        {
            int volatileId = _volatileID;
            this._volatileID = (ushort)((_volatileID + 1) % ushort.MaxValue);
            return (ushort)volatileId;
        }

        private ushort GetUrgentID()
        {
            int urgentId = _urgentID;
            this._urgentID = (ushort)((_urgentID + 1) % ushort.MaxValue);
            return (ushort)urgentId;
        }

        public void WriteMessageData(NetMessage pMessage, BitBuffer pData)
        {
            pData.Write(pMessage.order);
            Mod fromTypeIgnoreCore = ModLoader.GetModFromTypeIgnoreCore(pMessage.GetType());
            if (fromTypeIgnoreCore != null && fromTypeIgnoreCore is DisabledMod)
            {
                pData.Write((byte)4);
                pData.Write(pMessage.serializedData, true);
                pData.Write((byte)pMessage.priority);
                pData.Write(fromTypeIgnoreCore.identifierHash);
            }
            else
            {
                pData.Write((byte)pMessage.priority);
                if (pMessage.priority == NetMessagePriority.ReliableOrdered)
                    pData.Write(pMessage.serializedData, true);
                else
                    pData.WriteBufferData(pMessage.serializedData);
            }
        }

        private NetworkPacket currentPacket
        {
            get
            {
                if (this._currentPacketInternal == null)
                    this._currentPacketInternal = new NetworkPacket(new BitBuffer(), this._connection, this.GetPacketOrder());
                return this._currentPacketInternal;
            }
        }

        public void Flush(bool sendUnacknowledged, bool pSkipUnacknowledged = false)
        {
            if (this._unacknowledgedMessages.Count == 0 && this._freshMessages.Count == 0 || this._freshMessages.Count == 0 && !sendUnacknowledged)
                return;
            lock (this._unacknowledgedMessages)
            {
                lock (this._freshMessages)
                {
                    if (!pSkipUnacknowledged)
                    {
                        ++this._retransmitCycle;
                        foreach (NetMessage unacknowledgedMessage in this._unacknowledgedMessages)
                        {
                            if (this.currentPacket.data.lengthInBytes > 400)
                            {
                                DevConsole.Log(DCSection.DuckNet, "@error |DGRED|Large retransmit! (" + this.currentPacket.data.lengthInBytes.ToString() + ")", this.connection);
                                break;
                            }
                            if (unacknowledgedMessage.priority != NetMessagePriority.Urgent || unacknowledgedMessage.timesRetransmitted >= 2)
                            {
                                int num = (int)(MathHelper.Clamp(this.ping, 0.064f, 1f) * 60.0) + 1;
                                if (unacknowledgedMessage.serializedData.lengthInBytes > 500)
                                    num += 30;
                                if (unacknowledgedMessage.lastTransmitted + num > Graphics.frame)
                                    continue;
                            }
                            unacknowledgedMessage.packetsActive.Add(this.currentPacket.order);
                            this.currentPacket.data.Write(true);
                            this.WriteMessageData(unacknowledgedMessage, this.currentPacket.data);
                            unacknowledgedMessage.lastTransmitted = Graphics.frame;
                            ++unacknowledgedMessage.timesRetransmitted;
                        }
                    }
                    for (int index = 0; index < this._freshMessages.Count; ++index)
                    {
                        if (this.currentPacket.data.lengthInBytes <= 1000)
                        {
                            NetMessage freshMessage = this._freshMessages[index];
                            if (this.connection.status != ConnectionStatus.Connected && !(freshMessage is IConnectionMessage))
                            {
                                if (this._prevSendStatus != this.connection.status)
                                {
                                    this._prevSendStatus = this.connection.status;
                                    DevConsole.Log(DCSection.DuckNet, "|DGRED|Holding back queued messages until a connection is established.", this.connection);
                                }
                            }
                            else if (this.connection.levelIndex != byte.MaxValue && connection.levelIndex != freshMessage.levelIndex && !(freshMessage is IConnectionMessage) && freshMessage.priority != NetMessagePriority.ReliableOrdered)
                            {
                                if (freshMessage.levelIndex < connection.levelIndex)
                                {
                                    this._freshMessages.Remove(freshMessage);
                                    --index;
                                }
                            }
                            else
                            {
                                switch (freshMessage.priority)
                                {
                                    case NetMessagePriority.ReliableOrdered:
                                        if (!(freshMessage is INetworkChunk) && NMMessageFragment.FragmentsRequired(freshMessage) > 1)
                                        {
                                            int num = index;
                                            this._freshMessages.RemoveAt(index);
                                            foreach (NMMessageFragment nmMessageFragment in NMMessageFragment.BreakApart(freshMessage))
                                            {
                                                nmMessageFragment.Serialize();
                                                this._freshMessages.Insert(index, nmMessageFragment);
                                                ++index;
                                            }
                                            index = num - 1;
                                            continue;
                                        }
                                        freshMessage.order = this.GetReliableOrder();
                                        DevConsole.Log(DCSection.DuckNet, "@sent Sent |WHITE|" + freshMessage.ToString() + "|PREV| (" + freshMessage.order.ToString() + ")", this.connection);
                                        break;
                                    case NetMessagePriority.Urgent:
                                        freshMessage.order = this.GetUrgentID();
                                        break;
                                    case NetMessagePriority.Volatile:
                                        freshMessage.order = this.GetVolatileID();
                                        break;
                                }
                                freshMessage.packetsActive.Add(this.currentPacket.order);
                                this.currentPacket.data.Write(true);
                                this.WriteMessageData(freshMessage, this.currentPacket.data);
                                freshMessage.lastTransmitted = Graphics.frame;
                                freshMessage.packet = this.currentPacket;
                                if (freshMessage.priority != NetMessagePriority.UnreliableUnordered && freshMessage.priority != NetMessagePriority.Volatile)
                                    this._unacknowledgedMessages.Add(freshMessage);
                                else if (freshMessage.priority == NetMessagePriority.Volatile)
                                {
                                    freshMessage.timeout = Math.Min(Math.Max(this.ping * 1.3f, 0.1f), 2f);
                                    this._unreliableMessages.Add(freshMessage);
                                }
                                this._freshMessages.Remove(freshMessage);
                                --index;
                            }
                        }
                        else
                            break;
                    }
                }
            }
            if (this.currentPacket.data.lengthInBits <= 0)
                return;
            this.currentPacket.data.Write(false);
            ++this._sent;
            Network.activeNetwork.core.SendPacket(this.currentPacket, this._connection);
            this._currentPacketInternal = null;
        }

        private class NetQueue
        {
            private ushort[] _buffer = new ushort[128];
            private int _size;
            private int _first;

            public NetQueue()
            {
                this._size = 0;
                this._first = 0;
            }

            public bool Contains(ushort val) => false;

            public void Add(ushort val)
            {
                if (this._size < 128)
                {
                    this._buffer[this._size++] = val;
                }
                else
                {
                    this._buffer[this._first++] = val;
                    this._first &= sbyte.MaxValue;
                }
            }
        }
    }
}
