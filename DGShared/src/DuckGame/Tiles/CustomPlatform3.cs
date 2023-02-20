// Decompiled with JetBrains decompiler
// Type: DuckGame.CustomPlatform3
//removed for regex reasons Culture=neutral, PublicKeyToken=null
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
            get => Custom.data[_customType][2];
            set
            {
                Custom.data[_customType][2] = value;
                Custom.Clear(CustomType.Platform, value);
            }
        }

        public CustomPlatform3(float x, float y, string tset)
          : base(x, y, "CUSTOMPLAT03")
        {
            customIndex = 2;
            _editorName = "Custom Platform 03";
            physicsMaterial = PhysicsMaterial.Metal;
            verticalWidth = 14f;
            verticalWidthThick = 15f;
            horizontalHeight = 8f;
            UpdateCurrentTileset();
        }
    }
}
