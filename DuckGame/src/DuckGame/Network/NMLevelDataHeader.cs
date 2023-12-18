namespace DuckGame
{
    public class NMLevelDataHeader : NMDuckNetwork
    {
        public ushort transferSession;
        public int length;
        public string levelName;

        public NMLevelDataHeader(ushort tSession, int dataLength, string pName)
        {
            transferSession = tSession;
            length = dataLength;
            levelName = pName;
        }

        public NMLevelDataHeader()
        {
        }

        public override void MessageWasReceived() => connection.dataTransferSize = length;
    }
}
