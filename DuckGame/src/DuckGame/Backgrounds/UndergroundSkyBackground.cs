namespace DuckGame
{
    public class UndergroundSkyBackground : BackgroundUpdater
    {
        private float _speedMult;
        private bool _moving;

        public UndergroundSkyBackground(float xpos, float ypos, bool moving = false, float speedMult = 1f)
          : base(xpos, ypos)
        {
            graphic = new SpriteMap("backgroundIcons", 16, 16)
            {
                frame = 1
            };
            center = new Vec2(8f, 8f);
            _collisionSize = new Vec2(16f, 16f);
            _collisionOffset = new Vec2(-8f, -8f);
            depth = (Depth)0.9f;
            layer = Layer.Foreground;
            _visibleInGame = false;
            _speedMult = speedMult;
            _moving = moving;
            visible = false;
        }

        public override void Initialize()
        {
            if (Level.current is Editor)
                return;
            Level.current.backgroundColor = new Color(0, 0, 0);
            _parallax = new ParallaxBackground("background/underground", 0f, 0f, 5);
            float speed = 0.9f * _speedMult;
            float distance = 0.99f;
            _parallax.AddZone(0, distance, speed);
            _parallax.AddZone(1, distance, speed);
            _parallax.AddZone(2, distance, speed);
            _parallax.AddZone(3, distance, speed);
            _parallax.AddZone(4, distance, speed);
            _parallax.AddZone(5, distance, speed);
            _parallax.AddZone(6, distance, speed);
            _parallax.AddZone(7, distance, speed);
            _parallax.AddZone(8, distance, speed);
            _parallax.AddZone(9, distance, speed);
            _parallax.AddZone(10, distance, speed);
            Level.Add(_parallax);
            _parallax.x -= 340f;
            _parallax.restrictBottom = false;
            _parallax.depth = -0.9f;
            _parallax.layer = new Layer("PARALLAX3", 115, new Camera(0f, 0f, 320f, 200f))
            {
                aspectReliesOnGameLayer = true,
                allowTallAspect = true
            };
            overrideBaseScissorCall = true;
            Layer.Add(_parallax.layer);
            Level.Add(_parallax);
        }

        public override void Terminate() => Level.Remove(_parallax);
    }
}
