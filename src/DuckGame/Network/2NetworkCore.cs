// Decompiled with JetBrains decompiler
// Type: DuckGame.NMDisconnect
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    [FixedNetworkID(8)]
    public class NMDisconnect : NMNetworkCoreMessage
    {
        public byte error;

        public DuckNetErrorInfo GetError() => new DuckNetErrorInfo((DuckNetError)this.error, ((DuckNetError)this.error).ToString());

        public NMDisconnect(DuckNetError pError) => this.error = (byte)pError;

        public NMDisconnect(byte pError) => this.error = pError;

        public NMDisconnect()
        {
        }
    }
}
