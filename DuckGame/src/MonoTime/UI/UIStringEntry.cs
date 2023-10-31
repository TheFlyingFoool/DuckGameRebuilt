namespace DuckGame
{
    public class UIStringEntry : UIText
    {
        private bool _directionalPassword;

        public UIStringEntry(
          bool directional,
          string textVal,
          Color c,
          UIAlign al = UIAlign.Center,
          float heightAdd = 0f,
          InputProfile controlProfile = null)
          : base(textVal, c, al, heightAdd, controlProfile)
        {
            _directionalPassword = directional;
        }

        public override void Draw()
        {
            UILerp.UpdateLerpState(position, MonoMain.IntraTick, MonoMain.UpdateLerpState);

            if (_directionalPassword && _text != "  NONE")
            {
                _collisionSize.x = 48f;
                float num = _text.Length * 8;
                Graphics.DrawPassword(_text, new Vec2(UILerp.x + (((align & UIAlign.Left) <= UIAlign.Center ? ((align & UIAlign.Right) <= UIAlign.Center ? (float)(-num / 2f) : width / 2f - num) : (float)-(width / 2f)) - 8f), UILerp.y + ((align & UIAlign.Top) <= UIAlign.Center ? ((align & UIAlign.Bottom) <= UIAlign.Center ? (float)(-_font.height / 2f) : height / 2f - _font.height) : (float)-(height / 2f))), _color, depth);
            }
            else
            {
                int textLength = _font.GetLength(_text);
                if (textLength > 10)
                {
                    _text = _font.Crop(_text, 0, 8) + "..";
                    _prevtext = text;
                    textLength = 10;
                }
                _collisionSize.x = 48f;
                float num = _font.GetWidth(_text);
                Graphics.DrawString(_text, new Vec2(UILerp.x + (((align & UIAlign.Left) <= UIAlign.Center ? ((align & UIAlign.Right) <= UIAlign.Center ? (float)(-num / 2f) : width / 2f - num) : (float)-(width / 2f)) - 8f), UILerp.y + ((align & UIAlign.Top) <= UIAlign.Center ? ((align & UIAlign.Bottom) <= UIAlign.Center ? (float)(-_font.height / 2f) : height / 2f - _font.height) : (float)-(height / 2f))), _color, depth);
            }
        }
    }
}