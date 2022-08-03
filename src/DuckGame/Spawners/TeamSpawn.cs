// Decompiled with JetBrains decompiler
// Type: DuckGame.TeamSpawn
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

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
                graphic.depth = (Depth)(float)(0.899999976158142 + 0.00999999977648258 * index);
                graphic.position = new Vec2((float)(index * 9.41176414489746 - 16.0 + 16.0), -2f);
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
                Graphics.Draw(_eight, x - 5f, y + 7f, (Depth)1f);
            }
            base.Draw();
        }
    }
}
