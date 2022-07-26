// Decompiled with JetBrains decompiler
// Type: DuckGame.NMConnect
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    [FixedNetworkID(43)]
    public class NMConnect : NMNetworkCoreMessage
    {
        public string version;
        public NetIndex4 connectsReceived;
        public NetIndex4 remoteSession;
        public string modHash;

        public NMConnect()
        {
        }

        public NMConnect(byte received, NetIndex4 s, string v, string mH)
        {
            this.version = v;
            this.connectsReceived = (NetIndex4)(int)received;
            this.remoteSession = s;
            this.modHash = mH;
        }
    }
}
