namespace DuckGame
{
    public class Elevator : MaterialThing, IPlatform
    {
        private SpriteMap _sprite;

        public Elevator(float xpos, float ypos)
          : base(xpos, ypos)
        {
            _sprite = new SpriteMap("elevator", 32, 37);
            graphic = _sprite;
            center = new Vec2(8f, 8f);
            collisionOffset = new Vec2(-8f, -6f);
            collisionSize = new Vec2(16f, 13f);
            depth = -0.5f;
            thickness = 4f;
            weight = 7f;
            flammable = 0.3f;
            collideSounds.Add("rockHitGround2");
        }
    }
}
