namespace DuckGame
{
    public class PlasmaFlare : Thing
    {
        private SpriteMap _sprite;

        public PlasmaFlare(float xpos, float ypos)
          : base(xpos, ypos)
        {
            _sprite = new SpriteMap("plasmaFlare", 16, 16);
            _sprite.AddAnimation("idle", 0.7f, false, 0, 1, 2);
            _sprite.SetAnimation("idle");
            graphic = _sprite;
            center = new Vec2(0f, 16f);
        }

        public override void Update()
        {
            if (!_sprite.finished)
                return;
            Level.Remove(this);
        }
    }
}
