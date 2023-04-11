// Decompiled with JetBrains decompiler
// Type: DuckGame.UIChangingText
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class UIChangingText : UIText
    {
        private FieldBinding _field;
        private FieldBinding _filterBinding;
        public string defaultSizeString = "ON OFF  ";

        public UIChangingText(float wide, float high, FieldBinding field, FieldBinding filterBinding)
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
