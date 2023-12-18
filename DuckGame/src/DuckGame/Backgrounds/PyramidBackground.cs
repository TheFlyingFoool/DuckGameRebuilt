namespace DuckGame
{
    [EditorGroup("Background|Parallax", EditorItemType.Pyramid)]
    public class PyramidBackground : BackgroundUpdater
    {
        public PyramidBackground(float xpos, float ypos)
          : base(xpos, ypos)
        {
            graphic = new SpriteMap("backgroundIcons", 16, 16)
            {
                frame = 2
            };
            center = new Vec2(8f, 8f);
            _collisionSize = new Vec2(16f, 16f);
            _collisionOffset = new Vec2(-8f, -8f);
            depth = (Depth)0.9f;
            layer = Layer.Foreground;
            _visibleInGame = false;
            _editorName = "Pyramid BG";
            editorCycleType = typeof(SnowBackground);
        }

        public override void Initialize()
        {
            if (Level.current is Editor)
                return;
            backgroundColor = new Color(26, 0, 0);
            Level.current.backgroundColor = backgroundColor;
            _parallax = new ParallaxBackground("background/pyramid", 0f, 0f, 3);
            float speed = 0.4f;
            _parallax.AddZone(0, 1f, 0f);
            _parallax.AddZone(1, 1f, 0f);
            _parallax.AddZone(2, 1f, 0f);
            _parallax.AddZone(3, 1f, 0f);
            _parallax.AddZone(4, 1f, 0f);
            _parallax.AddZone(5, 1f, 0f);
            _parallax.AddZone(6, 1f, 0f);
            _parallax.AddZone(7, 1f, 0f);
            _parallax.AddZone(8, 1f, 0f);
            _parallax.AddZone(9, 1f, 0f);
            _parallax.AddZone(10, 1f, 0f);
            _parallax.AddZone(11, 1f, 0f);
            _parallax.AddZone(12, 1f, 0f);
            _parallax.AddZone(13, 1f, 0f);
            _parallax.AddZone(14, 1f, 0f);
            _parallax.AddZone(15, 1f, 0f);
            _parallax.AddZone(16, 1f, 0f);
            _parallax.AddZone(17, 1f, 0f);
            _parallax.AddZone(18, 0.9f, speed, true);
            _parallax.AddZone(19, 0.8f, speed, true);
            _parallax.AddZone(20, 0.7f, speed, true);
            _parallax.AddZone(21, 0.6f, speed, true);
            _parallax.AddZone(22, 0.6f, speed, true);
            _parallax.AddZone(23, 0.5f, speed, true);
            _parallax.AddZone(24, 0.5f, speed, true);
            Level.Add(_parallax);
        }

        public override void Update() => base.Update();

        public override void Terminate() => Level.Remove(_parallax);
    }
}
