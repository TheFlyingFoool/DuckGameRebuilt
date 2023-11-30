namespace DuckGame
{
    [EditorGroup("Background|custom", EditorItemType.Custom)]
    [BaggedProperty("isInDemo", false)]
    public class CustomBackground2 : CustomBackground
    {
        private static CustomType _customType = CustomType.Background;

        public static string customBackground02
        {
            get => Custom.data[_customType][1];
            set
            {
                Custom.data[_customType][1] = value;
                Custom.Clear(CustomType.Background, value);
            }
        }

        public CustomBackground2(float x, float y)
          : base(x, y)
        {
            customIndex = 1;
            graphic = new SpriteMap("arcadeBackground", 16, 16, true);
            _opacityFromGraphic = true;
            center = new Vec2(8f, 8f);
            collisionSize = new Vec2(16f, 16f);
            collisionOffset = new Vec2(-8f, -8f);
            _editorName = "02";
            UpdateCurrentTileset();
        }
    }
}
