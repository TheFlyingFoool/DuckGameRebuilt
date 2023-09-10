namespace DuckGame
{
    [ClientOnly]
    public class FakeCrashPopup : Thing, IDrawToDifferentLayers
    {
        public FakeCrashPopup(float xpos, float ypos) : base(xpos, ypos)
        {
            graphic = new Sprite("FakeCrash");
            graphic.CenterOrigin();
            scale = new Vec2(0.21f);
            alpha = 0;
            startTime = Rando.Int(200, 300);
            time = startTime;
            center = new Vec2(285.5f, 227);
        }
        public override void Draw()
        {
        }
        public int startTime, time;
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
                int timeDelta = startTime - time;
                float timeFactor;

                if (timeDelta <= 15)
                {
                    timeFactor = Ease.Out.Quad(timeDelta / 15f);
                    scale = new Vec2((float)(0.21f + (timeFactor * 0.04f)));
                    alpha = timeFactor;
                }

                if (time < 10)
                {
                    timeFactor = Ease.Out.Quad(time * 0.1f);
                    scale = new Vec2((float)(0.225f + (timeFactor * 0.025f)));
                    alpha = timeFactor;
                }

                base.Draw();
            }
        }
    }
}
