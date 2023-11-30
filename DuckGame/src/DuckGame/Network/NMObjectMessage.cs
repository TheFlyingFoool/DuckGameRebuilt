namespace DuckGame
{
    public class NMObjectMessage : NetMessage
    {
        public ushort objectID;

        public NMObjectMessage(ushort id) => objectID = id;

        public NMObjectMessage()
        {
        }
    }
}
