namespace DuckGame
{
    [EditorGroup("survival")]
    [BaggedProperty("isOnlineCapable", false)]
    public class PowerSocket : Thing
    {
        public PowerSocket(float xpos, float ypos)
          : base(xpos, ypos)
        {
            graphic = new Sprite("survival/cryoSocket");
            center = new Vec2(8f, 8f);
            _collisionSize = new Vec2(14f, 14f);
            _collisionOffset = new Vec2(-7f, -7f);
            depth = -0.9f;
        }
    }
}
