// Decompiled with JetBrains decompiler
// Type: DuckGame.StateFlagBase
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public abstract class StateFlagBase : StateBinding
    {
        public ushort _value;

        public override System.Type type => typeof(ushort);

        public override object classValue
        {
            get => ushortValue;
            set => ushortValue = (ushort)value;
        }

        public abstract override ushort ushortValue { get; set; }

        public StateFlagBase(GhostPriority p, int bits)
          : base("multiple", bits)
        {
            _priority = p;
        }

        public override void Connect(Thing t) => _thing = t;
    }
}
