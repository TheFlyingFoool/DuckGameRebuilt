using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace DuckGame
{
    public class BitBuffer
    {
        public string ReadTokenizedString()
        {
            int token = ReadInt();
            if (token >= TokenDeserializer.instance._tokens.Count)
                throw new Exception("BitBuffer.ReadTokenizedString() encountered an invalid token.");

            return TokenDeserializer.instance._tokens[token];
        }

        private void WriteTokenizedString(string val)
        {
            Write(TokenSerializer.instance.Token(val));
        }

        public override string ToString()
        {
            string s = "";
            for (int i = 0; i < lengthInBytes; i++)
            {
                s += _buffer[i].ToString();
                s += "|";
            }

            return s;
        }

        public static BitBuffer FromString(string s)
        {
            BitBuffer b = new BitBuffer();
            try
            {
                foreach (string part in s.Split('|'))
                {
                    if (part == "")
                        continue;
                    b.Write(Convert.ToByte(part));
                }
                b.position = 0;
            }
            catch (Exception e)
            {
                DevConsole.Log(DCSection.General, "BitBuffer conversion from string failed.");
                return new BitBuffer();
            }

            return b;
        }

        private static int[] _maxMasks;
        public static long GetMaxValue(int bits)
        {
            if (_maxMasks == null)
            {
                _maxMasks = new int[64];
                int val = 0;
                for (int i = 0; i < 64; i++)
                {
                    val |= 1;
                    _maxMasks[i] = val;
                    val = val << 1;
                }
            }
            return _maxMasks[bits];
        }


        byte[] _buffer = new byte[64];

        /// <summary>
        /// This BitBuffers internal buffer. This may have zeroes at the end, as the buffer size is doubled whenever it's filled.
        /// </summary>
		public byte[] buffer { get { return _buffer; } }

        /// <summary>
        /// A byte[] representation of all data in the buffer.
        /// </summary>
        public byte[] data
        {
            get
            {
                try
                {
                    byte[] ret = new byte[lengthInBytes];
                    Array.Copy(_buffer, ret, lengthInBytes);
                    return ret;
                }
                catch (Exception e)
                {
                    return null;
                }
            }
        }


        bool _dirty = false;
        int _offsetPosition = 0; //position in bytes
        int _endPosition = 0;
        public int position
        {
            get { return _offsetPosition; }
            set
            {
                if (_offsetPosition != value)
                    _dirty = true;

                _offsetPosition = value;
                if (_offsetPosition > _endPosition)
                {
                    _endPosition = _offsetPosition;
                    _bitEndOffset = 0;
                }
            }
        }

        public uint positionInBits
        {
            get { return (((uint)position * 8) + (uint)bitOffset); }
            set
            {
                position = (int)(value / 8);
                bitOffset = (int)(value % 8);
            }
        }

        int _bitEndOffset = 0;
        int _bitOffsetPosition = 0; //offset from position in bits
        public int bitOffset
        {
            get { return _bitOffsetPosition; }
            set
            {
                if (_bitOffsetPosition != value)
                    _dirty = true;

                _bitOffsetPosition = value;
                if (_endPosition == _offsetPosition && _bitOffsetPosition > _bitEndOffset)
                    _bitEndOffset = value;
            }
        }

        public bool isPacked { get { return _bitEndOffset != 0; } }
        public int lengthInBits { get { return (_endPosition * 8) + _bitEndOffset; } }
        public int lengthInBytes { get { return _endPosition + (_bitEndOffset > 0 ? 1 : 0); } set { _endPosition = value; } }

        private byte[] _trimmedBuffer = null;
        public byte[] GetBytes()
        {
            if (_trimmedBuffer != null && _dirty == false)
                return _trimmedBuffer;
            _dirty = false;
            _trimmedBuffer = new byte[lengthInBytes];
            for (int i = 0; i < lengthInBytes; i++)
                _trimmedBuffer[i] = _buffer[i];
            return _trimmedBuffer;
        }

        private static int[] _readMasks;
        private static ulong[] _longMasks;
        private static int[] _chunkMasks;
        private void calculateReadMasks()
        {
            if (_readMasks == null)
            {
                _readMasks = new int[64];

                int val = 0;
                for (int i = 0; i < 64; i++)
                {
                    val = val | 1;
                    _readMasks[i] = val;
                    val = val << 1;
                }

                _longMasks = new ulong[65];

                ulong lval = 0;
                for (int i = 1; i < 65; i++)
                {
                    lval = lval | 1;
                    _longMasks[i] = lval;
                    lval = lval << 1;
                }

                _chunkMasks = new int[8];

                int chnk = 0;
                for (int i = 7; i >= 0; i--)
                {
                    chnk = chnk | (byte)1;
                    _chunkMasks[i] = chnk;
                    chnk = chnk << (byte)1;
                }
            }
        }

        private bool _allowPacking = true;
        public bool allowPacking { get { return _allowPacking; } }
        public BitBuffer(bool allowPacking = true)
        {
            calculateReadMasks();
            _allowPacking = allowPacking;
        }


        public BitBuffer(byte[] data, int bits = 0, bool allowPacking = true)
        {
            _allowPacking = allowPacking;

            calculateReadMasks();
            Write(data);
            SeekToStart();

            if (bits > 0 && _endPosition * 8 > bits)
            {
                _endPosition -= 1;
                _bitEndOffset = bits - (_endPosition * 8);
            }
        }

        void ConstructSetBits(int bits)
        {
            if (bits > 0 && _endPosition * 8 > bits)
            {
                _endPosition -= 1;
                _bitEndOffset = bits - (_endPosition * 8);
            }
        }

        public BitBuffer(byte[] data, bool copyData)
        {
            _allowPacking = false;

            calculateReadMasks();

            if (copyData)
            {
                Write(data);
                SeekToStart();
            }
            else
                _buffer = data;
        }

        //public void CheckForTokenization()
        //{
        //    int pos = position;
        //    if (lengthInBytes > 8)
        //    {
        //        position = lengthInBytes - 8;
        //        long read = ReadLong();
        //        if (read == kTokenizedIdentifier)
        //            MakeTokenized();;
        //    }

        //    position = pos;
        //}

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

        //Returns a copy of the buffer as it is right now, with the same underlying data reference.
        public BitBuffer Instance()
        {
            BitBuffer ret = new BitBuffer();
            ret._buffer = buffer;
            ret._offsetPosition = _offsetPosition;
            ret._endPosition = _endPosition;
            ret._bitEndOffset = _bitEndOffset;
            ret._bitOffsetPosition = _bitOffsetPosition;
            return ret;
        }

        public int ReadPackedBits(int bits)
        {
            if (bits == 0)
                return 0;

            int number = 0;
            if (bits <= 8 - _bitOffsetPosition)
            {
                number = (_buffer[_offsetPosition] >> _bitOffsetPosition) & _readMasks[bits - 1];
                _bitOffsetPosition += bits;
            }
            else
            {
                int read = 0;
                int soFar = 0;
                while (true)
                {
                    if (_bitOffsetPosition > 7)
                    {
                        _bitOffsetPosition = 0;
                        _offsetPosition += 1;
                    }
                    if (bits <= 0)
                    {
                        break;
                    }

                    int numRead = 8 - _bitOffsetPosition;
                    if (numRead > bits)
                        numRead = bits;


                    read = (_buffer[_offsetPosition] >> _bitOffsetPosition) & _readMasks[numRead - 1];
                    bits -= numRead;
                    read = read << soFar;
                    number = number | read;
                    _bitOffsetPosition += numRead;
                    soFar += numRead;
                }
            }
            if (_bitOffsetPosition > 7)
            {
                _bitOffsetPosition = 0;
                _offsetPosition += 1;
            }
            return number;
        }


        //public int ReadPackedBits2(int bits)
        //{
        //    if (bits == 0)
        //        return 0;

        //    int ret = 0;
        //    for (int i = 0; i < bits; ++i)
        //    {
        //        ret |= (int)(_buffer[position] >> (8 - bitOffset)) & 1;
        //        bitOffset++;

        //        if (bitOffset == 8)
        //        {
        //            position++;
        //            bitOffset = 0;
        //        }
        //    }
        //    return ret;
        //}

        public byte[] ReadPacked(int bytes)
        {
            byte[] data = new byte[bytes];
            for (int i = 0; i < bytes; i++)
                data[i] = (byte)ReadPackedBits(8);
            return data;
        }

        public unsafe ulong ReadAsULong(int bytes)
        {
            ulong data = 0;
            if (_bitOffsetPosition != 0)
            {
                byte* ul = (byte*)&data;
                for (int i = 0; i < bytes; i++)
                {
                    ul[i] = (byte)ReadPackedBits(8);
                }
            }
            else
            {
                byte* ul = (byte*)&data;
                for (int i = 0; i < bytes; i++)
                {
                    ul[i] = _buffer[_offsetPosition + i];
                }
                position += bytes;
            }
            return data;
        }

        private int offset = 0;
        private int currentBit = 0;


        int _localPosition;
        //      public void WritePacked(uint number, int bits)
        //{
        //          //try
        //          //{
        //          if (lengthInBits + bits > _buffer.Length * 8)
        //              resize(_buffer.Length * 2);

        //          currentBit = 0;
        //          while (bits > 0)
        //          {
        //              _buffer[_offsetPosition] |= (byte)((number & 1) << _bitOffsetPosition);
        //              number = number >> 1;
        //              _bitOffsetPosition++;
        //              bits--;

        //              if (_bitOffsetPosition == 8)
        //              {
        //                  _offsetPosition++;
        //                  _bitOffsetPosition = 0;
        //              }
        //          }
        //          //}
        //          //catch(Exception e)
        //          //{
        //          //    Main.SpecialCode += bits.ToString() + ", " + lengthInBits.ToString() + ", " + _buffer.Length.ToString() + ", " + position.ToString() + ", " + number.ToString();
        //          //    throw e;
        //          //}


        //          if (_offsetPosition > _endPosition)
        //              _endPosition = _offsetPosition;

        //          if (_bitOffsetPosition > _bitEndOffset)
        //              _bitEndOffset = _bitOffsetPosition;
        //      }

        public void WritePackedSlow(int number, int bits)
        {
            try
            {
                if (lengthInBits + bits > _buffer.Length * 8)
                    resize(_buffer.Length * 2);

                currentBit = 0;
                while (bits > 0)
                {
                    _buffer[_offsetPosition] |= (byte)((number & 1) << _bitOffsetPosition);
                    number = number >> 1;
                    _bitOffsetPosition++;
                    bits--;

                    if (_bitOffsetPosition == 8)
                    {
                        _offsetPosition++;
                        _bitOffsetPosition = 0;
                    }
                }
            }
            catch (Exception e)
            {
                Main.SpecialCode += bits.ToString() + ", " + lengthInBits.ToString() + ", " + _buffer.Length.ToString() + ", " + position.ToString() + ", " + number.ToString();
                throw e;
            }

            if (_offsetPosition > _endPosition)
                _endPosition = _offsetPosition;

            if (_bitOffsetPosition > _bitEndOffset)
                _bitEndOffset = _bitOffsetPosition;
        }

        int _dif;
        public void WritePacked(ulong number, int bits)
        {
            if (lengthInBits + bits >= _buffer.Length * 8)
                resize(_buffer.Length * 2);

            while (bits > 0)
            {
                _buffer[_offsetPosition] |= (byte)((number & _longMasks[bits]) << _bitOffsetPosition);
                _dif = (8 - _bitOffsetPosition);
                number = number >> _dif;
                _bitOffsetPosition += (sbyte)Math.Min(bits, _dif);
                bits -= _dif;
                if (_bitOffsetPosition >= 8)
                {
                    _bitOffsetPosition -= 8;
                    _offsetPosition += 1;
                }
            }

            if (_offsetPosition > _endPosition)
            {
                _endPosition = _offsetPosition;
                _bitEndOffset = _bitOffsetPosition;
            }

            if (_offsetPosition == _endPosition && _bitOffsetPosition > _bitEndOffset)
                _bitEndOffset = _bitOffsetPosition;
        }

        public void WritePacked(long number, int bits)
        {
            if (lengthInBits + bits >= _buffer.Length * 8)
                resize(_buffer.Length * 2);

            while (bits > 0)
            {
                _buffer[_offsetPosition] |= (byte)((number & (long)_longMasks[bits]) << _bitOffsetPosition);
                _dif = (8 - _bitOffsetPosition);
                number = number >> _dif;
                _bitOffsetPosition += (sbyte)Math.Min(bits, _dif);
                bits -= _dif;
                if (_bitOffsetPosition >= 8)
                {
                    _bitOffsetPosition -= 8;
                    _offsetPosition += 1;
                }
            }

            if (_offsetPosition > _endPosition)
            {
                _endPosition = _offsetPosition;
                _bitEndOffset = _bitOffsetPosition;
            }

            if (_offsetPosition == _endPosition && _bitOffsetPosition > _bitEndOffset)
                _bitEndOffset = _bitOffsetPosition;
        }

        public void WritePacked(byte[] data)
        {
            foreach (byte b in data)
                WritePacked(b, 8);
        }

        public void WritePacked(byte[] data, int bits)
        {
            if (position + (int)Math.Ceiling(bits / 8.0f) > _buffer.Length)
                resize((position + (int)Math.Ceiling(bits / 8.0f)) * 2);

            int cur = 0;
            if (!isPacked)
            {
                while (bits >= 8)
                {
                    _buffer[position] = data[cur];
                    position++;
                    cur++;
                    bits -= 8;
                }
            }
            else
            {
                while (bits >= 8)
                {
                    WritePacked(data[cur], 8);
                    cur += 1;
                    bits -= 8;
                }
            }
            if (bits > 0)
                WritePacked(data[cur], bits);
        }

        /*
                // this one should work, and allocate less, but it doesn't and I don't know why
                public BitBuffer ReadBitBuffer(bool allowPacking = true)
                {
                    ushort bits = ReadUShort();

                    BitBuffer newbuf = new BitBuffer(allowPacking);

                    if (allowPacking)
                    {
                        int bytes = (int)Math.Ceiling((float)bits / 8);

                        ushort writeBits = bits;
                        int size = 0;
                        for (int i = 0; i < bytes; i++)
                        {
                            size = (writeBits >= 8 ? 8 : writeBits);
                            byte val = (byte)ReadPackedBits(size);
                            newbuf.WritePacked(val, size);
                            if (writeBits >= 8)
                                writeBits -= 8;
                        }
                    }
                    else
                    {
                        int bytes = bits;
                        for (int i = 0; i < bits; i++)
                            newbuf.Write((byte)ReadByte());
                    }
                    return newbuf;
                }
        */


        private static Stack<BitBuffer> _bufferPool = new Stack<BitBuffer>();
        private bool _pooled = false;
        public static BitBuffer GetFromPool(int minSize = 0, bool allowPacking = true)
        {
            BitBuffer b = null;
            lock (_bufferPool)
            {
                if (_bufferPool.Count > 0)
                    b = _bufferPool.Pop();
                else
                    b = new BitBuffer();
            }

            b.QuickClear();
            b._allowPacking = allowPacking;
            b._pooled = false;

            if (minSize != 0 && b._buffer.Length < minSize)
                b._buffer = new byte[minSize];

            return b;
        }

        public void ReturnToPool()
        {
            if (_pooled == false)
            {
                _pooled = true;
                lock (_bufferPool)
                {
                    _bufferPool.Push(this);
                }
            }
        }

        public BitBuffer ReadBitBufferPooled(bool allowPacking = true)
        {
            int bits = ReadUShort();
            if (bits == ushort.MaxValue)
                bits = ReadInt();

            BitBuffer b = GetFromPool();
            if (allowPacking)
            {
                b._allowPacking = true;
                int bytes = (int)Math.Ceiling((float)bits / 8);
                if (b._buffer.Length < bytes)
                    b._buffer = new byte[bytes];

                int writeBits = bits;
                for (int i = 0; i < bytes; i++)
                {
                    b._buffer[i] = (byte)ReadPackedBits(writeBits >= 8 ? 8 : writeBits);
                    if (writeBits >= 8)
                        writeBits -= 8;
                }
            }
            else
            {
                b._allowPacking = false;
                if (b._buffer.Length < bits)
                    b._buffer = new byte[bits];

                Array.Copy(buffer, position, b._buffer, 0, bits);
                position += bits;

                //for (int i = 0; i < bytes; i++)
                //    data[i] = (byte)ReadByte();

                bits = 0;
            }

            b.ConstructSetBits(bits);
            return b;
        }


        public BitBuffer ReadBitBuffer(bool allowPacking = true)
        {
            int bits = ReadUShort();
            if (bits == ushort.MaxValue)
                bits = ReadInt();


            byte[] data = null;
            if (allowPacking)
            {
                int bytes = (int)Math.Ceiling((float)bits / 8);
                data = new byte[bytes];
                int writeBits = bits;
                for (int i = 0; i < bytes; i++)
                {
                    data[i] = (byte)ReadPackedBits(writeBits >= 8 ? 8 : writeBits);
                    if (writeBits >= 8)
                        writeBits -= 8;
                }
            }
            else
            {
                data = new byte[bits];
                Array.Copy(buffer, position, data, 0, bits);
                position += bits;

                //for (int i = 0; i < bytes; i++)
                //    data[i] = (byte)ReadByte();

                bits = 0;
            }
            return new BitBuffer(data, bits, allowPacking);
        }


        public string ReadString()
        {
            if (TokenDeserializer.instance != null)
                return ReadTokenizedString();

            int length = ReadUShort();
            if (length == ushort.MaxValue)
            {
                int tBitOffset = bitOffset;
                int tPosition = position;

                int p2 = ReadUShort();
                //compatibility stuff for longer strings
                if (p2 == 42252)
                    length = ReadInt();
                else
                {
                    position = tPosition;
                    bitOffset = tBitOffset;
                }

            }

            if (bitOffset != 0)
            {
                byte[] data = ReadPacked(length);
                return System.Text.Encoding.UTF8.GetString(data);
            }
            else
            {
                string s = System.Text.Encoding.UTF8.GetString(_buffer, position, length);
                position += length;
                return s;
            }
        }

        public long ReadLong()
        {
            ulong val = ReadAsULong(sizeof(long));
            unsafe { return ((long*)&val)[0]; }
        }

        public ulong ReadULong()
        {
            return ReadAsULong(sizeof(ulong));
        }

        public int ReadInt()
        {
            ulong val = ReadAsULong(sizeof(int));
            unsafe { return ((int*)&val)[0]; }
        }

        public uint ReadUInt()
        {
            ulong val = ReadAsULong(sizeof(uint));
            unsafe { return ((uint*)&val)[0]; }
        }

        public short ReadShort()
        {
            ulong val = ReadAsULong(sizeof(short));
            unsafe { return ((short*)&val)[0]; }
        }

        public ushort ReadUShort()
        {
            ulong val = ReadAsULong(sizeof(ushort));
            unsafe { return ((ushort*)&val)[0]; }
        }

        public float ReadFloat()
        {
            ulong val = ReadAsULong(sizeof(float));
            unsafe { return ((float*)&val)[0]; }
        }

        public Vec2 ReadVec2()
        {
            Vec2 val = new Vec2();
            val.x = ReadFloat();
            val.y = ReadFloat();
            return val;
        }

        public Color ReadColor()
        {
            Color val = new Color();
            val.r = ReadByte();
            val.g = ReadByte();
            val.b = ReadByte();
            val.a = ReadByte();

            return val;
        }

        public Color ReadRGBColor()
        {
            Color val = new Color();
            val.r = ReadByte();
            val.g = ReadByte();
            val.b = ReadByte();
            val.a = 255;

            return val;
        }

        public double ReadDouble()
        {
            ulong val = ReadAsULong(sizeof(double));
            unsafe { return ((double*)&val)[0]; }
        }

        public char ReadChar()
        {
            ulong val = ReadAsULong(sizeof(char));
            unsafe { return ((char*)&val)[0]; }
        }

        public byte ReadByte()
        {
            ulong val = ReadAsULong(sizeof(byte));
            unsafe { return ((byte*)&val)[0]; }
        }

        public byte[] ReadBytes()
        {
            int size = ReadInt();
            byte[] data = new byte[size];
            Array.Copy(buffer, position, data, 0, size);
            position += size;
            return data;
        }



        public sbyte ReadSByte()
        {
            ulong val = ReadAsULong(sizeof(sbyte));
            unsafe { return ((sbyte*)&val)[0]; }
        }

        public bool ReadBool()
        {
            if (_allowPacking)
                return ReadPackedBits(1) > 0;
            return ReadByte() > 0 ? true : false;
        }

        public NetIndex4 ReadNetIndex4()
        {
            return new NetIndex4(ReadPackedBits(4));
        }

        public NetIndex8 ReadNetIndex8()
        {
            return new NetIndex8(ReadPackedBits(8));
        }

        public NetIndex16 ReadNetIndex16()
        {
            return new NetIndex16(ReadPackedBits(16));
        }


        public byte[] ReadData(int length)
        {
            byte[] data = new byte[length];
            Buffer.BlockCopy(buffer, position, data, 0, length);
            position += length;
            return data;
        }

        public object Read(Type type, bool allowPacking = true)
        {
            if (type == typeof(string))
                return ReadString();
            else if (type == typeof(float))
                return ReadFloat();
            else if (type == typeof(double))
                return ReadDouble();
            else if (type == typeof(byte))
                return ReadByte();
            else if (type == typeof(sbyte))
                return ReadSByte();
            else if (type == typeof(bool))
                return ReadBool();
            else if (type == typeof(short))
                return ReadShort();
            else if (type == typeof(ushort))
                return ReadUShort();
            else if (type == typeof(int))
                return ReadInt();
            else if (type == typeof(uint))
                return ReadUInt();
            else if (type == typeof(long))
                return ReadLong();
            else if (type == typeof(ulong))
                return ReadULong();
            else if (type == typeof(char))
                return ReadChar();
            else if (type == typeof(Vec2))
                return ReadVec2();
            else if (type == typeof(byte[]))
                return ReadBytes();
            else if (type == typeof(BitBuffer))
                return ReadBitBuffer(allowPacking);
            else if (type == typeof(NetIndex16))
                return new NetIndex16((int)ReadUShort());
            else if (type == typeof(NetIndex2))
                return new NetIndex2((int)ReadBits(typeof(int), 2));
            else if (type == typeof(NetIndex4))
                return new NetIndex4((int)ReadBits(typeof(int), 4));
            else if (type == typeof(NetIndex8))
                return new NetIndex8((int)ReadBits(typeof(int), 8));
            else if (typeof(Thing).IsAssignableFrom(type))
                return ReadThing(type);
            else
                throw (new Exception("Trying to read unsupported type " + type + " from BitBuffer!"));
        }

        public Thing ReadThing(Type pThingType)
        {
            Main.SpecialCode2 = "10101";
            byte levelIndex = ReadByte();
            ushort typeIndex = (ushort)ReadBits(typeof(ushort), 10);
            ushort readIndex = ReadUShort();
            Main.SpecialCode2 = "10102";

            if (levelIndex != DuckNetwork.levelIndex || readIndex == 0)
                return null;


            Main.SpecialCode2 = "10103";
            if (typeIndex == 0)
                return GhostManager.context.GetSpecialSync(readIndex);


            Main.SpecialCode2 = "10104";
            NetIndex16 index = readIndex;
            Main.SpecialCode2 = "10105";
            Profile p = GhostObject.IndexToProfile(index);
            Main.SpecialCode2 = "10106";
            if (p != null && p.removedGhosts.ContainsKey(index))
            {
                Main.SpecialCode2 = "10107";
                GhostObject g = p.removedGhosts[index];
                //DevConsole.Log(DCSection.GhostMan, "Ignoring removed ghost(" + g != null ? g.ToString() : index + ")", NetworkConnection.context);
                return g.thing;
            }

            Main.SpecialCode2 = "10108";
            Type realType = Editor.IDToType[typeIndex];
            Main.SpecialCode2 = "10109";
            if (pThingType.IsAssignableFrom(realType) == false)
            {
                Main.SpecialCode2 = "10110";
                DevConsole.Log(DCSection.GhostMan, "@error Type mismatch, ignoring ghost (" + index.ToString() + "(" + realType.GetType().Name + " vs. " + pThingType.Name + "))@error");
                return null;
            }

            Main.SpecialCode2 = "10111";
            GhostObject ghost = GhostManager.context.GetGhost(index);
            //Kill ghost on type mismatch
            Main.SpecialCode2 = "10112";
            if (ghost != null && ghost.thing.GetType() != realType)
            {
                Main.SpecialCode2 = "10113";
                DevConsole.Log(DCSection.GhostMan, "@error Type mismatch, removing ghost (" + index.ToString() + " " + ghost.thing.GetType().ToString() + "(my type) vs. " + realType.ToString() + "(your type))@error");
                GhostManager.changingGhostType = true;
                GhostManager.context.RemoveGhost(ghost, 0);
                GhostManager.changingGhostType = false;
                ghost = null;
            }

            Main.SpecialCode2 = "10114";
            if (ghost == null)
            {
                //HAT FLOATING IN CORNER CAUSED BY DUCK REFERENCE SYNCHRONIZING BEFORE ACTUAL DUCK DOES
                //if (realType == typeof(Duck)) //THIS LINE IS IMPORTANT. REMOVING IT LEADS TO UNINITIALIZED DUCK.PROFILE CRASHES AND MORE
                //    return null;

                Main.SpecialCode2 = "10115";
                Thing t = Editor.CreateThing(realType);
                t.connection = NetworkConnection.context;
                t.authority = 1; //Bit buffer created objects start with a lower authority to ensure smooth transfer

                if (p != null && index > p.latestGhostIndex)
                    p.latestGhostIndex = index;

                Main.SpecialCode2 = "10116";
                if (levelIndex != Level.core.currentLevel.networkIndex)
                {
                    //pending ghost
                    Main.SpecialCode2 = "10117";
                    ghost = new GhostObject(t, GhostManager.context, index, false);
                    t.position = new Vec2(-2000, -2000);

                    GhostManager.context.pendingBitBufferGhosts.Add(ghost);
                    Main.SpecialCode2 = "10119";
                }
                else
                {
                    Main.SpecialCode2 = "10118";
                    ghost = GhostManager.context.MakeGhost(t, index, false);
                    Main.SpecialCode2 = "20050";
                    t.position = new Vec2(-2000, -2000);

                    Main.SpecialCode2 = "20060";
                    if (NetworkConnection.context != null) ghost.ClearStateMask(NetworkConnection.context);
                    Main.SpecialCode2 = "20070";
                    t.level = Level.current;
                    Main.SpecialCode2 = "20080";
                    t.isBitBufferCreatedGhostThing = true;
                    Main.SpecialCode2 = "10120";
                }
                t.connection = NetworkConnection.context;
            }

            Main.SpecialCode2 = "10200";
            return ghost.thing;
        }


        public object ReadBits(Type t, int bits)
        {
            if (bits == -1)
                return Read(t);

            int val = ReadPackedBits(bits);
            return ConvertType(val, t);
        }

        public T ReadBits<T>(int bits)
        {
            if (bits < 1)
                return default(T);

            int val = ReadPackedBits(bits);
            return (T)ConvertType(val, typeof(T));
        }

        protected object ConvertType(int obj, Type type)
        {
            if (type == typeof(float))
                return (float)obj;
            else if (type == typeof(double))
                return (double)obj;
            else if (type == typeof(byte))
                return (byte)obj;
            else if (type == typeof(sbyte))
                return (sbyte)obj;
            else if (type == typeof(short))
                return (short)obj;
            else if (type == typeof(ushort))
                return (ushort)obj;
            else if (type == typeof(int))
                return (int)obj;
            else if (type == typeof(uint))
                return (uint)obj;
            else if (type == typeof(long))
                return (long)obj;
            else if (type == typeof(ulong))
                return (ulong)obj;
            else if (type == typeof(char))
                return (char)obj;
            else
                throw (new Exception("unrecognized conversion type " + type));
        }

        public T Read<T>()
        {
            return (T)Read(typeof(T));
        }

        public void AlignToByte()
        {
            if (bitOffset > 0)
            {
                position += 1;
                bitOffset = 0;
            }
        }

        public void WriteBufferData(BitBuffer val)
        {
            if (!val.isPacked && !isPacked)
            {
                if (position + val.lengthInBytes > _buffer.Length)
                    resize(position + val.lengthInBytes);

                for (int i = 0; i < val.lengthInBytes; i++)
                {
                    _buffer[position] = val.buffer[i];
                    position += 1;
                }
            }
            else
                WritePacked(val.buffer, val.lengthInBits);
        }

        public void Write(BitBuffer val, bool writeLength = true)
        {
            if (writeLength)
            {
                int size = val.allowPacking ? val.lengthInBits : val.lengthInBytes;
                if (size > ushort.MaxValue - 1)
                {
                    Write(ushort.MaxValue);
                    Write(size);
                }
                else
                    Write((ushort)size);
            }

            WriteBufferData(val);
        }

        public void Write(byte[] val, bool writeLength)
        {
            if (writeLength)
                Write(val.Length);

            Write(val, 0, val.Length);
        }

        public void Write(byte[] data, int offset = 0, int length = -1)
        {
            if (!isPacked || bitOffset == 0)
            {
                if (length < 0)
                    length = data.Length;

                if (position + length > _buffer.Length)
                    resize(position + length);

                Array.Copy(data, offset, buffer, position, length);
                position += length;

                //            for (int i = 0; i < length; i++)
                //{
                //                _buffer[position] = data[offset + i];
                //	position += 1;
                //}
            }
            else
                WritePacked(data);
        }

        public void Write(string val)
        {
            if (TokenSerializer.instance != null)
                WriteTokenizedString(val);
            else
            {
                byte[] stringData = System.Text.Encoding.UTF8.GetBytes(val);
                if (bitOffset != 0)
                {
                    Write((ushort)stringData.Count());
                    WritePacked(stringData);
                }
                else
                {
                    int len = stringData.Count();
                    if (len > ushort.MaxValue)
                    {
                        Write(ushort.MaxValue);
                        Write((ushort)42252);
                        Write(len);
                    }
                    else
                        Write((ushort)stringData.Count());

                    int size = stringData.Count();
                    if (position + size > _buffer.Count())
                        resize(position + size);
                    stringData.CopyTo(_buffer, position);
                    position += size;
                }
            }
        }


        public void Write(long val)
        {
            if (bitOffset != 0)
            {
                unsafe
                {
                    ulong* pi = (ulong*)&val;
                    WritePacked(pi[0], sizeof(ulong) * 8);
                }
            }
            else
            {
                byte size = sizeof(ulong);
                if (_offsetPosition + size > _buffer.Length)
                    resize(_offsetPosition + size);

                unsafe
                {
                    fixed (byte* pRawData = _buffer)
                    {
                        byte* pi = (byte*)&val;
                        pRawData[_offsetPosition] = pi[0];
                        pRawData[_offsetPosition + 1] = pi[1];
                        pRawData[_offsetPosition + 2] = pi[2];
                        pRawData[_offsetPosition + 3] = pi[3];
                        pRawData[_offsetPosition + 4] = pi[4];
                        pRawData[_offsetPosition + 5] = pi[5];
                        pRawData[_offsetPosition + 6] = pi[6];
                        pRawData[_offsetPosition + 7] = pi[7];
                        position += size;
                    }
                }
            }
        }

        public void Write(ulong val)
        {
            if (bitOffset != 0)
                WritePacked(val, sizeof(ulong) * 8);
            else
            {
                byte size = sizeof(ulong);
                if (_offsetPosition + size > _buffer.Length)
                    resize(_offsetPosition + size);

                unsafe
                {
                    fixed (byte* pRawData = _buffer)
                    {
                        byte* pi = (byte*)&val;
                        pRawData[_offsetPosition] = pi[0];
                        pRawData[_offsetPosition + 1] = pi[1];
                        pRawData[_offsetPosition + 2] = pi[2];
                        pRawData[_offsetPosition + 3] = pi[3];
                        pRawData[_offsetPosition + 4] = pi[4];
                        pRawData[_offsetPosition + 5] = pi[5];
                        pRawData[_offsetPosition + 6] = pi[6];
                        pRawData[_offsetPosition + 7] = pi[7];
                        position += size;
                    }
                }
            }
        }

        public void Write(int val)
        {
            if (bitOffset != 0)
            {
                unsafe
                {
                    ulong* pi = (ulong*)&val;
                    WritePacked(pi[0], sizeof(int) * 8);
                }
            }
            else
            {
                byte size = sizeof(uint);
                if (_offsetPosition + size > _buffer.Length)
                    resize(_offsetPosition + size);

                unsafe
                {
                    fixed (byte* pRawData = _buffer)
                    {
                        byte* pi = (byte*)&val;
                        pRawData[_offsetPosition] = pi[0];
                        pRawData[_offsetPosition + 1] = pi[1];
                        pRawData[_offsetPosition + 2] = pi[2];
                        pRawData[_offsetPosition + 3] = pi[3];
                        position += size;
                    }
                }
            }
        }

        public void Write(uint val)
        {
            if (bitOffset != 0)
            {
                unsafe
                {
                    ulong* pi = (ulong*)&val;
                    WritePacked(pi[0], sizeof(uint) * 8);
                }
            }
            else
            {
                byte size = sizeof(uint);
                if (_offsetPosition + size > _buffer.Length)
                    resize(_offsetPosition + size);

                unsafe
                {
                    fixed (byte* pRawData = _buffer)
                    {
                        byte* pi = (byte*)&val;
                        pRawData[_offsetPosition] = pi[0];
                        pRawData[_offsetPosition + 1] = pi[1];
                        pRawData[_offsetPosition + 2] = pi[2];
                        pRawData[_offsetPosition + 3] = pi[3];
                        position += size;
                    }
                }
            }
        }

        public void Write(short val)
        {
            if (bitOffset != 0)
            {
                unsafe
                {
                    ulong* pi = (ulong*)&val;
                    WritePacked(pi[0], sizeof(short) * 8);
                }
            }
            else
            {
                byte size = sizeof(short);
                if (_offsetPosition + size > _buffer.Length)
                    resize(_offsetPosition + size);

                unsafe
                {
                    fixed (byte* pRawData = _buffer)
                    {
                        byte* pi = (byte*)&val;
                        pRawData[_offsetPosition] = pi[0];
                        pRawData[_offsetPosition + 1] = pi[1];
                        position += size;
                    }
                }
            }
        }

        public void Write(ushort val)
        {
            if (bitOffset != 0)
            {
                unsafe
                {
                    ulong* pi = (ulong*)&val;
                    WritePacked(pi[0], sizeof(ushort) * 8);
                }
            }
            else
            {
                byte size = sizeof(ushort);
                if (_offsetPosition + size > _buffer.Length)
                    resize(_offsetPosition + size);

                unsafe
                {
                    fixed (byte* pRawData = _buffer)
                    {
                        byte* pi = (byte*)&val;
                        pRawData[_offsetPosition] = pi[0];
                        pRawData[_offsetPosition + 1] = pi[1];
                        position += size;
                    }
                }
            }
        }


        public void Write(float val)
        {
            if (bitOffset != 0)
            {
                unsafe
                {
                    uint* pi = (uint*)&val;
                    WritePacked(pi[0], sizeof(float) * 8);
                }
            }
            else
            {
                byte size = sizeof(float);
                if (_offsetPosition + size > _buffer.Length)
                    resize(_offsetPosition + size);

                unsafe
                {
                    fixed (byte* pRawData = _buffer)
                    {
                        byte* pi = (byte*)&val;
                        pRawData[_offsetPosition] = pi[0];
                        pRawData[_offsetPosition + 1] = pi[1];
                        pRawData[_offsetPosition + 2] = pi[2];
                        pRawData[_offsetPosition + 3] = pi[3];
                        position += size;
                    }
                }
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
            if (bitOffset != 0)
            {
                unsafe
                {
                    ulong* pi = (ulong*)&val;
                    WritePacked(pi[0], sizeof(double) * 8);
                }
            }
            else
            {
                byte size = sizeof(double);
                if (_offsetPosition + size > _buffer.Length)
                    resize(_offsetPosition + size);

                unsafe
                {
                    fixed (byte* pRawData = _buffer)
                    {
                        byte* pi = (byte*)&val;
                        pRawData[_offsetPosition] = pi[0];
                        pRawData[_offsetPosition + 1] = pi[1];
                        pRawData[_offsetPosition + 2] = pi[2];
                        pRawData[_offsetPosition + 3] = pi[3];
                        pRawData[_offsetPosition + 4] = pi[4];
                        pRawData[_offsetPosition + 5] = pi[5];
                        pRawData[_offsetPosition + 6] = pi[6];
                        pRawData[_offsetPosition + 7] = pi[7];
                        position += size;
                    }
                }
            }
        }

        public void Write(char val)
        {
            if (bitOffset != 0)
                WritePacked((ulong)val, sizeof(char) * 8);
            else
            {
                byte size = sizeof(char);
                if (_offsetPosition + size > _buffer.Length)
                    resize(_offsetPosition + size);

                unsafe
                {
                    fixed (byte* pRawData = _buffer)
                    {
                        byte* pi = (byte*)&val;
                        pRawData[_offsetPosition] = pi[0];
                        pRawData[_offsetPosition + 1] = pi[1];
                        position += size;
                    }
                }
            }
        }

        public void Write(byte val)
        {
            if (bitOffset != 0)
                WritePacked(val, 8);
            else
            {
                if (position + 1 > _buffer.Count())
                    resize(position + 1);

                _buffer[position] = val;
                position += 1;
            }
        }

        public void Write(sbyte val)
        {
            if (bitOffset != 0)
            {
                unsafe
                {
                    ulong* pi = (ulong*)&val;
                    WritePacked(pi[0], sizeof(sbyte) * 8);
                }
            }
            else
            {
                if (position + 1 > _buffer.Count())
                    resize(position + 1);

                _buffer[position] = (byte)val;
                position += 1;
            }
        }


        public void Write(bool val)
        {
            if (_allowPacking)
                WritePacked(val ? (uint)1 : 0, 1);
            else
                Write((byte)(val ? 1 : 0));
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
            sbyte idx = ReadSByte();
            Profile p = null;
            if (idx >= 0 && idx < DuckNetwork.profiles.Count)
                p = DuckNetwork.profiles[idx];

            return p;
        }

        public void WriteTeam(Team pValue)
        {
            int teamSel = -1;
            if (pValue != null)
                teamSel = Teams.IndexOf(pValue);

            Write((ushort)teamSel);
        }

        public Team ReadTeam()
        {
            return Teams.ParseFromIndex(ReadUShort());
        }

        public static List<Type> kTypeIndexList = new List<Type>()
        {
            typeof(string),
            typeof(byte[]),
            typeof(BitBuffer),
            typeof(float),
            typeof(double),
            typeof(byte),
            typeof(sbyte),
            typeof(bool),
            typeof(short),
            typeof(ushort),
            typeof(int),
            typeof(uint),
            typeof(long),
            typeof(ulong),
            typeof(char),
            typeof(Vec2),
            typeof(Color),
            typeof(NetIndex16),
            typeof(NetIndex2),
            typeof(NetIndex4),
            typeof(NetIndex8),
            typeof(Thing)
        };

        public void WriteObject(object obj)
        {
            int idx = 255;
            if (obj != null)
            {
                if (obj is Thing)
                    idx = kTypeIndexList.IndexOf(typeof(Thing));
                else
                    idx = kTypeIndexList.IndexOf(obj.GetType());
            }
            if (idx < 0)
                throw (new Exception("Trying to write unsupported type to BitBuffer through WriteObject!"));

            Write((byte)idx);
            Write(obj);
        }

        public object ReadObject(out Type pTypeRead)
        {
            byte typeByte = ReadByte();
            if (typeByte == 255 || typeByte >= kTypeIndexList.Count)
            {
                pTypeRead = typeof(Thing);
                return null;
            }

            pTypeRead = kTypeIndexList[typeByte];
            return Read(pTypeRead);
        }

        public void Write(object obj)
        {
            if (obj is string)
                Write((string)obj);
            else if (obj is byte[])
                Write((byte[])obj, true);
            else if (obj is BitBuffer)
                Write(obj as BitBuffer);
            else if (obj is float)
                Write((float)obj);
            else if (obj is double)
                Write((double)obj);
            else if (obj is byte)
                Write((byte)obj);
            else if (obj is sbyte)
                Write((sbyte)obj);
            else if (obj is bool)
                Write((bool)obj);
            else if (obj is short)
                Write((short)obj);
            else if (obj is ushort)
                Write((ushort)obj);
            else if (obj is int)
                Write((int)obj);
            else if (obj is uint)
                Write((uint)obj);
            else if (obj is long)
                Write((long)obj);
            else if (obj is ulong)
                Write((ulong)obj);
            else if (obj is char)
                Write((char)obj);
            else if (obj is Vec2)
                Write((Vec2)obj);
            else if (obj is Color)
                Write((Color)obj);
            else if (obj is NetIndex16)
                Write((ushort)(int)((NetIndex16)obj));
            else if (obj is NetIndex2)
                WritePacked((int)((NetIndex2)obj), 2);
            else if (obj is NetIndex4)
                WritePacked((int)((NetIndex4)obj), 4);
            else if (obj is NetIndex8)
                WritePacked((int)((NetIndex8)obj), 8);
            else if (obj is Thing)
            {
                if (((obj as Thing).isStateObject == false && (obj as Thing).specialSyncIndex == 0) || (obj as Thing).level == null)
                {
                    if ((obj as Thing).level != null && MonoMain.modDebugging)
                    {
                        DevConsole.Log(DCSection.NetCore, "@error |DGRED|!!BitBuffer.Write() - " + obj.GetType().Name + " is not a State Object (isStateObject == false), it has no StateBindings and cannot be written to a Bitbuffer.");
                        DevConsole.Log(DCSection.NetCore, "@error |DGRED|!!Are you sending a NetMessage with a non GhostObject member variable?");
                    }

                    //Fail gracefully this time
                    Write((object)null);
                }
                else
                {
                    Write((obj as Thing).level.networkIndex);
                    if ((obj as Thing).isStateObject)
                    {
                        WritePacked((int)Editor.IDToType[(obj as Thing).GetType()], 10);
                        GhostObject g = GhostManager.context.MakeGhostLater(obj as Thing);
                        Write((ushort)(int)(g.ghostObjectIndex));

                        if (g.thing.connection == null)
                            g.thing.connection = DuckNetwork.localConnection;
                    }
                    else
                    {
                        WritePacked((int)0, 10);
                        Write((ushort)(int)((obj as Thing).specialSyncIndex));
                        GhostManager.context.MapSpecialSync((obj as Thing), (obj as Thing).specialSyncIndex);
                    }
                }
            }
            else if (obj == null)
            {
                Write(DuckNetwork.levelIndex);
                WritePacked((ushort)0, 10);
                Write((ushort)0);
                //Write((byte)0);
            }
            else
                throw (new Exception("Trying to write unsupported type " + obj.GetType() + " to BitBuffer!"));
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
            //Always double size
            int reqBytes = _buffer.Count() * 2;
            while (reqBytes < bytes)
                reqBytes *= 2;

            byte[] newBytes = new byte[reqBytes];
            _buffer.CopyTo(newBytes, 0);
            _buffer = newBytes;
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
