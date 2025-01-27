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

        public Thing thing => _thing;

        public ContextBackgroundTile(Thing thing, IContextListener owner, bool placement = true)
          : base(owner)
        {
            _placement = placement;
            _thing = thing;
            _image = thing.GetEditorImage();
            itemSize.x = 180f;
            itemSize.y = 16f;
            _text = thing.editorName;
            itemSize.x = _text.Length * 8 + 16;
            _canExpand = true;
            depth = (Depth)0.8f;
            if (_thing is CustomBackground)
                _file = new ContextFile("LOAD FILE...", this, new FieldBinding(_thing, "customBackground0" + ((thing as CustomBackground).customIndex + 1).ToString()), ContextFileType.Background);
            IReadOnlyPropertyBag bag = ContentProperties.GetBag(thing.GetType());
            //if (!Main.isDemo || bag.GetOrDefault("isInDemo", false))
            //    return;
            //this.greyOut = true;
        }

        public override bool HasOpen() => opened;

        public override void Selected()
        {
            if (greyOut)
                return;
            SFX.Play("highClick", 0.3f, 0.2f);
            if (_owner == null)
                return;
            _owner.Selected(this);
        }

        public override void Update()
        {
            base.Update();
            if (opened && position.y + menuSize.y + _openedOffset > 350)
                _openedOffset = 350 - position.y - menuSize.y;
        }

        public override void Draw()
        {
            if (!_root)
            {
                float num = 1f;
                if (greyOut)
                    num = 0.3f;
                if (_hover && !greyOut)
                    Graphics.DrawRect(position, position + itemSize, new Color(70, 70, 70), (Depth)0.82f);
                Graphics.DrawFancyString(_text, position + new Vec2(2f, 4f), Color.White * num, (Depth)0.85f);
                _contextArrow.color = Color.White * num;
                Graphics.Draw(_contextArrow, (x + itemSize.x - 11f), y + 3f, (Depth)0.85f);
            }
            if (opened)
            {
                SpriteMap graphic = _thing.graphic as SpriteMap;
                int num1 = graphic.texture.width / graphic.w;
                int num2 = graphic.texture.height / graphic.h;
                if (justOpened)
                {
                    tooltip = _text;
                    int placementCost = Editor.CalculatePlacementCost(_thing);
                    if (placementCost > 0)
                        tooltip = tooltip + ": (" + placementCost.ToString() + " @EDITORCURRENCY@)";
                    _hoverPos = new Vec2(_selectedIndex % num1 * graphic.w, _selectedIndex / num1 * graphic.h);
                    if (Editor.inputMode == EditorInput.Mouse && positionCursor)
                    {
                        _rememberedMousePosition = Mouse.position;
                        Mouse.position = _hoverPos + position + new Vec2(8f, 8f);
                        positionCursor = false;
                    }
                }
                menuSize = new Vec2(graphic.texture.width + 2, graphic.texture.height + 2);
                float x = menuSize.x;
                float y = menuSize.y;
                Vec2 p1 = new Vec2(this.x, this.y + _openedOffset);
                if (Editor.inputMode != EditorInput.Mouse && !_root)
                    p1.y = 16f;
                if (!_root)
                {
                    p1.x += itemSize.x + 4f;
                    p1.y -= 2f;
                }
                //Vec2 vec2_1 = new Vec2(graphic.position); WHAT -NIK0
                _thing.x = (float)(p1.x + 1f + graphic.w / 2f);
                _thing.y = (float)(p1.y + 1f + graphic.h / 2f);
                _thing.depth = (Depth)0.7f;
                Graphics.DrawRect(p1, p1 + new Vec2(x, y), new Color(70, 70, 70), (Depth)0.5f);
                Graphics.DrawRect(p1 + new Vec2(1f, 1f), p1 + new Vec2(x - 1f, y - 1f), new Color(30, 30, 30), (Depth)0.6f);
                _lastDrawPos = p1;
                Graphics.Draw(graphic.texture, new Vec2(_thing.x, _thing.y), new Rectangle?(), Color.White, 0f, _thing.center, _thing.scale, SpriteEffects.None, (Depth)0.7f);
                if (_root && _file != null)
                {
                    Vec2 vec2_2 = new Vec2(p1 + new Vec2(x + 4f, 0f));
                    //Vec2 vec2_3 = new Vec2(p1 + new Vec2(x + 97f, 12f)); what -NiK0
                    _file.position = vec2_2;
                    _file.Update();
                    _file.Draw();
                }
                if (Editor.inputMode == EditorInput.Touch && (_file == null || !_file.hover))
                {
                    Vec2 vec2_4 = new Vec2(-1f, -1f);
                    if (TouchScreen.GetTap() != Touch.None)
                    {
                        Vec2 vec2_5 = TouchScreen.GetTap().Transform(layer.camera);
                        _hoverPos = new Vec2(vec2_5.x - _thing.x, vec2_5.y - _thing.y);
                    }
                }
                else if (Editor.inputMode == EditorInput.Gamepad && (_file == null || !_file.hover) && !Editor.clickedMenu)
                {
                    _hoverPos = new Vec2(_selectedIndex % num1 * graphic.w, _selectedIndex / num1 * graphic.h);
                    if (Input.Pressed(Triggers.MenuLeft) && MonoMain.UpdateLerpState)
                    {
                        if (_selectedIndex == 0 && _owner != null)
                        {
                            Selected(null);
                            opened = false;
                        }
                        else
                            --_selectedIndex;
                    }
                    if (Input.Pressed(Triggers.MenuRight) && MonoMain.UpdateLerpState)
                    {
                        if (_file != null && _selectedIndex == num1 - 1)
                            _file.hover = true;
                        else
                            ++_selectedIndex;
                    }
                    if (Input.Pressed(Triggers.MenuUp) && MonoMain.UpdateLerpState)
                        _selectedIndex -= num1;
                    if (Input.Pressed(Triggers.MenuDown) && MonoMain.UpdateLerpState)
                        _selectedIndex += num1;
                    if (_selectedIndex < 0)
                        _selectedIndex = 0;
                    if (_selectedIndex > num1 * num2 - 1)
                        _selectedIndex = num1 * num2 - 1;
                }
                else if (Editor.inputMode == EditorInput.Mouse)
                    _hoverPos = new Vec2(Mouse.x - _thing.x, Mouse.y - _thing.y);
                if (_file != null && _file.hover && Input.Pressed(Triggers.MenuLeft) && MonoMain.UpdateLerpState)
                {
                    _file.hover = false;
                    _selectedIndex = num1 - 1;
                }
                Editor current = Level.current as Editor;
                _hoverPos.x = (float)Math.Round(_hoverPos.x / graphic.w) * graphic.w;
                _hoverPos.y = (float)Math.Round(_hoverPos.y / graphic.h) * graphic.h;
                if ((_file == null || !_file.hover) && _hoverPos.x >= 0f && _hoverPos.x < graphic.texture.width && _hoverPos.y >= 0f && _hoverPos.y < graphic.texture.height)
                {
                    Graphics.DrawRect(_hoverPos + p1, _hoverPos + p1 + new Vec2(graphic.w + 2, graphic.h + 2), Color.Lime * 0.8f, (Depth)0.8f, false);
                    if (Editor.inputMode == EditorInput.Mouse && Mouse.left == InputState.Pressed || Editor.inputMode == EditorInput.Gamepad && Input.Pressed(Triggers.Select) && MonoMain.UpdateLerpState && !justOpened || Editor.inputMode == EditorInput.Touch && TouchScreen.GetTap() != Touch.None)
                    {
                        if (_thing is BackgroundTile)
                            (_thing as BackgroundTile).frame = (int)(_hoverPos.x / graphic.w + _hoverPos.y / graphic.h * (graphic.texture.width / graphic.w));
                        else
                            graphic.frame = (int)(_hoverPos.x / graphic.w + _hoverPos.y / graphic.h * (graphic.texture.width / graphic.w));
                        current.placementType = _thing;
                        current.placementType = _thing;
                        if (!floatMode || Editor.inputMode == EditorInput.Gamepad)
                        {
                            Disappear();
                            current.CloseMenu();
                        }
                    }
                }
                if (!justOpened && Input.Pressed(Triggers.Menu1) && owner == null && MonoMain.UpdateLerpState)
                {
                    Disappear();
                    current.CloseMenu();
                }
                justOpened = false;
            }
            else
            {
                tooltip = "";
                justOpened = true;
            }
        }

        public override void Disappear()
        {
            if (_rememberedMousePosition != Vec2.Zero)
            {
                Mouse.position = _rememberedMousePosition;
                _rememberedMousePosition = Vec2.Zero;
            }
            base.Disappear();
        }
    }
}
