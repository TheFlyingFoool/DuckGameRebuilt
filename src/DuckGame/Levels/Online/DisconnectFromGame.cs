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

        public DisconnectFromGame() => this._centeredView = true;

        public DisconnectFromGame(ulong pAndJoin)
        {
            this._centeredView = true;
            this.joinAddress = pAndJoin;
        }

        public override void Initialize()
        {
            DuckNetwork.ClosePauseMenu();
            if (Network.isActive)
            {
                this._startCalled = true;
                DuckNetwork.Disconnect();
                Send.Message((NetMessage)new NMDisconnect(DuckNetError.ControlledDisconnect), NetMessagePriority.ReliableOrdered);
                ConnectionStatusUI.Hide();
            }
            base.Initialize();
        }

        public override void Update()
        {
            this.wait -= Maths.IncFrameTimer();
            if ((double)this.wait < 0.0)
            {
                if (!this._didDisconnect)
                {
                    this._didDisconnect = true;
                    Network.EndNetworkingSession(new DuckNetErrorInfo(DuckNetError.ControlledDisconnect, "Disconnecting from game."));
                    ConnectionStatusUI.Hide();
                }
                if (this._disconnected)
                {
                    if (this.joinAddress != 0UL)
                        Level.current = (Level)new JoinServer(this.joinAddress);
                    else
                        Graphics.fade = Lerp.Float(Graphics.fade, 0.0f, 0.05f);
                    if ((double)Graphics.fade <= 0.0)
                        Level.current = (Level)new TitleScreen();
                }
            }
            base.Update();
        }

        public override void OnSessionEnded(DuckNetErrorInfo error) => this._disconnected = true;

        public override void Draw()
        {
            this._dots += 0.01f;
            if ((double)this._dots > 1.0)
                this._dots = 0.0f;
            string str = "";
            for (int index = 0; index < 3; ++index)
            {
                if ((double)this._dots * 4.0 > (double)(index + 1))
                    str += ".";
            }
            string text = "Disconnecting";
            Graphics.DrawString(text + str, new Vec2((float)((double)Layer.HUD.width / 2.0 - (double)Graphics.GetStringWidth(text) / 2.0), (float)((double)Layer.HUD.height / 2.0 - 4.0)), Color.White);
        }
    }
}
