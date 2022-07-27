// Decompiled with JetBrains decompiler
// Type: DuckGame.ImportMachine
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    [EditorGroup("Special|Arcade", EditorItemType.Arcade)]
    [BaggedProperty("isOnlineCapable", false)]
    public class ImportMachine : ArcadeMachine
    {
        public EditorProperty<bool> Underlay_Style;
        public EditorProperty<int> Style_Offset_X;
        public EditorProperty<int> Style_Offset_Y;
        public EditorProperty<int> Screen_Offset_X;
        public EditorProperty<int> Screen_Offset_Y;

        public override void EditorPropertyChanged(object property)
        {
            this._underlayStyle = this.Underlay_Style.value;
            this._styleOffsetX = this.Style_Offset_X.value;
            this._styleOffsetY = this.Style_Offset_Y.value;
            this._screenOffsetX = this.Screen_Offset_X.value;
            this._screenOffsetY = this.Screen_Offset_Y.value;
            base.EditorPropertyChanged(property);
        }

        public ImportMachine(float xpos, float ypos)
          : base(xpos, ypos, null, 0)
        {
            this.Underlay_Style = new EditorProperty<bool>(true, this);
            this.Style_Offset_X = new EditorProperty<int>(0, this, -16f, 16f, 1f);
            this.Style_Offset_Y = new EditorProperty<int>(0, this, -16f, 16f, 1f);
            this.Screen_Offset_X = new EditorProperty<int>(0, this, -32f, 32f, 1f);
            this.Screen_Offset_Y = new EditorProperty<int>(0, this, -32f, 32f, 1f);
            this._contextMenuFilter.Add("lit");
            this._contextMenuFilter.Add("style");
            this._contextMenuFilter.Add("requirement");
            this._contextMenuFilter.Add("respect");
            this._sprite = new SpriteMap("arcade/userMachine", 48, 48);
            this.graphic = _sprite;
            this.depth = -0.5f;
            this.center = new Vec2(this._sprite.width / 2 - 1, this._sprite.h / 2 + 6);
            this.lit.value = false;
            this.Underlay_Style._tooltip = "If disabled, the Arcade Machine art will be completely replaced by your custom style.";
        }

        public override void Initialize()
        {
            this.lit = (EditorProperty<bool>)false;
            base.Initialize();
        }
    }
}
