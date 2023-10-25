using System.Collections.Generic;

namespace DuckGame
{
    public class MainPage : Page, IPageListener
    {
        private BitmapFont _font;
        private List<Card> _cards = new List<Card>();
        private Card _pageToOpen;
        private Thing _strip;
        private bool _grid;

        public MainPage(List<Card> cards, bool grid)
        {
            _grid = grid;
            _cards = cards;
        }

        public override void DeactivateAll() => _strip.active = false;

        public override void ActivateAll() => _strip.active = true;

        public override void TransitionOutComplete()
        {
            if (!(_pageToOpen.specialText == "Upload Thing"))
                return;
            current = new UploadPage(_cards, true);
        }

        public void CardSelected(Card card)
        {
            _state = CategoryState.OpenPage;
            _pageToOpen = card;
        }

        public override void Initialize()
        {
            Layer.HUD.camera.x = camOffset;
            backgroundColor = new Color(8, 12, 13);
            _font = new BitmapFont("biosFont", 8);
            HUD.AddCornerControl(HUDCorner.BottomLeft, "@SELECT@SELECT");
            HUD.AddCornerControl(HUDCorner.BottomRight, "@CANCEL@BACK");
            CategoryGrid categoryGrid = new CategoryGrid(12f, 20f, null, this);
            Add(categoryGrid);
            categoryGrid.AddStrip(new StripInfo(false)
            {
                cards = {
          new Card("Level Editor"),
          new Card("Hat Editor"),
          new Card("Weapon Editor"),
          new Card("Sandwich Editor"),
          new Card("Religion Editor")
        },
                header = "Editors",
                cardsVisible = 4
            });
            if (_cards.Count > 4)
                _cards.Insert(4, new LevelInfo(false, "Upload Thing"));
            StripInfo infos = new StripInfo(false);
            infos.cards.AddRange(_cards);
            infos.header = "Your Things";
            infos.cardsVisible = 4;
            categoryGrid.AddStrip(infos);
            categoryGrid.AddStrip(new StripInfo(false)
            {
                cards = {
           new LevelInfo(false, "Not a thing.")
        },
                header = "Browse Things",
                cardsVisible = 4
            });
            _strip = categoryGrid;
            base.Initialize();
        }

        public override void Update()
        {
        }

        public override void PostDrawLayer(Layer layer)
        {
            if (layer != Layer.HUD)
                return;
            _font.xscale = _font.yscale = 1f;
            _font.Draw("Workshop", 8f, 8f, Color.White, (Depth)0.95f);
        }
    }
}
