using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace DuckGame
{
    public class TeamSelect2 : Level, IHaveAVirtualTransition
    {
        public static bool didcreatelanlobby;
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
        private float _timeoutFade;
        private float _topScroll;
        private float _afkMaxTimeout = 300f;
        private float _afkShowTimeout = 241f;
        private int _timeoutBeep;
        private bool _spectatorCountdownStop;

        private List<Profile> _lastKnownProfiles = new List<Profile>();

        public override string networkIdentifier => "@TEAMSELECT";

        public ProfileBox2 GetBox(byte box) => _profiles[box];

        public static List<MatchSetting> matchSettings => DuckNetwork.core.matchSettings;

        public static List<MatchSetting> onlineSettings => DuckNetwork.core.onlineSettings;

        public static string DefaultGameName()
        {
            List<Profile> activep = Profiles.active;
            if (Network.lanMode)
            {
                if (activep.Count > 0)
                {
                    return activep[0].name + "'s LAN Game";
                }
            }
            string hostName = "";
            activep.ForEach((profile) =>
            {
                if (profile.isHost)
                    hostName = profile.name;
            });
            if (hostName != "")
                return hostName + "'s Game";
            return Profiles.experienceProfile.name + "'s Game";
        }

        public static void DefaultSettings(bool resetMatchSettings = true)
        {
            if (resetMatchSettings)
            {
                foreach (MatchSetting matchSetting in matchSettings)
                    matchSetting.value = matchSetting.defaultValue;
            }
            foreach (MatchSetting onlineSetting in onlineSettings)
                onlineSetting.value = onlineSetting.defaultValue;
            if (resetMatchSettings)
            {
                foreach (UnlockData unlock in Unlocks.GetUnlocks(UnlockType.Modifier))
                    unlock.enabled = false;
                Editor.activatedLevels.Clear();
            }
            UpdateModifierStatus();
        }

        public static void DefaultSettingsHostWindow()
        {
            if (_hostGameEditedMatchSettings)
                DefaultSettings();
            _hostGameEditedMatchSettings = false;
        }

        public static MatchSetting GetMatchSetting(string id) => matchSettings.FirstOrDefault(x => x.id == id);

        public static MatchSetting GetOnlineSetting(string id) => onlineSettings.FirstOrDefault(x => x.id == id);

        public static int GetSettingInt(string id)
        {
            foreach (MatchSetting onlineSetting in onlineSettings)
            {
                if (onlineSetting.id == id && onlineSetting.value is int)
                    return (int)onlineSetting.value;
            }
            foreach (MatchSetting matchSetting in matchSettings)
            {
                if (matchSetting.id == id && matchSetting.value is int)
                    return (int)matchSetting.value;
            }
            return -1;
        }

        public void ClearTeam(int index)
        {
            if (index < 0 || index >= DG.MaxPlayers || _profiles == null || _profiles[index]._hatSelector == null)
                return;
            _profiles[index]._hatSelector._desiredTeamSelection = (sbyte)index;
            if (_profiles[index].duck != null)
                _profiles[index].duck.profile.team = Teams.all[index];
            _profiles[index]._hatSelector.ConfirmTeamSelection();
            _profiles[index]._hatSelector._teamSelection = _profiles[index]._hatSelector._desiredTeamSelection = (sbyte)index;
        }

        public static bool GetSettingBool(string id)
        {
            foreach (MatchSetting onlineSetting in onlineSettings)
            {
                if (onlineSetting.id == id && onlineSetting.value is bool)
                    return (bool)onlineSetting.value;
            }
            foreach (MatchSetting matchSetting in matchSettings)
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
                return Convert.ToInt32((string)GetOnlineSetting("port").value);
            }
            catch (Exception)
            {
                return 1337;
            }
        }
        public bool sign;
        public TeamSelect2()
        {
            _centeredView = true;
            DuckNetwork.core.startCountdown = false;
        }

        public TeamSelect2(bool pReturningFromGame)
          : this()
        {
            _returnedFromGame = pReturningFromGame;
        }

        public void CloseAllDialogs()
        {
            if (_playOnlineGroup != null)
                _playOnlineGroup.Close();
            if (_playOnlineMenu != null)
                _playOnlineMenu.Close();
            if (_joinGameMenu != null)
                _joinGameMenu.Close();
            if (_filtersMenu != null)
                _filtersMenu.Close();
            if (_filterModifierMenu != null)
                _filterModifierMenu.Close();
            if (_hostGameMenu != null)
                _hostGameMenu.Close();
            if (_hostMatchSettingsMenu != null)
                _hostMatchSettingsMenu.Close();
            if (_hostModifiersMenu != null)
                _hostModifiersMenu.Close();
            if (_hostLevelSelectMenu != null)
                _hostLevelSelectMenu.Close();
            if (_matchmaker == null)
                return;
            _matchmaker.Close();
        }

        public bool menuOpen => _multiplayerMenu.open || _modifierMenu.open || MonoMain.pauseMenu != null;

        public static bool Enabled(string id, bool ignoreTeamSelect = false)
        {
            if (!ignoreTeamSelect && !Network.inGameLevel)
                return false;
            UnlockData unlock = Unlocks.GetUnlock(id);
            if (unlock == null || Network.isActive && !unlock.onlineEnabled)
                return false;
            bool flag;
            _modifierStatus.TryGetValue(id, out flag);
            return flag;
        }

        public static bool UpdateModifierStatus()
        {
            bool flag = false;
            foreach (UnlockData unlock in Unlocks.GetUnlocks(UnlockType.Modifier))
            {
                _modifierStatus[unlock.id] = false;
                if (unlock.enabled)
                {
                    flag = true;
                    _modifierStatus[unlock.id] = true;
                }
            }
            if (Network.isActive && Network.isServer && Network.activeNetwork.core.lobby != null)
            {
                Network.activeNetwork.core.lobby.SetLobbyData("modifiers", flag ? "true" : "false");
                Network.activeNetwork.core.lobby.SetLobbyData("customLevels", Editor.customLevelCount.ToString());
            }
            return flag;
        }

        public bool isInPlayOnlineMenu => _playOnlineGroup == null || _playOnlineGroup.open || MonoMain.pauseMenu != null && MonoMain.pauseMenu.open && MonoMain.pauseMenu is UIGameConnectionBox;

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
            foreach (ProfileBox2 profile in _profiles)
            {
                if (profile.profile == pProfile && profile._hatSelector != null && profile._hatSelector.open)
                    return true;
            }
            return false;
        }

        public void OpenDoor(int index, Duck d) => _profiles[index].OpenDoor(d);

        public void PrepareForOnline()
        {
            _hostGameEditedMatchSettings = false;
            if (!Network.isServer)
                return;
            GhostManager.context.SetGhostIndex((NetIndex16)32);
            int index = 0;
            foreach (ProfileBox2 profile in _profiles)
            {
                profile.ChangeProfile(DuckNetwork.profiles[index]);
                ++index;
            }
            foreach (Duck duck in current.things[typeof(Duck)])
            {
                if (duck.ragdoll != null)
                    duck.ragdoll.Unragdoll();
            }
            things.RefreshState();
            foreach (Thing thing in things)
            {
                thing.DoNetworkInitialize();
                if (Network.isServer && thing.isStateObject)
                    GhostManager.context.MakeGhost(thing);
            }

            if (DGRSettings.LastMatchSettings != null && DGRSettings.LastMatchSettings.lengthInBytes > 32 && DGRSettings.RememberMatchSettings)
            {
                /*bf.Write((byte)GetSettingInt("requiredwins"));
            bf.Write((byte)GetSettingInt("restsevery"));
            bf.Write((byte)GetSettingInt("randommaps"));
            bf.Write((byte)GetSettingInt("workshopmaps"));
            bf.Write((byte)GetSettingInt("normalmaps"));
            bf.Write((bool)GetOnlineSetting("teams").value);
            bf.Write((byte)GetSettingInt("custommaps"));
            bf.Write(GetSettingBool("wallmode"));
            bf.Write(GetSettingBool("clientlevelsenabled"));

            List<byte> bs = GetNetworkModifierList();
            bf.Write(bs.Count);
            for (int i = 0; i < bs.Count; i++) bf.Write(bs[i]);


            bf.Write(Editor.activatedLevels.Count);
            for (int i = 0; i < Editor.activatedLevels.Count; i++)
            {
                string s = Editor.activatedLevels[i];
                bf.Write(s);
            }
                */

                BitBuffer bf = DGRSettings.LastMatchSettings;
                bf.position = 0;
                bf.bitOffset = 0;
                
                //by-by-by-by-by-bo-bo-by-int-bys-bo-int-strings
                int varWinsPerSet = bf.ReadByte();
                if (varWinsPerSet == 0) return; //anti match setting destruction system -NiK0
                GetMatchSetting("requiredwins").value = varWinsPerSet;
                int varRoundsPerIntermission = bf.ReadByte();
                GetMatchSetting("restsevery").value = varRoundsPerIntermission;
                int varRandomPercent = bf.ReadByte();
                GetMatchSetting("randommaps").value = varRandomPercent;
                int varWorkshopPercent = bf.ReadByte();
                GetMatchSetting("workshopmaps").value = varWorkshopPercent;
                int varNormalPercent = bf.ReadByte();
                GetMatchSetting("normalmaps").value = varNormalPercent;
                GetOnlineSetting("teams").value = bf.ReadBool();
                int varCustomPercent = bf.ReadByte();
                GetMatchSetting("custommaps").value = varCustomPercent;

                bool varWallmode = bf.ReadBool();
                GetMatchSetting("wallmode").value = varWallmode;

                GetMatchSetting("clientlevelsenabled").value = bf.ReadBool();

                int c = bf.ReadInt();
                List<byte> enabledModifiers = new List<byte>();
                for (int i = 0; i < c; i++)
                {
                    enabledModifiers.Add(bf.ReadByte());
                }
                foreach (UnlockData unlock in Unlocks.GetUnlocks(UnlockType.Modifier))
                {
                    if (Unlocks.modifierToByte.ContainsKey(unlock.id))
                    {
                        byte num2 = Unlocks.modifierToByte[unlock.id];
                        if (enabledModifiers.Contains(num2)) unlock.enabled = true;
                        else unlock.enabled = false;
                        unlock.prevEnabled = unlock.enabled;
                    }
                }

                GameMode.roundsBetweenIntermission = varRoundsPerIntermission;
                GameMode.winsPerSet = varWinsPerSet;
                Deathmatch.userMapsPercent = varCustomPercent;
                randomMapPercent = varRandomPercent;
                normalMapPercent = varNormalPercent;
                workshopMapPercent = varWorkshopPercent;
                UpdateModifierStatus();
                RockScoreboard.wallMode = varWallmode;


                c = bf.ReadInt();
                DuckNetwork.core._activatedLevels = new List<string>();
                for (int i = 0; i < c; i++)
                {
                    string s = bf.ReadString();
                    DuckNetwork.core._activatedLevels.Add(s);
                }
            }
        }

        private void ShowEightPlayer() => showEightPlayerSelected = !showEightPlayerSelected;

        public void BuildPauseMenu()
        {
            if (_pauseGroup != null)
                Remove(_pauseGroup);
            _pauseGroup = new UIComponent(Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 0f, 0f)
            {
                isPauseMenu = true
            };
            _pauseMenu = new UIMenu("@LWING@MULTIPLAYER@RWING@", Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 200f, conString: "@CANCEL@CLOSE @SELECT@SELECT");
            _inviteMenu = new UIInviteMenu("INVITE FRIENDS", null, Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 160f);
            ((UIInviteMenu)_inviteMenu).SetAction(new UIMenuActionOpenMenu(_inviteMenu, _pauseMenu));
            UIDivider component1 = new UIDivider(true, 0.8f);
            component1.rightSection.Add(new UIImage("pauseIcons", UIAlign.Right), true);
            _pauseMenu.Add(component1, true);
            component1.leftSection.Add(new UIMenuItem("RESUME", new UIMenuActionCloseMenu(_pauseGroup)), true);
            component1.leftSection.Add(new UIMenuItem("OPTIONS", new UIMenuActionOpenMenu(_pauseMenu, Options.optionsMenu)), true);
            component1.leftSection.Add(new UIText("", Color.White), true);
            Options.openOnClose = _pauseMenu;
            Options.AddMenus(_pauseGroup);
            if (Network.isActive)
            {
                if (Network.isServer)
                    component1.leftSection.Add(new UIMenuItem("END SESSION", new UIMenuActionCloseMenuSetBoolean(_pauseGroup, _backOut)), true);
                else
                    component1.leftSection.Add(new UIMenuItem("DISCONNECT", new UIMenuActionCloseMenuSetBoolean(_pauseGroup, _backOut)), true);
            }
            else
            {
                if (_pauseMenuProfile.playerActive)
                    component1.leftSection.Add(new UIMenuItem("BACK OUT", new UIMenuActionCloseMenuSetBoolean(_pauseGroup, _backOut)), true);
                component1.leftSection.Add(new UIMenuItem("|DGRED|MAIN MENU", new UIMenuActionCloseMenuSetBoolean(_pauseGroup, _returnToMenu)), true);
            }
            bool flag = false;
            if (!eightPlayersActive)
            {
                flag = true;
                component1.leftSection.Add(new UIText("", Color.White), true);
                if (showEightPlayerSelected)
                    component1.leftSection.Add(new UIMenuItem("|DGGREEN|HIDE 8 PLAYER", new UIMenuActionCloseMenuCallFunction(_pauseGroup, new UIMenuActionCloseMenuCallFunction.Function(ShowEightPlayer))), true);
                else
                    component1.leftSection.Add(new UIMenuItem("|DGGREEN|SHOW 8 PLAYER", new UIMenuActionCloseMenuCallFunction(_pauseGroup, new UIMenuActionCloseMenuCallFunction.Function(ShowEightPlayer))), true);
            }
            if (Network.available && _pauseMenuProfile != null && _pauseMenuProfile.profile.steamID != 0UL && _pauseMenuProfile.profile == Profiles.experienceProfile)
            {
                if (!flag)
                    component1.leftSection.Add(new UIText("", Color.White), true);
                component1.leftSection.Add(new UIMenuItem("|DGGREEN|INVITE FRIENDS", new UIMenuActionOpenMenu(_pauseMenu, _inviteMenu), UIAlign.Right), true);
                component1.leftSection.Add(new UIMenuItem("|DGGREEN|COPY INVITE LINK", new UIMenuActionCloseMenuCallFunction(_pauseGroup, new UIMenuActionCloseMenuCallFunction.Function(HostGameInviteLink)), UIAlign.Left), true);
            }
            _pauseMenu.Close();
            _pauseGroup.Add(_pauseMenu, false);
            _inviteMenu.Close();
            _pauseGroup.Add(_inviteMenu, false);
            _inviteMenu.DoUpdate();
            _pauseGroup.Close();
            Add(_pauseGroup);
            _pauseGroup.Update();
            _pauseGroup.Update();
            _pauseGroup.Update();
            if (_localPauseGroup != null)
                Remove(_localPauseGroup);
            _localPauseGroup = new UIComponent(Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 0f, 0f);
            _localPauseMenu = new UIMenu("MULTIPLAYER", Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 160f);
            UIDivider component2 = new UIDivider(true, 0.8f);
            component2.rightSection.Add(new UIImage("pauseIcons", UIAlign.Right), true);
            _localPauseMenu.Add(component2, true);
            component2.leftSection.Add(new UIMenuItem("RESUME", new UIMenuActionCloseMenu(_localPauseGroup)), true);
            component2.leftSection.Add(new UIMenuItem("BACK OUT", new UIMenuActionCloseMenuSetBoolean(_localPauseGroup, _localBackOut)), true);
            _localPauseMenu.Close();
            _localPauseGroup.Add(_localPauseMenu, false);
            _localPauseGroup.Close();
            Add(_localPauseGroup);
            _localPauseGroup.Update();
            _localPauseGroup.Update();
            _localPauseGroup.Update();
        }

        public void ClearFilters()
        {
            foreach (MatchSetting matchSetting in matchSettings)
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
                foreach (ProfileBox2 profile in _profiles)
                {
                    if (matchmakingProfile.originallySelectedProfile == profile.profile)
                    {
                        profile.profile.team = matchmakingProfile.team;
                        profile.profile.inputProfile = matchmakingProfile.inputProfile;
                    }
                }
                ++num;
            }
            DefaultSettingsHostWindow();
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
            string matchSettingString = "" + GetSettingInt("requiredwins").ToString() + GetSettingInt("restsevery").ToString() + GetSettingInt("randommaps").ToString() + GetSettingInt("workshopmaps").ToString() + GetSettingInt("normalmaps").ToString() + ((bool)GetOnlineSetting("teams").value).ToString() + GetSettingInt("custommaps").ToString() + Editor.activatedLevels.Count.ToString() + GetSettingBool("wallmode").ToString() + GetSettingBool("clientlevelsenabled").ToString();
            foreach (byte networkModifier in GetNetworkModifierList())
                matchSettingString += networkModifier.ToString();
            return matchSettingString;
        }

        public static void SendMatchSettings(NetworkConnection c = null, bool initial = false)
        {
            BitBuffer bf = new BitBuffer();
            bf.Write((byte)GetSettingInt("requiredwins"));
            bf.Write((byte)GetSettingInt("restsevery"));
            bf.Write((byte)GetSettingInt("randommaps"));
            bf.Write((byte)GetSettingInt("workshopmaps"));
            bf.Write((byte)GetSettingInt("normalmaps"));
            bf.Write((bool)GetOnlineSetting("teams").value);
            bf.Write((byte)GetSettingInt("custommaps"));
            bf.Write((bool)GetSettingBool("wallmode"));
            bf.Write((bool)GetSettingBool("clientlevelsenabled"));

            List<byte> bs = GetNetworkModifierList();
            bf.Write(bs.Count);
            for (int i = 0; i < bs.Count; i++) bf.Write(bs[i]);


            bf.Write(Editor.activatedLevels.Count);
            for (int i = 0; i < Editor.activatedLevels.Count; i++)
            {
                string s = Editor.activatedLevels[i];
                bf.Write(s);
            }

            bf.position = 0;
            bf.bitOffset = 0;
            DGRSettings.LMatchSetSave = bf.buffer;
            
            UpdateModifierStatus();
            if (!Network.isActive)
                return;
            Send.Message(new NMMatchSettings(initial, (byte)GetSettingInt("requiredwins"), (byte)GetSettingInt("restsevery"), (byte)GetSettingInt("randommaps"), (byte)GetSettingInt("workshopmaps"), (byte)GetSettingInt("normalmaps"), (bool)GetOnlineSetting("teams").value, (byte)GetSettingInt("custommaps"), Editor.activatedLevels.Count, GetSettingBool("wallmode"), GetNetworkModifierList(), GetSettingBool("clientlevelsenabled")), c);
        }

        public void OpenFindGameMenu() => OpenFindGameMenu(true);

        public void OpenFindGameMenu(bool pThroughModWindow)
        {
            _playOnlineGroup.Open();
            _playOnlineMenu.Open();
            MonoMain.pauseMenu = _playOnlineGroup;
            if (ModLoader.modHash != "nomods")
                HUD.AddCornerMessage(HUDCorner.TopLeft, "@PLUG@|LIME|Mods enabled.");
            new UIMenuActionOpenMenu(_playOnlineMenu, _joinGameMenu).Activate();
        }

        public void OpenCreateGameMenu() => OpenCreateGameMenu(true);

        public void OpenCreateGameMenu(bool pThroughModWindow)
        {
            _playOnlineGroup.Open();
            _playOnlineMenu.Open();
            MonoMain.pauseMenu = _playOnlineGroup;
            if (ModLoader.modHash != "nomods")
                HUD.AddCornerMessage(HUDCorner.TopLeft, "@PLUG@|LIME|Mods enabled.");
            new UIMenuActionOpenMenu(_playOnlineMenu, _hostGameMenu).Activate();
        }

        public void OpenNoModsFindGame()
        {
            DefaultSettings(false);
            if (!Options.Data.showNetworkModWarning)
                OpenFindGameMenu(false);
            else
                DuckNetwork.OpenNoModsWindow(new UIMenuActionCloseMenuCallFunction.Function(OpenFindGameMenu));
        }

        public void OpenNoModsCreateGame()
        {
            if (!Options.Data.showNetworkModWarning)
                OpenCreateGameMenu(false);
            else
                DuckNetwork.OpenNoModsWindow(new UIMenuActionCloseMenuCallFunction.Function(OpenCreateGameMenu));
        }

        private void SetMatchSettingsOpenedFromHostGame()
        {
            _hostGameEditedMatchSettings = true;
            _hostMatchSettingsMenu.SetBackFunction(new UIMenuActionOpenMenuCallFunction(_hostMatchSettingsMenu, _hostSettingsMenu, HUD.CloseAllCorners));
        }

        private void BuildHostMatchSettingsMenu()
        {
            _hostMatchSettingsMenu = new UIMenu("@LWING@MATCH SETTINGS@RWING@", Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 190f, conString: "@CANCEL@BACK @SELECT@SELECT");
            _hostLevelSelectMenu = new LevelSelectCompanionMenu(160f, 90f, _hostMatchSettingsMenu);
            _playOnlineGroup.Add(_hostLevelSelectMenu, false);
            _hostModifiersMenu = new UIMenu("MODIFIERS", Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 240f, conString: "@CANCEL@BACK @SELECT@SELECT");
            foreach (UnlockData unlock in Unlocks.GetUnlocks(UnlockType.Modifier))
            {
                if (unlock.onlineEnabled)
                {
                    if (unlock.unlocked)
                        _hostModifiersMenu.Add(new UIMenuItemToggle(unlock.GetShortNameForDisplay(), field: new FieldBinding(unlock, "enabled")), true);
                    else
                        _hostModifiersMenu.Add(new UIMenuItem("@TINYLOCK@LOCKED", c: Color.Red), true);
                }
            }
            _hostModifiersMenu.SetBackFunction(new UIMenuActionOpenMenu(_hostModifiersMenu, _hostMatchSettingsMenu));
            _hostModifiersMenu.Close();
            _playOnlineGroup.Add(_hostModifiersMenu, false);
            _hostMatchSettingsMenu.AddMatchSetting(GetOnlineSetting("teams"), false);
            foreach (MatchSetting matchSetting in matchSettings)
            {
                if (!(matchSetting.id == "workshopmaps") || Network.available) //if ((!(matchSetting.id == "workshopmaps") || Network.available) && (!(matchSetting.id == "custommaps") || !ParentalControls.AreParentalControlsActive()))
                {
                    if (matchSetting.id != "partymode")
                        _hostMatchSettingsMenu.AddMatchSettingL(matchSetting, false);
                    if (matchSetting.id == "wallmode")
                        _hostMatchSettingsMenu.Add(new UIText(" ", Color.White), true);
                }
            }
            _hostMatchSettingsMenu.Add(new UIText(" ", Color.White), true);
            //if (!ParentalControls.AreParentalControlsActive()) and move the below block back into place
            _hostMatchSettingsMenu.Add(new UICustomLevelMenu(new UIMenuActionOpenMenu(_hostMatchSettingsMenu, _hostLevelSelectMenu)), true);
            _hostMatchSettingsMenu.Add(new UIModifierMenuItem(new UIMenuActionOpenMenu(_hostMatchSettingsMenu, _hostModifiersMenu)), true);
            _hostMatchSettingsMenu.SetBackFunction(new UIMenuActionOpenMenuCallFunction(_hostMatchSettingsMenu, _hostSettingsMenu, HUD.CloseAllCorners));
            _hostMatchSettingsMenu.SetOpenFunction(new UIMenuActionCallFunction(() =>
            {
                HUD.CloseAllCorners();
                HUD.AddCornerControl(HUDCorner.BottomRight, "@ALT@FINE ADJUST");
            }));
            _hostMatchSettingsMenu.Close();
            _playOnlineGroup.Add(_hostMatchSettingsMenu, false);
        }

        public void OpenHostGameMenuNonMini()
        {
            miniHostMenu = false;
            _hostGameMenu.SetBackFunction(new UIMenuActionOpenMenu(_hostGameMenu, _playOnlineMenu));
        }

        public static void ControllerLayoutsChanged()
        {
        }

        private void CreateGame()
        {
            if (!miniHostMenu)
                _createGame.value = true;
            else
                _hostGame.value = true;
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
            DGRSettings.InitializeFavoritedHats();
            if (sign)
            {
                Add(new VersionSign(32, -20) { fadeTime = 300 });
            }
            Program.main.IsFixedTimeStep = true;
            Program.gameLoadedSuccessfully = true;
            Vote.ClearVotes();
            ControllerLayoutsChanged();
            ++Global.data.bootedSinceUpdate;
            ++Global.data.bootedSinceSwitchHatPatch;
            Global.Save();
            if (!Network.isActive)
                Profiles.SaveActiveProfiles();
            if (!Network.isActive)
                core.gameInProgress = false;
            DuckNetwork.inGame = false;
            if (!core.gameInProgress)
            {
                Main.ResetMatchStuff();
                Main.ResetGameStuff();
                DuckNetwork.ClosePauseMenu();
            }
            if (_returnedFromGame)
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
            _littleFont = new BitmapFont("smallBiosFontUI", 7, 5);
            _countdownScreen = new Sprite("title/wideScreen");
            backgroundColor = Color.Black;
            if (Network.isActive && Network.isServer)
            {
                Network.ContextSwitch(0);
                DuckNetwork.ChangeSlotSettings();
                networkIndex = 0;
            }
            _countdown = new SpriteMap("countdown", 32, 32)
            {
                center = new Vec2(16f, 16f)
            };
            showEightPlayerSelected = false;
            List<Profile> defaultProfiles = this.defaultProfiles;
            ProfileBox2 profileBox2_1 = new ProfileBox2(1, 1f, InputProfile.Get(InputProfile.MPPlayer1), defaultProfiles[0], this, 0);
            _profiles.Add(profileBox2_1);
            Add(profileBox2_1);
            ProfileBox2 profileBox2_2 = new ProfileBox2(179, 1f, InputProfile.Get(InputProfile.MPPlayer2), defaultProfiles[1], this, 1);
            _profiles.Add(profileBox2_2);
            Add(profileBox2_2);
            ProfileBox2 profileBox2_3 = new ProfileBox2(1, 90f, InputProfile.Get(InputProfile.MPPlayer3), defaultProfiles[2], this, 2);
            _profiles.Add(profileBox2_3);
            Add(profileBox2_3);
            ProfileBox2 profileBox2_4 = new ProfileBox2(179, 90f, InputProfile.Get(InputProfile.MPPlayer4), defaultProfiles[3], this, 3);
            _profiles.Add(profileBox2_4);
            Add(profileBox2_4);
            growCamera = false;
            ProfileBox2 profileBox2_5 = new ProfileBox2(357, 1f, InputProfile.Get(InputProfile.MPPlayer5), defaultProfiles[4], this, 4);
            _profiles.Add(profileBox2_5);
            Add(profileBox2_5);
            ProfileBox2 profileBox2_6 = new ProfileBox2(357, 90f, InputProfile.Get(InputProfile.MPPlayer6), defaultProfiles[5], this, 5);
            _profiles.Add(profileBox2_6);
            Add(profileBox2_6);
            ProfileBox2 profileBox2_7 = new ProfileBox2(2f, 179, InputProfile.Get(InputProfile.MPPlayer7), defaultProfiles[6], this, 6);
            _profiles.Add(profileBox2_7);
            Add(profileBox2_7);
            ProfileBox2 profileBox2_8 = new ProfileBox2(356, 179, InputProfile.Get(InputProfile.MPPlayer8), defaultProfiles[7], this, 7);
            _profiles.Add(profileBox2_8);
            Add(profileBox2_8);
            Add(new BlankDoor(178f, 179f));
            Add(new HostTable(160f, 170f));
            if (Network.isActive)
                PrepareForOnline();
            _font = new BitmapFont("biosFont", 8)
            {
                scale = new Vec2(1f, 1f)
            };
            _buttons = new SpriteMap("buttons", 14, 14);
            _buttons.CenterOrigin();
            _buttons.depth = (Depth)0.9f;
            Music.Play("CharacterSelect");
            _beam = new TeamBeam(160f, 0f);
            _beam2 = new TeamBeam(338f, 0f);
            Add(_beam);
            Add(_beam2);
            UpdateModifierStatus();
            _configGroup = new UIComponent(Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 0f, 0f);
            _multiplayerMenu = new UIMenu("@LWING@MATCH SETTINGS@RWING@", Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 190f, conString: "@CANCEL@BACK @SELECT@SELECT");
            _modifierMenu = new UIMenu("MODIFIERS", Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 240f, conString: "@CANCEL@BACK @SELECT@SELECT");
            _modifierMenu.SetBackFunction(new UIMenuActionOpenMenu(_modifierMenu, _multiplayerMenu));
            _levelSelectMenu = new LevelSelectCompanionMenu(Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, _multiplayerMenu);
            foreach (UnlockData unlock in Unlocks.GetUnlocks(UnlockType.Modifier))
            {
                if (unlock.unlocked)
                    _modifierMenu.Add(new UIMenuItemToggle(unlock.GetShortNameForDisplay(), field: new FieldBinding(unlock, "enabled")), true);
                else
                    _modifierMenu.Add(new UIMenuItem("@TINYLOCK@LOCKED", c: Color.Red), true);
            }
            _modifierMenu.Close();
            foreach (MatchSetting matchSetting in matchSettings)
            {
                if (!(matchSetting.id == "clientlevelsenabled") && (!(matchSetting.id == "workshopmaps") || Network.available))
                {
                    _multiplayerMenu.AddMatchSetting(matchSetting, false);
                    if (matchSetting.id == "wallmode")
                        _multiplayerMenu.Add(new UIText(" ", Color.White), true);
                }
            }
            _multiplayerMenu.Add(new UIText(" ", Color.White), true);
            _multiplayerMenu.Add(new UICustomLevelMenu(new UIMenuActionOpenMenu(_multiplayerMenu, _levelSelectMenu)), true);
            _multiplayerMenu.Add(new UIModifierMenuItem(new UIMenuActionOpenMenu(_multiplayerMenu, _modifierMenu)), true);
            _multiplayerMenu.Close();
            _configGroup.Add(_multiplayerMenu, false);
            _configGroup.Add(_modifierMenu, false);
            _configGroup.Add(_levelSelectMenu, false);
            _configGroup.Close();
            Add(_configGroup);
            _playOnlineGroup = new UIComponent(Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 0f, 0f);
            _playOnlineMenu = new UIMenu("@PLANET@PLAY ONLINE@PLANET@", Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 190f, conString: "@CANCEL@BACK @SELECT@SELECT");
            _hostGameMenu = new UIMenu("@LWING@CREATE GAME@RWING@", Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 190f, conString: "@CANCEL@BACK @SELECT@SELECT");
            _hostSettingsMenu = new UIMenu("@LWING@HOST SETTINGS@RWING@", Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 190f, conString: "@CANCEL@BACK @SELECT@SELECT");
            int pMinLength = 50;
            float heightAdd = 3f;
            _playOnlineBumper = new UIMenu("PLAYING ONLINE", Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 220f, conString: "@SELECT@OK!");
            UIText component1 = new UIText("", Color.White, heightAdd: -3f)
            {
                scale = new Vec2(0.5f)
            };
            _playOnlineBumper.Add(component1, true);
            UIText component2 = new UIText("There are many tools of expression", Color.White, heightAdd: -4f)
            {
                scale = new Vec2(0.5f)
            };
            _playOnlineBumper.Add(component2, true);
            UIText component3 = new UIText("in Duck Game. Please use them for", Color.White, heightAdd: -4f)
            {
                scale = new Vec2(0.5f)
            };
            _playOnlineBumper.Add(component3, true);
            UIText component4 = new UIText("|PINK|love|WHITE| and not for |DGRED|hate...|WHITE|", Color.White, heightAdd: -4f)
            {
                scale = new Vec2(0.5f)
            };
            _playOnlineBumper.Add(component4, true);
            UIText component5 = new UIText("", Color.White, heightAdd: -3f)
            {
                scale = new Vec2(0.5f)
            };
            _playOnlineBumper.Add(component5, true);
            UIText component6 = new UIText("Things every Duck aught to remember:", Color.White, heightAdd: -4f)
            {
                scale = new Vec2(0.5f)
            };
            _playOnlineBumper.Add(component6, true);
            UIText component7 = new UIText("", Color.White, heightAdd: -3f)
            {
                scale = new Vec2(0.5f)
            };
            _playOnlineBumper.Add(component7, true);
            UIText component8 = new UIText("-Trolling and hate appear exactly the same online.".Padded(pMinLength), Colors.DGBlue, heightAdd: (-heightAdd))
            {
                scale = new Vec2(0.5f)
            };
            _playOnlineBumper.Add(component8, true);
            UIText component9 = new UIText("-Please! be kind to one another.".Padded(pMinLength), Colors.DGBlue, heightAdd: (-heightAdd))
            {
                scale = new Vec2(0.5f)
            };
            _playOnlineBumper.Add(component9, true);
            UIText component10 = new UIText("-Please! don't use hate speech or strong words.".Padded(pMinLength), Colors.DGBlue, heightAdd: (-heightAdd))
            {
                scale = new Vec2(0.5f)
            };
            _playOnlineBumper.Add(component10, true);
            UIText component11 = new UIText("-Please! don't use hacks in public lobbies.".Padded(pMinLength), Colors.DGBlue, heightAdd: (-heightAdd))
            {
                scale = new Vec2(0.5f)
            };
            _playOnlineBumper.Add(component11, true);
            UIText component12 = new UIText("-Please! keep custom content tasteful.".Padded(pMinLength), Colors.DGBlue, heightAdd: (-heightAdd))
            {
                scale = new Vec2(0.5f)
            };
            _playOnlineBumper.Add(component12, true);
            UIText component13 = new UIText("-Angle shots are neat (and are not hacks).".Padded(pMinLength), Colors.DGBlue, heightAdd: (-heightAdd))
            {
                scale = new Vec2(0.5f)
            };
            _playOnlineBumper.Add(component13, true);
            UIText component14 = new UIText("", Color.White, heightAdd: -3f)
            {
                scale = new Vec2(0.5f)
            };
            _playOnlineBumper.Add(component14, true);
            UIText component15 = new UIText("If anyone is hacking or being unkind, please", Color.White, heightAdd: -4f)
            {
                scale = new Vec2(0.5f)
            };
            _playOnlineBumper.Add(component15, true);
            UIText component16 = new UIText("hover their name in the pause menu", Color.White, heightAdd: -4f)
            {
                scale = new Vec2(0.5f)
            };
            _playOnlineBumper.Add(component16, true);
            UIText component17 = new UIText("and go 'Mute -> Block'.", Color.White, heightAdd: -4f)
            {
                scale = new Vec2(0.5f)
            };
            _playOnlineBumper.Add(component17, true);
            UIText component18 = new UIText("", Color.White, heightAdd: -3f)
            {
                scale = new Vec2(0.5f)
            };
            _playOnlineBumper.Add(component18, true);
            _playOnlineBumper.SetAcceptFunction(new UIMenuActionOpenMenu(_playOnlineBumper, _playOnlineMenu));
            _playOnlineBumper.SetBackFunction(new UIMenuActionOpenMenu(_playOnlineBumper, _playOnlineMenu));


            _browseGamesMenu = new UIServerBrowser(_playOnlineMenu, "SERVER BROWSER", Layer.HUD.camera.width, Layer.HUD.camera.height, 550f);
            if (Network.available)
            {
                _joinGameMenu = new UIMenu("@LWING@FIND GAME@RWING@", Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 190f, conString: "@CANCEL@BACK @SELECT@SELECT");
                _filtersMenu = new UIMenu("@LWING@FILTERS@RWING@", Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 190f, conString: "@SELECT@SELECT  @MENU2@TYPE");
                _filterModifierMenu = new UIMenu("@LWING@FILTER MODIFIERS@RWING@", Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 240f, conString: "@CANCEL@BACK @SELECT@SELECT");
            }
            if (Network.available)
                _matchmaker = UIMatchmakerMark2.Platform_GetMatchkmaker(null, _joinGameMenu);
            if (ModLoader.modHash != "nomods")
            {
                if (Network.available)
                    _playOnlineMenu.Add(new UIMenuItem("FIND GAME", new UIMenuActionCloseMenuCallFunction(_playOnlineMenu, new UIMenuActionCloseMenuCallFunction.Function(OpenNoModsFindGame))), true);
                _playOnlineMenu.Add(new UIMenuItem("CREATE GAME", new UIMenuActionCloseMenuCallFunction(_playOnlineMenu, new UIMenuActionCloseMenuCallFunction.Function(OpenNoModsCreateGame))), true);
            }
            else
            {
                if (Network.available)
                    _playOnlineMenu.Add(new UIMenuItem("FIND GAME", new UIMenuActionOpenMenu(_playOnlineMenu, _joinGameMenu)), true);
                _playOnlineMenu.Add(new UIMenuItem("CREATE GAME", new UIMenuActionOpenMenuCallFunction(_playOnlineMenu, _hostGameMenu, new UIMenuActionOpenMenuCallFunction.Function(OpenHostGameMenuNonMini))), true);
            }
            _playOnlineMenu.Add(new UIMenuItem("BROWSE GAMES", new UIMenuActionOpenMenu(_playOnlineMenu, _browseGamesMenu)), true);
            _playOnlineMenu.SetBackFunction(new UIMenuActionCloseMenuCallFunction(_playOnlineGroup, new UIMenuActionCloseMenuCallFunction.Function(ClosedOnline)));
            _playOnlineMenu.Close();
            _playOnlineGroup.Add(_playOnlineMenu, false);
            _playOnlineBumper.Close();
            _playOnlineGroup.Add(_playOnlineBumper, false);
            string str = "";
            bool flag = false;
            foreach (MatchSetting onlineSetting in onlineSettings)
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
                            UIComponent uiComponent = _hostSettingsMenu.AddMatchSetting(onlineSetting, false);
                            if (uiComponent != null && uiComponent is UIMenuItemString && uiComponent is UIMenuItemString uiMenuItemString)
                                uiMenuItemString.InitializeEntryMenu(_playOnlineGroup, _hostSettingsMenu);
                        }
                        else
                        {
                            UIComponent uiComponent = _hostGameMenu.AddMatchSetting(onlineSetting, false);
                            if (uiComponent != null && uiComponent is UIMenuItemString && uiComponent is UIMenuItemString uiMenuItemString)
                                uiMenuItemString.InitializeEntryMenu(_playOnlineGroup, _hostGameMenu);
                        }
                        str = onlineSetting.id;
                    }
                }
            }
            _hostGameMenu.Add(new UIText(" ", Color.White), true);
            BuildHostMatchSettingsMenu();
            _hostGameMenu.Add(new UIMenuItem("|DGBLUE|SETTINGS", new UIMenuActionOpenMenu(_hostGameMenu, _hostSettingsMenu)), true);
            _hostSettingsMenu.Add(new UIMenuItem("|DGBLUE|MATCH SETTINGS", new UIMenuActionOpenMenuCallFunction(_hostSettingsMenu, _hostMatchSettingsMenu, new UIMenuActionOpenMenuCallFunction.Function(SetMatchSettingsOpenedFromHostGame))), true);
            _hostSettingsMenu.SetBackFunction(new UIMenuActionOpenMenu(_hostSettingsMenu, _hostGameMenu));
            _hostGameMenu.Add(new UIMenuItem("|DGGREEN|CREATE GAME", new UIMenuActionCloseMenuCallFunction(_playOnlineGroup, new UIMenuActionCloseMenuCallFunction.Function(CreateGame))), true);
            _hostGameMenu.SetBackFunction(new UIMenuActionOpenMenu(_hostGameMenu, _playOnlineMenu));
            _hostGameMenu.Close();
            _browseGamesMenu.Close();
            _playOnlineGroup.Add(_browseGamesMenu, false);
            _playOnlineGroup.Add(_browseGamesMenu._passwordEntryMenu, false);
            _playOnlineGroup.Add(_browseGamesMenu._portEntryMenu, false);
            _playOnlineGroup.Add(_hostGameMenu, false);
            _hostSettingsMenu.Close();
            _playOnlineGroup.Add(_hostSettingsMenu, false);
            if (Network.available)
            {
                foreach (MatchSetting onlineSetting in onlineSettings)
                {
                    //if (!onlineSetting.createOnly && (!(onlineSetting.id == "customlevelsenabled") || !ParentalControls.AreParentalControlsActive()))
                    //    this._joinGameMenu.AddMatchSetting(onlineSetting, true);
                    if (!onlineSetting.createOnly)
                    {
                        _joinGameMenu.AddMatchSetting(onlineSetting, true, true);
                    }
                }
                _joinGameMenu.Add(new UIText(" ", Color.White), true);
                _joinGameMenu.Add(new UIMenuItemNumber("Ping", field: new FieldBinding(typeof(UIMatchmakerMark2), "searchMode", 0f, 2f, 0.1f), valStrings: new List<string>()
        {
          "|DGYELLO|PREFER GOOD",
          "|DGGREEN|GOOD PING",
          "|DGREDDD|ANY PING"
        }), true);
                _joinGameMenu.Add(new UIText(" ", Color.White), true);
                _joinGameMenu.Add(new UIMenuItem("|DGGREEN|FIND GAME", new UIMenuActionOpenMenu(_joinGameMenu, _matchmaker)), true);
                _joinGameMenu.AssignDefaultSelection();
                _joinGameMenu.SetBackFunction(new UIMenuActionOpenMenu(_joinGameMenu, _playOnlineMenu));
                _joinGameMenu.Close();
                _playOnlineGroup.Add(_joinGameMenu, false);
                foreach (MatchSetting matchSetting in matchSettings)
                {
                    if (!(matchSetting.id == "workshopmaps") || Network.available)
                        _filtersMenu.AddMatchSetting(matchSetting, true);
                }
                _filtersMenu.Add(new UIText(" ", Color.White), true);
                _filtersMenu.Add(new UIModifierMenuItem(new UIMenuActionOpenMenu(_filtersMenu, _filterModifierMenu)), true);
                _filtersMenu.Add(new UIText(" ", Color.White), true);
                _filtersMenu.Add(new UIMenuItem("|DGBLUE|CLEAR FILTERS", new UIMenuActionCallFunction(new UIMenuActionCallFunction.Function(ClearFilters))), true);
                _filtersMenu.Add(new UIMenuItem("BACK", new UIMenuActionOpenMenu(_filtersMenu, _joinGameMenu), backButton: true), true);
                _filtersMenu.Close();
                _playOnlineGroup.Add(_filtersMenu, false);
                foreach (UnlockData unlock in Unlocks.GetUnlocks(UnlockType.Modifier))
                    _filterModifierMenu.Add(new UIMenuItemToggle(unlock.GetShortNameForDisplay(), field: new FieldBinding(unlock, "enabled"), filterBinding: new FieldBinding(unlock, "filtered")), true);
                _filterModifierMenu.Add(new UIText(" ", Color.White), true);
                _filterModifierMenu.Add(new UIMenuItem("BACK", new UIMenuActionOpenMenu(_filterModifierMenu, _filtersMenu), backButton: true), true);
                _filterModifierMenu.Close();
                _playOnlineGroup.Add(_filterModifierMenu, false);
                _matchmaker.Close();
                _playOnlineGroup.Add(_matchmaker, false);
            }
            _playOnlineGroup.Close();
            Add(_playOnlineGroup);
            Graphics.fade = 0f;
            Layer l = new Layer("HUD2", -85, new Camera());
            l.camera.width /= 2f;
            l.camera.height /= 2f;
            Layer.Add(l);
            Layer hud = Layer.HUD;
            Layer.HUD = l;
            Layer.HUD = hud;
            if (!DuckNetwork.isDedicatedServer && !DuckNetwork.ShowUserXPGain() && Unlockables.HasPendingUnlocks())
                MonoMain.pauseMenu = new UIUnlockBox(Unlockables.GetPendingUnlocks().ToList(), Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 190f);
            core.endedGameInProgress = false;

            
        }

        public void OpenPauseMenu(ProfileBox2 pProfile)
        {
            _pauseMenuProfile = pProfile;
            BuildPauseMenu();
            if (Network.isActive && pProfile.profile != DuckNetwork.localProfile)
            {
                _localPauseGroup.DoUpdate();
                _localPauseMenu.DoUpdate();
                _localPauseGroup.Open();
                _localPauseMenu.Open();
                MonoMain.pauseMenu = _localPauseGroup;
            }
            else
            {
                _pauseGroup.DoUpdate();
                _pauseMenu.DoUpdate();
                _pauseGroup.Open();
                _pauseMenu.Open();
                SFX.Play("pause", 0.6f);
                MonoMain.pauseMenu = _pauseGroup;
            }
        }

        public override void NetworkDebuggerPrepare() => PrepareForOnline();

        public static void HostGameInviteLink()
        {
            if (!Network.isActive)
            {
                FillMatchmakingProfiles();
                DuckNetwork.Host(8, NetworkLobbyType.Private);
                (current as TeamSelect2).PrepareForOnline();
                _didHost = true;
                DevConsole.Log(DCSection.Connection, "Hosting Server via Invite Link!");
            }
            else
                DevConsole.Log(DCSection.Connection, "Copied Invite Link!");
            Main.SpecialCode = "Copied Invite Link.";
            _copyInviteLink = true;
        }

        public static void DoInvite()
        {
            if (!Network.isActive)
            {
                FillMatchmakingProfiles();
                DuckNetwork.Host(8, NetworkLobbyType.Private);
                (current as TeamSelect2).PrepareForOnline();
                _didHost = true;
            }
            _attemptingToInvite = true;
            _copyInviteLink = false;
        }

        public static void InvitedFriend(User u)
        {
            if (!Network.inLobby || u == null)
                return;
            _invitedUsers.Add(u);
            DuckNetwork.core._invitedFriends.Add(u.id);
            DoInvite();
            Main.SpecialCode = "Invited Friend (" + u.id.ToString() + ")";
            DevConsole.Log(DCSection.Connection, Main.SpecialCode);
        }

        public static bool CheckForCTeams(Profile p2)
        {
            try
            {
                foreach (Profile p in Profiles.activeNonSpectators)
                {
                    if (p != p2)
                    {
                        if (p.team.name == p2.team.name && !p.team.defaultTeam && p.team != p2.team)
                        {
                            p2.team = p.team;
                            DuckNetwork.SendToEveryone(new NMSetTeam(p2, p.team, true));
                            return true;
                        }
                    }
                }
            }
            catch { }
            return false;
        }
        public override void Update()
        {
            if (Keyboard.Pressed(Keys.F6)) Options.ReloadHats();
            if (MonoMain.pauseMenu == null && Options.Data.showControllerWarning && Input.mightHavePlaystationController && !_showedPS4Warning)
            {
                _showedPS4Warning = true;
                MonoMain.pauseMenu = Options.controllerWarning;
                Options.controllerWarning.Open();
            }
            if (core.endedGameInProgress && !DuckNetwork.isDedicatedServer)
            {
                _waitToShow -= Maths.IncFrameTimer();
                if (_waitToShow <= 0 && MonoMain.pauseMenu == null)
                {
                    if (!DuckNetwork.ShowUserXPGain() && Unlockables.HasPendingUnlocks())
                        MonoMain.pauseMenu = new UIUnlockBox(Unlockables.GetPendingUnlocks().ToList(), Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 190f);
                    core.endedGameInProgress = false;
                }
            }
            backgroundColor = Color.Black;
            if (_copyInviteLink && Steam.user != null && Steam.lobby != null && Steam.lobby.id != 0UL)
            {
                DuckNetwork.CopyInviteLink();
                _copyInviteLink = false;
            }
            bool flag1 = false;
            if (Network.isActive)
            {
                int num = 0;
                foreach (Profile profile in DuckNetwork.profiles)
                {
                    if (profile.slotType != SlotType.Closed && profile.slotType != SlotType.Spectator && (profile.slotType != SlotType.Invite || profile.connection != null || explicitlyCreated) && num > 3)
                        flag1 = true;
                    ++num;
                }

                Lobby lobby = Steam.lobby;

                if (Network.isServer && lobby != null)
                {
                    List<Profile> profiles = Profiles.active;

                    if (!Enumerable.SequenceEqual(_lastKnownProfiles, profiles))
                    {
                        StringBuilder builder = new StringBuilder();

                        foreach (Profile profile in profiles)
                        {
                            string name = profile.name.Replace("\n", "_");

                            builder.Append(name);
                            builder.Append("\n");
                        }

                        lobby.SetLobbyData("players", builder.ToString());
                    }
                }
            }
            eightPlayersActive = Profiles.activeNonSpectators.Count > 4;
            zoomedOut = false;
            if (((growCamera ? 1 : (UISlotEditor.editingSlots ? 1 : 0)) | (flag1 ? 1 : 0)) != 0 || eightPlayersActive || showEightPlayerSelected)
            {
                if (oldCameraSize == Vec2.Zero)
                {
                    oldCameraSize = current.camera.size;
                    oldCameraPos = current.camera.position;
                }
                float x = 500f;
                current.camera.size = Lerp.Vec2Smooth(current.camera.size, new Vec2(x, x / 1.77777f), 0.1f, 0.08f);
                current.camera.position = Lerp.Vec2Smooth(current.camera.position, new Vec2(-1f, -7f), 0.1f, 0.08f);
                eightPlayersActive = true;
                zoomedOut = true;
            }
            else if (oldCameraSize != Vec2.Zero)
            {
                current.camera.size = Lerp.Vec2Smooth(current.camera.size, oldCameraSize, 0.1f, 0.08f);
                current.camera.position = Lerp.Vec2Smooth(current.camera.position, oldCameraPos, 0.1f, 0.08f);
            }
            growCamera = false;
            if (_findGame.value)
            {
                _findGame.value = false;
                int num = Network.isActive ? 1 : 0;
            }
            if (Program.testServer && !didcreatelanlobby)
            {
                didcreatelanlobby = true;
                _createGame.value = true;
                _hostGame.value = true;
                GetOnlineSetting("type").value = 3;
            }
            if (_createGame.value || _hostGame.value)
            {
                explicitlyCreated = _createGame.value;
                if (!Network.available)
                    GetOnlineSetting("type").value = 3;
                if ((string)GetOnlineSetting("name").value == "")
                    GetOnlineSetting("name").value = DefaultGameName();
                DuckNetwork.ChangeSlotSettings();
                if (_hostGame.value)
                    FillMatchmakingProfiles();
                bool flag2 = (bool)GetOnlineSetting("dedicated").value;
                foreach (MatchmakingPlayer matchmakingProfile in UIMatchmakingBox.core.matchmakingProfiles)
                    matchmakingProfile.spectator = flag2;
                DuckNetwork.Host(GetSettingInt("maxplayers"), (NetworkLobbyType)GetSettingInt("type"), true);
                PrepareForOnline();
                if (_hostGame.value)
                    _beam.ClearBeam();
                DevConsole.Log(DCSection.Connection, "Hosting Game(" + UIMatchmakingBox.core.matchmakingProfiles.Count.ToString() + ", " + ((NetworkLobbyType)GetSettingInt("type")).ToString() + ")");
                _createGame.value = false;
                _hostGame.value = false;
            }
            if (_inviteFriends.value || _invitedUsers.Count > 0)
            {
                _inviteFriends.value = false;
                if (!Network.isActive)
                {
                    FillMatchmakingProfiles();
                    DuckNetwork.Host(4, NetworkLobbyType.Private);
                    PrepareForOnline();
                }
                _attemptingToInvite = true;
            }
            if (_attemptingToInvite && Network.isActive && (!_didHost || Steam.lobby != null && !Steam.lobby.processing))
            {
                foreach (User invitedUser in _invitedUsers)
                    Steam.InviteUser(invitedUser, Steam.lobby);
                _invitedUsers.Clear();
                _attemptingToInvite = false;
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
                    foreach (ProfileBox2 profile in _profiles)
                    {
                        if (profile.profile != null && (profile.playerActive || profile.profile.connection != null) && profile.profile.inputProfile.genericController == defaultProfile.genericController)
                        {
                            flag3 = false;
                            break;
                        }
                    }
                    if (flag3 && defaultProfile.Pressed(Triggers.Start))
                    {
                        foreach (ProfileBox2 profile in _profiles)
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
            if (_levelSelector != null)
            {
                if (_levelSelector.isClosed)
                {
                    _levelSelector.Terminate();
                    _levelSelector = null;
                    Layer.skipDrawing = false;
                    _beam.active = true;
                    _beam.visible = true;
                    Editor.selectingLevel = false;
                }
                else
                {
                    _levelSelector.Update();
                    _beam.active = false;
                    _beam.visible = false;
                    Editor.selectingLevel = true;
                    return;
                }
            }
            if (openLevelSelect)
            {
                Graphics.fade = Lerp.Float(Graphics.fade, 0f, 0.04f);
                if (Graphics.fade >= 0.01f)
                    return;
                _levelSelector = new LevelSelect(returnLevel: this);
                _levelSelector.Initialize();
                openLevelSelect = false;
                Layer.skipDrawing = true;
            }
            else
            {
                int num1 = 0;
                activePlayers = 0;
                foreach (ProfileBox2 profile in _profiles)
                {
                    if (profile.ready && profile.profile != null)
                        ++num1;
                    if ((profile.playerActive || profile.duck != null && !profile.duck.dead && !profile.duck.removeFromLevel) && profile.profile != null)
                        ++activePlayers;
                }
                _beam.active = !menuOpen;
                if (!menuOpen)
                    HUD.CloseAllCorners();
                if (_backOut.value)
                {
                    if (Network.isActive)
                    {
                        current = new DisconnectFromGame();
                    }
                    else
                    {
                        _backOut.value = false;
                        if (_pauseMenuProfile != null)
                        {
                            if (_beam != null)
                                _beam.RemoveDuck(_pauseMenuProfile.duck);
                            _pauseMenuProfile.CloseDoor();
                            _pauseMenuProfile = null;
                        }
                    }
                }
                if (Graphics.fade <= 0 && _returnToMenu.value)
                    current = new TitleScreen();
                if (!Network.isActive)
                    DuckNetwork.core.startCountdown = _starting;
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
                    activePlayers = 2;
                    num2 = 0;
                }
                if (activePlayers == num1 && !_returnToMenu.value && (!Network.isActive && num1 > 0 || Network.isActive && num1 > num2))
                {
                    _singlePlayer = num1 == 1;
                    if (DuckNetwork.core.startCountdown)
                    {
                        DuckNetwork.inGame = true;
                        dim = Maths.LerpTowards(dim, 0.8f, 0.02f);
                        _countTime -= 0.006666667f;
                        if (_countTime <= 0 && Network.isServer && (Graphics.fade <= 0 || NetworkDebugger.enabled))
                        {
                            UpdateModifierStatus();
                            DevConsole.qwopMode = Enabled("QWOPPY", true);
                            DevConsole.splitScreen = Enabled("SPLATSCR", true);
                            DevConsole.rhythmMode = Enabled("RHYM", true);
                            DuckNetwork.SetMatchSettings(true, GetSettingInt("requiredwins"), GetSettingInt("restsevery"), (bool)GetOnlineSetting("teams").value, GetSettingBool("wallmode"), GetSettingInt("normalmaps"), GetSettingInt("randommaps"), GetSettingInt("workshopmaps"), GetSettingInt("custommaps"), Editor.activatedLevels.Count, GetNetworkModifierList(), GetSettingBool("clientlevelsenabled"));
                            partyMode = GetSettingBool("partymode");
                            if (Network.isActive && Network.isServer)
                            {
                                foreach (Profile profile in DuckNetwork.profiles)
                                {
                                    if (profile.connection == null && profile.slotType != SlotType.Reserved && profile.slotType != SlotType.Spectator)
                                        profile.slotType = SlotType.Closed;
                                }
                            }
                            if (Network.isActive)
                                SendMatchSettings();
                            if (!core.gameInProgress)
                                Main.ResetMatchStuff();
                            Music.Stop();
                            MonoMain.FinishLazyLoad();
                            if (_singlePlayer)
                            {
                                current.Clear();
                                current = new ArcadeLevel(Content.GetLevelID("arcade"));
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
                                Level level = !ctfMode ? new GameLevel(Deathmatch.RandomLevelString()) : (Level)new CTFLevel(Deathmatch.RandomLevelString(folder: "ctf"));
                                _spectatorCountdownStop = false;
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
                                sendCustomLevels = true;
                                current = level;
                                return;
                            }
                        }
                    }
                    else
                    {
                        DuckNetwork.inGame = false;
                        dim = Maths.LerpTowards(dim, 0f, 0.1f);
                        if (dim < 0.05f)
                            _countTime = 1.5f;
                    }
                    _matchSetup = true;
                    if (Network.isServer)
                    {
                        if (!Network.isActive)
                        {
                            if (!_singlePlayer && !_starting && !menuOpen && Input.Pressed(Triggers.Menu1))
                            {
                                _configGroup.Open();
                                _multiplayerMenu.Open();
                                MonoMain.pauseMenu = _configGroup;
                            }
                            if (!_starting && !menuOpen && Input.Pressed(Triggers.Menu2))
                                PlayOnlineSinglePlayer();
                        }
                        if (!menuOpen && Input.Pressed(Triggers.Select) && (!_singlePlayer || Profiles.active.Count > 0 && !Profiles.IsDefault(Profiles.active[0])) || DuckNetwork.isDedicatedServer && !_sentDedicatedCountdown && !_spectatorCountdownStop)
                        {
                            if (Network.isActive)
                            {
                                Send.Message(new NMBeginCountdown());
                                _sentDedicatedCountdown = true;
                                _spectatorCountdownStop = false;
                            }
                            else
                                _starting = true;
                        }
                        if (Network.isActive && DuckNetwork.isDedicatedServer && !_spectatorCountdownStop && Input.Pressed(Triggers.Cancel))
                        {
                            _spectatorCountdownStop = true;
                            _sentDedicatedCountdown = false;
                            _starting = false;
                            DuckNetwork.core.startCountdown = false;
                            Send.Message(new NMCancelCountdown());
                        }
                    }
                }
                else
                {
                    dim = Maths.LerpTowards(dim, 0f, 0.1f);
                    if (dim < 0.05f)
                        _countTime = 1.5f;
                    _matchSetup = false;
                    _starting = false;
                    DuckNetwork.core.startCountdown = false;
                    DuckNetwork.inGame = false;
                    _sentDedicatedCountdown = false;
                    _spectatorCountdownStop = false;
                }
                base.Update();
                if (Network.isActive)
                {
                    _afkTimeout += Maths.IncFrameTimer();
                    foreach (Profile profile in DuckNetwork.profiles)
                    {
                        if (profile.localPlayer && profile.inputProfile != null && profile.inputProfile.Pressed(Triggers.Any, true))
                            _afkTimeout = 0f;
                    }
                    if (DuckNetwork.lobbyType == DuckNetwork.LobbyType.FriendsOnly || DuckNetwork.lobbyType == DuckNetwork.LobbyType.Private)
                        _afkTimeout = 0f;
                    if (_afkTimeout > _afkShowTimeout && (int)_afkTimeout != _timeoutBeep)
                    {
                        _timeoutBeep = (int)_afkTimeout;
                        SFX.Play("cameraBeep");
                    }
                    if (_afkTimeout > _afkMaxTimeout)
                        current = new DisconnectFromGame();
                }
                else _afkTimeout = 0f;
                Graphics.fade = Lerp.Float(Graphics.fade, _returnToMenu.value || _countTime <= 0f ? 0f : 1f, 0.02f);
                _setupFade = Lerp.Float(_setupFade, !_matchSetup || menuOpen || DuckNetwork.core.startCountdown ? 0f : 1f, 0.05f);
                Layer.Game.fade = Lerp.Float(Layer.Game.fade, _matchSetup ? 0.5f : 1f, 0.05f);
            }
        }

        private void PlayOnlineSinglePlayer()
        {
            DefaultSettings(false);
            PlayOnlineSinglePlayerAfterOnline();
        }

        private void PlayOnlineSinglePlayerAfterOnline()
        {
            FillMatchmakingProfiles();
            _playOnlineGroup.Open();
            if (!DGRSettings.skipOnlineBumper)
            {
                DGRSettings.skipOnlineBumper = true;
                _playOnlineBumper.Open();
            }
            else _playOnlineMenu.Open();
            MonoMain.pauseMenu = _playOnlineGroup;
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
                if (current is TeamSelect2)
                    (current as TeamSelect2).ClearTeam(index);
            }
            Profile profile1 = Profiles.active.FirstOrDefault(x => x == Profiles.experienceProfile);
            Profile profile2 = null;
            if (profile1 == null)
            {
                profile2 = Profiles.active[0];
                profile1 = Profiles.experienceProfile;
            }
            UIMatchmakingBox.core.matchmakingProfiles.Clear();
            foreach (Profile profile3 in Profiles.active.ToList())
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
            if (_levelSelector != null)
            {
                if (!_levelSelector.isInitialized)
                    return;
                _levelSelector.PostDrawLayer(layer);
            }
            else
            {
                if (layer == Layer.Game && UISlotEditor.editingSlots)
                {
                    foreach (ProfileBox2 profileBox2 in current.things[typeof(ProfileBox2)])
                    {
                        if (UISlotEditor._slot == profileBox2.controllerIndex)
                            Graphics.DrawRect(profileBox2.position, profileBox2.position + new Vec2(141f, 89f), Color.White, (Depth)0.95f, false);
                        else
                            Graphics.DrawRect(profileBox2.position, profileBox2.position + new Vec2(141f, 89f), Color.Black * 0.5f, (Depth)0.95f);
                    }
                    foreach (BlankDoor blankDoor in current.things[typeof(BlankDoor)])
                        Graphics.DrawRect(blankDoor.position, blankDoor.position + new Vec2(141f, 89f), Color.Black * 0.5f, (Depth)0.95f);
                }
                Layer background = Layer.Background;
                if (layer == Layer.HUD)
                {
                    if (_afkTimeout >= _afkShowTimeout)
                    {
                        _timeoutFade = Lerp.Float(_timeoutFade, 1f, 0.05f);
                        Graphics.DrawRect(new Vec2(-1000f, -1000f), new Vec2(10000f, 10000f), Color.Black * 0.7f * _timeoutFade, (Depth)0.95f);
                        string text1 = "AFK TIMEOUT IN";
                        string text2 = ((int)(_afkMaxTimeout - _afkTimeout)).ToString();
                        Graphics.DrawString(text1, new Vec2((float)(layer.width / 2f - Graphics.GetStringWidth(text1) / 2f), (float)(layer.height / 2f - 8f)), Color.White * _timeoutFade, (Depth)0.96f);
                        Graphics.DrawString(text2, new Vec2(layer.width / 2f - Graphics.GetStringWidth(text2), (float)(layer.height / 2f + 4f)), Color.White * _timeoutFade, (Depth)0.96f, scale: 2f);
                    }
                    else
                        _timeoutFade = Lerp.Float(_timeoutFade, 0f, 0.05f);
                    foreach (Profile profile in DuckNetwork.profiles)
                    {
                        if (profile.reservedUser != null)
                        {
                            if (profile.slotType == SlotType.Reserved)
                                break;
                        }
                    }
                    if (core.gameInProgress)
                    {
                        Vec2 vec2 = new Vec2(0f, Layer.HUD.barSize);
                        Graphics.DrawRect(new Vec2(0f, vec2.y), new Vec2(320f, vec2.y + 10f), Color.Black, (Depth)0.9f);
                        _littleFont.depth = (Depth)0.95f;
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
                        _topScroll += num1;
                        if (_topScroll > 1)
                            --_topScroll;
                        if (_topScroll < 0)
                            ++_topScroll;
                        _littleFont.Draw(text4, new Vec2((float)(1f - _topScroll * (_littleFont.GetWidth(text3) + 7f)), vec2.y + 3f), Color.White, (Depth)0.95f);
                    }
                    if (_setupFade > 0.01f)
                    {
                        float num = (float)(Layer.HUD.camera.height / 2f - 28f);
                        string str1 = "@MENU2@PLAY ONLINE";
                        if (!Network.available)
                        {
                            str1 = "@MENU2@PLAY LAN (NO STEAM)";
                            if (Steam.user != null && Steam.user.state == SteamUserState.Offline)
                                str1 = "@MENU2@PLAY LAN (STEAM OFFLINE MODE)";
                        }
                        else if (Profiles.active.Count > 3)
                            str1 = "|GRAY|ONLINE UNAVAILABLE (FULL GAME)";
                        if (_singlePlayer)
                        {
                            string str2 = "@SELECT@CHALLENGE ARCADE";
                            if (Profiles.active.Count == 0 || Profiles.IsDefault(Profiles.active[0]))
                                str2 = "|GRAY|NO ARCADE (SELECT A PROFILE)";
                            if (Network.available)
                            {
                                string text5 = str2;
                                _font.alpha = _setupFade;
                                _font.Draw(text5, (float)(Layer.HUD.width / 2f - _font.GetWidth(text5) / 2f), num + 15f, Color.White, (Depth)0.81f);
                                string text6 = str1;
                                _font.alpha = _setupFade;
                                _font.Draw(text6, (float)(Layer.HUD.width / 2f - _font.GetWidth(text6) / 2f), (float)(num + 12f + 17f), Color.White, (Depth)0.81f);
                            }
                            else
                            {
                                string text7 = str2;
                                _font.alpha = _setupFade;
                                _font.Draw(text7, (float)(Layer.HUD.width / 2f - _font.GetWidth(text7) / 2f), num + 15f, Color.White, (Depth)0.81f);
                                string text8 = str1;
                                _font.alpha = _setupFade;
                                _font.Draw(text8, (float)(Layer.HUD.width / 2f - _font.GetWidth(text8) / 2f), (float)(num + 12f + 17f), Color.White, (Depth)0.81f);
                            }
                        }
                        else
                        {
                            _font.alpha = _setupFade;
                            if (Network.isClient)
                            {
                                string text = "WAITING FOR HOST TO START";
                                if (core.gameInProgress)
                                    text = "WAITING FOR HOST TO RESUME";
                                _font.Draw(text, (float)(Layer.HUD.width / 2f - _font.GetWidth(text) / 2f), num + 22f, Color.White, (Depth)0.81f);
                            }
                            else if (!Network.isActive)
                            {
                                string text9 = "@SELECT@START MATCH";
                                _font.Draw(text9, (float)(Layer.HUD.width / 2f - _font.GetWidth(text9) / 2f), num + 9f, Color.White, (Depth)0.81f);
                                string text10 = "@MENU1@MATCH SETTINGS";
                                _font.Draw(text10, (float)(Layer.HUD.width / 2f - _font.GetWidth(text10) / 2f), num + 22f, Color.White, (Depth)0.81f);
                                string text11 = str1;
                                _font.Draw(text11, (float)(Layer.HUD.width / 2f - _font.GetWidth(text11) / 2f), num + 35f, Color.White, (Depth)0.81f);
                            }
                            else
                            {
                                string text = "@SELECT@START MATCH";
                                if (core.gameInProgress)
                                    text = "@SELECT@RESUME MATCH";
                                _font.Draw(text, (float)(Layer.HUD.width / 2f - _font.GetWidth(text) / 2f), num + 22f, Color.White, (Depth)0.81f);
                            }
                        }
                        _countdownScreen.alpha = _setupFade;
                        _countdownScreen.depth = (Depth)0.8f;
                        _countdownScreen.centery = _countdownScreen.height / 2;
                        Graphics.Draw(_countdownScreen, Layer.HUD.camera.x, Layer.HUD.camera.height / 2f);
                    }
                    if (dim > 0.01f)
                    {
                        _countdownScreen.alpha = 1f;
                        _countdownScreen.depth = (Depth)0.8f;
                        _countdownScreen.centery = _countdownScreen.height / 2;
                        Graphics.Draw(_countdownScreen, Layer.HUD.camera.x, Layer.HUD.camera.height / 2f);
                        _countdown.alpha = dim * 1.2f;
                        _countdown.depth = (Depth)0.81f;
                        _countdown.frame = (int)(float)Math.Ceiling((1f - _countTime) * 2f);
                        _countdown.centery = _countdown.height / 2;
                        if (DuckNetwork.isDedicatedServer)
                        {
                            Graphics.Draw(_countdown, 160f, (float)(Layer.HUD.camera.height / 2f - 8f));
                            string text = "@CANCEL@STOP COUNTDOWN";
                            _font.alpha = dim * 1.2f;
                            _font.Draw(text, (float)(Layer.HUD.width / 2f - _font.GetWidth(text) / 2f), (float)(Layer.HUD.camera.height / 2f + 8f), Color.White, (Depth)0.81f);
                        }
                        else
                            Graphics.Draw(_countdown, 160f, (float)(Layer.HUD.camera.height / 2f - 3f));
                    }
                }
                base.PostDrawLayer(layer);
            }
        }

        public HatSelector GetHatSelector(int index) => _profiles[index]._hatSelector;

        public override void OnMessage(NetMessage m)
        {
        }

        public override void OnNetworkConnected(Profile p)
        {
        }

        public override void OnNetworkConnecting(Profile p)
        {
            if (p.networkIndex < _profiles.Count)
            {
                _profiles[p.networkIndex].Despawn();
                _profiles[p.networkIndex].PrepareDoor();
            }
            else
                DevConsole.Log(DCSection.Connection, "@error@|DGRED|TeamSelect2.OnNetworkConnecting out of range(" + p.networkIndex.ToString() + "," + p.slotType.ToString() + ")");
        }

        public override void OnNetworkDisconnected(Profile p)
        {
            if (UIMatchmakerMark2.instance != null || p.networkIndex >= _profiles.Count)
                return;
            _profiles[p.networkIndex].Despawn();
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
