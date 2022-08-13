// Decompiled with JetBrains decompiler
// Type: DuckGame.NMRequestLogs
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class NMRequestLogs : NMEvent
    {
        public override void Activate()
        {
            if (connection.profile == null)
                return;
            DevConsole.Log("@error@" + connection.ToString() + " is requesting your Netlog!", Color.Red);
            DevConsole.Log("@error@Only accept this request if it's from someone you trust!", Color.Red);
            DevConsole.Log("@error@type |WHITE|accept " + connection.profile.networkIndex.ToString() + " |RED|to begin transfer.", Color.Red);
            DevConsole.core.transferRequestsPending.Add(connection);
        }
    }
}
