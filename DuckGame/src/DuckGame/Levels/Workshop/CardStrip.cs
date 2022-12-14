// Decompiled with JetBrains decompiler
// Type: DuckGame.CardStrip
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections.Generic;
using System.Linq;

namespace DuckGame
{
    public class CardStrip : Thing
    {
        private int _levelIndex;
        private int _selectedCardIndex;
        private float _indexSlide;
        private List<Card> _cards = new List<Card>();
        private Sprite _arrow;
        private Card _cardSelected;
        private IPageListener _listener;
        private int _numCardsPerScreen = 3;
        private int _maxCardsPerStrip = 5;
        private bool _large;
        private string _heading;
        private bool _selected;
        private static BitmapFont _font = new BitmapFont("biosFont", 8);

        public Card cardSelected => _cardSelected;

        public bool selected
        {
            get => _selected;
            set
            {
                if (_selected == value)
                    return;
                _selected = value;
                _selectedCardIndex = _levelIndex;
            }
        }

        public CardStrip(
          float xpos,
          float ypos,
          List<Card> cards,
          IPageListener listener,
          bool largeCard,
          int cardsPerScreen = 3,
          string heading = null)
          : base(xpos, ypos)
        {
            if (cards.Count > _maxCardsPerStrip)
            {
                _cards = cards.GetRange(0, 5);
                if (cards[0] is LevelInfo)
                {
                    LevelInfo card = _cards[0] as LevelInfo;
                    List<Card> cards1 = _cards;
                    LevelInfo levelInfo = new LevelInfo
                    {
                        specialText = "VIEW ALL",
                        large = card.large
                    };
                    cards1.Add(levelInfo);
                }
            }
            else
                _cards = cards;
            _listener = listener;
            _large = largeCard;
            _numCardsPerScreen = cardsPerScreen;
            _heading = heading;
            if (cards == null || cards.Count <= 0)
                return;
            float height = cards[0].height;
            if (heading != null && heading != "")
                height += 10f;
            collisionSize = new Vec2(_numCardsPerScreen * (cards[0].width + 4f), height);
        }

        public override void Initialize()
        {
            layer = Layer.HUD;
            _arrow = new Sprite("levelBrowserArrow");
            _arrow.CenterOrigin();
        }

        public override void Update()
        {
            if (_selected)
            {
                if (InputProfile.active.Pressed("MENULEFT"))
                    --_selectedCardIndex;
                else if (InputProfile.active.Pressed("MENURIGHT"))
                    ++_selectedCardIndex;
                else if (InputProfile.active.Pressed("SELECT"))
                    _listener.CardSelected(_cards[_selectedCardIndex]);
            }
            if (_selectedCardIndex >= _cards.Count())
                _selectedCardIndex = _cards.Count() - 1;
            else if (_selectedCardIndex < 0)
                _selectedCardIndex = 0;
            if (_levelIndex + (_numCardsPerScreen - 1) < _selectedCardIndex)
            {
                if (_indexSlide > -1.0)
                    _indexSlide = Lerp.FloatSmooth(_indexSlide, -1.2f, 0.2f);
                if (_indexSlide <= -1.0)
                {
                    ++_levelIndex;
                    _indexSlide = 0f;
                }
            }
            if (_levelIndex <= _selectedCardIndex)
                return;
            if (_indexSlide < 1.0)
                _indexSlide = Lerp.FloatSmooth(_indexSlide, 1.2f, 0.2f);
            if (_indexSlide < 1.0)
                return;
            --_levelIndex;
            _indexSlide = 0f;
        }

        public override void Draw()
        {
            float y = this.y;
            if (_heading != null && _heading != "")
            {
                _font.scale = new Vec2(0.75f, 0.75f);
                _font.Draw(_heading, x + 4f, this.y, Color.White, (Depth)0.95f);
                y += 10f;
            }
            Vec2 vec2_1 = Vec2.Zero;
            Vec2 vec2_2 = Vec2.Zero;
            if (_cards.Count > 0)
            {
                vec2_2 = new Vec2(_cards[0].width, _cards[0].height);
                vec2_1 = new Vec2((x - (vec2_2.x + 4f) + _indexSlide * (vec2_2.x + 4f)), y);
            }
            int num1 = 0;
            for (int index = _levelIndex - 1; index < _levelIndex + (_numCardsPerScreen + 1); ++index)
            {
                if (index >= 0 && index < _cards.Count)
                {
                    Card card = _cards[index];
                    float num2 = 1f;
                    if (num1 == _numCardsPerScreen + 1)
                        num2 = Math.Abs(_indexSlide);
                    else if (num1 == _numCardsPerScreen && _indexSlide > 0f)
                    {
                        num2 = 1f - Math.Abs(_indexSlide);
                    }
                    else
                    {
                        switch (num1)
                        {
                            case 0:
                                num2 = Math.Abs(_indexSlide);
                                break;
                            case 1:
                                if (_indexSlide < 0f)
                                {
                                    num2 = 1f - Math.Abs(_indexSlide);
                                    break;
                                }
                                break;
                        }
                    }
                    Vec2 position = vec2_1;
                    int num3 = !_selected ? 0 : (index == _selectedCardIndex ? 1 : 0);
                    double alpha = num2;
                    card.Draw(position, num3 != 0, (float)alpha);
                }
                vec2_1.x += vec2_2.x + 4f;
                ++num1;
            }
            _arrow.xscale = _arrow.yscale = 0.25f;
            _arrow.depth = (Depth)0.98f;
            if (_levelIndex + _numCardsPerScreen < _cards.Count)
            {
                _arrow.flipH = false;
                Graphics.Draw(_arrow, 312f, y + vec2_2.y / 2f);
            }
            if (_levelIndex <= 0)
                return;
            _arrow.flipH = true;
            Graphics.Draw(_arrow, 8f, y + vec2_2.y / 2f);
        }
    }
}
