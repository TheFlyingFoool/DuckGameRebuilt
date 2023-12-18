namespace DuckGame
{
    [EditorGroup("Details|Terrain")]
    public class RockWall : Block, IBigStupidWall
    {
        private Sprite _wall;

        public RockWall(float xpos, float ypos, System.Type c = null)
          : base(xpos, ypos)
        {
            graphic = new Sprite("laserSpawner");
            center = new Vec2(8f, 8f);
            collisionSize = new Vec2(12f, 12f);
            collisionOffset = new Vec2(-6f, -6f);
            depth = -0.6f;
            hugWalls = WallHug.None;
            layer = Layer.Foreground;
            physicsMaterial = PhysicsMaterial.Metal;
            _visibleInGame = true;
            _wall = new Sprite("rockWall");
            _wall.center = new Vec2(_wall.w - 4, _wall.h / 2);
            editorTooltip = "Adds an infinite vertical rock wall.";
        }

        public override void Initialize()
        {
            if (!(Level.current is Editor))
            {
                collisionSize = new Vec2(64f, 4096f);
                collisionOffset = new Vec2(-61f, -700f);
            }
            base.Initialize();
        }

        public override void Draw()
        {
            _wall.flipH = flipHorizontal;
            if (!(Level.current is Editor))
            {
                Graphics.Draw(_wall, x, y);
                if (Level.current.topLeft.y < y - 500)
                    Graphics.Draw(_wall, x, y - _wall.h);
                if (Level.current.bottomRight.y <= y + 500)
                    return;
                Graphics.Draw(_wall, x, y + _wall.h);
            }
            else
            {
                Graphics.DrawLine(position, position + new Vec2(flipHorizontal ? 16f : -16f, 0f), Color.Red);
                base.Draw();
            }
        }
    }
}
