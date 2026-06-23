using System;
using System.Collections.Generic;
using System.Linq;

namespace DuckGame
{
    public class NCSteam : NCNetworkImplementation
    {
        private Lobby.UserStatusChangeDelegate _userChange;
        private Lobby.ChatMessageDelegate _chatDelegate;
        private Steam.ConnectionRequestedDelegate _connectionRequest;
        private Steam.ConnectionFailedDelegate _connectionFailed;
        private Steam.InviteReceivedDelegate _inviteReceived;
        private Steam.LobbySearchCompleteDelegate _lobbySearchComplete;
        private Steam.RequestCurrentStatsDelegate _requestStatsComplete;
        private string _serverIdentifier = "";
        //private int _port;
        //private ulong _connectionPacketIdentifier = 6094567099491692639;
        private bool _initializedSettings;
        private bool _isDGRLocked;
        private bool _lobbyCreationComplete;
        public static bool NoDGRBroadcast;
        public static ulong inviteLobbyID;
        private bool gotPingString;
        private int pingWaitTimeout;
        public static bool globalSearch;

        public NCSteam(Network c, int networkIndex)
          : base(c, networkIndex)
        {
            HookUpDelegates();
        }

        public override NCError OnSendPacket(byte[] data, int length, object connection)
        {
            if (length < 1200)
                Steam.SendPacket(connection as User, data, (uint)length, P2PDataSendType.Unreliable);
            else
                Steam.SendPacket(connection as User, data, (uint)length, P2PDataSendType.Reliable);
            return null;
        }

        public void HookUpDelegates()
        {
            if (_connectionRequest != null)
                return;
            _connectionRequest = new Steam.ConnectionRequestedDelegate(OnConnectionRequest);
            _connectionFailed = new Steam.ConnectionFailedDelegate(OnConnectionFailed);
            _inviteReceived = new Steam.InviteReceivedDelegate(OnInviteReceived);
            _lobbySearchComplete = new Steam.LobbySearchCompleteDelegate(OnLobbySearchComplete);
            _requestStatsComplete = new Steam.RequestCurrentStatsDelegate(OnRequestStatsComplete);
            Steam.ConnectionRequested += _connectionRequest;
            Steam.ConnectionFailed += _connectionFailed;
            Steam.InviteReceived += _inviteReceived;
            Steam.LobbySearchComplete += _lobbySearchComplete;
            Steam.RequestCurrentStatsComplete += _requestStatsComplete;
        }

        public void UnhookDelegates()
        {
            if (_connectionRequest == null)
                return;
            Steam.ConnectionRequested -= _connectionRequest;
            Steam.ConnectionFailed -= _connectionFailed;
            Steam.InviteReceived -= _inviteReceived;
            Steam.LobbySearchComplete -= _lobbySearchComplete;
            _connectionRequest = null;
        }

        public override NCError OnHostServer(
          string identifier,
          int port,
          NetworkLobbyType lobbyType,
          int maxConnections)
        {
            gotPingString = false;
            pingWaitTimeout = 0;
            if (_lobby != null)
            {
                Steam.LeaveLobby(_lobby);
                UnhookLobbyUserStatusChange(_lobby, new Lobby.UserStatusChangeDelegate(OnUserStatusChange));
                UnhookLobbyChatMessage(_lobby, new Lobby.ChatMessageDelegate(OnChatMessage));
                DevConsole.Log(DCSection.Steam, "|DGYELLOW|Leaving lobby to host new lobby.");
            }
            _lobby = null;
            HookUpDelegates();
            _initializedSettings = false;
            _lobby = Steam.CreateLobby((SteamLobbyType)lobbyType, maxConnections);
            _lobby.name = identifier;
            if (_lobby == null)
                return new NCError("|DGORANGE|STEAM |DGRED|Steam is not running.", NCErrorType.Error);
            _userChange = new Lobby.UserStatusChangeDelegate(OnUserStatusChange);
            HookUpLobbyUserStatusChange(_lobby, _userChange);
            _chatDelegate = new Lobby.ChatMessageDelegate(OnChatMessage);
            HookUpLobbyChatMessage(_lobby, _chatDelegate);
            _serverIdentifier = identifier;
            //this._port = port;
            StartServerThread();
            return new NCError("|DGORANGE|STEAM |DGYELLOW|Attempting to create server lobby...", NCErrorType.Message);
        }

        private void HookUpLobbyUserStatusChange(Lobby l, Lobby.UserStatusChangeDelegate del) => l.UserStatusChange += del;

        private void HookUpLobbyChatMessage(Lobby l, Lobby.ChatMessageDelegate del) => l.ChatMessage += del;

        private void UnhookLobbyUserStatusChange(Lobby l, Lobby.UserStatusChangeDelegate del)
        {
            try
            {
                l.UserStatusChange -= del;
            }
            catch (Exception)
            {
            }
        }

        private void UnhookLobbyChatMessage(Lobby l, Lobby.ChatMessageDelegate del)
        {
            try
            {
                l.ChatMessage -= del;
            }
            catch (Exception)
            {
            }
        }

        public override NCError OnJoinServer(string identifier, int port, string ip)
        {
            gotPingString = false;
            pingWaitTimeout = 0;
            if (_lobby != null)
            {
                Steam.LeaveLobby(_lobby);
                UnhookLobbyUserStatusChange(_lobby, new Lobby.UserStatusChangeDelegate(OnUserStatusChange));
                UnhookLobbyChatMessage(_lobby, new Lobby.ChatMessageDelegate(OnChatMessage));
                DevConsole.Log(DCSection.Steam, "|DGYELLOW|Leaving lobby to join new lobby.");
            }
            _lobby = null;
            HookUpDelegates();
            _serverIdentifier = identifier;
            if (identifier == "joinTest")
            {
                _lobby = Steam.JoinLobby(1UL);
                _serverIdentifier = _lobby.id.ToString();
            }
            else
                _lobby = Steam.JoinLobby(Convert.ToUInt64(identifier));
            if (_lobby == null)
                return new NCError("Steam is not running.", NCErrorType.Error);
            _userChange = new Lobby.UserStatusChangeDelegate(OnUserStatusChange);
            HookUpLobbyUserStatusChange(_lobby, _userChange);
            _chatDelegate = new Lobby.ChatMessageDelegate(OnChatMessage);
            HookUpLobbyChatMessage(_lobby, _chatDelegate);
            //this._port = port;
            StartClientThread();
            return new NCError("|DGORANGE|STEAM |DGGREEN|Connecting to lobbyID " + identifier + ".", NCErrorType.Message);
        }

        public void OnUserStatusChange(User who, SteamLobbyUserStatusFlags flags, User responsible)
        {
            DevConsole.Log(DCSection.Connection, "NCSteam.LobbyStatusChange(" + GetDrawString(who) + ", " + flags.ToString() + ")");
            if ((flags & SteamLobbyUserStatusFlags.Entered) != 0)
            {
                DevConsole.Log(DCSection.Steam, "|DGGREEN|" + who.name + " (" + who.id.ToString() + ") has joined the Steam lobby.");
                if (Network.isServer && DuckNetwork.localConnection.status == ConnectionStatus.Connected)
                    AttemptConnection(who);
            }
            else if ((flags & SteamLobbyUserStatusFlags.Left) != 0)
                DevConsole.Log(DCSection.Steam, "|DGRED|" + GetDrawString(who) + " has left the Steam lobby.");
            else if ((flags & SteamLobbyUserStatusFlags.Disconnected) != 0)
                DevConsole.Log(DCSection.Steam, "|DGRED|" + GetDrawString(who) + " has disconnected from the Steam lobby.");
            if ((flags & SteamLobbyUserStatusFlags.Kicked) == 0)
                return;
            DevConsole.Log(DCSection.Steam, "|DGYELLOW|" + GetDrawString(responsible) + " kicked " + GetDrawString(who) + ".");
        }

        public void OnChatMessage(User who, byte[] data)
        {
            Steam_LobbyMessage steamLobbyMessage = Steam_LobbyMessage.Receive(who, data);
            if (steamLobbyMessage == null)
                return;
            if (steamLobbyMessage.message == "COM_FAIL" && steamLobbyMessage.context == Steam.user)
            {
                DevConsole.Log(DCSection.Connection, "Communication failure with " + who.name + "... Disconnecting!");
                Network.EndNetworkingSession(new DuckNetErrorInfo(DuckNetError.EveryoneDisconnected, "Could not connect to server."));
            }
            else
            {
                if (!(steamLobbyMessage.message == "IM_OUTTAHERE"))
                    return;
                DevConsole.Log(DCSection.Connection, "Received lobby exit message from " + who.name + "...");
                Network.DisconnectClient(GetConnection(who), new DuckNetErrorInfo(DuckNetError.ClientDisconnected, who.name + " left the lobby."));
            }
        }

        private string GetDrawString(User pUser) => pUser.name + " (" + pUser.id.ToString() + ")";

        public void OnConnectionRequest(User who)
        {
            DevConsole.Log(DCSection.Connection, "NCSteam.OnConnectionRequest(" + GetDrawString(who) + ")");
            if ((GetConnection(who) != null || lobby != null && lobby.users.Contains(who)) && Network.isActive)
            {
                DevConsole.Log(DCSection.Steam, "|DGYELLOW|" + GetDrawString(who) + " has requested a connection.");
                Steam.AcceptConnection(who);
            }
            else if (!Network.isActive)
                DevConsole.Log(DCSection.Steam, "|DGRED| Connection request ignored(" + GetDrawString(who) + ")(Network.isActive == false)");
            else
                DevConsole.Log(DCSection.Steam, "|DGRED| Connection request ignored(" + GetDrawString(who) + ")(User not found)");
        }

        public void OnConnectionFailed(User who, byte pError) => DevConsole.Log(DCSection.Steam, "|DGRED|Connection with " + GetDrawString(who) + " has failed (" + pError.ToString() + ")!");

        public static void PrepareProfilesForJoin()
        {
            foreach (Team team in Teams.all)
                team.ClearProfiles();
            Profile.defaultProfileMappings[0] = Profiles.experienceProfile;
            Teams.Player1.Join(Profiles.experienceProfile);
            TeamSelect2.ControllerLayoutsChanged();
        }

        public void OnInviteReceived(User who, Lobby lobby)
        {
            inviteLobbyID = lobby.id;
            switch (Level.current)
            {
                case TitleScreen _:
                case Editor _:
                case DuckGameTestArea _:
                label_2:
                    PrepareProfilesForJoin();
                    break;
                case GameLevel _:
                    if (!(Level.current as GameLevel)._editorTestMode)
                        break;
                    goto label_2;
            }
            Level.current = new JoinServer(lobby.id);
        }

        public void OnLobbySearchComplete(Lobby lobby)
        {
        }

        public void OnRequestStatsComplete()
        {
        }

        protected override object GetConnectionObject(string identifier) => User.GetUser(Convert.ToUInt64(identifier));

        public override string GetConnectionIdentifier(object connection) => connection is User user ? user.id.ToString() : "no info";

        public override string GetConnectionName(object connection) => connection is User user ? user.name : "no info";

        protected override string OnGetLocalName() => Steam.user != null ? Steam.user.name : "no info";

        protected override NCError OnSpinServerThread()
        {
            if (_lobby == null)
                return NetworkDebugger.enabled ? null : new NCError("|DGORANGE|STEAM |DGRED|Lobby was closed.", NCErrorType.CriticalError);
            if (_lobby.processing)
                return null;
            return _lobby.id == 0UL ? new NCError("|DGORANGE|STEAM |DGRED|Failed to create lobby.", NCErrorType.CriticalError) : RunSharedLogic();
        }

        protected override NCError OnSpinClientThread()
        {
            if (_lobby == null)
                return new NCError("|DGORANGE|STEAM |DGYELLOW|Lobby was closed.", NCErrorType.CriticalError);
            if (_lobby.processing)
                return null;
            return _lobby.id == 0UL ? new NCError("|DGORANGE|STEAM |DGRED|Failed to join lobby.", NCErrorType.CriticalError) : RunSharedLogic();
        }

        protected NCError RunSharedLogic()
        {
            while (true)
            {
                SteamPacket steamPacket = Steam.ReadPacket();
                if (steamPacket != null)
                    OnPacket(steamPacket.data, steamPacket.connection);
                else
                    break;
            }
            return null;
        }

        protected override void Disconnect(NetworkConnection c)
        {
            if (c != null && c.data is User data)
            {
                DevConsole.Log(DCSection.Steam, "|DGRED|Closing connection with " + GetDrawString(data) + ".");
                Steam.CloseConnection(data);
            }
            base.Disconnect(c);
        }

        protected override void KillConnection()
        {
            if (_lobby != null)
            {
                if (_lobby.owner == Steam.user && DuckNetwork.potentialHostObject is User potentialHostObject && _lobby.users.Contains(potentialHostObject))
                    _lobby.owner = potentialHostObject;
                Steam_LobbyMessage.Send("IM_OUTTAHERE", null);
                Steam.LeaveLobby(_lobby);
                UnhookLobbyUserStatusChange(_lobby, new Lobby.UserStatusChangeDelegate(OnUserStatusChange));
                UnhookLobbyChatMessage(_lobby, new Lobby.ChatMessageDelegate(OnChatMessage));
                DevConsole.Log(DCSection.Steam, "|DGYELLOW|Leaving lobby to host new lobby.");
            }
            _lobby = null;
            _lobbyCreationComplete = false;
            _initializedSettings = false;
            base.KillConnection();
        }

        public override void ApplyLobbyData()
        {
            foreach (MatchSetting matchSetting in TeamSelect2.matchSettings)
            {
                if (matchSetting.value is int)
                    _lobby.SetLobbyData(matchSetting.id, ((int)matchSetting.value).ToString());
                else if (matchSetting.value is bool)
                    _lobby.SetLobbyData(matchSetting.id, ((bool)matchSetting.value ? 1 : 0).ToString());
            }
            foreach (MatchSetting onlineSetting in TeamSelect2.onlineSettings)
            {
                if (onlineSetting.id == "password")
                    _lobby.SetLobbyData("password", (string)onlineSetting.value != "" ? "true" : "false");
                if (onlineSetting.id == "modifiers")
                {
                    if (onlineSetting.filtered)
                        _lobby.SetLobbyData(onlineSetting.id, (bool)onlineSetting.value ? "true" : "false");
                }
                else if (onlineSetting.id == "dedicated")
                    _lobby.SetLobbyData(onlineSetting.id, (bool)onlineSetting.value ? "true" : "false");
                else if (onlineSetting.value is int)
                    _lobby.SetLobbyData(onlineSetting.id, ((int)onlineSetting.value).ToString());
                else if (onlineSetting.value is bool)
                    _lobby.SetLobbyData(onlineSetting.id, ((bool)onlineSetting.value ? 1 : 0).ToString());
            }
            foreach (UnlockData allUnlock in Unlocks.allUnlocks)
                _lobby.SetLobbyData(allUnlock.id, (allUnlock.enabled ? 1 : 0).ToString());
            _lobby.SetLobbyData("customLevels", Editor.customLevelCount.ToString());
        }

        private void TryGettingPingString()
        {
            if (_lobby != null && !_lobby.processing && _lobby.id != 0UL && pingWaitTimeout <= 0 && !gotPingString)
            {
                string localPingString = Steam.GetLocalPingString();
                _lobby.SetLobbyData("pingstring", localPingString);
                if (localPingString != null && localPingString != "") gotPingString = true;
                pingWaitTimeout = 60;
            }
            --pingWaitTimeout;
        }

        public override void Update()
        {
            if (_lobby != null && !_lobby.processing && _lobby.id != 0UL)
            {
                if (!_lobbyCreationComplete)
                {
                    _lobbyCreationComplete = true;
                    if (Network.isServer)
                    {
                        if (_lobby.joinResult == SteamLobbyJoinResult.Success)
                        {
                            DevConsole.Log(DCSection.Steam, "|DGGREEN|Lobby created.");
                        }
                        else
                        {
                            DevConsole.Log(DCSection.Steam, "|DGGREEN|Lobby creation failed!");
                            Network.EndNetworkingSession(new DuckNetErrorInfo(DuckNetError.ControlledDisconnect, "Failed to create steam lobby."));
                            return;
                        }
                    }
                    else
                    {
                        if (_lobby.owner != null && Options.Data.blockedPlayers.Contains(_lobby.owner.id))
                        {
                            DuckNetwork.FailWithBlockedUser();
                            DevConsole.Log(DCSection.Steam, "|DGRED|You have blocked the host! (" + _lobby.owner.name + ")");
                            return;
                        }
                        if (UIMatchmakerMark2.instance != null)
                            UIMatchmakerMark2.instance.Hook_OnLobbyProcessed(_lobby);
                        if (_lobby.joinResult == SteamLobbyJoinResult.Success)
                        {
                            string lobbyData1 = _lobby.GetLobbyData("version");
                            NMVersionMismatch.Type type = DuckNetwork.CheckVersion(lobbyData1);
                            if (type != NMVersionMismatch.Type.Match)
                            {
                                DuckNetwork.FailWithVersionMismatch(lobbyData1, type);
                                DevConsole.Log(DCSection.Steam, "|DGRED|Lobby version mismatch! (" + type.ToString() + ")");
                                return;
                            }
                            if (_lobby.GetLobbyData("modhash").Trim() != ModLoader.modHash)
                            {
                                ConnectionError.joinLobby = Steam.lobby;
                                DuckNetwork.FailWithDifferentModsError();
                                return;
                            }
                            string str1 = _lobby.GetLobbyData("datahash").Trim();
                            if (str1 != Network.gameDataHash.ToString())
                            {
                                DuckNetwork.FailWithDatahashMismatch();
                                DevConsole.Log(DCSection.Steam, "|DGRED|Lobby datahash mismatch! (" + Network.gameDataHash.ToString() + " vs. " + str1 + ")");
                                return;
                            }
                            string lobbyData2 = _lobby.GetLobbyData("mods");
                            if (lobbyData2 != null && lobbyData2 != "")
                            {
                                lobbyData2 = lobbyData2.Replace("|3132351890,0", ""); //dumb but works -Lucky
                                lobbyData2 = lobbyData2.Replace("3132351890,0", "");
                                string str2 = lobbyData2;
                                char[] chArray = new char[1] { '|' };
                                foreach (string str3 in str2.Split(chArray))
                                {
                                    try
                                    {
                                        if (!(str3 == ""))
                                        {
                                            if (!(str3 == "LOCAL"))
                                            {
                                                string[] strArray = str3.Split(',');
                                                if (strArray.Length == 2)
                                                {
                                                    long uint64 = (long)Convert.ToUInt64(strArray[0].Trim());
                                                    uint uint32 = Convert.ToUInt32(strArray[1].Trim());
                                                    Mod modFromWorkshopId = ModLoader.GetModFromWorkshopID((ulong)uint64);
                                                    if (modFromWorkshopId != null)
                                                    {
                                                        if ((int)modFromWorkshopId.dataHash != (int)uint32)
                                                            DuckNetwork.FailWithModDatahashMismatch(modFromWorkshopId);
                                                    }
                                                    else
                                                        DevConsole.Log("|DGRED|Non-existing Mod found in Lobby mod list, this should never happen!");
                                                }
                                            }
                                        }
                                    }
                                    catch (Exception)
                                    {
                                    }
                                }
                            }
                            DevConsole.Log(DCSection.Steam, "|DGGREEN|----------------------------------------");
                            DevConsole.Log(DCSection.Steam, "|DGGREEN|Lobby Joined (" + _lobby.owner.name + ")");
                            AttemptConnection(_lobby.owner, true);
                        }
                        else
                        {
                            SteamLobbyJoinResult joinResult = _lobby.joinResult;
                            DevConsole.Log(DCSection.Steam, "|DGGREEN|Failed to join lobby (" + joinResult.ToString() + ")");
                            string msg;
                            if (_lobby.joinResult == SteamLobbyJoinResult.DoesntExist)
                                msg = "Steam Lobby No Longer Exists.";
                            else if (_lobby.joinResult == SteamLobbyJoinResult.NotAllowed)
                            {
                                msg = "Failed to Join Lobby (Access Denied)";
                            }
                            else
                            {
                                joinResult = _lobby.joinResult;
                                msg = "Failed to Join Lobby (" + joinResult.ToString() + ")";
                            }
                            Network.EndNetworkingSession(new DuckNetErrorInfo(DuckNetError.ControlledDisconnect, msg));
                            return;
                        }
                    }
                }
                if (Network.isServer)
                {
                    if (!_initializedSettings && _lobby.id != 0UL)
                    {
                        UpdateRandomID(_lobby);
                        _lobby.SetLobbyData("started", "false");
                        _lobby.SetLobbyData("version", DG.version);
                        _lobby.SetLobbyData("beta", "2.0");
                        _lobby.SetLobbyData("dev", DG.devBuild ? "true" : "false");
                        _lobby.SetLobbyData("modifiers", "false");
                        _lobby.SetLobbyData("modhash", ModLoader.modHash);
                        _lobby.SetLobbyData("datahash", Network.gameDataHash.ToString());
                        _lobby.SetLobbyData("name", Steam.user.name + "'s Lobby");
                        _lobby.SetLobbyData("numSlots", DuckNetwork.numSlots.ToString());
                        if (!NoDGRBroadcast)
                        {
                            _lobby.SetLobbyData("DGR", "true");
                            _lobby.SetLobbyData("DGRVersion", Program.CURRENT_VERSION_ID);
                        }
                        _lobby.name = _serverIdentifier;
                        if (_lobby.name != TeamSelect2.DefaultGameName())
                            _lobby.SetLobbyData("customName", "true");
                        string modList = "";
                        bool first = true;
                        foreach (Mod m in ModLoader.accessibleMods)
                        {
                            if (m is not CoreMod && m is not DisabledMod && m.configuration != null && !m.configuration.disabled)
                            {
                                if (!first)
                                    modList += "|";
                                modList += m.configuration.isWorkshop ? m.configuration.workshopID.ToString() + "," + m.dataHash : "LOCAL";
                                first = false;
                            }
                        }
                        if (DGRSettings.DGROnly)
                        {
                            modList += "3132351890,0";
                        }
                        _lobby.SetLobbyModsData(modList);
                        ApplyLobbyData();
                        _initializedSettings = true;
                    }
                    if (((!_isDGRLocked && (DGRSettings.DGROnly)) || (_isDGRLocked && !(DGRSettings.DGROnly))) && _lobby.id != 0UL)
                    {
                        string modList = "";
                        bool first = true;
                        foreach (Mod m in ModLoader.accessibleMods)
                        {
                            if (m is not CoreMod && m is not DisabledMod && m.configuration != null && !m.configuration.disabled)
                            {
                                if (!first)
                                    modList += "|";
                                modList += m.configuration.isWorkshop ? m.configuration.workshopID.ToString() + "," + m.dataHash : "LOCAL";
                                first = false;
                            }
                        }
                        _isDGRLocked = false;
                        if (DGRSettings.DGROnly)
                        {
                            //https://steamcommunity.com/sharedfiles/filedetails/?id=3132351890
                            if (!first) modList += "|";
                            modList += "3132351890,0";
                            _isDGRLocked = true;
                        }
                        _lobby.SetLobbyModsData(modList);
                    }
                    if (!gotPingString)
                        TryGettingPingString();
                }
                if (_lobby.owner == Steam.user && !Network.isServer)
                {
                    foreach (NetworkConnection connection in connections)
                    {
                        if (connection.data is User && connection.isHost && _lobby.users.Contains(connection.data as User))
                            _lobby.owner = connection.data as User;
                    }
                }
            }
            base.Update();
        }

        public override void Terminate()
        {
            _initializedSettings = false;
            UnhookDelegates();
            base.Terminate();
        }

        public override void AddLobbyStringFilter(string key, string value, LobbyFilterComparison op) => Steam.AddLobbyStringFilter(key, value, (SteamLobbyComparison)op);

        public override void AddLobbyNumericalFilter(string key, int value, LobbyFilterComparison op) => Steam.AddLobbyNumericalFilter(key, value, (SteamLobbyComparison)op);

        public override void ApplyTS2LobbyFilters()
        {
            foreach (MatchSetting matchSetting in TeamSelect2.matchSettings)
            {
                if (matchSetting.value is int)
                {
                    if (matchSetting.filtered)
                        Steam.AddLobbyNumericalFilter(matchSetting.id, (int)matchSetting.value, (SteamLobbyComparison)matchSetting.filterMode);
                    else if (!matchSetting.filtered)
                        Steam.AddLobbyNearFilter(matchSetting.id, (int)matchSetting.defaultValue);
                }
                if (matchSetting.value is bool)
                {
                    if (matchSetting.filtered)
                        Steam.AddLobbyNumericalFilter(matchSetting.id, (bool)matchSetting.value ? 1 : 0, (SteamLobbyComparison)matchSetting.filterMode);
                    else if (!matchSetting.filtered)
                        Steam.AddLobbyNearFilter(matchSetting.id, (bool)matchSetting.defaultValue ? 1 : 0);
                }
            }
            foreach (MatchSetting onlineSetting in TeamSelect2.onlineSettings)
            {
                if (onlineSetting.value is int)
                {
                    if (onlineSetting.filtered)
                        Steam.AddLobbyNumericalFilter(onlineSetting.id, (int)onlineSetting.value, (SteamLobbyComparison)onlineSetting.filterMode);
                    else if (!onlineSetting.filtered)
                        Steam.AddLobbyNearFilter(onlineSetting.id, (int)onlineSetting.defaultValue);
                }
                if (onlineSetting.value is bool)
                {
                    if (onlineSetting.id == "modifiers")
                    {
                        if (onlineSetting.filtered)
                            Steam.AddLobbyStringFilter(onlineSetting.id, (bool)onlineSetting.value ? "true" : "false", SteamLobbyComparison.Equal);
                    }
                    else if (onlineSetting.id == "customlevelsenabled")
                    {
                        if (onlineSetting.filtered)
                        {
                            if ((bool)onlineSetting.value)
                                Steam.AddLobbyNumericalFilter(onlineSetting.id, 0, SteamLobbyComparison.GreaterThan);
                            else
                                Steam.AddLobbyNumericalFilter(onlineSetting.id, 0, SteamLobbyComparison.Equal);
                        }
                    }
                    else if (onlineSetting.filtered)
                        Steam.AddLobbyNumericalFilter(onlineSetting.id, (bool)onlineSetting.value ? 1 : 0, (SteamLobbyComparison)onlineSetting.filterMode);
                    else if (!onlineSetting.filtered)
                        Steam.AddLobbyNearFilter(onlineSetting.id, (bool)onlineSetting.defaultValue ? 1 : 0);
                }
            }
        }

        public override void SearchForLobby()
        {
            if (globalSearch)
                Steam.SearchForLobbyWorldwide();
            else
                Steam.SearchForLobby(null);
            globalSearch = false;
        }

        public override void RequestGlobalStats() => Steam.RequestGlobalStats();

        public override bool IsLobbySearchComplete() => Steam.lobbySearchComplete;

        public override int NumLobbiesFound() => Steam.lobbiesFound;

        public override bool TryRequestDailyKills(out long kills)
        {
            kills = 0L;
            if (!Steam.waitingForGlobalStats)
                kills = (long)Steam.GetDailyGlobalStat(nameof(kills));
            Steam.RequestGlobalStats();
            return true;
        }

        public override Lobby GetSearchLobbyAtIndex(int i) => Steam.GetSearchLobbyAtIndex(i);
    }
}
