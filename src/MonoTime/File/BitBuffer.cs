// Decompiled with JetBrains decompiler
// Type: DuckGame.BitBuffer
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DuckGame
{
    public class BitBuffer
    {
        private static int[] _maxMasks;
        private byte[] _buffer = new byte[64];
        private bool _dirty;
        private int _offsetPosition;
        private int _endPosition;
        private int _bitEndOffset;
        private int _bitOffsetPosition;
        private byte[] _trimmedBuffer;
        private static int[] _readMasks;
        private bool _allowPacking = true;
        //private int offset;
        //private int currentBit;
        public static List<System.Type> kTypeIndexList = new List<System.Type>()
    {
      typeof (string),
      typeof (byte[]),
      typeof (BitBuffer),
      typeof (float),
      typeof (double),
      typeof (byte),
      typeof (sbyte),
      typeof (bool),
      typeof (short),
      typeof (ushort),
      typeof (int),
      typeof (uint),
      typeof (long),
      typeof (ulong),
      typeof (char),
      typeof (Vec2),
      typeof (Color),
      typeof (NetIndex16),
      typeof (NetIndex2),
      typeof (NetIndex4),
      typeof (NetIndex8),
      typeof (Thing)
    };

        public string ReadTokenizedString()
        {
            int index = this.ReadInt();
            if (index >= TokenDeserializer.instance._tokens.Count)
                throw new Exception("BitBuffer.ReadTokenizedString() encountered an invalid token.");
            return TokenDeserializer.instance._tokens[index];
        }

        private void WriteTokenizedString(string val) => this.Write(TokenSerializer.instance.Token(val));

        public override string ToString()
        {
            string str = "";
            for (int index = 0; index < this.lengthInBytes; ++index)
                str = str + this._buffer[index].ToString() + "|";
            return str;
        }

        public static BitBuffer FromString(string s)
        {
            BitBuffer bitBuffer = new BitBuffer();
            try
            {
                string str1 = s;
                char[] chArray = new char[1] { '|' };
                foreach (string str2 in str1.Split(chArray))
                {
                    if (!(str2 == ""))
                        bitBuffer.Write(Convert.ToByte(str2));
                }
                bitBuffer.position = 0;
            }
            catch (Exception)
            {
                DevConsole.Log(DCSection.General, "BitBuffer conversion from string failed.");
                return new BitBuffer();
            }
            return bitBuffer;
        }

        public static long GetMaxValue(int bits)
        {
            if (BitBuffer._maxMasks == null)
            {
                BitBuffer._maxMasks = new int[64];
                int num1 = 0;
                for (int index = 0; index < 64; ++index)
                {
                    int num2 = num1 | 1;
                    BitBuffer._maxMasks[index] = num2;
                    num1 = num2 << 1;
                }
            }
            return BitBuffer._maxMasks[bits];
        }

        /// <summary>
        /// This BitBuffers internal buffer. This may have zeroes at the end, as the buffer size is doubled whenever it's filled.
        /// </summary>
        public byte[] buffer => this._buffer;

        /// <summary>A byte[] representation of all data in the buffer.</summary>
        public byte[] data
        {
            get
            {
                try
                {
                    byte[] destinationArray = new byte[this.lengthInBytes];
                    Array.Copy(_buffer, destinationArray, this.lengthInBytes);
                    return destinationArray;
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        public int position
        {
            get => this._offsetPosition;
            set
            {
                if (this._offsetPosition != value)
                    this._dirty = true;
                this._offsetPosition = value;
                if (this._offsetPosition <= this._endPosition)
                    return;
                this._endPosition = this._offsetPosition;
                this._bitEndOffset = 0;
            }
        }

        public uint positionInBits
        {
            get => (uint)(this.position * 8 + this.bitOffset);
            set
            {
                this.position = (int)(value / 8U);
                this.bitOffset = (int)(value % 8U);
            }
        }

        public int bitOffset
        {
            get => this._bitOffsetPosition;
            set
            {
                if (this._bitOffsetPosition != value)
                    this._dirty = true;
                this._bitOffsetPosition = value;
                if (this._endPosition != this._offsetPosition || this._bitOffsetPosition <= this._bitEndOffset)
                    return;
                this._bitEndOffset = value;
            }
        }

        public bool isPacked => this._bitEndOffset != 0;

        public int lengthInBits => this._endPosition * 8 + this._bitEndOffset;

        public int lengthInBytes
        {
            get => this._endPosition + (this._bitEndOffset > 0 ? 1 : 0);
            set => this._endPosition = value;
        }

        public byte[] GetBytes()
        {
            if (this._trimmedBuffer != null && !this._dirty)
                return this._trimmedBuffer;
            this._dirty = false;
            this._trimmedBuffer = new byte[this.lengthInBytes];
            for (int index = 0; index < this.lengthInBytes; ++index)
                this._trimmedBuffer[index] = this._buffer[index];
            return this._trimmedBuffer;
        }

        private void calculateReadMasks()
        {
            if (BitBuffer._readMasks != null)
                return;
            BitBuffer._readMasks = new int[64];
            int num1 = 0;
            for (int index = 0; index < 64; ++index)
            {
                int num2 = num1 | 1;
                BitBuffer._readMasks[index] = num2;
                num1 = num2 << 1;
            }
        }

        public bool allowPacking => this._allowPacking;

        public BitBuffer(bool allowPacking = true)
        {
            this.calculateReadMasks();
            this._allowPacking = allowPacking;
        }

        public BitBuffer(byte[] data, int bits = 0, bool allowPacking = true)
        {
            this._allowPacking = allowPacking;
            this.calculateReadMasks();
            this.Write(data, 0, -1);
            this.SeekToStart();
            if (bits <= 0 || this._endPosition * 8 <= bits)
                return;
            --this._endPosition;
            this._bitEndOffset = bits - this._endPosition * 8;
        }

        public BitBuffer(byte[] data, bool copyData)
        {
            this._allowPacking = false;
            this.calculateReadMasks();
            if (copyData)
            {
                this.Write(data, 0, -1);
                this.SeekToStart();
            }
            else
                this._buffer = data;
        }

        public void SeekToStart()
        {
            this.position = 0;
            this._bitOffsetPosition = 0;
        }

        public void Fill(byte[] bytes, int offset = 0, int vbitOffset = 0)
        {
            this._buffer = bytes;
            this.position = offset;
            this._bitOffsetPosition = vbitOffset;
        }

        public BitBuffer Instance() => new BitBuffer()
        {
            _buffer = this.buffer,
            _offsetPosition = this._offsetPosition,
            _endPosition = this._endPosition,
            _bitEndOffset = this._bitEndOffset,
            _bitOffsetPosition = this._bitOffsetPosition
        };

        public int ReadPackedBits(int bits)
        {
            if (bits == 0)
                return 0;
            int num1 = 0;
            if (bits <= 8 - this.bitOffset)
            {
                num1 = this._buffer[this.position] >> this.bitOffset & BitBuffer._readMasks[bits - 1];
                this.bitOffset += bits;
            }
            else
            {
                int num2 = 0;
                while (true)
                {
                    if (this.bitOffset > 7)
                    {
                        this.bitOffset = 0;
                        ++this.position;
                    }
                    if (bits > 0)
                    {
                        int num3 = 8 - this.bitOffset;
                        if (num3 > bits)
                            num3 = bits;
                        int num4 = this._buffer[this.position] >> this.bitOffset & BitBuffer._readMasks[num3 - 1];
                        bits -= num3;
                        int num5 = num4 << num2;
                        num1 |= num5;
                        this.bitOffset += num3;
                        num2 += num3;
                    }
                    else
                        break;
                }
            }
            if (this.bitOffset > 7)
            {
                this.bitOffset = 0;
                ++this.position;
            }
            return num1;
        }

        public byte[] ReadPacked(int bytes)
        {
            byte[] numArray = new byte[bytes];
            for (int index = 0; index < bytes; ++index)
                numArray[index] = (byte)this.ReadPackedBits(8);
            return numArray;
        }

        public void WritePacked(int number, int bits)
        {
            try
            {
                if (this.lengthInBits + bits > this._buffer.Length * 8)
                    this.resize(this._buffer.Length * 2);
                //this.currentBit = 0;
                while (bits > 0)
                {
                    this._buffer[this.position] |= (byte)((number & 1) << this.bitOffset);
                    number >>= 1;
                    ++this.bitOffset;
                    --bits;
                    if (this.bitOffset == 8)
                    {
                        ++this.position;
                        this.bitOffset = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                string[] strArray = new string[10];
                strArray[0] = Main.SpecialCode;
                strArray[1] = bits.ToString();
                strArray[2] = ", ";
                int num = this.lengthInBits;
                strArray[3] = num.ToString();
                strArray[4] = ", ";
                num = this._buffer.Length;
                strArray[5] = num.ToString();
                strArray[6] = ", ";
                num = this.position;
                strArray[7] = num.ToString();
                strArray[8] = ", ";
                strArray[9] = number.ToString();
                Main.SpecialCode = string.Concat(strArray);
                throw ex;
            }
        }

        public void WritePacked(byte[] data)
        {
            foreach (int number in data)
                this.WritePacked(number, 8);
        }

        public void WritePacked(byte[] data, int bits)
        {
            if (this.position + (int)Math.Ceiling(bits / 8.0) > this._buffer.Length)
                this.resize((this.position + (int)Math.Ceiling(bits / 8.0)) * 2);
            int index = 0;
            if (!this.isPacked)
            {
                for (; bits >= 8; bits -= 8)
                {
                    this._buffer[this.position] = data[index];
                    ++this.position;
                    ++index;
                }
            }
            else
            {
                for (; bits >= 8; bits -= 8)
                {
                    this.WritePacked(data[index], 8);
                    ++index;
                }
            }
            if (bits <= 0)
                return;
            this.WritePacked(data[index], bits);
        }

        public BitBuffer ReadBitBuffer(bool allowPacking = true)
        {
            int length1 = this.ReadUShort();
            if (length1 == ushort.MaxValue)
                length1 = this.ReadInt();
            byte[] numArray;
            if (allowPacking)
            {
                int length2 = (int)Math.Ceiling(length1 / 8.0);
                numArray = new byte[length2];
                int num = length1;
                for (int index = 0; index < length2; ++index)
                {
                    numArray[index] = (byte)this.ReadPackedBits(num >= 8 ? 8 : num);
                    if (num >= 8)
                        num -= 8;
                }
            }
            else
            {
                numArray = new byte[length1];
                Array.Copy(buffer, this.position, numArray, 0, length1);
                this.position += length1;
                length1 = 0;
            }
            return new BitBuffer(numArray, length1, allowPacking);
        }

        public string ReadString()
        {
            if (TokenDeserializer.instance != null)
                return this.ReadTokenizedString();
            int num = this.ReadUShort();
            if (num == ushort.MaxValue)
            {
                int bitOffset = this.bitOffset;
                int position = this.position;
                if (this.ReadUShort() == 42252)
                {
                    num = this.ReadInt();
                }
                else
                {
                    this.position = position;
                    this.bitOffset = bitOffset;
                }
            }
            if (this.bitOffset != 0)
                return Encoding.UTF8.GetString(this.ReadPacked(num));
            string str = Encoding.UTF8.GetString(this._buffer, this.position, num);
            this.position += num;
            return str;
        }

        public long ReadLong()
        {
            if (this.bitOffset != 0)
                return BitConverter.ToInt64(this.ReadPacked(8), 0);
            long int64 = BitConverter.ToInt64(this._buffer, this.position);
            this.position += 8;
            return int64;
        }

        public ulong ReadULong()
        {
            if (this.bitOffset != 0)
                return BitConverter.ToUInt64(this.ReadPacked(8), 0);
            long uint64 = (long)BitConverter.ToUInt64(this._buffer, this.position);
            this.position += 8;
            return (ulong)uint64;
        }

        public int ReadInt()
        {
            if (this.bitOffset != 0)
                return BitConverter.ToInt32(this.ReadPacked(4), 0);
            int int32 = BitConverter.ToInt32(this._buffer, this.position);
            this.position += 4;
            return int32;
        }

        public uint ReadUInt()
        {
            if (this.bitOffset != 0)
                return BitConverter.ToUInt32(this.ReadPacked(4), 0);
            int uint32 = (int)BitConverter.ToUInt32(this._buffer, this.position);
            this.position += 4;
            return (uint)uint32;
        }

        public short ReadShort()
        {
            if (this.bitOffset != 0)
                return BitConverter.ToInt16(this.ReadPacked(2), 0);
            int int16 = BitConverter.ToInt16(this._buffer, this.position);
            this.position += 2;
            return (short)int16;
        }

        public ushort ReadUShort()
        {
            if (this.bitOffset != 0)
                return BitConverter.ToUInt16(this.ReadPacked(2), 0);
            int uint16 = BitConverter.ToUInt16(this._buffer, this.position);
            this.position += 2;
            return (ushort)uint16;
        }

        public float ReadFloat()
        {
            if (this.bitOffset != 0)
                return BitConverter.ToSingle(this.ReadPacked(4), 0);
            double single = (double)BitConverter.ToSingle(this._buffer, this.position);
            this.position += 4;
            return (float)single;
        }

        public Vec2 ReadVec2() => new Vec2()
        {
            x = this.ReadFloat(),
            y = this.ReadFloat()
        };

        public Color ReadColor() => new Color()
        {
            r = this.ReadByte(),
            g = this.ReadByte(),
            b = this.ReadByte(),
            a = this.ReadByte()
        };

        public Color ReadRGBColor() => new Color()
        {
            r = this.ReadByte(),
            g = this.ReadByte(),
            b = this.ReadByte(),
            a = byte.MaxValue
        };

        public double ReadDouble()
        {
            if (this.bitOffset != 0)
                return BitConverter.ToDouble(this.ReadPacked(8), 0);
            double num = BitConverter.ToDouble(this._buffer, this.position);
            this.position += 8;
            return num;
        }

        public char ReadChar()
        {
            if (this.bitOffset != 0)
                return BitConverter.ToChar(this.ReadPacked(2), 0);
            int num = BitConverter.ToChar(this._buffer, this.position);
            this.position += 2;
            return (char)num;
        }

        public byte ReadByte()
        {
            if (this.bitOffset != 0)
                return this.ReadPacked(1)[0];
            int num = this._buffer[this.position];
            ++this.position;
            return (byte)num;
        }

        public byte[] ReadBytes()
        {
            int length = this.ReadInt();
            byte[] destinationArray = new byte[length];
            Array.Copy(buffer, this.position, destinationArray, 0, length);
            this.position += length;
            return destinationArray;
        }

        public sbyte ReadSByte()
        {
            if (this.bitOffset != 0)
                return (sbyte)this.ReadPacked(1)[0];
            int num = (sbyte)this._buffer[this.position];
            ++this.position;
            return (sbyte)num;
        }

        public bool ReadBool()
        {
            if (this._allowPacking)
                return this.ReadPackedBits(1) > 0;
            return this.ReadByte() > 0;
        }

        public NetIndex4 ReadNetIndex4() => new NetIndex4(this.ReadPackedBits(4));

        public NetIndex8 ReadNetIndex8() => new NetIndex8(this.ReadPackedBits(8));

        public NetIndex16 ReadNetIndex16() => new NetIndex16(this.ReadPackedBits(16));

        public byte[] ReadData(int length)
        {
            byte[] dst = new byte[length];
            Buffer.BlockCopy(buffer, this.position, dst, 0, length);
            this.position += length;
            return dst;
        }

        public object Read(System.Type type, bool allowPacking = true)
        {
            if (type == typeof(string))
                return this.ReadString();
            if (type == typeof(float))
                return this.ReadFloat();
            if (type == typeof(double))
                return this.ReadDouble();
            if (type == typeof(byte))
                return this.ReadByte();
            if (type == typeof(sbyte))
                return this.ReadSByte();
            if (type == typeof(bool))
                return this.ReadBool();
            if (type == typeof(short))
                return this.ReadShort();
            if (type == typeof(ushort))
                return this.ReadUShort();
            if (type == typeof(int))
                return this.ReadInt();
            if (type == typeof(uint))
                return this.ReadUInt();
            if (type == typeof(long))
                return this.ReadLong();
            if (type == typeof(ulong))
                return this.ReadULong();
            if (type == typeof(char))
                return this.ReadChar();
            if (type == typeof(Vec2))
                return this.ReadVec2();
            if (type == typeof(BitBuffer))
                return this.ReadBitBuffer(allowPacking);
            if (type == typeof(NetIndex16))
                return new NetIndex16(this.ReadUShort());
            if (type == typeof(NetIndex2))
                return new NetIndex2((int)this.ReadBits(typeof(int), 2));
            if (type == typeof(NetIndex4))
                return new NetIndex4((int)this.ReadBits(typeof(int), 4));
            if (type == typeof(NetIndex8))
                return new NetIndex8((int)this.ReadBits(typeof(int), 8));
            return typeof(Thing).IsAssignableFrom(type) ? (object)this.ReadThing(type) : throw new Exception("Trying to read unsupported type " + type?.ToString() + " from BitBuffer!");
        }

        public Thing ReadThing(System.Type pThingType)
        {
            byte num = this.ReadByte();
            ushort key = (ushort)this.ReadBits(typeof(ushort), 10);
            ushort index = this.ReadUShort();
            if (num != DuckNetwork.levelIndex || index == 0)
                return null;
            if (key == 0)
                return GhostManager.context.GetSpecialSync(index);
            NetIndex16 netIndex16 = (NetIndex16)index;
            Profile profile = GhostObject.IndexToProfile(netIndex16);
            if (profile != null && profile.removedGhosts.ContainsKey(netIndex16))
                return profile.removedGhosts[netIndex16].thing;
            System.Type type = Editor.IDToType[key];
            if (!pThingType.IsAssignableFrom(type))
            {
                DevConsole.Log(DCSection.GhostMan, "@error Type mismatch, ignoring ghost (" + netIndex16.ToString() + "(" + type.GetType().Name + " vs. " + pThingType.Name + "))@error");
                return null;
            }
            GhostObject ghost = GhostManager.context.GetGhost(netIndex16);
            if (ghost != null && ghost.thing.GetType() != type)
            {
                DevConsole.Log(DCSection.GhostMan, "@error Type mismatch, removing ghost (" + netIndex16.ToString() + " " + ghost.thing.GetType().ToString() + "(my type) vs. " + type.ToString() + "(your type))@error");
                GhostManager.changingGhostType = true;
                GhostManager.context.RemoveGhost(ghost, (NetIndex16)0);
                GhostManager.changingGhostType = false;
                ghost = null;
            }
            if (ghost == null)
            {
                Thing thing = Editor.CreateThing(type);
                thing.connection = NetworkConnection.context;
                thing.authority = (NetIndex8)1;
                if (profile != null && netIndex16 > profile.latestGhostIndex)
                    profile.latestGhostIndex = netIndex16;
                if (num != Level.core.currentLevel.networkIndex)
                {
                    ghost = new GhostObject(thing, GhostManager.context, (int)netIndex16);
                    thing.position = new Vec2(-2000f, -2000f);
                    GhostManager.context.pendingBitBufferGhosts.Add(ghost);
                }
                else
                {
                    ghost = GhostManager.context.MakeGhost(thing, (int)netIndex16);
                    thing.position = new Vec2(-2000f, -2000f);
                    ghost.ClearStateMask(NetworkConnection.context);
                    thing.level = Level.current;
                    thing.isBitBufferCreatedGhostThing = true;
                }
                thing.connection = NetworkConnection.context;
            }
            return ghost.thing;
        }

        public object ReadBits(System.Type t, int bits) => bits == -1 ? this.Read(t) : this.ConvertType(this.ReadPackedBits(bits), t);

        public T ReadBits<T>(int bits) => bits < 1 ? default(T) : (T)this.ConvertType(this.ReadPackedBits(bits), typeof(T));

        protected object ConvertType(int obj, System.Type type)
        {
            if (type == typeof(float))
                return (float)obj;
            if (type == typeof(double))
                return (double)obj;
            if (type == typeof(byte))
                return (byte)obj;
            if (type == typeof(sbyte))
                return (sbyte)obj;
            if (type == typeof(short))
                return (short)obj;
            if (type == typeof(ushort))
                return (ushort)obj;
            if (type == typeof(int))
                return obj;
            if (type == typeof(uint))
                return (uint)obj;
            if (type == typeof(long))
                return (long)obj;
            if (type == typeof(ulong))
                return (ulong)obj;
            if (type == typeof(char))
                return (char)obj;
            throw new Exception("unrecognized conversion type " + type?.ToString());
        }

        public T Read<T>() => (T)this.Read(typeof(T));

        public void AlignToByte()
        {
            if (this.bitOffset <= 0)
                return;
            ++this.position;
            this.bitOffset = 0;
        }

        public void WriteBufferData(BitBuffer val)
        {
            if (!val.isPacked && !this.isPacked)
            {
                if (this.position + val.lengthInBytes > this._buffer.Length)
                    this.resize(this.position + val.lengthInBytes);
                for (int index = 0; index < val.lengthInBytes; ++index)
                {
                    this._buffer[this.position] = val.buffer[index];
                    ++this.position;
                }
            }
            else
                this.WritePacked(val.buffer, val.lengthInBits);
        }

        public void Write(BitBuffer val, bool writeLength = true)
        {
            if (writeLength)
            {
                int val1 = val.allowPacking ? val.lengthInBits : val.lengthInBytes;
                if (val1 > 65534)
                {
                    this.Write(ushort.MaxValue);
                    this.Write(val1);
                }
                else
                    this.Write((ushort)val1);
            }
            this.WriteBufferData(val);
        }

        public void Write(byte[] val, bool writeLength)
        {
            if (writeLength)
                this.Write(val.Length);
            this.Write(val, length: val.Length);
        }

        public void Write(byte[] data, int offset = 0, int length = -1)
        {
            if (!this.isPacked || this.bitOffset == 0)
            {
                if (length < 0)
                    length = data.Length;
                if (this.position + length > this._buffer.Length)
                    this.resize(this.position + length);
                Array.Copy(data, offset, buffer, this.position, length);
                this.position += length;
            }
            else
                this.WritePacked(data);
        }

        public void Write(string val)
        {
            if (TokenSerializer.instance != null)
            {
                this.WriteTokenizedString(val);
            }
            else
            {
                byte[] bytes = Encoding.UTF8.GetBytes(val);
                if (this.bitOffset != 0)
                {
                    this.Write((ushort)bytes.Count<byte>());
                    this.WritePacked(bytes);
                }
                else
                {
                    int val1 = bytes.Count<byte>();
                    if (val1 > ushort.MaxValue)
                    {
                        this.Write(ushort.MaxValue);
                        this.Write((ushort)42252);
                        this.Write(val1);
                    }
                    else
                        this.Write((ushort)bytes.Count<byte>());
                    int num = bytes.Count<byte>();
                    if (this.position + num > _buffer.Count<byte>())
                        this.resize(this.position + num);
                    bytes.CopyTo(_buffer, this.position);
                    this.position += num;
                }
            }
        }

        public void Write(long val)
        {
            byte[] bytes = BitConverter.GetBytes(val);
            if (this.bitOffset != 0)
            {
                this.WritePacked(bytes);
            }
            else
            {
                byte num = (byte)bytes.Count<byte>();
                if (this.position + num > _buffer.Count<byte>())
                    this.resize(this.position + num);
                bytes.CopyTo(_buffer, this.position);
                this.position += bytes.Count<byte>();
            }
        }

        public void Write(ulong val)
        {
            byte[] bytes = BitConverter.GetBytes(val);
            if (this.bitOffset != 0)
            {
                this.WritePacked(bytes);
            }
            else
            {
                byte num = (byte)bytes.Count<byte>();
                if (this.position + num > _buffer.Count<byte>())
                    this.resize(this.position + num);
                bytes.CopyTo(_buffer, this.position);
                this.position += bytes.Count<byte>();
            }
        }

        public void Write(int val)
        {
            byte[] bytes = BitConverter.GetBytes(val);
            if (this.bitOffset != 0)
            {
                this.WritePacked(bytes);
            }
            else
            {
                byte num = (byte)bytes.Count<byte>();
                if (this.position + num > _buffer.Count<byte>())
                    this.resize(this.position + num);
                bytes.CopyTo(_buffer, this.position);
                this.position += bytes.Count<byte>();
            }
        }

        public void Write(uint val)
        {
            byte[] bytes = BitConverter.GetBytes(val);
            if (this.bitOffset != 0)
            {
                this.WritePacked(bytes);
            }
            else
            {
                byte num = (byte)bytes.Count<byte>();
                if (this.position + num > _buffer.Count<byte>())
                    this.resize(this.position + num);
                bytes.CopyTo(_buffer, this.position);
                this.position += bytes.Count<byte>();
            }
        }

        public void Write(short val)
        {
            byte[] bytes = BitConverter.GetBytes(val);
            if (this.bitOffset != 0)
            {
                this.WritePacked(bytes);
            }
            else
            {
                byte num = (byte)bytes.Count<byte>();
                if (this.position + num > _buffer.Count<byte>())
                    this.resize(this.position + num);
                bytes.CopyTo(_buffer, this.position);
                this.position += bytes.Count<byte>();
            }
        }

        public void Write(ushort val)
        {
            byte[] bytes = BitConverter.GetBytes(val);
            if (this.bitOffset != 0)
            {
                this.WritePacked(bytes);
            }
            else
            {
                byte num = (byte)bytes.Count<byte>();
                if (this.position + num > _buffer.Count<byte>())
                    this.resize(this.position + num);
                bytes.CopyTo(_buffer, this.position);
                this.position += bytes.Count<byte>();
            }
        }

        public void Write(float val)
        {
            byte[] bytes = BitConverter.GetBytes(val);
            if (this.bitOffset != 0)
            {
                this.WritePacked(bytes);
            }
            else
            {
                byte num = (byte)bytes.Count<byte>();
                if (this.position + num > _buffer.Count<byte>())
                    this.resize(this.position + num);
                bytes.CopyTo(_buffer, this.position);
                this.position += bytes.Count<byte>();
            }
        }

        public void Write(Vec2 val)
        {
            this.Write(val.x);
            this.Write(val.y);
        }

        public void Write(Color val)
        {
            this.Write(val.r);
            this.Write(val.g);
            this.Write(val.b);
            this.Write(val.a);
        }

        public void WriteRGBColor(Color val)
        {
            this.Write(val.r);
            this.Write(val.g);
            this.Write(val.b);
        }

        public void Write(double val)
        {
            byte[] bytes = BitConverter.GetBytes(val);
            if (this.bitOffset != 0)
            {
                this.WritePacked(bytes);
            }
            else
            {
                byte num = (byte)bytes.Count<byte>();
                if (this.position + num > _buffer.Count<byte>())
                    this.resize(this.position + num);
                bytes.CopyTo(_buffer, this.position);
                this.position += bytes.Count<byte>();
            }
        }

        public void Write(char val)
        {
            byte[] bytes = BitConverter.GetBytes(val);
            if (this.bitOffset != 0)
            {
                this.WritePacked(bytes);
            }
            else
            {
                byte num = (byte)bytes.Count<byte>();
                if (this.position + num > _buffer.Count<byte>())
                    this.resize(this.position + num);
                bytes.CopyTo(_buffer, this.position);
                this.position += bytes.Count<byte>();
            }
        }

        public void Write(byte val)
        {
            if (this.bitOffset != 0)
            {
                this.WritePacked(val, 8);
            }
            else
            {
                if (this.position + 1 > _buffer.Count<byte>())
                    this.resize(this.position + 1);
                this._buffer[this.position] = val;
                ++this.position;
            }
        }

        public void Write(sbyte val)
        {
            if (this.bitOffset != 0)
            {
                this.WritePacked(val, 8);
            }
            else
            {
                if (this.position + 1 > _buffer.Count<byte>())
                    this.resize(this.position + 1);
                this._buffer[this.position] = (byte)val;
                ++this.position;
            }
        }

        public void Write(bool val)
        {
            if (this._allowPacking)
                this.WritePacked(val ? 1 : 0, 1);
            else
                this.Write(val ? (byte)1 : (byte)0);
        }

        public void WriteProfile(Profile pValue)
        {
            if (pValue == null)
                this.Write((sbyte)-1);
            else
                this.Write((sbyte)pValue.networkIndex);
        }

        public Profile ReadProfile()
        {
            sbyte index = this.ReadSByte();
            Profile profile = null;
            if (index >= 0 && index < DuckNetwork.profiles.Count)
                profile = DuckNetwork.profiles[index];
            return profile;
        }

        public void WriteTeam(Team pValue)
        {
            int val = -1;
            if (pValue != null)
                val = Teams.IndexOf(pValue);
            this.Write((ushort)val);
        }

        public Team ReadTeam() => Teams.ParseFromIndex(this.ReadUShort());

        public void WriteObject(object obj)
        {
            int val = byte.MaxValue;
            if (obj != null)
                val = !(obj is Thing) ? BitBuffer.kTypeIndexList.IndexOf(obj.GetType()) : BitBuffer.kTypeIndexList.IndexOf(typeof(Thing));
            if (val < 0)
                throw new Exception("Trying to write unsupported type to BitBuffer through WriteObject!");
            this.Write((byte)val);
            this.Write(obj);
        }

        public object ReadObject(out System.Type pTypeRead)
        {
            byte index = this.ReadByte();
            if (index == byte.MaxValue || index >= BitBuffer.kTypeIndexList.Count)
            {
                pTypeRead = typeof(Thing);
                return null;
            }
            pTypeRead = BitBuffer.kTypeIndexList[index];
            return this.Read(pTypeRead);
        }

        public void Write(object obj)
        {
            switch (obj)
            {
                case string _:
                    this.Write((string)obj);
                    break;
                case byte[] _:
                    this.Write((byte[])obj, 0, -1);
                    break;
                case BitBuffer _:
                    this.Write(obj as BitBuffer, true);
                    break;
                case float val1:
                    this.Write(val1);
                    break;
                case double val2:
                    this.Write(val2);
                    break;
                case byte val3:
                    this.Write(val3);
                    break;
                case sbyte val4:
                    this.Write(val4);
                    break;
                case bool val5:
                    this.Write(val5);
                    break;
                case short val6:
                    this.Write(val6);
                    break;
                case ushort val7:
                    this.Write(val7);
                    break;
                case int val8:
                    this.Write(val8);
                    break;
                case uint val9:
                    this.Write(val9);
                    break;
                case long val10:
                    this.Write(val10);
                    break;
                case ulong val11:
                    this.Write(val11);
                    break;
                case char val12:
                    this.Write(val12);
                    break;
                case Vec2 val13:
                    this.Write(val13);
                    break;
                case Color val14:
                    this.Write(val14);
                    break;
                case NetIndex16 val15:
                    this.Write((ushort)(int)val15);
                    break;
                case NetIndex2 number1:
                    this.WritePacked((int)number1, 2);
                    break;
                case NetIndex4 number2:
                    this.WritePacked((int)number2, 4);
                    break;
                case NetIndex8 number3:
                    this.WritePacked((int)number3, 8);
                    break;
                case Thing _:
                    if (!(obj as Thing).isStateObject && (obj as Thing).specialSyncIndex == 0 || (obj as Thing).level == null)
                    {
                        if ((obj as Thing).level != null && MonoMain.modDebugging)
                        {
                            DevConsole.Log(DCSection.NetCore, "@error |DGRED|!!BitBuffer.Write() - " + obj.GetType().Name + " is not a State Object (isStateObject == false), it has no StateBindings and cannot be written to a Bitbuffer.");
                            DevConsole.Log(DCSection.NetCore, "@error |DGRED|!!Are you sending a NetMessage with a non GhostObject member variable?");
                        }
                        this.Write((object)null);
                        break;
                    }
                    this.Write((obj as Thing).level.networkIndex);
                    if ((obj as Thing).isStateObject)
                    {
                        this.WritePacked(Editor.IDToType[(obj as Thing).GetType()], 10);
                        GhostObject ghostObject = GhostManager.context.MakeGhostLater(obj as Thing);
                        this.Write((ushort)(int)ghostObject.ghostObjectIndex);
                        if (ghostObject.thing.connection != null)
                            break;
                        ghostObject.thing.connection = DuckNetwork.localConnection;
                        break;
                    }
                    this.WritePacked(0, 10);
                    this.Write((obj as Thing).specialSyncIndex);
                    GhostManager.context.MapSpecialSync(obj as Thing, (obj as Thing).specialSyncIndex);
                    break;
                case null:
                    this.Write(DuckNetwork.levelIndex);
                    this.WritePacked(0, 10);
                    this.Write((ushort)0);
                    break;
                default:
                    throw new Exception("Trying to write unsupported type " + obj.GetType()?.ToString() + " to BitBuffer!");
            }
        }

        public void WriteBits(object obj, int bits)
        {
            if (bits == -1)
                this.Write(obj);
            else
                this.WritePacked(Convert.ToInt32(obj), bits);
        }

        private void resize(int bytes)
        {
            int length = _buffer.Count<byte>() * 2;
            while (length < bytes)
                length *= 2;
            byte[] numArray = new byte[length];
            this._buffer.CopyTo(numArray, 0);
            this._buffer = numArray;
        }

        public void Clear()
        {
            this.position = 0;
            this._endPosition = 0;
            this._bitOffsetPosition = 0;
            this._bitEndOffset = 0;
            Array.Clear(_buffer, 0, this._buffer.Length);
        }

        public void QuickClear()
        {
            this.position = 0;
            this._endPosition = 0;
            this._bitOffsetPosition = 0;
            this._bitEndOffset = 0;
        }
    }
}
