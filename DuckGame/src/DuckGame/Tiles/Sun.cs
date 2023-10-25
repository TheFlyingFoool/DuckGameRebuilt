namespace DuckGame
{
    [EditorGroup("Details|Lights", EditorItemType.Lighting)]
    [BaggedProperty("isInDemo", true)]
    public class Sun : Thing
    {
        public Sun(float xpos, float ypos)
          : base(xpos, ypos)
        {
            graphic = new Sprite("officeLight");
            center = new Vec2(16f, 3f);
            _collisionSize = new Vec2(30f, 6f);
            _collisionOffset = new Vec2(-15f, -3f);
            depth = (Depth)0.9f;
            hugWalls = WallHug.Ceiling;
            layer = Layer.Game;
            editorCycleType = typeof(TroubleLight);
        }

        public override void Initialize()
        {
            if (Level.current is Editor)
                return;
            Level.Add(new SunLight(x, y - 1f, new Color((int)byte.MaxValue, (int)byte.MaxValue, (int)byte.MaxValue), 100f));
            Level.Remove(this);
        }
    }
}
