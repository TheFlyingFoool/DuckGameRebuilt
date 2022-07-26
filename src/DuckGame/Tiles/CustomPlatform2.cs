// Decompiled with JetBrains decompiler
// Type: DuckGame.CustomPlatform2
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    [EditorGroup("Blocks|custom", EditorItemType.Custom)]
    [BaggedProperty("isInDemo", false)]
    public class CustomPlatform2 : CustomPlatform
    {
        private static CustomType _customType = CustomType.Platform;

        public static string customPlatform02
        {
            get => Custom.data[CustomPlatform2._customType][1];
            set
            {
                Custom.data[CustomPlatform2._customType][1] = value;
                Custom.Clear(CustomType.Platform, value);
            }
        }

        public CustomPlatform2(float x, float y, string tset)
          : base(x, y, "CUSTOMPLAT02")
        {
            this.customIndex = 1;
            this._editorName = "Custom Platform 02";
            this.physicsMaterial = PhysicsMaterial.Metal;
            this.verticalWidth = 14f;
            this.verticalWidthThick = 15f;
            this.horizontalHeight = 8f;
            this.UpdateCurrentTileset();
        }
    }
}
