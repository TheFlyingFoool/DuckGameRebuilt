// Decompiled with JetBrains decompiler
// Type: DuckGame.EventManager
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

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
