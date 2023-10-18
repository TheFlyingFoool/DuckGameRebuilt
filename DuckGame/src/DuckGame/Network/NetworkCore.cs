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
						ushort messageOrder = _data.ReadUShort();
						NetMessagePriority priority = (NetMessagePriority)_data.ReadByte();
						bool skipDeserialize = false;
						bool messageSizeCanBeStored = priority == NetMessagePriority.ReliableOrdered && IsValidSession();
						if (messageSizeCanBeStored)
						{
							uint existingSize = connection.manager.GetExistingReceivedReliableMessageSize(messageOrder);
							if (existingSize > 0U)
							{
								_data.positionInBits += existingSize;
								skipDeserialize = true;
							}
						}
						if (!skipDeserialize)
						{
							NetMessage message = null;
							BitBuffer readBuffer = (priority == NetMessagePriority.ReliableOrdered || priority == NetMessagePriority.MAX_VALUE_DONOT_USE) ? _data.ReadBitBuffer(true) : _data;
							uint sizeBefore = _data.positionInBits;
							ushort messageType = readBuffer.ReadUShort();
							if (priority == NetMessagePriority.MAX_VALUE_DONOT_USE)
							{
								priority = (NetMessagePriority)_data.ReadByte();
								Mod i = ModLoader.GetModFromHash(_data.ReadUInt());
								if (i != null)
								{
									message = (i.constructorToMessageID[messageType].Invoke(null) as NetMessage);
								}
								else
								{
									DevConsole.Log(DCSection.DuckNet, "|GRAY|Ignoring message from unknown client mod.", -1);
								}
							}
							else
							{
								message = (Network.constructorToMessageID[messageType].Invoke(null) as NetMessage);
							}
							if (message != null)
							{
								message.priority = priority;
								message.connection = _receivedFrom;
								message.session = sessionID;
								message.typeIndex = messageType;
								message.order = messageOrder;
								message.packet = this;
								if (priority != NetMessagePriority.ReliableOrdered)
								{
									message.Deserialize(readBuffer);
								}
								else
								{
									message.SetSerializedData(readBuffer);
								}
								uint sizeInBits = _data.positionInBits - sizeBefore;
								if (messageSizeCanBeStored)
								{
									connection.manager.StoreReceivedReliableMessageSize(messageOrder, sizeInBits);
								}
								unpackedMessages.Add(message.priority, message);
							}
						}
					}
					while (_data.ReadBool());
				}
			}
			if ((ulong)_data.positionInBits < (ulong)((long)_data.lengthInBits) && _data.ReadBool())
			{
				synchronizedTime = _data.ReadNetIndex16();
			}
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
