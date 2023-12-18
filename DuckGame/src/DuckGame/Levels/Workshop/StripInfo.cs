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
