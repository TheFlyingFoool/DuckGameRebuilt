// Decompiled with JetBrains decompiler
// Type: DuckGame.NMConsoleMessage
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class NMConsoleMessage : NetMessage
    {
        public string message;

        public NMConsoleMessage()
        {
        }

        public NMConsoleMessage(string msg) => message = msg;
    }
}
