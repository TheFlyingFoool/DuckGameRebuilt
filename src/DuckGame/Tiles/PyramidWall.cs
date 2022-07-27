// Decompiled with JetBrains decompiler
// Type: DuckGame.PyramidWall
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
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
            this.graphic = new Sprite("pyramidEdge");
            this.collisionSize = new Vec2(200f, 153f);
            this.collisionOffset = new Vec2(-4f, -4f);
            this._corner = new Sprite("pyWallCorner");
            this._corner2 = new Sprite("pyWallCorner2");
            this.physicsMaterial = PhysicsMaterial.Metal;
            this.depth = -0.9f;
        }

        public override void Initialize() => base.Initialize();

        public void AddExtraWalls()
        {
            if (this.hasUp)
            {
                for (int index = -1; index < 13; ++index)
                {
                    Vec2 point = new Vec2((float)((double)this.left + index * 16 + 12.0), this.top - 4f);
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
            if (this.hasDown)
            {
                for (int index = -1; index < 13; ++index)
                {
                    Vec2 point = new Vec2((float)((double)this.left + index * 16 + 12.0), this.bottom + 4f);
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
            if (this.hasLeft)
            {
                for (int index = 0; index < 9; ++index)
                {
                    Vec2 point = new Vec2(this.left - 4f, (float)((double)this.top + index * 16 + 12.0));
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
            if (!this.hasRight)
                return;
            for (int index = 0; index < 9; ++index)
            {
                Vec2 point = new Vec2(this.right + 4f, (float)((double)this.top + index * 16 + 12.0));
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
            this.graphic.depth = -0.8f;
            Graphics.Draw(this.graphic, this.x - 8f, this.y - 8f, new Rectangle(0.0f, 0.0f, 208f, 8f));
            this.graphic.depth = -0.85f;
            Graphics.Draw(this.graphic, this.x, this.y + 144f, new Rectangle(8f, 152f, 192f, 8f));
            this.graphic.depth = -0.86f;
            Graphics.Draw(this.graphic, this.x + 192f, this.y, new Rectangle(200f, 8f, 8f, 144f));
            Graphics.Draw(this.graphic, this.x - 8f, this.y - 8f, new Rectangle(0.0f, 0.0f, 8f, 152f));
            this._corner.depth = -0.9f;
            Graphics.Draw(this._corner, this.x - 8f, this.y + 144f);
            this._corner2.depth = -0.9f;
            Graphics.Draw(this._corner2, this.x + 192f, this.y + 144f);
            this.graphic.depth = -0.7f;
            Graphics.Draw(this.graphic, this.x, this.y, new Rectangle(8f, 8f, 192f, 144f));
            if (!DevConsole.showCollision)
                return;
            Graphics.DrawRect(this.rectangle, Color.Red, (Depth)1f, false);
            if (this.hasUp)
            {
                for (int index = 0; index < 12; ++index)
                {
                    Vec2 vec2 = new Vec2((float)((double)this.left + index * 16 + 12.0), this.top - 2f);
                    Graphics.DrawRect(vec2 + new Vec2(-2f, -2f), vec2 + new Vec2(2f, 2f), Color.Orange, (Depth)1f);
                }
            }
            if (this.hasDown)
            {
                for (int index = -1; index < 13; ++index)
                {
                    Vec2 vec2 = new Vec2((float)((double)this.left + index * 16 + 12.0), this.bottom + 2f);
                    Graphics.DrawRect(vec2 + new Vec2(-2f, -2f), vec2 + new Vec2(2f, 2f), Color.Orange, (Depth)1f);
                }
            }
            if (this.hasLeft)
            {
                for (int index = 0; index < 9; ++index)
                {
                    Vec2 vec2 = new Vec2(this.left - 2f, (float)((double)this.top + index * 16 + 12.0));
                    Graphics.DrawRect(vec2 + new Vec2(-2f, -2f), vec2 + new Vec2(2f, 2f), Color.Orange, (Depth)1f);
                }
            }
            if (!this.hasRight)
                return;
            for (int index = 0; index < 9; ++index)
            {
                Vec2 vec2 = new Vec2(this.right + 2f, (float)((double)this.top + index * 16 + 12.0));
                Graphics.DrawRect(vec2 + new Vec2(-2f, -2f), vec2 + new Vec2(2f, 2f), Color.Orange, (Depth)1f);
            }
        }
    }
}
