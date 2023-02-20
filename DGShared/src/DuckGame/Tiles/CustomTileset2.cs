// Decompiled with JetBrains decompiler
// Type: DuckGame.CustomTileset2
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    [EditorGroup("Blocks|custom", EditorItemType.Custom)]
    [BaggedProperty("isInDemo", false)]
    public class CustomTileset2 : CustomTileset
    {
        private static CustomType _customType;

        public static string customTileset02
        {
            get => Custom.data[_customType][1];
            set
            {
                Custom.data[_customType][1] = value;
                Custom.Clear(CustomType.Block, value);
            }
        }

        public CustomTileset2(float x, float y, string tset)
          : base(x, y, "CUSTOM02")
        {
            customIndex = 1;
            _editorName = "Custom Block 02";
            physicsMaterial = PhysicsMaterial.Metal;
            verticalWidthThick = 16f;
            verticalWidth = 14f;
            horizontalHeight = 16f;
            UpdateCurrentTileset();
        }
    }
}
