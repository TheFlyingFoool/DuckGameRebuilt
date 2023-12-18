namespace DuckGame
{
    public class WaterSplash : Thing
    {
        private SpriteMap _sprite;

        public WaterSplash(float xpos, float ypos, FluidData fluid)
          : base(xpos, ypos)
        {
            _sprite = new SpriteMap("whiteSplash", 16, 16);
            _sprite.AddAnimation("splash", 0.42f, false, 0, 1, 2, 3);
            _sprite.SetAnimation("splash");
            _sprite.color = new Color(fluid.color);
            center = new Vec2(8f, 16f);
            graphic = _sprite;
            depth = (Depth)0.7f;
        }

        public override void Update()
        {
            if (!currentlyDrawing) _sprite.UpdateFrame(true);
            if (!_sprite.finished)
                return;
            Level.Remove(this);
        }

        public override void Draw() => base.Draw();
    }
}
