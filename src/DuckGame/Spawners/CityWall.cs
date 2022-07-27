// Decompiled with JetBrains decompiler
// Type: DuckGame.CityWall
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    [EditorGroup("Details|Terrain")]
    public class CityWall : RockWall
    {
        private Sprite _wall;

        public CityWall(float xpos, float ypos, System.Type c = null)
          : base(xpos, ypos)
        {
            this.graphic = new Sprite("laserSpawner");
            this.center = new Vec2(8f, 8f);
            this.collisionSize = new Vec2(12f, 12f);
            this.collisionOffset = new Vec2(-6f, -6f);
            this.depth = - 0.6f;
            this.hugWalls = WallHug.None;
            this.layer = Layer.Foreground;
            this.physicsMaterial = PhysicsMaterial.Metal;
            this._visibleInGame = true;
            this._wall = new Sprite("citywall");
            this._wall.center = new Vec2(this._wall.w - 4, this._wall.h / 2);
            this.editorTooltip = "Adds an infinite vertical building.";
        }

        public override void Initialize()
        {
            if (!(Level.current is Editor))
            {
                this.collisionSize = new Vec2(64f, 4096f);
                this.collisionOffset = new Vec2(-61f, -700f);
            }
            base.Initialize();
        }

        public override void Draw()
        {
            this._wall.flipH = this.flipHorizontal;
            if (!(Level.current is Editor))
            {
                Graphics.Draw(this._wall, this.x, this.y);
                if (Level.current.topLeft.y < (double)this.y - 500.0)
                    Graphics.Draw(this._wall, this.x, this.y - _wall.h);
                if (Level.current.bottomRight.y <= (double)this.y + 500.0)
                    return;
                Graphics.Draw(this._wall, this.x, this.y + _wall.h);
            }
            else
            {
                Graphics.DrawLine(this.position, this.position + new Vec2(this.flipHorizontal ? 16f : -16f, 0.0f), Color.Red);
                base.Draw();
            }
        }
    }
}
