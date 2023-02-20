// Decompiled with JetBrains decompiler
// Type: DuckGame.NMPurpleBoxServed
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class NMPurpleBoxServed : NMEvent
    {
        public Duck duck;
        public PurpleBlock block;

        public NMPurpleBoxServed()
        {
        }

        public NMPurpleBoxServed(Duck pDuck, PurpleBlock pBlock)
        {
            duck = pDuck;
            block = pBlock;
        }

        public override void Activate()
        {
            if (duck != null && duck.profile != null && block != null && !block._served.Contains(duck.profile))
                block._served.Add(duck.profile);
            base.Activate();
        }
    }
}
