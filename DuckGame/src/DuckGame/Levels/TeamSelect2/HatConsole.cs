namespace DuckGame
{
    public class HatConsole : Thing
    {
        public ProfileBox2 box;

        public HatConsole(float xpos, float ypos, ProfileBox2 bbox)
          : base(xpos, ypos)
        {
            box = bbox;
        }
    }
}
