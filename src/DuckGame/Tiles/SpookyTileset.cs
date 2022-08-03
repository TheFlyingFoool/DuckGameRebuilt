// Decompiled with JetBrains decompiler
// Type: DuckGame.SpookyTileset
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    [EditorGroup("Blocks")]
    public class SpookyTileset : AutoBlock
    {
        public SpookyTileset(float x, float y)
          : base(x, y, "spookyTileset")
        {
            _editorName = "Spooky";
            physicsMaterial = PhysicsMaterial.Wood;
            verticalWidth = 10f;
            verticalWidthThick = 15f;
            horizontalHeight = 14f;
        }

        public override void Draw() => base.Draw();
    }
}
