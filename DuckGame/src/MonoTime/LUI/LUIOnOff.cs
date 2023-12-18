namespace DuckGame
{
    public class LUIOnOff : LUIText
    {
        private FieldBinding _field;
        private FieldBinding _filterBinding;

        public LUIOnOff(float wide, float high, FieldBinding field, FieldBinding filterBinding)
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