namespace DuckGame
{
    [EditorGroup("Blocks|custom", EditorItemType.Custom)]
    [BaggedProperty("isInDemo", false)]
    public class CustomTileset3 : CustomTileset
    {
        private static CustomType _customType;

        public static string customTileset03
        {
            get => Custom.data[_customType][2];
            set
            {
                Custom.data[_customType][2] = value;
                Custom.Clear(CustomType.Block, value);
            }
        }

        public CustomTileset3(float x, float y, string tset)
          : base(x, y, "CUSTOM03")
        {
            customIndex = 2;
            _editorName = "Custom Block 03";
            physicsMaterial = PhysicsMaterial.Metal;
            verticalWidthThick = 16f;
            verticalWidth = 14f;
            horizontalHeight = 16f;
            UpdateCurrentTileset();
        }
    }
}
