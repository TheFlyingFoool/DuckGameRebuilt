// Decompiled with JetBrains decompiler
// Type: DuckGame.CategoryPage
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System.Collections.Generic;

namespace DuckGame
{
    public class CategoryPage : Level, IPageListener
    {
        private CategoryState _state;
        private BitmapFont _font;
        private List<Card> _cards = new List<Card>();
        private Card _pageToOpen;
        private Thing _strip;
        private bool _grid;
        public static float camOffset;

        public CategoryPage(List<Card> cards, bool grid)
        {
            _grid = grid;
            _cards = cards;
        }

        public void CardSelected(Card card)
        {
            _state = CategoryState.OpenPage;
            _pageToOpen = card;
        }

        public override void Initialize()
        {
            Layer.HUD.camera.x = CategoryPage.camOffset;
            backgroundColor = new Color(8, 12, 13);
            _font = new BitmapFont("biosFont", 8);
            HUD.AddCornerControl(HUDCorner.BottomLeft, "@SELECT@SELECT");
            HUD.AddCornerControl(HUDCorner.BottomRight, "@CANCEL@BACK");
            if (_grid)
            {
                _strip = new CategoryGrid(12f, 31f, _cards, this);
                Level.Add(_strip);
            }
            else
            {
                _strip = new CardStrip(12f, 31f, _cards, this, false, 4);
                Level.Add(_strip);
            }
            base.Initialize();
        }

        public override void Update()
        {
            Layer.HUD.camera.x = CategoryPage.camOffset;
            if (_state == CategoryState.OpenPage)
            {
                _strip.active = false;
                CategoryPage.camOffset = Lerp.FloatSmooth(CategoryPage.camOffset, 360f, 0.1f);
                if (camOffset <= 330.0 || !(_pageToOpen.specialText == "VIEW ALL"))
                    return;
                Level.current = new CategoryPage(_cards, true);
            }
            else
            {
                if (_state != CategoryState.Idle)
                    return;
                CategoryPage.camOffset = Lerp.FloatSmooth(CategoryPage.camOffset, -40f, 0.1f);
                if (camOffset < 0.0)
                    CategoryPage.camOffset = 0f;
                _strip.active = camOffset == 0.0;
            }
        }

        public override void PostDrawLayer(Layer layer)
        {
            if (layer != Layer.HUD)
                return;
            _font.xscale = _font.yscale = 1f;
            _font.Draw("CUSTOM LEVELS", 8f, 8f, Color.White, (Depth)0.95f);
            _font.xscale = _font.yscale = 0.75f;
            _font.Draw("BEST NEW LEVELS", 14f, 22f, Color.White, (Depth)0.95f);
        }
    }
}
