namespace DuckGame
{
    [EditorGroup("Details|Lights", EditorItemType.Lighting)]
    [BaggedProperty("isInDemo", true)]
    public class Sun : Thing
    {
        public Sun(float xpos, float ypos)
          : base(xpos, ypos)
        {
            SpriteMap s = new SpriteMap("backgroundIcons", 16, 16);
            s.frame = 0;
            graphic = s;

            center = new Vec2(8, 8);
            _collisionSize = new Vec2(14, 14);
            _collisionOffset = new Vec2(-7, -7);

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
