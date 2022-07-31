// Decompiled with JetBrains decompiler
// Type: DuckGame.UIBox
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

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

        public int selection => this._selection;

        public UIBox(float xpos, float ypos, float wide = -1f, float high = -1f, bool vert = true, bool isVisible = true)
          : base(xpos, ypos, wide, high)
        {
            this._sections = new SpriteMap("uiBox", 10, 10);
            this._vertical = vert;
            this._borderVisible = isVisible;
            this.borderSize = this._borderVisible ? new Vec2(8f, 8f) : Vec2.Zero;
            this._canFit = true;
        }

        public UIBox(bool vert = true, bool isVisible = true)
          : base(0f, 0f, -1f, -1f)
        {
            this._sections = new SpriteMap("uiBox", 10, 10);
            this._vertical = vert;
            this._borderVisible = isVisible;
            this.borderSize = this._borderVisible ? new Vec2(8f, 8f) : Vec2.Zero;
            this._canFit = true;
        }

        public override void Add(UIComponent component, bool doAnchor = true)
        {
            if (component is UIMenuItem)
            {
                this._isMenu = true;
                if ((component as UIMenuItem).isBackButton)
                    this._backButton = component as UIMenuItem;
            }
            base.Add(component, doAnchor);
        }

        public override void Insert(UIComponent component, int position, bool doAnchor = true)
        {
            if (component is UIMenuItem)
            {
                this._isMenu = true;
                if ((component as UIMenuItem).isBackButton)
                    this._backButton = component as UIMenuItem;
            }
            base.Insert(component, position, doAnchor);
        }

        public virtual void AssignDefaultSelection() => this._defaultSelection = this._components.Where<UIComponent>(val =>
       {
           if (!(val is UIMenuItem))
               return false;
           return val.condition == null || val.condition();
       }).ToList<UIComponent>().Count - 1;

        public override void Open()
        {
            Graphics.fade = 1f;
            if (!MonoMain.dontResetSelection)
            {
                this._selection = this._defaultSelection;
                if (this._willSelectLast)
                    this._selection = this._components.Where<UIComponent>(val => val is UIMenuItem).ToList<UIComponent>().Count - 1;
            }
            base.Open();
        }

        protected override void SizeChildren()
        {
            foreach (UIComponent component in this._components)
            {
                if ((component.condition == null || component.condition()) && component.canFit)
                {
                    if (this.vertical)
                        component.collisionSize = new Vec2(this.collisionSize.x - this.borderSize.x * 2f, component.collisionSize.y);
                    else
                        component.collisionSize = new Vec2(component.collisionSize.x, this.collisionSize.y - this.borderSize.y * 2f);
                }
            }
        }

        protected override void OnResize()
        {
            if (this._vertical)
            {
                float num1 = 0f;
                float num2 = 0f;
                foreach (UIComponent component in this._components)
                {
                    if (component.condition == null || component.condition())
                    {
                        num2 += component.collisionSize.y + this._seperation;
                        if (component.collisionSize.x > (double)num1)
                            num1 = component.collisionSize.x;
                    }
                }
                float num3 = num1 + this.borderSize.x * 2f;
                float num4 = num2 - this._seperation + this.borderSize.y * 2f;
                if (this._autoSizeHor && (this.fit & UIFit.Horizontal) == UIFit.None && (double)num3 > _collisionSize.x)
                    this._collisionSize.x = num3;
                if (this._autoSizeVert && (this.fit & UIFit.Vertical) == UIFit.None && (double)num4 > _collisionSize.y)
                    this._collisionSize.y = num4;
                float num5 = (float)(-(double)num4 / 2.0) + this.borderSize.y;
                foreach (UIComponent component in this._components)
                {
                    if (component.condition == null || component.condition())
                    {
                        component.anchor.offset.x = 0f;
                        if ((component.align & UIAlign.Left) > UIAlign.Center)
                            component.anchor.offset.x = (float)(-(double)this.collisionSize.x / 2.0 + borderSize.x + component.collisionSize.x / 2.0);
                        else if ((component.align & UIAlign.Right) > UIAlign.Center)
                            component.anchor.offset.x = (float)(collisionSize.x / 2.0 - borderSize.x - component.collisionSize.x / 2.0);
                        component.anchor.offset.y = (float)((double)num5 * scale.y + (double)component.height / 2.0);
                        num5 += component.collisionSize.y + this._seperation;
                    }
                }
            }
            else
            {
                float num6 = 0f;
                float num7 = 0f;
                foreach (UIComponent component in this._components)
                {
                    if (component.condition == null || component.condition())
                    {
                        num6 += component.collisionSize.x + this._seperation;
                        if (component.collisionSize.y > (double)num7)
                            num7 = component.collisionSize.y;
                    }
                }
                float num8 = num7 + this.borderSize.y * 2f;
                float num9 = num6 - this._seperation + this.borderSize.x * 2f;
                if (this._autoSizeHor && (this.fit & UIFit.Horizontal) == UIFit.None && (double)num9 > _collisionSize.x)
                    this._collisionSize.x = num9;
                if (this._autoSizeVert && (this.fit & UIFit.Vertical) == UIFit.None && (double)num8 > _collisionSize.y)
                    this._collisionSize.y = num8;
                float num10 = (float)(-(double)num9 / 2.0) + this.borderSize.x;
                foreach (UIComponent component in this._components)
                {
                    if (component.condition == null || component.condition())
                    {
                        component.anchor.offset.x = (float)((double)num10 * scale.x + (double)component.width / 2.0);
                        component.anchor.offset.y = 0f;
                        num10 += component.collisionSize.x + this._seperation;
                    }
                }
            }
        }

        public virtual void SelectLastMenuItem()
        {
            this._selection = this._components.Where<UIComponent>(val => val is UIMenuItem).ToList<UIComponent>().Count - 1;
            this._willSelectLast = true;
        }

        private void SelectPrevious()
        {
            int selection = this._selection;
            do
            {
                --this._selection;
                if (this._selection < 0)
                    this._selection = this._currentMenuItemSelection.Count - 1;
            }
            while (this._currentMenuItemSelection[this._selection].mode != MenuItemMode.Normal && selection != this._selection);
            SFX.Play("textLetter", 0.7f);
        }

        private void SelectNext()
        {
            int selection = this._selection;
            do
            {
                ++this._selection;
                if (this._selection >= this._currentMenuItemSelection.Count)
                    this._selection = 0;
            }
            while (this._currentMenuItemSelection[this._selection].mode != MenuItemMode.Normal && selection != this._selection);
            SFX.Play("textLetter", 0.7f);
        }

        public override void Update()
        {
            if (!UIMenu.globalUILock && !this._close && !this._inputLock)
            {
                if (Input.Pressed("CANCEL") && this.allowBackButton)
                {
                    if (this._backButton != null || this._backFunction != null)
                    {
                        if (!this._animating)
                        {
                            MonoMain.dontResetSelection = true;
                            if (this._backButton != null)
                                this._backButton.Activate("SELECT");
                            else
                                this._backFunction.Activate();
                            MonoMain.menuOpenedThisFrame = true;
                        }
                    }
                    else if (!MonoMain.menuOpenedThisFrame && this._isMenu)
                        MonoMain.closeMenus = true;
                }
                else if (Input.Pressed("SELECT") && this._acceptFunction != null && !this._animating)
                {
                    MonoMain.dontResetSelection = true;
                    this._acceptFunction.Activate();
                    MonoMain.menuOpenedThisFrame = true;
                }
                if (this._isMenu)
                {
                    this._currentMenuItemSelection = this._components.Where<UIComponent>(val =>
                   {
                       if (!(val is UIMenuItem))
                           return false;
                       return val.condition == null || val.condition();
                   }).ToList<UIComponent>();
                    if (this._vertical)
                    {
                        if (!this._animating && Input.Pressed("MENUUP"))
                            this.SelectPrevious();
                        if (!this._animating && Input.Pressed("MENUDOWN"))
                            this.SelectNext();
                    }
                    else
                    {
                        if (!this._animating && Input.Pressed("MENULEFT"))
                            this.SelectPrevious();
                        if (!this._animating && Input.Pressed("MENURIGHT"))
                            this.SelectNext();
                    }
                    this._hoverControlString = null;
                    for (int index = 0; index < this._currentMenuItemSelection.Count; ++index)
                    {
                        UIMenuItem uiMenuItem = this._currentMenuItemSelection[index] as UIMenuItem;
                        uiMenuItem.selected = index == this._selection;
                        if (index == this._selection)
                        {
                            this._hoverControlString = uiMenuItem.controlString;
                            if (uiMenuItem.isEnabled)
                            {
                                if (!this._animating && Input.Pressed("SELECT"))
                                {
                                    uiMenuItem.Activate("SELECT");
                                    SFX.Play("rockHitGround", 0.7f);
                                }
                                else if (!this._animating && Input.Pressed("MENU1"))
                                    uiMenuItem.Activate("MENU1");
                                else if (!this._animating && Input.Pressed("MENU2"))
                                    uiMenuItem.Activate("MENU2");
                                else if (!this._animating && Input.Pressed("RAGDOLL"))
                                    uiMenuItem.Activate("RAGDOLL");
                                else if (!this._animating && Input.Pressed("STRAFE"))
                                    uiMenuItem.Activate("STRAFE");
                                else if (!this._animating && Input.Pressed("MENULEFT"))
                                    uiMenuItem.Activate("MENULEFT");
                                else if (!this._animating && Input.Pressed("MENURIGHT"))
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
            if (this._borderVisible)
            {
                this._sections.scale = this.scale;
                this._sections.alpha = this.alpha;
                this._sections.depth = this.depth;
                this._sections.frame = 0;
                Graphics.Draw(_sections, -this.halfWidth + this.x, -this.halfHeight + this.y);
                this._sections.frame = 2;
                Graphics.Draw(_sections, (float)((double)this.halfWidth + (double)this.x - _sections.w * (double)this.scale.x), -this.halfHeight + this.y);
                this._sections.frame = 1;
                this._sections.xscale = (this._collisionSize.x - this._sections.w * 2) / _sections.w * this.xscale;
                Graphics.Draw(_sections, (float)(-(double)this.halfWidth + (double)this.x + _sections.w * (double)this.scale.x), -this.halfHeight + this.y);
                this._sections.xscale = this.xscale;
                this._sections.frame = 3;
                this._sections.yscale = (this._collisionSize.y - this._sections.h * 2) / _sections.h * this.yscale;
                Graphics.Draw(_sections, -this.halfWidth + this.x, (float)(-(double)this.halfHeight + (double)this.y + _sections.h * (double)this.scale.y));
                this._sections.frame = 5;
                Graphics.Draw(_sections, (float)((double)this.halfWidth + (double)this.x - _sections.w * (double)this.scale.x), (float)(-(double)this.halfHeight + (double)this.y + _sections.h * (double)this.scale.y));
                this._sections.frame = 4;
                this._sections.xscale = (this._collisionSize.x - this._sections.w * 2) / _sections.w * this.xscale;
                Graphics.Draw(_sections, (float)(-(double)this.halfWidth + (double)this.x + _sections.w * (double)this.scale.x), (float)(-(double)this.halfHeight + (double)this.y + _sections.h * (double)this.scale.y));
                this._sections.xscale = this.xscale;
                this._sections.yscale = this.yscale;
                this._sections.frame = 6;
                Graphics.Draw(_sections, -this.halfWidth + this.x, (float)((double)this.halfHeight + (double)this.y - _sections.h * (double)this.scale.y));
                this._sections.frame = 8;
                Graphics.Draw(_sections, (float)((double)this.halfWidth + (double)this.x - _sections.w * (double)this.scale.x), (float)((double)this.halfHeight + (double)this.y - _sections.h * (double)this.scale.y));
                this._sections.frame = 7;
                this._sections.xscale = (this._collisionSize.x - this._sections.w * 2) / _sections.w * this.xscale;
                Graphics.Draw(_sections, (float)(-(double)this.halfWidth + (double)this.x + _sections.w * (double)this.scale.x), (float)((double)this.halfHeight + (double)this.y - _sections.h * (double)this.scale.y));
            }
            base.Draw();
        }
    }
}
