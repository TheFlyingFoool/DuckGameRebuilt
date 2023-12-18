namespace DuckGame
{
    public class DoorFrame : Thing
    {
        public DoorFrame(float xpos, float ypos, bool secondaryFrame)
          : base(xpos, ypos)
        {
            graphic = new Sprite(secondaryFrame ? "pyramidDoorFrame" : "doorFrame");
            center = new Vec2(5f, 26f);
            depth = -0.95f;
            _editorCanModify = false;
            shouldbeinupdateloop = false;
        }
    }
}
