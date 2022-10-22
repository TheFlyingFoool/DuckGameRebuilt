// Decompiled with JetBrains decompiler
// Type: DuckGame.NMLogRequestChunk
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class NMLogRequestChunk : NMEvent
    {
        public string data;

        public NMLogRequestChunk(string pData) => data = pData;

        public NMLogRequestChunk()
        {
        }

        public override void Activate()
        {
            if (!DevConsole.core.requestingLogs.Contains(connection))
                return;
            DevConsole.core.ReceiveLogData(data, connection);
            ++connection.logTransferProgress;
            Send.Message(new NMLogPartWasReceived(), connection);
            if (connection.logTransferProgress != connection.logTransferSize)
                return;
            DevConsole.LogTransferComplete(connection);
        }
    }
}
