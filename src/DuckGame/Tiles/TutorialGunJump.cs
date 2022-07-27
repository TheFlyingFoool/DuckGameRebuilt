// Decompiled with JetBrains decompiler
// Type: DuckGame.TutorialGunJump
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    [EditorGroup("Details|Signs|Tutorial", EditorItemType.PowerUser)]
    public class TutorialGunJump : TutorialSign
    {
        public TutorialGunJump(float xpos, float ypos)
          : base(xpos, ypos, "tutorial/gunjump", "Gun Jump")
        {
        }

        public override void Draw()
        {
            Color color = new Color(sbyte.MaxValue, sbyte.MaxValue, sbyte.MaxValue);
            Graphics.DrawString("@SHOOT@", new Vec2(this.x - 16f, this.y + 8f), Color.White * 0.5f);
            Graphics.DrawString("@JUMP@", new Vec2(this.x - 39f, this.y + 8f), Color.White * 0.5f);
            this.depth = (Depth)0.99f;
            this.graphic.color = color;
            base.Draw();
        }
    }
}
