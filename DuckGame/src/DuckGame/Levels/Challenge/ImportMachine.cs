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
            _underlayStyle = Underlay_Style.value;
            _styleOffsetX = Style_Offset_X.value;
            _styleOffsetY = Style_Offset_Y.value;
            _screenOffsetX = Screen_Offset_X.value;
            _screenOffsetY = Screen_Offset_Y.value;
            base.EditorPropertyChanged(property);
        }

        public ImportMachine(float xpos, float ypos)
          : base(xpos, ypos, null, 0)
        {
            Underlay_Style = new EditorProperty<bool>(true, this);
            Style_Offset_X = new EditorProperty<int>(0, this, -16f, 16f, 1f);
            Style_Offset_Y = new EditorProperty<int>(0, this, -16f, 16f, 1f);
            Screen_Offset_X = new EditorProperty<int>(0, this, -32f, 32f, 1f);
            Screen_Offset_Y = new EditorProperty<int>(0, this, -32f, 32f, 1f);
            _contextMenuFilter.Add("lit");
            _contextMenuFilter.Add("style");
            _contextMenuFilter.Add("requirement");
            _contextMenuFilter.Add("respect");
            _sprite = new SpriteMap("arcade/userMachine", 48, 48);
            graphic = _sprite;
            depth = -0.5f;
            center = new Vec2(_sprite.width / 2 - 1, _sprite.h / 2 + 6);
            lit.value = false;
            Underlay_Style._tooltip = "If disabled, the Arcade Machine art will be completely replaced by your custom style.";
        }

        public override void Initialize()
        {
            lit = (EditorProperty<bool>)false;
            base.Initialize();
        }
    }
}
