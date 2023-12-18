namespace DuckGame
{
    [EditorGroup("Details|Signs|Tutorial", EditorItemType.PowerUser)]
    public class TutorialSign01 : TutorialSign
    {
        public TutorialSign01(float xpos, float ypos)
          : base(xpos, ypos, "tutorial/groundPound", "Ground Pound")
        {
            editorCycleType = typeof(TutorialGunCrouchJump);
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
