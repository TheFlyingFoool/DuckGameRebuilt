// Decompiled with JetBrains decompiler
// Type: DuckGame.FreeSpawn
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

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
            this.graphic = spriteMap;
            this._editorName = "Spawn Point";
            this.center = new Vec2(16f, 23f);
            this.collisionSize = new Vec2(16f, 16f);
            this.collisionOffset = new Vec2(-8f, -16f);
            this._visibleInGame = false;
            this.editorTooltip = "Basic spawn point for a single Duck. Every level needs at least one.";
            this.secondSpawn._tooltip = "If set, this duck will be the alternate duck in a 1V1 pair.";
        }

        public override void Draw()
        {
            this.frame = (int)this.spawnType;
            if (this.secondSpawn.value)
                this.frame = 3;
            if (this.eightPlayerOnly.value)
            {
                if (this._eight == null)
                {
                    this._eight = new SpriteMap("redEight", 10, 10);
                    this._eight.CenterOrigin();
                }
                Graphics.Draw(_eight, this.x - 5f, this.y + 7f, (Depth)1f);
            }
            base.Draw();
        }
    }
}
