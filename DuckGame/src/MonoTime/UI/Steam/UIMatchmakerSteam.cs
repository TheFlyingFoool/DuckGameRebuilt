using System;
using System.Collections.Generic;
using System.Linq;

namespace DuckGame
{
    internal class UIMatchmakerSteam : UIMatchmakerMark2
    {
        public int _searchAttempts;
        public List<Lobby> lobbies = new List<Lobby>();
        private int _takeIndex;
        protected bool _desparate;

        public UIMatchmakerSteam(UIServerBrowser.LobbyData joinLobby, UIMenu openOnClose)
          : base(joinLobby, openOnClose)
        {
        }

        protected override void Platform_Open()
        {
            _state = State.GetNumberOfLobbies;
            _searchAttempts = 0;
            _resetNetwork = false;
            _desparate = false;
        }

        public List<Lobby> GetOrderedLobbyList()
        {
            int num1 = 0;
            try
            {
                if (_hostedLobby != null)
                    num1 = Convert.ToInt32(_hostedLobby.GetLobbyData("randomID"));
            }
            catch
            {
            }
            List<Lobby> source = new List<Lobby>();
            int num2 = Network.activeNetwork.core.NumLobbiesFound();
            for (int i = 0; i < num2; ++i)
            {
                Lobby searchLobbyAtIndex = Network.activeNetwork.core.GetSearchLobbyAtIndex(i);
                //foreach (User user in searchLobbyAtIndex.users) empty generated code
                //    ;
                if (searchLobbyAtIndex.owner != Steam.user && searchLobbyAtIndex.joinable && !blacklist.Contains(searchLobbyAtIndex.id) && !attempted.Contains(searchLobbyAtIndex.id) && (UIMatchmakingBox.core == null || !UIMatchmakingBox.core.blacklist.Contains(searchLobbyAtIndex.id)))
                {
                    if (num1 != 0)
                    {
                        int num3 = 0;
                        try
                        {
                            num3 = Convert.ToInt32(searchLobbyAtIndex.GetLobbyData("randomID"));
                        }
                        catch
                        {
                        }
                        if (num1 > num3)
                            continue;
                    }
                    source.Add(searchLobbyAtIndex);
                }
            }
            return source.OrderBy(x =>
           {
               int orderedLobbyList = 100;
               if (x.GetLobbyData("version") != DG.version)
                   orderedLobbyList += 100;
               if (UIMatchmakingBox.core != null && UIMatchmakingBox.core.nonPreferredServers.Contains(x.id))
                   orderedLobbyList += 50;
               return orderedLobbyList;
           }).ToList();
        }

        private Lobby TakeLobby()
        {
            if (!HasLobby())
                return null;
            Lobby lobby = lobbies[_takeIndex];
            ++_takeIndex;
            return lobby;
        }

        private Lobby PeekLobby() => HasLobby() ? lobbies[_takeIndex] : null;

        private bool HasLobby() => lobbies.Count > 0 && _takeIndex < lobbies.Count;

        private void GetDesparate()
        {
            if (_desparate)
                return;
            _desparate = true;
            messages.Add("|DGYELLOW|Searching far and wide...");
        }

        protected override void Platform_ResetLogic()
        {
            if (_hostedLobby == null)
                return;
            _hostedLobby.joinable = false;
            Steam.LeaveLobby(_hostedLobby);
        }

        public override void Platform_Update()
        {
            if (_state == State.JoinLobby && _timeInState > 480)
                Reset();
            if (Input.Pressed(Triggers.Grab))
            {
                _desparate = false;
                GetDesparate();
            }
            if (Network.connections.Count <= 0)
                return;
            if (_state != State.JoinLobby)
            {
                messages.Add("|PURPLE|LOBBY |DGGREEN|Connecting...");
                DevConsole.Log("|PURPLE|LOBBY    |DGGREEN|Network appears to be connecting...", Color.White);
            }
            ChangeState(State.JoinLobby);
            _wait = 0;
        }

        public override void Hook_OnLobbyProcessed(object pLobby)
        {
            if (pLobby is Lobby lobby)
            {
                messages.Clear();
                if (lobby.owner != null)
                    messages.Add("|LIME|Trying to join " + lobby.owner.name + "'s lobby...");
                else
                    messages.Add("|LIME|Trying to join lobby " + _takeIndex.ToString() + "/" + lobbies.Count.ToString() + "...");
            }
            base.Hook_OnLobbyProcessed(pLobby);
        }
        private static ulong[] lobbyBotIds = {109775242502588761, 109775242502636562 };
        public override void Platform_MatchmakerLogic()
        {
            if (_state == State.GetNumberOfLobbies)
            {
                NCSteam.globalSearch = true;
                Network.activeNetwork.core.SearchForLobby();
                Network.activeNetwork.core.RequestGlobalStats();
                pulseLocal = true;
                ChangeState(State.WaitForQuery);
            }
            else if (_state == State.SearchForLobbies)
            {
                ++_searchAttempts;
                if (searchMode == 2 && _searchAttempts > 1)
                    GetDesparate();
                else if (searchMode != 1 && _searchAttempts > 5)
                    GetDesparate();
                NCSteam.globalSearch = _desparate;
                Network.activeNetwork.core.ApplyTS2LobbyFilters();
                Network.activeNetwork.core.AddLobbyStringFilter("started", "false", LobbyFilterComparison.Equal);
                Network.activeNetwork.core.AddLobbyStringFilter("modhash", ModLoader.modHash, LobbyFilterComparison.Equal);
                Network.activeNetwork.core.AddLobbyStringFilter("password", "false", LobbyFilterComparison.Equal);
                Network.activeNetwork.core.SearchForLobby();
                pulseLocal = true;
                ChangeState(State.WaitForQuery);
            }
            else if (_state == State.TryJoiningLobbies)
            {
                if (_directConnectLobby != null)
                {
                    _processing = _directConnectLobby.lobby;
                    if (_processing == null)
                    {
                        messages.Clear();
                        messages.Add("|LIME|Trying to join lobby...");
                        DuckNetwork.Join("", _directConnectLobby.lanAddress, _passwordAttempt);
                        ChangeState(State.JoinLobby);
                        return;
                    }
                }
                else
                    _processing = PeekLobby();
                if (_processing == null)
                {
                    if (_directConnectLobby != null)
                        ChangeState(State.Failed);
                    else if (searchMode == 2 && _searchAttempts < 2)
                        ChangeState(State.SearchForLobbies);
                    else if (HostLobby())
                    {
                        _wait = 240;
                        ChangeState(State.SearchForLobbies);
                    }
                    else
                        _wait = 60;
                }
                else
                {
                    attempted.Add(_processing.id);
                    switch (DuckNetwork.CheckVersion(_processing.GetLobbyData("version")))
                    {
                        case NMVersionMismatch.Type.Match:
                            if (_processing.GetLobbyData("dedicated") == "true" || lobbyBotIds.Contains(_processing.id))
                            {
                                messages.Add("|PURPLE|LOBBY |DGRED|Skipped Lobby (Dedicated Likley a lobby bot)...");
                                TakeLobby();
                                if (_directConnectLobby == null)
                                    return;
                                ChangeState(State.Failed);
                                return;
                            }
                            if (_processing.GetLobbyData("datahash").Trim() != Network.gameDataHash.ToString())
                            {
                                messages.Add("|PURPLE|LOBBY |DGRED|Skipped Lobby (Incompatible)...");
                                TakeLobby();
                                if (_directConnectLobby == null)
                                    return;
                                ChangeState(State.Failed);
                                return;
                            }
                            if (!Reset())
                                return;
                            TakeLobby();
                            if (_directConnectLobby != null)
                            {
                                messages.Clear();
                                if (_directConnectLobby.name != "" && _directConnectLobby.name != null)
                                    messages.Add("|LIME|Trying to join " + _directConnectLobby.name + "...");
                                else
                                    messages.Add("|LIME|Trying to join lobby...");
                            }
                            DuckNetwork.Join(_processing.id.ToString(), "localhost", _passwordAttempt);
                            ChangeState(State.JoinLobby);
                            return;
                        case NMVersionMismatch.Type.Older:
                            messages.Add("|PURPLE|LOBBY |DGRED|Skipped Lobby (Their version's too old)...");
                            break;
                        case NMVersionMismatch.Type.Newer:
                            messages.Add("|PURPLE|LOBBY |DGRED|Skipped Lobby (Their version's too new)...");
                            break;
                        default:
                            messages.Add("|PURPLE|LOBBY |DGRED|Skipped Lobby (ERROR)...");
                            break;
                    }
                    TakeLobby();
                    if (_directConnectLobby == null)
                        return;
                    ChangeState(State.Failed);
                }
            }
            else if (_state == State.JoinLobby)
            {
                if (Network.isActive)
                    return;
                ChangeState(State.SearchForLobbies);
            }
            else if (_state == State.Aborting)
            {
                if (Network.isActive)
                    return;
                FinishAndClose();
            }
            else
            {
                if (_state != State.WaitForQuery || !Network.activeNetwork.core.IsLobbySearchComplete())
                    return;
                if (_previousState == State.GetNumberOfLobbies)
                {
                    pulseNetwork = true;
                    _totalLobbies = Network.activeNetwork.core.NumLobbiesFound();
                    messages.Add("|DGGREEN|Connected to Moon!");
                    messages.Add("");
                    messages.Add("|DGYELLOW|Searching for companions...");
                    ChangeState(State.SearchForLobbies);
                }
                else
                {
                    if (_previousState != State.SearchForLobbies)
                        return;
                    _joinableLobbies = Network.activeNetwork.core.NumLobbiesFound();
                    List<Lobby> lobbyList = new List<Lobby>();
                    int num = Math.Max(_joinableLobbies, 0);
                    DevConsole.Log("|PURPLE|LOBBY    |LIME|found " + num.ToString() + " lobbies.", Color.White);
                    lobbies = GetOrderedLobbyList();
                    num = lobbies.Count;
                    DevConsole.Log("|PURPLE|LOBBY    |LIME|found " + num.ToString() + " compatible lobbies.", Color.White);
                    _takeIndex = 0;
                    List<string> messages = this.messages;
                    num = lobbies.Count;
                    string str = "Found " + num.ToString() + " potential lobbies...";
                    messages.Add(str);
                    ChangeState(State.TryJoiningLobbies);
                }
            }
        }
    }
}
