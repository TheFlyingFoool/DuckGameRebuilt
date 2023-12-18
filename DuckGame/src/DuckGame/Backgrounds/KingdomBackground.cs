namespace DuckGame
{
    [EditorGroup("Background|Parallax")]
    [BaggedProperty("previewPriority", true)]
    public class KingdomBackground : BackgroundUpdater
    {
        private float _speedMult;
        private bool _moving;

        public KingdomBackground(float xpos, float ypos, bool moving = false, float speedMult = 1f)
          : base(xpos, ypos)
        {
            graphic = new SpriteMap("backgroundIcons", 16, 16)
            {
                frame = 3
            };
            center = new Vec2(8f, 8f);
            _collisionSize = new Vec2(16f, 16f);
            _collisionOffset = new Vec2(-8f, -8f);
            depth = (Depth)0.9f;
            layer = Layer.Foreground;
            _visibleInGame = false;
            _speedMult = speedMult;
            _moving = moving;
            _editorName = "Kingdom BG";
            editorCycleType = typeof(NatureBackground);
        }

        public override void Initialize()
        {
            if (Level.current is Editor)
                return;
            backgroundColor = new Color(216, 240, 248);
            level.backgroundColor = backgroundColor;
            _parallax = new ParallaxBackground("background/kingdomSky", 0f, 0f, 3);
            float speed = 0.4f * _speedMult;
            _parallax.AddZone(16, 0.8f, speed, _moving);
            _parallax.AddZone(17, 0.8f, speed, _moving);
            _parallax.AddZone(18, 0.7f, speed, _moving);
            _parallax.AddZone(19, 0.7f, speed, _moving);
            _parallax.AddZone(20, 0.7f, speed, _moving);
            _parallax.AddZone(21, 0.7f, speed, _moving);
            _parallax.AddZone(22, 0.6f, speed, _moving);
            _parallax.AddZone(23, 0.6f, speed, _moving);
            _parallax.AddZone(24, 0.6f, speed, _moving);
            _parallax.AddZone(25, 0.6f, speed, _moving);
            _parallax.AddZone(26, 0.6f, speed, _moving);
            _parallax.AddZone(27, 0.6f, speed, _moving);
            _parallax.AddZone(28, 0.6f, speed, _moving);
            _parallax.AddZone(29, 0.6f, speed, _moving);
            Level.Add(_parallax);
        }

        public override void Update() => base.Update();

        public override void Terminate() => Level.Remove(_parallax);
    }
}
