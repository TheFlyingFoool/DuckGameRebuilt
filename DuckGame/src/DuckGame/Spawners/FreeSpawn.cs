namespace DuckGame
{
    [EditorGroup("Spawns")]
    [BaggedProperty("isInDemo", true)]
    [BaggedProperty("previewPriority", true)]
    public class FreeSpawn : SpawnPoint
    {
        public EditorProperty<int> spawnType = new EditorProperty<int>(0, max: 2f, increment: 1f);
        public EditorProperty<bool> secondSpawn = new EditorProperty<bool>(false);
        public EditorProperty<bool> eightPlayerOnly = new EditorProperty<bool>(false);
        private SpriteMap _eight;

        public FreeSpawn(float xpos = 0f, float ypos = 0f)
          : base(xpos, ypos)
        {
            SpriteMap spriteMap = new SpriteMap("duckSpawn", 32, 32)
            {
                depth = (Depth)0.9f
            };
            graphic = spriteMap;
            _editorName = "Spawn Point";
            center = new Vec2(16f, 23f);
            collisionSize = new Vec2(16f, 16f);
            collisionOffset = new Vec2(-8f, -16f);
            _visibleInGame = false;
            editorTooltip = "Basic spawn point for a single Duck. Every level needs at least one.";
            secondSpawn._tooltip = "If set, this duck will be the alternate duck in a 1V1 pair.";
        }

        public override void Draw()
        {
            frame = (int)spawnType;
            if (secondSpawn.value)
                frame = 3;
            if (eightPlayerOnly.value)
            {
                if (_eight == null)
                {
                    _eight = new SpriteMap("redEight", 10, 10);
                    _eight.CenterOrigin();
                }
                Graphics.Draw(ref _eight, x - 5f, y + 7f, (Depth)1f);
            }
            base.Draw();
        }
    }
}
