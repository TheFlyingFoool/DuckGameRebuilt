namespace DuckGame
{
    [ClientOnly]
    [EditorGroup("Rebuilt|Details")]
    public class WindTile : Thing
    {
        public EditorProperty<float> wind = new EditorProperty<float>(0, null, -5, 5);
        public WindTile(float xpos, float ypos) : base(xpos, ypos)
        {
            graphic = new Sprite("wind");
            center = new Vec2(8);
            collisionSize = new Vec2(16);
            _collisionOffset = new Vec2(-8);
            _visibleInGame = false;
            maxPlaceable = 1;
            _editorName = "Wind";
            editorTooltip = "Adds a nice breeze of wind to the level, or a hurricane.";
        }
        public override void Terminate()
        {
            GameLevel.rainwind = 0;
        }
        public override void Update()
        {
            GameLevel.rainwind = wind.value;
        }
        public override void EditorUpdate()
        {
            GameLevel.rainwind = wind.value;
        }
    }
}
