namespace DuckGame
{
    [ClientOnly]
    public class WumpMagnumbullet : Bullet
    {
        public WumpMagnumbullet(float xval, float yval, AmmoType type, float ang = -1f, Thing owner = null, bool rbound = false, float distance = -1f, bool tracer = false, bool network = true) : base(xval, yval, type, ang, owner, rbound, distance, tracer, network)
        {
        }
        public override void Initialize()
        {
            color = Color.Red;
            base.Initialize();
        }
        protected override void Rebound(Vec2 pos, float dir, float rng)
        {
            if (_teleporter != null)
            {
                base.Rebound(pos, dir, rng);
                return;
            }
            SFX.Play("ting");
            ++reboundBulletsCreated;
            ATWumpMagnum ammor = new ATWumpMagnum();
            ammor.range = ammo.range * 2;
            ammor.rebound = false;
            WumpMagnumbullet bullet = ammor.GetBullet(pos.x, pos.y, angle: (-dir), firedFrom: firedFrom, distance: rng, tracer: _tracer) as WumpMagnumbullet;
            bullet._teleporter = _teleporter;
            bullet.timesRebounded = timesRebounded + 1;
            bullet.lastReboundSource = lastReboundSource;
            bullet.isLocal = isLocal;
            bullet.rebound = false;
            _reboundedBullet = bullet;
            reboundCalled = true;


            Level.Add(bullet);
        }
        public override void Update()
        {
            base.Update();
        }
        public static bool reboundstop;
    }
}
