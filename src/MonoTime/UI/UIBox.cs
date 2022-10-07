// Decompiled with JetBrains decompiler
// Type: DuckGame.UIBox
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Linq;
using static DuckGame.CMD;

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

        public virtual void AssignDefaultSelection() => _defaultSelection = _components.Where<UIComponent>(val =>
       {
           if (!(val is UIMenuItem))
               return false;
           return val.condition == null || val.condition();
       }).ToList<UIComponent>().Count - 1;
        public UIMenu UIParentMenu;
        public override void Open()
        {
            UIComponent UIComponent = this.parent;
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
                    _selection = _components.Where<UIComponent>(val => val is UIMenuItem).ToList<UIComponent>().Count - 1;
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
                float num1 = 0f;
                float num2 = 0f;
                foreach (UIComponent component in _components)
                {
                    if (component.condition == null || component.condition())
                    {
                        num2 += component.collisionSize.y + _seperation;
                        if (component.collisionSize.x > num1)
                            num1 = component.collisionSize.x;
                    }
                }
                float num3 = num1 + borderSize.x * 2f;
                float num4 = num2 - _seperation + borderSize.y * 2f;
                if (_autoSizeHor && (fit & UIFit.Horizontal) == UIFit.None && num3 > _collisionSize.x)
                    _collisionSize.x = num3;
                if (_autoSizeVert && (fit & UIFit.Vertical) == UIFit.None && num4 > _collisionSize.y)
                    _collisionSize.y = num4;
                float num5 = (float)(-num4 / 2.0) + borderSize.y;
                foreach (UIComponent component in _components)
                {
                    if (component.condition == null || component.condition())
                    {
                        component.anchor.offset.x = 0f;
                        if ((component.align & UIAlign.Left) > UIAlign.Center)
                            component.anchor.offset.x = (float)(-collisionSize.x / 2.0 + borderSize.x + component.collisionSize.x / 2.0);
                        else if ((component.align & UIAlign.Right) > UIAlign.Center)
                            component.anchor.offset.x = (float)(collisionSize.x / 2.0 - borderSize.x - component.collisionSize.x / 2.0);
                        component.anchor.offset.y = (float)(num5 * scale.y + component.height / 2.0);
                        num5 += component.collisionSize.y + _seperation;
                    }
                }
            }
            else
            {
                float num6 = 0f;
                float num7 = 0f;
                foreach (UIComponent component in _components)
                {
                    if (component.condition == null || component.condition())
                    {
                        num6 += component.collisionSize.x + _seperation;
                        if (component.collisionSize.y > num7)
                            num7 = component.collisionSize.y;
                    }
                }
                float num8 = num7 + borderSize.y * 2f;
                float num9 = num6 - _seperation + borderSize.x * 2f;
                if (_autoSizeHor && (fit & UIFit.Horizontal) == UIFit.None && num9 > _collisionSize.x)
                    _collisionSize.x = num9;
                if (_autoSizeVert && (fit & UIFit.Vertical) == UIFit.None && num8 > _collisionSize.y)
                    _collisionSize.y = num8;
                float num10 = (float)(-num9 / 2.0) + borderSize.x;
                foreach (UIComponent component in _components)
                {
                    if (component.condition == null || component.condition())
                    {
                        component.anchor.offset.x = (float)(num10 * scale.x + component.width / 2.0);
                        component.anchor.offset.y = 0f;
                        num10 += component.collisionSize.x + _seperation;
                    }
                }
            }
        }

        public virtual void SelectLastMenuItem()
        {
            _selection = _components.Where<UIComponent>(val => val is UIMenuItem).ToList<UIComponent>().Count - 1;
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
            SFX.Play("textLetter", 0.7f);
        }
        public static bool dubberspeed;
        public override void Update()
        {
            if (UIParentMenu != null && UIParentMenu.domouse && !UIParentMenu.gamepadMode && _currentMenuItemSelection != null && Mouse.available)
            {
                for (int i = 0; i < _currentMenuItemSelection.Count; i++)
                {
                    UIComponent uIComponent = _currentMenuItemSelection[i];
                    Rectangle r = new Rectangle(uIComponent.position + new Vec2(-(this.width / 2f), uIComponent.height / 2f), uIComponent.position + new Vec2(-(this.width / 2f) + uIComponent.width, -(uIComponent.height / 2f)));
                    if (Collision.Point(Mouse.position, r))
                    {
                        if (!_animating && uIComponent is UIMenuItem)
                        {
                            UIMenuItem uIMenuItem = uIComponent as UIMenuItem;
                            //UIMenuItemSlider
                            if (uIMenuItem is UIMenuItemSlider)
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
                if (Input.Pressed("CANCEL") && allowBackButton)
                {
                    if (_backButton != null || _backFunction != null)
                    {
                        if (!_animating)
                        {
                            MonoMain.dontResetSelection = true;
                            if (_backButton != null)
                                _backButton.Activate("SELECT");
                            else
                                _backFunction.Activate();
                            MonoMain.menuOpenedThisFrame = true;
                        }
                    }
                    else if (!MonoMain.menuOpenedThisFrame && _isMenu)
                        MonoMain.closeMenus = true;
                }
                else if (Input.Pressed("SELECT") && _acceptFunction != null && !_animating)
                {
                    MonoMain.dontResetSelection = true;
                    _acceptFunction.Activate();
                    MonoMain.menuOpenedThisFrame = true;
                }
                if (_isMenu)
                {
                    if (dubberspeed && _currentMenuItemSelection != null)
                    {
                        Keys[] keysOfInterest =
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

                        int c = _currentMenuItemSelection.Count;
                        for (int i = 0; i < keysOfInterest.Length; i++)
                        {
                            if (Keyboard.Pressed(keysOfInterest[i]) && i < c)
                            {
                                SFX.Play("rockHitGround");
                                ((UIMenuItem)_currentMenuItemSelection[i]).Activate(Triggers.Select);
                            }
                        }
                    }

                    _currentMenuItemSelection = _components.Where<UIComponent>(val =>
                   {
                       if (!(val is UIMenuItem))
                           return false;
                       return val.condition == null || val.condition();
                   }).ToList<UIComponent>();
                    if (_vertical)
                    {
                        if (!_animating && Input.Pressed("MENUUP"))
                            SelectPrevious();
                        if (!_animating && Input.Pressed("MENUDOWN"))
                            SelectNext();
                    }
                    else
                    {
                        if (!_animating && Input.Pressed("MENULEFT"))
                            SelectPrevious();
                        if (!_animating && Input.Pressed("MENURIGHT"))
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
                                if (!_animating && Input.Pressed("SELECT"))
                                {
                                    uiMenuItem.Activate("SELECT");
                                    SFX.Play("rockHitGround", 0.7f);
                                }
                                else if (!_animating && Input.Pressed("MENU1"))
                                    uiMenuItem.Activate("MENU1");
                                else if (!_animating && Input.Pressed("MENU2"))
                                    uiMenuItem.Activate("MENU2");
                                else if (!_animating && Input.Pressed("RAGDOLL"))
                                    uiMenuItem.Activate("RAGDOLL");
                                else if (!_animating && Input.Pressed("STRAFE"))
                                    uiMenuItem.Activate("STRAFE");
                                else if (!_animating && Input.Pressed("MENULEFT"))
                                    uiMenuItem.Activate("MENULEFT");
                                else if (!_animating && Input.Pressed("MENURIGHT"))
                                    uiMenuItem.Activate("MENURIGHT");
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
                _sections.scale = scale;
                _sections.alpha = alpha;
                _sections.depth = depth;
                _sections.frame = 0;
                Graphics.Draw(_sections, -halfWidth + x, -halfHeight + y);
                _sections.frame = 2;
                Graphics.Draw(_sections, (float)(halfWidth + x - _sections.w * scale.x), -halfHeight + y);
                _sections.frame = 1;
                _sections.xscale = (_collisionSize.x - _sections.w * 2) / _sections.w * xscale;
                Graphics.Draw(_sections, (float)(-halfWidth + x + _sections.w * scale.x), -halfHeight + y);
                _sections.xscale = xscale;
                _sections.frame = 3;
                _sections.yscale = (_collisionSize.y - _sections.h * 2) / _sections.h * yscale;
                Graphics.Draw(_sections, -halfWidth + x, (float)(-halfHeight + y + _sections.h * scale.y));
                _sections.frame = 5;
                Graphics.Draw(_sections, (float)(halfWidth + x - _sections.w * scale.x), (float)(-halfHeight + y + _sections.h * scale.y));
                _sections.frame = 4;
                _sections.xscale = (_collisionSize.x - _sections.w * 2) / _sections.w * xscale;
                Graphics.Draw(_sections, (float)(-halfWidth + x + _sections.w * scale.x), (float)(-halfHeight + y + _sections.h * scale.y));
                _sections.xscale = xscale;
                _sections.yscale = yscale;
                _sections.frame = 6;
                Graphics.Draw(_sections, -halfWidth + x, (float)(halfHeight + y - _sections.h * scale.y));
                _sections.frame = 8;
                Graphics.Draw(_sections, (float)(halfWidth + x - _sections.w * scale.x), (float)(halfHeight + y - _sections.h * scale.y));
                _sections.frame = 7;
                _sections.xscale = (_collisionSize.x - _sections.w * 2) / _sections.w * xscale;
                Graphics.Draw(_sections, (float)(-halfWidth + x + _sections.w * scale.x), (float)(halfHeight + y - _sections.h * scale.y));
            }
            base.Draw();
        }
    }
}
