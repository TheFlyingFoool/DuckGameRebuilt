// Decompiled with JetBrains decompiler
// Type: DuckGame.NublessSnowIceTileset
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    [EditorGroup("Blocks|Snow")]
    [BaggedProperty("isInDemo", false)]
    public class NublessSnowIceTileset : SnowIceTileset
    {
        public NublessSnowIceTileset(float x, float y)
          : base(x, y, "nublessIceTileset")
        {
            _editorName = "Snow Ice NONUBS";
            physicsMaterial = PhysicsMaterial.Metal;
            verticalWidthThick = 15f;
            verticalWidth = 14f;
            horizontalHeight = 15f;
            _impactThreshold = -1f;
            willHeat = true;
            _tileset = "snowTileset";
            _sprite = new SpriteMap("nublessIceTileset", 16, 16)
            {
                frame = 40
            };
            graphic = _sprite;
            _hasNubs = false;
            meltedTileset = "nublessSnow";
            frozenTileset = "nublessIceTileset";
        }
    }
}
