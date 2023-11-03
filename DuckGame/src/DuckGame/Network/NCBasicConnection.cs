using System.Net;

namespace DuckGame
{
    public class NCBasicConnection
    {
        public IPEndPoint connection;
        public NCBasicStatus status;
        public Timer timeout = new Timer();
        public Timer heartbeat = new Timer();
        public int attempts;
        public int beatsReceived;
        public bool isHost;
        public int packets;

        public NCBasicConnection() => heartbeat.Start();
    }
}
