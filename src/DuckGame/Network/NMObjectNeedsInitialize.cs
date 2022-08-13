// Decompiled with JetBrains decompiler
// Type: DuckGame.NMObjectNeedsInitialize
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class NMObjectNeedsInitialize : NMDuckNetworkEvent
    {
        public Thing thing;

        public NMObjectNeedsInitialize(Thing t) => thing = t;

        public NMObjectNeedsInitialize()
        {
        }

        public override void Activate()
        {
            if (thing == null || thing.ghostObject == null)
                return;
            if (thing.connection == connection)
                Thing.Fondle(thing, DuckNetwork.localConnection);
            if (!thing.isServerForObject)
                return;
            thing.ghostObject.DirtyStateMask(long.MaxValue, connection);
        }
    }
}
