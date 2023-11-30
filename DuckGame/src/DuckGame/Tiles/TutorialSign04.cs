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
