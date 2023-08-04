namespace DuckGame
{
    [ClientOnly]
    public class LaserDiskParticle : Thing
    {
        public LaserDiskParticle(float xpos, float ypos, Color c) : base(xpos, ypos)
        {
            graphic = new Sprite("dDisk");
            graphic.color = c;
            center = new Vec2(16, 16);
        }
        public Vec2 spd;
        public override void Update()
        {
            alpha -= 0.01f;
            position += spd;
            spd = Lerp.Vec2Smooth(spd, Vec2.Zero, 0.02f);
            if (alpha <= 0)
            {
                Level.Remove(this);
            }
        }
    }
}
