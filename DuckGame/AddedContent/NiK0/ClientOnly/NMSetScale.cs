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
            t.scale = v;
        }
        protected override void OnSerialize()
        {
            base.OnSerialize();
        }

        public override void OnDeserialize(BitBuffer d)
        {
            base.OnDeserialize(d);
        }
    }
}
