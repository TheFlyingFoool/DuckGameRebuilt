// Decompiled with JetBrains decompiler
// Type: DuckGame.ContextObject
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
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

        public Thing thing => this._thing;

        public ContextObject(Thing thing, IContextListener owner, bool placement = true)
          : base(owner)
        {
            this._placement = placement;
            this._thing = thing;
            this._image = thing.GeneratePreview(transparentBack: true);
            this.itemSize.y = 16f;
            this._text = thing.editorName;
            this.itemSize.x = Graphics.GetFancyStringWidth(this._text) + 26f;
            this._thingBag = ContentProperties.GetBag(thing.GetType());
            //if (Main.isDemo && !this._thingBag.GetOrDefault("isInDemo", false))
            //    this.greyOut = true;
            //else
            this.greyOut = false;
            if (this._thingBag.GetOrDefault("previewPriority", false))
                this._previewPriority = true;
            this.tooltip = thing.editorTooltip;
            if (!this._thingBag.GetOrDefault("isOnlineCapable", true))
                this.tooltip = "(OFFLINE ONLY) " + this.tooltip;
            int placementCost = Editor.CalculatePlacementCost(thing);
            bool flag = false;
            if (placementCost > 0 && Editor.placementLimit > 0)
            {
                this.tooltip = "(" + placementCost.ToString() + " @EDITORCURRENCY@) " + this.tooltip;
                flag = true;
            }
            if (this.tooltip == null)
                this.tooltip = "";
            if (!(this.tooltip != "" | flag))
                return;
            this.tooltip = thing.editorName + ": " + this.tooltip;
        }

        public override void Selected()
        {
            bool flag = false;
            if (this._framesSinceSelected < 20 || Editor.inputMode != EditorInput.Touch)
                flag = true;
            this._framesSinceSelected = 0;
            if (this.scrollButtonDirection != 0)
            {
                this._owner.Selected(this);
            }
            else
            {
                //if (Main.isDemo && !this._thingBag.GetOrDefault("isInDemo", false))
                //    return;
                if (this._placement)
                {
                    if (!(Level.current is Editor current))
                        return;
                    current.placementType = this._thing;
                    if (flag)
                        current.CloseMenu();
                    if (this._thing.forceEditorGrid != 0)
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
                    if (this._owner == null)
                        return;
                    this._owner.Selected(this);
                }
            }
        }

        public override void Draw()
        {
            ++this._framesSinceSelected;
            if (this._hover && !this.greyOut)
                Graphics.DrawRect(this.position, this.position + this.itemSize, new Color(70, 70, 70), this.depth + 1);
            if (this.scrollButtonDirection != 0)
            {
                this._arrow.depth = this.depth + 2;
                if (this.scrollButtonDirection > 0)
                {
                    this._arrow.flipV = true;
                    Graphics.Draw(this._arrow, this.position.x + (this._owner as ContextMenu).menuSize.x / 2f, this.position.y + 8f);
                }
                else
                {
                    this._arrow.flipV = false;
                    Graphics.Draw(this._arrow, this.position.x + (this._owner as ContextMenu).menuSize.x / 2f, this.position.y + 8f);
                }
            }
            else
            {
                Color color = Color.White;
                if (this.greyOut)
                    color = Color.White * 0.3f;
                Graphics.DrawFancyString(this._text, this.position + new Vec2(22f, 4f), color, this.depth + 2);
                this._image.depth = this.depth + 3;
                this._image.x = this.x + 1f;
                this._image.y = this.y;
                this._image.color = color;
                this._image.scale = new Vec2(1f);
                this._image.Draw();
            }
        }
    }
}
