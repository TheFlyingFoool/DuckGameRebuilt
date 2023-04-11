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
            int val = (int)_field.value;
            string drawText = "";
            if (_compressed && val < _captions.Count)
            {
                drawText = _captions[val];
            }
            else
            {
                int num = 0;
                foreach (string caption in _captions)
                {
                    if (num != 0)
                        drawText += " ";
                    drawText = num != val ? drawText + "|GRAY|" : drawText + "|WHITE|";
                    drawText += caption;
                    ++num;
                }
            }

            text = drawText;
            base.Draw();
        }
    }
}
