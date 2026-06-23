namespace DuckGame
{
    public class ConnectingScreen : Level
    {
        private float _dots;

        public ConnectingScreen() => _centeredView = true;

        public override void Initialize()
        {
            DuckNetwork.ClosePauseMenu();
            ConnectionStatusUI.Hide();
            base.Initialize();
        }

        public override void Draw()
        {
            if (MonoMain.UpdateLerpState)
            {
                _dots += 0.01f;
                if (_dots > 1f) _dots = 0f;
            }
            string str = "";
            for (int index = 0; index < 3; ++index)
            {
                if (_dots * 4f > index + 1) str += ".";
            }
            string text = "Connecting";
            Graphics.DrawString(text + str, new Vec2((Layer.HUD.width / 2f - Graphics.GetStringWidth(text) / 2f), (Layer.HUD.height / 2f - 4f)), Color.White);
        }
    }
}
