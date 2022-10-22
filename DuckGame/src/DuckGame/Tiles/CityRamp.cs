// Decompiled with JetBrains decompiler
// Type: DuckGame.CityRamp
//removed for regex reasons Culture=neutral, PublicKeyToken=null
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
            _canFlipVert = true;
            graphic = new SpriteMap("cityWedge", 17, 17);
            hugWalls = WallHug.Left | WallHug.Right | WallHug.Floor;
            center = new Vec2(8f, 14f);
            collisionSize = new Vec2(14f, 8f);
            collisionOffset = new Vec2(-7f, -6f);
            _editorName = "Ramp";
            depth = -0.9f;
        }
    }
}
