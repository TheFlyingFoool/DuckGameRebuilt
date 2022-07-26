// Decompiled with JetBrains decompiler
// Type: DuckGame.NetMessage
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
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
            get => this._connection;
            set => this._connection = value;
        }

        public override string ToString() => this.GetType().Name;

        public virtual void CopyTo(NetMessage pMessage)
        {
        }

        public NetworkPacket packet
        {
            get => this._packet;
            set => this._packet = value;
        }

        public virtual bool MessageIsCompleted() => true;

        public BitBuffer serializedData => this._serializedData;

        public void SetSerializedData(BitBuffer data) => this._serializedData = data;

        public void ClearSerializedData() => this._serializedData = (BitBuffer)null;

        public void Deserialize(BitBuffer msg) => this.OnDeserialize(msg);

        private FieldInfo[] getFields()
        {
            System.Type type = this.GetType();
            FieldInfo[] fields1 = (FieldInfo[])null;
            if (NetMessage._messageFields.TryGetValue(type, out fields1))
                return fields1;
            FieldInfo[] fields2 = type.GetFields();
            List<FieldInfo> fieldInfoList = new List<FieldInfo>();
            foreach (FieldInfo fieldInfo in fields2)
            {
                if (fieldInfo.DeclaringType != typeof(NetMessage))
                    fieldInfoList.Add(fieldInfo);
            }
            FieldInfo[] array = fieldInfoList.ToArray();
            NetMessage._messageFields[type] = array;
            return array;
        }

        public virtual void OnDeserialize(BitBuffer msg)
        {
            foreach (FieldInfo field in this.getFields())
            {
                if (field.FieldType == typeof(string))
                    field.SetValue((object)this, (object)msg.ReadString());
                else if (field.FieldType == typeof(float))
                    field.SetValue((object)this, (object)msg.ReadFloat());
                else if (field.FieldType == typeof(bool) && field.Name != "activated" && field.Name != "queued")
                    field.SetValue((object)this, (object)msg.ReadBool());
                else if (field.FieldType == typeof(byte))
                    field.SetValue((object)this, (object)msg.ReadByte());
                else if (field.FieldType == typeof(sbyte))
                    field.SetValue((object)this, (object)msg.ReadSByte());
                else if (field.FieldType == typeof(double))
                    field.SetValue((object)this, (object)msg.ReadDouble());
                else if (field.FieldType == typeof(int))
                    field.SetValue((object)this, (object)msg.ReadInt());
                else if (field.FieldType == typeof(ulong))
                    field.SetValue((object)this, (object)msg.ReadULong());
                else if (field.FieldType == typeof(uint))
                    field.SetValue((object)this, (object)msg.ReadUInt());
                else if (field.FieldType == typeof(ushort) && field.Name != "order" && field.Name != "typeIndex")
                    field.SetValue((object)this, (object)msg.ReadUShort());
                else if (field.FieldType == typeof(short))
                    field.SetValue((object)this, (object)msg.ReadShort());
                else if (field.FieldType == typeof(NetIndex4) && field.Name != "session")
                    field.SetValue((object)this, (object)msg.ReadNetIndex4());
                else if (field.FieldType == typeof(NetIndex16))
                    field.SetValue((object)this, (object)msg.ReadNetIndex16());
                else if (field.FieldType == typeof(Vec2))
                    field.SetValue((object)this, (object)new Vec2()
                    {
                        x = msg.ReadFloat(),
                        y = msg.ReadFloat()
                    });
                else if (field.FieldType == typeof(Profile))
                    field.SetValue((object)this, (object)msg.ReadProfile());
                else if (field.FieldType == typeof(Team))
                    field.SetValue((object)this, (object)msg.ReadTeam());
                else if (typeof(Thing).IsAssignableFrom(field.FieldType))
                {
                    Thing thing = msg.ReadThing(field.FieldType);
                    if (thing == null || field.FieldType.IsAssignableFrom(thing.GetType()))
                        field.SetValue((object)this, (object)thing);
                    else
                        DevConsole.Log("|DGRED|NetMessage.OnDeserialize invalid assignment (" + field.FieldType.Name + " = " + thing.GetType().Name + ")");
                }
            }
        }

        public BitBuffer Serialize()
        {
            if (this._serializedData != null)
                return this._serializedData;
            this._serializedData = new BitBuffer();
            this._serializedData.Write(Network.allMessageTypesToID[this.GetType()]);
            this.OnSerialize();
            return this._serializedData;
        }

        public void SerializePacketData()
        {
            this._serializedData = (BitBuffer)null;
            this.OnSerialize();
        }

        public BitBuffer SerializeToBitBuffer()
        {
            this._serializedData = new BitBuffer();
            this.OnSerialize();
            return this._serializedData;
        }

        protected virtual void OnSerialize()
        {
            if (this._serializedData == null)
                this._serializedData = new BitBuffer();
            foreach (FieldInfo field in this.getFields())
            {
                if (field.FieldType == typeof(string))
                    this._serializedData.Write(field.GetValue((object)this) as string);
                else if (field.FieldType == typeof(float))
                    this._serializedData.Write((float)field.GetValue((object)this));
                else if (field.FieldType == typeof(bool))
                    this._serializedData.Write((bool)field.GetValue((object)this));
                else if (field.FieldType == typeof(byte))
                    this._serializedData.Write((byte)field.GetValue((object)this));
                else if (field.FieldType == typeof(sbyte))
                    this._serializedData.Write((sbyte)field.GetValue((object)this));
                else if (field.FieldType == typeof(double))
                    this._serializedData.Write((double)field.GetValue((object)this));
                else if (field.FieldType == typeof(int))
                    this._serializedData.Write((int)field.GetValue((object)this));
                else if (field.FieldType == typeof(ulong))
                    this._serializedData.Write((ulong)field.GetValue((object)this));
                else if (field.FieldType == typeof(uint))
                    this._serializedData.Write((uint)field.GetValue((object)this));
                else if (field.FieldType == typeof(ushort))
                    this._serializedData.Write((ushort)field.GetValue((object)this));
                else if (field.FieldType == typeof(short))
                    this._serializedData.Write((short)field.GetValue((object)this));
                else if (field.FieldType == typeof(NetIndex4))
                    this._serializedData.WritePacked((int)(NetIndex4)field.GetValue((object)this), 4);
                else if (field.FieldType == typeof(NetIndex16))
                    this._serializedData.WritePacked((int)(NetIndex16)field.GetValue((object)this), 16);
                else if (field.FieldType == typeof(Vec2))
                {
                    Vec2 vec2 = (Vec2)field.GetValue((object)this);
                    this._serializedData.Write(vec2.x);
                    this._serializedData.Write(vec2.y);
                }
                else if (field.FieldType == typeof(Profile))
                    this._serializedData.WriteProfile((Profile)field.GetValue((object)this));
                else if (field.FieldType == typeof(Team))
                    this._serializedData.WriteTeam((Team)field.GetValue((object)this));
                else if (typeof(Thing).IsAssignableFrom(field.FieldType))
                    this._serializedData.Write((object)(field.GetValue((object)this) as Thing));
            }
        }

        public virtual void DoMessageWasReceived()
        {
            if (!this._wasReceived)
                this.MessageWasReceived();
            this._wasReceived = true;
        }

        public virtual void MessageWasReceived()
        {
        }
    }
}
