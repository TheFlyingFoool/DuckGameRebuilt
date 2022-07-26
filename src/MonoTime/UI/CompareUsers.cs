// Decompiled with JetBrains decompiler
// Type: DuckGame.CompareUsers
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System.Collections.Generic;

namespace DuckGame
{
    public class CompareUsers : IComparer<UIInviteUser>
    {
        public int Compare(UIInviteUser h1, UIInviteUser h2)
        {
            if (h1 == h2)
                return 0;
            int num = 0;
            if (h1.inDuckGame)
                num -= 4;
            else if (h1.inGame)
                ++num;
            if (h2.inDuckGame)
                num += 4;
            else if (h2.inGame)
                --num;
            if (h1.inMyLobby)
                --num;
            if (h2.inMyLobby)
                ++num;
            if (h1.triedInvite)
                num -= 8;
            if (h2.triedInvite)
                num += 8;
            if (h1.state == SteamUserState.Online)
                num -= 2;
            if (h2.state == SteamUserState.Online)
                num += 2;
            return num;
        }
    }
}
