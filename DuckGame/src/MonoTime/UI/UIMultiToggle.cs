// Decompiled with JetBrains decompiler
// Type: DuckGame.UIMultiToggle
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System.Collections.Generic;

namespace DuckGame
{
    public class UIMultiToggle : UIText
    {
        private FieldBinding _field;
        private List<string> _captions;
        private bool _compressed;

        public void SetFieldBinding(FieldBinding f) => _field = f;

        public UIMultiToggle(
          float wide,
          float high,
          FieldBinding field,
          List<string> captions,
          bool compressed = false)
          : base("AAAAAAAAA", Color.White)
        {
            _field = field;
            _captions = captions;
            _compressed = compressed;
        }

        public override void Draw()
        {
            _font.scale = this.scale;
            _font.alpha = alpha;
            int index = (int)_field.value;
            string text = "";
            if (_compressed && index < _captions.Count)
            {
                text = _captions[index];
            }
            else
            {
                int num = 0;
                foreach (string caption in _captions)
                {
                    if (num != 0)
                        text += " ";
                    text = num != index ? text + "|GRAY|" : text + "|WHITE|";
                    text += caption;
                    ++num;
                }
            }
            Vec2 scale = _font.scale;
            if (specialScale != 0.0)
                _font.scale = new Vec2(specialScale);
            float width = _font.GetWidth(text);
            float num1 = (align & UIAlign.Left) <= UIAlign.Center ? ((align & UIAlign.Right) <= UIAlign.Center ? (float)(-width / 2.0) : this.width / 2f - width) : (float)-(this.width / 2.0);
            float num2 = (align & UIAlign.Top) <= UIAlign.Center ? ((align & UIAlign.Bottom) <= UIAlign.Center ? (float)(-_font.height / 2.0) : height / 2f - _font.height) : (float)-(height / 2.0);
            _font.Draw(text, x + num1, y + num2, Color.White, depth);
            _font.scale = scale;
        }
    }
}
