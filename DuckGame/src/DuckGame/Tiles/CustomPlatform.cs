namespace DuckGame
{
    [EditorGroup("Blocks|custom", EditorItemType.Custom)]
    [BaggedProperty("isInDemo", false)]
    public class CustomPlatform : AutoPlatform
    {
        private static CustomType _customType = CustomType.Platform;
        public int customIndex;
        private string _currentTileset = "";

        public static string customPlatform01
        {
            get => Custom.data[_customType][0];
            set
            {
                Custom.data[_customType][0] = value;
                Custom.Clear(CustomType.Platform, value);
            }
        }

        public CustomPlatform(float x, float y, string t = "CUSTOMPLAT01")
          : base(x, y, "")
        {
            _tileset = t;
            customIndex = 0;
            _editorName = "Custom Platform 01";
            physicsMaterial = PhysicsMaterial.Metal;
            verticalWidth = 14f;
            verticalWidthThick = 15f;
            horizontalHeight = 8f;
            UpdateCurrentTileset();
        }

        public void UpdateCurrentTileset()
        {
            CustomTileData data = Custom.GetData(customIndex, _customType);
            int num = 0;
            if (_sprite != null)
                num = _sprite.frame;
            if (data != null && data.texture != null)
            {
                _sprite = new SpriteMap((Tex2D)data.texture, 16, 16);
                horizontalHeight = data.horizontalHeight;
                verticalWidth = data.verticalWidth;
                verticalWidthThick = data.verticalWidthThick;
                _hasLeftNub = data.leftNubber;
                _hasRightNub = data.rightNubber;
            }
            else
            {
                _sprite = new SpriteMap("scaffolding", 16, 16);
                verticalWidth = 14f;
                verticalWidthThick = 15f;
                horizontalHeight = 8f;
            }
            if (horizontalHeight == 0)
                horizontalHeight = 8f;
            if (verticalWidth == 0)
                verticalWidth = 14f;
            if (verticalWidthThick == 0)
                verticalWidthThick = 15f;
            _sprite.frame = num;
            _tileset = "CUSTOMPLAT0" + (customIndex + 1).ToString();
            _currentTileset = Custom.data[_customType][customIndex];
            graphic = _sprite;
            UpdateNubbers();
        }

        public override void Update() => base.Update();

        public override void EditorUpdate()
        {
            if (!(Level.current is Editor) || !(_currentTileset != Custom.data[_customType][customIndex]))
                return;
            UpdateCurrentTileset();
        }

        public override ContextMenu GetContextMenu()
        {
            EditorGroupMenu contextMenu = new EditorGroupMenu(null, true);
            contextMenu.AddItem(new ContextFile("style", null, new FieldBinding(this, "customPlatform0" + (customIndex + 1).ToString()), ContextFileType.Platform));
            return contextMenu;
        }
    }
}
