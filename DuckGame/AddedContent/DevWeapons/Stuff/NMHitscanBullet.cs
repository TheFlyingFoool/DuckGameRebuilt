namespace DuckGame
{
    [ClientOnly]
    public class NMHitscanBullet : NMEvent
    {
        public NMHitscanBullet(HitscanBullet b)
        {
            v1 = b.position;
            v2 = b.pos2;
            if (b.c != Color.White) color = true;
        }
        public NMHitscanBullet()
        {
        }
        public Vec2 v1;
        public Vec2 v2;
        public bool color;
        public override void Activate()
        {
            Color col = Color.White;
            float bulThick = 2.5f;
            if (color)
            {
                col = Color.Yellow;
                bulThick = 3.5f;
            }
            Level.Add(new HitscanBullet(v1.x, v1.y, v2) { c = col, bulWidth = bulThick, isLocal = false });
            base.Activate();
        }
    }
}
