namespace DuckGame
{
    [EditorGroup("Spawns")]
    [BaggedProperty("isInDemo", true)]
    public class TeamSpawn : SpawnPoint
    {
        public EditorProperty<bool> eightPlayerOnly = new EditorProperty<bool>(false);
        private SpriteMap _eight;

        public TeamSpawn(float xpos = 0f, float ypos = 0f)
          : base(xpos, ypos)
        {
            GraphicList graphicList = new GraphicList();
            for (int index = 0; index < 3; ++index)
            {
                SpriteMap graphic = new SpriteMap("duck", 32, 32);
                graphic.CenterOrigin();
                graphic.depth = (Depth)(float)(0.9f + 0.01f * index);
                graphic.position = new Vec2((float)(index * 9.411764f - 16 + 16), -2f);
                graphicList.Add(graphic);
            }
            graphic = graphicList;
            _editorName = "Team Spawn";
            center = new Vec2(8f, 5f);
            collisionSize = new Vec2(32f, 16f);
            collisionOffset = new Vec2(-16f, -8f);
            _visibleInGame = false;
            editorTooltip = "Spawn point for a whole team of Ducks.";
        }

        public override void Draw()
        {
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
