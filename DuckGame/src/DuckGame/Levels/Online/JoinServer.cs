namespace DuckGame
{
    public class JoinServer : Level, IConnectionScreen
    {
        //private bool _attemptedConnection;
        private ulong _lobbyID;
        private float _dots;
        private ulong _timeout;
        private bool _teamSelect;
        private string password = "";
        private bool _startedJoining;
        private string _lobbyIP = "";
        public JoinServer(ulong lobbyAddress)
        {
            _lobbyID = lobbyAddress;
            _centeredView = true;
            _teamSelect = current is TeamSelect2;
        }

        public JoinServer(ulong lobbyAddress, string pPassword)
          : this(lobbyAddress)
        {
            password = pPassword;
        }
        public JoinServer(string ip)
          : this(0UL)
        {
            _lobbyIP = ip;
        }
        public override void Initialize()
        {
            if (Network.isActive)
            {
                current = new DisconnectFromGame(_lobbyID);
            }
            else
            {
                DuckNetwork.ClosePauseMenu();
                ConnectionStatusUI.Hide();
                SkipStart();
                DevConsole.Log(DCSection.NetCore, "!~----------------JoinServer Level Begins----------------~!");
                if (Profiles.active.Count == 0 || !_teamSelect)
                {
                    foreach (Profile prof in Profiles.active)
                        prof.team.Leave(prof);
                    if (Profiles.experienceProfile != null)
                    {
                        Profiles.experienceProfile.team = Teams.Player1;
                        Profiles.experienceProfile.persona = Persona.Duck1;
                    }
                    else
                    {
                        Profiles.DefaultPlayer1.team = Teams.Player1;
                        Profiles.DefaultPlayer1.persona = Persona.Duck1;
                    }
                }
                TeamSelect2.FillMatchmakingProfiles();
                if (_lobbyIP != "")
                {
                    DuckNetwork.Join(_lobbyIP, _lobbyIP, password);
                }
                else if (_lobbyID == 0UL || NetworkDebugger.enabled)
                {
                    DuckNetwork.joinPort = (int)_lobbyID;
                    DuckNetwork.Join("joinTest");
                }
                else
                {
                    DuckNetwork.Join(_lobbyID.ToString(), "localhost", password);
                }
                //this._attemptedConnection = true;
                _startedJoining = true;
                _timeout = 0UL;
                base.Initialize();
            }
        }

        public override void Update()
        {
            if (_timeout++ > 1200UL)
            {
                Network.DisconnectClient(DuckNetwork.localConnection, new DuckNetErrorInfo(DuckNetError.ConnectionTimeout, "Connection timeout!"));
                current = new ConnectionError("|RED|CONNECTION FAILED!");
                _timeout = 0UL;
            }
            base.Update();
        }

        public override void OnSessionEnded(DuckNetErrorInfo error)
        {
            if (!_startedJoining)
                return;
            if (error != null)
                current = new ConnectionError(error.message);
            else
                current = new ConnectionError("|RED|CONNECTION FAILED!");
        }

        public override void Draw()
        {
            _dots += 0.01f;
            if (_dots > 1f) _dots = 0f;
            string str = "";
            for (int index = 0; index < 3; ++index)
            {
                if (_dots * 4f > index + 1) str += ".";
            }
            string text = "Connecting";
            Graphics.DrawString(text + str, new Vec2((float)(Layer.HUD.width / 2f - Graphics.GetStringWidth(text) / 2f), (float)(Layer.HUD.height / 2f - 4f)), Color.White);
        }
    }
}
