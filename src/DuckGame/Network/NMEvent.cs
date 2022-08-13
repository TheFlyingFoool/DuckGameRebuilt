// Decompiled with JetBrains decompiler
// Type: DuckGame.NMEvent
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class NMEvent : NetMessage
    {
        public NMEvent() => manager = BelongsToManager.EventManager;

        public virtual void Activate()
        {
        }
    }
}
