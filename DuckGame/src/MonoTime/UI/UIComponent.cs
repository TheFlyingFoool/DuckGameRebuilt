﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace DuckGame
{
    public class UIComponent : Thing
    {
        public MenuItemMode mode;
        public bool debug;
        public Func<bool> condition;
        protected Interp UILerp = new Interp(true);
        protected Interp AnimLerp = new Interp(true);

        public string idStr = null;

        public bool manualFormatting = false;
        public string dgrDescription
        {
            get
            {
                if (manualFormatting)
                    return _dgrDescription;
                
                if (_dgrDescription is null or {Length: 0})
                    return "";
                
                string[] lines = _dgrDescription.SplitByLength(40);
                int longest = lines.Max(x => x.Length);
                
                for (int i = 0; i < lines.Length; i++)
                {
                    string myLine = lines[i];
                    int myLength = myLine.Length;
                    
                    if (myLength < longest)
                    {
                        lines[i] = myLine.Insert(0, new string(' ', (longest - myLength) / 2));
                    }
                }
                
                return string.Join("\n", lines);
            }
            set => _dgrDescription = value;
        }

        protected UIComponent _parent;
        public bool isEnabled = true;
        protected bool _didResize;
        protected bool _dirty;
        protected Vec2 _offset = Vec2.Zero;
        protected bool _vertical;
        protected List<UIComponent> _components = new List<UIComponent>();
        protected bool _canFit;
        private UIFit _fit;
        private UIAlign _align;
        public Vec2 borderSize = Vec2.Zero;
        private bool _animate;
        protected bool _close;
        protected bool _animating;
        private Vec2 _startPosition;
        private bool _startInitialized;
        private bool _initialSizingComplete;
        protected bool _autoSizeVert;
        protected bool _autoSizeHor;
        public bool inWorld;
        public UIMenuAction _backFunction;
        public UIMenuAction _closeFunction;
        public UIMenuAction _acceptFunction;
        public UIMenuAction _openFunction;
        private bool _isPauseMenu;

        public bool ignoreSeperation = false;

        public UIComponent parent => _parent;

        public UIMenu rootMenu
        {
            get
            {
                if (this is UIMenu)
                    return this as UIMenu;
                return _parent != null ? _parent.rootMenu : null;
            }
        }
        public UIMenuAction backFunction
        { 
            get 
            { 
                return _backFunction; 
            } 
        }
        public bool dirty
        {
            get => _dirty;
            set => _dirty = value;
        }

        public Vec2 offset
        {
            get => _offset;
            set => _offset = value;
        }

        public bool vertical
        {
            get => _vertical;
            set => _vertical = value;
        }

        public IList<UIComponent> components => _components;

        public bool canFit => _canFit;

        public UIFit fit
        {
            get => _fit;
            set
            {
                if (_fit != value)
                    _dirty = true;
                _fit = value;
            }
        }

        public UIAlign align
        {
            get => _align;
            set
            {
                if (_align != value)
                    _dirty = true;
                _align = value;
            }
        }

        public bool animate => _animate;

        public bool open => !_close;

        public bool animating
        {
            get => _animating;
            set
            {
                foreach (UIComponent component in _components)
                    component.animating = value;
                _animating = value;
            }
        }

        public bool autoSizeVert => _autoSizeVert;

        public bool autoSizeHor => _autoSizeHor;

        public UIComponent(float xpos, float ypos, float wide, float high)
          : base(xpos, ypos)
        {
            _collisionSize = new Vec2(wide, high);
            layer = Layer.HUD;
            depth = (Depth)0f;
            _autoSizeHor = wide < 0.0;
            _autoSizeVert = high < 0.0;
        }

        public virtual void Open()
        {
            MonoMain.menuOpenedThisFrame = true;
            _close = false;
            animating = true;
            foreach (UIComponent component in _components)
            {
                if (component.anchor == this)
                    component.Open();
            }
            _initialSizingComplete = false;

            if (_openFunction != null)
                _openFunction.Activate();
        }

        public virtual void Close()
        {
            _close = true;
            animating = true;
            foreach (UIComponent component in _components)
                component.Close();
            if (!inWorld && rootMenu == this && !MonoMain.closeMenuUpdate.Contains(this))
                MonoMain.closeMenuUpdate.Add(this);
            OnClose();
        }

        public override void DoUpdate()
        {
            base.DoUpdate();
        }

        public virtual void OnClose()
        {
        }

        public override void Added(Level parent)
        {
            inWorld = true;
            base.Added(parent);
        }

        public virtual void UpdateParts()
        {
        }

        public bool isPauseMenu
        {
            get
            {
                if (_isPauseMenu)
                    return true;
                return _parent != null && _parent.isPauseMenu;
            }
            set => _isPauseMenu = value;
        }
        public bool reducedMovement;
        public int reducedMovementFrames; //jank jank jank jank jank
        public override void Update()
        {
            if (reducedMovementFrames > 0)
            {
                reducedMovementFrames--;
                reducedMovement = reducedMovementFrames > 0;
            }
            if (!_startInitialized)
            {
                _startInitialized = true;
                _startPosition = position;
                position.y = layer.camera.height * 2f;
            }
            if (!isAnchored)
            {
                float to = _close ? layer.camera.height * 2f : _startPosition.y;
                if (animating)
                    position.y = Lerp.FloatSmooth(position.y, to, 0.2f, 1.05f);
                bool flag = position.y != to;
                //stuff might look weird but i have to introduce a 1 frame delay cuz otherwise the open menu input can go through
                //multiple menus and do bad stuff -Lucky
                if (DGRSettings.ReducedMovement || reducedMovement) 
                {
                    if (position.y == to) animating = false;
                    position.y = to;
                }
                else if (animating != flag) animating = flag;
            }
            if (open && !animating)
                UpdateParts();
            if (_parent != null || open || animating)
            {
                SizeChildren();
                foreach (UIComponent component in _components)
                {
                    if (component != this && component.condition == null || component.condition())
                    {
                        component.DoUpdate();
                        if (component._didResize)
                            _dirty = true;
                        component._didResize = false;
                    }
                }
            }
            if (_dirty)
                Resize();
            if (!UIMenu.globalUILock && !MonoMain.menuOpenedThisFrame && MonoMain.pauseMenu == this && (Input.Pressed(Triggers.Start) && isPauseMenu || MonoMain.closeMenus))
            {
                MonoMain.closeMenus = false;
                if (_closeFunction != null)
                    _closeFunction.Activate();
                Close();
            }
            _dirty = false;
            _initialSizingComplete = true;
        }

        protected virtual void SizeChildren()
        {
        }

        public override void DoDraw()
        {
            if (!_initialSizingComplete || !_animating && _close)
                return;
            base.DoDraw();
        }
        public long lastframeupdate = -1;
        public long lastframedraw = -1;
        private string _dgrDescription = "";

        public override void Draw()
        {
            if (HUD.hide)
                return;
            foreach (UIComponent component in _components)
            {
                if (component.condition == null || component.condition())
                {
                    if (component is UIMenuItem)
                        UIMenu.disabledDraw = component.mode == MenuItemMode.Disabled;
                    component.depth = depth + 10;
                    if (component.visible && component.mode != MenuItemMode.Hidden)
                    {
                        component.Draw();
                    }
                    if (component is UIMenuItem)
                        UIMenu.disabledDraw = false;
                }
            }
            int num = debug ? 1 : 0;
        }

        public void Resize()
        {
            _dirty = false;
            _didResize = true;
            OnResize();
        }

        protected virtual void OnResize()
        {
        }

        public virtual void Add(UIComponent component, bool doAnchor = true)
        {
            _components.Add(component);
            component._parent = this;
            _dirty = true;
            component.dirty = true;
            if (!doAnchor)
                return;
            component.anchor = (Anchor)this;
        }

        public virtual void Insert(UIComponent component, int position, bool doAnchor = true)
        {
            if (position >= _components.Count)
                position = _components.Count;
            _components.Insert(position, component);
            component._parent = this;
            _dirty = true;
            component.dirty = true;
            if (!doAnchor)
                return;
            component.anchor = (Anchor)this;
        }

        public virtual void Remove(UIComponent component)
        {
            _components.Remove(component);
            if (component._parent == this)
                component._parent = null;
            if (component.anchor == this)
                component.anchor = null;
            _dirty = true;
        }
    }
}
