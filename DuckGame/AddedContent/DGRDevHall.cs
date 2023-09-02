namespace DuckGame
{
    public class DGRDevHall : Level
    {
        public Duck duck;
        public DGRDevHall(Duck d)
        {
            starfield = new Sprite("background/starField");
            duck = d;
        }
        public Sprite starfield;
        public override void PostDrawLayer(Layer layer)
        {
            if (layer == Layer.Parallax)
            {
                starfield.alpha = 1;
                Graphics.Draw(starfield, 0f, 0, -0.99f);
            }
            base.PostDrawLayer(layer);
        }
        public override void Update()
        {
            duck.x = Maths.Clamp(duck.x, 10, 310);
            camera.y = Lerp.FloatSmooth(camera.y, 0, 0.1f, 0.95f);
            base.Update();
        }
        public override void Initialize()
        {
            camera.y -= 640;
            Add(duck);
            duck.position = new Vec2(160, 138);
            Add(new GlassPlatform(160, 96));
            Add(new InvisibleBlock(0, 165, 126, 13));
            Add(new InvisibleBlock(194, 165, 126, 13));
            Add(new InvisibleBlock(0, 88, 120, 13));
            Add(new InvisibleBlock(200, 88, 120, 13));
            Add(new InvisibleBlock(0, 33, 120, 13));
            Add(new InvisibleBlock(200, 33, 120, 13));

            Add(new Platform(118, 112, 84, 8));
            Add(new Platform(118, 57, 84, 8));
            base.Initialize();
        }
    }
}
