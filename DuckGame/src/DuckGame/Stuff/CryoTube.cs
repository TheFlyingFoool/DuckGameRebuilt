namespace DuckGame
{
    [EditorGroup("survival")]
    [BaggedProperty("isOnlineCapable", false)]
    public class CryoTube : Thing
    {
        private CryoPlug _plug;

        public CryoTube(float xpos, float ypos)
          : base(xpos, ypos)
        {
            graphic = new Sprite("survival/cryoTube");
            center = new Vec2(16f, 15f);
            _collisionSize = new Vec2(18f, 32f);
            _collisionOffset = new Vec2(-9f, -16f);
            depth = (Depth)0.9f;
            hugWalls = WallHug.Floor;
        }

        public override void Initialize()
        {
            _plug = new CryoPlug(x - 20f, y);
            Level.Add(_plug);
            _plug.AttachTo(this);
        }

        public override void Terminate() => Level.Remove(_plug);
    }
}
