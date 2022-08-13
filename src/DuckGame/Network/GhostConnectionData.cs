// Decompiled with JetBrains decompiler
// Type: DuckGame.GhostConnectionData
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class GhostConnectionData
    {
        private long _connectionStateMask = long.MaxValue;
        public ushort prevInputState;
        public uint latestCommandTickReceived;
        public NetIndex16 lastTickSent = (NetIndex16)1;
        public NetIndex16 lastTickReceived = (NetIndex16)0;
        public NetIndex8 authority = (NetIndex8)1;

        public long connectionStateMask
        {
            get => _connectionStateMask;
            set => _connectionStateMask = value;
        }
    }
}
