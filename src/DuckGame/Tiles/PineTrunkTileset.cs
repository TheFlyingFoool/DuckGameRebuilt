// Decompiled with JetBrains decompiler
// Type: DuckGame.PineTrunkTileset
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    [EditorGroup("Blocks|Snow")]
    [BaggedProperty("isInDemo", false)]
    public class PineTrunkTileset : AutoPlatform
    {
        public PineTrunkTileset(float x, float y)
          : base(x, y, "pineTreeTileset")
        {
            this._editorName = "Pine Trunk";
            this.physicsMaterial = PhysicsMaterial.Default;
            this.verticalWidth = 6f;
            this.verticalWidthThick = 15f;
            this.horizontalHeight = 8f;
            this._hasNubs = false;
            this.depth = -0.6f;
            this.placementLayerOverride = Layer.Blocks;
        }

        public override void OnSolidImpact(MaterialThing with, ImpactedFrom from)
        {
            if (with.impactPowerV > 2.40000009536743)
            {
                Level.CheckPoint<PineTree>(this.x, this.y)?.KnockOffSnow(with.velocity, true);
                Level.CheckPoint<PineTree>(this.x, this.y - 16f)?.KnockOffSnow(with.velocity, true);
            }
            this.OnSoftImpact(with, from);
        }
    }
}
