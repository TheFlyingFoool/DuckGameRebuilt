namespace DuckGame
{
    public class BananaSlip : Thing
    {
        private SpriteMap _sprite;

        public BananaSlip(float xpos, float ypos, bool flip)
          : base(xpos, ypos)
        {
            _sprite = new SpriteMap("slip", 32, 32);
            _sprite.AddAnimation("slip", 0.45f, false, 0, 1, 2, 3);
            _sprite.SetAnimation("slip");
            center = new Vec2(19f, 31f);
            graphic = _sprite;
            _sprite.flipH = flip;
        }

        public override void Update()
        {
            if (!_sprite.finished)
                return;
            Level.Remove(this);
        }

        public override void Draw() => base.Draw();
    }
}
