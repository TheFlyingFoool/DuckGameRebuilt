namespace DuckGame
{
    public class LUIStringEntry : LUIText
    {
        private bool _directionalPassword;

        public LUIStringEntry(
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
            if (_text.Length > 10)
            {
                _text = _text.Substring(0, 8) + "..";
            }

            if (_directionalPassword && _text != "  NONE")
            {
                Vec2 alignOffset = calcAlignOffset();
                Graphics.DrawPassword(_text, new Vec2(x + alignOffset.x, y + alignOffset.y), _color, depth);
            }
            else
                base.Draw();
        }
    }
}