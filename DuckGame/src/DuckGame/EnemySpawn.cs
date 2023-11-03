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
            _spawnSprite = new SpriteMap("spawnSheet", 16, 16)
            {
                depth = (Depth)0.95f
            };
            graphicList.Add(_spawnSprite);
            this.graphic = graphicList;
            _editorName = "enemy spawn";
            center = new Vec2(8f, 8f);
            collisionSize = new Vec2(16f, 16f);
            collisionOffset = new Vec2(-8f, -8f);
        }

        public override void Draw()
        {
            _spawnSprite.frame = 0;
            base.Draw();
        }
    }
}
