// Decompiled with JetBrains decompiler
// Type: DuckGame.Card
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class Card
    {
        protected string _specialText;
        private static BitmapFont _font = new BitmapFont("biosFont", 8);

        public string specialText
        {
            get => this._specialText;
            set => this._specialText = value;
        }

        public virtual float width => 71f;

        public virtual float height => 12f;

        public Card(string text) => this._specialText = text;

        public Card()
        {
        }

        public virtual void Draw(Vec2 position, bool selected, float alpha)
        {
            Graphics.DrawRect(position, position + new Vec2(this.width, this.height), new Color(25, 38, 41) * alpha, (Depth)0.9f);
            if (selected)
                Graphics.DrawRect(position + new Vec2(-1f, 0f), position + new Vec2(this.width + 1f, this.height), Color.White * alpha, (Depth)0.97f, false);
            Card._font.scale = new Vec2(0.5f, 0.5f);
            Card._font.Draw(this._specialText, position.x + 4f, position.y + 4f, Color.White * alpha, (Depth)0.98f);
        }
    }
}
