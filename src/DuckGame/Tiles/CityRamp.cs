// Decompiled with JetBrains decompiler
// Type: DuckGame.CityRamp
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    [EditorGroup("Details|Terrain")]
    public class CityRamp : IceWedge
    {
        public CityRamp(float xpos, float ypos, int dir)
          : base(xpos, ypos, dir)
        {
            this._canFlipVert = true;
            this.graphic = new SpriteMap("cityWedge", 17, 17);
            this.hugWalls = WallHug.Left | WallHug.Right | WallHug.Floor;
            this.center = new Vec2(8f, 14f);
            this.collisionSize = new Vec2(14f, 8f);
            this.collisionOffset = new Vec2(-7f, -6f);
            this._editorName = "Ramp";
            this.depth = - 0.9f;
        }
    }
}
