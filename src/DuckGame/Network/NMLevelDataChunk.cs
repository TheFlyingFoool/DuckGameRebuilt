// Decompiled with JetBrains decompiler
// Type: DuckGame.NMLevelDataChunk
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class NMLevelDataChunk : NMDuckNetwork, INetworkChunk
    {
        public ushort transferSession;
        private BitBuffer _buffer;

        public BitBuffer GetBuffer() => _buffer;

        public NMLevelDataChunk(ushort tSession, BitBuffer dat)
        {
            transferSession = tSession;
            _buffer = dat;
        }

        public NMLevelDataChunk()
        {
        }

        public override void MessageWasReceived() => connection.dataTransferProgress += _buffer.lengthInBytes;

        protected override void OnSerialize()
        {
            serializedData.Write(_buffer, true);
            base.OnSerialize();
        }

        public override void OnDeserialize(BitBuffer msg)
        {
            _buffer = msg.ReadBitBuffer();
            base.OnDeserialize(msg);
        }
    }
}
