// Decompiled with JetBrains decompiler
// Type: DuckGame.ContextObject
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class ContextObject : ContextMenu
    {
        private IReadOnlyPropertyBag _thingBag;
        private Thing _thing;
        private bool _placement;
        public static int lastForceGrid;
        private int _framesSinceSelected = 999;

        public Thing thing => _thing;

        public ContextObject(Thing thing, IContextListener owner, bool placement = true)
          : base(owner)
        {
            _placement = placement;
            _thing = thing;
            _image = thing.GeneratePreview(transparentBack: true);
            itemSize.y = 16f;
            _text = thing.editorName;
            itemSize.x = Graphics.GetFancyStringWidth(_text) + 26f;
            _thingBag = ContentProperties.GetBag(thing.GetType());
            //if (Main.isDemo && !this._thingBag.GetOrDefault("isInDemo", false))
            //    this.greyOut = true;
            //else
            greyOut = false;
            if (_thingBag.GetOrDefault("previewPriority", false))
                _previewPriority = true;
            tooltip = thing.editorTooltip;
            if (!_thingBag.GetOrDefault("isOnlineCapable", true))
                tooltip = "(OFFLINE ONLY) " + tooltip;
            int placementCost = Editor.CalculatePlacementCost(thing);
            bool flag = false;
            if (placementCost > 0 && Editor.placementLimit > 0)
            {
                tooltip = "(" + placementCost.ToString() + " @EDITORCURRENCY@) " + tooltip;
                flag = true;
            }
            if (tooltip == null)
                tooltip = "";
            if (!(tooltip != "" | flag))
                return;
            tooltip = thing.editorName + ": " + tooltip;
        }

        public override void Selected()
        {
            bool flag = false;
            if (_framesSinceSelected < 20 || Editor.inputMode != EditorInput.Touch)
                flag = true;
            _framesSinceSelected = 0;
            if (scrollButtonDirection != 0)
            {
                _owner.Selected(this);
            }
            else
            {
                //if (Main.isDemo && !this._thingBag.GetOrDefault("isInDemo", false))
                //    return;
                if (_placement)
                {
                    if (!(Level.current is Editor current))
                        return;
                    current.placementType = _thing;
                    if (flag)
                        current.CloseMenu();
                    if (_thing.forceEditorGrid != 0)
                    {
                        current.cellSize = _thing.forceEditorGrid;
                        ContextObject.lastForceGrid = (int)current.cellSize;
                    }
                    else if (ContextObject.lastForceGrid != 0)
                    {
                        ContextObject.lastForceGrid = 0;
                        current.cellSize = 16f;
                    }
                    SFX.Play("lowClick", 0.3f);
                }
                else
                {
                    if (_owner == null)
                        return;
                    _owner.Selected(this);
                }
            }
        }

        public override void Draw()
        {
            ++_framesSinceSelected;
            if (_hover && !greyOut)
                Graphics.DrawRect(position, position + itemSize, new Color(70, 70, 70), depth + 1);
            if (scrollButtonDirection != 0)
            {
                _arrow.depth = depth + 2;
                if (scrollButtonDirection > 0)
                {
                    _arrow.flipV = true;
                    Graphics.Draw(_arrow, position.x + (_owner as ContextMenu).menuSize.x / 2f, position.y + 8f);
                }
                else
                {
                    _arrow.flipV = false;
                    Graphics.Draw(_arrow, position.x + (_owner as ContextMenu).menuSize.x / 2f, position.y + 8f);
                }
            }
            else
            {
                Color color = Color.White;
                if (greyOut)
                    color = Color.White * 0.3f;
                Graphics.DrawFancyString(_text, position + new Vec2(22f, 4f), color, depth + 2);
                _image.depth = depth + 3;
                _image.x = x + 1f;
                _image.y = y;
                _image.color = color;
                _image.scale = new Vec2(1f);
                _image.Draw();
            }
        }
    }
}
