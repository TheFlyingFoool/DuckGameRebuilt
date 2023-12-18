namespace DuckGame
{
    public class DisconnectError : Level
    {
        private Profile _profile;

        public DisconnectError(Profile who)
        {
            _profile = who;
            _centeredView = true;
        }

        public override void Initialize()
        {
            DuckNetwork.ClosePauseMenu();
            ConnectionStatusUI.Hide();
            HUD.AddCornerMessage(HUDCorner.BottomRight, "@START@CONTINUE");
            _startCalled = true;
            base.Initialize();
        }

        public override void Update()
        {
            if (Input.Pressed(Triggers.Start))
                current = new TitleScreen();
            base.Update();
        }

        public override void Draw()
        {
            if (_profile != null)
            {
                string text = " |RED|" + _profile.name + " has disconnected.";
                Graphics.DrawString(text, new Vec2((float)(Layer.HUD.camera.width / 2f - Graphics.GetStringWidth(text) / 2f), Layer.HUD.camera.height / 2f), Color.White);
            }
            else
            {
                string text = " |RED|The host has disconnected.";
                Graphics.DrawString(text, new Vec2((float)(Layer.HUD.camera.width / 2f - Graphics.GetStringWidth(text) / 2f), Layer.HUD.camera.height / 2f), Color.White);
            }
        }
    }
}
