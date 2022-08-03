// Decompiled with JetBrains decompiler
// Type: DuckGame.NetSoundBinding
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class NetSoundBinding : StateBinding
    {
        public override System.Type type => typeof(byte);

        public override object classValue
        {
            get => byteValue;
            set => byteValue = (byte)value;
        }

        public override byte byteValue
        {
            get => (byte)(_accessor.getAccessor(_thing) as NetSoundEffect).index;
            set => (_accessor.getAccessor(_thing) as NetSoundEffect).index = value;
        }

        public NetSoundBinding(string field)
          : base(field, 2)
        {
        }

        public NetSoundBinding(GhostPriority p, string field)
          : base(field, 2)
        {
            _priority = p;
        }
    }
}
