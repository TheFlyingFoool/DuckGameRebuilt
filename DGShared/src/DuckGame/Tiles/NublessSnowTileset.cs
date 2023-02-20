// Decompiled with JetBrains decompiler
// Type: DuckGame.NublessSnowTileset
//removed for regex reasons Culture=neutral, PublicKeyToken=null
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
            _editorName = "Snow NONUBS";
            physicsMaterial = PhysicsMaterial.Metal;
            verticalWidthThick = 15f;
            verticalWidth = 14f;
            horizontalHeight = 15f;
            _tileset = "snowTileset";
            _sprite = new SpriteMap("nublessSnow", 16, 16);
            graphic = _sprite;
            _sprite.frame = 40;
            willHeat = true;
            _impactThreshold = -1f;
            _hasNubs = false;
            meltedTileset = "nublessSnow";
            frozenTileset = "nublessIceTileset";
        }
    }
}
