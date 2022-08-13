// Decompiled with JetBrains decompiler
// Type: DuckGame.NMKillDuck
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    [ClientOnly]
    public class ScaleNMTest : NMEvent
    {

        public ScaleNMTest()
        {
        }

        public override void Activate()
        {
            if (Level.current == null)
            {
                return;
            }
            foreach(Thing T in Level.current.things)
            {
                T.scale = new Vec2(0.5f, 0.5f);
            }
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
