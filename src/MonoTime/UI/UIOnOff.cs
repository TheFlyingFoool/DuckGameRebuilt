// Decompiled with JetBrains decompiler
// Type: DuckGame.UIOnOff
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class UIOnOff : UIText
    {
        private FieldBinding _field;
        private FieldBinding _filterBinding;

        public UIOnOff(float wide, float high, FieldBinding field, FieldBinding filterBinding)
          : base("ON OFF", Color.White)
        {
            _field = field;
            _filterBinding = filterBinding;
        }

        public override void Draw()
        {
            _font.scale = scale;
            _font.alpha = alpha;
            float width = _font.GetWidth("ON OFF");
            float num1 = (align & UIAlign.Left) <= UIAlign.Center ? ((align & UIAlign.Right) <= UIAlign.Center ? (float)(-width / 2.0) : this.width / 2f - width) : (float)-(this.width / 2.0);
            float num2 = (align & UIAlign.Top) <= UIAlign.Center ? ((align & UIAlign.Bottom) <= UIAlign.Center ? (float)(-_font.height / 2.0) : height / 2f - _font.height) : (float)-(height / 2.0);
            bool flag = (bool)_field.value;
            if (_filterBinding != null)
            {
                if (!(bool)_filterBinding.value)
                    _font.Draw("   ANY", x + num1, y + num2, Color.White, depth);
                else if (flag)
                    _font.Draw("    ON", x + num1, y + num2, Color.White, depth);
                else
                    _font.Draw("   OFF", x + num1, y + num2, Color.White, depth);
            }
            else
            {
                _font.Draw("ON", x + num1, y + num2, flag ? Color.White : new Color(70, 70, 70), depth);
                _font.Draw("   OFF", x + num1, y + num2, !flag ? Color.White : new Color(70, 70, 70), depth);
            }
        }
    }
}
