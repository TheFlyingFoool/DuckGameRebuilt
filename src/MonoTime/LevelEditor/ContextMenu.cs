// Decompiled with JetBrains decompiler
// Type: DuckGame.ContextMenu
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections.Generic;
using System.Linq;

namespace DuckGame
{
    public class ContextMenu : Thing, IContextListener
    {
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
            get => this._text;
            set => this._text = value;
        }

        public string data
        {
            get => this._data;
            set => this._data = value;
        }

        public int selectedIndex
        {
            get => this._selectedIndex;
            set => this._selectedIndex = value;
        }

        public bool dontPush
        {
            get => this._dontPush;
            set
            {
                if (value)
                {
                    if (!this.pinned)
                        this.pinOpened = true;
                    this._dontPush = true;
                }
                else
                {
                    this._dontPush = false;
                    this.pinOpened = false;
                }
            }
        }

        protected virtual void OnClose()
        {
        }

        public bool opened
        {
            get => this._opened;
            set
            {
                if (!this._opened && value)
                {
                    this._lastDrawPos = Vec2.Zero;
                    this.pinOpened = false;
                    foreach (ContextMenu contextMenu in this._items)
                    {
                        contextMenu._lastDrawPos = Vec2.Zero;
                        contextMenu.opened = false;
                        contextMenu._hover = false;
                        if (contextMenu.pinned && !Editor.ignorePinning)
                            this.pinOpened = true;
                    }
                    this._openedOffset = 0f;
                    this.PositionItems();
                    this._selectedIndex = 0;
                    if (this._items.Count > 0)
                    {
                        while (this._selectedIndex < this._items.Count<ContextMenu>() - 1 && this._items[this._selectedIndex].greyOut)
                            ++this._selectedIndex;
                        if (!this._items[this._selectedIndex].greyOut)
                            this._opening = true;
                    }
                    else
                        this._opening = true;
                    foreach (ContextMenu contextMenu in this._items)
                    {
                        contextMenu.dontPush = false;
                        if (contextMenu.pinned && !Editor.ignorePinning)
                        {
                            contextMenu.dontPush = true;
                            contextMenu.opened = true;
                        }
                    }
                    this.PushLeft();
                }
                if (this._opened && !value)
                {
                    foreach (ContextMenu contextMenu in this._items)
                    {
                        contextMenu.opened = false;
                        contextMenu._hover = false;
                        contextMenu.OnClose();
                    }
                    this.Closed();
                    if (this._root)
                        Editor.ignorePinning = false;
                }
                if (!value)
                {
                    foreach (ContextMenu contextMenu in this._items)
                        contextMenu.ParentCloseAction();
                }
                if (value)
                {
                    if (this._autoSelectItem >= 0)
                        this._selectedIndex = this._autoSelectItem;
                    this._autoSelectItem = -1;
                }
                this._opened = value;
            }
        }

        public bool hover
        {
            get => this._hover;
            set => this._hover = value;
        }

        public bool root
        {
            get => this._root;
            set => this._root = value;
        }

        public virtual void ParentCloseAction()
        {
        }

        public Sprite image
        {
            get => this._image;
            set => this._image = value;
        }

        public bool pinned
        {
            get
            {
                if (this._pinned && Editor.pretendPinned == null || Editor.pretendPinned == this)
                    return true;
                foreach (ContextMenu contextMenu in this._items)
                {
                    if (contextMenu.pinned)
                        return true;
                }
                return false;
            }
            set => this._pinned = value;
        }

        public ContextMenu(IContextListener owner, SpriteMap img = null, bool hasToproot = false, Vec2 topRoot = default(Vec2))
          : base()
        {
            this._owner = owner;
            if (Level.current is Editor)
                this.layer = Editor.objectMenuLayer;
            else
                this.layer = Layer.HUD;
            this._contextArrow = new Sprite("contextArrowRight");
            this.itemSize.x = 100f;
            this.itemSize.y = 16f;
            this._root = owner == null;
            this._image = img;
            this.depth = (Depth)0.8f;
            this._arrow.CenterOrigin();
            this._pin.CenterOrigin();
            this._pinPinned.CenterOrigin();
            this._toprootPosition = topRoot;
            this._hasToproot = hasToproot;
        }

        public override void Initialize()
        {
        }

        public virtual bool HasOpen()
        {
            foreach (ContextMenu contextMenu in this._items)
            {
                if (contextMenu.opened)
                    return true;
            }
            return false;
        }

        public virtual void Toggle(ContextMenu item)
        {
            if (this._owner == null)
                return;
            this.isToggle = true;
            this._owner.Selected(item);
            this.isToggle = false;
        }

        public bool IsPartOf(Thing menu)
        {
            if (menu == null || this == menu)
                return true;
            return this._owner != null && this._owner is ContextMenu owner && owner.IsPartOf(menu);
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
            foreach (ContextMenu contextMenu2 in this._items)
            {
                if (contextMenu2.opened)
                {
                    contextMenu2.PushLeft();
                    contextMenu1 = contextMenu2;
                }
            }
            if (Keyboard.Down(Keys.Y))
                return;
            Vec2 vec2_1 = new Vec2(this.x, this.y);
            Vec2 vec2_2 = new Vec2(0f, 0f);
            if (!this._root && !this.dontPush)
                vec2_2 = new Vec2(this.itemSize.x + 4f, -2f);
            Vec2 vec2_3 = vec2_1 + vec2_2;
            bool flag = false;
            if (_lastDrawPos.x + this.menuSize.x + 4f > this.layer.camera.width)
            {
                if (Editor.bigInterfaceMode)
                    vec2_3.x -= this.menuSize.x;
                else
                    vec2_3.x = (this.layer.camera.width - menuSize.x - 4f);
                flag = true;
            }
            if (contextMenu1 != null && contextMenu1.x != vec2_3.x && !this.pinOpened)
            {
                vec2_3.x = !this._root ? (this._pinned || Editor.pretendPinned == this ? contextMenu1.x - 4f : contextMenu1.x - 2f) : contextMenu1.x;
                flag = true;
            }
            this.position = vec2_3 - vec2_2;
            if (!flag)
                return;
            this.PositionItems();
        }

        private void PinChanged(ContextMenu c)
        {
            if (this._owner != null && this._owner is ContextMenu)
                (this._owner as ContextMenu).PinChanged(c);
            else
                this.PinChangedDrillDown(c);
        }

        private void PinChangedDrillDown(ContextMenu c)
        {
            if (c != this)
                this.pinned = false;
            foreach (ContextMenu contextMenu in this._items)
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
            foreach (ContextMenu contextMenu1 in this._items)
            {
                if (contextMenu1 == t || contextMenu1 is ContextObject && (contextMenu1 as ContextObject).thing.GetType() == t.GetType())
                {
                    this._autoSelectItem = num;
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
            if (!this.HasOpen() || !this._canExpand)
                return;
            bool flag = false;
            foreach (ContextMenu contextMenu in this._items)
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
            this.Selected(null);
        }

        public virtual void Disappear()
        {
        }

        public override void Update()
        {
            if (!this.visible || this.disabled || this._opening)
            {
                this._opening = false;
            }
            else
            {
                if (!this.IsPartOf(Editor.lockInput))
                    return;
                Vec2 lastDrawPos = this._lastDrawPos;
                if (this.opened)
                {
                    if (Editor.inputMode == EditorInput.Touch && TouchScreen.GetTap().Check(new Rectangle(this._lastDrawPos.x, this._lastDrawPos.y, this._lastDrawPos.x + this.menuSize.x, this._lastDrawPos.y + this.menuSize.y), this.layer.camera) && !this.pinOpened)
                        Editor.clickedContextBackground = true;
                    if (!this.pinOpened)
                    {
                        Vec2 vec2_1 = new Vec2((float)(lastDrawPos.x + this.menuSize.x - 5f), lastDrawPos.y - 4f);
                        this._hoverPin = false;
                        Vec2 vec2_2 = Vec2.Zero;
                        bool flag1 = false;
                        switch (Editor.inputMode)
                        {
                            case EditorInput.Mouse:
                                vec2_2 = Mouse.position;
                                flag1 = Mouse.left == InputState.Pressed;
                                break;
                            case EditorInput.Touch:
                                vec2_2 = TouchScreen.GetTap().Transform(this.layer.camera);
                                flag1 = TouchScreen.GetTap() != Touch.None;
                                break;
                        }
                        if (vec2_2.x > vec2_1.x - 5.0 && vec2_2.x < vec2_1.x + 3.0 && vec2_2.y > vec2_1.y - 4.0 && vec2_2.y < vec2_1.y + 4.0)
                        {
                            this._hoverPin = true;
                            if (flag1 && (!this._root || this.pinned))
                            {
                                if (this._root && this.pinned)
                                {
                                    this._pinned = false;
                                    this.PinChanged(this);
                                    Editor.openPosition = this._lastDrawPos;
                                    Editor.reopenContextMenu = true;
                                    Editor.ignorePinning = false;
                                    Editor.clickedMenu = true;
                                    this._autoSelectItem = this._selectedIndex;
                                }
                                else
                                {
                                    bool pinned = this.pinned;
                                    this._pinned = !this._pinned;
                                    if (this._owner != null && this._owner is ContextMenu)
                                        (this._owner as ContextMenu).PinChanged(this);
                                    if (this._pinned != pinned && Editor.pretendPinned != this)
                                    {
                                        Editor.openPosition = this._lastDrawPos;
                                        Editor.reopenContextMenu = true;
                                        Editor.ignorePinning = false;
                                    }
                                    Editor.clickedMenu = true;
                                    this._autoSelectItem = this._selectedIndex;
                                }
                            }
                        }
                        if (!this.waitInputFrame && Editor.inputMode == EditorInput.Gamepad && this.drawControls)
                        {
                            if (Input.Pressed("MENU1"))
                            {
                                if (this.isPinnable && this._root && this.pinned)
                                {
                                    this._pinned = false;
                                    this.PinChanged(this);
                                    Editor.openPosition = this._lastDrawPos;
                                    Editor.reopenContextMenu = true;
                                    Editor.ignorePinning = false;
                                    this._autoSelectItem = this._selectedIndex;
                                }
                                else if (this.isPinnable && this.owner != null)
                                {
                                    this._pinned = !this._pinned;
                                    if (this._owner != null && this._owner is ContextMenu)
                                        (this._owner as ContextMenu).PinChanged(this);
                                    Editor.openPosition = this._lastDrawPos;
                                    Editor.reopenContextMenu = true;
                                    Editor.ignorePinning = false;
                                    this._autoSelectItem = this._selectedIndex;
                                }
                            }
                            else if (this.pinned && this._owner != null && this._owner is ContextMenu && (this._owner as ContextMenu).pinOpened && Input.Pressed("MENULEFT"))
                            {
                                bool flag2 = false;
                                foreach (ContextMenu contextMenu in this._items)
                                {
                                    if (contextMenu.opened)
                                    {
                                        flag2 = true;
                                        break;
                                    }
                                }
                                if (!flag2)
                                {
                                    if (!Editor.bigInterfaceMode || !this.IsEditorPlacementMenu())
                                    {
                                        Editor.ignorePinning = true;
                                        Editor.reopenContextMenu = true;
                                    }
                                    else
                                    {
                                        Editor.reopenContextMenu = true;
                                        Editor.clickedMenu = true;
                                        Editor.tookInput = true;
                                        Editor.openContextThing = (this.owner as ContextMenu)._items[0];
                                        Editor.pretendPinned = this.owner;
                                    }
                                }
                            }
                        }
                        Vec2 vec2_3 = new Vec2(12f, 12f);
                        if (Editor.inputMode == EditorInput.Touch)
                            vec2_3 = new Vec2(24f, 24f);
                        Vec2 vec2_4 = lastDrawPos + new Vec2(-(vec2_3.x + 2f), 0f);
                        Vec2 vec2_5 = lastDrawPos + new Vec2(-2f, vec2_3.y);
                        this._hoverBackArrow = false;
                        if (vec2_2.x > vec2_4.x && vec2_2.x < vec2_5.x && vec2_2.y > vec2_4.y && vec2_2.y < vec2_5.y)
                        {
                            this._hoverBackArrow = true;
                            if (flag1)
                            {
                                Editor.reopenContextMenu = true;
                                Editor.clickedMenu = true;
                                Editor.tookInput = true;
                                if (Editor.inputMode == EditorInput.Touch)
                                {
                                    Editor.openContextThing = (this.owner as ContextMenu)._items[0];
                                    Editor.pretendPinned = this.owner;
                                }
                                else
                                    Editor.ignorePinning = true;
                            }
                        }
                    }
                    this.PushLeft();
                    int count = this._items.Count;
                    for (int index = 0; index < count; ++index)
                    {
                        if (!this.pinOpened || this._items[index].pinned)
                        {
                            if (this._alwaysDrawLast)
                            {
                                if (index == this._items.Count - 1 || index >= this._drawIndex && index < this._drawIndex + this._maxNumToDraw && (index == this._items.Count - 1 || index != this._drawIndex + this._maxNumToDraw - 1))
                                    this._items[index].DoUpdate();
                            }
                            else if (index >= this._drawIndex && index < this._drawIndex + this._maxNumToDraw)
                                this._items[index].DoUpdate();
                            if (this._collectionChanged)
                            {
                                this._collectionChanged = false;
                                return;
                            }
                        }
                    }
                }
                this.waitInputFrame = false;
                if (this.pinOpened)
                    return;
                if (this._items.Count > 0)
                    this._canExpand = true;
                bool flag3 = false;
                if (!Editor.HasFocus())
                {
                    if (this._hover && this._dragMode && Editor.inputMode == EditorInput.Mouse && (!this._sliding && Mouse.left == InputState.Pressed || this._sliding && Mouse.left == InputState.Down))
                        flag3 = true;
                    if (Editor.inputMode == EditorInput.Mouse && Mouse.right == InputState.Pressed && this.closeOnRight)
                    {
                        this.Disappear();
                        this._owner.Selected(null);
                        this.opened = false;
                        return;
                    }
                }
                if (this._hover && this.tooltip != null)
                    Editor.tooltip = this.tooltip;
                if (!Editor.HasFocus())
                {
                    if ((Editor.lockInput == null || this.IsPartOf(Editor.lockInput)) && !Editor.tookInput && Editor.inputMode == EditorInput.Gamepad)
                    {
                        bool flag4 = this.HasOpen();
                        if (flag4 && Input.Pressed("MENULEFT") && this._canExpand)
                        {
                            bool flag5 = false;
                            foreach (ContextMenu contextMenu in this._items)
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
                                this.Selected(null);
                            }
                        }
                        this._takingInput = false;
                        if (!flag4)
                        {
                            if (this.opened && this._items.Count > 0)
                            {
                                this._takingInput = true;
                                bool flag6 = false;
                                if (Input.Pressed("MENUUP"))
                                {
                                    flag6 = true;
                                    if (this._selectedIndex == this._items.Count - 1 && this._alwaysDrawLast)
                                    {
                                        this._selectedIndex = this._drawIndex + this._maxNumToDraw >= this._items.Count ? this._drawIndex + this._maxNumToDraw - 1 : this._drawIndex + this._maxNumToDraw - 2;
                                        if (this._selectedIndex > this._items.Count - 1)
                                            this._selectedIndex = this._items.Count - 1;
                                    }
                                    --this._selectedIndex;
                                    for (int index = 0; index < this._items.Count; ++index)
                                    {
                                        if (this._selectedIndex < 0)
                                        {
                                            this._selectedIndex = this._items.Count - 1;
                                            if (this._alwaysDrawLast)
                                            {
                                                this._drawIndex = 0;
                                            }
                                            else
                                            {
                                                this._drawIndex = this._selectedIndex - this._maxNumToDraw + 1;
                                                if (this._drawIndex < 0)
                                                    this._drawIndex = 0;
                                            }
                                        }
                                        if (this._items[this._selectedIndex].greyOut)
                                        {
                                            --this._selectedIndex;
                                            this._drawIndex = this._selectedIndex;
                                        }
                                        else
                                            break;
                                    }
                                }
                                else if (Input.Pressed("MENUDOWN"))
                                {
                                    flag6 = true;
                                    ++this._selectedIndex;
                                    for (int index = 0; index < this._items.Count; ++index)
                                    {
                                        if (this._selectedIndex > this._items.Count - 1)
                                        {
                                            this._selectedIndex = 0;
                                            this._drawIndex = this._selectedIndex;
                                        }
                                        if (this._items[this._selectedIndex].greyOut)
                                        {
                                            ++this._selectedIndex;
                                            this._drawIndex = this._selectedIndex;
                                        }
                                        else
                                            break;
                                    }
                                }
                                if (flag6)
                                    this.PositionItems();
                                int num = 0;
                                foreach (ContextMenu contextMenu in this._items)
                                {
                                    if (num == this._selectedIndex)
                                    {
                                        contextMenu._hover = true;
                                        contextMenu.Hover();
                                    }
                                    else
                                        contextMenu._hover = false;
                                    ++num;
                                }
                            }
                            Rectangle rectangle = new Rectangle(this.x, this.y, this.itemSize.x, this.itemSize.y);
                            if (this._hover && (Input.Pressed("SELECT") || this._canExpand && Input.Pressed("MENURIGHT") || this.scrollButtonDirection != 0))
                            {
                                if (this.owner is ContextMenu owner)
                                {
                                    foreach (ContextMenu contextMenu in owner._items)
                                        contextMenu._hover = false;
                                }
                                this._hover = true;
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
                                if (Editor.inputMode == EditorInput.Mouse && Mouse.x >= this.x && Mouse.x <= this.x + itemSize.x && Mouse.y >= this.y + 1f && Mouse.y <= this.y + itemSize.y - 1f)
                                {
                                    if (Mouse.left == InputState.Pressed)
                                        flag3 = true;
                                    Editor.hoverUI = true;
                                    this._hover = true;
                                    flag7 = true;
                                }
                                if (flag7)
                                {
                                    if (this.owner is ContextMenu owner1)
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
                                    if (!this._dragMode || Mouse.left != InputState.Down || TouchScreen.IsTouchScreenActive() && !TouchScreen.IsScreenTouched())
                                    {
                                        this._hover = false;
                                        break;
                                    }
                                    break;
                                }
                            case EditorInput.Touch:
                                Rectangle pRect = new Rectangle(this.x, this.y, this.itemSize.x, this.itemSize.y);
                                if (TouchScreen.GetTap().Check(pRect, this.layer.camera))
                                {
                                    flag3 = true;
                                    if (!this._hover)
                                    {
                                        this._canEditSlide = false;
                                        this._enteringSlideMode = true;
                                    }
                                    this._hover = true;
                                    if (this.owner is ContextMenu owner2)
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
                    if (Editor.inputMode == EditorInput.Mouse && Mouse.x > lastDrawPos.x && Mouse.x < lastDrawPos.x + this.menuSize.x && Mouse.y > lastDrawPos.y && Mouse.y < lastDrawPos.y + this.menuSize.y)
                    {
                        if (Mouse.scroll != 0f && !ContextMenu._didContextScroll)
                        {
                            this._drawIndex += Mouse.scroll > 0f ? 1 : -1;
                            this._drawIndex = Maths.Clamp(this._drawIndex, 0, this._items.Count - this._maxNumToDraw);
                            SFX.Play("highClick", 0.3f, 0.2f);
                            foreach (ContextMenu contextMenu in this._items)
                            {
                                contextMenu.opened = false;
                                contextMenu._hover = false;
                            }
                            this.PositionItems();
                        }
                        ContextMenu._didContextScroll = false;
                    }
                }
                if (!Editor.HasFocus() && this._hover && this._dragMode && Editor.inputMode == EditorInput.Touch && TouchScreen.GetTouch() != Touch.None && TouchScreen.GetTouch().Check(new Rectangle(this.x, this.y, this.itemSize.x, this.itemSize.y), this.layer.camera))
                    flag3 = true;
                if (!flag3)
                    return;
                Editor.clickedMenu = true;
                this.Selected();
            }
        }

        public override void Terminate()
        {
        }

        public void ClearItems()
        {
            this._collectionChanged = true;
            this._items.Clear();
        }

        public virtual void Hover()
        {
        }

        private void UpdatePositioning()
        {
            bool flag = false;
            if (this.y + _openedOffset + menuSize.y + 16f > this.layer.height)
            {
                this._openedOffset = (this.layer.height - menuSize.y - this.y - 16f);
                flag = true;
            }
            if (this.y + _openedOffset < 0f)
            {
                this._openedOffset = -this.y;
                flag = true;
            }
            if (!flag)
                return;
            this.PositionItems();
        }

        public override void Draw()
        {
            this.position += this.offset;
            if (!this._root && !this.pinOpened && !this.dontPush)
            {
                float num1 = 1f;
                if (this.greyOut)
                    num1 = 0.3f;
                float num2 = num1 * this.alpha;
                if (this._hover && !this.greyOut)
                    Graphics.DrawRect(this.position, this.position + this.itemSize, new Color(70, 70, 70) * this.alpha, this.depth);
                if (this.scrollButtonDirection != 0)
                {
                    this._arrow.depth = this.depth + 1;
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
                    if (this._image != null)
                    {
                        this._image.depth = this.depth + 3;
                        this._image.x = this.x + 1f;
                        this._image.y = this.y;
                        this._image.color = Color.White * num2;
                        this._image.Draw();
                        Graphics.DrawString(this._text, this.position + new Vec2(20f, 4f), Color.White * num2, this.depth + 1);
                    }
                    else if (this._text == "custom")
                        Graphics.DrawString(this._text, this.position + new Vec2(2f, 4f), Colors.DGBlue * num2, this.depth + 1);
                    else if (this.fancy)
                    {
                        float num3 = 0f;
                        if (this.customIcon != null)
                        {
                            Vec2 vec2 = this.position + new Vec2(2f, 4f);
                            Graphics.Draw(this.customIcon.texture, vec2.x, vec2.y, depth: (this.depth + 1));
                            num3 += 8f;
                        }
                        Graphics.DrawFancyString(this._text, this.position + new Vec2(2f + num3, 4f), Color.White * num2, this.depth + 1);
                        Vec2 vec2_1 = this.position + new Vec2(this.itemSize.x - 24f, 0f);
                        int num4 = 0;
                        for (int index1 = 0; index1 < 3 && num4 != 4; ++index1)
                        {
                            using (List<ContextMenu>.Enumerator enumerator = this._items.GetEnumerator())
                            {
                            label_31:
                                while (enumerator.MoveNext())
                                {
                                    ContextMenu current = enumerator.Current;
                                    if (num4 != 4)
                                    {
                                        for (int index2 = 0; index2 < current._items.Count + 1; ++index2)
                                        {
                                            ContextMenu contextMenu = current;
                                            if (index1 == 1)
                                            {
                                                if (index2 != current._items.Count)
                                                    contextMenu = current._items[index2];
                                                else
                                                    break;
                                            }
                                            if (contextMenu._image != null && (index1 == 2 && !contextMenu._previewPriority || index1 != 2 && contextMenu._previewPriority))
                                            {
                                                Sprite image = contextMenu._image;
                                                if (image != null)
                                                {
                                                    image.depth = this.depth + 3;
                                                    image.x = vec2_1.x + 1f;
                                                    image.y = vec2_1.y;
                                                    image.scale = new Vec2(0.5f);
                                                    image.Draw();
                                                    vec2_1.x += 8f;
                                                    ++num4;
                                                    switch (num4)
                                                    {
                                                        case 2:
                                                            vec2_1.x -= 16f;
                                                            vec2_1.y += 8f;
                                                            break;
                                                        case 4:
                                                            goto label_31;
                                                    }
                                                }
                                            }
                                            if (index1 != 1)
                                                break;
                                        }
                                    }
                                    else
                                        break;
                                }
                            }
                        }
                    }
                    else
                        Graphics.DrawString(this._text, this.position + new Vec2(2f, 4f), Color.White * num2, this.depth + 1);
                    if (this._items.Count > 0)
                    {
                        this._contextArrow.color = Color.White * num2;
                        Graphics.Draw(this._contextArrow, (this.x + itemSize.x - 8f), this.y + 4f, this.depth + 1);
                        this._contextArrow.color = Color.White;
                    }
                }
            }
            if (this.opened)
            {
                if (!this.pinOpened)
                {
                    this.UpdatePositioning();
                    float x = this.menuSize.x;
                    float y = this.menuSize.y;
                    Vec2 p1_1 = new Vec2(this.x + this._openedOffsetX, this.y + this._openedOffset) + new Vec2(-2f, -2f);
                    if (!this._root && !this.dontPush)
                        p1_1.x += this.itemSize.x + 6f;
                    if (this._showBackground)
                    {
                        Graphics.DrawRect(p1_1, p1_1 + new Vec2(x, y), new Color(70, 70, 70) * this.alpha, this.depth);
                        Graphics.DrawRect(p1_1 + new Vec2(1f, 1f), p1_1 + new Vec2(x - 1f, y - 1f), new Color(30, 30, 30) * this.alpha, this.depth + 1);
                        this._lastDrawPos = p1_1;
                        if (this._items.Count > 0 && this.isPinnable && (!this._root || this.pinned))
                        {
                            Sprite g = this._pin;
                            if (this.pinned && Editor.pretendPinned != this || this._pinned)
                                g = this._pinPinned;
                            g.depth = this.depth + 2;
                            if (this._hoverPin)
                                g.alpha = 1f;
                            else
                                g.alpha = 0.5f;
                            Vec2 vec2_2 = new Vec2((p1_1.x + x - 5f), p1_1.y - 4f);
                            Graphics.Draw(g, vec2_2.x, vec2_2.y);
                            Vec2 p1_2 = vec2_2 + new Vec2(-6f, -6f);
                            Vec2 p2_1 = p1_2 + new Vec2(11f, 11f);
                            if (Editor.inputMode == EditorInput.Gamepad && this._takingInput)
                                p1_2.x -= 10f;
                            Graphics.DrawRect(p1_2, p2_1, new Color(70, 70, 70) * this.alpha, this.depth);
                            Graphics.DrawRect(p1_2 + new Vec2(1f, 1f), p2_1 + new Vec2(-1f, 0f), new Color(30, 30, 30) * this.alpha, this.depth + 1);
                            if (this._owner != null && this._owner is ContextMenu && (this._owner as ContextMenu).pinOpened)
                            {
                                Vec2 vec2_3 = new Vec2(12f, 12f);
                                if (Editor.bigInterfaceMode)
                                    vec2_3 = new Vec2(24f, 24f);
                                Vec2 p1_3 = p1_1 + new Vec2((float)-(vec2_3.x + 2.0), 0f);
                                Vec2 p2_2 = p1_1 + new Vec2(-2f, vec2_3.y);
                                Graphics.DrawRect(p1_3, p2_2, new Color(70, 70, 70) * this.alpha, this.depth);
                                Graphics.DrawRect(p1_3 + new Vec2(1f, 1f), p2_2 + new Vec2(-1f, -1f), new Color(30, 30, 30) * this.alpha, this.depth + 1);
                                this._contextArrow.flipH = true;
                                this._contextArrow.depth = this.depth + 2;
                                if (this._hoverBackArrow)
                                    this._contextArrow.alpha = 1f;
                                else
                                    this._contextArrow.alpha = 0.5f;
                                this._contextArrow.alpha = 1f;
                                Graphics.Draw(this._contextArrow, p1_3.x + vec2_3.x / 2f + this._contextArrow.width / 2, p1_3.y + vec2_3.y / 2f - this._contextArrow.height / 2);
                                this._contextArrow.flipH = false;
                            }
                        }
                    }
                    if (Editor.inputMode == EditorInput.Gamepad && this.drawControls && this._takingInput)
                    {
                        string text = "";
                        bool flag = false;
                        foreach (ContextMenu contextMenu in this._items)
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
                        if (!this._root && !flag)
                            text += "  @LEFT@BACK";
                        Graphics.DrawRect(p1_1 + new Vec2(0f, y), p1_1 + new Vec2(x, y + 15f), Color.Black * this.alpha, this.depth);
                        Graphics.DrawString(text, p1_1 + new Vec2(0f, y + 4f), Color.White * this.alpha, this.depth + 1);
                        if (this.isPinnable && (!this._root || this.pinned))
                        {
                            Graphics._biosFont.spriteScale = new Vec2(0.75f);
                            Graphics.DrawString("@MENU1@", p1_1 + new Vec2(this.menuSize.x - 20f, -7f), Color.White * this.alpha, this.depth + 4, scale: 0.5f);
                            Graphics._biosFont.spriteScale = new Vec2(1f);
                        }
                    }
                    if (this._hasToproot && !this.dontPush)
                        Graphics.DrawRect(this._toprootPosition, this._toprootPosition + new Vec2(16f, 32f), new Color(70, 70, 70) * this.alpha, this.depth - 4);
                }
                int num = 0;
                foreach (ContextMenu contextMenu in this._items)
                {
                    if (!this.pinOpened || contextMenu.opened)
                    {
                        contextMenu.scrollButtonDirection = 0;
                        if (num == this._drawIndex && this._drawIndex > 0)
                            contextMenu.scrollButtonDirection = -1;
                        else if ((num - this._drawIndex == this._maxNumToDraw - 1 || this._alwaysDrawLast && num - this._drawIndex == this._maxNumToDraw - 2) && this._items.Count - (this._alwaysDrawLast ? 2 : 1) > num)
                            contextMenu.scrollButtonDirection = 1;
                        if (this._alwaysDrawLast)
                        {
                            if (num == this._items.Count - 1)
                                contextMenu.scrollButtonDirection = 0;
                            if ((num == this._items.Count - 1 || num >= this._drawIndex && num < this._drawIndex + this._maxNumToDraw && (num == this._items.Count - 1 || num != this._drawIndex + this._maxNumToDraw - 1)) && contextMenu.visible)
                                contextMenu.DoDraw();
                        }
                        else if (num >= this._drawIndex && num - this._drawIndex < this._maxNumToDraw && contextMenu.visible)
                            contextMenu.DoDraw();
                    }
                    ++num;
                }
            }
            this.position -= this.offset;
        }

        public virtual void Selected()
        {
            if (this.greyOut || this._owner == null)
                return;
            this._owner.Selected(this);
        }

        public bool IsEditorPlacementMenu() => true;

        public virtual void Selected(ContextMenu item)
        {
            if (this.greyOut)
                return;
            if (item != null && item.scrollButtonDirection != 0)
            {
                this._drawIndex += item.scrollButtonDirection;
                this._drawIndex = Maths.Clamp(this._drawIndex, 0, this._items.Count - this._maxNumToDraw);
                SFX.Play("highClick", 0.3f, 0.2f);
                foreach (ContextMenu contextMenu in this._items)
                {
                    contextMenu.opened = false;
                    contextMenu._hover = false;
                }
                this.PositionItems();
            }
            else
            {
                foreach (ContextMenu contextMenu in this._items)
                {
                    if (contextMenu != item)
                        contextMenu.opened = false;
                }
                if (item != null)
                {
                    if (this.IsEditorPlacementMenu() && Editor.bigInterfaceMode && item is EditorGroupMenu || item.text == "More...")
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
                this.waitInputFrame = true;
            }
        }

        public void AddItem(ContextMenu item) => this.AddItem(item, -1);

        public void AddItem(ContextMenu item, int index)
        {
            item.Initialize();
            if (index >= 0)
                this._items.Insert(index, item);
            else
                this._items.Add(item);
            item.owner = this;
            this.PositionItems();
        }

        public Thing GetPlacementType(System.Type pType)
        {
            if (this is ContextObject && (this as ContextObject).thing.GetType() == pType)
                return (this as ContextObject).thing;
            foreach (ContextMenu contextMenu in this._items)
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
            return source == target ? 1f : 1f - this.ComputeLevenshteinDistance(source, target) / Math.Max(source.Length, target.Length);
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

        public List<ContextMenu.SearchPair> Search(string pTerm) => this.Search(pTerm.ToLowerInvariant(), new List<ContextMenu.SearchPair>()).OrderBy<ContextMenu.SearchPair, double>(x => -x.relevance).ToList<ContextMenu.SearchPair>();

        private List<ContextMenu.SearchPair> Search(
          string pTerm,
          List<ContextMenu.SearchPair> pCurrentTerms)
        {
            if (this is ContextObject)
            {
                string lowerInvariant = this._text.ToLowerInvariant();
                double similarity = this.CalculateSimilarity(pTerm, lowerInvariant);
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
                    pCurrentTerms.Add(new ContextMenu.SearchPair()
                    {
                        relevance = similarity,
                        thing = this as ContextObject
                    });
            }
            foreach (ContextMenu contextMenu in this._items)
                contextMenu.Search(pTerm, pCurrentTerms);
            return pCurrentTerms;
        }

        public void PositionItems()
        {
            float num1 = 0f;
            float num2 = this.y + this._openedOffset;
            this._openedOffsetX = 0f;
            if (Editor.inputMode != EditorInput.Mouse && !this._root)
            {
                num2 = 16f;
                this._openedOffset = (-this.y + 16f);
            }
            for (int index = 0; index < this._items.Count; ++index)
            {
                ContextMenu contextMenu = this._items[index];
                if (!contextMenu.opened || contextMenu is ContextToolbarItem && (contextMenu as ContextToolbarItem).isPushingUp)
                {
                    if (!this._root && !this.dontPush)
                        contextMenu.x = (this.x + 3f + itemSize.x + 3f);
                    else
                        contextMenu.x = this.x;
                    if ((this._pinned || Editor.pretendPinned == this) && !this._root)
                    {
                        if (Editor.bigInterfaceMode)
                        {
                            contextMenu.x += 14f;
                            this._openedOffsetX = 14f;
                        }
                        else
                        {
                            contextMenu.x += 4f;
                            this._openedOffsetX = 4f;
                        }
                    }
                    contextMenu.y = num2;
                }
                if (index >= this._drawIndex && !this.pinOpened && (this._alwaysDrawLast && (index == this._items.Count - 1 || index != this._drawIndex + this._maxNumToDraw - 1 && index < this._drawIndex + this._maxNumToDraw) || !this._alwaysDrawLast))
                    num2 += contextMenu.itemSize.y + 1f;
                if (contextMenu.itemSize.x < 107f)
                    contextMenu.itemSize.x = 107f;
                if (contextMenu.itemSize.x + 4.0 > menuSize.x)
                    this.menuSize.x = contextMenu.itemSize.x + 4f;
                contextMenu.depth = this.depth + 2;
                if (contextMenu.itemSize.x > num1)
                    num1 = contextMenu.itemSize.x;
            }
            int num3 = 0;
            float num4 = 0f;
            foreach (ContextMenu contextMenu in this._items)
            {
                if (num3 < this._maxNumToDraw)
                    num4 += contextMenu.itemSize.y + 1f;
                contextMenu.itemSize.x = num1;
                ++num3;
            }
            this.menuSize.y = num4 + 3f;
        }

        public void CloseMenus()
        {
            foreach (ContextMenu contextMenu in this._items)
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
