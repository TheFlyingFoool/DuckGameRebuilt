// Decompiled with JetBrains decompiler
// Type: DuckGame.UIGameConnectionBox
//removed for regex reasons Culture=neutral, PublicKeyToken=null
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
            playMusic = false;
            _connectLobby = connect;
            _continueSearchOnFail = false;
            _caption = "JOINING";
        }

        public void SetPasswordAttempt(string pPassword) => _passwordAttempt = pPassword;

        public override void Open()
        {
            base.Open();
            _tryConnectLobby = _connectLobby.lobby;
            if (_connectLobby.lobby == null)
                DuckNetwork.Join("", _connectLobby.lanAddress, _passwordAttempt);
            else
                DuckNetwork.Join(_tryConnectLobby.id.ToString(), "localhost", _passwordAttempt);
            ChangeState(MatchmakingState.Connecting);
            _newStatusList.Add("|DGGREEN|Connecting to game...");
        }

        protected override void UpdateAdditionalMatchmakingLogic()
        {
        }
    }
}
