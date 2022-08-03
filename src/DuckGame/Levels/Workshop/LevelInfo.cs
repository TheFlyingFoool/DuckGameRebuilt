// Decompiled with JetBrains decompiler
// Type: DuckGame.LevelInfo
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class LevelInfo : Card
    {
        private string _name;
        private string _description;
        private Sprite _sprite;
        private Tex2D _image;
        protected bool _large;
        private const float largeCardWidth = 96f;
        private const float largeCardHeight = 62f;
        private const float smallCardWidth = 71f;
        private const float smallCardHeight = 48f;
        private static BitmapFont _font = new BitmapFont("biosFont", 8);
        public const float cardSpacing = 4f;

        public string name
        {
            get => _name;
            set => _name = value;
        }

        public string description
        {
            get => _description;
            set => _description = value;
        }

        public Tex2D image
        {
            get => _image;
            set
            {
                _image = value;
                _sprite = new Sprite(_image);
            }
        }

        public bool large
        {
            get => _large;
            set => _large = value;
        }

        public override float width => !_large ? 71f : 96f;

        public override float height => !_large ? 48f : 62f;

        public LevelInfo(bool large = true, string text = null)
        {
            _large = large;
            _specialText = text;
        }

        public override void Draw(Vec2 position, bool selected, float alpha)
        {
            Graphics.DrawRect(position, position + new Vec2(width, height), new Color(25, 38, 41) * alpha, (Depth)0.9f);
            if (selected)
                Graphics.DrawRect(position + new Vec2(-1f, 0f), position + new Vec2(width + 1f, height), Color.White * alpha, (Depth)0.97f, false);
            if (_specialText != null)
            {
                LevelInfo._font.scale = new Vec2(0.5f, 0.5f);
                LevelInfo._font.Draw(_specialText, (float)(position.x + width / 2.0 - LevelInfo._font.GetWidth(_specialText) / 2.0), (float)(position.y + height / 2.0 - 3.0), Color.White * alpha, (Depth)0.95f);
            }
            else
            {
                LevelInfo._font.scale = new Vec2(0.5f, 0.5f);
                LevelInfo._font.Draw(_name, position.x + 3f, (float)(position.y + height - 6.0), Color.White * alpha, (Depth)0.95f);
                _sprite.xscale = _sprite.yscale = width / _sprite.width;
                _sprite.depth = (Depth)0.95f;
                _sprite.alpha = alpha;
                Graphics.Draw(_sprite, position.x, position.y);
            }
        }
    }
}
