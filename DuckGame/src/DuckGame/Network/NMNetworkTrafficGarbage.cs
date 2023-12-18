namespace DuckGame
{
    public class NMNetworkTrafficGarbage : NMEvent
    {
        private int _numBytes;

        public NMNetworkTrafficGarbage(int numBytes) => _numBytes = numBytes;

        public NMNetworkTrafficGarbage()
        {
        }

        protected override void OnSerialize()
        {
            if (_numBytes < 4)
                _numBytes = 4;
            _serializedData.Write(_numBytes);
            for (int index = 0; index < _numBytes - 4; ++index)
                _serializedData.Write((byte)Rando.Int(byte.MaxValue));
            base.OnSerialize();
        }

        public override void OnDeserialize(BitBuffer msg)
        {
            _numBytes = msg.ReadInt();
            for (int index = 0; index < _numBytes - 4; ++index)
            {
                int num = msg.ReadByte();
            }
            base.OnDeserialize(msg);
        }
    }
}
