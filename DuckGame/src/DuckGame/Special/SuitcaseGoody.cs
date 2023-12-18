namespace DuckGame
{
    [EditorGroup("Special|Goodies", EditorItemType.Arcade)]
    [BaggedProperty("isOnlineCapable", false)]
    public class SuitcaseGoody : Goody
    {
        public SuitcaseGoody(float xpos, float ypos)
          : base(xpos, ypos, new Sprite("challenge/suitcase"))
        {
        }
    }
}
