using System;
using System.Collections.Generic;
using System.Linq;

namespace DuckGame
{
    public class ContextMenu : Thing, IContextListener
    {
        public int _previewWidth = 16;
        public Mod mod;
        public string tooltip;
        protected List<ContextMenu> _items = new List<ContextMenu>();
        public Vec2 menuSize;
        protected Sprite _contextArrow;
        public Vec2 itemSize;
        public bool isModRoot;
        public bool isModPath;
        public object zipItem;
        public Thing contextThing;
        protected string _text = "";
        protected string _data = "";
        protected bool _canExpand;
        protected int _selectedIndex;
        protected bool _showBackground = true;
        public bool greyOut;
        public bool drawControls = true;
        public bool disabled;
        private bool _opened;
        public float _openedOffset;
        public float _openedOffsetX;
        protected bool _dragMode;
        public bool _opening;
        private bool _dontPush;
        public bool pinOpened;
        protected bool _hover;
        protected bool _root;
        protected bool _collectionChanged;
        protected IContextListener _owner;
        public Vec2 offset;
        protected Sprite _image;
        protected bool _previewPriority;
        protected Sprite _arrow = new Sprite("tinyUpArrow");
        protected Sprite _pin = new Sprite("pinIcon");
        protected Sprite _pinPinned = new Sprite("pinIconPinned");
        private bool _hasToproot;
        public Vec2 _toprootPosition;
        private bool _hoverBackArrow;
        private bool _pinned;
        public bool isPinnable;
        public bool isToggle;
        protected bool _alwaysDrawLast;
        private bool _takingInput;
        public bool closeOnRight;
        protected Vec2 _lastDrawPos;
        public int _autoSelectItem = -1;
        protected static bool _didContextScroll;
        protected bool _sliding;
        protected bool _canEditSlide;
        protected bool _enteringSlideMode;
        public Sprite customIcon;
        private bool _hoverPin;
        public bool fancy;
        protected int _drawIndex;
        protected int _maxNumToDraw = 9999;
        public int scrollButtonDirection;
        private bool waitInputFrame;

        public string text
        {
            get => _text;
            set => _text = value;
        }

        public string data
        {
            get => _data;
            set => _data = value;
        }

        public int selectedIndex
        {
            get => _selectedIndex;
            set => _selectedIndex = value;
        }

        public bool dontPush
        {
            get => _dontPush;
            set
            {
                if (value)
                {
                    if (!pinned)
                        pinOpened = true;
                    _dontPush = true;
                }
                else
                {
                    _dontPush = false;
                    pinOpened = false;
                }
            }
        }

        protected virtual void OnClose()
        {
        }

        public bool opened
        {
            get => _opened;
            set
            {
                if (!_opened && value)
                {
                    _lastDrawPos = Vec2.Zero;
                    pinOpened = false;
                    foreach (ContextMenu contextMenu in _items)
                    {
                        contextMenu._lastDrawPos = Vec2.Zero;
                        contextMenu.opened = false;
                        contextMenu._hover = false;
                        if (contextMenu.pinned && !Editor.ignorePinning)
                            pinOpened = true;
                    }
                    _openedOffset = 0f;
                    PositionItems();
                    _selectedIndex = 0;
                    if (_items.Count > 0)
                    {
                        while (_selectedIndex < _items.Count - 1 && _items[_selectedIndex].greyOut)
                            ++_selectedIndex;
                        if (!_items[_selectedIndex].greyOut)
                            _opening = true;
                    }
                    else
                        _opening = true;
                    foreach (ContextMenu contextMenu in _items)
                    {
                        contextMenu.dontPush = false;
                        if (contextMenu.pinned && !Editor.ignorePinning)
                        {
                            contextMenu.dontPush = true;
                            contextMenu.opened = true;
                        }
                    }
                    PushLeft();
                }
                if (_opened && !value)
                {
                    foreach (ContextMenu contextMenu in _items)
                    {
                        contextMenu.opened = false;
                        contextMenu._hover = false;
                        contextMenu.OnClose();
                    }
                    Closed();
                    if (_root)
                        Editor.ignorePinning = false;
                }
                if (!value)
                {
                    foreach (ContextMenu contextMenu in _items)
                        contextMenu.ParentCloseAction();
                }
                if (value)
                {
                    if (_autoSelectItem >= 0)
                        _selectedIndex = _autoSelectItem;
                    _autoSelectItem = -1;
                }
                _opened = value;
            }
        }

        public bool hover
        {
            get => _hover;
            set => _hover = value;
        }

        public bool root
        {
            get => _root;
            set => _root = value;
        }

        public virtual void ParentCloseAction()
        {
        }

        public Sprite image
        {
            get => _image;
            set => _image = value;
        }

        public bool pinned
        {
            get
            {
                if (_pinned && Editor.pretendPinned == null || Editor.pretendPinned == this)
                    return true;
                foreach (ContextMenu contextMenu in _items)
                {
                    if (contextMenu.pinned)
                        return true;
                }
                return false;
            }
            set => _pinned = value;
        }

        public ContextMenu(IContextListener owner, SpriteMap img = null, bool hasToproot = false, Vec2 topRoot = default(Vec2))
          : base()
        {
            _owner = owner;
            if (Level.current is Editor)
                layer = Editor.objectMenuLayer;
            else
                layer = Layer.HUD;
            _contextArrow = new Sprite("contextArrowRight");
            itemSize.x = 100f;
            itemSize.y = 16f;
            _root = owner == null;
            _image = img;
            depth = (Depth)0.8f;
            _arrow.CenterOrigin();
            _pin.CenterOrigin();
            _pinPinned.CenterOrigin();
            _toprootPosition = topRoot;
            _hasToproot = hasToproot;
        }

        public override void Initialize()
        {
        }

        public virtual bool HasOpen()
        {
            foreach (ContextMenu contextMenu in _items)
            {
                if (contextMenu.opened)
                    return true;
            }
            return false;
        }

        public virtual void Toggle(ContextMenu item)
        {
            if (_owner == null)
                return;
            isToggle = true;
            _owner.Selected(item);
            isToggle = false;
        }

        public bool IsPartOf(Thing menu)
        {
            if (menu == null || this == menu)
                return true;
            return _owner != null && _owner is ContextMenu owner && owner.IsPartOf(menu);
        }

        public override void DoUpdate()
        {
            if (Editor.clickedMenu)
                return;
            base.DoUpdate();
        }

        public void PushLeft()
        {
            ContextMenu contextMenu1 = null;
            foreach (ContextMenu contextMenu2 in _items)
            {
                if (contextMenu2.opened)
                {
                    contextMenu2.PushLeft();
                    contextMenu1 = contextMenu2;
                }
            }
            if (Keyboard.Down(Keys.Y))
                return;
            Vec2 vec2_1 = new Vec2(x, y);
            Vec2 vec2_2 = new Vec2(0f, 0f);
            if (!_root && !dontPush)
                vec2_2 = new Vec2(itemSize.x + 4f, -2f);
            Vec2 vec2_3 = vec2_1 + vec2_2;
            bool flag = false;
            if (_lastDrawPos.x + menuSize.x + 4f > layer.camera.width)
            {
                if (Editor.bigInterfaceMode)
                    vec2_3.x -= menuSize.x;
                else
                    vec2_3.x = (layer.camera.width - menuSize.x - 4f);
                flag = true;
            }
            if (contextMenu1 != null && contextMenu1.x != vec2_3.x && !pinOpened)
            {
                vec2_3.x = !_root ? (_pinned || Editor.pretendPinned == this ? contextMenu1.x - 4f : contextMenu1.x - 2f) : contextMenu1.x;
                flag = true;
            }
            position = vec2_3 - vec2_2;
            if (!flag)
                return;
            PositionItems();
        }

        private void PinChanged(ContextMenu c)
        {
            if (_owner != null && _owner is ContextMenu)
                (_owner as ContextMenu).PinChanged(c);
            else
                PinChangedDrillDown(c);
        }

        private void PinChangedDrillDown(ContextMenu c)
        {
            if (c != this)
                pinned = false;
            foreach (ContextMenu contextMenu in _items)
                contextMenu.PinChangedDrillDown(c);
        }

        public bool OpenInto(object t, List<ContextMenu> pRecurseStack = null)
        {
            bool flag = true;
            if (pRecurseStack == null)
            {
                pRecurseStack = new List<ContextMenu>();
                flag = false;
            }
            int num = 0;
            foreach (ContextMenu contextMenu1 in _items)
            {
                if (contextMenu1 == t || contextMenu1 is ContextObject && (contextMenu1 as ContextObject).thing.GetType() == t.GetType())
                {
                    _autoSelectItem = num;
                    return true;
                }
                if (contextMenu1 is EditorGroupMenu && contextMenu1.OpenInto(t, pRecurseStack))
                {
                    pRecurseStack.Add(contextMenu1);
                    if (!flag)
                    {
                        ContextMenu contextMenu2 = this;
                        for (int index = pRecurseStack.Count - 1; index >= 0; --index)
                        {
                            contextMenu2.selectedIndex = contextMenu2._items.IndexOf(pRecurseStack[index]);
                            pRecurseStack[index].opened = true;
                            pRecurseStack[index].hover = true;
                            pRecurseStack[index].UpdatePositioning();
                            contextMenu2 = pRecurseStack[index];
                        }
                    }
                    return true;
                }
                ++num;
            }
            return false;
        }

        private void CloseChildren()
        {
            if (!HasOpen() || !_canExpand)
                return;
            bool flag = false;
            foreach (ContextMenu contextMenu in _items)
            {
                if (contextMenu.HasOpen())
                {
                    flag = true;
                    break;
                }
            }
            if (flag)
                return;
            Editor.tookInput = true;
            Selected(null);
        }

        public virtual void Disappear()
        {
        }

        public override void Update()
        {
            if (!visible || disabled || _opening)
            {
                _opening = false;
            }
            else
            {
                if (!IsPartOf(Editor.lockInput))
                    return;
                Vec2 lastDrawPos = _lastDrawPos;
                if (opened)
                {
                    if (Editor.inputMode == EditorInput.Touch && TouchScreen.GetTap().Check(new Rectangle(_lastDrawPos.x, _lastDrawPos.y, _lastDrawPos.x + menuSize.x, _lastDrawPos.y + menuSize.y), layer.camera) && !pinOpened)
                        Editor.clickedContextBackground = true;
                    if (!pinOpened)
                    {
                        Vec2 vec2_1 = new Vec2((float)(lastDrawPos.x + menuSize.x - 5f), lastDrawPos.y - 4f);
                        _hoverPin = false;
                        Vec2 vec2_2 = Vec2.Zero;
                        bool flag1 = false;
                        switch (Editor.inputMode)
                        {
                            case EditorInput.Mouse:
                                vec2_2 = Mouse.position;
                                flag1 = Mouse.left == InputState.Pressed;
                                break;
                            case EditorInput.Touch:
                                vec2_2 = TouchScreen.GetTap().Transform(layer.camera);
                                flag1 = TouchScreen.GetTap() != Touch.None;
                                break;
                        }
                        if (vec2_2.x > vec2_1.x - 5f && vec2_2.x < vec2_1.x + 3f && vec2_2.y > vec2_1.y - 4f && vec2_2.y < vec2_1.y + 4f)
                        {
                            _hoverPin = true;
                            if (flag1 && (!_root || pinned))
                            {
                                if (_root && pinned)
                                {
                                    _pinned = false;
                                    PinChanged(this);
                                    Editor.openPosition = _lastDrawPos;
                                    Editor.reopenContextMenu = true;
                                    Editor.ignorePinning = false;
                                    Editor.clickedMenu = true;
                                    _autoSelectItem = _selectedIndex;
                                }
                                else
                                {
                                    bool pinned = this.pinned;
                                    _pinned = !_pinned;
                                    if (_owner != null && _owner is ContextMenu)
                                        (_owner as ContextMenu).PinChanged(this);
                                    if (_pinned != pinned && Editor.pretendPinned != this)
                                    {
                                        Editor.openPosition = _lastDrawPos;
                                        Editor.reopenContextMenu = true;
                                        Editor.ignorePinning = false;
                                    }
                                    Editor.clickedMenu = true;
                                    _autoSelectItem = _selectedIndex;
                                }
                            }
                        }
                        if (!waitInputFrame && Editor.inputMode == EditorInput.Gamepad && drawControls)
                        {
                            if (Input.Pressed(Triggers.Menu1))
                            {
                                if (isPinnable && _root && pinned)
                                {
                                    _pinned = false;
                                    PinChanged(this);
                                    Editor.openPosition = _lastDrawPos;
                                    Editor.reopenContextMenu = true;
                                    Editor.ignorePinning = false;
                                    _autoSelectItem = _selectedIndex;
                                }
                                else if (isPinnable && owner != null)
                                {
                                    _pinned = !_pinned;
                                    if (_owner != null && _owner is ContextMenu)
                                        (_owner as ContextMenu).PinChanged(this);
                                    Editor.openPosition = _lastDrawPos;
                                    Editor.reopenContextMenu = true;
                                    Editor.ignorePinning = false;
                                    _autoSelectItem = _selectedIndex;
                                }
                            }
                            else if (pinned && _owner != null && _owner is ContextMenu && (_owner as ContextMenu).pinOpened && Input.Pressed(Triggers.MenuLeft))
                            {
                                bool flag2 = false;
                                foreach (ContextMenu contextMenu in _items)
                                {
                                    if (contextMenu.opened)
                                    {
                                        flag2 = true;
                                        break;
                                    }
                                }
                                if (!flag2)
                                {
                                    if (!Editor.bigInterfaceMode || !IsEditorPlacementMenu())
                                    {
                                        Editor.ignorePinning = true;
                                        Editor.reopenContextMenu = true;
                                    }
                                    else
                                    {
                                        Editor.reopenContextMenu = true;
                                        Editor.clickedMenu = true;
                                        Editor.tookInput = true;
                                        Editor.openContextThing = (owner as ContextMenu)._items[0];
                                        Editor.pretendPinned = owner;
                                    }
                                }
                            }
                        }
                        Vec2 vec2_3 = new Vec2(12f, 12f);
                        if (Editor.inputMode == EditorInput.Touch)
                            vec2_3 = new Vec2(24f, 24f);
                        Vec2 vec2_4 = lastDrawPos + new Vec2(-(vec2_3.x + 2f), 0f);
                        Vec2 vec2_5 = lastDrawPos + new Vec2(-2f, vec2_3.y);
                        _hoverBackArrow = false;
                        if (vec2_2.x > vec2_4.x && vec2_2.x < vec2_5.x && vec2_2.y > vec2_4.y && vec2_2.y < vec2_5.y)
                        {
                            _hoverBackArrow = true;
                            if (flag1)
                            {
                                Editor.reopenContextMenu = true;
                                Editor.clickedMenu = true;
                                Editor.tookInput = true;
                                if (Editor.inputMode == EditorInput.Touch)
                                {
                                    Editor.openContextThing = (owner as ContextMenu)._items[0];
                                    Editor.pretendPinned = owner;
                                }
                                else
                                    Editor.ignorePinning = true;
                            }
                        }
                    }
                    PushLeft();
                    int count = _items.Count;
                    for (int index = 0; index < count; ++index)
                    {
                        if (!pinOpened || _items[index].pinned)
                        {
                            if (_alwaysDrawLast)
                            {
                                if (index == _items.Count - 1 || index >= _drawIndex && index < _drawIndex + _maxNumToDraw && (index == _items.Count - 1 || index != _drawIndex + _maxNumToDraw - 1))
                                    _items[index].DoUpdate();
                            }
                            else if (index >= _drawIndex && index < _drawIndex + _maxNumToDraw)
                                _items[index].DoUpdate();
                            if (_collectionChanged)
                            {
                                _collectionChanged = false;
                                return;
                            }
                        }
                    }
                }
                waitInputFrame = false;
                if (pinOpened)
                    return;
                if (_items.Count > 0)
                    _canExpand = true;
                bool flag3 = false;
                if (!Editor.HasFocus())
                {
                    if (_hover && _dragMode && Editor.inputMode == EditorInput.Mouse && (!_sliding && Mouse.left == InputState.Pressed || _sliding && Mouse.left == InputState.Down))
                        flag3 = true;
                    if (Editor.inputMode == EditorInput.Mouse && Mouse.right == InputState.Pressed && closeOnRight)
                    {
                        Disappear();
                        _owner.Selected(null);
                        opened = false;
                        return;
                    }
                }
                if (_hover && tooltip != null)
                    Editor.tooltip = tooltip;
                if (!Editor.HasFocus())
                {
                    if ((Editor.lockInput == null || IsPartOf(Editor.lockInput)) && !Editor.tookInput && Editor.inputMode == EditorInput.Gamepad)
                    {
                        bool flag4 = HasOpen();
                        if (flag4 && Input.Pressed(Triggers.MenuLeft) && _canExpand)
                        {
                            bool flag5 = false;
                            foreach (ContextMenu contextMenu in _items)
                            {
                                if (contextMenu.HasOpen())
                                {
                                    flag5 = true;
                                    break;
                                }
                            }
                            if (!flag5)
                            {
                                Editor.tookInput = true;
                                Selected(null);
                            }
                        }
                        _takingInput = false;
                        if (!flag4)
                        {
                            if (opened && _items.Count > 0)
                            {
                                _takingInput = true;
                                bool flag6 = false;
                                if (Input.Pressed(Triggers.MenuUp))
                                {
                                    flag6 = true;
                                    if (_selectedIndex == _items.Count - 1 && _alwaysDrawLast)
                                    {
                                        _selectedIndex = _drawIndex + _maxNumToDraw >= _items.Count ? _drawIndex + _maxNumToDraw - 1 : _drawIndex + _maxNumToDraw - 2;
                                        if (_selectedIndex > _items.Count - 1)
                                            _selectedIndex = _items.Count - 1;
                                    }
                                    --_selectedIndex;
                                    for (int index = 0; index < _items.Count; ++index)
                                    {
                                        if (_selectedIndex < 0)
                                        {
                                            _selectedIndex = _items.Count - 1;
                                            if (_alwaysDrawLast)
                                            {
                                                _drawIndex = 0;
                                            }
                                            else
                                            {
                                                _drawIndex = _selectedIndex - _maxNumToDraw + 1;
                                                if (_drawIndex < 0)
                                                    _drawIndex = 0;
                                            }
                                        }
                                        if (_items[_selectedIndex].greyOut)
                                        {
                                            --_selectedIndex;
                                            _drawIndex = _selectedIndex;
                                        }
                                        else
                                            break;
                                    }
                                }
                                else if (Input.Pressed(Triggers.MenuDown))
                                {
                                    flag6 = true;
                                    ++_selectedIndex;
                                    for (int index = 0; index < _items.Count; ++index)
                                    {
                                        if (_selectedIndex > _items.Count - 1)
                                        {
                                            _selectedIndex = 0;
                                            _drawIndex = _selectedIndex;
                                        }
                                        if (_items[_selectedIndex].greyOut)
                                        {
                                            ++_selectedIndex;
                                            _drawIndex = _selectedIndex;
                                        }
                                        else
                                            break;
                                    }
                                }
                                if (flag6)
                                    PositionItems();
                                int num = 0;
                                foreach (ContextMenu contextMenu in _items)
                                {
                                    if (num == _selectedIndex)
                                    {
                                        contextMenu._hover = true;
                                        contextMenu.Hover();
                                    }
                                    else
                                        contextMenu._hover = false;
                                    ++num;
                                }
                            }
                            Rectangle rectangle = new Rectangle(x, y, itemSize.x, itemSize.y);
                            if (_hover && (Input.Pressed(Triggers.Select) || _canExpand && Input.Pressed(Triggers.MenuRight) || scrollButtonDirection != 0))
                            {
                                if (this.owner is ContextMenu owner)
                                {
                                    foreach (ContextMenu contextMenu in owner._items)
                                        contextMenu._hover = false;
                                }
                                _hover = true;
                                flag3 = true;
                            }
                        }
                    }
                    else
                    {
                        switch (Editor.inputMode)
                        {
                            case EditorInput.Mouse:
                                bool flag7 = false;
                                if (Editor.inputMode == EditorInput.Mouse && Mouse.x >= x && Mouse.x <= x + itemSize.x && Mouse.y >= y + 1f && Mouse.y <= y + itemSize.y - 1f)
                                {
                                    if (Mouse.left == InputState.Pressed)
                                        flag3 = true;
                                    Editor.hoverUI = true;
                                    _hover = true;
                                    flag7 = true;
                                }
                                if (flag7)
                                {
                                    if (owner is ContextMenu owner1)
                                    {
                                        int num = 0;
                                        using (List<ContextMenu>.Enumerator enumerator = owner1._items.GetEnumerator())
                                        {
                                            while (enumerator.MoveNext())
                                            {
                                                if (enumerator.Current == this)
                                                {
                                                    owner1._selectedIndex = num;
                                                    break;
                                                }
                                                ++num;
                                            }
                                            break;
                                        }
                                    }
                                    else
                                        break;
                                }
                                else
                                {
                                    if (!_dragMode || Mouse.left != InputState.Down || TouchScreen.IsTouchScreenActive() && !TouchScreen.IsScreenTouched())
                                    {
                                        _hover = false;
                                        break;
                                    }
                                    break;
                                }
                            case EditorInput.Touch:
                                Rectangle pRect = new Rectangle(x, y, itemSize.x, itemSize.y);
                                if (TouchScreen.GetTap().Check(pRect, layer.camera))
                                {
                                    flag3 = true;
                                    if (!_hover)
                                    {
                                        _canEditSlide = false;
                                        _enteringSlideMode = true;
                                    }
                                    _hover = true;
                                    if (owner is ContextMenu owner2)
                                    {
                                        int num = 0;
                                        using (List<ContextMenu>.Enumerator enumerator = owner2._items.GetEnumerator())
                                        {
                                            while (enumerator.MoveNext())
                                            {
                                                ContextMenu current = enumerator.Current;
                                                if (current == this)
                                                    owner2._selectedIndex = num;
                                                else
                                                    current._hover = false;
                                                ++num;
                                            }
                                            break;
                                        }
                                    }
                                    else
                                        break;
                                }
                                else
                                    break;
                        }
                    }
                    if (Editor.inputMode == EditorInput.Mouse && Mouse.x > lastDrawPos.x && Mouse.x < lastDrawPos.x + menuSize.x && Mouse.y > lastDrawPos.y && Mouse.y < lastDrawPos.y + menuSize.y)
                    {
                        if (Mouse.scroll != 0f && !_didContextScroll)
                        {
                            _drawIndex += Mouse.scroll > 0f ? 1 : -1;
                            _drawIndex = Maths.Clamp(_drawIndex, 0, _items.Count - _maxNumToDraw);
                            SFX.Play("highClick", 0.3f, 0.2f);
                            foreach (ContextMenu contextMenu in _items)
                            {
                                contextMenu.opened = false;
                                contextMenu._hover = false;
                            }
                            PositionItems();
                        }
                        _didContextScroll = false;
                    }
                }
                if (!Editor.HasFocus() && _hover && _dragMode && Editor.inputMode == EditorInput.Touch && TouchScreen.GetTouch() != Touch.None && TouchScreen.GetTouch().Check(new Rectangle(x, y, itemSize.x, itemSize.y), layer.camera))
                    flag3 = true;
                if (!flag3)
                    return;
                Editor.clickedMenu = true;
                Selected();
            }
        }

        public override void Terminate()
        {
        }

        public void ClearItems()
        {
            _collectionChanged = true;
            _items.Clear();
        }

        public virtual void Hover()
        {
        }

        private void UpdatePositioning()
        {
            bool flag = false;
            if (y + _openedOffset + menuSize.y + 16f > layer.height)
            {
                _openedOffset = (layer.height - menuSize.y - y - 16f);
                flag = true;
            }
            if (y + _openedOffset < 0f)
            {
                _openedOffset = -y;
                flag = true;
            }
            if (!flag)
                return;
            PositionItems();
        }

        public override void Draw()
        {
            position += offset;
            if (!_root && !pinOpened && !dontPush)
            {
                float num1 = 1f;
                if (greyOut)
                    num1 = 0.3f;
                float num2 = num1 * alpha;
                if (_hover && !greyOut)
                    Graphics.DrawRect(position, position + itemSize, new Color(70, 70, 70) * alpha, depth);
                if (scrollButtonDirection != 0)
                {
                    _arrow.depth = depth + 1;
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
                    if (_image != null)
                    {
                        _image.depth = depth + 3;
                        _image.x = x + 1f;
                        _image.y = y;
                        _image.color = Color.White * num2;
                        _image.Draw();
                        Graphics.DrawString(_text, position + new Vec2(20f, 4f), Color.White * num2, depth + 1);
                    }
                    else if (_text == "custom")
                        Graphics.DrawString(_text, position + new Vec2(2f, 4f), Colors.DGBlue * num2, depth + 1);
                    else if (fancy)
                    {
                        float num3 = 0f;
                        if (customIcon != null)
                        {
                            Vec2 vec2 = position + new Vec2(2f, 4f);
                            Graphics.Draw(customIcon.texture, vec2.x, vec2.y, depth: (depth + 1));
                            num3 += 8f;
                        }
                        Graphics.DrawFancyString(_text, position + new Vec2(2f + num3, 4f), Color.White * num2, depth + 1);
                        Vec2 previewPos = position + new DuckGame.Vec2(itemSize.x - 24, 0);
                        int numDraw = 0;

                        for (int i = 0; i < 3; i++)
                        {
                            if (numDraw == 4)
                                break;

                            foreach (ContextMenu mz in _items)
                            {
                                if (numDraw == 4)
                                    break;

                                for (int j = 0; j < mz._items.Count + 1; j++)
                                {
                                    ContextMenu m = mz;
                                    if (i == 1)
                                    {
                                        if (j == mz._items.Count)
                                            break;

                                        m = mz._items[j];
                                    }

                                    if (m._image != null && ((i == 2 && !m._previewPriority) || (i != 2 && m._previewPriority)))
                                    {
                                        Sprite s = m._image;
                                        if (s != null)
                                        {
                                            s.depth = depth + 3;
                                            s.x = previewPos.x + 1;
                                            s.y = previewPos.y;
                                            //s.color = Color.White * 0.5f;
                                            s.scale = new Vec2(0.5f);

                                            if (m._previewWidth > 16)
                                                Graphics.Draw(s, previewPos.x + 1, previewPos.y, new Rectangle(8, 0, 16, 16));
                                            else
                                                s.Draw();

                                            previewPos.x += 8;
                                            numDraw++;
                                            if (numDraw == 2)
                                            {
                                                previewPos.x -= 8 * 2;
                                                previewPos.y += 8;
                                            }
                                            else if (numDraw == 4)
                                                break;
                                        }
                                    }

                                    if (i != 1)
                                        break;
                                }
                            }
                        }

                    }
                    else
                        Graphics.DrawString(_text, position + new Vec2(2f, 4f), Color.White * num2, depth + 1);
                    if (_items.Count > 0)
                    {
                        _contextArrow.color = Color.White * num2;
                        Graphics.Draw(_contextArrow, (x + itemSize.x - 8f), y + 4f, depth + 1);
                        _contextArrow.color = Color.White;
                    }
                }
            }
            if (opened)
            {
                if (!pinOpened)
                {
                    UpdatePositioning();
                    float x = menuSize.x;
                    float y = menuSize.y;
                    Vec2 p1_1 = new Vec2(this.x + _openedOffsetX, this.y + _openedOffset) + new Vec2(-2f, -2f);
                    if (!_root && !dontPush)
                        p1_1.x += itemSize.x + 6f;
                    if (_showBackground)
                    {
                        Graphics.DrawRect(p1_1, p1_1 + new Vec2(x, y), new Color(70, 70, 70) * alpha, depth);
                        Graphics.DrawRect(p1_1 + new Vec2(1f, 1f), p1_1 + new Vec2(x - 1f, y - 1f), new Color(30, 30, 30) * alpha, depth + 1);
                        _lastDrawPos = p1_1;
                        if (_items.Count > 0 && isPinnable && (!_root || pinned))
                        {
                            Sprite g = _pin;
                            if (pinned && Editor.pretendPinned != this || _pinned)
                                g = _pinPinned;
                            g.depth = depth + 2;
                            if (_hoverPin)
                                g.alpha = 1f;
                            else
                                g.alpha = 0.5f;
                            Vec2 vec2_2 = new Vec2((p1_1.x + x - 5f), p1_1.y - 4f);
                            Graphics.Draw(g, vec2_2.x, vec2_2.y);
                            Vec2 p1_2 = vec2_2 + new Vec2(-6f, -6f);
                            Vec2 p2_1 = p1_2 + new Vec2(11f, 11f);
                            if (Editor.inputMode == EditorInput.Gamepad && _takingInput)
                                p1_2.x -= 10f;
                            Graphics.DrawRect(p1_2, p2_1, new Color(70, 70, 70) * alpha, depth);
                            Graphics.DrawRect(p1_2 + new Vec2(1f, 1f), p2_1 + new Vec2(-1f, 0f), new Color(30, 30, 30) * alpha, depth + 1);
                            if (_owner != null && _owner is ContextMenu && (_owner as ContextMenu).pinOpened)
                            {
                                Vec2 vec2_3 = new Vec2(12f, 12f);
                                if (Editor.bigInterfaceMode)
                                    vec2_3 = new Vec2(24f, 24f);
                                Vec2 p1_3 = p1_1 + new Vec2((float)-(vec2_3.x + 2f), 0f);
                                Vec2 p2_2 = p1_1 + new Vec2(-2f, vec2_3.y);
                                Graphics.DrawRect(p1_3, p2_2, new Color(70, 70, 70) * alpha, depth);
                                Graphics.DrawRect(p1_3 + new Vec2(1f, 1f), p2_2 + new Vec2(-1f, -1f), new Color(30, 30, 30) * alpha, depth + 1);
                                _contextArrow.flipH = true;
                                _contextArrow.depth = depth + 2;
                                if (_hoverBackArrow)
                                    _contextArrow.alpha = 1f;
                                else
                                    _contextArrow.alpha = 0.5f;
                                _contextArrow.alpha = 1f;
                                Graphics.Draw(_contextArrow, p1_3.x + vec2_3.x / 2f + _contextArrow.width / 2, p1_3.y + vec2_3.y / 2f - _contextArrow.height / 2);
                                _contextArrow.flipH = false;
                            }
                        }
                    }
                    if (Editor.inputMode == EditorInput.Gamepad && drawControls && _takingInput)
                    {
                        string text = "";
                        bool flag = false;
                        foreach (ContextMenu contextMenu in _items)
                        {
                            if (contextMenu.hover)
                            {
                                if (contextMenu._items.Count > 0 || contextMenu is ContextBackgroundTile)
                                {
                                    text = "@SELECT@@RIGHT@EXPAND";
                                }
                                else
                                {
                                    switch (contextMenu)
                                    {
                                        case ContextSlider _:
                                            if ((contextMenu as ContextSlider).adjust)
                                            {
                                                text = "@WASD@EDIT @STRAFE@SLOW @RAGDOLL@FAST ";
                                                flag = true;
                                                continue;
                                            }
                                            text = "@SELECT@EDIT";
                                            continue;
                                        case ContextTextbox _:
                                            text = "@SELECT@ENTER TEXT";
                                            continue;
                                        case ContextRadio _:
                                        case ContextCheckBox _:
                                            text = "@SELECT@TOGGLE";
                                            continue;
                                        default:
                                            text = "@SELECT@SELECT";
                                            continue;
                                    }
                                }
                            }
                        }
                        if (!_root && !flag)
                            text += "  @LEFT@BACK";
                        Graphics.DrawRect(p1_1 + new Vec2(0f, y), p1_1 + new Vec2(x, y + 15f), Color.Black * alpha, depth);
                        Graphics.DrawString(text, p1_1 + new Vec2(0f, y + 4f), Color.White * alpha, depth + 1);
                        if (isPinnable && (!_root || pinned))
                        {
                            Graphics._biosFont.spriteScale = new Vec2(0.75f);
                            Graphics.DrawString("@MENU1@", p1_1 + new Vec2(menuSize.x - 20f, -7f), Color.White * alpha, depth + 4, scale: 0.5f);
                            Graphics._biosFont.spriteScale = new Vec2(1f);
                        }
                    }
                    if (_hasToproot && !dontPush)
                        Graphics.DrawRect(_toprootPosition, _toprootPosition + new Vec2(16f, 32f), new Color(70, 70, 70) * alpha, depth - 4);
                }
                int num = 0;
                foreach (ContextMenu contextMenu in _items)
                {
                    if (!pinOpened || contextMenu.opened)
                    {
                        contextMenu.scrollButtonDirection = 0;
                        if (num == _drawIndex && _drawIndex > 0)
                            contextMenu.scrollButtonDirection = -1;
                        else if ((num - _drawIndex == _maxNumToDraw - 1 || _alwaysDrawLast && num - _drawIndex == _maxNumToDraw - 2) && _items.Count - (_alwaysDrawLast ? 2 : 1) > num)
                            contextMenu.scrollButtonDirection = 1;
                        if (_alwaysDrawLast)
                        {
                            if (num == _items.Count - 1)
                                contextMenu.scrollButtonDirection = 0;
                            if ((num == _items.Count - 1 || num >= _drawIndex && num < _drawIndex + _maxNumToDraw && (num == _items.Count - 1 || num != _drawIndex + _maxNumToDraw - 1)) && contextMenu.visible)
                                contextMenu.DoDraw();
                        }
                        else if (num >= _drawIndex && num - _drawIndex < _maxNumToDraw && contextMenu.visible)
                            contextMenu.DoDraw();
                    }
                    ++num;
                }
            }
            position -= offset;
        }

        public virtual void Selected()
        {
            if (greyOut || _owner == null)
                return;
            _owner.Selected(this);
        }

        public bool IsEditorPlacementMenu() => true;

        public virtual void Selected(ContextMenu item)
        {
            if (greyOut)
                return;
            if (item != null && item.scrollButtonDirection != 0)
            {
                _drawIndex += item.scrollButtonDirection;
                _drawIndex = Maths.Clamp(_drawIndex, 0, _items.Count - _maxNumToDraw);
                SFX.Play("highClick", 0.3f, 0.2f);
                foreach (ContextMenu contextMenu in _items)
                {
                    contextMenu.opened = false;
                    contextMenu._hover = false;
                }
                PositionItems();
            }
            else
            {
                foreach (ContextMenu contextMenu in _items)
                {
                    if (contextMenu != item)
                        contextMenu.opened = false;
                }
                if (item != null)
                {
                    if (IsEditorPlacementMenu() && Editor.bigInterfaceMode && item is EditorGroupMenu || item.text == "More...")
                    {
                        if (!item.opened)
                        {
                            Editor.ignorePinning = false;
                            Editor.reopenContextMenu = true;
                            Editor.clickedMenu = true;
                            Editor.tookInput = true;
                            Editor.openContextThing = item;
                            Editor.pretendPinned = item;
                            SFX.Play("highClick", 0.3f, 0.2f);
                        }
                    }
                    else
                    {
                        item.opened = true;
                        Editor.clickedMenu = true;
                        Editor.tookInput = true;
                        SFX.Play("highClick", 0.3f, 0.2f);
                    }
                }
                waitInputFrame = true;
            }
        }

        public void AddItem(ContextMenu item) => AddItem(item, -1);

        public void AddItem(ContextMenu item, int index)
        {
            item.Initialize();
            if (index >= 0)
                _items.Insert(index, item);
            else
                _items.Add(item);
            item.owner = this;
            PositionItems();
        }

        public Thing GetPlacementType(Type pType)
        {
            if (this is ContextObject && (this as ContextObject).thing.GetType() == pType)
                return (this as ContextObject).thing;
            foreach (ContextMenu contextMenu in _items)
            {
                Thing placementType = contextMenu.GetPlacementType(pType);
                if (placementType != null)
                    return placementType;
            }
            return null;
        }

        /// <summary>
        /// Calculate percentage similarity of two strings
        /// <param name="source">Source String to Compare with</param>
        /// <param name="target">Targeted String to Compare</param>
        /// <returns>Return Similarity between two strings from 0 to 1.0</returns>
        /// </summary>
        private double CalculateSimilarity(string source, string target)
        {
            if (source == null || target == null || source.Length == 0 || target.Length == 0)
                return 0f;
            return source == target ? 1f : 1f - ComputeLevenshteinDistance(source, target) / Math.Max(source.Length, target.Length);
        }

        /// <summary>
        /// Returns the number of steps required to transform the source string
        /// into the target string.
        /// </summary>
        private int ComputeLevenshteinDistance(string source, string target)
        {
            if (source == null || target == null || source.Length == 0 || target.Length == 0)
                return 0;
            if (source == target)
                return source.Length;
            int length1 = source.Length;
            int length2 = target.Length;
            if (length1 == 0)
                return length2;
            if (length2 == 0)
                return length1;
            int[,] numArray = new int[length1 + 1, length2 + 1];
            int index1 = 0;
            while (index1 <= length1)
                numArray[index1, 0] = index1++;
            int index2 = 0;
            while (index2 <= length2)
                numArray[0, index2] = index2++;
            for (int index3 = 1; index3 <= length1; ++index3)
            {
                for (int index4 = 1; index4 <= length2; ++index4)
                {
                    int num = target[index4 - 1] == source[index3 - 1] ? 0 : 1;
                    numArray[index3, index4] = Math.Min(Math.Min(numArray[index3 - 1, index4] + 1, numArray[index3, index4 - 1] + 1), numArray[index3 - 1, index4 - 1] + num);
                }
            }
            return numArray[length1, length2];
        }

        public List<SearchPair> Search(string pTerm) => Search(pTerm.ToLowerInvariant(), new List<SearchPair>()).OrderBy(x => -x.relevance).ToList();

        private List<SearchPair> Search(
          string pTerm,
          List<SearchPair> pCurrentTerms)
        {
            if (this is ContextObject)
            {
                string lowerInvariant = _text.ToLowerInvariant();
                double similarity = CalculateSimilarity(pTerm, lowerInvariant);
                float num = 1f;
                for (int index = 0; index < pTerm.Length && index < lowerInvariant.Length; ++index)
                {
                    if (pTerm[index] == lowerInvariant[index])
                    {
                        similarity += 0.1f * num;
                        ++num;
                    }
                    else
                        similarity -= 0.05f * num;
                }
                if (lowerInvariant.Contains(pTerm))
                    similarity += 0.6f;
                if (similarity > 0.25f)
                    pCurrentTerms.Add(new SearchPair()
                    {
                        relevance = similarity,
                        thing = this as ContextObject
                    });
            }
            foreach (ContextMenu contextMenu in _items)
                contextMenu.Search(pTerm, pCurrentTerms);
            return pCurrentTerms;
        }

        public bool imageOnly = false;
        public void PositionItems()
        {
            float largestWidth = 0.0f;
            float ypos = y + _openedOffset;
            _openedOffsetX = 0;

            if (Editor.inputMode != EditorInput.Mouse && /*pinned == false && */_root == false)
            {
                ypos = 16;
                _openedOffset = (-y) + 16;
            }

            for (int i = 0; i < _items.Count; i++)
            {
                ContextMenu item = _items[i];
                if (!item.opened || (item is ContextToolbarItem && (item as ContextToolbarItem).isPushingUp))
                {
                    if (_root == false && !dontPush)
                        item.x = x + 3 + itemSize.x + 3;
                    else
                        item.x = x;



                    if ((_pinned || Editor.pretendPinned == this) && _root == false)
                    {
                        if (Editor.bigInterfaceMode)
                        {
                            item.x += 14;
                            _openedOffsetX = 14;
                        }
                        else
                        {
                            item.x += 4;
                            _openedOffsetX = 4;
                        }
                    }


                    item.y = ypos;
                }

                if (i >= _drawIndex && !pinOpened)
                {
                    if ((_alwaysDrawLast && (i == _items.Count - 1 || (i != (_drawIndex + _maxNumToDraw) - 1 && i < _drawIndex + _maxNumToDraw))) || (!_alwaysDrawLast))
                        ypos += item.itemSize.y + 1;
                }

                //if (_root == false && !dontPush)
                //    item.y -= 2;
                //else
                //    item.y -= 1;

                if (item.imageOnly == false && item.itemSize.x < 107)
                    item.itemSize.x = 107;

                if (item.itemSize.x + 4 > menuSize.x)
                    menuSize.x = item.itemSize.x + 4;

                item.depth = depth + 2;
                if (item.itemSize.x > largestWidth)
                    largestWidth = item.itemSize.x;
            }

            int numItemsSized = 0;
            float hval = 0;
            foreach (ContextMenu item in _items)
            {
                if (numItemsSized < _maxNumToDraw)
                    hval += item.itemSize.y + 1;

                item.itemSize.x = largestWidth;
                numItemsSized++;
            }
            menuSize.y = hval + 3;
        }

        public void CloseMenus()
        {
            foreach (ContextMenu contextMenu in _items)
                contextMenu.opened = false;
        }

        public virtual void Closed()
        {
        }

        public class SearchPair
        {
            public double relevance;
            public ContextObject thing;
        }
    }
}
