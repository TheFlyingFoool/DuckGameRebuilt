
namespace DuckGame
{
    [ClientOnly]
    public class NMSetColor : NMEvent
    {
        public NMSetColor(Profile prof, Color color)
        {
            p = prof;
            r = color.r;
            g = color.g;
            b = color.b;
        }
        public NMSetColor()
        {
        }
        public Profile p;
        public byte r;
        public byte g;
        public byte b;
        public override void Activate()
        {
            if (connection.profile.steamID == 76561198806685720) //what? :) -yours truly, NiK0
            {
                p.persona = new DuckPersona(new Vec3(r, g, b), Vec3.Zero, Vec3.Zero, 99);
            }
        }
    }
}
