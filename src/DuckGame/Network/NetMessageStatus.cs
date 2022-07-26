// Decompiled with JetBrains decompiler
// Type: DuckGame.NetMessageStatus
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class NetMessageStatus
    {
        public int timesResent;
        public int timesDropped;
        public int framesSinceSent;
        public uint tickOnSend;

        public void Clear()
        {
            this.timesResent = 0;
            this.timesDropped = 0;
            this.framesSinceSent = 0;
            this.tickOnSend = 0U;
        }
    }
}
