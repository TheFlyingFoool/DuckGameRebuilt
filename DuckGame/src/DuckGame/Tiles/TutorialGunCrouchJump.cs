namespace DuckGame
{
    [EditorGroup("Details|Signs|Tutorial", EditorItemType.PowerUser)]
    public class TutorialGunCrouchJump : TutorialSign
    {
        public TutorialGunCrouchJump(float xpos, float ypos)
          : base(xpos, ypos, "tutorial/guncrouchjump", "Gun Crouch Jump")
        {
            editorCycleType = typeof(TutorialGunJump);
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
