// Decompiled with JetBrains decompiler
// Type: DuckGame.JoinServer
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

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

        public JoinServer(ulong lobbyAddress)
        {
            this._lobbyID = lobbyAddress;
            this._centeredView = true;
            this._teamSelect = Level.current is TeamSelect2;
        }

        public JoinServer(ulong lobbyAddress, string pPassword)
          : this(lobbyAddress)
        {
            this.password = pPassword;
        }

        public override void Initialize()
        {
            if (Network.isActive)
            {
                Level.current = (Level)new DisconnectFromGame(this._lobbyID);
            }
            else
            {
                DuckNetwork.ClosePauseMenu();
                ConnectionStatusUI.Hide();
                this.SkipStart();
                DevConsole.Log(DCSection.NetCore, "!~----------------JoinServer Level Begins----------------~!");
                if (Profiles.active.Count == 0 || !this._teamSelect)
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
                if (this._lobbyID == 0UL || NetworkDebugger.enabled)
                {
                    DuckNetwork.joinPort = (int)this._lobbyID;
                    DuckNetwork.Join("joinTest");
                }
                else
                    DuckNetwork.Join(this._lobbyID.ToString(), "localhost", this.password);
                //this._attemptedConnection = true;
                this._startedJoining = true;
                this._timeout = 0UL;
                base.Initialize();
            }
        }

        public override void Update()
        {
            if (this._timeout++ > 1200UL)
            {
                Network.DisconnectClient(DuckNetwork.localConnection, new DuckNetErrorInfo(DuckNetError.ConnectionTimeout, "Connection timeout!"));
                Level.current = (Level)new ConnectionError("|RED|CONNECTION FAILED!");
                this._timeout = 0UL;
            }
            base.Update();
        }

        public override void OnSessionEnded(DuckNetErrorInfo error)
        {
            if (!this._startedJoining)
                return;
            if (error != null)
                Level.current = (Level)new ConnectionError(error.message);
            else
                Level.current = (Level)new ConnectionError("|RED|CONNECTION FAILED!");
        }

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
            string text = "Connecting";
            Graphics.DrawString(text + str, new Vec2((float)((double)Layer.HUD.width / 2.0 - (double)Graphics.GetStringWidth(text) / 2.0), (float)((double)Layer.HUD.height / 2.0 - 4.0)), Color.White);
        }
    }
}
