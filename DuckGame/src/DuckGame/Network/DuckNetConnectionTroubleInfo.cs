namespace DuckGame
{
    public class DuckNetConnectionTroubleInfo : DuckNetErrorInfo
    {
        public DuckNetConnectionTroubleInfo()
        {
        }

        public DuckNetConnectionTroubleInfo(DuckNetError e, string msg)
        {
            message = msg;
            error = e;
        }
    }
}
