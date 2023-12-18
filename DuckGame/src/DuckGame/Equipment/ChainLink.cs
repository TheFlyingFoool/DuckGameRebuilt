namespace DuckGame
{
    public class ChainLink : PhysicsObject
    {
        public ChainLink(float xpos, float ypos)
        {
            graphic = new Sprite("chainLink");
            center = new Vec2(3f, 2f);
            _collisionOffset = new Vec2(-2f, -2f);
            _collisionSize = new Vec2(4f, 4f);
            _skipPlatforms = true;
            weight = 0.1f;
            _impactThreshold = 999f;
        }
    }
}
