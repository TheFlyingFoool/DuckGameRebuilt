namespace DuckGame
{
    [EditorGroup("Details|Signs|Tutorial", EditorItemType.PowerUser)]
    public class TutorialSign02 : TutorialSign
    {
        public TutorialSign02(float xpos, float ypos)
          : base(xpos, ypos, "tutorial/slideUnder", "Slide Under")
        {
            editorCycleType = typeof(TutorialSign04);
        }

        public override void Draw()
        {
            graphic.color = Color.White * 0.5f;
            base.Draw();
        }
    }
}
