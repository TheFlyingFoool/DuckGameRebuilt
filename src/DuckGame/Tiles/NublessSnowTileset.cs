// Decompiled with JetBrains decompiler
// Type: DuckGame.NublessSnowTileset
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    [EditorGroup("Blocks|Snow")]
    [BaggedProperty("isInDemo", false)]
    public class NublessSnowTileset : SnowTileset
    {
        public NublessSnowTileset(float x, float y)
          : base(x, y, "nublessSnow")
        {
            this._editorName = "Snow NONUBS";
            this.physicsMaterial = PhysicsMaterial.Metal;
            this.verticalWidthThick = 15f;
            this.verticalWidth = 14f;
            this.horizontalHeight = 15f;
            this._tileset = "snowTileset";
            this._sprite = new SpriteMap("nublessSnow", 16, 16);
            this.graphic = (Sprite)this._sprite;
            this._sprite.frame = 40;
            this.willHeat = true;
            this._impactThreshold = -1f;
            this._hasNubs = false;
            this.meltedTileset = "nublessSnow";
            this.frozenTileset = "nublessIceTileset";
        }
    }
}
