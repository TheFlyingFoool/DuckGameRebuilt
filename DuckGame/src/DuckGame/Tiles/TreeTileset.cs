using System;

namespace DuckGame
{
    [EditorGroup("Blocks|Jump Through")]
    [BaggedProperty("isInDemo", true)]
    public class TreeTileset : AutoPlatform
    {
        public TreeTileset(float x, float y)
          : base(x, y, "treeTileset")
        {
            _editorName = "Tree";
            physicsMaterial = PhysicsMaterial.Default;
            verticalWidth = 6f;
            verticalWidthThick = 15f;
            horizontalHeight = 8f;
            _hasNubs = false;
            depth = -0.15f;
            placementLayerOverride = Layer.Blocks;
            treeLike = true;
        }

        public override Type TabRotate(bool control)
        {
            if (control)
                return typeof(PineTrunkTileset);
            else
                return typeof(ScaffoldingTileset);
        }

    }
}
