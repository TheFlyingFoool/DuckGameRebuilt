namespace DuckGame
{
    [EditorGroup("Stuff|Ropes")]
    [BaggedProperty("canSpawn", false)]
    [BaggedProperty("isOnlineCapable", false)]
    public class PhysicsChain : PhysicsRope
    {
        public PhysicsChain(float xpos, float ypos, PhysicsRope next = null)
          : base(xpos, ypos)
        {
            chain = true;
            _vine = new SpriteMap("chain", 16, 16);
            graphic = _vine;
            center = new Vec2(8f, 8f);
            _vineEnd = new Sprite("chainStretchEnd")
            {
                center = new Vec2(8f, 0f)
            };
            collisionOffset = new Vec2(-5f, -4f);
            collisionSize = new Vec2(11f, 7f);
            graphic = _vine;
            _beam = new Sprite("chainStretch");
            _editorName = "Chain";
            editorTooltip = "It's like a metal rope! Great for swinging through a factory.";
        }

        public override Vine GetSection(float x, float y, int div) => new ChainPart(x, y, div);
    }
}
