// Decompiled with JetBrains decompiler
// Type: DuckGame.PlacementMenu
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class PlacementMenu : EditorGroupMenu
    {
        private ContextMenu _noneMenu;
        private ContextMenu _searchMenu;

        public PlacementMenu(float xpos, float ypos)
          : base(null)
        {
            this._alwaysDrawLast = true;
            this.x = xpos;
            this.y = ypos;
            this._root = true;
            this.willOnlineGrayout = false;
            this._noneMenu = new ContextMenu(this)
            {
                text = "None"
            };
            this.AddItem(this._noneMenu);
            this.fancy = true;
            this.isPinnable = true;
            this.InitializeGroups(Editor.Placeables, setPinnable: this.isPinnable);
            this._searchMenu = new ContextSearch(this);
            this.AddItem(this._searchMenu);
            this.AddItem(new ContextToolbarItem(this));
        }

        public override void Selected(ContextMenu item)
        {
            if (item == this._noneMenu && item.scrollButtonDirection == 0)
            {
                if (!(Level.current is Editor current))
                    return;
                current.placementType = null;
                current.CloseMenu();
            }
            else
                base.Selected(item);
        }

        public override void Initialize()
        {
        }
    }
}
