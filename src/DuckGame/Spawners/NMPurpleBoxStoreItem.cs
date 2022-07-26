// Decompiled with JetBrains decompiler
// Type: DuckGame.NMPurpleBoxStoreItem
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class NMPurpleBoxStoreItem : NMEvent
    {
        public Duck duck;
        public PhysicsObject thing;

        public NMPurpleBoxStoreItem()
        {
        }

        public NMPurpleBoxStoreItem(Duck pDuck, PhysicsObject pThing)
        {
            this.duck = pDuck;
            this.thing = pThing;
        }

        public override void Activate()
        {
            if (this.duck != null && this.thing != null && this.duck.profile != null)
                PurpleBlock.StoreItem(this.duck.profile, (Thing)this.thing);
            base.Activate();
        }
    }
}
