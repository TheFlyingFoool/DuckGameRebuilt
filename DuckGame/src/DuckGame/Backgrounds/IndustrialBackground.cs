namespace DuckGame
{
    [EditorGroup("Background|Parallax")]
    [BaggedProperty("previewPriority", true)]
    public class IndustrialBackground : BackgroundUpdater
    {
        public IndustrialBackground(float xpos, float ypos)
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
            _editorName = "Industrial BG";
            editorCycleType = typeof(KingdomBackground);
        }

        public override void Initialize()
        {
            if (Level.current is Editor)
                return;
            backgroundColor = new Color(26, 0, 0);
            Level.current.backgroundColor = backgroundColor;
            _parallax = new ParallaxBackground("background/industrial", 0f, 0f, 3);
            float speed = 0.4f;
            _parallax.AddZone(0, 0f, -speed, true);
            _parallax.AddZone(1, 0f, -speed, true);
            _parallax.AddZone(2, 0f, -speed, true);
            _parallax.AddZone(3, 0.2f, -speed, true);
            _parallax.AddZone(4, 0.2f, -speed, true);
            _parallax.AddZone(5, 0.4f, -speed, true);
            _parallax.AddZone(6, 0.6f, -speed, true);
            float distance = 0.8f;
            _parallax.AddZone(16, distance, speed);
            _parallax.AddZone(17, distance, speed);
            _parallax.AddZone(18, distance, speed);
            _parallax.AddZone(19, distance, speed);
            _parallax.AddZone(20, distance, speed);
            _parallax.AddZone(21, distance, speed);
            _parallax.AddZone(22, 0.3f, speed);
            _parallax.AddZone(23, 0.3f, speed);
            _parallax.AddZone(24, 0.2f, speed);
            _parallax.AddZone(25, 0.2f, speed);
            _parallax.AddZone(26, 0.1f, speed);
            _parallax.AddZone(27, 0.1f, speed);
            _parallax.AddZone(28, 0.1f, speed);
            _parallax.AddZone(29, 0f, speed);
            Level.Add(_parallax);
        }

        public override void Update() => base.Update();

        public override void Terminate() => Level.Remove(_parallax);
    }
}
