namespace DuckGame
{
    /// <summary>
    /// A piece of an image file. A <see cref="chunkCount"/> of 0 means the sender doesn't have the image anymore.
    /// </summary>
    [ClientOnly]
    public class NMChatImageChunk : NMDuckNetworkEvent, INetworkChunk
    {
        public uint imageId;
        public ushort chunkIndex;
        public ushort chunkCount;
        public int totalBytes;
        private BitBuffer _data;

        public BitBuffer data => _data;

        /// <summary>Sender side, called when the other end acknowledged this chunk. Not sent over the network.</summary>
        internal System.Action onAcknowledged;

        public NMChatImageChunk()
        {
        }

        public NMChatImageChunk(uint pImageId, ushort pChunkIndex, ushort pChunkCount, int pTotalBytes, BitBuffer pData)
        {
            imageId = pImageId;
            chunkIndex = pChunkIndex;
            chunkCount = pChunkCount;
            totalBytes = pTotalBytes;
            _data = pData;
        }

        protected override void OnSerialize()
        {
            if (_serializedData == null)
                _serializedData = new BitBuffer();
            _serializedData.Write(imageId);
            _serializedData.Write(chunkIndex);
            _serializedData.Write(chunkCount);
            _serializedData.Write(totalBytes);
            _serializedData.Write(_data ?? new BitBuffer(), true);
        }

        public override void OnDeserialize(BitBuffer msg)
        {
            imageId = msg.ReadUInt();
            chunkIndex = msg.ReadUShort();
            chunkCount = msg.ReadUShort();
            totalBytes = msg.ReadInt();
            _data = msg.ReadBitBuffer();
        }

        public override void MessageWasReceived() => onAcknowledged?.Invoke();

        public override void Activate() => ChatImages.OnChunk(connection, this);
    }
}
