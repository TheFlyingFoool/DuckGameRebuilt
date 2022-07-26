// Decompiled with JetBrains decompiler
// Type: DuckGame.UploadPage
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

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
            this._grid = grid;
            this._cards = cards;
        }

        public override void DeactivateAll() => this._strip.active = false;

        public override void ActivateAll() => this._strip.active = true;

        public override void TransitionOutComplete()
        {
            if (!(this._pageToOpen.specialText == "Upload Thing"))
                return;
            Level.current = (Level)new MainPage(this._cards, true);
        }

        public void CardSelected(Card card)
        {
            this._state = CategoryState.OpenPage;
            this._pageToOpen = card;
        }

        public override void Initialize()
        {
            Layer.HUD.camera.x = Page.camOffset;
            this.backgroundColor = new Color(8, 12, 13);
            this._font = new BitmapFont("biosFont", 8);
            HUD.AddCornerControl(HUDCorner.BottomLeft, "@SELECT@SELECT");
            HUD.AddCornerControl(HUDCorner.BottomRight, "@CANCEL@BACK");
            CategoryGrid categoryGrid = new CategoryGrid(12f, 20f, (List<Card>)null, (IPageListener)this);
            Level.Add((Thing)categoryGrid);
            if (this._cards.Count > 4)
                this._cards.Insert(4, (Card)new LevelInfo(false, "Upload Thing"));
            StripInfo infos = new StripInfo(false);
            infos.cards.AddRange((IEnumerable<Card>)this._cards);
            infos.header = "Your Things";
            infos.cardsVisible = 4;
            categoryGrid.AddStrip(infos);
            categoryGrid.AddStrip(new StripInfo(false)
            {
                cards = {
          (Card) new LevelInfo(false, "Not a thing.")
        },
                header = "Browse Things",
                cardsVisible = 4
            });
            this._strip = (Thing)categoryGrid;
            base.Initialize();
        }

        public override void PostDrawLayer(Layer layer)
        {
            if (layer != Layer.HUD)
                return;
            this._font.xscale = this._font.yscale = 1f;
            this._font.Draw("Upload", 8f, 8f, Color.White, (Depth)0.95f);
        }
    }
}
