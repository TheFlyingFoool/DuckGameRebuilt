namespace DuckGame
{
    [ClientOnly]
    public class FreezeSmoke : Thing
    {
        private float _angleInc;
        private float _scaleInc;
        private float _fade;
        public Vec2 move;
        public Vec2 fly;
        private float _fastGrow;
        private Sprite _backgroundSmoke;

        public FreezeSmoke(float xpos, float ypos)
          : base(xpos, ypos)
        {
            xscale = 0.15f + Rando.Float(0.15f);
            yscale = xscale;
            angle = Maths.DegToRad(Rando.Float(360f));
            _fastGrow = 0.6f + Rando.Float(0.3f);
            _angleInc = Maths.DegToRad(Rando.Float(2f) - 1f);
            _scaleInc = 1f / 1000f + Rando.Float(1f / 1000f);
            _fade = 0.0015f + Rando.Float(1f / 1000f);
            move.x = Rando.Float(0.2f) - 0.1f;
            move.y = Rando.Float(0.2f) - 0.1f;
            GraphicList graphicList = new GraphicList();
            Sprite graphic1 = new Sprite("smoke")
            {
                depth = (Depth)1f
            };
            graphic1.CenterOrigin();
            graphicList.Add(graphic1);
            Sprite graphic2 = new Sprite("smokeBack")
            {
                depth = -0.1f
            };
            graphic2.CenterOrigin();
            graphicList.Add(graphic2);
            graphic = graphicList;
            graphic1.color = new Color(0, 255, 255);
            graphic2.color = new Color(0, 255, 255);
            center = new Vec2(0f, 0f);
            depth = (Depth)1f;
            _backgroundSmoke = new Sprite("smokeBack");
        }

        public override void Update()
        {
            angle += _angleInc;
            xscale += _scaleInc;
            if (_fastGrow > 0f)
            {
                _fastGrow -= 0.05f;
                xscale += 0.05f;
            }
            if (fly.x > 0.01f || fly.x < -0.01f)
            {
                x += fly.x;
                fly.x *= 0.9f;
            }
            if (fly.y > 0.01f || fly.y < -0.01f)
            {
                y += fly.y;
                fly.y *= 0.9f;
            }
            yscale = xscale;
            x += move.x;
            y += move.y;
            xscale -= 0.005f;
            if (xscale >= 0.1f)
                return;
            Level.Remove(this);
        }
    }
}
