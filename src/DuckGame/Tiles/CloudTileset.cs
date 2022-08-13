// Decompiled with JetBrains decompiler
// Type: DuckGame.CloudTileset
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    [EditorGroup("Blocks")]
    public class CloudTileset : AutoBlock
    {
        public CloudTileset(float x, float y)
          : base(x, y, "cloudTileset")
        {
            _editorName = "Kingdom";
            physicsMaterial = PhysicsMaterial.Metal;
            verticalWidth = 12f;
            verticalWidthThick = 14f;
            horizontalHeight = 13f;
        }

        public override void Draw() => base.Draw();
    }
}
