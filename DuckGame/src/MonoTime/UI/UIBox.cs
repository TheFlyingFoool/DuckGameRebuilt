using System.Collections.Generic;
using System.Linq;

namespace DuckGame
{
    public class UIBox : UIComponent
    {
        private SpriteMap _sections;
        private float _seperation = 1f;
        public string _hoverControlString;
        private bool _borderVisible = true;
        protected int _selection;
        public bool _isMenu;
        public UIMenuItem _backButton;
        public int _defaultSelection;
        private bool _willSelectLast;
        private List<UIComponent> _currentMenuItemSelection;
        public bool allowBackButton = true;
        protected bool _inputLock;

        public int selection => _selection;

        public UIBox(float xpos, float ypos, float wide = -1f, float high = -1f, bool vert = true, bool isVisible = true)
          : base(xpos, ypos, wide, high)
        {
            _sections = new SpriteMap("uiBox", 10, 10);
            _vertical = vert;
            _borderVisible = isVisible;
            borderSize = _borderVisible ? new Vec2(8f, 8f) : Vec2.Zero;
            _canFit = true;
        }

        public UIBox(bool vert = true, bool isVisible = true)
          : base(0f, 0f, -1f, -1f)
        {
            _sections = new SpriteMap("uiBox", 10, 10);
            _vertical = vert;
            _borderVisible = isVisible;
            borderSize = _borderVisible ? new Vec2(8f, 8f) : Vec2.Zero;
            _canFit = true;
        }

        public override void Add(UIComponent component, bool doAnchor = true)
        {
            if (component is UIMenuItem)
            {
                _isMenu = true;
                if ((component as UIMenuItem).isBackButton)
                    _backButton = component as UIMenuItem;
            }
            base.Add(component, doAnchor);
        }

        public override void Insert(UIComponent component, int position, bool doAnchor = true)
        {
            if (component is UIMenuItem)
            {
                _isMenu = true;
                if ((component as UIMenuItem).isBackButton)
                    _backButton = component as UIMenuItem;
            }
            base.Insert(component, position, doAnchor);
        }

        public virtual void AssignDefaultSelection() => _defaultSelection = _components.Where(val =>
       {
           if (!(val is UIMenuItem))
               return false;
           return val.condition == null || val.condition();
       }).ToList().Count - 1;
        public UIMenu UIParentMenu;
        public override void Open()
        {
            UIComponent UIComponent = parent;
            while (UIComponent != null) // IMPROVEME idk man coded a system that pass down the main uimenu i guess
            {
                UIComponent = UIComponent.parent;
                if (UIComponent is UIMenu)
                {
                    UIParentMenu = UIComponent as UIMenu;
                }
            }
            Graphics.fade = 1f;
            if (!MonoMain.dontResetSelection)
            {
                _selection = _defaultSelection;
                if (_willSelectLast)
                    _selection = _components.Where(val => val is UIMenuItem).ToList().Count - 1;
            }
            base.Open();
        }

        protected override void SizeChildren()
        {
            foreach (UIComponent component in _components)
            {
                if ((component.condition == null || component.condition()) && component.canFit)
                {
                    if (vertical)
                        component.collisionSize = new Vec2(collisionSize.x - borderSize.x * 2f, component.collisionSize.y);
                    else
                        component.collisionSize = new Vec2(component.collisionSize.x, collisionSize.y - borderSize.y * 2f);
                }
            }
        }

        protected override void OnResize()
        {
            if (_vertical)
            {
                float wide = 0f;
                float high = 0f;
                foreach (UIComponent component in _components)
                {
                    if (component.condition == null || component.condition())
                    {
                        if (!component.ignoreSeperation)
                            high += component.collisionSize.y + _seperation;
                        if (component.collisionSize.x > wide)
                            wide = component.collisionSize.x;
                    }
                }
                float wide2 = wide + borderSize.x * 2f;
                float high2 = high - _seperation + borderSize.y * 2f;
                if (_autoSizeHor && (fit & UIFit.Horizontal) == UIFit.None && wide2 > _collisionSize.x)
                    _collisionSize.x = wide2;
                if (_autoSizeVert && (fit & UIFit.Vertical) == UIFit.None && high2 > _collisionSize.y)
                    _collisionSize.y = high2;
                float yDraw = (float)(-high2 / 2.0) + borderSize.y;
                foreach (UIComponent component in _components)
                {
                    if (component.condition == null || component.condition())
                    {
                        component.anchor.offset.x = 0f;
                        if ((component.align & UIAlign.Left) > UIAlign.Center)
                            component.anchor.offset.x = (float)(-collisionSize.x / 2.0 + borderSize.x + component.collisionSize.x / 2.0);
                        else if ((component.align & UIAlign.Right) > UIAlign.Center)
                            component.anchor.offset.x = (float)(collisionSize.x / 2.0 - borderSize.x - component.collisionSize.x / 2.0);
                        component.anchor.offset.y = (float)(yDraw * scale.y + component.height / 2.0);
                        if (!component.ignoreSeperation)
                            yDraw += component.collisionSize.y + _seperation;
                    }
                }
            }
            else
            {
                float wide = 0f;
                float high = 0f;
                foreach (UIComponent component in _components)
                {
                    if (component.condition == null || component.condition())
                    {
                        if (!component.ignoreSeperation)
                            wide += component.collisionSize.x + _seperation;
                        if (component.collisionSize.y > high)
                            high = component.collisionSize.y;
                    }
                }
                float wide2 = high + borderSize.y * 2f;
                float high2 = wide - _seperation + borderSize.x * 2f;
                if (_autoSizeHor && (fit & UIFit.Horizontal) == UIFit.None && high2 > _collisionSize.x)
                    _collisionSize.x = high2;
                if (_autoSizeVert && (fit & UIFit.Vertical) == UIFit.None && wide2 > _collisionSize.y)
                    _collisionSize.y = wide2;
                float xDraw = (float)(-high2 / 2.0) + borderSize.x;
                foreach (UIComponent component in _components)
                {
                    if (component.condition == null || component.condition())
                    {
                        component.anchor.offset.x = (float)(xDraw * scale.x + component.width / 2.0);
                        component.anchor.offset.y = 0f;
                        if (!component.ignoreSeperation)
                            xDraw += component.collisionSize.x + _seperation;
                    }
                }
            }
        }

        public virtual void SelectLastMenuItem()
        {
            _selection = _components.Where(val => val is UIMenuItem).ToList().Count - 1;
            _willSelectLast = true;
        }

        private void SelectPrevious()
        {
            int selection = _selection;
            do
            {
                --_selection;
                if (_selection < 0)
                    _selection = _currentMenuItemSelection.Count - 1;
            }
            while (_currentMenuItemSelection[_selection].mode != MenuItemMode.Normal && selection != _selection);
            SFX.DontSave = 1;
            SFX.Play("textLetter", 0.7f);
        }

        private void SelectNext()
        {
            int selection = _selection;
            do
            {
                ++_selection;
                if (_selection >= _currentMenuItemSelection.Count)
                    _selection = 0;
            }
            while (_currentMenuItemSelection[_selection].mode != MenuItemMode.Normal && selection != _selection);
            SFX.DontSave = 1;
            SFX.Play("textLetter", 0.7f);
        }
        public static Keys[] DubberKeys =
        {
            Keys.D1,
            Keys.D2,
            Keys.D3,
            Keys.D4,
            Keys.D5,
            Keys.D6,
            Keys.D7,
            Keys.D8,
            Keys.D9,
            Keys.D0
        };
        public override void Update()
        {
            if (UIParentMenu != null && UIParentMenu.domouse && !UIParentMenu.gamepadMode && _currentMenuItemSelection != null && Mouse.available)
            {
                for (int i = 0; i < _currentMenuItemSelection.Count; i++)
                {
                    UIComponent uIComponent = _currentMenuItemSelection[i];
                    Rectangle r = new Rectangle(uIComponent.position + new Vec2(-(width / 2f), uIComponent.height / 2f), uIComponent.position + new Vec2(-(width / 2f) + uIComponent.width, -(uIComponent.height / 2f)));
                    if (Collision.Point(Mouse.position, r))
                    {
                        if (!_animating && uIComponent is UIMenuItem)
                        {
                            UIMenuItem uIMenuItem = uIComponent as UIMenuItem;
                            //UIMenuItemSlider
                            if (uIMenuItem is UIMenuItemSlider || uIMenuItem is UIMenuItemNumber || uIMenuItem is UIMenuItemToggle)
                            {
                                if (Mouse.left == InputState.Pressed)
                                {
                                    uIMenuItem.Activate(Triggers.MenuRight);
                                    _selection = i;
                                    break;
                                }
                                else if (Mouse.right == InputState.Pressed)
                                {
                                    uIMenuItem.Activate(Triggers.MenuLeft);
                                    _selection = i;
                                    break;
                                }

                            }
                            else
                            {
                                if (Mouse.left == InputState.Pressed)
                                {
                                    uIMenuItem.Activate(Triggers.Select);
                                    _selection = i;
                                    break;
                                }
                            }
                            if (Mouse.prevScrollDown)
                            {
                                uIMenuItem.Activate(Triggers.MenuLeft);
                            }
                            else if (Mouse.prevScrollUp)
                            {
                                uIMenuItem.Activate(Triggers.MenuRight);

                            }

                        }
                        _selection = i;
                        break;
                    }
                }
            }
            if (!UIMenu.globalUILock && !_close && !_inputLock)
            {
                if (Input.Pressed(Triggers.Cancel) && allowBackButton)
                {
                    if (_backButton != null || _backFunction != null)
                    {
                        if (!_animating)
                        {
                            MonoMain.dontResetSelection = true;
                            if (_backButton != null)
                                _backButton.Activate(Triggers.Select);
                            else
                                _backFunction.Activate();
                            MonoMain.menuOpenedThisFrame = true;
                        }
                    }
                    else if (!MonoMain.menuOpenedThisFrame && _isMenu)
                        MonoMain.closeMenus = true;
                }
                else if (Input.Pressed(Triggers.Select) && _acceptFunction != null && !_animating)
                {
                    MonoMain.dontResetSelection = true;
                    _acceptFunction.Activate();
                    MonoMain.menuOpenedThisFrame = true;
                }
                if (_isMenu)
                {
                    if (DGRSettings.dubberspeed && _currentMenuItemSelection != null)
                    {
                        Main.SpecialCode = "DubberSpeed Logic I";
                        int c = _currentMenuItemSelection.Count;
                        int dubberOffset = -1;
                        if (Keyboard.Down(Keys.LeftShift)) dubberOffset = 0;
                        Main.SpecialCode = "DubberSpeed Logic II";
                        for (int i = 0; i < DubberKeys.Length; i++)
                        {
                            Main.SpecialCode = "DubberSpeed Logic III";
                            if (Keyboard.Pressed(DubberKeys[i]) && i < c)
                            {
                                //optimal -NiK0
                                Main.SpecialCode = "DubberSpeed Logic IV";
                                if (dubberOffset == -1) dubberOffset = _currentMenuItemSelection.FindAll(ui => ui is UIConnectionInfo).Count;
                                SFX.DontSave = 1;
                                SFX.Play("rockHitGround");
                                Main.SpecialCode = "DubberSpeed Logic V";
                                if (i + dubberOffset < c)
                                {
                                _selection = i + dubberOffset;
                                    ((UIMenuItem)_currentMenuItemSelection[i + dubberOffset]).Activate(Triggers.Select);
                                }
                            }
                        }
                        Main.SpecialCode = "DubberSpeed Logic VI";
                    }

                    _currentMenuItemSelection = _components.Where(val =>
                   {
                       if (!(val is UIMenuItem))
                           return false;
                       return val.condition == null || val.condition();
                   }).ToList();
                    if (_vertical)
                    {
                        if (!_animating && Input.Pressed(Triggers.MenuUp))
                            SelectPrevious();
                        if (!_animating && Input.Pressed(Triggers.MenuDown))
                            SelectNext();
                    }
                    else
                    {
                        if (!_animating && Input.Pressed(Triggers.MenuLeft))
                            SelectPrevious();
                        if (!_animating && Input.Pressed(Triggers.MenuRight))
                            SelectNext();
                    }
                    _hoverControlString = null;
                    for (int index = 0; index < _currentMenuItemSelection.Count; ++index)
                    {
                        UIMenuItem uiMenuItem = _currentMenuItemSelection[index] as UIMenuItem;
                        uiMenuItem.selected = index == _selection;
                        if (index == _selection)
                        {
                            _hoverControlString = uiMenuItem.controlString;
                            if (uiMenuItem.isEnabled)
                            {
                                if (!_animating && Input.Pressed(Triggers.Select))
                                {
                                    uiMenuItem.Activate(Triggers.Select);
                                    SFX.DontSave = 1;
                                    SFX.Play("rockHitGround", 0.7f);
                                }
                                else if (!_animating && Input.Pressed(Triggers.Menu1))
                                    uiMenuItem.Activate(Triggers.Menu1);
                                else if (!_animating && Input.Pressed(Triggers.Menu2))
                                    uiMenuItem.Activate(Triggers.Menu2);
                                else if (!_animating && Input.Pressed(Triggers.Ragdoll))
                                    uiMenuItem.Activate(Triggers.Ragdoll);
                                else if (!_animating && Input.Pressed(Triggers.Strafe))
                                    uiMenuItem.Activate(Triggers.Strafe);
                                else if (!_animating && Input.Pressed(Triggers.MenuLeft))
                                    uiMenuItem.Activate(Triggers.MenuLeft);
                                else if (!_animating && Input.Pressed(Triggers.MenuRight))
                                    uiMenuItem.Activate(Triggers.MenuRight);
                            }
                        }
                    }
                }
            }
            base.Update();
        }

        public override void Draw()
        {
            if (_borderVisible)
            {
                UILerp.UpdateLerpState(new Interp.InterpState(position, angle), MonoMain.IntraTick, MonoMain.UpdateLerpState);

                _sections.scale = scale;
                _sections.alpha = alpha;
                _sections.depth = depth;
                _sections.frame = 0;
                Graphics.Draw(_sections, -halfWidth + UILerp.x, -halfHeight + UILerp.y);
                _sections.frame = 2;
                Graphics.Draw(_sections, (float)(halfWidth + UILerp.x - _sections.w * scale.x), -halfHeight + UILerp.y);
                _sections.frame = 1;
                _sections.xscale = (_collisionSize.x - _sections.w * 2) / _sections.w * xscale;
                Graphics.Draw(_sections, (float)(-halfWidth + UILerp.x + _sections.w * scale.x), -halfHeight + UILerp.y);
                _sections.xscale = xscale;
                _sections.frame = 3;
                _sections.yscale = (_collisionSize.y - _sections.h * 2) / _sections.h * yscale;
                Graphics.Draw(_sections, -halfWidth + UILerp.x, (float)(-halfHeight + UILerp.y + _sections.h * scale.y));
                _sections.frame = 5;
                Graphics.Draw(_sections, (float)(halfWidth + UILerp.x - _sections.w * scale.x), (float)(-halfHeight + UILerp.y + _sections.h * scale.y));
                _sections.frame = 4;
                _sections.xscale = (_collisionSize.x - _sections.w * 2) / _sections.w * xscale;
                Graphics.Draw(_sections, (float)(-halfWidth + UILerp.x + _sections.w * scale.x), (float)(-halfHeight + UILerp.y + _sections.h * scale.y));
                _sections.xscale = xscale;
                _sections.yscale = yscale;
                _sections.frame = 6;
                Graphics.Draw(_sections, -halfWidth + UILerp.x, (float)(halfHeight + UILerp.y - _sections.h * scale.y));
                _sections.frame = 8;
                Graphics.Draw(_sections, (float)(halfWidth + UILerp.x - _sections.w * scale.x), (float)(halfHeight + UILerp.y - _sections.h * scale.y));
                _sections.frame = 7;
                _sections.xscale = (_collisionSize.x - _sections.w * 2) / _sections.w * xscale;
                Graphics.Draw(_sections, (float)(-halfWidth + UILerp.x + _sections.w * scale.x), (float)(halfHeight + UILerp.y - _sections.h * scale.y));
            }

            if (_isMenu && DGRSettings.dubberspeed && _currentMenuItemSelection != null)
            {
                bool isUIControlConfig = idStr == "cc";
                if (!isUIControlConfig)
                {
                    int dubberOffset = _currentMenuItemSelection.FindAll(ui => ui is UIConnectionInfo).Count;
                    if (Keyboard.Down(Keys.LeftShift))
                        dubberOffset = 0;

                    for (int i = dubberOffset, j = 0; i < _currentMenuItemSelection.Count; ++i, j = i - dubberOffset)
                    {
                        UIMenuItem item = (UIMenuItem)_currentMenuItemSelection[i];
                        int index = -1;
                        if (j == 9)
                            index = 0;
                        else if (j < 9)
                            index = j + 1;
                        float xAdjust = -1f;
                        if (!item.selected && index != -1)
                            Graphics.DrawString(index.ToString(), new Vec2(Layer.HUD.camera.width / 2f - parent.halfWidth - xAdjust, item.top - item.halfHeight), Color.White * 0.4f, 1);
                    }
                }
            }
            base.Draw();
        }
    }
}
