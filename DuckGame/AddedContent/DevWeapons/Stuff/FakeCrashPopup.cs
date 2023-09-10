namespace DuckGame
{
    [ClientOnly]
    public class FakeCrashPopup : Thing, IDrawToDifferentLayers
    {
        public FakeCrashPopup(float xpos, float ypos) : base(xpos, ypos)
        {
            graphic = new Sprite("FakeCrash");
            graphic.CenterOrigin();
            scale = new Vec2(0.15f);
            alpha = 0;
            time = Rando.Int(200, 300);
            center = new Vec2(285.5f, 227);
        }
        public override void Draw()
        {
        }
        public int time;
        public Camera origCamera;
        public bool froze;
        public override void Update()
        {
            if (!froze && !Level.current.camera.skipUpdate)
            {
                Level.current.camera.skipUpdate = true;
            }
            time--;
            if (time < 20) if (origCamera != null) Level.current.camera.skipUpdate = false;
            if (time < 0) Level.Remove(this);
        }
        public void OnDrawLayer(Layer l)
        {
            if (l == Layer.HUD)
            {
                if (time > 20)
                {
                    scale = Lerp.Vec2(scale, new Vec2(0.25f), 0.05f);
                    alpha = Lerp.Float(alpha, 1, 0.1f);
                }
                else
                {
                    alpha -= 0.1f;
                    scale = Lerp.Vec2(scale, Vec2.Zero, 0.05f);
                }
                base.Draw();
            }
        }
    }
}
