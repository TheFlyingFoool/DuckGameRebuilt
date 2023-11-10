namespace DuckGame
{
    public class NetMessageStatus
    {
        public int timesResent;
        public int timesDropped;
        public int framesSinceSent;
        public uint tickOnSend;

        public void Clear()
        {
            timesResent = 0;
            timesDropped = 0;
            framesSinceSent = 0;
            tickOnSend = 0U;
        }
    }
}
