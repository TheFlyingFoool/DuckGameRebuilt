// Decompiled with JetBrains decompiler
// Type: DuckGame.CustomPlatform3
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    [EditorGroup("Blocks|custom", EditorItemType.Custom)]
    [BaggedProperty("isInDemo", false)]
    public class CustomPlatform3 : CustomPlatform
    {
        private static CustomType _customType = CustomType.Platform;

        public static string customPlatform03
        {
            get => Custom.data[CustomPlatform3._customType][2];
            set
            {
                Custom.data[CustomPlatform3._customType][2] = value;
                Custom.Clear(CustomType.Platform, value);
            }
        }

        public CustomPlatform3(float x, float y, string tset)
          : base(x, y, "CUSTOMPLAT03")
        {
            this.customIndex = 2;
            this._editorName = "Custom Platform 03";
            this.physicsMaterial = PhysicsMaterial.Metal;
            this.verticalWidth = 14f;
            this.verticalWidthThick = 15f;
            this.horizontalHeight = 8f;
            this.UpdateCurrentTileset();
        }
    }
}
