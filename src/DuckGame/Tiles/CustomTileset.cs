// Decompiled with JetBrains decompiler
// Type: DuckGame.CustomTileset
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    [EditorGroup("Blocks|custom", EditorItemType.Custom)]
    [BaggedProperty("isInDemo", false)]
    public class CustomTileset : AutoBlock
    {
        private static CustomType _customType;
        public int customIndex;
        private string _currentTileset = "";

        public static string customTileset01
        {
            get => Custom.data[CustomTileset._customType][0];
            set
            {
                Custom.data[CustomTileset._customType][0] = value;
                Custom.Clear(CustomType.Block, value);
            }
        }

        public CustomTileset(float x, float y, string tset = "CUSTOM01")
          : base(x, y, "")
        {
            _tileset = tset;
            customIndex = 0;
            _editorName = "Custom Block 01";
            physicsMaterial = PhysicsMaterial.Metal;
            verticalWidthThick = 16f;
            verticalWidth = 14f;
            horizontalHeight = 16f;
            UpdateCurrentTileset();
        }

        public void UpdateCurrentTileset()
        {
            CustomTileData data = Custom.GetData(customIndex, CustomTileset._customType);
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
                _sprite = new SpriteMap("blueprintTileset", 16, 16);
                verticalWidthThick = 16f;
                verticalWidth = 14f;
                horizontalHeight = 16f;
            }
            if (horizontalHeight == 0.0)
                horizontalHeight = 16f;
            if (verticalWidth == 0.0)
                verticalWidth = 14f;
            if (verticalWidthThick == 0.0)
                verticalWidthThick = 16f;
            _sprite.frame = num;
            _tileset = "CUSTOM0" + (customIndex + 1).ToString();
            _currentTileset = Custom.data[CustomTileset._customType][customIndex];
            graphic = _sprite;
            UpdateNubbers();
        }

        public override void Update() => base.Update();

        public override void EditorUpdate()
        {
            if (!(Level.current is Editor) || !(_currentTileset != Custom.data[CustomTileset._customType][customIndex]))
                return;
            UpdateCurrentTileset();
        }

        public override void Draw() => base.Draw();

        public override ContextMenu GetContextMenu()
        {
            EditorGroupMenu contextMenu = new EditorGroupMenu(null, true);
            contextMenu.AddItem(new ContextFile("style", null, new FieldBinding(this, "customTileset0" + (customIndex + 1).ToString()), ContextFileType.Block));
            return contextMenu;
        }
    }
}
