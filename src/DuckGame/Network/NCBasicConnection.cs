// Decompiled with JetBrains decompiler
// Type: DuckGame.NCBasicConnection
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

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
