// Decompiled with JetBrains decompiler
// Type: DuckGame.BinaryClassChunk
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DuckGame
{
    public class BinaryClassChunk
    {
        private BinaryClassChunk _extraHeaderInfo;
        public bool ignore;
        private long _magicNumber;
        private ushort _version;
        private uint _size;
        private uint _offset;
        private uint _checksum;
        private BitBuffer _data;
        private BitBuffer _serializedData;
        private Dictionary<string, BinaryClassChunk> _headerDictionary = new Dictionary<string, BinaryClassChunk>();
        private MultiMap<string, object> _extraProperties;
        private DeserializeResult _result;
        private Exception _exception;
        public static bool fullDeserializeMode;

        public BinaryClassChunk GetExtraHeaderInfo() => this._extraHeaderInfo;

        public void SetExtraHeaderInfo(BinaryClassChunk pValue) => this._extraHeaderInfo = pValue;

        public T Header<T>() => (T)(object)this._extraHeaderInfo;

        public ushort GetVersion() => this._version;

        public BitBuffer GetData()
        {
            if (this._data == null && this._serializedData == null)
                this._serializedData = this.Serialize();
            return this._serializedData == null ? this._data : this._serializedData;
        }

        public Exception GetException() => this._exception;

        public DeserializeResult GetResult() => this._result;

        public uint GetChecksum()
        {
            if (this._data == null && this._serializedData == null)
                this._serializedData = this.Serialize();
            if (this._checksum == 0U)
                this._checksum = Editor.Checksum(this.GetData().buffer, (int)this._offset, (int)this._size);
            return this._checksum;
        }

        public void AddProperty(string id, object value)
        {
            if (this._extraProperties == null)
                this._extraProperties = new MultiMap<string, object>();
            this._extraProperties.Add(id, value);
        }

        public T GetProperty<T>(string id)
        {
            object property = this.GetProperty(id);
            return property == null ? default(T) : (T)property;
        }

        public List<T> GetProperties<T>(string id)
        {
            List<object> property = this.GetProperty(id, true) as List<object>;
            List<T> properties = new List<T>();
            foreach (object obj in property)
                properties.Add((T)obj);
            return properties;
        }

        public bool HasProperty(string id) => this._extraProperties != null && this._extraProperties.ContainsKey(id);

        public object GetProperty(string id, bool multiple = false)
        {
            if (this._extraProperties == null)
                return null;
            List<object> list;
            this._extraProperties.TryGetValue(id, out list);
            if (list != null)
            {
                foreach (object property in list)
                {
                    if (property is BinaryClassChunk)
                    {
                        BinaryClassChunk binaryClassChunk = property as BinaryClassChunk;
                        if (binaryClassChunk._result == DeserializeResult.HeaderDeserialized)
                            binaryClassChunk.Deserialize();
                    }
                    if (property is BitBuffer)
                        (property as BitBuffer).SeekToStart();
                    if (!multiple)
                        return property;
                }
            }
            return list;
        }

        public T GetPrimitive<T>(string id)
        {
            if (this._extraProperties == null)
                return default(T);
            List<object> list;
            this._extraProperties.TryGetValue(id, out list);
            return list != null ? (T)list.First<object>() : default(T);
        }

        public void SetData(BitBuffer data) => this.SetData(data, false);

        public bool SetData(BitBuffer data, bool pHeaderOnly)
        {
            BinaryClassChunk.DeserializeHeader(this.GetType(), data, this);
            if (!pHeaderOnly && this._result == DeserializeResult.HeaderDeserialized)
                this.Deserialize();
            return this._result == DeserializeResult.HeaderDeserialized;
        }

        public static T FromData<T>(BitBuffer data) where T : BinaryClassChunk => BinaryClassChunk.FromData<T>(data, false);

        public static T FromData<T>(BitBuffer data, bool pHeaderOnly) where T : BinaryClassChunk
        {
            BinaryClassChunk instance = Activator.CreateInstance(typeof(T), null) as BinaryClassChunk;
            instance.SetData(data, pHeaderOnly);
            return (T)(object)instance;
        }

        private Array DeserializeArray(System.Type type, System.Type arrayType, BitBuffer data)
        {
            int length = this._data.ReadInt();
            Array instance = Array.CreateInstance(arrayType, length);
            for (int index = 0; index < length; ++index)
            {
                int num = this._data.ReadBool() ? 1 : 0;
                object obj = null;
                if (num != 0)
                {
                    if (typeof(BinaryClassChunk).IsAssignableFrom(arrayType))
                    {
                        BinaryClassChunk binaryClassChunk = BinaryClassChunk.DeserializeHeader(arrayType, this._data, root: false, skipData: true);
                        binaryClassChunk?.Deserialize();
                        obj = binaryClassChunk;
                    }
                    else
                        obj = this._data.Read(arrayType);
                }
                instance.SetValue(obj, index);
            }
            return instance;
        }

        public bool Deserialize()
        {
            if (this._data == null)
            {
                this._result = DeserializeResult.NoData;
                return false;
            }
            if (this._result == DeserializeResult.Success)
                return true;
            try
            {
                this._data.position = (int)this._offset;
                ushort num1 = this._data.ReadUShort();
                System.Type type = this.GetType();
                for (int index = 0; index < num1; ++index)
                {
                    string str = this._data.ReadString();
                    System.Type key = null;
                    ClassMember classMember = null;
                    byte num2 = 0;
                    if (str.StartsWith("@"))
                    {
                        if (this._extraProperties == null)
                            this._extraProperties = new MultiMap<string, object>();
                        num2 = this._data.ReadByte();
                        if (num2 != byte.MaxValue)
                        {
                            if ((num2 & 1) != 0)
                            {
                                num2 >>= 1;
                                BinaryClassMember.typeMap.TryGetKey(num2, out key);
                            }
                            else
                                key = Editor.GetType(this._data.ReadString());
                            str = str.Substring(1, str.Length - 1);
                        }
                    }
                    else
                    {
                        classMember = Editor.GetMember(type, str);
                        if (classMember != null)
                            key = classMember.type;
                    }
                    if (num2 != byte.MaxValue)
                    {
                        if (key != null)
                        {
                            string fullName = key.FullName;
                        }
                        if (classMember != null)
                        {
                            string name = classMember.name;
                            if (classMember.field != null)
                                classMember.field.GetType();
                            else if (classMember.property != null)
                                classMember.property.GetType();
                        }
                        uint num3 = this._data.ReadUInt();
                        if (num3 != 0U)
                        {
                            if (key != null)
                            {
                                int position = this._data.position;
                                if (typeof(BinaryClassChunk).IsAssignableFrom(key))
                                {
                                    BinaryClassChunk element = BinaryClassChunk.DeserializeHeader(key, this._data, root: false);
                                    if (BinaryClassChunk.fullDeserializeMode && element._result == DeserializeResult.HeaderDeserialized)
                                        element.Deserialize();
                                    if (classMember == null)
                                        this._extraProperties.Add(str, element);
                                    else
                                        this._headerDictionary[str] = element;
                                    this._data.position = position + (int)num3;
                                }
                                else if (key.IsArray)
                                {
                                    Array element = this.DeserializeArray(key, key.GetElementType(), this._data);
                                    if (classMember == null)
                                        this._extraProperties.Add(str, element);
                                    else
                                        classMember.SetValue(this, element);
                                }
                                else if (key.IsGenericType && key.GetGenericTypeDefinition() == typeof(List<>))
                                {
                                    Array array = this.DeserializeArray(typeof(object[]), key.GetGenericArguments()[0], this._data);
                                    IList instance = Activator.CreateInstance(key) as IList;
                                    foreach (object obj in array)
                                        instance.Add(obj);
                                    if (classMember == null)
                                        this._extraProperties.Add(str, instance);
                                    else
                                        classMember.SetValue(this, instance);
                                }
                                else if (key.IsGenericType && key.GetGenericTypeDefinition() == typeof(HashSet<>))
                                {
                                    Array array = this.DeserializeArray(typeof(object[]), key.GetGenericArguments()[0], this._data);
                                    IList instance1 = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(key.GetGenericArguments()[0]));
                                    foreach (object obj in array)
                                        instance1.Add(obj);
                                    object instance2 = Activator.CreateInstance(key, instance1);
                                    if (classMember == null)
                                        this._extraProperties.Add(str, instance2);
                                    else
                                        classMember.SetValue(this, instance2);
                                }
                                else
                                {
                                    object element = !key.IsEnum ? this._data.Read(key, false) : this._data.ReadInt();
                                    if (classMember == null)
                                        this._extraProperties.Add(str, element);
                                    else
                                        classMember.SetValue(this, element);
                                }
                            }
                            else
                                this._data.position += (int)num3;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this._exception = ex;
                this._result = DeserializeResult.ExceptionThrown;
                return false;
            }
            this._result = DeserializeResult.Success;
            return true;
        }

        public static BinaryClassChunk DeserializeHeader(
          System.Type t,
          BitBuffer data,
          BinaryClassChunk target = null,
          bool root = true,
          bool skipData = false)
        {
            if (target == null)
                target = Activator.CreateInstance(t, null) as BinaryClassChunk;
            try
            {
                long num1 = 0;
                if (root)
                {
                    num1 = data.ReadLong();
                    if (num1 != BinaryClassChunk.MagicNumber(t))
                    {
                        target._result = DeserializeResult.InvalidMagicNumber;
                        return target;
                    }
                    target._checksum = data.ReadUInt();
                }
                ushort num2 = data.ReadUShort();
                ushort num3 = BinaryClassChunk.ChunkVersion(t);
                bool flag = true;
                if (num2 != num3)
                {
                    if (num2 > num3)
                    {
                        target._result = DeserializeResult.FileVersionTooNew;
                        flag = false;
                    }
                    else if (num3 != 2)
                    {
                        target._result = DeserializeResult.FileVersionTooOld;
                        flag = false;
                    }
                    if (!flag)
                        return target;
                }
                target._magicNumber = num1;
                target._version = num2;
                if (num2 > 1 && target is LevelData && data.ReadBool())
                {
                    System.Type type = Editor.GetType(data.ReadString());
                    target.SetExtraHeaderInfo(BinaryClassChunk.DeserializeHeader(type, data, root: false));
                    if (target.GetExtraHeaderInfo() != null && target.GetExtraHeaderInfo()._result == DeserializeResult.HeaderDeserialized)
                        target.GetExtraHeaderInfo().Deserialize();
                }
                target._size = data.ReadUInt();
                target._offset = (uint)data.position;
                target._data = data;
                target._result = DeserializeResult.HeaderDeserialized;
                if (skipData)
                    data.position = (int)target._offset + (int)target._size;
                return target;
            }
            catch (Exception ex)
            {
                target._exception = ex;
                target._result = DeserializeResult.ExceptionThrown;
                return target;
            }
        }

        public T GetChunk<T>(string name) => this.GetChunk<T>(name, false);

        public T GetChunk<T>(string name, bool pPartialDeserialize) => this.GetChunk<T>(name, pPartialDeserialize, false);

        public T GetChunk<T>(string name, bool pPartialDeserialize, bool pForceCreation)
        {
            if (this._result == DeserializeResult.HeaderDeserialized)
                this.Deserialize();
            BinaryClassChunk chunk;
            if (!this._headerDictionary.TryGetValue(name, out chunk))
            {
                chunk = Activator.CreateInstance(typeof(T), null) as BinaryClassChunk;
                chunk._result = DeserializeResult.Success;
                this._headerDictionary[name] = chunk;
            }
            if (chunk == null)
                return default(T);
            if (chunk._result == DeserializeResult.HeaderDeserialized)
                chunk.Deserialize();
            return (T)(object)chunk;
        }

        private void SerializeArray(Array array, System.Type arrayType, BitBuffer data)
        {
            data.Write(array.Length);
            for (int index = 0; index < array.Length; ++index)
            {
                object obj = array.GetValue(index);
                data.Write(obj != null);
                if (obj != null)
                {
                    if (typeof(BinaryClassChunk).IsAssignableFrom(arrayType))
                        (obj as BinaryClassChunk).Serialize(data, false);
                    else
                        data.Write(obj);
                }
            }
        }

        public virtual BitBuffer Serialize(BitBuffer data = null, bool root = true)
        {
            if (data == null)
                data = new BitBuffer(false);
            this._serializedData = data;
            if (data.allowPacking)
                throw new Exception("This class does not support serialization with a packed bit buffer. Construct the buffer with allowPacking set to false.");
            System.Type type1 = this.GetType();
            List<ClassMember> members = Editor.GetMembers(type1);
            List<BinaryClassMember> binaryClassMemberList = new List<BinaryClassMember>();
            foreach (ClassMember classMember in members)
            {
                if (!classMember.isPrivate && !(classMember.name == "metaData") && (classMember.type.IsEnum || classMember.type.IsPrimitive || classMember.type.Equals(typeof(string)) || typeof(BinaryClassChunk).IsAssignableFrom(classMember.type) || classMember.type.IsArray || classMember.type.IsGenericType && (classMember.type.GetGenericTypeDefinition() == typeof(List<>) || classMember.type.GetGenericTypeDefinition() == typeof(HashSet<>))))
                {
                    object obj = classMember.GetValue(this);
                    if (classMember.type.IsEnum)
                        obj = (int)obj;
                    if (obj != null)
                    {
                        BinaryClassMember binaryClassMember = new BinaryClassMember()
                        {
                            name = classMember.name,
                            data = obj
                        };
                        binaryClassMemberList.Add(binaryClassMember);
                    }
                }
            }
            if (this._extraProperties != null)
            {
                foreach (KeyValuePair<string, List<object>> extraProperty in (MultiMap<string, object, List<object>>)this._extraProperties)
                {
                    if (extraProperty.Value != null)
                    {
                        foreach (object obj1 in extraProperty.Value)
                        {
                            object obj2 = obj1;
                            if (obj2 != null && obj2.GetType().IsEnum)
                                obj2 = (int)obj2;
                            BinaryClassMember binaryClassMember = new BinaryClassMember()
                            {
                                name = "@" + extraProperty.Key,
                                data = obj2,
                                extra = true
                            };
                            binaryClassMemberList.Add(binaryClassMember);
                        }
                    }
                }
            }
            if (root)
            {
                long val = BinaryClassChunk.MagicNumber(type1);
                data.Write(val);
                data.Write(0U);
            }
            data.Write(BinaryClassChunk.ChunkVersion(type1));
            if (BinaryClassChunk.ChunkVersion(type1) == 2)
            {
                if (this.GetExtraHeaderInfo() != null)
                {
                    data.Write(true);
                    data.Write(ModLoader.SmallTypeName(this.GetExtraHeaderInfo().GetType()));
                    this.GetExtraHeaderInfo().Serialize(data, false);
                }
                else
                    data.Write(false);
            }
            int position1 = data.position;
            data.Write(0U);
            data.Write((ushort)binaryClassMemberList.Count);
            foreach (BinaryClassMember binaryClassMember in binaryClassMemberList)
            {
                data.Write(binaryClassMember.name);
                byte val = 0;
                if (binaryClassMember.extra)
                {
                    if (binaryClassMember.data == null)
                    {
                        val = byte.MaxValue;
                        data.Write(val);
                    }
                    else
                    {
                        System.Type type2 = binaryClassMember.data.GetType();
                        if (BinaryClassMember.typeMap.TryGetValue(type2, out val))
                            val = (byte)(val << 1 | 1);
                        data.Write(val);
                        if (val == 0)
                            data.Write(ModLoader.SmallTypeName(type2));
                    }
                }
                if (val != byte.MaxValue)
                {
                    int position2 = data.position;
                    data.Write(0U);
                    if (binaryClassMember.data is BinaryClassChunk)
                    {
                        if (!(binaryClassMember.data as BinaryClassChunk).ignore)
                            (binaryClassMember.data as BinaryClassChunk).Serialize(data, false);
                        else
                            continue;
                    }
                    else if (binaryClassMember.data is Array)
                        this.SerializeArray(binaryClassMember.data as Array, binaryClassMember.data.GetType().GetElementType(), data);
                    else if (binaryClassMember.data.GetType().IsGenericType && binaryClassMember.data.GetType().GetGenericTypeDefinition() == typeof(List<>))
                    {
                        IList data1 = binaryClassMember.data as IList;
                        Array array = new object[data1.Count];
                        data1.CopyTo(array, 0);
                        this.SerializeArray(array, binaryClassMember.data.GetType().GetGenericArguments()[0], data);
                    }
                    else if (binaryClassMember.data.GetType().IsGenericType && binaryClassMember.data.GetType().GetGenericTypeDefinition() == typeof(HashSet<>))
                    {
                        IEnumerable data2 = binaryClassMember.data as IEnumerable;
                        List<object> objectList = new List<object>();
                        foreach (object obj in data2)
                            objectList.Add(obj);
                        object[] array = new object[objectList.Count];
                        objectList.CopyTo(array, 0);
                        this.SerializeArray(array, binaryClassMember.data.GetType().GetGenericArguments()[0], data);
                    }
                    else
                        data.Write(binaryClassMember.data);
                    int position3 = data.position;
                    data.position = position2;
                    data.Write((uint)(position3 - position2 - 4));
                    data.position = position3;
                }
            }
            int position4 = data.position;
            data.position = position1;
            data.Write((uint)(position4 - position1 - 4));
            if (root)
            {
                this._checksum = Editor.Checksum(data.buffer);
                data.position = 8;
                data.Write(this._checksum);
            }
            data.position = position4;
            return data;
        }

        public static long MagicNumber<T>() => BinaryClassChunk.MagicNumber(typeof(T));

        public static long MagicNumber(System.Type t)
        {
            object[] customAttributes = t.GetCustomAttributes(typeof(MagicNumberAttribute), true);
            return customAttributes.Length != 0 ? (customAttributes[0] as MagicNumberAttribute).magicNumber : 0L;
        }

        public static ushort ChunkVersion<T>() => BinaryClassChunk.ChunkVersion(typeof(T));

        public static ushort ChunkVersion(System.Type t)
        {
            object[] customAttributes = t.GetCustomAttributes(typeof(ChunkVersionAttribute), true);
            return customAttributes.Length != 0 ? (customAttributes[0] as ChunkVersionAttribute).version : (ushort)0;
        }
    }
}
