// Decompiled with JetBrains decompiler
// Type: DuckGame.ButtonImage
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class ButtonImage : Sprite
    {
        private BitmapFont _font;
        private Sprite _keySprite;
        private string _keyString;

        public ButtonImage(char key)
        {
            _font = new BitmapFont("tinyNumbers", 4, 5);
            _keySprite = new Sprite("buttons/genericButton");
            _keyString = ((int)key).ToString() ?? "";
            _texture = _keySprite.texture;
        }

        public override void Draw()
        {
            _keySprite.position = position;
            _keySprite.alpha = alpha;
            _keySprite.color = color;
            _keySprite.depth = depth;
            _keySprite.scale = scale;
            _keySprite.Draw();
            _font.scale = scale;
            _font.Draw(_keyString, position + new Vec2((float)(_keySprite.width * scale.x / 2f - _font.GetWidth(_keyString) / 2f), 4f * scale.y), new Color(20, 32, 34), depth + 2);
        }
    }
}
