// Decompiled with JetBrains decompiler
// Type: DuckGame.TeamSelect2
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections.Generic;
using System.Linq;

namespace DuckGame
{
    public class TeamSelect2 : Level, IHaveAVirtualTransition
    {
        public static bool KillsForPoints = false;
        public static bool QUACK3;
        private float dim;
        public static bool fakeOnlineImmediately = false;
        public static int customLevels = 0;
        public static int prevCustomLevels = 0;
        public static int prevNumModifiers = 0;
        private BitmapFont _font;
        private SpriteMap _countdown;
        private float _countTime = 1.5f;
        public List<ProfileBox2> _profiles = new List<ProfileBox2>();
        private SpriteMap _buttons;
        private bool _matchSetup;
        private float _setupFade;
        private bool _starting;
        public static UIMenu openImmediately;
        private bool _returnedFromGame;
        public static bool userMapsOnly = false;
        public static bool enableRandom = false;
        public static bool randomMapsOnly = false;
        public static int randomMapPercent = 10;
        public static int normalMapPercent = 90;
        public static int workshopMapPercent = 0;
        public static bool partyMode = false;
        public static bool ctfMode = false;
        private static Dictionary<string, bool> _modifierStatus = new Dictionary<string, bool>();
        public static bool doCalc = false;
        public int setsPerGame = 3;
        private UIMenu _multiplayerMenu;
        private UIMenu _modifierMenu;
        public TeamBeam _beam;
        public TeamBeam _beam2;
        private Sprite _countdownScreen;
        private UIComponent _pauseGroup;
        private UIMenu _pauseMenu;
        private UIComponent _localPauseGroup;
        private UIMenu _localPauseMenu;
        private UIComponent _playOnlineGroup;
        private UIMenu _playOnlineMenu;
        private UIMenu _joinGameMenu;
        private UIMenu _playOnlineBumper;
        private UIMenu _filtersMenu;
        private UIMenu _filterModifierMenu;
        private UIMenu _hostGameMenu;
        private UIMenu _hostMatchSettingsMenu;
        private UIMenu _hostModifiersMenu;
        private UIMenu _hostLevelSelectMenu;
        private UIMenu _hostSettingsMenu;
        //private UIMenu _hostSettingsWirelessGameMenu;
        private UIServerBrowser _browseGamesMenu;
        private UIMatchmakerMark2 _matchmaker;
        private MenuBoolean _returnToMenu = new MenuBoolean();
        private MenuBoolean _inviteFriends = new MenuBoolean();
        private MenuBoolean _findGame = new MenuBoolean();
        private MenuBoolean _backOut = new MenuBoolean();
        private MenuBoolean _localBackOut = new MenuBoolean();
        private MenuBoolean _createGame = new MenuBoolean();
        private MenuBoolean _hostGame = new MenuBoolean();
        public bool openLevelSelect;
        private LevelSelect _levelSelector;
        private UIMenu _inviteMenu;
        private static bool _hostGameEditedMatchSettings = false;
        private bool miniHostMenu;
        public static bool growCamera;
        //private static bool eight = true;
        private UIComponent _configGroup;
        private UIMenu _levelSelectMenu;
        private BitmapFont _littleFont;
        private ProfileBox2 _pauseMenuProfile;
        private bool _singlePlayer;
        private int activePlayers;
        private static bool _attemptingToInvite = false;
        private static List<User> _invitedUsers = new List<User>();
        private static bool _didHost = false;
        private static bool _copyInviteLink = false;
        public static bool showEightPlayerSelected = false;
        //private int fakeOnlineWait = 40;
        private bool _sentDedicatedCountdown;
        private bool explicitlyCreated;
        private Vec2 oldCameraPos = Vec2.Zero;
        private Vec2 oldCameraSize = Vec2.Zero;
        public static bool eightPlayersActive;
        public static bool zoomedOut;
        private float _waitToShow = 1f;
        private static bool _showedPS4Warning = false;
        private float _afkTimeout;
        private static bool _showedOnlineBumper = false;
        private float _timeoutFade;
        private float _topScroll;
        private float _afkMaxTimeout = 300f;
        private float _afkShowTimeout = 241f;
        private int _timeoutBeep;
        private bool _spectatorCountdownStop;

        public override string networkIdentifier => "@TEAMSELECT";

        public ProfileBox2 GetBox(byte box) => this._profiles[box];

        public static List<MatchSetting> matchSettings => DuckNetwork.core.matchSettings;

        public static List<MatchSetting> onlineSettings => DuckNetwork.core.onlineSettings;

        public static string DefaultGameName() => TeamSelect2.GetSettingInt("type") >= 3 && Profiles.active.Count > 0 ? Profiles.active[0].name + "'s LAN Game" : Profiles.experienceProfile.name + "'s Game";

        public static void DefaultSettings(bool resetMatchSettings = true)
        {
            if (resetMatchSettings)
            {
                foreach (MatchSetting matchSetting in TeamSelect2.matchSettings)
                    matchSetting.value = matchSetting.defaultValue;
            }
            foreach (MatchSetting onlineSetting in TeamSelect2.onlineSettings)
                onlineSetting.value = onlineSetting.defaultValue;
            if (resetMatchSettings)
            {
                foreach (UnlockData unlock in Unlocks.GetUnlocks(UnlockType.Modifier))
                    unlock.enabled = false;
                Editor.activatedLevels.Clear();
            }
            TeamSelect2.UpdateModifierStatus();
        }

        public static void DefaultSettingsHostWindow()
        {
            if (TeamSelect2._hostGameEditedMatchSettings)
                TeamSelect2.DefaultSettings();
            TeamSelect2._hostGameEditedMatchSettings = false;
        }

        public static MatchSetting GetMatchSetting(string id) => TeamSelect2.matchSettings.FirstOrDefault<MatchSetting>(x => x.id == id);

        public static MatchSetting GetOnlineSetting(string id) => TeamSelect2.onlineSettings.FirstOrDefault<MatchSetting>(x => x.id == id);

        public static int GetSettingInt(string id)
        {
            foreach (MatchSetting onlineSetting in TeamSelect2.onlineSettings)
            {
                if (onlineSetting.id == id && onlineSetting.value is int)
                    return (int)onlineSetting.value;
            }
            foreach (MatchSetting matchSetting in TeamSelect2.matchSettings)
            {
                if (matchSetting.id == id && matchSetting.value is int)
                    return (int)matchSetting.value;
            }
            return -1;
        }

        public void ClearTeam(int index)
        {
            if (index < 0 || index >= DG.MaxPlayers || this._profiles == null || this._profiles[index]._hatSelector == null)
                return;
            this._profiles[index]._hatSelector._desiredTeamSelection = (sbyte)index;
            if (this._profiles[index].duck != null)
                this._profiles[index].duck.profile.team = Teams.all[index];
            this._profiles[index]._hatSelector.ConfirmTeamSelection();
            this._profiles[index]._hatSelector._teamSelection = this._profiles[index]._hatSelector._desiredTeamSelection = (sbyte)index;
        }

        public static bool GetSettingBool(string id)
        {
            foreach (MatchSetting onlineSetting in TeamSelect2.onlineSettings)
            {
                if (onlineSetting.id == id && onlineSetting.value is bool)
                    return (bool)onlineSetting.value;
            }
            foreach (MatchSetting matchSetting in TeamSelect2.matchSettings)
            {
                if (matchSetting.id == id && matchSetting.value is bool)
                    return (bool)matchSetting.value;
            }
            return false;
        }

        public static int GetLANPort()
        {
            try
            {
                return Convert.ToInt32((string)TeamSelect2.GetOnlineSetting("port").value);
            }
            catch (Exception)
            {
                return 1337;
            }
        }

        public TeamSelect2()
        {
            this._centeredView = true;
            DuckNetwork.core.startCountdown = false;
        }

        public TeamSelect2(bool pReturningFromGame)
          : this()
        {
            this._returnedFromGame = pReturningFromGame;
        }

        public void CloseAllDialogs()
        {
            if (this._playOnlineGroup != null)
                this._playOnlineGroup.Close();
            if (this._playOnlineMenu != null)
                this._playOnlineMenu.Close();
            if (this._joinGameMenu != null)
                this._joinGameMenu.Close();
            if (this._filtersMenu != null)
                this._filtersMenu.Close();
            if (this._filterModifierMenu != null)
                this._filterModifierMenu.Close();
            if (this._hostGameMenu != null)
                this._hostGameMenu.Close();
            if (this._hostMatchSettingsMenu != null)
                this._hostMatchSettingsMenu.Close();
            if (this._hostModifiersMenu != null)
                this._hostModifiersMenu.Close();
            if (this._hostLevelSelectMenu != null)
                this._hostLevelSelectMenu.Close();
            if (this._matchmaker == null)
                return;
            this._matchmaker.Close();
        }

        public bool menuOpen => this._multiplayerMenu.open || this._modifierMenu.open || MonoMain.pauseMenu != null;

        public static bool Enabled(string id, bool ignoreTeamSelect = false)
        {
            if (!ignoreTeamSelect && !Network.InGameLevel())
                return false;
            UnlockData unlock = Unlocks.GetUnlock(id);
            if (unlock == null || Network.isActive && !unlock.onlineEnabled)
                return false;
            bool flag;
            TeamSelect2._modifierStatus.TryGetValue(id, out flag);
            return flag;
        }

        public static bool UpdateModifierStatus()
        {
            bool flag = false;
            foreach (UnlockData unlock in Unlocks.GetUnlocks(UnlockType.Modifier))
            {
                TeamSelect2._modifierStatus[unlock.id] = false;
                if (unlock.enabled)
                {
                    flag = true;
                    TeamSelect2._modifierStatus[unlock.id] = true;
                }
            }
            if (Network.isActive && Network.isServer && Network.activeNetwork.core.lobby != null)
            {
                Network.activeNetwork.core.lobby.SetLobbyData("modifiers", flag ? "true" : "false");
                Network.activeNetwork.core.lobby.SetLobbyData("customLevels", Editor.customLevelCount.ToString());
            }
            return flag;
        }

        public bool isInPlayOnlineMenu => this._playOnlineGroup == null || this._playOnlineGroup.open || MonoMain.pauseMenu != null && MonoMain.pauseMenu.open && MonoMain.pauseMenu is UIGameConnectionBox;

        public bool MatchmakerOpen()
        {
            if (UIMatchmakerMark2.instance != null)
                return true;
            if (MonoMain.pauseMenu != null && MonoMain.pauseMenu.open)
            {
                foreach (UIComponent component in (IEnumerable<UIComponent>)MonoMain.pauseMenu.components)
                {
                    if (component is UIMatchmakingBox && component.open || component is UIMatchmakerMark2 && component.open)
                        return true;
                }
            }
            return false;
        }

        public bool HasBoxOpen(Profile pProfile)
        {
            foreach (ProfileBox2 profile in this._profiles)
            {
                if (profile.profile == pProfile && profile._hatSelector != null && profile._hatSelector.open)
                    return true;
            }
            return false;
        }

        public void OpenDoor(int index, Duck d) => this._profiles[index].OpenDoor(d);

        public void PrepareForOnline()
        {
            TeamSelect2._hostGameEditedMatchSettings = false;
            if (!Network.isServer)
                return;
            GhostManager.context.SetGhostIndex((NetIndex16)32);
            int index = 0;
            foreach (ProfileBox2 profile in this._profiles)
            {
                profile.ChangeProfile(DuckNetwork.profiles[index]);
                ++index;
            }
            foreach (Duck duck in Level.current.things[typeof(Duck)])
            {
                if (duck.ragdoll != null)
                    duck.ragdoll.Unragdoll();
            }
            this.things.RefreshState();
            foreach (Thing thing in this.things)
            {
                thing.DoNetworkInitialize();
                if (Network.isServer && thing.isStateObject)
                    GhostManager.context.MakeGhost(thing);
            }
        }

        private void ShowEightPlayer() => TeamSelect2.showEightPlayerSelected = !TeamSelect2.showEightPlayerSelected;

        public void BuildPauseMenu()
        {
            if (this._pauseGroup != null)
                Level.Remove(_pauseGroup);
            this._pauseGroup = new UIComponent(Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 0.0f, 0.0f)
            {
                isPauseMenu = true
            };
            this._pauseMenu = new UIMenu("@LWING@MULTIPLAYER@RWING@", Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 200f, conString: "@CANCEL@CLOSE @SELECT@SELECT");
            this._inviteMenu = new UIInviteMenu("INVITE FRIENDS", null, Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 160f);
            ((UIInviteMenu)this._inviteMenu).SetAction(new UIMenuActionOpenMenu(_inviteMenu, _pauseMenu));
            UIDivider component1 = new UIDivider(true, 0.8f);
            component1.rightSection.Add(new UIImage("pauseIcons", UIAlign.Right), true);
            this._pauseMenu.Add(component1, true);
            component1.leftSection.Add(new UIMenuItem("RESUME", new UIMenuActionCloseMenu(this._pauseGroup)), true);
            component1.leftSection.Add(new UIMenuItem("OPTIONS", new UIMenuActionOpenMenu(_pauseMenu, Options.optionsMenu)), true);
            component1.leftSection.Add(new UIText("", Color.White), true);
            Options.openOnClose = this._pauseMenu;
            Options.AddMenus(this._pauseGroup);
            if (Network.isActive)
            {
                if (Network.isServer)
                    component1.leftSection.Add(new UIMenuItem("END SESSION", new UIMenuActionCloseMenuSetBoolean(this._pauseGroup, this._backOut)), true);
                else
                    component1.leftSection.Add(new UIMenuItem("DISCONNECT", new UIMenuActionCloseMenuSetBoolean(this._pauseGroup, this._backOut)), true);
            }
            else
            {
                if (this._pauseMenuProfile.playerActive)
                    component1.leftSection.Add(new UIMenuItem("BACK OUT", new UIMenuActionCloseMenuSetBoolean(this._pauseGroup, this._backOut)), true);
                component1.leftSection.Add(new UIMenuItem("|DGRED|MAIN MENU", new UIMenuActionCloseMenuSetBoolean(this._pauseGroup, this._returnToMenu)), true);
            }
            bool flag = false;
            if (!TeamSelect2.eightPlayersActive)
            {
                flag = true;
                component1.leftSection.Add(new UIText("", Color.White), true);
                if (TeamSelect2.showEightPlayerSelected)
                    component1.leftSection.Add(new UIMenuItem("|DGGREEN|HIDE 8 PLAYER", new UIMenuActionCloseMenuCallFunction(this._pauseGroup, new UIMenuActionCloseMenuCallFunction.Function(this.ShowEightPlayer))), true);
                else
                    component1.leftSection.Add(new UIMenuItem("|DGGREEN|SHOW 8 PLAYER", new UIMenuActionCloseMenuCallFunction(this._pauseGroup, new UIMenuActionCloseMenuCallFunction.Function(this.ShowEightPlayer))), true);
            }
            if (Network.available && this._pauseMenuProfile != null && this._pauseMenuProfile.profile.steamID != 0UL && this._pauseMenuProfile.profile == Profiles.experienceProfile)
            {
                if (!flag)
                    component1.leftSection.Add(new UIText("", Color.White), true);
                component1.leftSection.Add(new UIMenuItem("|DGGREEN|INVITE FRIENDS", new UIMenuActionOpenMenu(_pauseMenu, _inviteMenu), UIAlign.Right), true);
                component1.leftSection.Add(new UIMenuItem("|DGGREEN|COPY INVITE LINK", new UIMenuActionCloseMenuCallFunction(this._pauseGroup, new UIMenuActionCloseMenuCallFunction.Function(TeamSelect2.HostGameInviteLink)), UIAlign.Left), true);
            }
            this._pauseMenu.Close();
            this._pauseGroup.Add(_pauseMenu, false);
            this._inviteMenu.Close();
            this._pauseGroup.Add(_inviteMenu, false);
            this._inviteMenu.DoUpdate();
            this._pauseGroup.Close();
            Level.Add(_pauseGroup);
            this._pauseGroup.Update();
            this._pauseGroup.Update();
            this._pauseGroup.Update();
            if (this._localPauseGroup != null)
                Level.Remove(_localPauseGroup);
            this._localPauseGroup = new UIComponent(Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 0.0f, 0.0f);
            this._localPauseMenu = new UIMenu("MULTIPLAYER", Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 160f);
            UIDivider component2 = new UIDivider(true, 0.8f);
            component2.rightSection.Add(new UIImage("pauseIcons", UIAlign.Right), true);
            this._localPauseMenu.Add(component2, true);
            component2.leftSection.Add(new UIMenuItem("RESUME", new UIMenuActionCloseMenu(this._localPauseGroup)), true);
            component2.leftSection.Add(new UIMenuItem("BACK OUT", new UIMenuActionCloseMenuSetBoolean(this._localPauseGroup, this._localBackOut)), true);
            this._localPauseMenu.Close();
            this._localPauseGroup.Add(_localPauseMenu, false);
            this._localPauseGroup.Close();
            Level.Add(_localPauseGroup);
            this._localPauseGroup.Update();
            this._localPauseGroup.Update();
            this._localPauseGroup.Update();
        }

        public void ClearFilters()
        {
            foreach (MatchSetting matchSetting in TeamSelect2.matchSettings)
                matchSetting.filtered = false;
            foreach (UnlockData unlock in Unlocks.GetUnlocks(UnlockType.Modifier))
            {
                unlock.filtered = false;
                unlock.enabled = false;
            }
        }

        public void ClosedOnline()
        {
            foreach (Profile profile in Profiles.all)
                profile.team = null;
            int num = 0;
            foreach (MatchmakingPlayer matchmakingProfile in UIMatchmakingBox.core.matchmakingProfiles)
            {
                foreach (ProfileBox2 profile in this._profiles)
                {
                    if (matchmakingProfile.originallySelectedProfile == profile.profile)
                    {
                        profile.profile.team = matchmakingProfile.team;
                        profile.profile.inputProfile = matchmakingProfile.inputProfile;
                    }
                }
                ++num;
            }
            TeamSelect2.DefaultSettingsHostWindow();
        }

        public static List<byte> GetNetworkModifierList()
        {
            List<byte> networkModifierList = new List<byte>();
            foreach (UnlockData unlock in Unlocks.GetUnlocks(UnlockType.Modifier))
            {
                if (unlock.unlocked && unlock.enabled && Unlocks.modifierToByte.ContainsKey(unlock.id))
                    networkModifierList.Add(Unlocks.modifierToByte[unlock.id]);
            }
            return networkModifierList;
        }

        public static string GetMatchSettingString()
        {
            string matchSettingString = "" + TeamSelect2.GetSettingInt("requiredwins").ToString() + TeamSelect2.GetSettingInt("restsevery").ToString() + TeamSelect2.GetSettingInt("randommaps").ToString() + TeamSelect2.GetSettingInt("workshopmaps").ToString() + TeamSelect2.GetSettingInt("normalmaps").ToString() + ((bool)TeamSelect2.GetOnlineSetting("teams").value).ToString() + TeamSelect2.GetSettingInt("custommaps").ToString() + Editor.activatedLevels.Count.ToString() + TeamSelect2.GetSettingBool("wallmode").ToString() + TeamSelect2.GetSettingBool("clientlevelsenabled").ToString();
            foreach (byte networkModifier in TeamSelect2.GetNetworkModifierList())
                matchSettingString += networkModifier.ToString();
            return matchSettingString;
        }

        public static void SendMatchSettings(NetworkConnection c = null, bool initial = false)
        {
            TeamSelect2.UpdateModifierStatus();
            if (!Network.isActive)
                return;
            Send.Message(new NMMatchSettings(initial, (byte)TeamSelect2.GetSettingInt("requiredwins"), (byte)TeamSelect2.GetSettingInt("restsevery"), (byte)TeamSelect2.GetSettingInt("randommaps"), (byte)TeamSelect2.GetSettingInt("workshopmaps"), (byte)TeamSelect2.GetSettingInt("normalmaps"), (bool)TeamSelect2.GetOnlineSetting("teams").value, (byte)TeamSelect2.GetSettingInt("custommaps"), Editor.activatedLevels.Count, TeamSelect2.GetSettingBool("wallmode"), TeamSelect2.GetNetworkModifierList(), TeamSelect2.GetSettingBool("clientlevelsenabled")), c);
        }

        public void OpenFindGameMenu() => this.OpenFindGameMenu(true);

        public void OpenFindGameMenu(bool pThroughModWindow)
        {
            this._playOnlineGroup.Open();
            this._playOnlineMenu.Open();
            MonoMain.pauseMenu = this._playOnlineGroup;
            if (ModLoader.modHash != "nomods")
                HUD.AddCornerMessage(HUDCorner.TopLeft, "@PLUG@|LIME|Mods enabled.");
            new UIMenuActionOpenMenu(_playOnlineMenu, _joinGameMenu).Activate();
        }

        public void OpenCreateGameMenu() => this.OpenCreateGameMenu(true);

        public void OpenCreateGameMenu(bool pThroughModWindow)
        {
            this._playOnlineGroup.Open();
            this._playOnlineMenu.Open();
            MonoMain.pauseMenu = this._playOnlineGroup;
            if (ModLoader.modHash != "nomods")
                HUD.AddCornerMessage(HUDCorner.TopLeft, "@PLUG@|LIME|Mods enabled.");
            new UIMenuActionOpenMenu(_playOnlineMenu, _hostGameMenu).Activate();
        }

        public void OpenNoModsFindGame()
        {
            TeamSelect2.DefaultSettings(false);
            if (!Options.Data.showNetworkModWarning)
                this.OpenFindGameMenu(false);
            else
                DuckNetwork.OpenNoModsWindow(new UIMenuActionCloseMenuCallFunction.Function(this.OpenFindGameMenu));
        }

        public void OpenNoModsCreateGame()
        {
            if (!Options.Data.showNetworkModWarning)
                this.OpenCreateGameMenu(false);
            else
                DuckNetwork.OpenNoModsWindow(new UIMenuActionCloseMenuCallFunction.Function(this.OpenCreateGameMenu));
        }

        private void SetMatchSettingsOpenedFromHostGame()
        {
            TeamSelect2._hostGameEditedMatchSettings = true;
            this._hostMatchSettingsMenu.SetBackFunction(new UIMenuActionOpenMenu(_hostMatchSettingsMenu, _hostSettingsMenu));
        }

        private void BuildHostMatchSettingsMenu()
        {
            float num1 = 320f;
            float num2 = 180f;
            this._hostMatchSettingsMenu = new UIMenu("@LWING@MATCH SETTINGS@RWING@", Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 190f, conString: "@CANCEL@BACK @SELECT@SELECT");
            this._hostLevelSelectMenu = new LevelSelectCompanionMenu(num1 / 2f, num2 / 2f, this._hostMatchSettingsMenu);
            this._playOnlineGroup.Add(_hostLevelSelectMenu, false);
            this._hostModifiersMenu = new UIMenu("MODIFIERS", Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 240f, conString: "@CANCEL@BACK @SELECT@SELECT");
            foreach (UnlockData unlock in Unlocks.GetUnlocks(UnlockType.Modifier))
            {
                if (unlock.onlineEnabled)
                {
                    if (unlock.unlocked)
                        this._hostModifiersMenu.Add(new UIMenuItemToggle(unlock.GetShortNameForDisplay(), field: new FieldBinding(unlock, "enabled")), true);
                    else
                        this._hostModifiersMenu.Add(new UIMenuItem("@TINYLOCK@LOCKED", c: Color.Red), true);
                }
            }
            this._hostModifiersMenu.SetBackFunction(new UIMenuActionOpenMenu(_hostModifiersMenu, _hostMatchSettingsMenu));
            this._hostModifiersMenu.Close();
            this._playOnlineGroup.Add(_hostModifiersMenu, false);
            this._hostMatchSettingsMenu.AddMatchSetting(TeamSelect2.GetOnlineSetting("teams"), false);
            foreach (MatchSetting matchSetting in TeamSelect2.matchSettings)
            {
                if (!(matchSetting.id == "workshopmaps") || Network.available) //if ((!(matchSetting.id == "workshopmaps") || Network.available) && (!(matchSetting.id == "custommaps") || !ParentalControls.AreParentalControlsActive()))
                {
                    if (matchSetting.id != "partymode")
                        this._hostMatchSettingsMenu.AddMatchSetting(matchSetting, false);
                    if (matchSetting.id == "wallmode")
                        this._hostMatchSettingsMenu.Add(new UIText(" ", Color.White), true);
                }
            }
            this._hostMatchSettingsMenu.Add(new UIText(" ", Color.White), true);
            //if (!ParentalControls.AreParentalControlsActive()) and move the below block back into place
            this._hostMatchSettingsMenu.Add(new UICustomLevelMenu(new UIMenuActionOpenMenu(_hostMatchSettingsMenu, _hostLevelSelectMenu)), true);
            this._hostMatchSettingsMenu.Add(new UIModifierMenuItem(new UIMenuActionOpenMenu(_hostMatchSettingsMenu, _hostModifiersMenu)), true);
            this._hostMatchSettingsMenu.SetBackFunction(new UIMenuActionOpenMenu(_hostMatchSettingsMenu, _hostSettingsMenu));
            this._hostMatchSettingsMenu.Close();
            this._playOnlineGroup.Add(_hostMatchSettingsMenu, false);
        }

        public void OpenHostGameMenuNonMini()
        {
            this.miniHostMenu = false;
            this._hostGameMenu.SetBackFunction(new UIMenuActionOpenMenu(_hostGameMenu, _playOnlineMenu));
        }

        public static void ControllerLayoutsChanged()
        {
        }

        private void CreateGame()
        {
            if (!this.miniHostMenu)
                this._createGame.value = true;
            else
                this._hostGame.value = true;
        }

        public List<Profile> defaultProfiles
        {
            get
            {
                List<Profile> defaultProfiles = new List<Profile>();
                if (Network.isActive)
                {
                    for (int index = 0; index < DG.MaxPlayers; ++index)
                        defaultProfiles.Add(DuckNetwork.profiles[index]);
                }
                else
                {
                    for (int index = 0; index < DG.MaxPlayers; ++index)
                        defaultProfiles.Add(Profile.defaultProfileMappings[index]);
                }
                return defaultProfiles;
            }
        }

        public override void Initialize()
        {
            Program.gameLoadedSuccessfully = true;
            Vote.ClearVotes();
            TeamSelect2.ControllerLayoutsChanged();
            ++Global.data.bootedSinceUpdate;
            ++Global.data.bootedSinceSwitchHatPatch;
            Global.Save();
            if (!Network.isActive)
                Profiles.SaveActiveProfiles();
            if (!Network.isActive)
                Level.core.gameInProgress = false;
            DuckNetwork.inGame = false;
            if (!Level.core.gameInProgress)
            {
                Main.ResetMatchStuff();
                Main.ResetGameStuff();
                DuckNetwork.ClosePauseMenu();
            }
            if (this._returnedFromGame)
            {
                ConnectionStatusUI.Hide();
                if (Network.isServer)
                {
                    if (Network.isActive && Network.activeNetwork.core.lobby != null)
                    {
                        Network.activeNetwork.core.lobby.SetLobbyData("started", "false");
                        Network.activeNetwork.core.lobby.joinable = true;
                    }
                    foreach (Profile profile in DuckNetwork.profiles)
                    {
                        if (profile.connection == null && profile.slotType != SlotType.Reserved && profile.slotType != SlotType.Spectator && profile.slotType != SlotType.Invite)
                            profile.slotType = SlotType.Closed;
                    }
                }
            }
            this._littleFont = new BitmapFont("smallBiosFontUI", 7, 5);
            this._countdownScreen = new Sprite("title/wideScreen");
            this.backgroundColor = Color.Black;
            if (Network.isActive && Network.isServer)
            {
                Network.ContextSwitch(0);
                DuckNetwork.ChangeSlotSettings();
                this.networkIndex = 0;
            }
            this._countdown = new SpriteMap("countdown", 32, 32)
            {
                center = new Vec2(16f, 16f)
            };
            TeamSelect2.showEightPlayerSelected = false;
            List<Profile> defaultProfiles = this.defaultProfiles;
            double xpos1 = 1.0;
            ProfileBox2 profileBox2_1 = new ProfileBox2((float)xpos1, 1f, InputProfile.Get(InputProfile.MPPlayer1), defaultProfiles[0], this, 0);
            this._profiles.Add(profileBox2_1);
            Level.Add(profileBox2_1);
            ProfileBox2 profileBox2_2 = new ProfileBox2((float)(xpos1 + 178.0), 1f, InputProfile.Get(InputProfile.MPPlayer2), defaultProfiles[1], this, 1);
            this._profiles.Add(profileBox2_2);
            Level.Add(profileBox2_2);
            ProfileBox2 profileBox2_3 = new ProfileBox2((float)xpos1, 90f, InputProfile.Get(InputProfile.MPPlayer3), defaultProfiles[2], this, 2);
            this._profiles.Add(profileBox2_3);
            Level.Add(profileBox2_3);
            ProfileBox2 profileBox2_4 = new ProfileBox2((float)(xpos1 + 178.0), 90f, InputProfile.Get(InputProfile.MPPlayer4), defaultProfiles[3], this, 3);
            this._profiles.Add(profileBox2_4);
            Level.Add(profileBox2_4);
            TeamSelect2.growCamera = false;
            double xpos2 = 357.0;
            float num1 = 0.0f;
            ProfileBox2 profileBox2_5 = new ProfileBox2((float)xpos2, num1 + 1f, InputProfile.Get(InputProfile.MPPlayer5), defaultProfiles[4], this, 4);
            this._profiles.Add(profileBox2_5);
            Level.Add(profileBox2_5);
            ProfileBox2 profileBox2_6 = new ProfileBox2((float)xpos2, num1 + 90f, InputProfile.Get(InputProfile.MPPlayer6), defaultProfiles[5], this, 5);
            this._profiles.Add(profileBox2_6);
            Level.Add(profileBox2_6);
            float ypos = 179f;
            ProfileBox2 profileBox2_7 = new ProfileBox2(2f, ypos, InputProfile.Get(InputProfile.MPPlayer7), defaultProfiles[6], this, 6);
            this._profiles.Add(profileBox2_7);
            Level.Add(profileBox2_7);
            ProfileBox2 profileBox2_8 = new ProfileBox2((float)(357.0 - 1.0), ypos, InputProfile.Get(InputProfile.MPPlayer8), defaultProfiles[7], this, 7);
            this._profiles.Add(profileBox2_8);
            Level.Add(profileBox2_8);
            Level.Add(new BlankDoor(178f, 179f));
            Level.Add(new HostTable(160f, 170f));
            if (Network.isActive)
                this.PrepareForOnline();
            this._font = new BitmapFont("biosFont", 8)
            {
                scale = new Vec2(1f, 1f)
            };
            this._buttons = new SpriteMap("buttons", 14, 14);
            this._buttons.CenterOrigin();
            this._buttons.depth = (Depth)0.9f;
            Music.Play("CharacterSelect");
            this._beam = new TeamBeam(160f, 0.0f);
            this._beam2 = new TeamBeam(338f, 0.0f);
            Level.Add(_beam);
            Level.Add(_beam2);
            TeamSelect2.UpdateModifierStatus();
            this._configGroup = new UIComponent(Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 0.0f, 0.0f);
            this._multiplayerMenu = new UIMenu("@LWING@MATCH SETTINGS@RWING@", Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 190f, conString: "@CANCEL@BACK @SELECT@SELECT");
            this._modifierMenu = new UIMenu("MODIFIERS", Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 240f, conString: "@CANCEL@BACK @SELECT@SELECT");
            this._modifierMenu.SetBackFunction(new UIMenuActionOpenMenu(_modifierMenu, _multiplayerMenu));
            this._levelSelectMenu = new LevelSelectCompanionMenu(Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, this._multiplayerMenu);
            foreach (UnlockData unlock in Unlocks.GetUnlocks(UnlockType.Modifier))
            {
                if (unlock.unlocked)
                    this._modifierMenu.Add(new UIMenuItemToggle(unlock.GetShortNameForDisplay(), field: new FieldBinding(unlock, "enabled")), true);
                else
                    this._modifierMenu.Add(new UIMenuItem("@TINYLOCK@LOCKED", c: Color.Red), true);
            }
            this._modifierMenu.Close();
            foreach (MatchSetting matchSetting in TeamSelect2.matchSettings)
            {
                if (!(matchSetting.id == "clientlevelsenabled") && (!(matchSetting.id == "workshopmaps") || Network.available))
                {
                    this._multiplayerMenu.AddMatchSetting(matchSetting, false);
                    if (matchSetting.id == "wallmode")
                        this._multiplayerMenu.Add(new UIText(" ", Color.White), true);
                }
            }
            this._multiplayerMenu.Add(new UIText(" ", Color.White), true);
            this._multiplayerMenu.Add(new UICustomLevelMenu(new UIMenuActionOpenMenu(_multiplayerMenu, _levelSelectMenu)), true);
            this._multiplayerMenu.Add(new UIModifierMenuItem(new UIMenuActionOpenMenu(_multiplayerMenu, _modifierMenu)), true);
            this._multiplayerMenu.Close();
            this._configGroup.Add(_multiplayerMenu, false);
            this._configGroup.Add(_modifierMenu, false);
            this._configGroup.Add(_levelSelectMenu, false);
            this._configGroup.Close();
            Level.Add(_configGroup);
            this._playOnlineGroup = new UIComponent(Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 0.0f, 0.0f);
            this._playOnlineMenu = new UIMenu("@PLANET@PLAY ONLINE@PLANET@", Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 190f, conString: "@CANCEL@BACK @SELECT@SELECT");
            this._hostGameMenu = new UIMenu("@LWING@CREATE GAME@RWING@", Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 190f, conString: "@CANCEL@BACK @SELECT@SELECT");
            this._hostSettingsMenu = new UIMenu("@LWING@HOST SETTINGS@RWING@", Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 190f, conString: "@CANCEL@BACK @SELECT@SELECT");
            int pMinLength = 50;
            float num2 = 3f;
            this._playOnlineBumper = new UIMenu("PLAYING ONLINE", Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 220f, conString: "@SELECT@OK!");
            UIMenu playOnlineBumper1 = this._playOnlineBumper;
            UIText component1 = new UIText("", Color.White, heightAdd: -3f)
            {
                scale = new Vec2(0.5f)
            };
            playOnlineBumper1.Add(component1, true);
            UIMenu playOnlineBumper2 = this._playOnlineBumper;
            UIText component2 = new UIText("There are many tools of expression", Color.White, heightAdd: -4f)
            {
                scale = new Vec2(0.5f)
            };
            playOnlineBumper2.Add(component2, true);
            UIMenu playOnlineBumper3 = this._playOnlineBumper;
            UIText component3 = new UIText("in Duck Game. Please use them for", Color.White, heightAdd: -4f)
            {
                scale = new Vec2(0.5f)
            };
            playOnlineBumper3.Add(component3, true);
            UIMenu playOnlineBumper4 = this._playOnlineBumper;
            UIText component4 = new UIText("|PINK|love|WHITE| and not for |DGRED|hate...|WHITE|", Color.White, heightAdd: -4f)
            {
                scale = new Vec2(0.5f)
            };
            playOnlineBumper4.Add(component4, true);
            UIMenu playOnlineBumper5 = this._playOnlineBumper;
            UIText component5 = new UIText("", Color.White, heightAdd: -3f)
            {
                scale = new Vec2(0.5f)
            };
            playOnlineBumper5.Add(component5, true);
            UIMenu playOnlineBumper6 = this._playOnlineBumper;
            UIText component6 = new UIText("Things every Duck aught to remember:", Color.White, heightAdd: -4f)
            {
                scale = new Vec2(0.5f)
            };
            playOnlineBumper6.Add(component6, true);
            UIMenu playOnlineBumper7 = this._playOnlineBumper;
            UIText component7 = new UIText("", Color.White, heightAdd: -3f)
            {
                scale = new Vec2(0.5f)
            };
            playOnlineBumper7.Add(component7, true);
            UIMenu playOnlineBumper8 = this._playOnlineBumper;
            UIText component8 = new UIText("-Trolling and hate appear exactly the same online.".Padded(pMinLength), Colors.DGBlue, heightAdd: (-num2))
            {
                scale = new Vec2(0.5f)
            };
            playOnlineBumper8.Add(component8, true);
            UIMenu playOnlineBumper9 = this._playOnlineBumper;
            UIText component9 = new UIText("-Please! be kind to one another.".Padded(pMinLength), Colors.DGBlue, heightAdd: (-num2))
            {
                scale = new Vec2(0.5f)
            };
            playOnlineBumper9.Add(component9, true);
            UIMenu playOnlineBumper10 = this._playOnlineBumper;
            UIText component10 = new UIText("-Please! don't use hate speech or strong words.".Padded(pMinLength), Colors.DGBlue, heightAdd: (-num2))
            {
                scale = new Vec2(0.5f)
            };
            playOnlineBumper10.Add(component10, true);
            UIMenu playOnlineBumper11 = this._playOnlineBumper;
            UIText component11 = new UIText("-Please! don't use hacks in public lobbies.".Padded(pMinLength), Colors.DGBlue, heightAdd: (-num2))
            {
                scale = new Vec2(0.5f)
            };
            playOnlineBumper11.Add(component11, true);
            UIMenu playOnlineBumper12 = this._playOnlineBumper;
            UIText component12 = new UIText("-Please! keep custom content tasteful.".Padded(pMinLength), Colors.DGBlue, heightAdd: (-num2))
            {
                scale = new Vec2(0.5f)
            };
            playOnlineBumper12.Add(component12, true);
            UIMenu playOnlineBumper13 = this._playOnlineBumper;
            UIText component13 = new UIText("-Angle shots are neat (and are not hacks).".Padded(pMinLength), Colors.DGBlue, heightAdd: (-num2))
            {
                scale = new Vec2(0.5f)
            };
            playOnlineBumper13.Add(component13, true);
            UIMenu playOnlineBumper14 = this._playOnlineBumper;
            UIText component14 = new UIText("", Color.White, heightAdd: -3f)
            {
                scale = new Vec2(0.5f)
            };
            playOnlineBumper14.Add(component14, true);
            UIMenu playOnlineBumper15 = this._playOnlineBumper;
            UIText component15 = new UIText("If anyone is hacking or being unkind, please", Color.White, heightAdd: -4f)
            {
                scale = new Vec2(0.5f)
            };
            playOnlineBumper15.Add(component15, true);
            UIMenu playOnlineBumper16 = this._playOnlineBumper;
            UIText component16 = new UIText("hover their name in the pause menu", Color.White, heightAdd: -4f)
            {
                scale = new Vec2(0.5f)
            };
            playOnlineBumper16.Add(component16, true);
            UIMenu playOnlineBumper17 = this._playOnlineBumper;
            UIText component17 = new UIText("and go 'Mute -> Block'.", Color.White, heightAdd: -4f)
            {
                scale = new Vec2(0.5f)
            };
            playOnlineBumper17.Add(component17, true);
            UIMenu playOnlineBumper18 = this._playOnlineBumper;
            UIText component18 = new UIText("", Color.White, heightAdd: -3f)
            {
                scale = new Vec2(0.5f)
            };
            playOnlineBumper18.Add(component18, true);
            this._playOnlineBumper.SetAcceptFunction(new UIMenuActionOpenMenu(_playOnlineBumper, _playOnlineMenu));
            this._playOnlineBumper.SetBackFunction(new UIMenuActionOpenMenu(_playOnlineBumper, _playOnlineMenu));
            this._browseGamesMenu = new UIServerBrowser(this._playOnlineMenu, "SERVER BROWSER", Layer.HUD.camera.width, Layer.HUD.camera.height, 550f);
            if (Network.available)
            {
                this._joinGameMenu = new UIMenu("@LWING@FIND GAME@RWING@", Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 190f, conString: "@CANCEL@BACK @SELECT@SELECT");
                this._filtersMenu = new UIMenu("@LWING@FILTERS@RWING@", Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 190f, conString: "@SELECT@SELECT  @MENU2@TYPE");
                this._filterModifierMenu = new UIMenu("@LWING@FILTER MODIFIERS@RWING@", Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 240f, conString: "@CANCEL@BACK @SELECT@SELECT");
            }
            if (Network.available)
                this._matchmaker = UIMatchmakerMark2.Platform_GetMatchkmaker(null, this._joinGameMenu);
            if (ModLoader.modHash != "nomods")
            {
                if (Network.available)
                    this._playOnlineMenu.Add(new UIMenuItem("FIND GAME", new UIMenuActionCloseMenuCallFunction(_playOnlineMenu, new UIMenuActionCloseMenuCallFunction.Function(this.OpenNoModsFindGame))), true);
                this._playOnlineMenu.Add(new UIMenuItem("CREATE GAME", new UIMenuActionCloseMenuCallFunction(_playOnlineMenu, new UIMenuActionCloseMenuCallFunction.Function(this.OpenNoModsCreateGame))), true);
            }
            else
            {
                if (Network.available)
                    this._playOnlineMenu.Add(new UIMenuItem("FIND GAME", new UIMenuActionOpenMenu(_playOnlineMenu, _joinGameMenu)), true);
                this._playOnlineMenu.Add(new UIMenuItem("CREATE GAME", new UIMenuActionOpenMenuCallFunction(_playOnlineMenu, _hostGameMenu, new UIMenuActionOpenMenuCallFunction.Function(this.OpenHostGameMenuNonMini))), true);
            }
            this._playOnlineMenu.Add(new UIMenuItem("BROWSE GAMES", new UIMenuActionOpenMenu(_playOnlineMenu, _browseGamesMenu)), true);
            this._playOnlineMenu.SetBackFunction(new UIMenuActionCloseMenuCallFunction(this._playOnlineGroup, new UIMenuActionCloseMenuCallFunction.Function(this.ClosedOnline)));
            this._playOnlineMenu.Close();
            this._playOnlineGroup.Add(_playOnlineMenu, false);
            this._playOnlineBumper.Close();
            this._playOnlineGroup.Add(_playOnlineBumper, false);
            string str = "";
            bool flag = false;
            foreach (MatchSetting onlineSetting in TeamSelect2.onlineSettings)
            {
                if (!onlineSetting.filterOnly)
                {
                    //if (onlineSetting.id == "customlevelsenabled" && ParentalControls.AreParentalControlsActive())
                    //    str = onlineSetting.id below was else if
                    if (onlineSetting.id == "type" && !Network.available)
                    {
                        str = onlineSetting.id;
                    }
                    else
                    {
                        if (str == "type")
                            flag = true;
                        if (flag)
                        {
                            UIComponent uiComponent = this._hostSettingsMenu.AddMatchSetting(onlineSetting, false);
                            if (uiComponent != null && uiComponent is UIMenuItemString && uiComponent is UIMenuItemString uiMenuItemString)
                                uiMenuItemString.InitializeEntryMenu(this._playOnlineGroup, this._hostSettingsMenu);
                        }
                        else
                        {
                            UIComponent uiComponent = this._hostGameMenu.AddMatchSetting(onlineSetting, false);
                            if (uiComponent != null && uiComponent is UIMenuItemString && uiComponent is UIMenuItemString uiMenuItemString)
                                uiMenuItemString.InitializeEntryMenu(this._playOnlineGroup, this._hostGameMenu);
                        }
                        str = onlineSetting.id;
                    }
                }
            }
            this._hostGameMenu.Add(new UIText(" ", Color.White), true);
            this.BuildHostMatchSettingsMenu();
            this._hostGameMenu.Add(new UIMenuItem("|DGBLUE|SETTINGS", new UIMenuActionOpenMenu(_hostGameMenu, _hostSettingsMenu)), true);
            this._hostSettingsMenu.Add(new UIMenuItem("|DGBLUE|MATCH SETTINGS", new UIMenuActionOpenMenuCallFunction(_hostSettingsMenu, _hostMatchSettingsMenu, new UIMenuActionOpenMenuCallFunction.Function(this.SetMatchSettingsOpenedFromHostGame))), true);
            this._hostSettingsMenu.SetBackFunction(new UIMenuActionOpenMenu(_hostSettingsMenu, _hostGameMenu));
            this._hostGameMenu.Add(new UIMenuItem("|DGGREEN|CREATE GAME", new UIMenuActionCloseMenuCallFunction(this._playOnlineGroup, new UIMenuActionCloseMenuCallFunction.Function(this.CreateGame))), true);
            this._hostGameMenu.SetBackFunction(new UIMenuActionOpenMenu(_hostGameMenu, _playOnlineMenu));
            this._hostGameMenu.Close();
            this._browseGamesMenu.Close();
            this._playOnlineGroup.Add(_browseGamesMenu, false);
            this._playOnlineGroup.Add(_browseGamesMenu._passwordEntryMenu, false);
            this._playOnlineGroup.Add(_browseGamesMenu._portEntryMenu, false);
            this._playOnlineGroup.Add(_hostGameMenu, false);
            this._hostSettingsMenu.Close();
            this._playOnlineGroup.Add(_hostSettingsMenu, false);
            if (Network.available)
            {
                foreach (MatchSetting onlineSetting in TeamSelect2.onlineSettings)
                {
                    //if (!onlineSetting.createOnly && (!(onlineSetting.id == "customlevelsenabled") || !ParentalControls.AreParentalControlsActive()))
                    //    this._joinGameMenu.AddMatchSetting(onlineSetting, true);
                    if (!onlineSetting.createOnly)
                    {
                        this._joinGameMenu.AddMatchSetting(onlineSetting, true, true);
                    }
                }
                this._joinGameMenu.Add(new UIText(" ", Color.White), true);
                this._joinGameMenu.Add(new UIMenuItemNumber("Ping", field: new FieldBinding(typeof(UIMatchmakerMark2), "searchMode", 0.0f, 2f, 0.1f), valStrings: new List<string>()
        {
          "|DGYELLO|PREFER GOOD",
          "|DGGREEN|GOOD PING",
          "|DGREDDD|ANY PING"
        }), true);
                this._joinGameMenu.Add(new UIText(" ", Color.White), true);
                this._joinGameMenu.Add(new UIMenuItem("|DGGREEN|FIND GAME", new UIMenuActionOpenMenu(_joinGameMenu, _matchmaker)), true);
                this._joinGameMenu.AssignDefaultSelection();
                this._joinGameMenu.SetBackFunction(new UIMenuActionOpenMenu(_joinGameMenu, _playOnlineMenu));
                this._joinGameMenu.Close();
                this._playOnlineGroup.Add(_joinGameMenu, false);
                foreach (MatchSetting matchSetting in TeamSelect2.matchSettings)
                {
                    if (!(matchSetting.id == "workshopmaps") || Network.available)
                        this._filtersMenu.AddMatchSetting(matchSetting, true);
                }
                this._filtersMenu.Add(new UIText(" ", Color.White), true);
                this._filtersMenu.Add(new UIModifierMenuItem(new UIMenuActionOpenMenu(_filtersMenu, _filterModifierMenu)), true);
                this._filtersMenu.Add(new UIText(" ", Color.White), true);
                this._filtersMenu.Add(new UIMenuItem("|DGBLUE|CLEAR FILTERS", new UIMenuActionCallFunction(new UIMenuActionCallFunction.Function(this.ClearFilters))), true);
                this._filtersMenu.Add(new UIMenuItem("BACK", new UIMenuActionOpenMenu(_filtersMenu, _joinGameMenu), backButton: true), true);
                this._filtersMenu.Close();
                this._playOnlineGroup.Add(_filtersMenu, false);
                foreach (UnlockData unlock in Unlocks.GetUnlocks(UnlockType.Modifier))
                    this._filterModifierMenu.Add(new UIMenuItemToggle(unlock.GetShortNameForDisplay(), field: new FieldBinding(unlock, "enabled"), filterBinding: new FieldBinding(unlock, "filtered")), true);
                this._filterModifierMenu.Add(new UIText(" ", Color.White), true);
                this._filterModifierMenu.Add(new UIMenuItem("BACK", new UIMenuActionOpenMenu(_filterModifierMenu, _filtersMenu), backButton: true), true);
                this._filterModifierMenu.Close();
                this._playOnlineGroup.Add(_filterModifierMenu, false);
                this._matchmaker.Close();
                this._playOnlineGroup.Add(_matchmaker, false);
            }
            this._playOnlineGroup.Close();
            Level.Add(_playOnlineGroup);
            Graphics.fade = 0.0f;
            Layer l = new Layer("HUD2", -85, new Camera());
            l.camera.width /= 2f;
            l.camera.height /= 2f;
            Layer.Add(l);
            Layer hud = Layer.HUD;
            Layer.HUD = l;
            Layer.HUD = hud;
            if (!DuckNetwork.isDedicatedServer && !DuckNetwork.ShowUserXPGain() && Unlockables.HasPendingUnlocks())
                MonoMain.pauseMenu = new UIUnlockBox(Unlockables.GetPendingUnlocks().ToList<Unlockable>(), Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 190f);
            Level.core.endedGameInProgress = false;
        }

        public void OpenPauseMenu(ProfileBox2 pProfile)
        {
            this._pauseMenuProfile = pProfile;
            this.BuildPauseMenu();
            if (Network.isActive && pProfile.profile != DuckNetwork.localProfile)
            {
                this._localPauseGroup.DoUpdate();
                this._localPauseMenu.DoUpdate();
                this._localPauseGroup.Open();
                this._localPauseMenu.Open();
                MonoMain.pauseMenu = this._localPauseGroup;
            }
            else
            {
                this._pauseGroup.DoUpdate();
                this._pauseMenu.DoUpdate();
                this._pauseGroup.Open();
                this._pauseMenu.Open();
                SFX.Play("pause", 0.6f);
                MonoMain.pauseMenu = this._pauseGroup;
            }
        }

        public override void NetworkDebuggerPrepare() => this.PrepareForOnline();

        public static void HostGameInviteLink()
        {
            if (!Network.isActive)
            {
                TeamSelect2.FillMatchmakingProfiles();
                DuckNetwork.Host(8, NetworkLobbyType.Private);
                (Level.current as TeamSelect2).PrepareForOnline();
                TeamSelect2._didHost = true;
                DevConsole.Log(DCSection.Connection, "Hosting Server via Invite Link!");
            }
            else
                DevConsole.Log(DCSection.Connection, "Copied Invite Link!");
            Main.SpecialCode = "Copied Invite Link.";
            TeamSelect2._copyInviteLink = true;
        }

        public static void DoInvite()
        {
            if (!Network.isActive)
            {
                TeamSelect2.FillMatchmakingProfiles();
                DuckNetwork.Host(8, NetworkLobbyType.Private);
                (Level.current as TeamSelect2).PrepareForOnline();
                TeamSelect2._didHost = true;
            }
            TeamSelect2._attemptingToInvite = true;
            TeamSelect2._copyInviteLink = false;
        }

        public static void InvitedFriend(User u)
        {
            if (!Network.InLobby() || u == null)
                return;
            TeamSelect2._invitedUsers.Add(u);
            DuckNetwork.core._invitedFriends.Add(u.id);
            TeamSelect2.DoInvite();
            Main.SpecialCode = "Invited Friend (" + u.id.ToString() + ")";
            DevConsole.Log(DCSection.Connection, Main.SpecialCode);
        }

        public override void Update()
        {
            if (MonoMain.pauseMenu == null && Options.Data.showControllerWarning && Input.mightHavePlaystationController && !TeamSelect2._showedPS4Warning)
            {
                TeamSelect2._showedPS4Warning = true;
                MonoMain.pauseMenu = Options.controllerWarning;
                Options.controllerWarning.Open();
            }
            if (Level.core.endedGameInProgress && !DuckNetwork.isDedicatedServer)
            {
                this._waitToShow -= Maths.IncFrameTimer();
                if (_waitToShow <= 0.0 && MonoMain.pauseMenu == null)
                {
                    if (!DuckNetwork.ShowUserXPGain() && Unlockables.HasPendingUnlocks())
                        MonoMain.pauseMenu = new UIUnlockBox(Unlockables.GetPendingUnlocks().ToList<Unlockable>(), Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 190f);
                    Level.core.endedGameInProgress = false;
                }
            }
            this.backgroundColor = Color.Black;
            if (TeamSelect2._copyInviteLink && Steam.user != null && Steam.lobby != null && Steam.lobby.id != 0UL)
            {
                DuckNetwork.CopyInviteLink();
                TeamSelect2._copyInviteLink = false;
            }
            bool flag1 = false;
            if (Network.isActive)
            {
                int num = 0;
                foreach (Profile profile in DuckNetwork.profiles)
                {
                    if (profile.slotType != SlotType.Closed && profile.slotType != SlotType.Spectator && (profile.slotType != SlotType.Invite || profile.connection != null || this.explicitlyCreated) && num > 3)
                        flag1 = true;
                    ++num;
                }
            }
            TeamSelect2.eightPlayersActive = Profiles.activeNonSpectators.Count > 4;
            TeamSelect2.zoomedOut = false;
            if (((TeamSelect2.growCamera ? 1 : (UISlotEditor.editingSlots ? 1 : 0)) | (flag1 ? 1 : 0)) != 0 || TeamSelect2.eightPlayersActive || TeamSelect2.showEightPlayerSelected)
            {
                if (this.oldCameraSize == Vec2.Zero)
                {
                    this.oldCameraSize = Level.current.camera.size;
                    this.oldCameraPos = Level.current.camera.position;
                }
                float x = 500f;
                Level.current.camera.size = Lerp.Vec2Smooth(Level.current.camera.size, new Vec2(x, x / 1.77777f), 0.1f, 0.08f);
                Level.current.camera.position = Lerp.Vec2Smooth(Level.current.camera.position, new Vec2(-1f, -7f), 0.1f, 0.08f);
                TeamSelect2.eightPlayersActive = true;
                TeamSelect2.zoomedOut = true;
            }
            else if (this.oldCameraSize != Vec2.Zero)
            {
                Level.current.camera.size = Lerp.Vec2Smooth(Level.current.camera.size, this.oldCameraSize, 0.1f, 0.08f);
                Level.current.camera.position = Lerp.Vec2Smooth(Level.current.camera.position, this.oldCameraPos, 0.1f, 0.08f);
            }
            TeamSelect2.growCamera = false;
            if (this._findGame.value)
            {
                this._findGame.value = false;
                int num = Network.isActive ? 1 : 0;
            }
            if (this._createGame.value || this._hostGame.value)
            {
                this.explicitlyCreated = this._createGame.value;
                if (!Network.available)
                    TeamSelect2.GetOnlineSetting("type").value = 3;
                if ((string)TeamSelect2.GetOnlineSetting("name").value == "")
                    TeamSelect2.GetOnlineSetting("name").value = TeamSelect2.DefaultGameName();
                DuckNetwork.ChangeSlotSettings();
                if (this._hostGame.value)
                    TeamSelect2.FillMatchmakingProfiles();
                bool flag2 = (bool)TeamSelect2.GetOnlineSetting("dedicated").value;
                foreach (MatchmakingPlayer matchmakingProfile in UIMatchmakingBox.core.matchmakingProfiles)
                    matchmakingProfile.spectator = flag2;
                DuckNetwork.Host(TeamSelect2.GetSettingInt("maxplayers"), (NetworkLobbyType)TeamSelect2.GetSettingInt("type"), true);
                this.PrepareForOnline();
                if (this._hostGame.value)
                    this._beam.ClearBeam();
                DevConsole.Log(DCSection.Connection, "Hosting Game(" + UIMatchmakingBox.core.matchmakingProfiles.Count.ToString() + ", " + ((NetworkLobbyType)TeamSelect2.GetSettingInt("type")).ToString() + ")");
                this._createGame.value = false;
                this._hostGame.value = false;
            }
            if (this._inviteFriends.value || TeamSelect2._invitedUsers.Count > 0)
            {
                this._inviteFriends.value = false;
                if (!Network.isActive)
                {
                    TeamSelect2.FillMatchmakingProfiles();
                    DuckNetwork.Host(4, NetworkLobbyType.Private);
                    this.PrepareForOnline();
                }
                TeamSelect2._attemptingToInvite = true;
            }
            if (TeamSelect2._attemptingToInvite && Network.isActive && (!TeamSelect2._didHost || Steam.lobby != null && !Steam.lobby.processing))
            {
                foreach (User invitedUser in TeamSelect2._invitedUsers)
                    Steam.InviteUser(invitedUser, Steam.lobby);
                TeamSelect2._invitedUsers.Clear();
                TeamSelect2._attemptingToInvite = false;
            }
            if (Network.isActive)
            {
                foreach (InputProfile defaultProfile in InputProfile.defaultProfiles)
                {
                    if (defaultProfile.JoinGamePressed())
                        DuckNetwork.JoinLocalDuck(defaultProfile);
                }
            }
            if (Network.isActive && NetworkDebugger.enabled && NetworkDebugger._instances[NetworkDebugger.currentIndex].hover)
            {
                foreach (InputProfile defaultProfile in InputProfile.defaultProfiles)
                {
                    bool flag3 = true;
                    foreach (ProfileBox2 profile in this._profiles)
                    {
                        if (profile.profile != null && (profile.playerActive || profile.profile.connection != null) && profile.profile.inputProfile.genericController == defaultProfile.genericController)
                        {
                            flag3 = false;
                            break;
                        }
                    }
                    if (flag3 && defaultProfile.Pressed("START"))
                    {
                        foreach (ProfileBox2 profile in this._profiles)
                        {
                            if (profile.profile.connection == null)
                            {
                                Profile p = DuckNetwork.JoinLocalDuck(defaultProfile);
                                if (p != null)
                                {
                                    p.inputProfile = defaultProfile;
                                    profile.OpenDoor();
                                    profile.ChangeProfile(p);
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            if (this._levelSelector != null)
            {
                if (this._levelSelector.isClosed)
                {
                    this._levelSelector.Terminate();
                    this._levelSelector = null;
                    Layer.skipDrawing = false;
                    this._beam.active = true;
                    this._beam.visible = true;
                    Editor.selectingLevel = false;
                }
                else
                {
                    this._levelSelector.Update();
                    this._beam.active = false;
                    this._beam.visible = false;
                    Editor.selectingLevel = true;
                    return;
                }
            }
            if (this.openLevelSelect)
            {
                Graphics.fade = Lerp.Float(Graphics.fade, 0.0f, 0.04f);
                if ((double)Graphics.fade >= 0.00999999977648258)
                    return;
                this._levelSelector = new LevelSelect(returnLevel: this);
                this._levelSelector.Initialize();
                this.openLevelSelect = false;
                Layer.skipDrawing = true;
            }
            else
            {
                int num1 = 0;
                this.activePlayers = 0;
                foreach (ProfileBox2 profile in this._profiles)
                {
                    if (profile.ready && profile.profile != null)
                        ++num1;
                    if ((profile.playerActive || profile.duck != null && !profile.duck.dead && !profile.duck.removeFromLevel) && profile.profile != null)
                        ++this.activePlayers;
                }
                this._beam.active = !this.menuOpen;
                if (!this.menuOpen)
                    HUD.CloseAllCorners();
                if (this._backOut.value)
                {
                    if (Network.isActive)
                    {
                        Level.current = new DisconnectFromGame();
                    }
                    else
                    {
                        this._backOut.value = false;
                        if (this._pauseMenuProfile != null)
                        {
                            if (this._beam != null)
                                this._beam.RemoveDuck(this._pauseMenuProfile.duck);
                            this._pauseMenuProfile.CloseDoor();
                            this._pauseMenuProfile = null;
                        }
                    }
                }
                if ((double)Graphics.fade <= 0.0 && this._returnToMenu.value)
                    Level.current = new TitleScreen();
                if (!Network.isActive)
                    DuckNetwork.core.startCountdown = this._starting;
                int num2 = 1;
                if (Network.isActive)
                {
                    num2 = 0;
                    foreach (Profile profile in DuckNetwork.profiles)
                    {
                        if (profile.connection == DuckNetwork.localConnection)
                            ++num2;
                    }
                }
                if (Keyboard.Down(Keys.F1) && !DuckNetwork.TryPeacefulResolution(false))
                {
                    num1 = 2;
                    this.activePlayers = 2;
                    num2 = 0;
                }
                if (this.activePlayers == num1 && !this._returnToMenu.value && (!Network.isActive && num1 > 0 || Network.isActive && num1 > num2))
                {
                    this._singlePlayer = num1 == 1;
                    if (DuckNetwork.core.startCountdown)
                    {
                        DuckNetwork.inGame = true;
                        this.dim = Maths.LerpTowards(this.dim, 0.8f, 0.02f);
                        this._countTime -= 0.006666667f;
                        if (_countTime <= 0.0 && Network.isServer && ((double)Graphics.fade <= 0.0 || NetworkDebugger.enabled))
                        {
                            TeamSelect2.UpdateModifierStatus();
                            DevConsole.qwopMode = TeamSelect2.Enabled("QWOPPY", true);
                            DevConsole.splitScreen = TeamSelect2.Enabled("SPLATSCR", true);
                            DevConsole.rhythmMode = TeamSelect2.Enabled("RHYM", true);
                            DuckNetwork.SetMatchSettings(true, TeamSelect2.GetSettingInt("requiredwins"), TeamSelect2.GetSettingInt("restsevery"), (bool)TeamSelect2.GetOnlineSetting("teams").value, TeamSelect2.GetSettingBool("wallmode"), TeamSelect2.GetSettingInt("normalmaps"), TeamSelect2.GetSettingInt("randommaps"), TeamSelect2.GetSettingInt("workshopmaps"), TeamSelect2.GetSettingInt("custommaps"), Editor.activatedLevels.Count, TeamSelect2.GetNetworkModifierList(), TeamSelect2.GetSettingBool("clientlevelsenabled"));
                            TeamSelect2.partyMode = TeamSelect2.GetSettingBool("partymode");
                            if (Network.isActive && Network.isServer)
                            {
                                foreach (Profile profile in DuckNetwork.profiles)
                                {
                                    if (profile.connection == null && profile.slotType != SlotType.Reserved && profile.slotType != SlotType.Spectator)
                                        profile.slotType = SlotType.Closed;
                                }
                            }
                            if (Network.isActive)
                                TeamSelect2.SendMatchSettings();
                            if (!Level.core.gameInProgress)
                                Main.ResetMatchStuff();
                            Music.Stop();
                            MonoMain.FinishLazyLoad();
                            if (this._singlePlayer)
                            {
                                Level.current.Clear();
                                Level.current = new ArcadeLevel(Content.GetLevelID("arcade"));
                            }
                            else
                            {
                                if (!Network.isServer)
                                    return;
                                foreach (Profile profile in DuckNetwork.profiles)
                                {
                                    profile.reservedUser = null;
                                    if ((profile.connection == null || profile.connection.status != ConnectionStatus.Connected) && profile.slotType == SlotType.Reserved)
                                        profile.slotType = SlotType.Closed;
                                }
                                Level level = !TeamSelect2.ctfMode ? new GameLevel(Deathmatch.RandomLevelString()) : (Level)new CTFLevel(Deathmatch.RandomLevelString(folder: "ctf"));
                                this._spectatorCountdownStop = false;
                                Main.lastLevel = level.level;
                                if (Network.isActive && Network.isServer)
                                {
                                    if (Network.activeNetwork.core.lobby != null)
                                    {
                                        Network.activeNetwork.core.lobby.SetLobbyData("started", "true");
                                        Network.activeNetwork.core.lobby.joinable = false;
                                    }
                                    DuckNetwork.inGame = true;
                                }
                                Level.sendCustomLevels = true;
                                Level.current = level;
                                return;
                            }
                        }
                    }
                    else
                    {
                        DuckNetwork.inGame = false;
                        this.dim = Maths.LerpTowards(this.dim, 0.0f, 0.1f);
                        if (dim < 0.0500000007450581)
                            this._countTime = 1.5f;
                    }
                    this._matchSetup = true;
                    if (Network.isServer)
                    {
                        if (!Network.isActive)
                        {
                            if (!this._singlePlayer && !this._starting && !this.menuOpen && Input.Pressed("MENU1"))
                            {
                                this._configGroup.Open();
                                this._multiplayerMenu.Open();
                                MonoMain.pauseMenu = this._configGroup;
                            }
                            if (!this._starting && !this.menuOpen && Input.Pressed("MENU2"))
                                this.PlayOnlineSinglePlayer();
                        }
                        if (!this.menuOpen && Input.Pressed("SELECT") && (!this._singlePlayer || Profiles.active.Count > 0 && !Profiles.IsDefault(Profiles.active[0])) || DuckNetwork.isDedicatedServer && !this._sentDedicatedCountdown && !this._spectatorCountdownStop)
                        {
                            if (Network.isActive)
                            {
                                Send.Message(new NMBeginCountdown());
                                this._sentDedicatedCountdown = true;
                                this._spectatorCountdownStop = false;
                            }
                            else
                                this._starting = true;
                        }
                        if (Network.isActive && DuckNetwork.isDedicatedServer && !this._spectatorCountdownStop && Input.Pressed("CANCEL"))
                        {
                            this._spectatorCountdownStop = true;
                            this._sentDedicatedCountdown = false;
                            this._starting = false;
                            DuckNetwork.core.startCountdown = false;
                            Send.Message(new NMCancelCountdown());
                        }
                    }
                }
                else
                {
                    this.dim = Maths.LerpTowards(this.dim, 0.0f, 0.1f);
                    if (dim < 0.0500000007450581)
                        this._countTime = 1.5f;
                    this._matchSetup = false;
                    this._starting = false;
                    DuckNetwork.core.startCountdown = false;
                    DuckNetwork.inGame = false;
                    this._sentDedicatedCountdown = false;
                    this._spectatorCountdownStop = false;
                }
                base.Update();
                if (Network.isActive)
                {
                    this._afkTimeout += Maths.IncFrameTimer();
                    foreach (Profile profile in DuckNetwork.profiles)
                    {
                        if (profile.localPlayer && profile.inputProfile != null && profile.inputProfile.Pressed("ANY", true))
                            this._afkTimeout = 0.0f;
                    }
                    if (DuckNetwork.lobbyType == DuckNetwork.LobbyType.FriendsOnly || DuckNetwork.lobbyType == DuckNetwork.LobbyType.Private)
                        this._afkTimeout = 0.0f;
                    if (_afkTimeout > (double)this._afkShowTimeout && (int)this._afkTimeout != this._timeoutBeep)
                    {
                        this._timeoutBeep = (int)this._afkTimeout;
                        SFX.Play("cameraBeep");
                    }
                    if (_afkTimeout > (double)this._afkMaxTimeout)
                        Level.current = new DisconnectFromGame();
                }
                else
                    this._afkTimeout = 0.0f;
                Graphics.fade = Lerp.Float(Graphics.fade, this._returnToMenu.value || _countTime <= 0.0 ? 0.0f : 1f, 0.02f);
                this._setupFade = Lerp.Float(this._setupFade, !this._matchSetup || this.menuOpen || DuckNetwork.core.startCountdown ? 0.0f : 1f, 0.05f);
                Layer.Game.fade = Lerp.Float(Layer.Game.fade, this._matchSetup ? 0.5f : 1f, 0.05f);
            }
        }

        private void PlayOnlineSinglePlayer()
        {
            TeamSelect2.DefaultSettings(false);
            this.PlayOnlineSinglePlayerAfterOnline();
        }

        private void PlayOnlineSinglePlayerAfterOnline()
        {
            TeamSelect2.FillMatchmakingProfiles();
            this._playOnlineGroup.Open();
            if (!TeamSelect2._showedOnlineBumper)
            {
                TeamSelect2._showedOnlineBumper = true;
                this._playOnlineBumper.Open();
            }
            else
                this._playOnlineMenu.Open();
            MonoMain.pauseMenu = this._playOnlineGroup;
        }

        //private void HostOnlineMultipleLocalPlayers() => this.HostOnlineMultipleLocalPlayersAfterOnline();

        //private void HostOnlineMultipleLocalPlayersAfterOnline()
        //{
        //    this._playOnlineGroup.Open();
        //    this.miniHostMenu = true;
        //    this._hostGameMenu.SetBackFunction((UIMenuAction)new UIMenuActionCloseMenuCallFunction(this._playOnlineGroup, new UIMenuActionCloseMenuCallFunction.Function(TeamSelect2.DefaultSettingsHostWindow)));
        //    this._hostGameMenu.Open();
        //    MonoMain.pauseMenu = this._playOnlineGroup;
        //}

        public static void FillMatchmakingProfiles()
        {
            if (Profiles.active.Count == 0)
                NCSteam.PrepareProfilesForJoin();
            for (int index = 0; index < DG.MaxPlayers; ++index)
            {
                if (Level.current is TeamSelect2)
                    (Level.current as TeamSelect2).ClearTeam(index);
            }
            Profile profile1 = Profiles.active.FirstOrDefault<Profile>(x => x == Profiles.experienceProfile);
            Profile profile2 = null;
            if (profile1 == null)
            {
                profile2 = Profiles.active[0];
                profile1 = Profiles.experienceProfile;
            }
            UIMatchmakingBox.core.matchmakingProfiles.Clear();
            foreach (Profile profile3 in Profiles.active.ToList<Profile>())
            {
                profile3.UpdatePersona();
                if (profile3.persona == null)
                    throw new Exception("FillMatchmakingProfiles() p.persona was null!");
                if (profile3.team == null)
                    throw new Exception("FillMatchmakingProfiles() p.team was null!");
                MatchmakingPlayer matchmakingPlayer = new MatchmakingPlayer()
                {
                    inputProfile = profile3.inputProfile,
                    team = profile3.team,
                    persona = profile3.persona,
                    originallySelectedProfile = profile3,
                    customData = null
                };
                if (profile3 == profile2)
                    matchmakingPlayer.isMaster = true;
                else if (profile1 != null && profile1 != profile3)
                    matchmakingPlayer.masterProfile = profile1;
                UIMatchmakingBox.core.matchmakingProfiles.Add(matchmakingPlayer);
            }
        }

        public override void Draw()
        {
        }

        public override void PostDrawLayer(Layer layer)
        {
            if (this._levelSelector != null)
            {
                if (!this._levelSelector.isInitialized)
                    return;
                this._levelSelector.PostDrawLayer(layer);
            }
            else
            {
                if (layer == Layer.Game && UISlotEditor.editingSlots)
                {
                    foreach (ProfileBox2 profileBox2 in Level.current.things[typeof(ProfileBox2)])
                    {
                        if (UISlotEditor._slot == profileBox2.controllerIndex)
                            Graphics.DrawRect(profileBox2.position, profileBox2.position + new Vec2(141f, 89f), Color.White, (Depth)0.95f, false);
                        else
                            Graphics.DrawRect(profileBox2.position, profileBox2.position + new Vec2(141f, 89f), Color.Black * 0.5f, (Depth)0.95f);
                    }
                    foreach (BlankDoor blankDoor in Level.current.things[typeof(BlankDoor)])
                        Graphics.DrawRect(blankDoor.position, blankDoor.position + new Vec2(141f, 89f), Color.Black * 0.5f, (Depth)0.95f);
                }
                Layer background = Layer.Background;
                if (layer == Layer.HUD)
                {
                    if (_afkTimeout >= (double)this._afkShowTimeout)
                    {
                        this._timeoutFade = Lerp.Float(this._timeoutFade, 1f, 0.05f);
                        Graphics.DrawRect(new Vec2(-1000f, -1000f), new Vec2(10000f, 10000f), Color.Black * 0.7f * this._timeoutFade, (Depth)0.95f);
                        string text1 = "AFK TIMEOUT IN";
                        string text2 = ((int)(_afkMaxTimeout - (double)this._afkTimeout)).ToString();
                        Graphics.DrawString(text1, new Vec2((float)((double)layer.width / 2.0 - (double)Graphics.GetStringWidth(text1) / 2.0), (float)((double)layer.height / 2.0 - 8.0)), Color.White * this._timeoutFade, (Depth)0.96f);
                        Graphics.DrawString(text2, new Vec2(layer.width / 2f - Graphics.GetStringWidth(text2), (float)((double)layer.height / 2.0 + 4.0)), Color.White * this._timeoutFade, (Depth)0.96f, scale: 2f);
                    }
                    else
                        this._timeoutFade = Lerp.Float(this._timeoutFade, 0.0f, 0.05f);
                    foreach (Profile profile in DuckNetwork.profiles)
                    {
                        if (profile.reservedUser != null)
                        {
                            if (profile.slotType == SlotType.Reserved)
                                break;
                        }
                    }
                    if (Level.core.gameInProgress)
                    {
                        Vec2 vec2 = new Vec2(0.0f, Layer.HUD.barSize);
                        Graphics.DrawRect(new Vec2(0.0f, vec2.y), new Vec2(320f, vec2.y + 10f), Color.Black, (Depth)0.9f);
                        this._littleFont.depth = (Depth)0.95f;
                        string text3 = "GAME STILL IN PROGRESS, HOST RETURNED TO LOBBY.";
                        string text4 = "";
                        if (text3.Length > 0)
                        {
                            int index = 0;
                            int num = text3.Length * 2;
                            if (num < 90)
                                num = 90;
                            while (text4.Length < num)
                            {
                                text4 += text3[index].ToString();
                                ++index;
                                if (index >= text3.Length)
                                {
                                    index = 0;
                                    text4 += " ";
                                }
                            }
                        }
                        float num1 = 0.01f;
                        if (text3.Length > 20)
                            num1 = 0.005f;
                        if (text3.Length > 30)
                            num1 = 1f / 500f;
                        this._topScroll += num1;
                        if (_topScroll > 1.0)
                            --this._topScroll;
                        if (_topScroll < 0.0)
                            ++this._topScroll;
                        this._littleFont.Draw(text4, new Vec2((float)(1.0 - _topScroll * ((double)this._littleFont.GetWidth(text3) + 7.0)), vec2.y + 3f), Color.White, (Depth)0.95f);
                    }
                    if (_setupFade > 0.00999999977648258)
                    {
                        float num = (float)((double)Layer.HUD.camera.height / 2.0 - 28.0);
                        string str1 = "@MENU2@PLAY ONLINE";
                        if (!Network.available)
                        {
                            str1 = "@MENU2@PLAY LAN (NO STEAM)";
                            if (Steam.user != null && Steam.user.state == SteamUserState.Offline)
                                str1 = "@MENU2@PLAY LAN (STEAM OFFLINE MODE)";
                        }
                        else if (Profiles.active.Count > 3)
                            str1 = "|GRAY|ONLINE UNAVAILABLE (FULL GAME)";
                        if (this._singlePlayer)
                        {
                            string str2 = "@SELECT@CHALLENGE ARCADE";
                            if (Profiles.active.Count == 0 || Profiles.IsDefault(Profiles.active[0]))
                                str2 = "|GRAY|NO ARCADE (SELECT A PROFILE)";
                            if (Network.available)
                            {
                                string text5 = str2;
                                this._font.alpha = this._setupFade;
                                this._font.Draw(text5, (float)((double)Layer.HUD.width / 2.0 - (double)this._font.GetWidth(text5) / 2.0), num + 15f, Color.White, (Depth)0.81f);
                                string text6 = str1;
                                this._font.alpha = this._setupFade;
                                this._font.Draw(text6, (float)((double)Layer.HUD.width / 2.0 - (double)this._font.GetWidth(text6) / 2.0), (float)((double)num + 12.0 + 17.0), Color.White, (Depth)0.81f);
                            }
                            else
                            {
                                string text7 = str2;
                                this._font.alpha = this._setupFade;
                                this._font.Draw(text7, (float)((double)Layer.HUD.width / 2.0 - (double)this._font.GetWidth(text7) / 2.0), num + 15f, Color.White, (Depth)0.81f);
                                string text8 = str1;
                                this._font.alpha = this._setupFade;
                                this._font.Draw(text8, (float)((double)Layer.HUD.width / 2.0 - (double)this._font.GetWidth(text8) / 2.0), (float)((double)num + 12.0 + 17.0), Color.White, (Depth)0.81f);
                            }
                        }
                        else
                        {
                            this._font.alpha = this._setupFade;
                            if (Network.isClient)
                            {
                                string text = "WAITING FOR HOST TO START";
                                if (Level.core.gameInProgress)
                                    text = "WAITING FOR HOST TO RESUME";
                                this._font.Draw(text, (float)((double)Layer.HUD.width / 2.0 - (double)this._font.GetWidth(text) / 2.0), num + 22f, Color.White, (Depth)0.81f);
                            }
                            else if (!Network.isActive)
                            {
                                string text9 = "@SELECT@START MATCH";
                                this._font.Draw(text9, (float)((double)Layer.HUD.width / 2.0 - (double)this._font.GetWidth(text9) / 2.0), num + 9f, Color.White, (Depth)0.81f);
                                string text10 = "@MENU1@MATCH SETTINGS";
                                this._font.Draw(text10, (float)((double)Layer.HUD.width / 2.0 - (double)this._font.GetWidth(text10) / 2.0), num + 22f, Color.White, (Depth)0.81f);
                                string text11 = str1;
                                this._font.Draw(text11, (float)((double)Layer.HUD.width / 2.0 - (double)this._font.GetWidth(text11) / 2.0), num + 35f, Color.White, (Depth)0.81f);
                            }
                            else
                            {
                                string text = "@SELECT@START MATCH";
                                if (Level.core.gameInProgress)
                                    text = "@SELECT@RESUME MATCH";
                                this._font.Draw(text, (float)((double)Layer.HUD.width / 2.0 - (double)this._font.GetWidth(text) / 2.0), num + 22f, Color.White, (Depth)0.81f);
                            }
                        }
                        this._countdownScreen.alpha = this._setupFade;
                        this._countdownScreen.depth = (Depth)0.8f;
                        this._countdownScreen.centery = this._countdownScreen.height / 2;
                        Graphics.Draw(this._countdownScreen, Layer.HUD.camera.x, Layer.HUD.camera.height / 2f);
                    }
                    if (dim > 0.00999999977648258)
                    {
                        this._countdownScreen.alpha = 1f;
                        this._countdownScreen.depth = (Depth)0.8f;
                        this._countdownScreen.centery = this._countdownScreen.height / 2;
                        Graphics.Draw(this._countdownScreen, Layer.HUD.camera.x, Layer.HUD.camera.height / 2f);
                        this._countdown.alpha = this.dim * 1.2f;
                        this._countdown.depth = (Depth)0.81f;
                        this._countdown.frame = (int)(float)Math.Ceiling((1.0 - _countTime) * 2.0);
                        this._countdown.centery = this._countdown.height / 2;
                        if (DuckNetwork.isDedicatedServer)
                        {
                            Graphics.Draw(_countdown, 160f, (float)((double)Layer.HUD.camera.height / 2.0 - 8.0));
                            string text = "@CANCEL@STOP COUNTDOWN";
                            this._font.alpha = this.dim * 1.2f;
                            this._font.Draw(text, (float)((double)Layer.HUD.width / 2.0 - (double)this._font.GetWidth(text) / 2.0), (float)((double)Layer.HUD.camera.height / 2.0 + 8.0), Color.White, (Depth)0.81f);
                        }
                        else
                            Graphics.Draw(_countdown, 160f, (float)((double)Layer.HUD.camera.height / 2.0 - 3.0));
                    }
                }
                base.PostDrawLayer(layer);
            }
        }

        public HatSelector GetHatSelector(int index) => this._profiles[index]._hatSelector;

        public override void OnMessage(NetMessage m)
        {
        }

        public override void OnNetworkConnected(Profile p)
        {
        }

        public override void OnNetworkConnecting(Profile p)
        {
            if (p.networkIndex < this._profiles.Count)
            {
                this._profiles[p.networkIndex].Despawn();
                this._profiles[p.networkIndex].PrepareDoor();
            }
            else
                DevConsole.Log(DCSection.Connection, "@error@|DGRED|TeamSelect2.OnNetworkConnecting out of range(" + p.networkIndex.ToString() + "," + p.slotType.ToString() + ")");
        }

        public override void OnNetworkDisconnected(Profile p)
        {
            if (UIMatchmakerMark2.instance != null || p.networkIndex >= this._profiles.Count)
                return;
            this._profiles[p.networkIndex].Despawn();
        }

        public override void OnSessionEnded(DuckNetErrorInfo error)
        {
            if (UIMatchmakerMark2.instance != null)
                return;
            base.OnSessionEnded(error);
        }

        public override void OnDisconnect(NetworkConnection n)
        {
            if (UIMatchmakerMark2.instance != null || n == null)
                return;
            base.OnDisconnect(n);
        }
    }
}
