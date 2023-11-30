using System.Collections.Generic;

namespace DuckGame
{
    public class LUIMultiToggle : LUIText
    {
        private FieldBinding _field;
        private List<string> _captions;
        private bool _compressed;

        public void SetFieldBinding(FieldBinding f) => _field = f;

        public LUIMultiToggle(
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