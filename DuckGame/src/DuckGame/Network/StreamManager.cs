// Decompiled with JetBrains decompiler
// Type: DuckGame.StreamManager
//removed for regex reasons Culture=neutral, PublicKeyToken=null
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
            _unacknowledgedMessages.RemoveAll(x => x.priority != 0);
            _unreliableMessages.Clear();
            _previousReliableMessageSizes.Clear();
            _receivedUrgentPackets.Clear();
            _receivedVolatilePackets.Clear();
            _freshMessages.RemoveAll(x => x.priority != 0);
        }

        public float ping => _ping;

        public float pingPeak => _pingPeak;

        public int losses => _losses;

        public int accumulatedLoss => _lossAccumulator;

        public void RecordLoss()
        {
            lossThisFrame = true;
            ++_lossAccumulator;
            ++_losses;
        }

        public int sent => _sent;

        public float jitter => _jitter;

        public float jitterPeak => _jitterPeak;

        public static StreamManager context => NetworkConnection.context != null ? NetworkConnection.context.manager : null;

        public void LogPing(float pingVal)
        {
            if (pingVal < 0.0)
                pingVal = 0f;
            _previousPings[_currentPing % 32] = pingVal;
            ++_currentPing;
            float num = 0f;
            for (int index = 0; index < 32; ++index)
                num += _previousPings[index];
            _ping = num / 32f;
            if (_ping <= _pingPeak)
                return;
            _pingPeak = _ping;
        }

        public EventManager eventManager => _eventManager;

        public long GetPendingStates(GhostObject obj)
        {
            long pendingStates = 0;
            foreach (NetMessage unreliableMessage in _unreliableMessages)
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

        public NetworkConnection connection => _connection;

        public ushort expectedReliableOrder => _expectedReliableOrder;

        private void IncrementExpectedOrder() => _expectedReliableOrder = (ushort)((_expectedReliableOrder + 1) % ushort.MaxValue);

        public int lastReceivedAck => _lastReceivedAck;

        public StreamManager(NetworkConnection connection)
        {
            _connection = connection;
            _eventManager = new EventManager(connection, this);
        }

        public void DoAcks(HashSet<ushort> acksReceived)
        {
            if (acksReceived != null && acksReceived.Count > 0)
            {
                List<NetMessage> netMessageList = new List<NetMessage>();
                lock (_unacknowledgedMessages)
                {
                    foreach (NetMessage unacknowledgedMessage in _unacknowledgedMessages)
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
                        _unacknowledgedMessages.Remove(netMessage);
                }
                lock (_unreliableMessages)
                {
                    foreach (NetMessage unreliableMessage in _unreliableMessages)
                    {
                        foreach (ushort num in acksReceived)
                        {
                            if (unreliableMessage.packetsActive.Contains(num))
                                netMessageList.Add(unreliableMessage);
                        }
                    }
                    foreach (NetMessage netMessage in netMessageList)
                        _unreliableMessages.Remove(netMessage);
                }
                acksReceived.Clear();
                int count = _unacknowledgedMessages.Count;
                foreach (NetMessage m in netMessageList)
                    NotifyAfterMessageAck(m, false);
            }
            lock (_unreliableMessages)
            {
                List<NetMessage> netMessageList = new List<NetMessage>();
                foreach (NetMessage unreliableMessage in _unreliableMessages)
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
                    _unreliableMessages.Remove(m);
                    NotifyAfterMessageAck(m, true);
                }
            }
        }

        public void NotifyAfterMessageAck(NetMessage m, bool dropped)
        {
            if (dropped)
                RecordLoss();
            else
                m.DoMessageWasReceived();
            if (m.manager != BelongsToManager.GhostManager)
                return;
            GhostManager.context.Notify(this, m, dropped);
        }

        public uint GetExistingReceivedReliableMessageSize(ushort pMessageOrder)
        {
            uint reliableMessageSize;
            _previousReliableMessageSizes.TryGetValue(pMessageOrder, out reliableMessageSize);
            return reliableMessageSize;
        }

        public void StoreReceivedReliableMessageSize(ushort pOrder, uint pSize) => _previousReliableMessageSizes[pOrder] = pSize;

        public void MessagesReceived(List<NetMessage> messages)
        {
            foreach (NetMessage message in messages)
            {
                NetMessage m = message;
                if (connection.status != ConnectionStatus.Connected && !(m is NMNetworkCoreMessage))
                {
                    if (connection.status == ConnectionStatus.Disconnecting || connection.status == ConnectionStatus.Disconnected)
                        DevConsole.Log(DCSection.NetCore, "@error Received |WHITE|" + m.ToString() + "|PREV| while disconnecting!!@error");
                    else
                        DevConsole.Log(DCSection.NetCore, "@error Received |WHITE|" + m.ToString() + "|PREV| while connecting!!@error");
                }
                if (m.priority == NetMessagePriority.ReliableOrdered)
                {
                    if (m.order >= _expectedReliableOrder && _orderedPackets.FirstOrDefault(x => x.order == m.order) == null)
                    {
                        int index = 0;
                        while (index < _orderedPackets.Count && _orderedPackets[index].order <= m.order)
                            ++index;
                        _orderedPackets.Insert(index, m);
                    }
                }
                else if (m.priority == NetMessagePriority.Volatile)
                {
                    if (!_receivedVolatilePackets.Contains(m.order))
                    {
                        NetMessageReceived(m);
                        _receivedVolatilePackets.Add(m.order);
                        _receivedVolatilePackets.Remove((ushort)(m.order - 64U));
                    }
                }
                else if (m.priority == NetMessagePriority.Urgent)
                {
                    if (!_receivedUrgentPackets.Contains(m.order))
                    {
                        NetMessageReceived(m);
                        _receivedUrgentPackets.Add(m.order);
                        _receivedUrgentPackets.Remove((ushort)(m.order - 64U));
                    }
                }
                else
                    NetMessageReceived(m);
            }
        }

        public void NetMessageReceived(NetMessage m)
        {
            if (m.priority == NetMessagePriority.ReliableOrdered)
                return;
            ProcessReceivedMessage(m);
        }

        public void ProcessReceivedMessage(NetMessage m)  //anticrash
        {
            try
            {
                if (Network.isServer)
                {
                    if (m.connection == null)
                    {
                        return;
                    }
                    if (!NetworkConnection.Stopwatch.IsRunning)
                    {
                        NetworkConnection.Stopwatch.Restart();
                        NetworkConnection.connectmessages = new Dictionary<string, int>();
                    }
                    else if (NetworkConnection.Stopwatch.ElapsedMilliseconds > 1000L)
                    {
                        NetworkConnection.Stopwatch.Restart();
                        NetworkConnection.connectmessages = new Dictionary<string, int>();
                    }
                    if (!NetworkConnection.connectmessages.ContainsKey(m.connection.identifier))
                    {
                        NetworkConnection.connectmessages[m.connection.identifier] = 0;
                    }
                    Dictionary<string, int> dictionary = NetworkConnection.connectmessages;
                    string identifier = m.connection.identifier;
                    dictionary[identifier]++;
                    if (NetworkConnection.connectmessages[m.connection.identifier] > 1000)
                    {
                        NMVersionMismatch msg = new NMVersionMismatch(NMVersionMismatch.Type.Older, new string(' ', 37) + "|DGRED|Thats To Many Messages Bro" + new string(' ', 34) + " 0.0.0.0");
                        Send.Message(msg, m.connection);
                        Send.Message(new NMKick(), m.connection);
                        if (m.connection.profile != null)
                        {
                            Send.Message(new NMKicked(m.connection.profile));
                        }
                        m.connection.kicking = true;
                        Network.activeNetwork.core.DisconnectClient(m.connection, new DuckNetErrorInfo(DuckNetError.Kicked, ""), true);
                        if (m.connection.profile != null)
                        {
                            DuckNetwork.Kick(m.connection.profile);
                        }
                        return;
                    }
                    if (NetworkConnection.bannedmessages.Contains(m.GetType()))
                    {
                        DevConsole.Log("blocked Messsage2 " + m.GetType().Name, Color.Red, 2f, -1);
                        return;
                    }
                    if (m is NMDisconnect && (m.connection == null || m.connection == DuckNetwork.localConnection))
                    {
                        DevConsole.Log("blocked Messsage2 " + m.GetType().Name, Color.Red, 2f, -1);
                        return;
                    }
                    if (m is NMKillDuck)
                    {
                        NMKillDuck nmkillDuck = m as NMKillDuck;
                        if ((int)nmkillDuck.index < DuckNetwork.profiles.Count && (int)nmkillDuck.index > -1)
                        {
                            Profile profile = DuckNetwork.profiles[(int)nmkillDuck.index];
                            if (profile.duck != null && nmkillDuck.cook && !profile.duck.onFire)
                            {
                                return;
                            }
                        }
                    }
                    if (m is NMDeathBeam && Level.current.things[typeof(HugeLaser)].Count<Thing>() == 0)
                    {
                        return;
                    }
                    if (m is NMEnergyScimitarBlast && Level.current.things[typeof(EnergyScimitar)].Count<Thing>() == 0 && Level.current.things[typeof(OldEnergyScimi)].Count<Thing>() == 0)
                    {
                        return;
                    }
                }
            }
            catch
            {
                DevConsole.Log("ReceivedMessage PreAnticheat crash", Color.Green, 2f, -1);
                return;
            }
            try
            {
                try
                {
                    NetworkConnection.context = _connection;
                    Main.codeNumber = m.typeIndex;
                    if (m.manager == BelongsToManager.GhostManager)
                        GhostManager.context.OnMessage(m);
                    else if (m.manager == BelongsToManager.EventManager)
                        _eventManager.OnMessage(m);
                    else if (m.manager == BelongsToManager.DuckNetwork)
                        DuckNetwork.OnMessage(m);
                    else if (m.manager == BelongsToManager.None)
                        Network.OnMessageStatic(m);
                    Main.codeNumber = 12345;
                    NetworkConnection.context = null;
                }
                catch (Exception ex)
                {
                    DevConsole.Log("StreamManager ReceivedMessage catch " + ex.Message, Color.Green, 2f, -1);
                }
                return;
            }
            catch
            {
                DevConsole.Log("ReceivedMessage try log crash", Color.Green, 2f, -1);
            }
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
                msg.connection = connection;
                lock (_freshMessages)
                    _freshMessages.Add(msg);
            }
        }

        public void SendImmediatelyUnreliable(NetMessage pMessage)
        {
            if (connection == null || connection.data == null)
                return;
            pMessage.connection = connection;
            BitBuffer bitBuffer = new BitBuffer();
            NetworkPacket packet = new NetworkPacket(bitBuffer, _connection, GetPacketOrder());
            bitBuffer.Write(true);
            WriteMessageData(pMessage, bitBuffer);
            pMessage.lastTransmitted = Graphics.frame;
            pMessage.packet = packet;
            bitBuffer.Write(false);
            Network.activeNetwork.core.SendPacket(packet, _connection);
        }

        public void Update()
        {
            if (_lossAccumulator > 0)
            {
                ++_lossAccumulatorInc;
                if (_lossAccumulatorInc > 8)
                {
                    _lossAccumulatorInc = 0;
                    --_lossAccumulator;
                }
                if (_lossAccumulator > 30)
                    _lossAccumulator = 30;
            }
            if (connection.status == ConnectionStatus.Disconnecting || connection.status == ConnectionStatus.Disconnected)
                return;
            _eventManager.Update();
            bool flag;
            do
            {
                flag = false;
                Queue<NetMessage> netMessageQueue = new Queue<NetMessage>();
                for (int index = 0; index < _orderedPackets.Count; ++index)
                {
                    NetMessage orderedPacket = _orderedPackets[index];
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
                            IncrementExpectedOrder();
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
                                NetMessage netMessage = nmMessageFragment.Finish(currentFragmentCollection);
                                if (netMessage != null)
                                {
                                    _orderedPackets[index] = netMessage;
                                    DevConsole.Log(DCSection.DuckNet, "@received |DGGREEN|NMMessageFragment assembled (" + netMessage.GetType().ToString() + ")");
                                    --index;
                                    currentFragmentCollection.Clear();
                                    continue;
                                }
                                currentFragmentCollection.Clear();
                            }
                            else
                                currentFragmentCollection.Add(nmMessageFragment);
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
                                ProcessReceivedMessage(orderedPacket);
                                orderedPacket.activated = true;
                            }
                            if (orderedPacket.MessageIsCompleted())
                            {
                                _orderedPackets.RemoveAt(index);
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
            _packetOrder = (ushort)((_packetOrder + 1) % ushort.MaxValue);
            if (_packetOrder == 0)
                DevConsole.Log(DCSection.NetCore, "@error !!Packet order index wrapped!!@error");
            return _packetOrder;
        }

        private ushort GetReliableOrder()
        {
            int reliableOrder = _reliableOrder;
            _reliableOrder = (ushort)((_reliableOrder + 1) % ushort.MaxValue);
            if (_reliableOrder != 0)
                return (ushort)reliableOrder;
            DevConsole.Log(DCSection.NetCore, "@error !!Reliable message order wrapped!!@error");
            _previousReliableMessageSizes.Clear();
            return (ushort)reliableOrder;
        }

        private ushort GetVolatileID()
        {
            int volatileId = _volatileID;
            _volatileID = (ushort)((_volatileID + 1) % ushort.MaxValue);
            return (ushort)volatileId;
        }

        private ushort GetUrgentID()
        {
            int urgentId = _urgentID;
            _urgentID = (ushort)((_urgentID + 1) % ushort.MaxValue);
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
                if (_currentPacketInternal == null)
                    _currentPacketInternal = new NetworkPacket(new BitBuffer(), _connection, GetPacketOrder());
                return _currentPacketInternal;
            }
        }

        public void Flush(bool sendUnacknowledged, bool pSkipUnacknowledged = false)
        {
            List<NetMessage> _unacknowledgedMessages2 = new List<NetMessage>();
            List<NetMessage> _freshMessages2 = new List<NetMessage>();
            if (_unacknowledgedMessages.Count == 0 && _freshMessages.Count == 0 || _freshMessages.Count == 0 && !sendUnacknowledged)
                return;
            lock (_unacknowledgedMessages)
            {
                lock (_freshMessages)
                {
                    _unacknowledgedMessages2 = new List<NetMessage>(_unacknowledgedMessages);
                    _freshMessages2 = new List<NetMessage>(_freshMessages);
                    if (!pSkipUnacknowledged)
                    {
                        ++_retransmitCycle;
                        foreach (NetMessage unacknowledgedMessage in _unacknowledgedMessages)
                        {
                            if (currentPacket.data.lengthInBytes > 400)
                            {
                                DevConsole.Log(DCSection.DuckNet, "@error |DGRED|Large retransmit! (" + currentPacket.data.lengthInBytes.ToString() + ")", connection);
                                break;
                            }
                            // DevConsole.Log("Sending unacknowledged " + unacknowledgedMessage.GetType().Name);
                            if (unacknowledgedMessage.priority != NetMessagePriority.Urgent || unacknowledgedMessage.timesRetransmitted >= 2)
                            {
                                int num = (int)(MathHelper.Clamp(ping, 0.064f, 1f) * 60.0) + 1;
                                if (unacknowledgedMessage.serializedData.lengthInBytes > 500)
                                    num += 30;
                                if (unacknowledgedMessage.lastTransmitted + num > Graphics.frame)
                                    continue;
                            }
                            unacknowledgedMessage.packetsActive.Add(currentPacket.order);
                            currentPacket.data.Write(true);
                            WriteMessageData(unacknowledgedMessage, currentPacket.data);
                            unacknowledgedMessage.lastTransmitted = Graphics.frame;
                            ++unacknowledgedMessage.timesRetransmitted;
                        }
                    }
                    for (int index = 0; index < _freshMessages.Count; ++index)
                    {
                        if (true)//if (currentPacket.data.lengthInBytes <= 1000)
                        {
                            NetMessage freshMessage = _freshMessages[index];
                            if (connection.status != ConnectionStatus.Connected && !(freshMessage is IConnectionMessage))
                            {
                                if (_prevSendStatus != connection.status)
                                {
                                    _prevSendStatus = connection.status;
                                    DevConsole.Log(DCSection.DuckNet, "|DGRED|Holding back queued messages until a connection is established.", connection);
                                }
                            }
                            else if (connection.levelIndex != byte.MaxValue && connection.levelIndex != freshMessage.levelIndex && !(freshMessage is IConnectionMessage) && freshMessage.priority != NetMessagePriority.ReliableOrdered)
                            {
                                if (freshMessage.levelIndex < connection.levelIndex)
                                {
                                    _freshMessages.Remove(freshMessage);
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
                                            _freshMessages.RemoveAt(index);
                                            foreach (NMMessageFragment nmMessageFragment in NMMessageFragment.BreakApart(freshMessage))
                                            {
                                                nmMessageFragment.Serialize();
                                                _freshMessages.Insert(index, nmMessageFragment);
                                                ++index;
                                            }
                                            index = num - 1;
                                            continue;
                                        }
                                        freshMessage.order = GetReliableOrder();
                                        DevConsole.Log(DCSection.DuckNet, "@sent Sent |WHITE|" + freshMessage.ToString() + "|PREV| (" + freshMessage.order.ToString() + ")", connection);
                                        break;
                                    case NetMessagePriority.Urgent:
                                        freshMessage.order = GetUrgentID();
                                        break;
                                    case NetMessagePriority.Volatile:
                                        freshMessage.order = GetVolatileID();
                                        break;
                                }
                                freshMessage.packetsActive.Add(currentPacket.order);
                                currentPacket.data.Write(true);
                                WriteMessageData(freshMessage, currentPacket.data);
                                freshMessage.lastTransmitted = Graphics.frame;
                                freshMessage.packet = currentPacket;
                                if (freshMessage.priority != NetMessagePriority.UnreliableUnordered && freshMessage.priority != NetMessagePriority.Volatile)
                                    _unacknowledgedMessages.Add(freshMessage);
                                else if (freshMessage.priority == NetMessagePriority.Volatile)
                                {
                                    freshMessage.timeout = Math.Min(Math.Max(ping * 1.3f, 0.1f), 2f);
                                    _unreliableMessages.Add(freshMessage);
                                }
                                _freshMessages.Remove(freshMessage);
                                --index;
                            }
                        }
                        // else
                        //      break;
                    }
                }
            }
            if (currentPacket.data.lengthInBits <= 0)
                return;
            currentPacket.data.Write(false);
            ++_sent;
            Network.activeNetwork.core.SendPacket(currentPacket, _connection);
            _currentPacketInternal = null;
        }

        private class NetQueue
        {
            private ushort[] _buffer = new ushort[128];
            private int _size;
            private int _first;

            public NetQueue()
            {
                _size = 0;
                _first = 0;
            }

            public bool Contains(ushort val) => false;

            public void Add(ushort val)
            {
                if (_size < 128)
                {
                    _buffer[_size++] = val;
                }
                else
                {
                    _buffer[_first++] = val;
                    _first &= sbyte.MaxValue;
                }
            }
        }
    }
}
