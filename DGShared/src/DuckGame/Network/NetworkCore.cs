// Decompiled with JetBrains decompiler
// Type: DuckGame.NetworkPacket
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections.Generic;
using System.Linq;

namespace DuckGame
{
    public class NetworkPacket
    {
        public bool valid;
        public List<NetMessage> messages = new List<NetMessage>();
        public ushort order;
        public bool serverPacket;
        public uint sessionID;
        private bool _received;
        private bool _sent;
        private BitBuffer _data;
        private NetworkConnection _receivedFrom;
        private float _timeSinceReceived;
        public NetIndex16 synchronizedTime;
        public bool dropPacket;

        public NetworkPacket(BitBuffer dat, NetworkConnection from, ushort orderVal)
        {
            if (from == null)
                throw new Exception("Network packet connection information cannot be null.");
            _data = dat;
            _receivedFrom = from;
            order = orderVal;
        }

        public bool IsValidSession() => (int)sessionID == (int)_receivedFrom.sessionID;

        public bool received => _received;

        public bool sent => _sent;

        public BitBuffer data => _data;

        public NetworkConnection connection => _receivedFrom;

        public float timeSinceReceived => _timeSinceReceived;

        public void Tick() => _timeSinceReceived += Maths.IncFrameTimer();

        public MultiMap<NetMessagePriority, NetMessage> unpackedMessages { get; private set; }

        public void Unpack()
        {
            if (unpackedMessages == null)
            {
                unpackedMessages = new MultiMap<NetMessagePriority, NetMessage>();
                if (_data.ReadBool())
                {
                    do
                    {
                        ushort num = _data.ReadUShort();
                        NetMessagePriority netMessagePriority = (NetMessagePriority)_data.ReadByte();
                        bool flag1 = false;
                        bool flag2 = netMessagePriority == NetMessagePriority.ReliableOrdered && IsValidSession();
                        if (flag2)
                        {
                            uint reliableMessageSize = connection.manager.GetExistingReceivedReliableMessageSize(num);
                            if (reliableMessageSize > 0U)
                            {
                                int positionInBits = (int)_data.positionInBits;
                                _data.positionInBits += reliableMessageSize;
                                flag1 = true;
                            }
                        }
                        if (!flag1)
                        {
                            NetMessage element = null;
                            BitBuffer bitBuffer = netMessagePriority == NetMessagePriority.ReliableOrdered || netMessagePriority == NetMessagePriority.MAX_VALUE_DONOT_USE ? _data.ReadBitBuffer() : _data;
                            uint positionInBits = _data.positionInBits;
                            ushort key = bitBuffer.ReadUShort();
                            if (netMessagePriority == NetMessagePriority.MAX_VALUE_DONOT_USE)
                            {
                                netMessagePriority = (NetMessagePriority)_data.ReadByte();
                                Mod modFromHash = ModLoader.GetModFromHash(_data.ReadUInt());
                                if (modFromHash != null)
                                    element = modFromHash.constructorToMessageID[key].Invoke(null) as NetMessage;
                                else
                                    DevConsole.Log(DCSection.DuckNet, "|GRAY|Ignoring message from unknown client mod.");
                            }
                            else
                                element = Network.constructorToMessageID[key].Invoke(null) as NetMessage;
                            if (element != null)
                            {
                                element.priority = netMessagePriority;
                                element.connection = _receivedFrom;
                                element.session = sessionID;
                                element.typeIndex = key;
                                element.order = num;
                                element.packet = this;
                                if (netMessagePriority != NetMessagePriority.ReliableOrdered)
                                    element.Deserialize(bitBuffer);
                                else
                                    element.SetSerializedData(bitBuffer);
                                uint pSize = _data.positionInBits - positionInBits;
                                if (flag2)
                                    connection.manager.StoreReceivedReliableMessageSize(num, pSize);
                                unpackedMessages.Add(element.priority, element);
                            }
                        }
                    }
                    while (_data.ReadBool());
                }
            }
            if (_data.positionInBits >= _data.lengthInBits || !_data.ReadBool())
                return;
            synchronizedTime = _data.ReadNetIndex16();
        }

        public List<NetMessage> GetAllMessages()
        {
            List<NetMessage> allMessages = new List<NetMessage>();
            foreach (NetMessagePriority key in Enum.GetValues(typeof(NetMessagePriority)).Cast<NetMessagePriority>())
            {
                List<NetMessage> list;
                if (unpackedMessages.TryGetValue(key, out list))
                    allMessages.AddRange(list);
            }
            return allMessages;
        }
    }
}
