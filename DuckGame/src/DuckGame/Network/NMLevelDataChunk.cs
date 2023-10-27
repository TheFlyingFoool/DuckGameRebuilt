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
