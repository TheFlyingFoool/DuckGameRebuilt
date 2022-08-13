// Decompiled with JetBrains decompiler
// Type: DuckGame.StripInfo
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System.Collections.Generic;

namespace DuckGame
{
    public struct StripInfo
    {
        public string header;
        public bool large;
        public int cardsVisible;
        public List<Card> cards;

        public StripInfo(bool l)
        {
            header = null;
            large = l;
            cardsVisible = 3;
            cards = new List<Card>();
        }
    }
}
