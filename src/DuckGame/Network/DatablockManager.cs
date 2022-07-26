// Decompiled with JetBrains decompiler
// Type: DuckGame.DatablockManager
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class DatablockManager
    {
        private NetworkConnection _connection;
        private StreamManager _manager;

        public DatablockManager(NetworkConnection connection, StreamManager streamManager)
        {
            this._connection = connection;
            this._manager = streamManager;
        }

        public void OnMessage(NetMessage m)
        {
        }

        public void Update()
        {
        }

        public static void BuildLevelInitializerBlock()
        {
        }
    }
}
