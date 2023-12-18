using System.Collections.Generic;

namespace DuckGame
{
    public class EventManager
    {
        private NetworkConnection _connection;
        private StreamManager _manager;
        private Dictionary<Thing, GhostObject> _ghosts = new Dictionary<Thing, GhostObject>();

        public EventManager(NetworkConnection connection, StreamManager streamManager)
        {
            _connection = connection;
            _manager = streamManager;
        }

        public void OnMessage(NetMessage m)
        {
            switch (m)
            {
                case NMEvent _:
                    (m as NMEvent).Activate();
                    break;
                case NMSynchronizedEvent _:
                    (m as NMSynchronizedEvent).Activate();
                    break;
                case NMConditionalEvent _:
                    (m as NMConditionalEvent).Activate();
                    break;
            }
            Level.current.OnMessage(m);
        }

        public void Update()
        {
        }
    }
}
