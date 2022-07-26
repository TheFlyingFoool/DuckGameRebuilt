// Decompiled with JetBrains decompiler
// Type: DuckGame.NMVersionMismatch
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class NMVersionMismatch : NMDuckNetwork
    {
        public byte byteCode;
        public string serverVersion;

        public NMVersionMismatch.Type GetCode() => (NMVersionMismatch.Type)this.byteCode;

        public NMVersionMismatch()
        {
        }

        public NMVersionMismatch(NMVersionMismatch.Type code, string ver)
        {
            this.byteCode = (byte)code;
            this.serverVersion = ver;
        }

        public enum Type
        {
            Match = -1, // 0xFFFFFFFF
            Older = 0,
            Newer = 1,
            Error = 2,
        }
    }
}
