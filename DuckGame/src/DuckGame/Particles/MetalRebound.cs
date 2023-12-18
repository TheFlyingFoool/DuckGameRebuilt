namespace DuckGame
{
    public class MetalRebound : Thing
    {
        public static int kMaxObjects = 32;
        public static MetalRebound[] _objects = new MetalRebound[kMaxObjects];
        public static int _lastActiveObject = 0;
        private SpriteMap _sprite;

        public static MetalRebound New(float xpos, float ypos, int offDir)
        {
            MetalRebound metalRebound;
            if (_objects[_lastActiveObject] == null)
            {
                metalRebound = new MetalRebound();
                _objects[_lastActiveObject] = metalRebound;
            }
            else
                metalRebound = _objects[_lastActiveObject];
            _lastActiveObject = (_lastActiveObject + 1) % kMaxObjects;
            metalRebound.Init(xpos, ypos, offDir);
            metalRebound.ResetProperties();
            return metalRebound;
        }

        public MetalRebound()
          : base()
        {
            _sprite = new SpriteMap("metalRebound", 16, 16);
            graphic = _sprite;
        }

        private void Init(float xpos, float ypos, int offDir)
        {
            position.x = xpos;
            position.y = ypos;
            alpha = 1f;
            _sprite.frame = Rando.Int(3);
            _sprite.flipH = offDir < 0;
            center = new Vec2(16f, 8f);
        }

        public override void Update()
        {
            alpha -= 0.1f;
            if (alpha >= 0)
                return;
            Level.Remove(this);
        }
    }
}
