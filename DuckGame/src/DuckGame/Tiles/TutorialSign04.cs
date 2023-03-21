// Decompiled with JetBrains decompiler
// Type: DuckGame.TutorialSign04
//removed for regex reasons Culture=neutral, PublicKeyToken=null
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
            editorCycleType = typeof(TutorialSign05);
        }

        public override void Draw()
        {
            Graphics.DrawString("@JUMP@", new Vec2(x + 7f, y - 30f), Color.White * 0.5f);
            Graphics.DrawString("@DOWN@", new Vec2(x - 18f, y - 30f), Color.White * 0.5f);
            graphic.color = Color.White * 0.5f;
            base.Draw();
        }
    }
}
