namespace DuckGame
{
    public class MuzzleFlare : Thing
    {
        private SpriteMap _sprite;

        public MuzzleFlare(float xpos, float ypos)
          : base(xpos, ypos)
        {
            _sprite = new SpriteMap("smallFlare", 16, 16);
            graphic = _sprite;
            center = new Vec2(0f, 8f);
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
