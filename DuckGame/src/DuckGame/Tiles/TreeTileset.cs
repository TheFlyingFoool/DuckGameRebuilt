// Decompiled with JetBrains decompiler
// Type: DuckGame.TreeTileset
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

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
