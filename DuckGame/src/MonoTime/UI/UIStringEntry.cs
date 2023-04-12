// Decompiled with JetBrains decompiler
// Type: DuckGame.UIStringEntry
//removed for regex reasons Culture=neutral, PublicKeyToken=null
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

        public override void Update()
        {
            if (_text.Length > 10)
            {
                _text = _text.Substring(0, 8) + "..";
            }
            _collisionSize.x = _font.GetWidth(_text);
            base.Update();
        }

        public override void Draw()
        {
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
