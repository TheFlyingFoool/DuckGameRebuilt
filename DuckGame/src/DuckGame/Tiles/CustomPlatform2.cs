namespace DuckGame
{
    [EditorGroup("Blocks|custom", EditorItemType.Custom)]
    [BaggedProperty("isInDemo", false)]
    public class CustomPlatform2 : CustomPlatform
    {
        private static CustomType _customType = CustomType.Platform;

        public static string customPlatform02
        {
            get => Custom.data[_customType][1];
            set
            {
                Custom.data[_customType][1] = value;
                Custom.Clear(CustomType.Platform, value);
            }
        }

        public CustomPlatform2(float x, float y, string tset)
          : base(x, y, "CUSTOMPLAT02")
        {
            customIndex = 1;
            _editorName = "Custom Platform 02";
            physicsMaterial = PhysicsMaterial.Metal;
            verticalWidth = 14f;
            verticalWidthThick = 15f;
            horizontalHeight = 8f;
            UpdateCurrentTileset();
        }
    }
}
