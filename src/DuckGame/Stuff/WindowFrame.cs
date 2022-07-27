// Decompiled with JetBrains decompiler
// Type: DuckGame.WindowFrame
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class WindowFrame : Thing
    {
        public float high;
        private bool floor;

        public WindowFrame(float xpos, float ypos, bool f)
          : base(xpos, ypos)
        {
            this.graphic = new Sprite("windowFrame");
            this.center = new Vec2(5f, 26f);
            this.depth = -0.95f;
            this._editorCanModify = false;
            this.floor = f;
            if (!this.floor)
                return;
            this.graphic.angleDegrees = -90f;
        }

        public override void Draw()
        {
            this.graphic.depth = this.depth;
            if (this.floor)
            {
                Graphics.Draw(this.graphic, this.x + 14f, this.y + 5f, new Rectangle(0.0f, this.graphic.height - 2, graphic.width, 2f));
                Graphics.Draw(this.graphic, this.x + 14f - this.high, this.y + 5f, new Rectangle(0.0f, 0.0f, graphic.width, 3f));
            }
            else
            {
                Graphics.Draw(this.graphic, this.x - 5f, this.y + 6f, new Rectangle(0.0f, this.graphic.height - 2, graphic.width, 2f));
                Graphics.Draw(this.graphic, this.x - 5f, this.y + 6f - this.high, new Rectangle(0.0f, 0.0f, graphic.width, 3f));
            }
        }
    }
}
