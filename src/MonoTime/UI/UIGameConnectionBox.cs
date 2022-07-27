// Decompiled with JetBrains decompiler
// Type: DuckGame.UIGameConnectionBox
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class UIGameConnectionBox : UIMatchmakingBox
    {
        //private UIMenu _openOnClose;
        private UIServerBrowser.LobbyData _connectLobby;
        private string _passwordAttempt;

        public UIGameConnectionBox(
          UIServerBrowser.LobbyData connect,
          UIMenu openOnClose,
          float xpos,
          float ypos,
          float wide = -1f,
          float high = -1f)
          : base(openOnClose, xpos, ypos, wide, high)
        {
            this.playMusic = false;
            this._connectLobby = connect;
            this._continueSearchOnFail = false;
            this._caption = "JOINING";
        }

        public void SetPasswordAttempt(string pPassword) => this._passwordAttempt = pPassword;

        public override void Open()
        {
            base.Open();
            this._tryConnectLobby = this._connectLobby.lobby;
            if (this._connectLobby.lobby == null)
                DuckNetwork.Join("", this._connectLobby.lanAddress, this._passwordAttempt);
            else
                DuckNetwork.Join(this._tryConnectLobby.id.ToString(), "localhost", this._passwordAttempt);
            this.ChangeState(MatchmakingState.Connecting);
            this._newStatusList.Add("|DGGREEN|Connecting to game...");
        }

        protected override void UpdateAdditionalMatchmakingLogic()
        {
        }
    }
}
