// Decompiled with JetBrains decompiler
// Type: DuckGame.PyramidWall
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class PyramidWall : Block, IBigStupidWall
    {
        private Sprite _corner;
        private Sprite _corner2;
        private Vec2 levelCenter = new Vec2(242f, 100f);
        public bool hasLeft;
        public bool hasRight;
        public bool hasUp;
        public bool hasDown;

        public PyramidWall(float xpos, float ypos)
          : base(xpos, ypos)
        {
            graphic = new Sprite("pyramidEdge");
            collisionSize = new Vec2(200f, 153f);
            collisionOffset = new Vec2(-4f, -4f);
            _corner = new Sprite("pyWallCorner");
            _corner2 = new Sprite("pyWallCorner2");
            physicsMaterial = PhysicsMaterial.Metal;
            depth = -0.9f;
        }

        public override void Initialize() => base.Initialize();

        public void AddExtraWalls()
        {
            if (hasUp)
            {
                for (int index = -1; index < 13; ++index)
                {
                    Vec2 point = new Vec2((float)(left + index * 16 + 12.0), top - 4f);
                    switch (Level.current.CollisionPoint<Thing>(point, this))
                    {
                        case null:
                        case AutoPlatform _:
                        case BackgroundTile _:
                            Level.Add(new PyramidTileset(point.x, point.y));
                            break;
                    }
                }
            }
            if (hasDown)
            {
                for (int index = -1; index < 13; ++index)
                {
                    Vec2 point = new Vec2((float)(left + index * 16 + 12.0), bottom + 4f);
                    switch (Level.current.CollisionPoint<Thing>(point, this))
                    {
                        case null:
                        case AutoPlatform _:
                        case BackgroundTile _:
                            Level.Add(new PyramidTileset(point.x, point.y - 1f));
                            break;
                    }
                }
            }
            if (hasLeft)
            {
                for (int index = 0; index < 9; ++index)
                {
                    Vec2 point = new Vec2(left - 4f, (float)(top + index * 16 + 12.0));
                    switch (Level.current.CollisionPoint<Thing>(point, this))
                    {
                        case null:
                        case AutoPlatform _:
                        case BackgroundTile _:
                            Level.Add(new PyramidTileset(point.x, point.y));
                            break;
                    }
                }
            }
            if (!hasRight)
                return;
            for (int index = 0; index < 9; ++index)
            {
                Vec2 point = new Vec2(right + 4f, (float)(top + index * 16 + 12.0));
                switch (Level.current.CollisionPoint<Thing>(point, this))
                {
                    case null:
                    case AutoPlatform _:
                    case BackgroundTile _:
                        Level.Add(new PyramidTileset(point.x, point.y));
                        break;
                }
            }
        }

        public override void Draw()
        {
            graphic.depth = -0.8f;
            Graphics.Draw(graphic, x - 8f, y - 8f, new Rectangle(0f, 0f, 208f, 8f));
            graphic.depth = -0.85f;
            Graphics.Draw(graphic, x, y + 144f, new Rectangle(8f, 152f, 192f, 8f));
            graphic.depth = -0.86f;
            Graphics.Draw(graphic, x + 192f, y, new Rectangle(200f, 8f, 8f, 144f));
            Graphics.Draw(graphic, x - 8f, y - 8f, new Rectangle(0f, 0f, 8f, 152f));
            _corner.depth = -0.9f;
            Graphics.Draw(_corner, x - 8f, y + 144f);
            _corner2.depth = -0.9f;
            Graphics.Draw(_corner2, x + 192f, y + 144f);
            graphic.depth = -0.7f;
            Graphics.Draw(graphic, x, y, new Rectangle(8f, 8f, 192f, 144f));
            if (!DevConsole.showCollision)
                return;
            Graphics.DrawRect(rectangle, Color.Red, (Depth)1f, false);
            if (hasUp)
            {
                for (int index = 0; index < 12; ++index)
                {
                    Vec2 vec2 = new Vec2((float)(left + index * 16 + 12.0), top - 2f);
                    Graphics.DrawRect(vec2 + new Vec2(-2f, -2f), vec2 + new Vec2(2f, 2f), Color.Orange, (Depth)1f);
                }
            }
            if (hasDown)
            {
                for (int index = -1; index < 13; ++index)
                {
                    Vec2 vec2 = new Vec2((float)(left + index * 16 + 12.0), bottom + 2f);
                    Graphics.DrawRect(vec2 + new Vec2(-2f, -2f), vec2 + new Vec2(2f, 2f), Color.Orange, (Depth)1f);
                }
            }
            if (hasLeft)
            {
                for (int index = 0; index < 9; ++index)
                {
                    Vec2 vec2 = new Vec2(left - 2f, (float)(top + index * 16 + 12.0));
                    Graphics.DrawRect(vec2 + new Vec2(-2f, -2f), vec2 + new Vec2(2f, 2f), Color.Orange, (Depth)1f);
                }
            }
            if (!hasRight)
                return;
            for (int index = 0; index < 9; ++index)
            {
                Vec2 vec2 = new Vec2(right + 2f, (float)(top + index * 16 + 12.0));
                Graphics.DrawRect(vec2 + new Vec2(-2f, -2f), vec2 + new Vec2(2f, 2f), Color.Orange, (Depth)1f);
            }
        }
    }
}
