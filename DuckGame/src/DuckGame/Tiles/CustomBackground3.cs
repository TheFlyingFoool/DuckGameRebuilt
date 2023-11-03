namespace DuckGame
{
    [EditorGroup("Background|custom", EditorItemType.Custom)]
    [BaggedProperty("isInDemo", false)]
    public class CustomBackground3 : CustomBackground
    {
        private static CustomType _customType = CustomType.Background;

        public static string customBackground03
        {
            get => Custom.data[_customType][2];
            set
            {
                Custom.data[_customType][2] = value;
                Custom.Clear(CustomType.Background, value);
            }
        }

        public CustomBackground3(float x, float y)
          : base(x, y)
        {
            customIndex = 2;
            graphic = new SpriteMap("arcadeBackground", 16, 16, true);
            _opacityFromGraphic = true;
            center = new Vec2(8f, 8f);
            collisionSize = new Vec2(16f, 16f);
            collisionOffset = new Vec2(-8f, -8f);
            _editorName = "03";
            UpdateCurrentTileset();
        }
    }
}
