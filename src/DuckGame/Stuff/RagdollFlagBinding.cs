// Decompiled with JetBrains decompiler
// Type: DuckGame.RagdollFlagBinding
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class RagdollFlagBinding : StateFlagBase
    {
        public override ushort ushortValue
        {
            get
            {
                _value = 0;
                Ragdoll thing = _thing as Ragdoll;
                if (thing.inSleepingBag)
                    _value |= 16;
                if (thing.solid)
                    _value |= 8;
                if (thing.enablePhysics)
                    _value |= 4;
                if (thing.active)
                    _value |= 2;
                if (thing.visible)
                    _value |= 1;
                return _value;
            }
            set
            {
                _value = value;
                Ragdoll thing = _thing as Ragdoll;
                thing.inSleepingBag = (_value & 16U) > 0U;
                thing.solid = (_value & 8U) > 0U;
                thing.enablePhysics = (_value & 4U) > 0U;
                thing.active = (_value & 2U) > 0U;
                thing.visible = (_value & 1U) > 0U;
            }
        }

        public RagdollFlagBinding(GhostPriority p = GhostPriority.Normal)
          : base(p, 5)
        {
        }
    }
}
