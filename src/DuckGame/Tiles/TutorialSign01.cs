// Decompiled with JetBrains decompiler
// Type: DuckGame.TutorialSign01
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    [EditorGroup("Details|Signs|Tutorial", EditorItemType.PowerUser)]
    public class TutorialSign01 : TutorialSign
    {
        public TutorialSign01(float xpos, float ypos)
          : base(xpos, ypos, "tutorial/groundPound", "Ground Pound")
        {
        }

        public override void Draw()
        {
            Graphics.DrawString("@JUMP@", new Vec2(x - 24f, y + 36f), Color.White * 0.5f);
            Graphics.DrawString("@DOWN@", new Vec2(x + 25f, y - 1f), Color.White * 0.5f);
            graphic.color = Color.White * 0.5f;
            base.Draw();
        }
    }
}
