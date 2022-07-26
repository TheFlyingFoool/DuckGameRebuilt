// Decompiled with JetBrains decompiler
// Type: DuckGame.TutorialSign04
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    [EditorGroup("Details|Signs|Tutorial", EditorItemType.PowerUser)]
    public class TutorialSign04 : TutorialSign
    {
        public TutorialSign04(float xpos, float ypos)
          : base(xpos, ypos, "tutorial/fallThrough", "Fall Through")
        {
        }

        public override void Draw()
        {
            Graphics.DrawString("@JUMP@", new Vec2(this.x + 7f, this.y - 30f), Color.White * 0.5f);
            Graphics.DrawString("@DOWN@", new Vec2(this.x - 18f, this.y - 30f), Color.White * 0.5f);
            this.graphic.color = Color.White * 0.5f;
            base.Draw();
        }
    }
}
