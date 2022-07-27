// Decompiled with JetBrains decompiler
// Type: DuckGame.NCSteam
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections.Generic;

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
        private bool _lobbyCreationComplete;
        public static ulong inviteLobbyID;
        private bool gotPingString;
        private int pingWaitTimeout;
        public static bool globalSearch;

        public NCSteam(Network c, int networkIndex)
          : base(c, networkIndex)
        {
            this.HookUpDelegates();
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
            if (this._connectionRequest != null)
                return;
            this._connectionRequest = new Steam.ConnectionRequestedDelegate(this.OnConnectionRequest);
            this._connectionFailed = new Steam.ConnectionFailedDelegate(this.OnConnectionFailed);
            this._inviteReceived = new Steam.InviteReceivedDelegate(this.OnInviteReceived);
            this._lobbySearchComplete = new Steam.LobbySearchCompleteDelegate(this.OnLobbySearchComplete);
            this._requestStatsComplete = new Steam.RequestCurrentStatsDelegate(this.OnRequestStatsComplete);
            Steam.ConnectionRequested += this._connectionRequest;
            Steam.ConnectionFailed += this._connectionFailed;
            Steam.InviteReceived += this._inviteReceived;
            Steam.LobbySearchComplete += this._lobbySearchComplete;
            Steam.RequestCurrentStatsComplete += this._requestStatsComplete;
        }

        public void UnhookDelegates()
        {
            if (this._connectionRequest == null)
                return;
            Steam.ConnectionRequested -= this._connectionRequest;
            Steam.ConnectionFailed -= this._connectionFailed;
            Steam.InviteReceived -= this._inviteReceived;
            Steam.LobbySearchComplete -= this._lobbySearchComplete;
            this._connectionRequest = null;
        }

        public override NCError OnHostServer(
          string identifier,
          int port,
          NetworkLobbyType lobbyType,
          int maxConnections)
        {
            this.gotPingString = false;
            this.pingWaitTimeout = 0;
            if (this._lobby != null)
            {
                Steam.LeaveLobby(this._lobby);
                this.UnhookLobbyUserStatusChange(this._lobby, new Lobby.UserStatusChangeDelegate(this.OnUserStatusChange));
                this.UnhookLobbyChatMessage(this._lobby, new Lobby.ChatMessageDelegate(this.OnChatMessage));
                DevConsole.Log(DCSection.Steam, "|DGYELLOW|Leaving lobby to host new lobby.");
            }
            this._lobby = null;
            this.HookUpDelegates();
            this._initializedSettings = false;
            this._lobby = Steam.CreateLobby((SteamLobbyType)lobbyType, maxConnections);
            this._lobby.name = identifier;
            if (this._lobby == null)
                return new NCError("|DGORANGE|STEAM |DGRED|Steam is not running.", NCErrorType.Error);
            this._userChange = new Lobby.UserStatusChangeDelegate(this.OnUserStatusChange);
            this.HookUpLobbyUserStatusChange(this._lobby, this._userChange);
            this._chatDelegate = new Lobby.ChatMessageDelegate(this.OnChatMessage);
            this.HookUpLobbyChatMessage(this._lobby, this._chatDelegate);
            this._serverIdentifier = identifier;
            //this._port = port;
            this.StartServerThread();
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
            this.gotPingString = false;
            this.pingWaitTimeout = 0;
            if (this._lobby != null)
            {
                Steam.LeaveLobby(this._lobby);
                this.UnhookLobbyUserStatusChange(this._lobby, new Lobby.UserStatusChangeDelegate(this.OnUserStatusChange));
                this.UnhookLobbyChatMessage(this._lobby, new Lobby.ChatMessageDelegate(this.OnChatMessage));
                DevConsole.Log(DCSection.Steam, "|DGYELLOW|Leaving lobby to join new lobby.");
            }
            this._lobby = null;
            this.HookUpDelegates();
            this._serverIdentifier = identifier;
            if (identifier == "joinTest")
            {
                this._lobby = Steam.JoinLobby(1UL);
                this._serverIdentifier = this._lobby.id.ToString();
            }
            else
                this._lobby = Steam.JoinLobby(Convert.ToUInt64(identifier));
            if (this._lobby == null)
                return new NCError("Steam is not running.", NCErrorType.Error);
            this._userChange = new Lobby.UserStatusChangeDelegate(this.OnUserStatusChange);
            this.HookUpLobbyUserStatusChange(this._lobby, this._userChange);
            this._chatDelegate = new Lobby.ChatMessageDelegate(this.OnChatMessage);
            this.HookUpLobbyChatMessage(this._lobby, this._chatDelegate);
            //this._port = port;
            this.StartClientThread();
            return new NCError("|DGORANGE|STEAM |DGGREEN|Connecting to lobbyID " + identifier + ".", NCErrorType.Message);
        }

        public void OnUserStatusChange(User who, SteamLobbyUserStatusFlags flags, User responsible)
        {
            DevConsole.Log(DCSection.Connection, "NCSteam.LobbyStatusChange(" + this.GetDrawString(who) + ", " + flags.ToString() + ")");
            if ((flags & SteamLobbyUserStatusFlags.Entered) != 0)
            {
                DevConsole.Log(DCSection.Steam, "|DGGREEN|" + who.name + " (" + who.id.ToString() + ") has joined the Steam lobby.");
                if (Network.isServer && DuckNetwork.localConnection.status == ConnectionStatus.Connected)
                    this.AttemptConnection(who);
            }
            else if ((flags & SteamLobbyUserStatusFlags.Left) != 0)
                DevConsole.Log(DCSection.Steam, "|DGRED|" + this.GetDrawString(who) + " has left the Steam lobby.");
            else if ((flags & SteamLobbyUserStatusFlags.Disconnected) != 0)
                DevConsole.Log(DCSection.Steam, "|DGRED|" + this.GetDrawString(who) + " has disconnected from the Steam lobby.");
            if ((flags & SteamLobbyUserStatusFlags.Kicked) == 0)
                return;
            DevConsole.Log(DCSection.Steam, "|DGYELLOW|" + this.GetDrawString(responsible) + " kicked " + this.GetDrawString(who) + ".");
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
                Network.DisconnectClient(this.GetConnection(who), new DuckNetErrorInfo(DuckNetError.ClientDisconnected, who.name + " left the lobby."));
            }
        }

        private string GetDrawString(User pUser) => pUser.name + " (" + pUser.id.ToString() + ")";

        public void OnConnectionRequest(User who)
        {
            DevConsole.Log(DCSection.Connection, "NCSteam.OnConnectionRequest(" + this.GetDrawString(who) + ")");
            if ((this.GetConnection(who) != null || this.lobby != null && this.lobby.users.Contains(who)) && Network.isActive)
            {
                DevConsole.Log(DCSection.Steam, "|DGYELLOW|" + this.GetDrawString(who) + " has requested a connection.");
                Steam.AcceptConnection(who);
            }
            else if (!Network.isActive)
                DevConsole.Log(DCSection.Steam, "|DGRED| Connection request ignored(" + this.GetDrawString(who) + ")(Network.isActive == false)");
            else
                DevConsole.Log(DCSection.Steam, "|DGRED| Connection request ignored(" + this.GetDrawString(who) + ")(User not found)");
        }

        public void OnConnectionFailed(User who, byte pError) => DevConsole.Log(DCSection.Steam, "|DGRED|Connection with " + this.GetDrawString(who) + " has failed (" + pError.ToString() + ")!");

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
            NCSteam.inviteLobbyID = lobby.id;
            switch (Level.current)
            {
                case TitleScreen _:
                case Editor _:
                case DuckGameTestArea _:
                label_2:
                    NCSteam.PrepareProfilesForJoin();
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
            if (this._lobby == null)
                return NetworkDebugger.enabled ? null : new NCError("|DGORANGE|STEAM |DGRED|Lobby was closed.", NCErrorType.CriticalError);
            if (this._lobby.processing)
                return null;
            return this._lobby.id == 0UL ? new NCError("|DGORANGE|STEAM |DGRED|Failed to create lobby.", NCErrorType.CriticalError) : this.RunSharedLogic();
        }

        protected override NCError OnSpinClientThread()
        {
            if (this._lobby == null)
                return new NCError("|DGORANGE|STEAM |DGYELLOW|Lobby was closed.", NCErrorType.CriticalError);
            if (this._lobby.processing)
                return null;
            return this._lobby.id == 0UL ? new NCError("|DGORANGE|STEAM |DGRED|Failed to join lobby.", NCErrorType.CriticalError) : this.RunSharedLogic();
        }

        protected NCError RunSharedLogic()
        {
            while (true)
            {
                SteamPacket steamPacket = Steam.ReadPacket();
                if (steamPacket != null)
                    this.OnPacket(steamPacket.data, steamPacket.connection);
                else
                    break;
            }
            return null;
        }

        protected override void Disconnect(NetworkConnection c)
        {
            if (c != null && c.data is User data)
            {
                DevConsole.Log(DCSection.Steam, "|DGRED|Closing connection with " + this.GetDrawString(data) + ".");
                Steam.CloseConnection(data);
            }
            base.Disconnect(c);
        }

        protected override void KillConnection()
        {
            if (this._lobby != null)
            {
                if (this._lobby.owner == Steam.user && DuckNetwork.potentialHostObject is User potentialHostObject && this._lobby.users.Contains(potentialHostObject))
                    this._lobby.owner = potentialHostObject;
                Steam_LobbyMessage.Send("IM_OUTTAHERE", null);
                Steam.LeaveLobby(this._lobby);
                this.UnhookLobbyUserStatusChange(this._lobby, new Lobby.UserStatusChangeDelegate(this.OnUserStatusChange));
                this.UnhookLobbyChatMessage(this._lobby, new Lobby.ChatMessageDelegate(this.OnChatMessage));
                DevConsole.Log(DCSection.Steam, "|DGYELLOW|Leaving lobby to host new lobby.");
            }
            this._lobby = null;
            this._lobbyCreationComplete = false;
            this._initializedSettings = false;
            base.KillConnection();
        }

        public override void ApplyLobbyData()
        {
            foreach (MatchSetting matchSetting in TeamSelect2.matchSettings)
            {
                if (matchSetting.value is int)
                    this._lobby.SetLobbyData(matchSetting.id, ((int)matchSetting.value).ToString());
                else if (matchSetting.value is bool)
                    this._lobby.SetLobbyData(matchSetting.id, ((bool)matchSetting.value ? 1 : 0).ToString());
            }
            foreach (MatchSetting onlineSetting in TeamSelect2.onlineSettings)
            {
                if (onlineSetting.id == "password")
                    this._lobby.SetLobbyData("password", (string)onlineSetting.value != "" ? "true" : "false");
                if (onlineSetting.id == "modifiers")
                {
                    if (onlineSetting.filtered)
                        this._lobby.SetLobbyData(onlineSetting.id, (bool)onlineSetting.value ? "true" : "false");
                }
                else if (onlineSetting.id == "dedicated")
                    this._lobby.SetLobbyData(onlineSetting.id, (bool)onlineSetting.value ? "true" : "false");
                else if (onlineSetting.value is int)
                    this._lobby.SetLobbyData(onlineSetting.id, ((int)onlineSetting.value).ToString());
                else if (onlineSetting.value is bool)
                    this._lobby.SetLobbyData(onlineSetting.id, ((bool)onlineSetting.value ? 1 : 0).ToString());
            }
            foreach (UnlockData allUnlock in Unlocks.allUnlocks)
                this._lobby.SetLobbyData(allUnlock.id, (allUnlock.enabled ? 1 : 0).ToString());
            this._lobby.SetLobbyData("customLevels", Editor.customLevelCount.ToString());
        }

        private void TryGettingPingString()
        {
            if (this._lobby != null && !this._lobby.processing && this._lobby.id != 0UL && this.pingWaitTimeout <= 0 && !this.gotPingString)
            {
                string localPingString = Steam.GetLocalPingString();
                this._lobby.SetLobbyData("pingstring", localPingString);
                if (localPingString != null && localPingString != "")
                    this.gotPingString = true;
                this.pingWaitTimeout = 60;
            }
            --this.pingWaitTimeout;
        }

        public override void Update()
        {
            if (this._lobby != null && !this._lobby.processing && this._lobby.id != 0UL)
            {
                if (!this._lobbyCreationComplete)
                {
                    this._lobbyCreationComplete = true;
                    if (Network.isServer)
                    {
                        if (this._lobby.joinResult == SteamLobbyJoinResult.Success)
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
                        if (this._lobby.owner != null && Options.Data.blockedPlayers.Contains(this._lobby.owner.id))
                        {
                            DuckNetwork.FailWithBlockedUser();
                            DevConsole.Log(DCSection.Steam, "|DGRED|You have blocked the host! (" + this._lobby.owner.name + ")");
                            return;
                        }
                        if (UIMatchmakerMark2.instance != null)
                            UIMatchmakerMark2.instance.Hook_OnLobbyProcessed(_lobby);
                        if (this._lobby.joinResult == SteamLobbyJoinResult.Success)
                        {
                            string lobbyData1 = this._lobby.GetLobbyData("version");
                            NMVersionMismatch.Type type = DuckNetwork.CheckVersion(lobbyData1);
                            if (type != NMVersionMismatch.Type.Match)
                            {
                                DuckNetwork.FailWithVersionMismatch(lobbyData1, type);
                                DevConsole.Log(DCSection.Steam, "|DGRED|Lobby version mismatch! (" + type.ToString() + ")");
                                return;
                            }
                            if (this._lobby.GetLobbyData("modhash").Trim() != ModLoader.modHash)
                            {
                                ConnectionError.joinLobby = Steam.lobby;
                                DuckNetwork.FailWithDifferentModsError();
                                return;
                            }
                            string str1 = this._lobby.GetLobbyData("datahash").Trim();
                            if (str1 != Network.gameDataHash.ToString())
                            {
                                DuckNetwork.FailWithDatahashMismatch();
                                DevConsole.Log(DCSection.Steam, "|DGRED|Lobby datahash mismatch! (" + Network.gameDataHash.ToString() + " vs. " + str1 + ")");
                                return;
                            }
                            string lobbyData2 = this._lobby.GetLobbyData("mods");
                            if (lobbyData2 != null && lobbyData2 != "")
                            {
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
                            DevConsole.Log(DCSection.Steam, "|DGGREEN|Lobby Joined (" + this._lobby.owner.name + ")");
                            this.AttemptConnection(_lobby.owner, true);
                        }
                        else
                        {
                            SteamLobbyJoinResult joinResult = this._lobby.joinResult;
                            DevConsole.Log(DCSection.Steam, "|DGGREEN|Failed to join lobby (" + joinResult.ToString() + ")");
                            string msg;
                            if (this._lobby.joinResult == SteamLobbyJoinResult.DoesntExist)
                                msg = "Steam Lobby No Longer Exists.";
                            else if (this._lobby.joinResult == SteamLobbyJoinResult.NotAllowed)
                            {
                                msg = "Failed to Join Lobby (Access Denied)";
                            }
                            else
                            {
                                joinResult = this._lobby.joinResult;
                                msg = "Failed to Join Lobby (" + joinResult.ToString() + ")";
                            }
                            Network.EndNetworkingSession(new DuckNetErrorInfo(DuckNetError.ControlledDisconnect, msg));
                            return;
                        }
                    }
                }
                if (Network.isServer)
                {
                    if (!this._initializedSettings && this._lobby.id != 0UL)
                    {
                        this.UpdateRandomID(this._lobby);
                        this._lobby.SetLobbyData("started", "false");
                        this._lobby.SetLobbyData("version", DG.version);
                        this._lobby.SetLobbyData("beta", "2.0");
                        this._lobby.SetLobbyData("dev", DG.devBuild ? "true" : "false");
                        this._lobby.SetLobbyData("modifiers", "false");
                        this._lobby.SetLobbyData("modhash", ModLoader.modHash);
                        this._lobby.SetLobbyData("datahash", Network.gameDataHash.ToString());
                        this._lobby.SetLobbyData("name", Steam.user.name + "'s Lobby");
                        this._lobby.SetLobbyData("numSlots", DuckNetwork.numSlots.ToString());
                        this._lobby.name = this._serverIdentifier;
                        if (this._lobby.name != TeamSelect2.DefaultGameName())
                            this._lobby.SetLobbyData("customName", "true");
                        string str = "";
                        bool flag = true;
                        foreach (Mod accessibleMod in (IEnumerable<Mod>)ModLoader.accessibleMods)
                        {
                            if (!(accessibleMod is CoreMod) && !(accessibleMod is DisabledMod) && accessibleMod.configuration != null && !accessibleMod.configuration.disabled)
                            {
                                if (!flag)
                                    str += "|";
                                str = accessibleMod.configuration.isWorkshop ? str + accessibleMod.configuration.workshopID.ToString() + "," + accessibleMod.dataHash.ToString() : str + "LOCAL";
                                flag = false;
                            }
                        }
                        this._lobby.SetLobbyModsData(str);
                        this.ApplyLobbyData();
                        this._initializedSettings = true;
                    }
                    if (!this.gotPingString)
                        this.TryGettingPingString();
                }
                if (this._lobby.owner == Steam.user && !Network.isServer)
                {
                    foreach (NetworkConnection connection in this.connections)
                    {
                        if (connection.data is User && connection.isHost && this._lobby.users.Contains(connection.data as User))
                            this._lobby.owner = connection.data as User;
                    }
                }
            }
            base.Update();
        }

        public override void Terminate()
        {
            this._initializedSettings = false;
            this.UnhookDelegates();
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
            if (NCSteam.globalSearch)
                Steam.SearchForLobbyWorldwide();
            else
                Steam.SearchForLobby(null);
            NCSteam.globalSearch = false;
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
