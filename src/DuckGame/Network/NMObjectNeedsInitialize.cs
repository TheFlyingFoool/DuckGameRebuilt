// Decompiled with JetBrains decompiler
// Type: DuckGame.NMObjectNeedsInitialize
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class NMObjectNeedsInitialize : NMDuckNetworkEvent
    {
        public Thing thing;

        public NMObjectNeedsInitialize(Thing t) => this.thing = t;

        public NMObjectNeedsInitialize()
        {
        }

        public override void Activate()
        {
            if (this.thing == null || this.thing.ghostObject == null)
                return;
            if (this.thing.connection == this.connection)
                Thing.Fondle(this.thing, DuckNetwork.localConnection);
            if (!this.thing.isServerForObject)
                return;
            this.thing.ghostObject.DirtyStateMask(long.MaxValue, this.connection);
        }
    }
}
