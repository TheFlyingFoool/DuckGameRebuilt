// Decompiled with JetBrains decompiler
// Type: DuckGame.ContextBackgroundTile
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using Microsoft.Xna.Framework.Graphics;
using System;

namespace DuckGame
{
    public class ContextBackgroundTile : ContextMenu
    {
        private Thing _thing;
        private new Sprite _image;
        private bool _placement;
        protected Vec2 _hoverPos = Vec2.Zero;
        public bool positionCursor;
        private ContextFile _file;
        private Vec2 _rememberedMousePosition;
        private bool justOpened = true;
        public bool floatMode;

        public Thing thing => this._thing;

        public ContextBackgroundTile(Thing thing, IContextListener owner, bool placement = true)
          : base(owner)
        {
            this._placement = placement;
            this._thing = thing;
            this._image = thing.GetEditorImage();
            this.itemSize.x = 180f;
            this.itemSize.y = 16f;
            this._text = thing.editorName;
            this.itemSize.x = this._text.Length * 8 + 16;
            this._canExpand = true;
            this.depth = (Depth)0.8f;
            if (this._thing is CustomBackground)
                this._file = new ContextFile("LOAD FILE...", this, new FieldBinding(_thing, "customBackground0" + ((thing as CustomBackground).customIndex + 1).ToString()), ContextFileType.Background);
            IReadOnlyPropertyBag bag = ContentProperties.GetBag(thing.GetType());
            //if (!Main.isDemo || bag.GetOrDefault("isInDemo", false))
            //    return;
            //this.greyOut = true;
        }

        public override bool HasOpen() => this.opened;

        public override void Selected()
        {
            if (this.greyOut)
                return;
            SFX.Play("highClick", 0.3f, 0.2f);
            if (this._owner == null)
                return;
            this._owner.Selected(this);
        }

        public override void Closed() => base.Closed();

        public override void Draw()
        {
            if (!this._root)
            {
                float num = 1f;
                if (this.greyOut)
                    num = 0.3f;
                if (this._hover && !this.greyOut)
                    Graphics.DrawRect(this.position, this.position + this.itemSize, new Color(70, 70, 70), (Depth)0.82f);
                Graphics.DrawFancyString(this._text, this.position + new Vec2(2f, 4f), Color.White * num, (Depth)0.85f);
                this._contextArrow.color = Color.White * num;
                Graphics.Draw(this._contextArrow, (this.x + itemSize.x - 11f), this.y + 3f, (Depth)0.85f);
            }
            if (this.opened)
            {
                SpriteMap graphic = this._thing.graphic as SpriteMap;
                int num1 = graphic.texture.width / graphic.w;
                int num2 = graphic.texture.height / graphic.h;
                if (this.justOpened)
                {
                    this.tooltip = this._text;
                    int placementCost = Editor.CalculatePlacementCost(this._thing);
                    if (placementCost > 0)
                        this.tooltip = this.tooltip + ": (" + placementCost.ToString() + " @EDITORCURRENCY@)";
                    this._hoverPos = new Vec2(this._selectedIndex % num1 * graphic.w, this._selectedIndex / num1 * graphic.h);
                    if (Editor.inputMode == EditorInput.Mouse && this.positionCursor)
                    {
                        this._rememberedMousePosition = Mouse.position;
                        Mouse.position = this._hoverPos + this.position + new Vec2(8f, 8f);
                        this.positionCursor = false;
                    }
                }
                this.menuSize = new Vec2(graphic.texture.width + 2, graphic.texture.height + 2);
                float x = this.menuSize.x;
                float y = this.menuSize.y;
                Vec2 p1 = new Vec2(this.x, this.y);
                if (Editor.inputMode != EditorInput.Mouse && !this._root)
                    p1.y = 16f;
                if (!this._root)
                {
                    p1.x += this.itemSize.x + 4f;
                    p1.y -= 2f;
                }
                Vec2 vec2_1 = new Vec2(graphic.position);
                this._thing.x = (float)(p1.x + 1.0 + graphic.w / 2.0);
                this._thing.y = (float)(p1.y + 1.0 + graphic.h / 2.0);
                this._thing.depth = (Depth)0.7f;
                DuckGame.Graphics.DrawRect(p1, p1 + new Vec2(x, y), new Color(70, 70, 70), (Depth)0.5f);
                DuckGame.Graphics.DrawRect(p1 + new Vec2(1f, 1f), p1 + new Vec2(x - 1f, y - 1f), new Color(30, 30, 30), (Depth)0.6f);
                this._lastDrawPos = p1;
                DuckGame.Graphics.Draw(graphic.texture, new Vec2(this._thing.x, this._thing.y), new Rectangle?(), Color.White, 0f, this._thing.center, this._thing.scale, SpriteEffects.None, (Depth)0.7f);
                if (this._root && this._file != null)
                {
                    Vec2 vec2_2 = new Vec2(p1 + new Vec2(x + 4f, 0f));
                    Vec2 vec2_3 = new Vec2(p1 + new Vec2(x + 97f, 12f));
                    this._file.position = vec2_2;
                    this._file.Update();
                    this._file.Draw();
                }
                if (Editor.inputMode == EditorInput.Touch && (this._file == null || !this._file.hover))
                {
                    Vec2 vec2_4 = new Vec2(-1f, -1f);
                    if (TouchScreen.GetTap() != Touch.None)
                    {
                        Vec2 vec2_5 = TouchScreen.GetTap().Transform(this.layer.camera);
                        this._hoverPos = new Vec2(vec2_5.x - this._thing.x, vec2_5.y - this._thing.y);
                    }
                }
                else if (Editor.inputMode == EditorInput.Gamepad && (this._file == null || !this._file.hover) && !Editor.clickedMenu)
                {
                    this._hoverPos = new Vec2(this._selectedIndex % num1 * graphic.w, this._selectedIndex / num1 * graphic.h);
                    if (Input.Pressed("MENULEFT"))
                    {
                        if (this._selectedIndex == 0 && this._owner != null)
                        {
                            this.Selected(null);
                            this.opened = false;
                        }
                        else
                            --this._selectedIndex;
                    }
                    if (Input.Pressed("MENURIGHT"))
                    {
                        if (this._file != null && this._selectedIndex == num1 - 1)
                            this._file.hover = true;
                        else
                            ++this._selectedIndex;
                    }
                    if (Input.Pressed("MENUUP"))
                        this._selectedIndex -= num1;
                    if (Input.Pressed("MENUDOWN"))
                        this._selectedIndex += num1;
                    if (this._selectedIndex < 0)
                        this._selectedIndex = 0;
                    if (this._selectedIndex > num1 * num2 - 1)
                        this._selectedIndex = num1 * num2 - 1;
                }
                else if (Editor.inputMode == EditorInput.Mouse)
                    this._hoverPos = new Vec2(Mouse.x - this._thing.x, Mouse.y - this._thing.y);
                if (this._file != null && this._file.hover && Input.Pressed("MENULEFT"))
                {
                    this._file.hover = false;
                    this._selectedIndex = num1 - 1;
                }
                Editor current = Level.current as Editor;
                this._hoverPos.x = (float)Math.Round(_hoverPos.x / graphic.w) * graphic.w;
                this._hoverPos.y = (float)Math.Round(_hoverPos.y / graphic.h) * graphic.h;
                if ((this._file == null || !this._file.hover) && _hoverPos.x >= 0f && _hoverPos.x < graphic.texture.width && _hoverPos.y >= 0f && _hoverPos.y < graphic.texture.height)
                {
                    DuckGame.Graphics.DrawRect(this._hoverPos + p1, this._hoverPos + p1 + new Vec2(graphic.w + 2, graphic.h + 2), Color.Lime * 0.8f, (Depth)0.8f, false);
                    if (Editor.inputMode == EditorInput.Mouse && Mouse.left == InputState.Pressed || Editor.inputMode == EditorInput.Gamepad && Input.Pressed("SELECT") && !this.justOpened || Editor.inputMode == EditorInput.Touch && TouchScreen.GetTap() != Touch.None)
                    {
                        if (this._thing is BackgroundTile)
                            (this._thing as BackgroundTile).frame = (int)(_hoverPos.x / graphic.w + _hoverPos.y / graphic.h * (graphic.texture.width / graphic.w));
                        else
                            graphic.frame = (int)(_hoverPos.x / graphic.w + _hoverPos.y / graphic.h * (graphic.texture.width / graphic.w));
                        current.placementType = this._thing;
                        current.placementType = this._thing;
                        if (!this.floatMode || Editor.inputMode == EditorInput.Gamepad)
                        {
                            this.Disappear();
                            current.CloseMenu();
                        }
                    }
                }
                if (!this.justOpened && Input.Pressed("MENU1") && this.owner == null)
                {
                    this.Disappear();
                    current.CloseMenu();
                }
                this.justOpened = false;
            }
            else
            {
                this.tooltip = "";
                this.justOpened = true;
            }
        }

        public override void Disappear()
        {
            if (this._rememberedMousePosition != Vec2.Zero)
            {
                Mouse.position = this._rememberedMousePosition;
                this._rememberedMousePosition = Vec2.Zero;
            }
            base.Disappear();
        }
    }
}
