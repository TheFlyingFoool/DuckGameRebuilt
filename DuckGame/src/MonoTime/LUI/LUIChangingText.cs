namespace DuckGame
{
    public class LUIChangingText : LUIText
    {
        private FieldBinding _field;
        private FieldBinding _filterBinding;
        public string defaultSizeString = "ON OFF  ";

        public LUIChangingText(float wide, float high, FieldBinding field, FieldBinding filterBinding)
          : base("ON OFF  ", Color.White)
        {
            _field = field;
            _filterBinding = filterBinding;
        }

        public override string text
        {
            get => _text;
            set
            {
                _text = value;
                if (minLength <= 0)
                    return;
                while (_text.Length < minLength)
                    _text = " " + _text;
            }
        }

        public override void Draw()
        {
            base.Draw();
        }
    }
}