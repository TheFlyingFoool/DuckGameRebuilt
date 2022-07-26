// Decompiled with JetBrains decompiler
// Type: DuckGame.NMPurpleBoxServed
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
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
            this.duck = pDuck;
            this.block = pBlock;
        }

        public override void Activate()
        {
            if (this.duck != null && this.duck.profile != null && this.block != null && !this.block._served.Contains(this.duck.profile))
                this.block._served.Add(this.duck.profile);
            base.Activate();
        }
    }
}
