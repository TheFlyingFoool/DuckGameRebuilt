// Decompiled with JetBrains decompiler
// Type: DuckGame.NMRequestLogs
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class NMRequestLogs : NMEvent
    {
        public override void Activate()
        {
            if (this.connection.profile == null)
                return;
            DevConsole.Log("@error@" + this.connection.ToString() + " is requesting your Netlog!", Color.Red);
            DevConsole.Log("@error@Only accept this request if it's from someone you trust!", Color.Red);
            DevConsole.Log("@error@type |WHITE|accept " + this.connection.profile.networkIndex.ToString() + " |RED|to begin transfer.", Color.Red);
            DevConsole.core.transferRequestsPending.Add(this.connection);
        }
    }
}
