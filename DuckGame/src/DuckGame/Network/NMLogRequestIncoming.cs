namespace DuckGame
{
    public class NMLogRequestIncoming : NMEvent
    {
        public int numChunks;

        public NMLogRequestIncoming(int pNumChunks) => numChunks = pNumChunks;

        public NMLogRequestIncoming()
        {
        }

        public override void Activate()
        {
            if (!DevConsole.core.requestingLogs.Contains(connection))
                return;
            connection.logTransferSize = numChunks;
            connection.logTransferProgress = 0;
        }
    }
}
