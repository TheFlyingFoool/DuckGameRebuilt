// Decompiled with JetBrains decompiler
// Type: DuckGame.PlacementMenu
//removed for regex reasons Culture=neutral, PublicKeyToken=null
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
            _alwaysDrawLast = true;
            x = xpos;
            y = ypos;
            _root = true;
            willOnlineGrayout = false;
            _noneMenu = new ContextMenu(this)
            {
                text = "None"
            };
            AddItem(_noneMenu);
            fancy = true;
            isPinnable = true;
            InitializeGroups(Editor.Placeables, setPinnable: isPinnable);
            _searchMenu = new ContextSearch(this);
            AddItem(_searchMenu);
            AddItem(new ContextToolbarItem(this));
        }

        public override void Selected(ContextMenu item)
        {
            if (item == _noneMenu && item.scrollButtonDirection == 0)
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
