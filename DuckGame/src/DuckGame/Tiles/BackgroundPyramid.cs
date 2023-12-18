namespace DuckGame
{
    [EditorGroup("Background", EditorItemType.Pyramid)]
    public class BackgroundPyramid : BackgroundTile
    {
        private bool inited;

        public BackgroundPyramid(float xpos, float ypos)
          : base(xpos, ypos)
        {
            graphic = new SpriteMap("pyramidBackground", 16, 16, true);
            center = new Vec2(8f, 8f);
            collisionSize = new Vec2(16f, 16f);
            collisionOffset = new Vec2(-8f, -8f);
            _editorName = "Pyramid";
        }

        public override void Initialize() => base.Initialize();

        public override void Update()
        {
            if (!inited)
            {
                inited = true;
                SpriteMap graphic = this.graphic as SpriteMap;
                if (!flipHorizontal && graphic.frame % 8 == 0)
                {
                    if (Level.CheckPoint<BackgroundPyramid>(position + new Vec2(-16f, 0f)) != null)
                    {
                        ++graphic.frame;
                        graphic.UpdateFrame();
                    }
                }
                else if (!flipHorizontal && graphic.frame % 8 == 7)
                {
                    if (Level.CheckPoint<BackgroundPyramid>(position + new Vec2(16f, 0f)) != null)
                    {
                        --graphic.frame;
                        graphic.UpdateFrame();
                    }
                }
                else if (flipHorizontal && graphic.frame % 8 == 0)
                {
                    if (Level.CheckPoint<BackgroundPyramid>(position + new Vec2(16f, 0f)) != null)
                    {
                        ++graphic.frame;
                        graphic.UpdateFrame();
                    }
                }
                else if (flipHorizontal && graphic.frame % 8 == 7 && Level.CheckPoint<BackgroundPyramid>(position + new Vec2(-16f, 0f)) != null)
                {
                    --graphic.frame;
                    graphic.UpdateFrame();
                }
            }
            base.Update();
        }
    }
}
