// Decompiled with JetBrains decompiler
// Type: DuckGame.BitBuffer
//removed for regex reasons Culture=neutral, PublicKeyToken=null
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
        public static List<Type> kTypeIndexList = new List<Type>()
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
            int index = ReadInt();
            if (index >= TokenDeserializer.instance._tokens.Count)
                throw new Exception("BitBuffer.ReadTokenizedString() encountered an invalid token.");
            return TokenDeserializer.instance._tokens[index];
        }

        private void WriteTokenizedString(string val) => Write(TokenSerializer.instance.Token(val));

        public override string ToString()
        {
            string str = "";
            for (int index = 0; index < lengthInBytes; ++index)
                str = str + _buffer[index].ToString() + "|";
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
            if (_maxMasks == null)
            {
                _maxMasks = new int[64];
                int num1 = 0;
                for (int index = 0; index < 64; ++index)
                {
                    int num2 = num1 | 1;
                    _maxMasks[index] = num2;
                    num1 = num2 << 1;
                }
            }
            return _maxMasks[bits];
        }

        /// <summary>
        /// This BitBuffers internal buffer. This may have zeroes at the end, as the buffer size is doubled whenever it's filled.
        /// </summary>
        public byte[] buffer => _buffer;

        /// <summary>A byte[] representation of all data in the buffer.</summary>
        public byte[] data
        {
            get
            {
                try
                {
                    byte[] destinationArray = new byte[lengthInBytes];
                    Array.Copy(_buffer, destinationArray, lengthInBytes);
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
            get => _offsetPosition;
            set
            {
                if (_offsetPosition != value)
                    _dirty = true;
                _offsetPosition = value;
                if (_offsetPosition <= _endPosition)
                    return;
                _endPosition = _offsetPosition;
                _bitEndOffset = 0;
            }
        }

        public uint positionInBits
        {
            get => (uint)(position * 8 + _bitOffsetPosition);
            set
            {
                position = (int)(value / 8U);
                bitOffset = (int)(value % 8U);
            }
        }

        public int bitOffset
        {
            get => _bitOffsetPosition;
            set
            {
                if (_bitOffsetPosition != value)
                    _dirty = true;
                _bitOffsetPosition = value;
                if (_endPosition != _offsetPosition || _bitOffsetPosition <= _bitEndOffset)
                    return;
                _bitEndOffset = value;
            }
        }

        public bool isPacked => _bitEndOffset != 0;

        public int lengthInBits => _endPosition * 8 + _bitEndOffset;

        public int lengthInBytes
        {
            get => _endPosition + (_bitEndOffset > 0 ? 1 : 0);
            set => _endPosition = value;
        }

        public byte[] GetBytes()
        {
            if (_trimmedBuffer != null && !_dirty)
                return _trimmedBuffer;
            _dirty = false;
            _trimmedBuffer = new byte[lengthInBytes];
            for (int index = 0; index < lengthInBytes; ++index)
                _trimmedBuffer[index] = _buffer[index];
            return _trimmedBuffer;
        }

        private void calculateReadMasks()
        {
            if (_readMasks != null)
                return;
            _readMasks = new int[64];
            int num1 = 0;
            for (int index = 0; index < 64; ++index)
            {
                int num2 = num1 | 1;
                _readMasks[index] = num2;
                num1 = num2 << 1;
            }
        }

        public bool allowPacking => _allowPacking;

        public BitBuffer(bool allowPacking = true)
        {
            calculateReadMasks();
            _allowPacking = allowPacking;
        }

        public BitBuffer(byte[] data, int bits = 0, bool allowPacking = true)
        {
            _allowPacking = allowPacking;
            calculateReadMasks();
            Write(data, 0, -1);
            SeekToStart();
            if (bits <= 0 || _endPosition * 8 <= bits)
                return;
            --_endPosition;
            _bitEndOffset = bits - _endPosition * 8;
        }

        public BitBuffer(byte[] data, bool copyData)
        {
            _allowPacking = false;
            calculateReadMasks();
            if (copyData)
            {
                Write(data, 0, -1);
                SeekToStart();
            }
            else
                _buffer = data;
        }

        public void SeekToStart()
        {
            position = 0;
            _bitOffsetPosition = 0;
        }

        public void Fill(byte[] bytes, int offset = 0, int vbitOffset = 0)
        {
            _buffer = bytes;
            position = offset;
            _bitOffsetPosition = vbitOffset;
        }

        public BitBuffer Instance() => new BitBuffer()
        {
            _buffer = buffer,
            _offsetPosition = _offsetPosition,
            _endPosition = _endPosition,
            _bitEndOffset = _bitEndOffset,
            _bitOffsetPosition = _bitOffsetPosition
        };

        public int ReadPackedBits(int bits)
        {
            if (bits == 0)
                return 0;
            int num1 = 0;
            if (bits <= 8 - _bitOffsetPosition)
            {
                num1 = _buffer[position] >> _bitOffsetPosition & _readMasks[bits - 1];
                bitOffset = _bitOffsetPosition + bits;
            }
            else
            {
                int num2 = 0;
                while (true)
                {
                    if (_bitOffsetPosition > 7)
                    {
                        bitOffset = 0;
                        ++position;
                    }
                    if (bits > 0)
                    {
                        int num3 = 8 - _bitOffsetPosition;
                        if (num3 > bits)
                            num3 = bits;
                        int num4 = _buffer[position] >> _bitOffsetPosition & _readMasks[num3 - 1];
                        bits -= num3;
                        int num5 = num4 << num2;
                        num1 |= num5;
                        bitOffset = _bitOffsetPosition + num3;
                        num2 += num3;
                    }
                    else
                        break;
                }
            }
            if (_bitOffsetPosition > 7)
            {
                bitOffset = 0;
                ++position;
            }
            return num1;
        }

        public byte[] ReadPacked(int bytes)
        {
            byte[] numArray = new byte[bytes];
            for (int index = 0; index < bytes; ++index)
                numArray[index] = (byte)ReadPackedBits(8);
            return numArray;
        }

        public void WritePacked(int number, int bits)
        {
            try
            {
                if (lengthInBits + bits > _buffer.Length * 8)
                    resize(_buffer.Length * 2);
                //this.currentBit = 0;
                while (bits > 0)
                {
                    _buffer[position] |= (byte)((number & 1) << _bitOffsetPosition);
                    number >>= 1;
                    bitOffset = _bitOffsetPosition + 1;
                    --bits;
                    if (_bitOffsetPosition == 8)
                    {
                        ++position;
                        bitOffset = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                string[] strArray = new string[10];
                strArray[0] = Main.SpecialCode;
                strArray[1] = bits.ToString();
                strArray[2] = ", ";
                int num = lengthInBits;
                strArray[3] = num.ToString();
                strArray[4] = ", ";
                num = _buffer.Length;
                strArray[5] = num.ToString();
                strArray[6] = ", ";
                num = position;
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
                WritePacked(number, 8);
        }

        public void WritePacked(byte[] data, int bits)
        {
            if (position + (int)Math.Ceiling(bits / 8f) > _buffer.Length)
                resize((position + (int)Math.Ceiling(bits / 8f)) * 2);
            int index = 0;
            if (!isPacked)
            {
                for (; bits >= 8; bits -= 8)
                {
                    _buffer[position] = data[index];
                    ++position;
                    ++index;
                }
            }
            else
            {
                for (; bits >= 8; bits -= 8)
                {
                    WritePacked(data[index], 8);
                    ++index;
                }
            }
            if (bits <= 0)
                return;
            WritePacked(data[index], bits);
        }

        public BitBuffer ReadBitBuffer(bool allowPacking = true)
        {
            int length1 = ReadUShort();
            if (length1 == ushort.MaxValue)
                length1 = ReadInt();
            byte[] numArray;
            if (allowPacking)
            {
                int length2 = (int)Math.Ceiling(length1 / 8f);
                numArray = new byte[length2];
                int num = length1;
                for (int index = 0; index < length2; ++index)
                {
                    numArray[index] = (byte)ReadPackedBits(num >= 8 ? 8 : num);
                    if (num >= 8)
                        num -= 8;
                }
            }
            else
            {
                numArray = new byte[length1];
                Array.Copy(buffer, position, numArray, 0, length1);
                position += length1;
                length1 = 0;
            }
            return new BitBuffer(numArray, length1, allowPacking);
        }

        public string ReadString()
        {
            if (TokenDeserializer.instance != null)
                return ReadTokenizedString();
            int num = ReadUShort();
            if (num == ushort.MaxValue)
            {
                int bitOffset = _bitOffsetPosition;
                int position = this.position;
                if (ReadUShort() == 42252)
                {
                    num = ReadInt();
                }
                else
                {
                    this.position = position;
                    this.bitOffset = bitOffset;
                }
            }
            if (_bitOffsetPosition != 0)
                return Encoding.UTF8.GetString(ReadPacked(num));
            string str = Encoding.UTF8.GetString(_buffer, position, num);
            position += num;
            return str;
        }

        public long ReadLong()
        {
            if (_bitOffsetPosition != 0)
                return BitConverter.ToInt64(ReadPacked(8), 0);
            long int64 = BitConverter.ToInt64(_buffer, position);
            position += 8;
            return int64;
        }

        public ulong ReadULong()
        {
            if (_bitOffsetPosition != 0)
                return BitConverter.ToUInt64(ReadPacked(8), 0);
            long uint64 = (long)BitConverter.ToUInt64(_buffer, position);
            position += 8;
            return (ulong)uint64;
        }

        public int ReadInt()
        {
            if (_bitOffsetPosition != 0)
                return BitConverter.ToInt32(ReadPacked(4), 0);
            int int32 = BitConverter.ToInt32(_buffer, position);
            position += 4;
            return int32;
        }

        public uint ReadUInt()
        {
            if (_bitOffsetPosition != 0)
                return BitConverter.ToUInt32(ReadPacked(4), 0);
            int uint32 = (int)BitConverter.ToUInt32(_buffer, position);
            position += 4;
            return (uint)uint32;
        }

        public short ReadShort()
        {
            if (_bitOffsetPosition != 0)
                return BitConverter.ToInt16(ReadPacked(2), 0);
            int int16 = BitConverter.ToInt16(_buffer, position);
            position += 2;
            return (short)int16;
        }

        public ushort ReadUShort()
        {
            if (_bitOffsetPosition != 0)
                return BitConverter.ToUInt16(ReadPacked(2), 0);
            int uint16 = BitConverter.ToUInt16(_buffer, position);
            position += 2;
            return (ushort)uint16;
        }

        public float ReadFloat()
        {
            if (_bitOffsetPosition != 0)
                return BitConverter.ToSingle(ReadPacked(4), 0);
            double single = BitConverter.ToSingle(_buffer, position);
            position += 4;
            return (float)single;
        }

        public Vec2 ReadVec2() => new Vec2()
        {
            x = ReadFloat(),
            y = ReadFloat()
        };

        public Color ReadColor() => new Color()
        {
            r = ReadByte(),
            g = ReadByte(),
            b = ReadByte(),
            a = ReadByte()
        };

        public Color ReadRGBColor() => new Color()
        {
            r = ReadByte(),
            g = ReadByte(),
            b = ReadByte(),
            a = byte.MaxValue
        };

        public double ReadDouble()
        {
            if (_bitOffsetPosition != 0)
                return BitConverter.ToDouble(ReadPacked(8), 0);
            double num = BitConverter.ToDouble(_buffer, position);
            position += 8;
            return num;
        }

        public char ReadChar()
        {
            if (_bitOffsetPosition != 0)
                return BitConverter.ToChar(ReadPacked(2), 0);
            int num = BitConverter.ToChar(_buffer, position);
            position += 2;
            return (char)num;
        }

        public byte ReadByte()
        {
            if (_bitOffsetPosition != 0)
                return ReadPacked(1)[0];
            int num = _buffer[position];
            ++position;
            return (byte)num;
        }

        public byte[] ReadBytes()
        {
            int length = ReadInt();
            byte[] destinationArray = new byte[length];
            Array.Copy(buffer, position, destinationArray, 0, length);
            position += length;
            return destinationArray;
        }

        public sbyte ReadSByte()
        {
            if (_bitOffsetPosition != 0)
                return (sbyte)ReadPacked(1)[0];
            int num = (sbyte)_buffer[position];
            ++position;
            return (sbyte)num;
        }

        public bool ReadBool()
        {
            if (_allowPacking)
                return ReadPackedBits(1) > 0;
            return ReadByte() > 0;
        }

        public NetIndex4 ReadNetIndex4() => new NetIndex4(ReadPackedBits(4));

        public NetIndex8 ReadNetIndex8() => new NetIndex8(ReadPackedBits(8));

        public NetIndex16 ReadNetIndex16() => new NetIndex16(ReadPackedBits(16));

        public byte[] ReadData(int length)
        {
            byte[] dst = new byte[length];
            Buffer.BlockCopy(buffer, position, dst, 0, length);
            position += length;
            return dst;
        }

        public object Read(Type type, bool allowPacking = true)
        {
            if (type == typeof(string))
                return ReadString();
            if (type == typeof(float))
                return ReadFloat();
            if (type == typeof(double))
                return ReadDouble();
            if (type == typeof(byte))
                return ReadByte();
            if (type == typeof(sbyte))
                return ReadSByte();
            if (type == typeof(bool))
                return ReadBool();
            if (type == typeof(short))
                return ReadShort();
            if (type == typeof(ushort))
                return ReadUShort();
            if (type == typeof(int))
                return ReadInt();
            if (type == typeof(uint))
                return ReadUInt();
            if (type == typeof(long))
                return ReadLong();
            if (type == typeof(ulong))
                return ReadULong();
            if (type == typeof(char))
                return ReadChar();
            if (type == typeof(Vec2))
                return ReadVec2();
            if (type == typeof(BitBuffer))
                return ReadBitBuffer(allowPacking);
            if (type == typeof(NetIndex16))
                return new NetIndex16(ReadUShort());
            if (type == typeof(NetIndex2))
                return new NetIndex2((int)ReadBits(typeof(int), 2));
            if (type == typeof(NetIndex4))
                return new NetIndex4((int)ReadBits(typeof(int), 4));
            if (type == typeof(NetIndex8))
                return new NetIndex8((int)ReadBits(typeof(int), 8));
            return typeof(Thing).IsAssignableFrom(type) ? (object)ReadThing(type) : throw new Exception("Trying to read unsupported type " + type?.ToString() + " from BitBuffer!");
        }

        public Thing ReadThing(Type pThingType)
        {
            byte num = ReadByte();
            ushort key = (ushort)ReadBits(typeof(ushort), 10);
            ushort index = ReadUShort();
            if (num != DuckNetwork.levelIndex || index == 0)
                return null;
            if (key == 0)
                return GhostManager.context.GetSpecialSync(index);
            NetIndex16 netIndex16 = (NetIndex16)index;
            Profile profile = GhostObject.IndexToProfile(netIndex16);
            if (profile != null && profile.removedGhosts.TryGetValue(netIndex16, out GhostObject removedGhost))
                return removedGhost.thing;
            Type type = Editor.IDToType[key];
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

        public object ReadBits(Type t, int bits) => bits == -1 ? Read(t) : ConvertType(ReadPackedBits(bits), t);

        public T ReadBits<T>(int bits) => bits < 1 ? default(T) : (T)ConvertType(ReadPackedBits(bits), typeof(T));

        protected object ConvertType(int obj, Type type)
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

        public T Read<T>() => (T)Read(typeof(T));

        public void AlignToByte()
        {
            if (_bitOffsetPosition <= 0)
                return;
            ++position;
            bitOffset = 0;
        }

        public void WriteBufferData(BitBuffer val)
        {
            if (!val.isPacked && !isPacked)
            {
                if (position + val.lengthInBytes > _buffer.Length)
                    resize(position + val.lengthInBytes);
                for (int index = 0; index < val.lengthInBytes; ++index)
                {
                    _buffer[position] = val.buffer[index];
                    ++position;
                }
            }
            else
                WritePacked(val.buffer, val.lengthInBits);
        }

        public void Write(BitBuffer val, bool writeLength = true)
        {
            if (writeLength)
            {
                int val1 = val.allowPacking ? val.lengthInBits : val.lengthInBytes;
                if (val1 > 65534)
                {
                    Write(ushort.MaxValue);
                    Write(val1);
                }
                else
                    Write((ushort)val1);
            }
            WriteBufferData(val);
        }

        public void Write(byte[] val, bool writeLength)
        {
            if (writeLength)
                Write(val.Length);
            Write(val, length: val.Length);
        }

        public void Write(byte[] data, int offset = 0, int length = -1)
        {
            if (!isPacked || _bitOffsetPosition == 0)
            {
                if (length < 0)
                    length = data.Length;
                if (position + length > _buffer.Length)
                    resize(position + length);
                Array.Copy(data, offset, buffer, position, length);
                position += length;
            }
            else
                WritePacked(data);
        }

        public void Write(string val)
        {
            if (TokenSerializer.instance != null)
            {
                WriteTokenizedString(val);
            }
            else
            {
                byte[] bytes = Encoding.UTF8.GetBytes(val);
                if (_bitOffsetPosition != 0)
                {
                    Write((ushort)bytes.Length);
                    WritePacked(bytes);
                }
                else
                {
                    int val1 = bytes.Length;
                    if (val1 > ushort.MaxValue)
                    {
                        Write(ushort.MaxValue);
                        Write((ushort)42252);
                        Write(val1);
                    }
                    else
                        Write((ushort)bytes.Length);
                    int num = bytes.Length;
                    if (position + num > _buffer.Length)
                        resize(position + num);
                    bytes.CopyTo(_buffer, position);
                    position += num;
                }
            }
        }

        public void Write(long val)
        {
            byte[] bytes = BitConverter.GetBytes(val);
            if (_bitOffsetPosition != 0)
            {
                WritePacked(bytes);
            }
            else
            {
                byte num = (byte)bytes.Length;
                if (position + num > _buffer.Length)
                    resize(position + num);
                bytes.CopyTo(_buffer, position);
                position += bytes.Length;
            }
        }

        public void Write(ulong val)
        {
            byte[] bytes = BitConverter.GetBytes(val);
            if (_bitOffsetPosition != 0)
            {
                WritePacked(bytes);
            }
            else
            {
                byte num = (byte)bytes.Length;
                if (position + num > _buffer.Length)
                    resize(position + num);
                bytes.CopyTo(_buffer, position);
                position += bytes.Length;
            }
        }

        public void Write(int val)
        {
            byte[] bytes = BitConverter.GetBytes(val);
            if (_bitOffsetPosition != 0)
            {
                WritePacked(bytes);
            }
            else
            {
                byte num = (byte)bytes.Length;
                if (position + num > _buffer.Length)
                    resize(position + num);
                bytes.CopyTo(_buffer, position);
                position += bytes.Length;
            }
        }

        public void Write(uint val)
        {
            byte[] bytes = BitConverter.GetBytes(val);
            if (_bitOffsetPosition != 0)
            {
                WritePacked(bytes);
            }
            else
            {
                byte num = (byte)bytes.Length;
                if (position + num > _buffer.Length)
                    resize(position + num);
                bytes.CopyTo(_buffer, position);
                position += bytes.Length;
            }
        }

        public void Write(short val)
        {
            byte[] bytes = BitConverter.GetBytes(val);
            if (_bitOffsetPosition != 0)
            {
                WritePacked(bytes);
            }
            else
            {
                byte num = (byte)bytes.Length;
                if (position + num > _buffer.Length)
                    resize(position + num);
                bytes.CopyTo(_buffer, position);
                position += bytes.Length;
            }
        }

        public void Write(ushort val)
        {
            byte[] bytes = BitConverter.GetBytes(val);
            if (_bitOffsetPosition != 0)
            {
                WritePacked(bytes);
            }
            else
            {
                byte num = (byte)bytes.Length;
                if (position + num > _buffer.Length)
                    resize(position + num);
                bytes.CopyTo(_buffer, position);
                position += bytes.Length;
            }
        }

        public void Write(float val)
        {
            byte[] bytes = BitConverter.GetBytes(val);
            if (_bitOffsetPosition != 0)
            {
                WritePacked(bytes);
            }
            else
            {
                byte num = (byte)bytes.Length;
                if (position + num > _buffer.Length)
                    resize(position + num);
                bytes.CopyTo(_buffer, position);
                position += bytes.Length;
            }
        }

        public void Write(Vec2 val)
        {
            Write(val.x);
            Write(val.y);
        }

        public void Write(Color val)
        {
            Write(val.r);
            Write(val.g);
            Write(val.b);
            Write(val.a);
        }

        public void WriteRGBColor(Color val)
        {
            Write(val.r);
            Write(val.g);
            Write(val.b);
        }

        public void Write(double val)
        {
            byte[] bytes = BitConverter.GetBytes(val);
            if (_bitOffsetPosition != 0)
            {
                WritePacked(bytes);
            }
            else
            {
                byte num = (byte)bytes.Length;
                if (position + num > _buffer.Length)
                    resize(position + num);
                bytes.CopyTo(_buffer, position);
                position += bytes.Length;
            }
        }

        public void Write(char val)
        {
            byte[] bytes = BitConverter.GetBytes(val);
            if (_bitOffsetPosition != 0)
            {
                WritePacked(bytes);
            }
            else
            {
                byte num = (byte)bytes.Length;
                if (position + num > _buffer.Length)
                    resize(position + num);
                bytes.CopyTo(_buffer, position);
                position += bytes.Length;
            }
        }

        public void Write(byte val)
        {
            if (_bitOffsetPosition != 0)
            {
                WritePacked(val, 8);
            }
            else
            {
                if (position + 1 > _buffer.Length)
                    resize(position + 1);
                _buffer[position] = val;
                ++position;
            }
        }

        public void Write(sbyte val)
        {
            if (_bitOffsetPosition != 0)
            {
                WritePacked(val, 8);
            }
            else
            {
                if (position + 1 > _buffer.Length)
                    resize(position + 1);
                _buffer[position] = (byte)val;
                ++position;
            }
        }

        public void Write(bool val)
        {
            if (_allowPacking)
                WritePacked(val ? 1 : 0, 1);
            else
                Write(val ? (byte)1 : (byte)0);
        }

        public void WriteProfile(Profile pValue)
        {
            if (pValue == null)
                Write((sbyte)-1);
            else
                Write((sbyte)pValue.networkIndex);
        }

        public Profile ReadProfile()
        {
            sbyte index = ReadSByte();
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
            Write((ushort)val);
        }

        public Team ReadTeam() => Teams.ParseFromIndex(ReadUShort());

        public void WriteObject(object obj)
        {
            int val = byte.MaxValue;
            if (obj != null)
                val = !(obj is Thing) ? kTypeIndexList.IndexOf(obj.GetType()) : kTypeIndexList.IndexOf(typeof(Thing));
            if (val < 0)
                throw new Exception("Trying to write unsupported type to BitBuffer through WriteObject!");
            Write((byte)val);
            Write(obj);
        }

        public object ReadObject(out Type pTypeRead)
        {
            byte index = ReadByte();
            if (index == byte.MaxValue || index >= kTypeIndexList.Count)
            {
                pTypeRead = typeof(Thing);
                return null;
            }
            pTypeRead = kTypeIndexList[index];
            return Read(pTypeRead);
        }

        public void Write(object obj)
        {
            switch (obj)
            {
                case string _:
                    Write((string)obj);
                    break;
                case byte[] _:
                    Write((byte[])obj, 0, -1);
                    break;
                case BitBuffer _:
                    Write(obj as BitBuffer, true);
                    break;
                case float val1:
                    Write(val1);
                    break;
                case double val2:
                    Write(val2);
                    break;
                case byte val3:
                    Write(val3);
                    break;
                case sbyte val4:
                    Write(val4);
                    break;
                case bool val5:
                    Write(val5);
                    break;
                case short val6:
                    Write(val6);
                    break;
                case ushort val7:
                    Write(val7);
                    break;
                case int val8:
                    Write(val8);
                    break;
                case uint val9:
                    Write(val9);
                    break;
                case long val10:
                    Write(val10);
                    break;
                case ulong val11:
                    Write(val11);
                    break;
                case char val12:
                    Write(val12);
                    break;
                case Vec2 val13:
                    Write(val13);
                    break;
                case Color val14:
                    Write(val14);
                    break;
                case NetIndex16 val15:
                    Write((ushort)(int)val15);
                    break;
                case NetIndex2 number1:
                    WritePacked((int)number1, 2);
                    break;
                case NetIndex4 number2:
                    WritePacked((int)number2, 4);
                    break;
                case NetIndex8 number3:
                    WritePacked((int)number3, 8);
                    break;
                case Thing _:
                    if (!(obj as Thing).isStateObject && (obj as Thing).specialSyncIndex == 0 || (obj as Thing).level == null)
                    {
                        if ((obj as Thing).level != null && MonoMain.modDebugging)
                        {
                            DevConsole.Log(DCSection.NetCore, "@error |DGRED|!!BitBuffer.Write() - " + obj.GetType().Name + " is not a State Object (isStateObject == false), it has no StateBindings and cannot be written to a Bitbuffer.");
                            DevConsole.Log(DCSection.NetCore, "@error |DGRED|!!Are you sending a NetMessage with a non GhostObject member variable?");
                        }
                        Write((object)null);
                        break;
                    }
                    Write((obj as Thing).level.networkIndex);
                    if ((obj as Thing).isStateObject)
                    {
                        WritePacked(Editor.IDToType[(obj as Thing).GetType()], 10);
                        GhostObject ghostObject = GhostManager.context.MakeGhostLater(obj as Thing);
                        Write((ushort)(int)ghostObject.ghostObjectIndex);
                        if (ghostObject.thing.connection != null)
                            break;
                        ghostObject.thing.connection = DuckNetwork.localConnection;
                        break;
                    }
                    WritePacked(0, 10);
                    Write((obj as Thing).specialSyncIndex);
                    GhostManager.context.MapSpecialSync(obj as Thing, (obj as Thing).specialSyncIndex);
                    break;
                case null:
                    Write(DuckNetwork.levelIndex);
                    WritePacked(0, 10);
                    Write((ushort)0);
                    break;
                default:
                    throw new Exception("Trying to write unsupported type " + obj.GetType()?.ToString() + " to BitBuffer!");
            }
        }

        public void WriteBits(object obj, int bits)
        {
            if (bits == -1)
                Write(obj);
            else
                WritePacked(Convert.ToInt32(obj), bits);
        }

        private void resize(int bytes)
        {
            int length = _buffer.Length * 2;
            while (length < bytes)
                length *= 2;
            byte[] numArray = new byte[length];
            _buffer.CopyTo(numArray, 0);
            _buffer = numArray;
        }

        public void Clear()
        {
            position = 0;
            _endPosition = 0;
            _bitOffsetPosition = 0;
            _bitEndOffset = 0;
            Array.Clear(_buffer, 0, _buffer.Length);
        }

        public void QuickClear()
        {
            position = 0;
            _endPosition = 0;
            _bitOffsetPosition = 0;
            _bitEndOffset = 0;
        }
    }
}
