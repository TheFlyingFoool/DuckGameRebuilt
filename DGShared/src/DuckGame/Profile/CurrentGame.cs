// Decompiled with JetBrains decompiler
// Type: DuckGame.CurrentGame
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class CurrentGame
    {
        private int _kills;
        private int _cash;

        public int kills
        {
            get => _kills;
            set => _kills = value;
        }

        public int cash
        {
            get => _cash;
            set => _cash = value;
        }
    }
}
