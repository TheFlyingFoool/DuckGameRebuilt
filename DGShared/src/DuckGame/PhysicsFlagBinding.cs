// Decompiled with JetBrains decompiler
// Type: DuckGame.PhysicsFlagBinding
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class PhysicsFlagBinding : StateFlagBase
    {
        public override ushort ushortValue
        {
            get
            {
                _value = 0;
                PhysicsObject thing = _thing as PhysicsObject;
                if (thing.solid)
                    _value |= 128;
                if (thing.enablePhysics)
                    _value |= 16;
                if (thing.active)
                    _value |= 8;
                if (thing.visible)
                    _value |= 4;
                if (thing.grounded)
                    _value |= 64;
                if (thing.onFire)
                    _value |= 32;
                if (thing._destroyed)
                    _value |= 2;
                if (thing.isSpawned)
                    _value |= 1;
                return _value;
            }
            set
            {
                _value = value;
                PhysicsObject thing = _thing as PhysicsObject;
                thing.solid = (_value & 128U) > 0U;
                thing.enablePhysics = (_value & 16U) > 0U;
                thing.active = (_value & 8U) > 0U;
                thing.visible = (_value & 4U) > 0U;
                thing.grounded = (_value & 64U) > 0U;
                thing.onFire = (_value & 32U) > 0U;
                thing._destroyed = (_value & 2U) > 0U;
                thing.isSpawned = (_value & 1U) > 0U;
            }
        }

        public PhysicsFlagBinding(GhostPriority p)
          : base(p, 8)
        {
        }
    }
}
