// Decompiled with JetBrains decompiler
// Type: DuckGame.UIOnOff
//removed for regex reasons Culture=neutral, PublicKeyToken=null
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
            Vec2 alignOffset = calcAlignOffset();
            bool flag = (bool)_field.value;
            if (_filterBinding != null)
            {
                if (!(bool)_filterBinding.value)
                    _font.Draw("   ANY", x + alignOffset.x, y + alignOffset.y, Color.White, depth);
                else if (flag)
                    _font.Draw("    ON", x + alignOffset.x, y + alignOffset.y, Color.White, depth);
                else
                    _font.Draw("   OFF", x + alignOffset.x, y + alignOffset.y, Color.White, depth);
            }
            else
            {
                _font.Draw("ON", x + alignOffset.x, y + alignOffset.y, flag ? Color.White : new Color(70, 70, 70), depth);
                _font.Draw("   OFF", x + alignOffset.x, y + alignOffset.y, !flag ? Color.White : new Color(70, 70, 70), depth);
            }
        }
    }
}
