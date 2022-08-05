// Decompiled with JetBrains decompiler
// Type: DuckGame.NMKillDuck
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    [ClientOnly]
    public class NMSetScale : NMEvent
    {
        public NMSetScale(Thing thing, Vec2 vec)
        {
            t = thing;
            v = vec;
        }
        public NMSetScale()
        {
        }
        public Thing t;
        public Vec2 v;

        public override void Activate()
        {
            t.scale = v;
        }
        protected override void OnSerialize()
        {
            base.OnSerialize();
        }

        public override void OnDeserialize(BitBuffer d)
        {
            base.OnDeserialize(d);
        }
    }
}
