namespace DuckGame
{
    public class DuckNetErrorInfo
    {
        public bool critical;
        public string message;
        public Profile user;
        public DuckNetError error;

        public DuckNetErrorInfo()
        {
        }

        public DuckNetErrorInfo(DuckNetError e, string msg)
        {
            message = msg;
            error = e;
        }
    }
}
