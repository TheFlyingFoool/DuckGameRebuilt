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
            this._tileset = tset;
            this.customIndex = 0;
            this._editorName = "Custom Block 01";
            this.physicsMaterial = PhysicsMaterial.Metal;
            this.verticalWidthThick = 16f;
            this.verticalWidth = 14f;
            this.horizontalHeight = 16f;
            this.UpdateCurrentTileset();
        }

        public void UpdateCurrentTileset()
        {
            CustomTileData data = Custom.GetData(this.customIndex, CustomTileset._customType);
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
                this._sprite = new SpriteMap("blueprintTileset", 16, 16);
                this.verticalWidthThick = 16f;
                this.verticalWidth = 14f;
                this.horizontalHeight = 16f;
            }
            if ((double)this.horizontalHeight == 0.0)
                this.horizontalHeight = 16f;
            if ((double)this.verticalWidth == 0.0)
                this.verticalWidth = 14f;
            if ((double)this.verticalWidthThick == 0.0)
                this.verticalWidthThick = 16f;
            this._sprite.frame = num;
            this._tileset = "CUSTOM0" + (this.customIndex + 1).ToString();
            this._currentTileset = Custom.data[CustomTileset._customType][this.customIndex];
            this.graphic = (Sprite)this._sprite;
            this.UpdateNubbers();
        }

        public override void Update() => base.Update();

        public override void EditorUpdate()
        {
            if (!(Level.current is Editor) || !(this._currentTileset != Custom.data[CustomTileset._customType][this.customIndex]))
                return;
            this.UpdateCurrentTileset();
        }

        public override void Draw() => base.Draw();

        public override ContextMenu GetContextMenu()
        {
            EditorGroupMenu contextMenu = new EditorGroupMenu((IContextListener)null, true);
            contextMenu.AddItem((ContextMenu)new ContextFile("style", (IContextListener)null, new FieldBinding((object)this, "customTileset0" + (this.customIndex + 1).ToString()), ContextFileType.Block));
            return (ContextMenu)contextMenu;
        }
    }
}
