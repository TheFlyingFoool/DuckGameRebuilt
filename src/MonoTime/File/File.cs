// Decompiled with JetBrains decompiler
// Type: DuckGame.BinaryFile
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.IO;

namespace DuckGame
{
    public class BinaryFile
    {
        private Stream _stream;
        private byte[] _readShort = new byte[2];
        private byte[] _readInt = new byte[4];

        public BinaryFile(string name, BinaryFileMode m) => _stream = new FileStream(name, (FileMode)m);

        public BinaryFile(byte[] data) => _stream = new MemoryStream(data);

        public void Close() => _stream.Close();

        public void SkipBytes(int bytes) => _stream.Seek(bytes, SeekOrigin.Current);

        public void ResetPosition() => _stream.Seek(0L, SeekOrigin.Begin);

        public byte ReadByte() => (byte)_stream.ReadByte();

        public byte[] ReadBytes(int num)
        {
            byte[] buffer = new byte[num];
            _stream.Read(buffer, 0, num);
            return buffer;
        }

        public short ReadShort()
        {
            _stream.Read(_readShort, 0, 2);
            return BitConverter.ToInt16(_readShort, 0);
        }

        public int ReadInt()
        {
            _stream.Read(_readInt, 0, 4);
            return BitConverter.ToInt32(_readInt, 0);
        }

        public void WriteByte(byte b) => _stream.WriteByte(b);

        public void WriteBytes(byte[] bytes, int length) => _stream.Write(bytes, 0, length);

        public void WriteUShort(ushort b)
        {
            _readShort = BitConverter.GetBytes(b);
            foreach (byte num in _readShort)
                _stream.WriteByte(num);
        }

        public void WriteInt(int b)
        {
            _readInt = BitConverter.GetBytes(b);
            foreach (byte num in _readInt)
                _stream.WriteByte(num);
        }
    }
}
