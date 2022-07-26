// Decompiled with JetBrains decompiler
// Type: DuckGame.CustomTileset3
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    [EditorGroup("Blocks|custom", EditorItemType.Custom)]
    [BaggedProperty("isInDemo", false)]
    public class CustomTileset3 : CustomTileset
    {
        private static CustomType _customType;

        public static string customTileset03
        {
            get => Custom.data[CustomTileset3._customType][2];
            set
            {
                Custom.data[CustomTileset3._customType][2] = value;
                Custom.Clear(CustomType.Block, value);
            }
        }

        public CustomTileset3(float x, float y, string tset)
          : base(x, y, "CUSTOM03")
        {
            this.customIndex = 2;
            this._editorName = "Custom Block 03";
            this.physicsMaterial = PhysicsMaterial.Metal;
            this.verticalWidthThick = 16f;
            this.verticalWidth = 14f;
            this.horizontalHeight = 16f;
            this.UpdateCurrentTileset();
        }
    }
}
