using System.Collections.Generic;

namespace DuckGame
{
    public class UploadPage : Page, IPageListener
    {
        private BitmapFont _font;
        private List<Card> _cards = new List<Card>();
        private Card _pageToOpen;
        private Thing _strip;
        private bool _grid;

        public UploadPage(List<Card> cards, bool grid)
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
            current = new MainPage(_cards, true);
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

        public override void PostDrawLayer(Layer layer)
        {
            if (layer != Layer.HUD)
                return;
            _font.xscale = _font.yscale = 1f;
            _font.Draw("Upload", 8f, 8f, Color.White, (Depth)0.95f);
        }
    }
}
