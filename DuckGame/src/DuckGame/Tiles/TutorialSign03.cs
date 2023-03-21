// Decompiled with JetBrains decompiler
// Type: DuckGame.TutorialSign03
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    [EditorGroup("Details|Signs|Tutorial", EditorItemType.PowerUser)]
    public class TutorialSign03 : TutorialSign
    {
        public TutorialSign03(float xpos, float ypos)
          : base(xpos, ypos, "tutorial/jumpThrough", "Jump Through")
        {
            editorCycleType = typeof(TutorialSign02);
        }

        public override void Draw()
        {
            Graphics.DrawString("@JUMP@", new Vec2(x + 40f, y + 13f), Color.White * 0.5f);
            graphic.color = Color.White * 0.5f;
            base.Draw();
        }
    }
}
