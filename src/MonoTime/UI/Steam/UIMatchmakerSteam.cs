// Decompiled with JetBrains decompiler
// Type: DuckGame.UIMatchmakerSteam
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

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
            this._state = UIMatchmakerMark2.State.GetNumberOfLobbies;
            this._searchAttempts = 0;
            this._resetNetwork = false;
            this._desparate = false;
        }

        public List<Lobby> GetOrderedLobbyList()
        {
            int num1 = 0;
            try
            {
                if (this._hostedLobby != null)
                    num1 = Convert.ToInt32(this._hostedLobby.GetLobbyData("randomID"));
            }
            catch
            {
            }
            List<Lobby> source = new List<Lobby>();
            int num2 = Network.activeNetwork.core.NumLobbiesFound();
            for (int i = 0; i < num2; ++i)
            {
                Lobby searchLobbyAtIndex = Network.activeNetwork.core.GetSearchLobbyAtIndex(i);
                foreach (User user in searchLobbyAtIndex.users)
                    ;
                if (searchLobbyAtIndex.owner != Steam.user && searchLobbyAtIndex.joinable && !this.blacklist.Contains(searchLobbyAtIndex.id) && !this.attempted.Contains(searchLobbyAtIndex.id) && (UIMatchmakingBox.core == null || !UIMatchmakingBox.core.blacklist.Contains(searchLobbyAtIndex.id)))
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
            return source.OrderBy<Lobby, int>((Func<Lobby, int>)(x =>
           {
               int orderedLobbyList = 100;
               if (x.GetLobbyData("version") != DG.version)
                   orderedLobbyList += 100;
               if (UIMatchmakingBox.core != null && UIMatchmakingBox.core.nonPreferredServers.Contains(x.id))
                   orderedLobbyList += 50;
               return orderedLobbyList;
           })).ToList<Lobby>();
        }

        private Lobby TakeLobby()
        {
            if (!this.HasLobby())
                return (Lobby)null;
            Lobby lobby = this.lobbies[this._takeIndex];
            ++this._takeIndex;
            return lobby;
        }

        private Lobby PeekLobby() => this.HasLobby() ? this.lobbies[this._takeIndex] : (Lobby)null;

        private bool HasLobby() => this.lobbies.Count<Lobby>() > 0 && this._takeIndex < this.lobbies.Count;

        private void GetDesparate()
        {
            if (this._desparate)
                return;
            this._desparate = true;
            this.messages.Add("|DGYELLOW|Searching far and wide...");
        }

        protected override void Platform_ResetLogic()
        {
            if (this._hostedLobby == null)
                return;
            this._hostedLobby.joinable = false;
            Steam.LeaveLobby(this._hostedLobby);
        }

        public override void Platform_Update()
        {
            if (this._state == UIMatchmakerMark2.State.JoinLobby && this._timeInState > 480)
                this.Reset();
            if (Input.Pressed("GRAB"))
            {
                this._desparate = false;
                this.GetDesparate();
            }
            if (Network.connections.Count <= 0)
                return;
            if (this._state != UIMatchmakerMark2.State.JoinLobby)
            {
                this.messages.Add("|PURPLE|LOBBY |DGGREEN|Connecting...");
                DevConsole.Log("|PURPLE|LOBBY    |DGGREEN|Network appears to be connecting...", Color.White);
            }
            this.ChangeState(UIMatchmakerMark2.State.JoinLobby);
            this._wait = 0;
        }

        public override void Hook_OnLobbyProcessed(object pLobby)
        {
            if (pLobby is Lobby lobby)
            {
                this.messages.Clear();
                if (lobby.owner != null)
                    this.messages.Add("|LIME|Trying to join " + lobby.owner.name + "'s lobby...");
                else
                    this.messages.Add("|LIME|Trying to join lobby " + this._takeIndex.ToString() + "/" + this.lobbies.Count.ToString() + "...");
            }
            base.Hook_OnLobbyProcessed(pLobby);
        }

        public override void Platform_MatchmakerLogic()
        {
            if (this._state == UIMatchmakerMark2.State.GetNumberOfLobbies)
            {
                NCSteam.globalSearch = true;
                Network.activeNetwork.core.SearchForLobby();
                Network.activeNetwork.core.RequestGlobalStats();
                UIMatchmakerMark2.pulseLocal = true;
                this.ChangeState(UIMatchmakerMark2.State.WaitForQuery);
            }
            else if (this._state == UIMatchmakerMark2.State.SearchForLobbies)
            {
                ++this._searchAttempts;
                if (UIMatchmakerMark2.searchMode == 2 && this._searchAttempts > 1)
                    this.GetDesparate();
                else if (UIMatchmakerMark2.searchMode != 1 && this._searchAttempts > 5)
                    this.GetDesparate();
                NCSteam.globalSearch = this._desparate;
                Network.activeNetwork.core.ApplyTS2LobbyFilters();
                Network.activeNetwork.core.AddLobbyStringFilter("started", "false", LobbyFilterComparison.Equal);
                Network.activeNetwork.core.AddLobbyStringFilter("modhash", ModLoader.modHash, LobbyFilterComparison.Equal);
                Network.activeNetwork.core.AddLobbyStringFilter("password", "false", LobbyFilterComparison.Equal);
                Network.activeNetwork.core.SearchForLobby();
                UIMatchmakerMark2.pulseLocal = true;
                this.ChangeState(UIMatchmakerMark2.State.WaitForQuery);
            }
            else if (this._state == UIMatchmakerMark2.State.TryJoiningLobbies)
            {
                if (this._directConnectLobby != null)
                {
                    this._processing = this._directConnectLobby.lobby;
                    if (this._processing == null)
                    {
                        this.messages.Clear();
                        this.messages.Add("|LIME|Trying to join lobby...");
                        DuckNetwork.Join("", this._directConnectLobby.lanAddress, this._passwordAttempt);
                        this.ChangeState(UIMatchmakerMark2.State.JoinLobby);
                        return;
                    }
                }
                else
                    this._processing = this.PeekLobby();
                if (this._processing == null)
                {
                    if (this._directConnectLobby != null)
                        this.ChangeState(UIMatchmakerMark2.State.Failed);
                    else if (UIMatchmakerMark2.searchMode == 2 && this._searchAttempts < 2)
                        this.ChangeState(UIMatchmakerMark2.State.SearchForLobbies);
                    else if (this.HostLobby())
                    {
                        this._wait = 240;
                        this.ChangeState(UIMatchmakerMark2.State.SearchForLobbies);
                    }
                    else
                        this._wait = 60;
                }
                else
                {
                    this.attempted.Add(this._processing.id);
                    switch (DuckNetwork.CheckVersion(this._processing.GetLobbyData("version")))
                    {
                        case NMVersionMismatch.Type.Match:
                            if (this._processing.GetLobbyData("datahash").Trim() != Network.gameDataHash.ToString())
                            {
                                this.messages.Add("|PURPLE|LOBBY |DGRED|Skipped Lobby (Incompatible)...");
                                this.TakeLobby();
                                if (this._directConnectLobby == null)
                                    return;
                                this.ChangeState(UIMatchmakerMark2.State.Failed);
                                return;
                            }
                            if (!this.Reset())
                                return;
                            this.TakeLobby();
                            if (this._directConnectLobby != null)
                            {
                                this.messages.Clear();
                                if (this._directConnectLobby.name != "" && this._directConnectLobby.name != null)
                                    this.messages.Add("|LIME|Trying to join " + this._directConnectLobby.name + "...");
                                else
                                    this.messages.Add("|LIME|Trying to join lobby...");
                            }
                            DuckNetwork.Join(this._processing.id.ToString(), "localhost", this._passwordAttempt);
                            this.ChangeState(UIMatchmakerMark2.State.JoinLobby);
                            return;
                        case NMVersionMismatch.Type.Older:
                            this.messages.Add("|PURPLE|LOBBY |DGRED|Skipped Lobby (Their version's too old)...");
                            break;
                        case NMVersionMismatch.Type.Newer:
                            this.messages.Add("|PURPLE|LOBBY |DGRED|Skipped Lobby (Their version's too new)...");
                            break;
                        default:
                            this.messages.Add("|PURPLE|LOBBY |DGRED|Skipped Lobby (ERROR)...");
                            break;
                    }
                    this.TakeLobby();
                    if (this._directConnectLobby == null)
                        return;
                    this.ChangeState(UIMatchmakerMark2.State.Failed);
                }
            }
            else if (this._state == UIMatchmakerMark2.State.JoinLobby)
            {
                if (Network.isActive)
                    return;
                this.ChangeState(UIMatchmakerMark2.State.SearchForLobbies);
            }
            else if (this._state == UIMatchmakerMark2.State.Aborting)
            {
                if (Network.isActive)
                    return;
                this.FinishAndClose();
            }
            else
            {
                if (this._state != UIMatchmakerMark2.State.WaitForQuery || !Network.activeNetwork.core.IsLobbySearchComplete())
                    return;
                if (this._previousState == UIMatchmakerMark2.State.GetNumberOfLobbies)
                {
                    UIMatchmakerMark2.pulseNetwork = true;
                    this._totalLobbies = Network.activeNetwork.core.NumLobbiesFound();
                    this.messages.Add("|DGGREEN|Connected to Moon!");
                    this.messages.Add("");
                    this.messages.Add("|DGYELLOW|Searching for companions...");
                    this.ChangeState(UIMatchmakerMark2.State.SearchForLobbies);
                }
                else
                {
                    if (this._previousState != UIMatchmakerMark2.State.SearchForLobbies)
                        return;
                    this._joinableLobbies = Network.activeNetwork.core.NumLobbiesFound();
                    List<Lobby> lobbyList = new List<Lobby>();
                    int num = Math.Max(this._joinableLobbies, 0);
                    DevConsole.Log("|PURPLE|LOBBY    |LIME|found " + num.ToString() + " lobbies.", Color.White);
                    this.lobbies = this.GetOrderedLobbyList();
                    num = this.lobbies.Count;
                    DevConsole.Log("|PURPLE|LOBBY    |LIME|found " + num.ToString() + " compatible lobbies.", Color.White);
                    this._takeIndex = 0;
                    List<string> messages = this.messages;
                    num = this.lobbies.Count;
                    string str = "Found " + num.ToString() + " potential lobbies...";
                    messages.Add(str);
                    this.ChangeState(UIMatchmakerMark2.State.TryJoiningLobbies);
                }
            }
        }
    }
}
