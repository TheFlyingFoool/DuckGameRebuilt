namespace DuckGame
{
    public class NMConsoleMessage : NetMessage
    {
        public string message;

        public NMConsoleMessage()
        {
        }

        public NMConsoleMessage(string msg) => message = msg;
    }
}
