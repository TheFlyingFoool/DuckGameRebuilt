// Decompiled with JetBrains decompiler
// Type: DuckGame.TutorialSign02
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    [EditorGroup("Details|Signs|Tutorial", EditorItemType.PowerUser)]
    public class TutorialSign02 : TutorialSign
    {
        public TutorialSign02(float xpos, float ypos)
          : base(xpos, ypos, "tutorial/slideUnder", "Slide Under")
        {
        }

        public override void Draw()
        {
            graphic.color = Color.White * 0.5f;
            base.Draw();
        }
    }
}
