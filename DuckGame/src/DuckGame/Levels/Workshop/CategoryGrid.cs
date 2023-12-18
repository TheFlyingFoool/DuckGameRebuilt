using System.Collections.Generic;

namespace DuckGame
{
    public class CategoryGrid : Thing
    {
        private List<Card> _cards = new List<Card>();
        private List<CardStrip> _strips = new List<CardStrip>();
        private IPageListener _listener;
        private int _selectedStripIndex;

        public CategoryGrid(float xpos, float ypos, List<Card> cards, IPageListener listener)
          : base(xpos, ypos)
        {
            _cards = cards;
            _listener = listener;
        }

        public void AddStrip(List<Card> infos)
        {
            List<Card> cards = new List<Card>();
            cards.AddRange(infos);
            CardStrip cardStrip = new CardStrip(x, y, cards, _listener, true);
            _strips.Add(cardStrip);
            Level.Add(cardStrip);
        }

        public void AddStrip(StripInfo infos)
        {
            List<Card> cards = new List<Card>();
            cards.AddRange(infos.cards);
            CardStrip cardStrip = new CardStrip(x, y, cards, _listener, infos.large, infos.cardsVisible, infos.header);
            _strips.Add(cardStrip);
            Level.Add(cardStrip);
        }

        public override void Initialize()
        {
            if (_cards != null)
            {
                List<Card> infos = new List<Card>();
                foreach (Card card in _cards)
                {
                    infos.Add(card);
                    if (infos.Count == 3)
                    {
                        AddStrip(infos);
                        infos.Clear();
                    }
                }
                if (infos.Count > 0)
                    AddStrip(infos);
            }
            base.Initialize();
        }

        public override void Update()
        {
            if (InputProfile.active.Pressed(Triggers.MenuUp))
                --_selectedStripIndex;
            else if (InputProfile.active.Pressed(Triggers.MenuDown))
                ++_selectedStripIndex;
            if (_selectedStripIndex < 0)
            {
                _selectedStripIndex = 0;
            }
            else
            {
                if (_selectedStripIndex < _strips.Count)
                    return;
                _selectedStripIndex = _strips.Count - 1;
            }
        }

        public override void Draw()
        {
            float y = this.y;
            int num = 0;
            foreach (CardStrip strip in _strips)
            {
                strip.y = y;
                y += strip.height + 4f;
                strip.selected = num == _selectedStripIndex;
                ++num;
            }
        }
    }
}
