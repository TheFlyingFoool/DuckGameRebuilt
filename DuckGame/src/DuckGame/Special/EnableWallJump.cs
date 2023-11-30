namespace DuckGame
{
    [EditorGroup("Special", EditorItemType.Arcade)]
    [BaggedProperty("isOnlineCapable", false)]
    public class EnableWallJump : Thing
    {
        public EnableWallJump()
          : base()
        {
            graphic = new Sprite("swirl");
            center = new Vec2(8f, 8f);
            collisionSize = new Vec2(16f, 16f);
            collisionOffset = new Vec2(-8f, -8f);
            _canFlip = false;
            _visibleInGame = false;
        }
    }
}
