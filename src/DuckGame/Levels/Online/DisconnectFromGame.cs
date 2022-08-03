// Decompiled with JetBrains decompiler
// Type: DuckGame.DisconnectFromGame
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

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
            if (wait < 0.0)
            {
                if (!_didDisconnect)
                {
                    _didDisconnect = true;
                    Network.EndNetworkingSession(new DuckNetErrorInfo(DuckNetError.ControlledDisconnect, "Disconnecting from game."));
                    ConnectionStatusUI.Hide();
                }
                if (_disconnected)
                {
                    if (joinAddress != 0UL)
                        Level.current = new JoinServer(joinAddress);
                    else
                        Graphics.fade = Lerp.Float(Graphics.fade, 0f, 0.05f);
                    if (Graphics.fade <= 0.0)
                        Level.current = new TitleScreen();
                }
            }
            base.Update();
        }

        public override void OnSessionEnded(DuckNetErrorInfo error) => _disconnected = true;

        public override void Draw()
        {
            _dots += 0.01f;
            if (_dots > 1.0)
                _dots = 0f;
            string str = "";
            for (int index = 0; index < 3; ++index)
            {
                if (_dots * 4.0 > index + 1)
                    str += ".";
            }
            string text = "Disconnecting";
            Graphics.DrawString(text + str, new Vec2((float)(Layer.HUD.width / 2.0 - Graphics.GetStringWidth(text) / 2.0), (float)(Layer.HUD.height / 2.0 - 4.0)), Color.White);
        }
    }
}
