namespace DuckGame
{
    public class MultiplayerSetup : Level
    {
        public int roundsPerSet = 8;
        public int setsPerGame = 3;

        public override void Initialize()
        {
            camera.x = 480f;
            UIMenu menu = new UIMenu("MULTIPLAYER", Graphics.width / 2f, Graphics.height / 2f, 160f)
            {
                scale = new Vec2(4f)
            };
            menu.Add(new UIMenuItemNumber("ROUNDS PER SET", field: new FieldBinding(this, "roundsPerSet", max: 50f)), true);
            menu.Add(new UIMenuItemNumber("SETS PER GAME", field: new FieldBinding(this, "setsPerGame", max: 50f)), true);
            menu.Add(new UIText(" ", Color.White), true);
            menu.Add(new UIMenuItem(Triggers.Start, new UIMenuActionChangeLevel(menu, new TeamSelect2())), true);
            menu.Add(new UIMenuItem("BACK", new UIMenuActionChangeLevel(menu, new TitleScreen())), true);
            Add(menu);
            base.Initialize();
        }

        public override void Update()
        {
        }
    }
}
