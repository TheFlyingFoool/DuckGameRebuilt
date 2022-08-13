// Decompiled with JetBrains decompiler
// Type: DuckGame.TutorialGunCrouchJump
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    [EditorGroup("Details|Signs|Tutorial", EditorItemType.PowerUser)]
    public class TutorialGunCrouchJump : TutorialSign
    {
        public TutorialGunCrouchJump(float xpos, float ypos)
          : base(xpos, ypos, "tutorial/guncrouchjump", "Gun Crouch Jump")
        {
        }

        public override void Draw()
        {
            Color color = new Color(sbyte.MaxValue, sbyte.MaxValue, sbyte.MaxValue);
            Graphics.DrawString("@SHOOT@", new Vec2(x - 16f, y + 8f), Color.White * 0.5f);
            Graphics.DrawString("@JUMP@", new Vec2(x - 39f, y + 8f), Color.White * 0.5f);
            depth = (Depth)0.99f;
            graphic.color = color;
            base.Draw();
        }
    }
}
