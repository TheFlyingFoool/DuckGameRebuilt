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
