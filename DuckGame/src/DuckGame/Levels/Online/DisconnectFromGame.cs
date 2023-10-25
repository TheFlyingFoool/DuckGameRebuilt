namespace DuckGame
{
    public class DisconnectFromGame : Level, IConnectionScreen
    {
        private float _dots;
        private bool _disconnected;
        private ulong joinAddress;
        private float wait = 1f;
        private bool _didDisconnect;

        public DisconnectFromGame() => _centeredView = true;

        public DisconnectFromGame(ulong pAndJoin)
        {
            _centeredView = true;
            joinAddress = pAndJoin;
        }

        public override void Initialize()
        {
            DuckNetwork.ClosePauseMenu();
            if (Network.isActive)
            {
                _startCalled = true;
                DuckNetwork.Disconnect();
                Send.Message(new NMDisconnect(DuckNetError.ControlledDisconnect), NetMessagePriority.ReliableOrdered);
                ConnectionStatusUI.Hide();
            }
            base.Initialize();
        }

        public override void Update()
        {
            wait -= Maths.IncFrameTimer();
            if (wait < 0f)
            {
                if (!_didDisconnect)
                {
                    _didDisconnect = true;
                    Network.EndNetworkingSession(new DuckNetErrorInfo(DuckNetError.ControlledDisconnect, "Disconnecting from game."));
                    ConnectionStatusUI.Hide();
                }
                if (_disconnected)
                {
                    if (joinAddress != 0UL) current = new JoinServer(joinAddress);
                    else Graphics.fade = Lerp.Float(Graphics.fade, 0f, 0.05f);
                    if (Graphics.fade <= 0f) current = new TitleScreen();
                }
            }
            base.Update();
        }

        public override void OnSessionEnded(DuckNetErrorInfo error) => _disconnected = true;

        public override void Draw()
        {
            if(MonoMain.UpdateLerpState)
                _dots += 0.01f;
            if (_dots > 1f) _dots = 0f;
            string str = "";
            for (int index = 0; index < 3; ++index)
            {
                if (_dots * 4f > index + 1) str += ".";
            }
            string text = "Disconnecting";
            Graphics.DrawString(text + str, new Vec2((float)(Layer.HUD.width / 2f - Graphics.GetStringWidth(text) / 2f), (float)(Layer.HUD.height / 2f - 4f)), Color.White);
        }
    }
}
