namespace DuckGame
{
    [EditorGroup("Special|Goodies", EditorItemType.Arcade)]
    [BaggedProperty("isOnlineCapable", false)]
    public class LapGoody : Goody
    {
        public LapGoody(float xpos, float ypos)
          : base(xpos, ypos, new Sprite("challenge/goal"))
        {
        }
    }
}
