using System;

namespace DuckGame
{
    [EditorGroup("Blocks|Snow")]
    [BaggedProperty("isInDemo", false)]
    public class PineTrunkTileset : AutoPlatform
    {
        public PineTrunkTileset(float x, float y)
          : base(x, y, "pineTreeTileset")
        {
            _editorName = "Pine Trunk";
            physicsMaterial = PhysicsMaterial.Default;
            verticalWidth = 6f;
            verticalWidthThick = 15f;
            horizontalHeight = 8f;
            _hasNubs = false;
            depth = -0.6f;
            placementLayerOverride = Layer.Blocks;
        }

        public override void OnSolidImpact(MaterialThing with, ImpactedFrom from)
        {
            if (with.impactPowerV > 2.4f)
            {
                Level.CheckPoint<PineTree>(x, y)?.KnockOffSnow(with.velocity, true);
                Level.CheckPoint<PineTree>(x, y - 16f)?.KnockOffSnow(with.velocity, true);
            }
            OnSoftImpact(with, from);
        }

        public override Type TabRotate(bool control)
        {
            if (control)
                return typeof(TreeTileset);
            return null;
        }
    }
}
