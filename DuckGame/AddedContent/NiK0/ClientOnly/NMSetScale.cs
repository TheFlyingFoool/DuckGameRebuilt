namespace DuckGame
{
    [ClientOnly]
    public class NMSetScale : NMEvent
    {
        public NMSetScale(Thing thing, Vec2 vec, bool collision = false)
        {
            t = thing;
            v = vec;
            col = collision;
        }
        public NMSetScale()
        {
        }
        public Thing t;
        public Vec2 v;
        public bool col;

        public override void Activate()
        {
            if (Level.current == null || t == null) return;
            t.scale = v;
            if (col)
            {
                t.collisionSize *= v;
                t.collisionOffset *= v;
            }
        }
    }
}
