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
            graphic = new Sprite("windowFrame");
            center = new Vec2(5f, 26f);
            depth = -0.95f;
            _editorCanModify = false;
            floor = f;
            if (!floor)
                return;
            graphic.angleDegrees = -90f;
        }

        public override void Draw()
        {
            graphic.depth = depth;
            if (floor)
            {
                Graphics.Draw(graphic, x + 14f, y + 5f, new Rectangle(0f, graphic.height - 2, graphic.width, 2f));
                Graphics.Draw(graphic, x + 14f - high, y + 5f, new Rectangle(0f, 0f, graphic.width, 3f));
            }
            else
            {
                Graphics.Draw(graphic, x - 5f, y + 6f, new Rectangle(0f, graphic.height - 2, graphic.width, 2f));
                Graphics.Draw(graphic, x - 5f, y + 6f - high, new Rectangle(0f, 0f, graphic.width, 3f));
            }
        }
    }
}
