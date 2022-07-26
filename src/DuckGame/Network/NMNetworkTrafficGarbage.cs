// Decompiled with JetBrains decompiler
// Type: DuckGame.NMNetworkTrafficGarbage
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class NMNetworkTrafficGarbage : NMEvent
    {
        private int _numBytes;

        public NMNetworkTrafficGarbage(int numBytes) => this._numBytes = numBytes;

        public NMNetworkTrafficGarbage()
        {
        }

        protected override void OnSerialize()
        {
            if (this._numBytes < 4)
                this._numBytes = 4;
            this._serializedData.Write(this._numBytes);
            for (int index = 0; index < this._numBytes - 4; ++index)
                this._serializedData.Write((byte)Rando.Int((int)byte.MaxValue));
            base.OnSerialize();
        }

        public override void OnDeserialize(BitBuffer msg)
        {
            this._numBytes = msg.ReadInt();
            for (int index = 0; index < this._numBytes - 4; ++index)
            {
                int num = (int)msg.ReadByte();
            }
            base.OnDeserialize(msg);
        }
    }
}
