namespace DuckGame
{
    public class LUINumber : LUIText
    {
        private FieldBinding _field;
        private string _append = "";
        private FieldBinding _filterField;
        private MatchSetting _setting;

        public LUINumber(
          float wide,
          float high,
          FieldBinding field,
          string append = "",
          FieldBinding filterField = null,
          MatchSetting setting = null)
          : base("0", Color.White)
        {
            _setting = setting;
            _field = field;
            _append = append;
            _filterField = filterField;
        }

        public override void Draw()
        {
            if (_field != null)
            {
                string str = "";
                if (_setting != null && _filterField != null)
                {
                    if (_setting.filterMode == FilterMode.GreaterThan)
                        str = ">=";
                    else if (_setting.filterMode == FilterMode.LessThan)
                        str = "<=";
                }
                if (_setting != null && _field.value is int && (int)_field.value == _setting.min && _setting.minString != null)
                {
                    _text = _setting.minString;
                }
                else
                {
                    _text = str + Change.ToString((int)_field.value) + _append;
                    if (_filterField != null && !(bool)_filterField.value)
                        _text = Triggers.Any;
                }
            }
            base.Draw();
        }
    }
}
