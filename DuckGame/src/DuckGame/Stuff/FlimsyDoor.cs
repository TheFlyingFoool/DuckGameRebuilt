namespace DuckGame
{
    [EditorGroup("Stuff|Pyramid", EditorItemType.Pyramid)]
    public class FlimsyDoor : Door
    {
        public FlimsyDoor(float xpos, float ypos)
          : base(xpos, ypos)
        {
            _editorName = "Flimsy Door";
        }

        public override void Initialize()
        {
            secondaryFrame = true;
            _sprite = new SpriteMap("flimsyDoor", 32, 32);
            graphic = _sprite;
            colWide = 4f;
            base.Initialize();
        }
    }
}
