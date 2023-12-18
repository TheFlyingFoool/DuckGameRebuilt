namespace DuckGame
{
    [ClientOnly]
    public class FadingAwayThing : Thing
    {
        public FadingAwayThing(Thing t, Vec2 bl, float a) : base(t.x, t.y)
        {
            blast = bl;
            center = t.center;
            ang = a;
            alpha = Rando.Float(1, 1.3f);
            graphic = t.graphic.Clone();
            if (red == null)
            {
                red = new MaterialRedFade();
            }
        }
        public static Material red;
        public Vec2 blast;
        public float ang;
        public override void Update()
        {
            position += blast;
            blast = Lerp.Vec2(blast, Vec2.Zero, 0.01f);
            angle += ang;
            alpha -= 0.02f;
            if (alpha <= 0)
            {
                Level.Remove(this);
            }
        }
        public override void Draw()
        {
            Graphics.material = red;
            base.Draw();
            Graphics.material = null;
        }
    }
}
