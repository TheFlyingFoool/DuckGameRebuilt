// Decompiled with JetBrains decompiler
// Type: DuckGame.CloudTileset
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
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
            this._editorName = "Kingdom";
            this.physicsMaterial = PhysicsMaterial.Metal;
            this.verticalWidth = 12f;
            this.verticalWidthThick = 14f;
            this.horizontalHeight = 13f;
        }

        public override void Draw() => base.Draw();
    }
}
