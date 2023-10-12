// Decompiled with JetBrains decompiler
// Type: DuckGame.NetMessage
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System.Collections.Generic;
using System.Reflection;

namespace DuckGame
{
    public class NetMessage
    {
        public byte levelIndex = byte.MaxValue;
        private NetworkConnection _connection;
        public ushort order;
        public bool activated;
        public bool queued;
        public ushort typeIndex;
        public uint session;
        public HashSet<ushort> packetsActive = new HashSet<ushort>();
        public float timeout;
        public long lastTransmitted;
        public byte timesRetransmitted;
        private NetworkPacket _packet;
        public NetMessagePriority priority;
        public BelongsToManager manager;
        protected BitBuffer _serializedData;
        private static Dictionary<System.Type, FieldInfo[]> _messageFields = new Dictionary<System.Type, FieldInfo[]>();
        private bool _wasReceived;

        public NetworkConnection connection
        {
            get => _connection;
            set => _connection = value;
        }

        public override string ToString() => GetType().Name;

        public virtual void CopyTo(NetMessage pMessage)
        {
        }

        public NetworkPacket packet
        {
            get => _packet;
            set => _packet = value;
        }

        public virtual bool MessageIsCompleted() => true;

        public BitBuffer serializedData => _serializedData;

        public void SetSerializedData(BitBuffer data) => _serializedData = data;

        public void ClearSerializedData() => _serializedData = null;

        public void Deserialize(BitBuffer msg) => OnDeserialize(msg);

        private FieldInfo[] getFields()
        {
            System.Type type = GetType();
            FieldInfo[] fields1;
            if (_messageFields.TryGetValue(type, out fields1))
                return fields1;
            FieldInfo[] fields2 = type.GetFields();
            List<FieldInfo> fieldInfoList = new List<FieldInfo>();
            foreach (FieldInfo fieldInfo in fields2)
            {
                if (fieldInfo.DeclaringType != typeof(NetMessage))
                    fieldInfoList.Add(fieldInfo);
            }
            FieldInfo[] array = fieldInfoList.ToArray();
            _messageFields[type] = array;
            return array;
        }

        public virtual void OnDeserialize(BitBuffer msg)
        {
            Main.SpecialCode2 = "hyp 10";
            foreach (FieldInfo field in getFields())
            {
                if (field.FieldType == typeof(string))
                    field.SetValue(this, msg.ReadString());
                else if (field.FieldType == typeof(float))
                    field.SetValue(this, msg.ReadFloat());
                else if (field.FieldType == typeof(bool) && field.Name != "activated" && field.Name != "queued")
                    field.SetValue(this, msg.ReadBool());
                else if (field.FieldType == typeof(byte))
                    field.SetValue(this, msg.ReadByte());
                else if (field.FieldType == typeof(sbyte))
                    field.SetValue(this, msg.ReadSByte());
                else if (field.FieldType == typeof(double))
                    field.SetValue(this, msg.ReadDouble());
                else if (field.FieldType == typeof(int))
                    field.SetValue(this, msg.ReadInt());
                else if (field.FieldType == typeof(ulong))
                    field.SetValue(this, msg.ReadULong());
                else if (field.FieldType == typeof(uint))
                    field.SetValue(this, msg.ReadUInt());
                else if (field.FieldType == typeof(ushort) && field.Name != "order" && field.Name != "typeIndex")
                    field.SetValue(this, msg.ReadUShort());
                else if (field.FieldType == typeof(short))
                    field.SetValue(this, msg.ReadShort());
                else if (field.FieldType == typeof(NetIndex4) && field.Name != "session")
                    field.SetValue(this, msg.ReadNetIndex4());
                else if (field.FieldType == typeof(NetIndex16))
                    field.SetValue(this, msg.ReadNetIndex16());
                else if (field.FieldType == typeof(Vec2))
                    field.SetValue(this, new Vec2()
                    {
                        x = msg.ReadFloat(),
                        y = msg.ReadFloat()
                    });
                else if (field.FieldType == typeof(Profile))
                    field.SetValue(this, msg.ReadProfile());
                else if (field.FieldType == typeof(Team))
                    field.SetValue(this, msg.ReadTeam());
                else if (typeof(Thing).IsAssignableFrom(field.FieldType))
                {
                    Thing thing = msg.ReadThing(field.FieldType);
                    if (thing == null || field.FieldType.IsAssignableFrom(thing.GetType()))
                        field.SetValue(this, thing);
                    else
                        DevConsole.Log("|DGRED|NetMessage.OnDeserialize invalid assignment (" + field.FieldType.Name + " = " + thing.GetType().Name + ")");
                }
            }
            Main.SpecialCode2 = "hyp 11";
        }

        public BitBuffer Serialize()
        {
            if (_serializedData != null)
                return _serializedData;
            _serializedData = new BitBuffer();
            _serializedData.Write(Network.allMessageTypesToID[GetType()]);
            OnSerialize();
            return _serializedData;
        }

        public void SerializePacketData()
        {
            _serializedData = null;
            OnSerialize();
        }

        public BitBuffer SerializeToBitBuffer()
        {
            _serializedData = new BitBuffer();
            OnSerialize();
            return _serializedData;
        }

        protected virtual void OnSerialize()
        {
            if (_serializedData == null)
                _serializedData = new BitBuffer();
            foreach (FieldInfo field in getFields())
            {
                if (field.FieldType == typeof(string))
                    _serializedData.Write(field.GetValue(this) as string);
                else if (field.FieldType == typeof(float))
                    _serializedData.Write((float)field.GetValue(this));
                else if (field.FieldType == typeof(bool))
                    _serializedData.Write((bool)field.GetValue(this));
                else if (field.FieldType == typeof(byte))
                    _serializedData.Write((byte)field.GetValue(this));
                else if (field.FieldType == typeof(sbyte))
                    _serializedData.Write((sbyte)field.GetValue(this));
                else if (field.FieldType == typeof(double))
                    _serializedData.Write((double)field.GetValue(this));
                else if (field.FieldType == typeof(int))
                    _serializedData.Write((int)field.GetValue(this));
                else if (field.FieldType == typeof(ulong))
                    _serializedData.Write((ulong)field.GetValue(this));
                else if (field.FieldType == typeof(uint))
                    _serializedData.Write((uint)field.GetValue(this));
                else if (field.FieldType == typeof(ushort))
                    _serializedData.Write((ushort)field.GetValue(this));
                else if (field.FieldType == typeof(short))
                    _serializedData.Write((short)field.GetValue(this));
                else if (field.FieldType == typeof(NetIndex4))
                    _serializedData.WritePacked((int)(NetIndex4)field.GetValue(this), 4);
                else if (field.FieldType == typeof(NetIndex16))
                    _serializedData.WritePacked((int)(NetIndex16)field.GetValue(this), 16);
                else if (field.FieldType == typeof(Vec2))
                {
                    Vec2 vec2 = (Vec2)field.GetValue(this);
                    _serializedData.Write(vec2.x);
                    _serializedData.Write(vec2.y);
                }
                else if (field.FieldType == typeof(Profile))
                    _serializedData.WriteProfile((Profile)field.GetValue(this));
                else if (field.FieldType == typeof(Team))
                    _serializedData.WriteTeam((Team)field.GetValue(this));
                else if (typeof(Thing).IsAssignableFrom(field.FieldType))
                    _serializedData.Write(field.GetValue(this) as Thing);
            }
        }

        public virtual void DoMessageWasReceived()
        {
            if (!_wasReceived)
                MessageWasReceived();
            _wasReceived = true;
        }

        public virtual void MessageWasReceived()
        {
        }
    }
}
