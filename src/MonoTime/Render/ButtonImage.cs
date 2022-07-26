// Decompiled with JetBrains decompiler
// Type: DuckGame.ButtonImage
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
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
            this._font = new BitmapFont("tinyNumbers", 4, 5);
            this._keySprite = new Sprite("buttons/genericButton");
            this._keyString = ((int)key).ToString() ?? "";
            this._texture = this._keySprite.texture;
        }

        public override void Draw()
        {
            this._keySprite.position = this.position;
            this._keySprite.alpha = this.alpha;
            this._keySprite.color = this.color;
            this._keySprite.depth = this.depth;
            this._keySprite.scale = this.scale;
            this._keySprite.Draw();
            this._font.scale = this.scale;
            this._font.Draw(this._keyString, this.position + new Vec2((float)((double)this._keySprite.width * (double)this.scale.x / 2.0 - (double)this._font.GetWidth(this._keyString) / 2.0), 4f * this.scale.y), new Color(20, 32, 34), this.depth + 2);
        }
    }
}
