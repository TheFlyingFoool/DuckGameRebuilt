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
            get => this._name;
            set => this._name = value;
        }

        public string description
        {
            get => this._description;
            set => this._description = value;
        }

        public Tex2D image
        {
            get => this._image;
            set
            {
                this._image = value;
                this._sprite = new Sprite(this._image);
            }
        }

        public bool large
        {
            get => this._large;
            set => this._large = value;
        }

        public override float width => !this._large ? 71f : 96f;

        public override float height => !this._large ? 48f : 62f;

        public LevelInfo(bool large = true, string text = null)
        {
            this._large = large;
            this._specialText = text;
        }

        public override void Draw(Vec2 position, bool selected, float alpha)
        {
            Graphics.DrawRect(position, position + new Vec2(this.width, this.height), new Color(25, 38, 41) * alpha, (Depth)0.9f);
            if (selected)
                Graphics.DrawRect(position + new Vec2(-1f, 0f), position + new Vec2(this.width + 1f, this.height), Color.White * alpha, (Depth)0.97f, false);
            if (this._specialText != null)
            {
                LevelInfo._font.scale = new Vec2(0.5f, 0.5f);
                LevelInfo._font.Draw(this._specialText, (float)(position.x + this.width / 2.0 - LevelInfo._font.GetWidth(this._specialText) / 2.0), (float)(position.y + this.height / 2.0 - 3.0), Color.White * alpha, (Depth)0.95f);
            }
            else
            {
                LevelInfo._font.scale = new Vec2(0.5f, 0.5f);
                LevelInfo._font.Draw(this._name, position.x + 3f, (float)(position.y + this.height - 6.0), Color.White * alpha, (Depth)0.95f);
                this._sprite.xscale = this._sprite.yscale = this.width / _sprite.width;
                this._sprite.depth = (Depth)0.95f;
                this._sprite.alpha = alpha;
                Graphics.Draw(this._sprite, position.x, position.y);
            }
        }
    }
}
