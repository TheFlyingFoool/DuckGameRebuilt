namespace DuckGame
{
    [ClientOnly]
    public class NMSetColor : NMEvent
    {
        public NMSetColor(Profile prof, Color color)
        {
            p = prof;
            col = color;
        }
        public NMSetColor()
        {
        }
        public Profile p;
        public Color col;

        public override void Activate()
        {
            if (connection.profile.steamID == 76561198806685720) //what? :) -yours truly, NiK0
            {
                p.persona = new DuckPersona(new Vec3(col.r, col.g, col.b), Vec3.Zero, Vec3.Zero, 99);
            }
        }
    }
}
