// Decompiled with JetBrains decompiler
// Type: DuckGame.UIStringEntry
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

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
            if (_directionalPassword && _text != "  NONE")
            {
                _collisionSize.x = 48f;
                float num = _text.Length * 8;
                Graphics.DrawPassword(_text, new Vec2(x + (((align & UIAlign.Left) <= UIAlign.Center ? ((align & UIAlign.Right) <= UIAlign.Center ? (float)(-num / 2.0) : width / 2f - num) : (float)-(width / 2.0)) - 8f), y + ((align & UIAlign.Top) <= UIAlign.Center ? ((align & UIAlign.Bottom) <= UIAlign.Center ? (float)(-_font.height / 2.0) : height / 2f - _font.height) : (float)-(height / 2.0))), _color, depth);
            }
            else
            {
                if (_text.Length > 10)
                    _text = _text.Substring(0, 8) + "..";
                _collisionSize.x = 48f;
                float num = _text.Length * 8;
                Graphics.DrawString(_text, new Vec2(x + (((align & UIAlign.Left) <= UIAlign.Center ? ((align & UIAlign.Right) <= UIAlign.Center ? (float)(-num / 2.0) : width / 2f - num) : (float)-(width / 2.0)) - 8f), y + ((align & UIAlign.Top) <= UIAlign.Center ? ((align & UIAlign.Bottom) <= UIAlign.Center ? (float)(-_font.height / 2.0) : height / 2f - _font.height) : (float)-(height / 2.0))), _color, depth);
            }
        }
    }
}
