// Decompiled with JetBrains decompiler
// Type: DuckGame.TutorialSign05
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    [EditorGroup("Details|Signs|Tutorial", EditorItemType.PowerUser)]
    public class TutorialSign05 : TutorialSign
    {
        public TutorialSign05(float xpos, float ypos)
          : base(xpos, ypos, "tutorial/fly", "Fly")
        {
        }

        public override void Draw()
        {
            Color color = new Color(sbyte.MaxValue, sbyte.MaxValue, sbyte.MaxValue);
            Graphics.DrawString("@JUMP@", new Vec2(this.x - 26f, this.y + 32f), Color.White * 0.5f);
            Graphics.DrawString("@JUMP@", new Vec2(this.x - 5f, this.y - 16f), Color.White * 0.5f);
            Graphics.DrawString("@JUMP@", new Vec2(this.x + 15f, this.y - 8f), Color.White * 0.5f);
            this.depth = (Depth)0.99f;
            this.graphic.color = color;
            base.Draw();
        }
    }
}
