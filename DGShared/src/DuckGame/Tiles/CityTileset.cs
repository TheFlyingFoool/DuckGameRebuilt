// Decompiled with JetBrains decompiler
// Type: DuckGame.CityTileset
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    [EditorGroup("Blocks")]
    public class CityTileset : AutoBlock
    {
        public CityTileset(float x, float y)
          : base(x, y, "cityTileset")
        {
            _editorName = "City";
            physicsMaterial = PhysicsMaterial.Metal;
            verticalWidth = 10f;
            verticalWidthThick = 15f;
            horizontalHeight = 13f;
        }

        public override void Draw() => base.Draw();
    }
}
