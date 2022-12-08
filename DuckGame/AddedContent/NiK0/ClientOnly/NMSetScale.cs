namespace DuckGame
{
    [ClientOnly]
    public class NMSetScale : NMEvent
    {
        public NMSetScale(Thing thing, Vec2 vec)
        {
            t = thing;
            v = vec;
        }
        public NMSetScale()
        {
        }
        public Thing t;
        public Vec2 v;

        public override void Activate()
        {
            if (Level.current == null || t == null) return;
            t.scale = v;
        }
    }
}
