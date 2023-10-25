namespace DuckGame
{
    public class StartLight : Thing
    {
        private SpriteMap _sprite;

        public StartLight()
          : base()
        {
            _sprite = new SpriteMap("trafficLight", 42, 23);
            center = new Vec2(_sprite.w / 2, _sprite.h / 2);
            graphic = _sprite;
            layer = Layer.HUD;
            x = Layer.HUD.camera.width / 2f;
            y = 20f;
        }

        public override void Update()
        {
        }
    }
}
