using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using static DuckGame.UIServerBrowser;

namespace DuckGame
{
    public class UIServerBrowser : UIMenu
    {
        private Queue<SearchMode> _modeQueue = new Queue<SearchMode>();
        private UIMenu _openOnClose;
        private Sprite _moreArrow;
        private Sprite _noImage;
        private Sprite _steamIcon;
        private Sprite _lanIcon;
        private Sprite _lockedServer;
        private Sprite _namedServer;
        private Sprite _globeIcon;
        private SpriteMap _cursor;
        private SpriteMap _localIcon;
        private SpriteMap _newIcon;
        private int _hoverIndex;
        private UIBox _box;
        private FancyBitmapFont _fancyFont;
        private int _maxLobbiesToShow;
        public UIMenu _editModMenu;
        public UIMenu _yesNoMenu;
        //private UIMenuItem _yesNoYes;
        //private UIMenuItem _yesNoNo;
        //private Textbox _updateTextBox;
        private int _pressWait;
        public UIMatchmakerMark2 _attemptConnection;
        public static bool _doLanSearch;
        public string enteredPassword = "";
        public string enteredPort = "";
        public int lanSearchPort = 1337;
        public UIStringEntryMenu _passwordEntryMenu;
        public UIStringEntryMenu _portEntryMenu;
        private Tex2D defaultImage;
        private Tex2D defaultImageLan;
        private UIMenu _downloadModsMenu;
        public UIModManagement _modsInfoMenu;
        private bool _enteringPort;
        private long _lobbySearchCooldownNextAvailable;
        //private const long MIN_FRAMES_BETWEEN_SEARCHES = 0;
        private float _refreshingDots;
        private bool _showingMenu;
        private bool _draggingScrollbar;
        private Vec2 _oldPos;
        public static LobbyData _selectedLobby;
        private static LobbyData _joiningLobby;
        private bool _searching;
        private List<LobbyData> _lobbies = new List<LobbyData>();
        private LobbyData _passwordLobby;
        private bool fixView = true;
        //private const int boxHeight = 36;
        //private const int scrollWidth = 12;
        //private const int boxSideMargin = 14;
        //private const int scrollBarHeight = 32;
        private int scrollBarTop;
        private int scrollBarBottom;
        private int scrollBarScrollableHeight;
        private int scrollBarOffset;
        private int _scrollItemOffset;
        private bool _gamepadMode = true;
        private static Dictionary<ulong, Tex2D> _previewMap = new Dictionary<ulong, Tex2D>();
        private static Dictionary<object, ulong> _clientMap = new Dictionary<object, ulong>();

        private readonly FancyBitmapFont _smallFont = new FancyBitmapFont("smallFont")
        {
            scale = new Vec2(0.8f),
            maxWidth = 55,
            singleLine = true
        };

        private readonly Sprite[] _mapSprites;
        private readonly Func<Lobby, int>[] _percentageFunctions;

        private readonly Vec2 _mapsOffset = new Vec2(410f, -0.5f);
        private readonly Vec2 _namesOffset = new Vec2(277f, 1f);

        private readonly int _nicknameRows = 4;

        public SearchMode mode => _modeQueue.Count > 0 ? _modeQueue.Peek() : SearchMode.None;

        public override void Close()
        {
            if (!fixView)
            {
                _showingMenu = false;
                _editModMenu.Close();
                Layer.HUD.camera.width /= 2f;
                Layer.HUD.camera.height /= 2f;
                fixView = true;
                DevConsole.RestoreDevConsole();
            }
            base.Close();
        }

        //private void ShowYesNo(UIMenu goBackTo, UIMenuActionCallFunction.Function onYes)
        //{
        //    this._yesNoNo.menuAction = (UIMenuAction)new UIMenuActionCallFunction((UIMenuActionCallFunction.Function)(() =>
        //  {
        //      this._yesNoMenu.Close();
        //      goBackTo.Open();
        //  }));
        //    this._yesNoYes.menuAction = (UIMenuAction)new UIMenuActionCallFunction(onYes);
        //    new UIMenuActionOpenMenu((UIComponent)this._editModMenu, (UIComponent)this._yesNoMenu).Activate();
        //}
        
        //maybe later -NiK0
        public static void DownloadRequiredMods()
        {
            string modList = "";
            if (ConnectionError.joinLobby != null)
            {
                _joiningLobby = new LobbyData();
                _joiningLobby.lobby = ConnectionError.joinLobby;
            }

            if (_joiningLobby != null)
            {
                string loadedMods = _joiningLobby.lobby.GetLobbyData("mods");
                if (loadedMods != null && loadedMods != "")
                    modList = loadedMods;

                Program.commandLine += " -downloadmods -tempMods " + modList + " +connect_lobby " + _joiningLobby.lobby.id;
                Program.commandLine = Program.commandLine.Replace("-nomods", "");
                ModLoader.RestartToVanillaDg = false;
                if (MonoMain.lobbyPassword != "")
                    Program.commandLine += " +password " + MonoMain.lobbyPassword;
                ModLoader.RestartGame();
            }
        }

        public static void SubscribeAndRestart()
        {
            foreach (Mod mod in ModLoader.allMods)
            {
                if (mod.clientMod || mod.configuration.modType is ModConfiguration.Type.Reskin or ModConfiguration.Type.MapPack)
                    continue;
                
                mod.configuration.disabled = true;
            }

            if (ConnectionError.joinLobby != null)
            {
                _joiningLobby = new LobbyData
                {
                    lobby = ConnectionError.joinLobby
                };
                string lobbyData = ConnectionError.joinLobby.GetLobbyData("mods");
                if (lobbyData != null && lobbyData != "")
                {
                    lobbyData = lobbyData.Replace("|3132351890,0", ""); //dumb but works -NiK0
                    lobbyData = lobbyData.Replace("3132351890,0", "");
                    string str1 = lobbyData;
                    char[] chArray = new char[1] { '|' };
                    foreach (string str2 in str1.Split(chArray))
                    {
                        if (str2 != null && !str2.Contains("LOCAL"))
                        {
                            string[] strArray = str2.Split(',');
                            if (strArray.Length == 2)
                            {
                                string str3 = strArray[0].Trim();
                                try
                                {
                                    WorkshopItem workshopItem = WorkshopItem.GetItem(Convert.ToUInt64(str3));
                                    _joiningLobby.workshopItems.Add(workshopItem);
                                }
                                catch (Exception)
                                {
                                    DevConsole.Log(DCSection.General, "SubscribeAndRestart failed to enable workshop item (" + str2.ToString() + ")");
                                }
                            }
                        }
                    }
                }
            }
            foreach (WorkshopItem workshopItem in _joiningLobby.workshopItems)
            {
                WorkshopItem w = workshopItem;
                Mod mod = ModLoader.allMods.FirstOrDefault(x => (long)x.configuration.workshopID == (long)w.id);
                if (mod != null)
                    mod.configuration.disabled = false;
                Steam.WorkshopSubscribe(w.id);
            }
            Program.commandLine = Program.commandLine + " -downloadmods +connect_lobby " + _joiningLobby.lobby.id.ToString();
            if (MonoMain.lobbyPassword != "")
                Program.commandLine = Program.commandLine + " +password " + MonoMain.lobbyPassword;
            ModLoader.DisabledModsChanged();
            ModLoader.RestartGame();
        }

        public UIServerBrowser(
          UIMenu openOnClose,
          string title,
          float xpos,
          float ypos,
          float wide = -1f,
          float high = -1f,
          InputProfile conProfile = null)
          : base(title, xpos, ypos, wide, high, "@WASD@@SELECT@JOIN @MENU1@REFRESH @CANCEL@BACK", conProfile)
        {
            defaultImage = Content.Load<Tex2D>("server_default");
            defaultImageLan = Content.Load<Tex2D>("server_default_lan");
            _splitter.topSection.components[0].align = UIAlign.Left;
            _openOnClose = openOnClose;
            _moreArrow = new Sprite("moreArrow");
            _moreArrow.CenterOrigin();
            _steamIcon = new Sprite("steamIconSmall")
            {
                scale = new Vec2(1f) / 2f
            };
            _lanIcon = new Sprite("lanIconSmall")
            {
                scale = new Vec2(1f) / 2f
            };
            _lockedServer = new Sprite("lockedServer");
            _globeIcon = new Sprite("smallEarth");
            _namedServer = new Sprite("namedServer");
            _localIcon = new SpriteMap("iconSheet", 16, 16)
            {
                scale = new Vec2(1f) / 2f
            };
            _localIcon.SetFrameWithoutReset(1);
            _newIcon = new SpriteMap("presents", 16, 16)
            {
                scale = new Vec2(2f)
            };
            _newIcon.SetFrameWithoutReset(0);
            _noImage = new Sprite("notexture")
            {
                scale = new Vec2(2f)
            };
            _cursor = new SpriteMap("cursors", 16, 16);
            _maxLobbiesToShow = 8;
            _box = new UIBox(0f, 0f, high: _maxLobbiesToShow * 36, isVisible: false);
            Add(_box, true);
            _fancyFont = new FancyBitmapFont("smallFont")
            {
                maxWidth = (int)width - 100,
                maxRows = 2
            };
            scrollBarOffset = 0;
            _editModMenu = new UIMenu("<mod name>", Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 160f, conString: "@SELECT@SELECT");
            _editModMenu.Add(new UIText(" ", Color.White), true);
            _editModMenu.Add(new UIMenuItem("BACK", new UIMenuActionOpenMenu(_editModMenu, this)), true);
            _editModMenu.Close();
            _yesNoMenu = new UIMenu("ARE YOU SURE?", Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 160f, conString: "@SELECT@SELECT");
            _yesNoMenu.Add(new UIMenuItem("YES"), true);
            _yesNoMenu.Add(new UIMenuItem("NO"), true);
            _yesNoMenu.Close();
            //this._updateTextBox = new Textbox(0f, 0f, 0f, 0f)
            //{
            //    depth = (Depth)0.9f,
            //    maxLength = 5000
            //};
            _downloadModsMenu = new UIMenu("MODS REQUIRED!", Layer.HUD.camera.width / 2, Layer.HUD.camera.height / 2, 290, -1, "@SELECT@SELECT");
            _downloadModsMenu.Add(new UIText("You're missing the mods required", Colors.DGBlue));
            _downloadModsMenu.Add(new UIText("to join this game!", Colors.DGBlue));
            _downloadModsMenu.Add(new UIText("", Colors.DGBlue));
            _downloadModsMenu.Add(new UIText("Would you like to restart the", Colors.DGBlue));
            _downloadModsMenu.Add(new UIText("game, automatically download the", Colors.DGBlue));
            _downloadModsMenu.Add(new UIText("required mods and join the game?", Colors.DGBlue));
            _downloadModsMenu.Add(new UIText("", Colors.DGBlue));

            _downloadModsMenu.Add(new UIMenuItem("CANCEL", new UIMenuActionOpenMenu(_downloadModsMenu, this)));
            _downloadModsMenu.Add(new UIMenuItem("RESTART AND DOWNLOAD", new UIMenuActionCloseMenuCallFunction(_downloadModsMenu, SubscribeAndRestart)));
            _downloadModsMenu.Close();

            _modsInfoMenu = new UIModManagement(this, "@WRENCH@ACTIVE SERVER MODS@SCREWDRIVER@", Layer.HUD.camera.width, Layer.HUD.camera.height, 550, -1, "@WASD@@SELECT@ADJUST @MENU1@TOGGLE @CANCEL@BACK", null, true);



            _passwordEntryMenu = new UIStringEntryMenu(false, "ENTER PASSWORD", new FieldBinding(this, nameof(enteredPassword)));
            _portEntryMenu = new UIStringEntryMenu(false, "ENTER PORT", new FieldBinding(this, nameof(enteredPort)), 6, true, 1337, 55535);
            _portEntryMenu.SetBackFunction(new UIMenuActionOpenMenu(_portEntryMenu, this));
            _portEntryMenu.Close();
            _passwordEntryMenu.SetBackFunction(new UIMenuActionOpenMenu(_passwordEntryMenu, this));
            _passwordEntryMenu.Close();

            _mapSprites = new Sprite[]
            {
                CreateMapSprite("normalIcon"),
                CreateMapSprite("randomIcons"),
                CreateMapSprite("customIcon"),
                CreateMapSprite("rainbowIcon")
            };

            _percentageFunctions = new Func<Lobby, int>[]
            {
                GetNormalMapsPercentage,
                GetRandomMapsPercentage,
                GetCustomMapsPercentage,
                GetInternetMapsPercentage
            };
        }

        public override void Open()
        {
            _selectedLobby = null;
            _pressWait = 30;
            base.Open();
            DevConsole.SuppressDevConsole();
            _oldPos = Mouse.positionScreen;
            _hoverIndex = -1;
            if (!_enteringPort)
                RefreshLobbySearch(SearchMode.Near, SearchMode.Global, SearchMode.LAN);
            _enteringPort = false;
        }

        public void RefreshLobbySearch() => RefreshLobbySearch(SearchMode.Near, SearchMode.Global, SearchMode.LAN);

        public void RefreshLobbySearch(params SearchMode[] pParts)
        {
            _modeQueue.Clear();
            bool SteamIsInitialized = Steam.IsInitialized();
            foreach (SearchMode pPart in pParts)
            {
                if (!SteamIsInitialized && pPart != SearchMode.LAN)
                {
                    continue;
                }
                _modeQueue.Enqueue(pPart);
            }
            _lobbies.Clear();
            _selectedLobby = null;
        }

        private void TryJoiningLobby(LobbyData pLobby)
        {
            _joiningLobby = pLobby;
            if (ModLoader.modHash == _joiningLobby.modHash)
            {
                Close();
                _attemptConnection = UIMatchmakerMark2.Platform_GetMatchkmaker(pLobby, this);
                _attemptConnection.SetPasswordAttempt(enteredPassword);
                enteredPassword = "";
                Level.Add(_attemptConnection);
                _attemptConnection.Open();
                MonoMain.pauseMenu = _attemptConnection;
            }
            else
            {
                MonoMain.lobbyPassword = enteredPassword;
                new UIMenuActionOpenMenu(this, _downloadModsMenu).Activate();
                enteredPassword = "";
            }
        }

        private void UpdateLobbySearch()
        {
            if (!_searching && mode != SearchMode.None && Graphics.frame >= _lobbySearchCooldownNextAvailable)
            {
                _lobbySearchCooldownNextAvailable = Graphics.frame;
                Network.lanMode = mode == SearchMode.LAN;
                NCBasic.lobbySearchPort = lanSearchPort;
                if (mode == SearchMode.Global)
                    NCSteam.globalSearch = true;
                _selectedLobby = null;
                Network.activeNetwork.core.SearchForLobby();
                _searching = true;
            }
            if (!_searching || mode == SearchMode.None || !Network.activeNetwork.core.IsLobbySearchComplete())
                return;
            _searching = false;
            int numLobbiesFound = Network.activeNetwork.core.NumLobbiesFound();
            List<WorkshopItem> queryItems = new List<WorkshopItem>();
            if (Network.lanMode)
            {
                foreach (LobbyData foundLobby in (Network.activeNetwork.core as NCBasic)._foundLobbies)
                    _lobbies.Add(foundLobby);
            }
            else
            {
                for (int i = 0; i < numLobbiesFound; ++i)
                {
                    Lobby lobby = Network.activeNetwork.core.GetSearchLobbyAtIndex(i);
                    if (_lobbies.FirstOrDefault(x => x.lobby != null && (long)x.lobby.id == (long)lobby.id) == null)
                    {
                        string lobbyData1 = lobby.GetLobbyData("name");
                        if (!string.IsNullOrEmpty(lobbyData1))
                        {
                            LobbyData d = new LobbyData
                            {
                                lobby = lobby,
                                name = DuckNetwork.core.FilterText(lobbyData1, null),
                                hasCustomName = lobby.GetLobbyData("customName") == "true",
                                modHash = lobby.GetLobbyData("modhash"),
                                requiredWins = lobby.GetLobbyData("requiredwins"),
                                restsEvery = lobby.GetLobbyData("restsevery"),
                                wallMode = lobby.GetLobbyData("wallmode"),
                                customLevels = lobby.GetLobbyData("customLevels"),
                                version = lobby.GetLobbyData("version"),
                                started = lobby.GetLobbyData("started"),
                                type = lobby.GetLobbyData("type")
                            };
                            try
                            {
                                d.numSlots = Convert.ToInt32(lobby.GetLobbyData("numSlots"));
                            }
                            catch (Exception)
                            {
                                d.numSlots = 0;
                            }
                            d.hasModifiers = lobby.GetLobbyData("modifiers");
                            d.hasPassword = lobby.GetLobbyData("password") == "true";
                            d.dedicated = lobby.GetLobbyData("dedicated") == "true";
                            d.pingstring = lobby.GetLobbyData("pingstring");
                            d.DGR = lobby.GetLobbyData("DGR") == "true";
                            if (d.pingstring != "" && d.pingstring != null)
                                d.estimatedPing = Steam.EstimatePing(d.pingstring);
                            try
                            {
                                d.datahash = Convert.ToInt64(lobby.GetLobbyData("datahash"));
                            }
                            catch (Exception)
                            {
                            }
                            d.isGlobalLobby = mode == SearchMode.Global;
                            d.hasFriends = false;
                            foreach (User user in lobby.users)
                            {
                                if (Steam.friends.Contains(user))
                                {
                                    d.hasFriends = true;
                                    break;
                                }
                            }
                            string loadedMods = lobby.GetLobbyData("mods");

                            if (loadedMods != null && loadedMods != "")
                            {
                                    loadedMods = loadedMods.Replace("|3132351890,0", ""); //dumb but works -NiK0
                                    loadedMods = loadedMods.Replace("3132351890,0", "");
                                string[] mods = loadedMods.Split('|');

                                foreach (string s in mods)
                                {
                                    try
                                    {
                                        if (s == "")
                                            continue;

                                        if (s == "LOCAL") d.hasLocalMods = true;
                                        else
                                        {
                                            string[] s2 = s.Split(',');
                                            string workshopID = "";
                                            if (s2.Length == 2) workshopID = s2[0].Trim();
                                            else workshopID = s;

                                            WorkshopItem w = WorkshopItem.GetItem(Convert.ToUInt64(workshopID));
                                            if (w != null)
                                            {
                                                queryItems.Add(w);
                                                d.workshopItems.Add(w);
                                            }
                                        }
                                    }
                                    catch (Exception e)
                                    {
                                        //how u doin daniel
                                    }
                                }
                            }
                            try
                            {
                                d.maxPlayers = Convert.ToInt32(lobby.GetLobbyData("maxplayers"));
                            }
                            catch (Exception)
                            {
                                d.maxPlayers = 0;
                            }
                            _lobbies.Add(d);
                        }
                    }
                }
            }
            if (queryItems.Count > 0)
                Steam.RequestWorkshopInfo(queryItems);
            int num2 = (int)_modeQueue.Dequeue();
        }

        public override void Update()
        {
            if (open)
            {
                if (_passwordLobby != null && _passwordLobby.hasPassword && enteredPassword != "")
                {
                    TryJoiningLobby(_passwordLobby);
                    _passwordLobby = null;
                    return;
                }

                if (!Network.available) _controlString = "@WASD@@SELECT@JOIN @MENU1@REFRESH @CANCEL@BACK";
                else _controlString = "@WASD@@SELECT@JOIN @MENU1@REFRESH @CANCEL@BACK @RAGDOLL@REFRESH LAN";

                if (_selectedLobby != null && _selectedLobby.modHash != "nomods")
                    _controlString += " @MENU2@VIEW MODS";

                UpdateLobbySearch();
            }
            if (_pressWait > 0)
                --_pressWait;
            if (_downloadModsMenu.open)
            {
                _downloadModsMenu.DoUpdate();
                if (!globalUILock && (Input.Pressed(Triggers.Cancel) || Keyboard.Pressed(Keys.Escape)))
                {
                    _downloadModsMenu.Close();
                    Open();
                    return;
                }
            }
            else if (open)
            {
                MonoMain.lobbyPassword = null;
                if (_gamepadMode)
                {
                    if (_hoverIndex < 0)
                        _hoverIndex = 0;
                }
                else
                {
                    _hoverIndex = -1;
                    for (int index = 0; index < _maxLobbiesToShow && _scrollItemOffset + index < _lobbies.Count; ++index)
                    {
                        if (new Rectangle((int)(_box.x - _box.halfWidth), (int)(_box.y - _box.halfHeight + 36 * index), (int)_box.width - 14, 36f).Contains(Mouse.position))
                        {
                            _hoverIndex = _scrollItemOffset + index;
                            break;
                        }
                    }
                }
                if (_hoverIndex != -1)
                {
                    if (Input.Pressed(Triggers.Menu1))
                    {
                        RefreshLobbySearch(SearchMode.Near, SearchMode.Global, SearchMode.LAN);
                        SFX.Play("rockHitGround", 0.8f);
                    }
                    else if (Input.Pressed(Triggers.Ragdoll) || enteredPort != "")
                    {
                        if (enteredPort == "")
                        {
                            _enteringPort = true;
                            _portEntryMenu.SetValue(lanSearchPort.ToString());
                            new UIMenuActionOpenMenu(this, _portEntryMenu).Activate();
                        }
                        else
                        {
                            try
                            {
                                lanSearchPort = Convert.ToInt32(enteredPort);
                            }
                            catch (Exception)
                            {
                            }
                            enteredPort = "";
                            RefreshLobbySearch(SearchMode.LAN);
                            SFX.Play("rockHitGround", 0.8f);
                        }
                    }
                    else if (Input.Pressed(Triggers.Menu2) && _selectedLobby != null && _selectedLobby.modHash != "nomods")
                    {
                        _modsInfoMenu._lobby = _selectedLobby;
                        _modsInfoMenu.RebuildModList();
                        (new UIMenuActionOpenMenu(this, _modsInfoMenu)).Activate();
                    }
                    if (_lobbies.Count > 0 && _hoverIndex < _lobbies.Count)
                    {
                        _selectedLobby = _lobbies[_hoverIndex];
                        if (Input.Pressed(Triggers.Select) && _pressWait == 0 && _gamepadMode || !_gamepadMode && Mouse.left == InputState.Pressed || enteredPassword != "")
                        {
                            if (!_selectedLobby.canJoin)
                            {
                                SFX.Play("consoleError");
                            }
                            else
                            {
                                SFX.Play("consoleSelect");
                                if (_selectedLobby.hasPassword && enteredPassword == "")
                                {
                                    _passwordLobby = _selectedLobby;
                                    new UIMenuActionOpenMenu(this, _passwordEntryMenu).Activate();
                                }
                                else
                                {
                                    TryJoiningLobby(_selectedLobby);
                                    return;
                                }
                            }
                        }
                    }
                }
                else
                    _selectedLobby = null;
                if (_gamepadMode)
                {
                    _draggingScrollbar = false;
                    if (Input.Pressed(Triggers.MenuDown))
                        ++_hoverIndex;
                    else if (Input.Pressed(Triggers.MenuUp))
                        --_hoverIndex;
                    if (Input.Pressed(Triggers.Strafe))
                        _hoverIndex -= 10;
                    else if (Input.Pressed(Triggers.Ragdoll))
                        _hoverIndex += 10;
                    if (_hoverIndex < 0)
                        _hoverIndex = 0;
                    if ((_oldPos - Mouse.positionScreen).lengthSq > 200.0)
                        _gamepadMode = false;
                }
                else
                {
                    if (!_draggingScrollbar)
                    {
                        if (Mouse.left == InputState.Pressed && ScrollBarBox().Contains(Mouse.position))
                        {
                            _draggingScrollbar = true;
                            _oldPos = Mouse.position;
                        }
                        if (Mouse.scroll > 0.0)
                        {
                            _scrollItemOffset += 5;
                            _hoverIndex += 5;
                        }
                        else if (Mouse.scroll < 0.0)
                        {
                            _scrollItemOffset -= 5;
                            _hoverIndex -= 5;
                            if (_hoverIndex < 0)
                                _hoverIndex = 0;
                        }
                    }
                    else if (Mouse.left != InputState.Down)
                    {
                        _draggingScrollbar = false;
                    }
                    else
                    {
                        Vec2 vec2 = Mouse.position - _oldPos;
                        _oldPos = Mouse.position;
                        scrollBarOffset += (int)vec2.y;
                        if (scrollBarOffset > scrollBarScrollableHeight)
                            scrollBarOffset = scrollBarScrollableHeight;
                        else if (scrollBarOffset < 0)
                            scrollBarOffset = 0;
                        _scrollItemOffset = (int)((_lobbies.Count - _maxLobbiesToShow) * (scrollBarOffset / (float)scrollBarScrollableHeight));
                    }
                    if (Input.Pressed(Triggers.Any))
                    {
                        _gamepadMode = true;
                        _oldPos = Mouse.positionScreen;
                    }
                }
                if (_scrollItemOffset < 0)
                    _scrollItemOffset = 0;
                else if (_scrollItemOffset > Math.Max(0, _lobbies.Count - _maxLobbiesToShow))
                    _scrollItemOffset = Math.Max(0, _lobbies.Count - _maxLobbiesToShow);
                if (_hoverIndex >= _lobbies.Count)
                    _hoverIndex = _lobbies.Count - 1;
                else if (_hoverIndex >= _scrollItemOffset + _maxLobbiesToShow)
                    _scrollItemOffset += _hoverIndex - (_scrollItemOffset + _maxLobbiesToShow) + 1;
                else if (_hoverIndex >= 0 && _hoverIndex < _scrollItemOffset)
                    _scrollItemOffset -= _scrollItemOffset - _hoverIndex;
                scrollBarOffset = _scrollItemOffset == 0 ? 0 : (int)Lerp.FloatSmooth(0f, scrollBarScrollableHeight, _scrollItemOffset / (float)(_lobbies.Count - _maxLobbiesToShow));
                if (!Editor.hoverTextBox && !globalUILock && (Input.Pressed(Triggers.Cancel) || Keyboard.Pressed(Keys.Escape)))
                {
                    new UIMenuActionOpenMenu(this, _openOnClose).Activate();
                    return;
                }
            }
            if (_showingMenu)
            {
                HUD.CloseAllCorners();
                _showingMenu = false;
            }
            base.Update();
        }

        private Rectangle ScrollBarBox() => new Rectangle((float)(_box.x + _box.halfWidth - 12.0 + 1.0), (float)(_box.y - _box.halfHeight + 1.0) + scrollBarOffset, 10f, 32f);

        private void Completed(object sender, AsyncCompletedEventArgs e)
        {
            if (!_clientMap.ContainsKey(sender))
                return;
            ulong client = _clientMap[sender];
            _clientMap.Remove(sender);
            if (!_previewMap.ContainsKey(client))
                return;
            Texture2D texture2D = ContentPack.LoadTexture2D(PreviewPathForWorkshopItem(client), false);
            if (texture2D == null)
                return;
            Tex2D tex2D = (Tex2D)texture2D;
            if (tex2D == null)
                return;
            _previewMap[client] = tex2D;
        }
        private static void WorkshopPreviewCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (_clientMap.ContainsKey(sender))
            {
                ulong id = _clientMap[sender];
                _clientMap.Remove(sender);

                if (_previewMap.ContainsKey(id))
                {
                    string path = PreviewPathForWorkshopItem(id);
                    Texture2D texture = ContentPack.LoadTexture2D(path, false);
                    if (texture != null)
                    {
                        Tex2D tex = texture;
                        if (tex != null)
                            _previewMap[id] = tex;
                    }
                }
            }
        }
            public static Tex2D GetWorkshopPreview(WorkshopItem w)
        {
            Tex2D tex = null;
            if (!_previewMap.TryGetValue(w.id, out tex))
            {
                if (w.data.previewPath != null && w.data.previewPath != "")
                {
                    try
                    {
                        WebClient client = new WebClient();
                        string file = PreviewPathForWorkshopItem(w.id);

                        DuckFile.CreatePath(file);
                        if (File.Exists(file))
                            DuckFile.Delete(file);

                        client.DownloadFileAsync(new Uri(w.data.previewPath), file);
                        client.DownloadFileCompleted += new AsyncCompletedEventHandler(WorkshopPreviewCompleted);
                        _clientMap[client] = w.id;
                    }
                    catch (Exception e)
                    {

                    }
                }

                _previewMap[w.id] = null;
            }

            return tex;
        }

        public static string PreviewPathForWorkshopItem(ulong id) => DuckFile.workshopDirectory + "/modPreview" + id.ToString() + "preview.png";

        //no longer needed -NiK0
        /*public void nikostuff(ref string str2, WorkshopItem workshopItem1, LobbyData lobby)
        {
            //str2 = !lobby.hasFirstMod ? "|RED|Requires " + workshopItem1.data.name : "|DGGREEN|Requires " + workshopItem1.data.name;

            WorkshopItem itd2 = WorkshopItem.GetItem(workshopItem1.id);

            

            str2 = $"id:{workshopItem1.id} name:{workshopItem1.name} dataname:{workshopItem1.data.name} itdName:{itd2.name} itd2DataName:{itd2.data.name}";
        }*/
        public override void Draw()
        {
            if (_downloadModsMenu.open)
                _downloadModsMenu.DoDraw();
            if (open)
            {
                scrollBarTop = (int)(_box.y - _box.halfHeight + 1.0 + 16.0);
                scrollBarBottom = (int)(_box.y + _box.halfHeight - 1.0 - 16.0);
                scrollBarScrollableHeight = scrollBarBottom - scrollBarTop;
                if (fixView)
                {
                    Layer.HUD.camera.width *= 2f;
                    Layer.HUD.camera.height *= 2f;
                    fixView = false;
                }
                Graphics.DrawRect(new Vec2(_box.x - _box.halfWidth, _box.y - _box.halfHeight), new Vec2((float)(_box.x + _box.halfWidth - 12.0 - 2.0), _box.y + _box.halfHeight), Color.Black, (Depth)0.4f);
                Graphics.DrawRect(new Vec2((float)(_box.x + _box.halfWidth - 12.0), _box.y - _box.halfHeight), new Vec2(_box.x + _box.halfWidth, _box.y + _box.halfHeight), Color.Black, (Depth)0.4f);
                Rectangle r = ScrollBarBox();
                Graphics.DrawRect(r, _draggingScrollbar || r.Contains(Mouse.position) ? Color.LightGray : Color.Gray, (Depth)0.5f);
                if (_lobbies.Count == 0)
                {
                    float num1 = _box.x - _box.halfWidth;
                    float num2 = _box.y - _box.halfHeight;
                    if (mode == SearchMode.None)
                        _fancyFont.Draw("No games found!", new Vec2(num1 + 10f, num2 + 2f), Color.Yellow, (Depth)0.5f);
                    else
                        _fancyFont.Draw("Waiting for game list.", new Vec2(num1 + 10f, num2 + 2f), Colors.DGGreen, (Depth)0.5f);
                }
                if (mode != SearchMode.None)
                {
                    float x = (float)(_box.x - _box.halfWidth + 116.0);
                    float y = _splitter.topSection.y - 5f;
                    if(MonoMain.UpdateLerpState)
                        _refreshingDots += 0.01f;
                    if (_refreshingDots > 1.0)
                        _refreshingDots = 0f;
                    string str = "(REFRESHING";
                    for (int index = 0; index < 3; ++index)
                    {
                        if (_refreshingDots * 4.0 > index + 1)
                            str += ".";
                    }
                    _fancyFont.Draw(str + ")", new Vec2(x, y), Colors.DGGreen, (Depth)0.5f);
                }
                for (int i = 0; i < _lobbies.Count; ++i)
                {
                    LobbyData lobby = _lobbies[i];
                    if (lobby != null)
                    {
                        lobby.UpdateUserCount();
                    }
                }
                _lobbies.Sort();
                for (int index1 = 0; index1 < _maxLobbiesToShow; ++index1)
                {
                    int index2 = _scrollItemOffset + index1;
                    if (index2 < _lobbies.Count)
                    {
                        float x1 = _box.x - _box.halfWidth;
                        float y = _box.y - _box.halfHeight + 36 * index1;
                        if (_hoverIndex == index2)
                            Graphics.DrawRect(new Vec2(x1, y), new Vec2((float)(x1 + _box.width - 14.0), y + 36f), Color.White * 0.6f, (Depth)0.4f);
                        else if ((index2 & 1) != 0)
                            Graphics.DrawRect(new Vec2(x1, y), new Vec2((float)(x1 + _box.width - 14.0), y + 36f), Color.White * 0.1f, (Depth)0.4f);
                        LobbyData lobby = _lobbies[index2];
                        if (lobby != null)
                        {
                            _noImage.texture = defaultImage;
                            if (lobby.lobby == null)
                                _noImage.texture = defaultImageLan;
                            _noImage.scale = new Vec2(1f, 1f);
                            List<Tex2D> workshopTextures = new List<Tex2D>();
                            string titleString = lobby.name;
                            if (lobby.DGR)
                            {
                                titleString += "@DGR@";
                            }
                            if (lobby.lobby == null)
                                titleString += !lobby.dedicated ? " (LAN)" : " |DGGREEN|(DEDICATED LAN SERVER)";
                            else if (lobby.dedicated)
                                titleString += " |DGGREEN|(DEDICATED SERVER)";
                            string text1 = "|WHITE||GRAY|\n";
                            if (lobby.workshopItems.Count > 0)
                            {
                                WorkshopItem workshopItem1 = lobby.workshopItems[0];
                                if (workshopItem1.data != null)
                                {
                                    lobby.workshopItems = lobby.workshopItems.OrderByDescending(x => x.data == null ? 0 : x.data.votesUp).ToList();
                                    if (!lobby.downloadedWorkshopItems)
                                    {
                                        lobby.hasFirstMod = true;
                                        lobby.hasRestOfMods = true;
                                        bool flag = true;
                                        foreach (WorkshopItem workshopItem2 in lobby.workshopItems)
                                        {
                                            ulong id = workshopItem2.id;
                                            if (ModLoader.accessibleMods.FirstOrDefault(x => (long)x.configuration.workshopID == (long)id) == null)
                                            {
                                                if (flag)
                                                    lobby.hasFirstMod = false;
                                                else
                                                    lobby.hasRestOfMods = false;
                                            }
                                            flag = false;
                                        }
                                        lobby.downloadedWorkshopItems = true;
                                    }
                                    string str2 = !lobby.hasFirstMod ? "|RED|Requires " + workshopItem1.name : "|DGGREEN|Requires " + workshopItem1.name;
                                    //if (Keyboard.Down(Keys.LeftControl) && Debugger.IsAttached)
                                    //{
                                    //    nikostuff(ref str2, workshopItem1, lobby);   
                                    //}
                                    string str3 = lobby.hasRestOfMods ? "|DGGREEN|" : "|RED|";
                                    if (lobby.workshopItems.Count == 2)
                                        str2 = str2 + str3 + " +" + (lobby.workshopItems.Count - 1).ToString() + " other mod.";
                                    else if (lobby.workshopItems.Count > 2)
                                        str2 = str2 + str3 + " +" + (lobby.workshopItems.Count - 1).ToString() + " other mods.";
                                    text1 = str2 + "\n|GRAY|";

                                    Tex2D tex = GetWorkshopPreview(workshopItem1);
                                    if (tex != null)
                                        workshopTextures.Add(tex);
                                }
                            }
                            if (!string.IsNullOrWhiteSpace(lobby.requiredWins))
                                text1 = text1 + "First to " + lobby.requiredWins.ToString() + " ";
                            if (!string.IsNullOrWhiteSpace(lobby.restsEvery))
                                text1 = text1 + "rests every " + lobby.restsEvery.ToString() + ". ";
                            if (!string.IsNullOrWhiteSpace(lobby.wallMode) && lobby.wallMode != "0")
                                text1 += "Wall Mode: ACTIVE. ";
                            if (!string.IsNullOrWhiteSpace(lobby.customLevels) && lobby.customLevels != "0")
                                text1 = text1 + "Custom Levels: " + lobby.customLevels.ToString() + ". ";
                            if (!string.IsNullOrWhiteSpace(lobby.hasModifiers) && lobby.hasModifiers != "false")
                                text1 += "Modifiers: ACTIVE.";
                            Graphics.DrawRect(new Vec2(x1 + 2f, y + 2f), new Vec2((float)(x1 + 36.0 - 2.0), (float)(y + 36.0 - 2.0)), Color.Gray, (Depth)0.5f, false, 2f);
                            if (workshopTextures.Count > 0)
                            {
                                Vec2 zero = Vec2.Zero;
                                for (int index3 = 0; index3 < 4; ++index3)
                                {
                                    if (index3 < workshopTextures.Count)
                                    {
                                        _noImage.texture = workshopTextures[index3];
                                        if (workshopTextures.Count > 1)
                                            _noImage.scale = new Vec2(16f / _noImage.texture.width);
                                        else
                                            _noImage.scale = new Vec2(32f / _noImage.texture.width);
                                        if (_noImage.texture.width != _noImage.texture.height)
                                        {
                                            if (_noImage.texture.width > _noImage.texture.height)
                                            {
                                                _noImage.scale = new Vec2(32f / _noImage.texture.height);
                                                Graphics.Draw(_noImage, x1 + 2f + zero.x, y + 2f + zero.y, new Rectangle(_noImage.texture.width / 2 - _noImage.texture.height / 2, 0f, _noImage.texture.height, _noImage.texture.height), (Depth)0.5f);
                                            }
                                            else
                                                Graphics.Draw(_noImage, x1 + 2f + zero.x, y + 2f + zero.y, new Rectangle(0f, 0f, _noImage.texture.width, _noImage.texture.width), (Depth)0.5f);
                                        }
                                        else
                                            Graphics.Draw(_noImage, x1 + 2f + zero.x, y + 2f + zero.y, (Depth)0.5f);
                                        zero.x += 16f;
                                        if (zero.x >= 32.0)
                                        {
                                            zero.x = 0f;
                                            zero.y += 16f;
                                        }
                                    }
                                }
                            }
                            else
                                Graphics.Draw(_noImage, x1 + 2f, y + 2f, (Depth)0.5f);
                            titleString += " (" + (lobby._userCount - (lobby.dedicated ? 1 : 0)).ToString() + "/" + lobby.numSlots.ToString() + ")";
                            if (lobby.hasFriends)
                                titleString += " |DGGREEN|FRIEND";
                            if (lobby.hasPassword)
                                titleString += " |DGRED|HAS PASSWORD";
                            if (!lobby.canJoin)
                            {
                                titleString += " |DGRED|(";
                                if (lobby.version != DG.version)
                                {
                                    switch (DuckNetwork.CheckVersion(lobby.version))
                                    {
                                        case NMVersionMismatch.Type.Older:
                                            titleString += "They have an older version.";
                                            break;
                                        case NMVersionMismatch.Type.Newer:
                                            titleString += "They have a newer version.";
                                            break;
                                        default:
                                            titleString += "They have a different version.";
                                            break;
                                    }
                                }
                                else
                                {
                                    if (lobby.datahash != Network.gameDataHash)
                                    {
                                        titleString += "Their version is incompatible.";
                                    }
                                    else if (lobby.started == "true")
                                    {
                                        titleString += "This game is in progress.";
                                    }
                                    else if (lobby._userCount >= lobby.numSlots)
                                    {
                                        titleString += "Lobby is full.";
                                    }
                                    else if (lobby.lobby != null && lobby.type != "2")
                                    {
                                        titleString += "This game is not public.";
                                    }
                                    else if (lobby.hasLocalMods)
                                    {
                                        titleString += "This game is using non-workshop mods.";
                                    }
                                    else
                                    {
                                        titleString += "Cannot join.";
                                    }
                                } //removed  ParentalControls.AreParentalControlsActive and unpacked
                                titleString += ")";
                                Graphics.DrawRect(new Vec2(x1, y), new Vec2((float)(x1 + _box.width - 14.0), y + 36f), Color.Black * 0.5f, (Depth)0.99f);
                            }
                            _fancyFont.maxWidth = 1000;
                            float num = 0f;
                            if (lobby.hasPassword)
                            {
                                Graphics.Draw(_lockedServer, (float)(x1 + 36.0 + 10.0), y + 2.5f, (Depth)0.5f);
                                num += 10f;
                            }
                            if (lobby.hasCustomName)
                            {
                                Graphics.Draw(_namedServer, (float)(x1 + num + 36.0 + 10.0), y + 2.5f, (Depth)0.5f);
                                num += 10f;
                            }
                            if (lobby.isGlobalLobby)
                            {
                                Graphics.Draw(_globeIcon, (float)(x1 + num + 36.0 + 10.0), y + 2.5f, (Depth)0.5f);
                                num += 10f;
                            }
                            _fancyFont.Draw(titleString, new Vec2((float)(x1 + 36.0 + num + 10.0), y + 2f), Color.Yellow, (Depth)0.5f);
                            if (lobby.version == DG.version)
                                _fancyFont.Draw(lobby.version, new Vec2((float)(x1 + 430.0 + 10.0), y + 2f), Colors.DGGreen * 0.45f, (Depth)0.5f);
                            else
                                _fancyFont.Draw(lobby.version, new Vec2((float)(x1 + 430.0 + 10.0), y + 2f), Colors.DGRed * 0.45f, (Depth)0.5f);
                            _fancyFont.Draw("|WHITE|Ping:", new Vec2(x1 + 440f, y + 26f), Color.White * 0.45f, (Depth)0.5f);
                            if (lobby.pingRefreshTimeout <= 0)
                            {
                                lobby.pingRefreshTimeout = 60;
                                lobby.estimatedPing = Steam.EstimatePing(lobby.pingstring);
                            }
                            --lobby.pingRefreshTimeout;
                            if (lobby.estimatedPing != -1)
                            {
                                Color color = Colors.DGGreen;
                                if (lobby.estimatedPing > 150)
                                    color = Colors.DGYellow;
                                if (lobby.estimatedPing > 250)
                                    color = Colors.DGRed;
                                _fancyFont.Draw(lobby.estimatedPing.ToString() + "ms", new Vec2(x1 + 470f, y + 26f), color * 0.45f, (Depth)0.5f);
                            }
                            else
                                _fancyFont.Draw("????ms", new Vec2(x1 + 470f, y + 26f), Colors.DGRed * 0.45f, (Depth)0.5f);
                            if (lobby.lobby != null)
                                Graphics.Draw(_steamIcon, x1 + 36f, y + 2.5f, (Depth)0.5f);
                            else
                                Graphics.Draw(_lanIcon, x1 + 36f, y + 2.5f, (Depth)0.5f);
                            _fancyFont.Draw(text1, new Vec2(x1 + 36f, y + 6f + _fancyFont.characterHeight), Color.LightGray, (Depth)0.5f);

                            Lobby steamLobby = lobby.lobby;

                            if (steamLobby is null)
                                continue;

                            if (DGRSettings.LobbyData)
                            {
                                Vec2 position = new Vec2(x1, y);

                                for (int i = 0; i < _percentageFunctions.Length; i++)
                                {
                                    float mapX = position.x + _mapsOffset.x;
                                    float mapY = position.y + _mapsOffset.y + 9f * i;

                                    int percentage = _percentageFunctions[i](steamLobby);

                                    Graphics.Draw(_mapSprites[i], mapX, mapY, 0.5f);
                                    _smallFont.Draw(percentage.ToString() + "%", new Vec2(mapX + 10f, mapY + 1f), Color.White, 0.5f);
                                }

                                string names = steamLobby.GetLobbyData("players");

                                if (names is null)
                                    continue;

                                string[] namesSplit = names.Split('\n');

                                for (int i = 0; i < namesSplit.Length; i++)
                                {
                                    float nameOffsetX = (_smallFont.maxWidth + 8f) * (i / _nicknameRows);
                                    float nameOffsetY = 9f * (i % _nicknameRows);

                                    _smallFont.Draw(namesSplit[i], position + _namesOffset + new Vec2(nameOffsetX, nameOffsetY), Color.White, 0.5f);
                                }
                            }
                        }
                    }
                    else
                        break;
                }
                if (Mouse.available && !_gamepadMode)
                {
                    _cursor.depth = (Depth)1f;
                    _cursor.scale = new Vec2(1f, 1f);
                    _cursor.position = Mouse.position;
                    _cursor.frame = 0;
                    if (Editor.hoverTextBox)
                    {
                        _cursor.frame = 5;
                        _cursor.position.y -= 4f;
                        _cursor.scale = new Vec2(0.5f, 1f);
                    }
                    _cursor.Draw();
                }
            }
            base.Draw();
        }

        private int GetNormalMapsPercentage(Lobby lobby)
        {
            return GetLobbyData(lobby, "normalmaps");
        }

        private int GetRandomMapsPercentage(Lobby lobby)
        {
            return GetLobbyData(lobby, "randommaps");
        }

        private int GetCustomMapsPercentage(Lobby lobby)
        {
            return GetLobbyData(lobby, "custommaps");
        }

        private int GetInternetMapsPercentage(Lobby lobby)
        {
            return 100 - GetNormalMapsPercentage(lobby) - GetRandomMapsPercentage(lobby) - GetCustomMapsPercentage(lobby);
        }

        private int GetLobbyData(Lobby lobby, string name)
        {
            string s = lobby.GetLobbyData(name);
            int result;
            int.TryParse(s, out result);
            return result;
        }

        private Sprite CreateMapSprite(string name)
        {
            return new Sprite(name)
            {
                scale = new Vec2(1.1f)
            };
        }

        public enum SearchMode
        {
            None,
            Near,
            Global,
            LAN,
        }

        public class LobbyData : IComparable<LobbyData>
        {
            public int _userCount;
            public string name;
            public bool hasCustomName;
            public string password;
            public int maxPlayers;
            public string lanAddress;
            public string restsEvery;
            public string requiredWins;
            public string wallMode;
            public string customLevels;
            public string version;
            public string started;
            public string type;
            public string hasModifiers;
            public long datahash;
            public bool DGR;
            public int numSlots;
            public string modHash;
            public bool hasLocalMods;
            public bool hasFirstMod;
            public bool hasRestOfMods;
            public bool downloadedWorkshopItems;
            public Lobby lobby;
            public bool hasPassword;
            public bool hasFriends;
            public bool dedicated;
            public string pingstring = "";
            public int estimatedPing = -1;
            public int pingRefreshTimeout;
            public bool isGlobalLobby;
            public List<WorkshopItem> workshopItems = new List<WorkshopItem>();

            public bool canJoin
            {
                get // removed AreParentalControlsActive
                {
                    return DG.version == version && (Network.gameDataHash == datahash || ModLoader.modHash != modHash) && started == "false" &&
                        (!hasLocalMods || ModLoader.modHash == modHash) && _userCount < numSlots && (type == "2" || lobby == null);
                }
            }

            public int userCount
            {
                get
                {
                    if (lobby != null)
                    {
                        return lobby.users.Count; // is slower than i would like
                    }
                    return _userCount;
                }
            }
            public void UpdateUserCount()
            {
                _userCount = userCount;
            }
            public int CompareTo(LobbyData other)
            {
                if (isGlobalLobby && !other.isGlobalLobby)
                    return 10000;
                if (!isGlobalLobby && other.isGlobalLobby)
                    return -10000;
                bool canjoinl = canJoin;
                bool othercanjoin = other.canJoin;
                if (canjoinl && !othercanjoin)
                    return -500;
                if (!canjoinl && othercanjoin)
                    return 500;
                if (hasFriends && !other.hasFriends)
                    return -100;
                if (!hasFriends && other.hasFriends)
                    return 100;
                if (!hasPassword && other.hasPassword)
                    return -10;
                if (hasPassword && !other.hasPassword)
                    return 10;
                if (lobby == null && other.lobby != null)
                    return -1000;
                if (lobby != null && other.lobby == null)
                    return 1000;
                if (dedicated && !other.dedicated)
                    return 1;
                if (!dedicated && other.dedicated)
                    return -1;
                if (estimatedPing < other.estimatedPing)
                    return -2;
                return estimatedPing > other.estimatedPing ? 2 : name.CompareTo(other.name);
            }
        }
    }
}