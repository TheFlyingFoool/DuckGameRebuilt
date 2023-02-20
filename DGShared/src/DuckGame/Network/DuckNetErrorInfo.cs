// Decompiled with JetBrains decompiler
// Type: DuckGame.DuckNetErrorInfo
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class DuckNetErrorInfo
    {
        public bool critical;
        public string message;
        public Profile user;
        public DuckNetError error;

        public DuckNetErrorInfo()
        {
        }

        public DuckNetErrorInfo(DuckNetError e, string msg)
        {
            message = msg;
            error = e;
        }
    }
}
