// Decompiled with JetBrains decompiler
// Type: DuckGame.NMNetworkTrafficGarbage
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

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
