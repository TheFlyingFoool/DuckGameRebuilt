// Decompiled with JetBrains decompiler
// Type: DuckGame.UIText
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections.Generic;
using System.Linq;

namespace DuckGame
{
    public class UIDGRDescribe : UIText
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

            if (uis.Count > sel) text = uis[sel].dgrDescription;
            else text = "";

            _font.scale = scale;
            _font.alpha = alpha;
            float width = _font.GetWidth(text);
            float num1 = (align & UIAlign.Left) <= UIAlign.Center ? ((align & UIAlign.Right) <= UIAlign.Center ? (float)(-width / 2.0) : this.width / 2f - width) : (float)-(this.width / 2.0);
            float num2 = (align & UIAlign.Top) <= UIAlign.Center ? ((align & UIAlign.Bottom) <= UIAlign.Center ? (float)(-_font.height / 2.0) : height / 2f - _font.height) : (float)-(height / 2.0);
            if (specialScale != 0.0)
            {
                Vec2 scale = _font.scale;
                _font.scale = new Vec2(specialScale);
                _font.Draw(text, x + num1, y + num2, UIMenu.disabledDraw ? Colors.BlueGray : _color, depth, _controlProfile);
                _font.scale = scale;
            }
            else
                _font.Draw(text, x + num1, y + num2, UIMenu.disabledDraw ? Colors.BlueGray : _color, depth, _controlProfile);
            base.Draw();
        }
    }
}
