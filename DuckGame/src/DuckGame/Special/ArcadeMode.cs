namespace DuckGame
{
    [EditorGroup("Special", EditorItemType.Arcade)]
    [BaggedProperty("isOnlineCapable", false)]
    public class ArcadeMode : Thing
    {
        public ArcadeMode()
          : base()
        {
            graphic = new Sprite("arcadeIcon");
            center = new Vec2(8f, 8f);
            _collisionSize = new Vec2(16f, 16f);
            _collisionOffset = new Vec2(-8f, -8f);
            depth = (Depth)0.9f;
            layer = Layer.Foreground;
            _visibleInGame = false;
            _editorName = "Arcade";
            _canFlip = false;
            _canHaveChance = false;
        }

        public override void Initialize()
        {
        }

        public override void Update()
        {
        }
    }
}
