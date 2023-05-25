// Decompiled with JetBrains decompiler
// Type: DuckGame.UIText
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;

namespace DuckGame
{
    public class UIText : UIComponent
    {
        protected Color _color;
        public BitmapFont _font;
        protected string _text;
        protected Func<string> _textFunc;
        public int minLength;
        private float _heightAdd;
        public InputProfile _controlProfile;
        public float specialScale;

        public virtual string text
        {
            get
            {
                if (_textFunc != null)
                    text = _textFunc();
                return _text;
            }
            set
            {
                _text = value;
                if (minLength > 0)
                {
                    while (_text.Length < minLength)
                        _text = " " + _text;
                }
                _collisionSize = new Vec2(_font.GetWidth(_text), _font.height + _heightAdd);
            }
        }

        public void SetFont(BitmapFont f)
        {
            if (f != null)
                _font = f;
            _collisionSize = new Vec2(_font.GetWidth(text), _font.height + _heightAdd);
        }

        public float scaleVal
        {
            set => _font.scale = new Vec2(value);
        }

        public UIText(
          string textVal,
          Color c,
          UIAlign al = UIAlign.Center,
          float heightAdd = 0f,
          InputProfile controlProfile = null)
          : base(0f, 0f, 0f, 0f)
        {
            textVal = LangHandler.Convert(textVal);
            _heightAdd = heightAdd;
            _font = new BitmapFont("biosFontUI", 8, 7);
            text = textVal;
            _color = c;
            align = al;
            _controlProfile = controlProfile;
        }

        public UIText(
          Func<string> textFunc,
          Color c,
          UIAlign al = UIAlign.Center,
          float heightAdd = 0f,
          InputProfile controlProfile = null)
          : base(0f, 0f, 0f, 0f)
        {
            _heightAdd = heightAdd;
            _font = new BitmapFont("biosFontUI", 8, 7);
            _textFunc = textFunc;
            text = _textFunc();
            _color = c;
            align = al;
            _controlProfile = controlProfile;
        }

        public override void Draw()
        {
            _font.scale = scale;
            _font.alpha = alpha;
            float width = _font.GetWidth(text);
            float num1 = (align & UIAlign.Left) <= UIAlign.Center ? ((align & UIAlign.Right) <= UIAlign.Center ? (float)(-width / 2f) : this.width / 2f - width) : (float)-(this.width / 2f);
            float num2 = (align & UIAlign.Top) <= UIAlign.Center ? ((align & UIAlign.Bottom) <= UIAlign.Center ? (float)(-_font.height / 2f) : height / 2f - _font.height) : (float)-(height / 2f);
            if (specialScale != 0.0)
            {
                Vec2 scale = _font.scale;
                _font.scale = new Vec2(specialScale);
                _font.Draw(text, x + num1, y + num2, UIMenu.disabledDraw ? Colors.BlueGray : _color, depth, _controlProfile);
                _font.scale = scale;
            }
            else
                _font.Draw(text, x + num1, y + num2, UIMenu.disabledDraw ? Colors.BlueGray : _color, depth, _controlProfile);
            base.Draw();
        }
    }
}
