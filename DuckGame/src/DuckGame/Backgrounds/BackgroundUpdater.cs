namespace DuckGame
{
    public class BackgroundUpdater : Thing
    {
        public ParallaxBackground _parallax;
        protected float _lastCameraX;
        protected bool _update = true;
        protected bool _yParallax = true;
        protected float _yOffset;
        public Rectangle scissor = new Rectangle(0f, 0f, 0f, 0f);
        public bool overrideBaseScissorCall;
        public float _extraYOffset;
        public Color backgroundColor;
        protected bool _skipMovement;

        public ParallaxBackground parallax => _parallax;

        public bool update
        {
            get => _update;
            set => _update = value;
        }

        public void SetVisible(bool vis)
        {
            _parallax.scissor = scissor;
            _parallax.visible = vis;
            if (scissor.width == 0f)
                return;
            _parallax.layer.scissor = scissor;
        }

        public BackgroundUpdater(float xpos, float ypos)
          : base(xpos, ypos)
        {
            shouldbegraphicculled = false;
            _isStatic = true;
            _opaque = true;
            editorTooltip = "Adds a parallaxing background visual to the level (limit 1 per level)";
        }

        public static Vec2 GetWallScissor()
        {
            Matrix matrix = Level.current.camera.getMatrix();
            int x = 0;
            int y = 0;
            float num = Graphics.width / Resolution.size.x;
            foreach (RockWall rockWall in Level.current.things[typeof(RockWall)])
            {
                if (y == 0)
                    y = (int)Resolution.size.x;
                Vec2 vec2 = Vec2.Transform(rockWall.position, matrix) * num;
                if (!rockWall.flipHorizontal && vec2.x > x)
                    x = (int)vec2.x;
                else if (rockWall.flipHorizontal && vec2.x < y)
                    y = (int)vec2.x;
            }
            if (y != 0)
                y -= x;
            if (y == 0)
                y = (int)Resolution.size.x;
            return new Vec2(x, y);
        }

        public override void Update()
        {
            if (!overrideBaseScissorCall)
            {
                Vec2 wallScissor = GetWallScissor();
                if (wallScissor != Vec2.Zero)
                    scissor = new Rectangle((int)wallScissor.x, 0f, (int)wallScissor.y, Resolution.current.y);
            }
            if (!_update)
                return;
            if (!_skipMovement)
            {
                float num = Level.current.camera.width * 4f / Graphics.width;
                if (_yParallax)
                {
                    _parallax.y = (float)(-(Level.current.camera.centerY / 12f) - 5f) + _yOffset;
                }
                else
                {
                    Layer.Parallax.camera = Level.current.camera;
                    _parallax.y = _extraYOffset - 108f;
                }
                _parallax.xmove = (_lastCameraX - Level.current.camera.centerX) / num;
            }
            _lastCameraX = Level.current.camera.centerX;
            if (scissor.width != 0f)
                _parallax.scissor = scissor;
            base.Update();
        }

        public override ContextMenu GetContextMenu() => null;
    }
}
