namespace DuckGame
{
    [ClientOnly]
    public class WumpMissile : Bullet
    {
        public WumpMissile(float xval, float yval, AmmoType type, float ang = -1f, Thing owner = null, bool rbound = false, float distance = -1f, bool tracer = false, bool network = true) : base(xval, yval, type, ang, owner, rbound, distance, tracer, network)
        {
        }
        public override void Initialize()
        {
            v = velocity.Rotate(0.01f * ATWumpMissile.lol, Vec2.Zero);
            color = Color.Purple;
            sw = new SinWave(0.1f * ATWumpMissile.lol);
            base.Initialize();
        }
        public override void Update()
        {
            velocity = v.Rotate(sw / 3, Vec2.Zero);
            base.Update();
        }
        public SinWave sw;
        public Vec2 v;
        public int plusplus;
    }
}
