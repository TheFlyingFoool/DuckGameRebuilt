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
