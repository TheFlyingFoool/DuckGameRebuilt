// Decompiled with JetBrains decompiler
// Type: DuckGame.NetSoundsBinding
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class NetSoundsBinding : StateBinding
    {
        private ushort _soundIndexBlock;

        public override System.Type type => typeof(ushort);

        public override object classValue
        {
            get => byteValue;
            set => byteValue = (byte)value;
        }

        public override ushort ushortValue
        {
            get => _soundIndexBlock;
            set => _soundIndexBlock = value;
        }

        public NetSoundsBinding(string field)
          : base(field, 2)
        {
        }

        public NetSoundsBinding(GhostPriority p, string field)
          : base(field, 2)
        {
            _priority = p;
        }
    }
}
