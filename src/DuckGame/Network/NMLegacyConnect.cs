// Decompiled with JetBrains decompiler
// Type: DuckGame.NMLegacyConnect
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    [FixedNetworkID(9)]
    public class NMLegacyConnect : NMNetworkCoreMessage
    {
        public byte connectsReceived;
        public NetIndex4 remoteSession;

        public NMLegacyConnect()
        {
        }

        public NMLegacyConnect(byte received, NetIndex4 s)
        {
            connectsReceived = received;
            remoteSession = s;
        }
    }
}
