// Decompiled with JetBrains decompiler
// Type: DuckGame.EnemySpawn
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class EnemySpawn : Thing
    {
        private SpriteMap _spawnSprite;

        public EnemySpawn(float xpos = 0f, float ypos = 0f)
          : base(xpos, ypos)
        {
            GraphicList graphicList = new GraphicList();
            SpriteMap graphic = new SpriteMap("duck", 32, 32)
            {
                depth = (Depth)0.9f,
                position = new Vec2(-8f, -18f)
            };
            graphicList.Add(graphic);
            this._spawnSprite = new SpriteMap("spawnSheet", 16, 16)
            {
                depth = (Depth)0.95f
            };
            graphicList.Add(_spawnSprite);
            this.graphic = graphicList;
            this._editorName = "enemy spawn";
            this.center = new Vec2(8f, 8f);
            this.collisionSize = new Vec2(16f, 16f);
            this.collisionOffset = new Vec2(-8f, -8f);
        }

        public override void Draw()
        {
            this._spawnSprite.frame = 0;
            base.Draw();
        }
    }
}
