// Decompiled with JetBrains decompiler
// Type: DuckGame.PyramidBrick
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    [EditorGroup("Stuff|Pyramid", EditorItemType.Pyramid)]
    public class PyramidBrick : Block
    {
        public PyramidBrick(float xpos, float ypos)
          : base(xpos, ypos)
        {
            this.graphic = new Sprite("pyramidBrick");
            this.center = new Vec2(8f, 8f);
            this.collisionOffset = new Vec2(-8f, -8f);
            this.collisionSize = new Vec2(16f, 16f);
            this.depth = -0.5f;
            this._editorName = "Pyramid Block";
            this.thickness = 4f;
            this.physicsMaterial = PhysicsMaterial.Metal;
        }
    }
}
