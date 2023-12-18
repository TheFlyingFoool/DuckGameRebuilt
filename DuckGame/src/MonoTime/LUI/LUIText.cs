using System;

namespace DuckGame
{
    public class LUIText : UIComponent
    {
        protected Color _color;
        public BitmapFont _font;
        protected string _text;
        protected Func<string> _textFunc;
        public int minLength;
        private float _heightAdd;
        public InputProfile _controlProfile;
        public float specialScale;
        protected Interp TextLerp = new Interp(true);

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
                    while (_font.GetLength(_text) < minLength)
                        _text = " " + _text;
                }
                _font.scale = new Vec2(1f);
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

        public LUIText(
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

        public LUIText(
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

        protected Vec2 calcAlignOffset()
        {
            float xOffset;
            float yOffset;
            if ((align & UIAlign.Left) > UIAlign.Center)
                xOffset = -parent.width / 2f;
            else if ((align & UIAlign.Right) > UIAlign.Center)
                xOffset = parent.width / 2f - width;
            else
                xOffset = -width / 2f;
            if ((align & UIAlign.Top) > UIAlign.Center)
                yOffset = -parent.height / 2f;
            else if ((align & UIAlign.Bottom) > UIAlign.Center)
                yOffset = parent.height / 2f - _font.height;
            else
                yOffset = -_font.height / 2f;
            return new Vec2(xOffset, yOffset);
        }

        public override void Draw()
        {
            this.x = parent.x;
            _font.scale = new Vec2(1f, 1f);
            _collisionSize.x = _font.GetWidth(_text);

            _font.scale = scale;
            _font.alpha = alpha;

            TextLerp.UpdateLerpState(position, MonoMain.IntraTick, MonoMain.UpdateLerpState);
            float x = TextLerp.x;
            float y = TextLerp.y;

            Vec2 alignOffset = calcAlignOffset();
            _font.Draw(text, x + alignOffset.x, y + alignOffset.y, UIMenu.disabledDraw ? Colors.BlueGray : _color, depth, _controlProfile);

            base.Draw();
        }
    }
}