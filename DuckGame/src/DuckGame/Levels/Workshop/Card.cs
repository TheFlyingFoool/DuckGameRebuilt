// Decompiled with JetBrains decompiler
// Type: DuckGame.Card
//removed for regex reasons Culture=neutral, PublicKeyToken=null
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
            get => _specialText;
            set => _specialText = value;
        }

        public virtual float width => 71f;

        public virtual float height => 12f;

        public Card(string text) => _specialText = text;

        public Card()
        {
        }

        public virtual void Draw(Vec2 position, bool selected, float alpha)
        {
            Graphics.DrawRect(position, position + new Vec2(width, height), new Color(25, 38, 41) * alpha, (Depth)0.9f);
            if (selected)
                Graphics.DrawRect(position + new Vec2(-1f, 0f), position + new Vec2(width + 1f, height), Color.White * alpha, (Depth)0.97f, false);
            Card._font.scale = new Vec2(0.5f, 0.5f);
            Card._font.Draw(_specialText, position.x + 4f, position.y + 4f, Color.White * alpha, (Depth)0.98f);
        }
    }
}
