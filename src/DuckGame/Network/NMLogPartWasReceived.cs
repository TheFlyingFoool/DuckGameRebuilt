// Decompiled with JetBrains decompiler
// Type: DuckGame.NMLogPartWasReceived
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class NMLogPartWasReceived : NMEvent
    {
        public override void Activate()
        {
            ++DuckNetwork.core.logTransferProgress;
            if (DuckNetwork.core.logTransferProgress != DuckNetwork.core.logTransferSize)
                return;
            DevConsole.LogSendingComplete(connection);
        }
    }
}
