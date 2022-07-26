// Decompiled with JetBrains decompiler
// Type: DuckGame.CustomPlatform
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

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
            get => Custom.data[CustomPlatform._customType][0];
            set
            {
                Custom.data[CustomPlatform._customType][0] = value;
                Custom.Clear(CustomType.Platform, value);
            }
        }

        public CustomPlatform(float x, float y, string t = "CUSTOMPLAT01")
          : base(x, y, "")
        {
            this._tileset = t;
            this.customIndex = 0;
            this._editorName = "Custom Platform 01";
            this.physicsMaterial = PhysicsMaterial.Metal;
            this.verticalWidth = 14f;
            this.verticalWidthThick = 15f;
            this.horizontalHeight = 8f;
            this.UpdateCurrentTileset();
        }

        public void UpdateCurrentTileset()
        {
            CustomTileData data = Custom.GetData(this.customIndex, CustomPlatform._customType);
            int num = 0;
            if (this._sprite != null)
                num = this._sprite.frame;
            if (data != null && data.texture != null)
            {
                this._sprite = new SpriteMap((Tex2D)data.texture, 16, 16);
                this.horizontalHeight = (float)data.horizontalHeight;
                this.verticalWidth = (float)data.verticalWidth;
                this.verticalWidthThick = (float)data.verticalWidthThick;
                this._hasLeftNub = data.leftNubber;
                this._hasRightNub = data.rightNubber;
            }
            else
            {
                this._sprite = new SpriteMap("scaffolding", 16, 16);
                this.verticalWidth = 14f;
                this.verticalWidthThick = 15f;
                this.horizontalHeight = 8f;
            }
            if ((double)this.horizontalHeight == 0.0)
                this.horizontalHeight = 8f;
            if ((double)this.verticalWidth == 0.0)
                this.verticalWidth = 14f;
            if ((double)this.verticalWidthThick == 0.0)
                this.verticalWidthThick = 15f;
            this._sprite.frame = num;
            this._tileset = "CUSTOMPLAT0" + (this.customIndex + 1).ToString();
            this._currentTileset = Custom.data[CustomPlatform._customType][this.customIndex];
            this.graphic = (Sprite)this._sprite;
            this.UpdateNubbers();
        }

        public override void Update() => base.Update();

        public override void EditorUpdate()
        {
            if (!(Level.current is Editor) || !(this._currentTileset != Custom.data[CustomPlatform._customType][this.customIndex]))
                return;
            this.UpdateCurrentTileset();
        }

        public override ContextMenu GetContextMenu()
        {
            EditorGroupMenu contextMenu = new EditorGroupMenu((IContextListener)null, true);
            contextMenu.AddItem((ContextMenu)new ContextFile("style", (IContextListener)null, new FieldBinding((object)this, "customPlatform0" + (this.customIndex + 1).ToString()), ContextFileType.Platform));
            return (ContextMenu)contextMenu;
        }
    }
}
