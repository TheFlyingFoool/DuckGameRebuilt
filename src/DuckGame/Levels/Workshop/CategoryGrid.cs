// Decompiled with JetBrains decompiler
// Type: DuckGame.CategoryGrid
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

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
            this._cards = cards;
            this._listener = listener;
        }

        public void AddStrip(List<Card> infos)
        {
            List<Card> cards = new List<Card>();
            cards.AddRange((IEnumerable<Card>)infos);
            CardStrip cardStrip = new CardStrip(this.x, this.y, cards, this._listener, true);
            this._strips.Add(cardStrip);
            Level.Add((Thing)cardStrip);
        }

        public void AddStrip(StripInfo infos)
        {
            List<Card> cards = new List<Card>();
            cards.AddRange((IEnumerable<Card>)infos.cards);
            CardStrip cardStrip = new CardStrip(this.x, this.y, cards, this._listener, infos.large, infos.cardsVisible, infos.header);
            this._strips.Add(cardStrip);
            Level.Add((Thing)cardStrip);
        }

        public override void Initialize()
        {
            if (this._cards != null)
            {
                List<Card> infos = new List<Card>();
                foreach (Card card in this._cards)
                {
                    infos.Add(card);
                    if (infos.Count == 3)
                    {
                        this.AddStrip(infos);
                        infos.Clear();
                    }
                }
                if (infos.Count > 0)
                    this.AddStrip(infos);
            }
            base.Initialize();
        }

        public override void Update()
        {
            if (InputProfile.active.Pressed("MENUUP"))
                --this._selectedStripIndex;
            else if (InputProfile.active.Pressed("MENUDOWN"))
                ++this._selectedStripIndex;
            if (this._selectedStripIndex < 0)
            {
                this._selectedStripIndex = 0;
            }
            else
            {
                if (this._selectedStripIndex < this._strips.Count)
                    return;
                this._selectedStripIndex = this._strips.Count - 1;
            }
        }

        public override void Draw()
        {
            float y = this.y;
            int num = 0;
            foreach (CardStrip strip in this._strips)
            {
                strip.y = y;
                y += strip.height + 4f;
                strip.selected = num == this._selectedStripIndex;
                ++num;
            }
        }
    }
}
