// Decompiled with JetBrains decompiler
// Type: DuckGame.DonutTileset
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    [EditorGroup("Blocks")]
    [BaggedProperty("isInDemo", true)]
    public class DonutTileset : AutoBlock
    {
        public DonutTileset(float x, float y)
          : base(x, y, "donutTileset")
        {
            _editorName = "Donut";
            physicsMaterial = PhysicsMaterial.Crust;
            verticalWidthThick = 15f;
            verticalWidth = 12f;
            horizontalHeight = 15f;
        }
    }
}
