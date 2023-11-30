using System;
using System.Collections.Generic;
using System.Linq;

namespace DuckGame
{
    public class UIDGRDescribe : LUIText
    {
        public UIDGRDescribe(Color c) : base("", c)
        {
        }

        public UIDGRDescribe(Func<string> textFunc, Color c) : base(textFunc, c) 
        {
        }

        public override void Draw()
        {
            int sel = ((UIBox)_parent).selection;

            List<UIComponent> uis = _parent.components.Where(t => t.dgrDescription != "").ToList();

            if (uis.Count > sel)
                text = uis[sel].dgrDescription;
            else
                text = "";


            x = parent.x;
            _font.scale = new Vec2(1f, 1f);
            _collisionSize.x = _font.GetWidth(_text);

            _font.scale = scale;
            _font.alpha = alpha;
            _font.ySpacing = 0.5f;
            UILerp.UpdateLerpState(position, MonoMain.IntraTick, MonoMain.UpdateLerpState);

            Vec2 alignOffset = calcAlignOffset();
            _font.Draw(text, UILerp.x + alignOffset.x, UILerp.y + alignOffset.y, UIMenu.disabledDraw ? Colors.BlueGray : _color, depth, _controlProfile);

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
    }
}