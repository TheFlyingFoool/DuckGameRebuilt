// Decompiled with JetBrains decompiler
// Type: DuckGame.UINumber
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class UINumber : UIText
    {
        private FieldBinding _field;
        private string _append = "";
        private FieldBinding _filterField;
        private MatchSetting _setting;

        public UINumber(
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
