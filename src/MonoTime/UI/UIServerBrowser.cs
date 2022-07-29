// Decompiled with JetBrains decompiler
// Type: DuckGame.UIServerBrowser
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;

namespace DuckGame
{
    public class UIServerBrowser : UIMenu
    {
        private Queue<UIServerBrowser.SearchMode> _modeQueue = new Queue<UIServerBrowser.SearchMode>();
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
        private bool _enteringPort;
        private long _lobbySearchCooldownNextAvailable;
        //private const long MIN_FRAMES_BETWEEN_SEARCHES = 0;
        private float _refreshingDots;
        private bool _showingMenu;
        private bool _draggingScrollbar;
        private Vec2 _oldPos;
        public static UIServerBrowser.LobbyData _selectedLobby;
        private static UIServerBrowser.LobbyData _joiningLobby;
        private bool _searching;
        private List<UIServerBrowser.LobbyData> _lobbies = new List<UIServerBrowser.LobbyData>();
        private UIServerBrowser.LobbyData _passwordLobby;
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

        public UIServerBrowser.SearchMode mode => this._modeQueue.Count > 0 ? this._modeQueue.Peek() : UIServerBrowser.SearchMode.None;

        public override void Close()
        {
            if (!this.fixView)
            {
                this._showingMenu = false;
                this._editModMenu.Close();
                Layer.HUD.camera.width /= 2f;
                Layer.HUD.camera.height /= 2f;
                this.fixView = true;
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

        public static void SubscribeAndRestart()
        {
            foreach (Mod allMod in (IEnumerable<Mod>)ModLoader.allMods)
                allMod.configuration.disabled = true;
            if (ConnectionError.joinLobby != null)
            {
                UIServerBrowser._joiningLobby = new UIServerBrowser.LobbyData
                {
                    lobby = ConnectionError.joinLobby
                };
                string lobbyData = ConnectionError.joinLobby.GetLobbyData("mods");
                if (lobbyData != null && lobbyData != "")
                {
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
                                    UIServerBrowser._joiningLobby.workshopItems.Add(workshopItem);
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
            foreach (WorkshopItem workshopItem in UIServerBrowser._joiningLobby.workshopItems)
            {
                WorkshopItem w = workshopItem;
                Mod mod = ModLoader.allMods.FirstOrDefault<Mod>(x => (long)x.configuration.workshopID == (long)w.id);
                if (mod != null)
                    mod.configuration.disabled = false;
                Steam.WorkshopSubscribe(w.id);
            }
            Program.commandLine = Program.commandLine + " -downloadmods +connect_lobby " + UIServerBrowser._joiningLobby.lobby.id.ToString();
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
            this.defaultImage = Content.Load<Tex2D>("server_default");
            this.defaultImageLan = Content.Load<Tex2D>("server_default_lan");
            this._splitter.topSection.components[0].align = UIAlign.Left;
            this._openOnClose = openOnClose;
            this._moreArrow = new Sprite("moreArrow");
            this._moreArrow.CenterOrigin();
            this._steamIcon = new Sprite("steamIconSmall")
            {
                scale = new Vec2(1f) / 2f
            };
            this._lanIcon = new Sprite("lanIconSmall")
            {
                scale = new Vec2(1f) / 2f
            };
            this._lockedServer = new Sprite("lockedServer");
            this._globeIcon = new Sprite("smallEarth");
            this._namedServer = new Sprite("namedServer");
            this._localIcon = new SpriteMap("iconSheet", 16, 16)
            {
                scale = new Vec2(1f) / 2f
            };
            this._localIcon.SetFrameWithoutReset(1);
            this._newIcon = new SpriteMap("presents", 16, 16)
            {
                scale = new Vec2(2f)
            };
            this._newIcon.SetFrameWithoutReset(0);
            this._noImage = new Sprite("notexture")
            {
                scale = new Vec2(2f)
            };
            this._cursor = new SpriteMap("cursors", 16, 16);
            this._maxLobbiesToShow = 8;
            this._box = new UIBox(0.0f, 0.0f, high: this._maxLobbiesToShow * 36, isVisible: false);
            this.Add(_box, true);
            this._fancyFont = new FancyBitmapFont("smallFont")
            {
                maxWidth = (int)this.width - 100,
                maxRows = 2
            };
            this.scrollBarOffset = 0;
            this._editModMenu = new UIMenu("<mod name>", Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 160f, conString: "@SELECT@SELECT");
            this._editModMenu.Add(new UIText(" ", Color.White), true);
            this._editModMenu.Add(new UIMenuItem("BACK", new UIMenuActionOpenMenu(_editModMenu, this)), true);
            this._editModMenu.Close();
            this._yesNoMenu = new UIMenu("ARE YOU SURE?", Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 160f, conString: "@SELECT@SELECT");
            this._yesNoMenu.Add(new UIMenuItem("YES"), true);
            this._yesNoMenu.Add(new UIMenuItem("NO"), true);
            this._yesNoMenu.Close();
            //this._updateTextBox = new Textbox(0.0f, 0.0f, 0.0f, 0.0f)
            //{
            //    depth = (Depth)0.9f,
            //    maxLength = 5000
            //};
            this._downloadModsMenu = new UIMenu("MODS REQUIRED!", Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 290f, conString: "@SELECT@SELECT");
            this._downloadModsMenu.Add(new UIText("You're missing the mods required", Colors.DGBlue), true);
            this._downloadModsMenu.Add(new UIText("to join this game. Would you", Colors.DGBlue), true);
            this._downloadModsMenu.Add(new UIText("like to automatically subscribe to", Colors.DGBlue), true);
            this._downloadModsMenu.Add(new UIText("all required mods, restart and", Colors.DGBlue), true);
            this._downloadModsMenu.Add(new UIText("join the game?", Colors.DGBlue), true);
            this._downloadModsMenu.Add(new UIText("", Colors.DGBlue), true);
            this._downloadModsMenu.Add(new UIMenuItem("NO!", new UIMenuActionOpenMenu(_downloadModsMenu, this)), true);
            this._downloadModsMenu.Add(new UIMenuItem("YES!", new UIMenuActionCloseMenuCallFunction(_downloadModsMenu, new UIMenuActionCloseMenuCallFunction.Function(UIServerBrowser.SubscribeAndRestart))), true);
            this._downloadModsMenu.Close();
            if (!Network.available)
                this._controlString = "@WASD@@SELECT@JOIN @MENU1@REFRESH @CANCEL@BACK";
            else
                this._controlString = "@WASD@@SELECT@JOIN @MENU1@REFRESH @CANCEL@BACK @MENU2@REFRESH LAN";
            this._passwordEntryMenu = new UIStringEntryMenu(false, "ENTER PASSWORD", new FieldBinding(this, nameof(enteredPassword)));
            this._portEntryMenu = new UIStringEntryMenu(false, "ENTER PORT", new FieldBinding(this, nameof(enteredPort)), 6, true, 1337, 55535);
            this._portEntryMenu.SetBackFunction(new UIMenuActionOpenMenu(_portEntryMenu, this));
            this._portEntryMenu.Close();
            this._passwordEntryMenu.SetBackFunction(new UIMenuActionOpenMenu(_passwordEntryMenu, this));
            this._passwordEntryMenu.Close();
        }

        public override void Open()
        {
            UIServerBrowser._selectedLobby = null;
            this._pressWait = 30;
            base.Open();
            DevConsole.SuppressDevConsole();
            this._oldPos = Mouse.positionScreen;
            this._hoverIndex = -1;
            if (!this._enteringPort)
                this.RefreshLobbySearch(UIServerBrowser.SearchMode.Near, UIServerBrowser.SearchMode.Global, UIServerBrowser.SearchMode.LAN);
            this._enteringPort = false;
        }

        public void RefreshLobbySearch() => this.RefreshLobbySearch(UIServerBrowser.SearchMode.Near, UIServerBrowser.SearchMode.Global, UIServerBrowser.SearchMode.LAN);

        public void RefreshLobbySearch(params UIServerBrowser.SearchMode[] pParts)
        {
            this._modeQueue.Clear();
            foreach (UIServerBrowser.SearchMode pPart in pParts)
                this._modeQueue.Enqueue(pPart);
            this._lobbies.Clear();
            UIServerBrowser._selectedLobby = null;
        }

        private void TryJoiningLobby(UIServerBrowser.LobbyData pLobby)
        {
            UIServerBrowser._joiningLobby = pLobby;
            if (ModLoader.modHash == UIServerBrowser._joiningLobby.modHash)
            {
                this.Close();
                this._attemptConnection = UIMatchmakerMark2.Platform_GetMatchkmaker(pLobby, this);
                this._attemptConnection.SetPasswordAttempt(this.enteredPassword);
                this.enteredPassword = "";
                Level.Add(_attemptConnection);
                this._attemptConnection.Open();
                MonoMain.pauseMenu = _attemptConnection;
            }
            else
            {
                MonoMain.lobbyPassword = this.enteredPassword;
                new UIMenuActionOpenMenu(this, _downloadModsMenu).Activate();
                this.enteredPassword = "";
            }
        }

        private void UpdateLobbySearch()
        {
            if (!this._searching && this.mode != UIServerBrowser.SearchMode.None && DuckGame.Graphics.frame >= this._lobbySearchCooldownNextAvailable)
            {
                this._lobbySearchCooldownNextAvailable = DuckGame.Graphics.frame;
                Network.lanMode = this.mode == UIServerBrowser.SearchMode.LAN;
                NCBasic.lobbySearchPort = this.lanSearchPort;
                if (this.mode == UIServerBrowser.SearchMode.Global)
                    NCSteam.globalSearch = true;
                UIServerBrowser._selectedLobby = null;
                Network.activeNetwork.core.SearchForLobby();
                this._searching = true;
            }
            if (!this._searching || this.mode == UIServerBrowser.SearchMode.None || !Network.activeNetwork.core.IsLobbySearchComplete())
                return;
            this._searching = false;
            int num1 = Network.activeNetwork.core.NumLobbiesFound();
            List<WorkshopItem> items = new List<WorkshopItem>();
            if (Network.lanMode)
            {
                foreach (UIServerBrowser.LobbyData foundLobby in (Network.activeNetwork.core as NCBasic)._foundLobbies)
                    this._lobbies.Add(foundLobby);
            }
            else
            {
                for (int i = 0; i < num1; ++i)
                {
                    Lobby lobby = Network.activeNetwork.core.GetSearchLobbyAtIndex(i);
                    if (this._lobbies.FirstOrDefault<UIServerBrowser.LobbyData>(x => x.lobby != null && (long)x.lobby.id == (long)lobby.id) == null)
                    {
                        string lobbyData1 = lobby.GetLobbyData("name");
                        if (!string.IsNullOrEmpty(lobbyData1))
                        {
                            UIServerBrowser.LobbyData lobbyData2 = new UIServerBrowser.LobbyData
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
                                lobbyData2.numSlots = Convert.ToInt32(lobby.GetLobbyData("numSlots"));
                            }
                            catch (Exception ex)
                            {
                                lobbyData2.numSlots = 0;
                            }
                            lobbyData2.hasModifiers = lobby.GetLobbyData("modifiers");
                            lobbyData2.hasPassword = lobby.GetLobbyData("password") == "true";
                            lobbyData2.dedicated = lobby.GetLobbyData("dedicated") == "true";
                            lobbyData2.pingstring = lobby.GetLobbyData("pingstring");
                            if (lobbyData2.pingstring != "" && lobbyData2.pingstring != null)
                                lobbyData2.estimatedPing = Steam.EstimatePing(lobbyData2.pingstring);
                            try
                            {
                                lobbyData2.datahash = Convert.ToInt64(lobby.GetLobbyData("datahash"));
                            }
                            catch (Exception ex)
                            {
                            }
                            lobbyData2.isGlobalLobby = this.mode == UIServerBrowser.SearchMode.Global;
                            lobbyData2.hasFriends = false;
                            foreach (User user in lobby.users)
                            {
                                if (Steam.friends.Contains(user))
                                {
                                    lobbyData2.hasFriends = true;
                                    break;
                                }
                            }
                            string lobbyData3 = lobby.GetLobbyData("mods");
                            if (lobbyData3 != null && lobbyData3 != "")
                            {
                                string str1 = lobbyData3;
                                char[] chArray = new char[1] { '|' };
                                foreach (string str2 in str1.Split(chArray))
                                {
                                    try
                                    {
                                        if (!(str2 == ""))
                                        {
                                            if (str2 == "LOCAL")
                                            {
                                                lobbyData2.hasLocalMods = true;
                                            }
                                            else
                                            {
                                                string[] strArray = str2.Split(',');
                                                WorkshopItem workshopItem = WorkshopItem.GetItem(Convert.ToUInt64(strArray.Length != 2 ? str2 : strArray[0].Trim()));
                                                if (workshopItem != null)
                                                {
                                                    items.Add(workshopItem);
                                                    lobbyData2.workshopItems.Add(workshopItem);
                                                }
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                    }
                                }
                            }
                            try
                            {
                                lobbyData2.maxPlayers = Convert.ToInt32(lobby.GetLobbyData("maxplayers"));
                            }
                            catch (Exception)
                            {
                                lobbyData2.maxPlayers = 0;
                            }
                            this._lobbies.Add(lobbyData2);
                        }
                    }
                }
            }
            if (items.Count > 0)
                Steam.RequestWorkshopInfo(items);
            int num2 = (int)this._modeQueue.Dequeue();
        }

        public override void Update()
        {
            if (this.open)
            {
                if (this._passwordLobby != null && this._passwordLobby.hasPassword && this.enteredPassword != "")
                {
                    this.TryJoiningLobby(this._passwordLobby);
                    this._passwordLobby = null;
                    return;
                }
                this.UpdateLobbySearch();
            }
            if (this._pressWait > 0)
                --this._pressWait;
            if (this._downloadModsMenu.open)
            {
                this._downloadModsMenu.DoUpdate();
                if (!UIMenu.globalUILock && (Input.Pressed("CANCEL") || Keyboard.Pressed(Keys.Escape)))
                {
                    this._downloadModsMenu.Close();
                    this.Open();
                    return;
                }
            }
            else if (this.open)
            {
                MonoMain.lobbyPassword = null;
                if (this._gamepadMode)
                {
                    if (this._hoverIndex < 0)
                        this._hoverIndex = 0;
                }
                else
                {
                    this._hoverIndex = -1;
                    for (int index = 0; index < this._maxLobbiesToShow && this._scrollItemOffset + index < this._lobbies.Count; ++index)
                    {
                        if (new Rectangle((int)(this._box.x - this._box.halfWidth), (int)(this._box.y - this._box.halfHeight + 36 * index), (int)this._box.width - 14, 36f).Contains(Mouse.position))
                        {
                            this._hoverIndex = this._scrollItemOffset + index;
                            break;
                        }
                    }
                }
                if (this._hoverIndex != -1)
                {
                    if (Input.Pressed("MENU1"))
                    {
                        this.RefreshLobbySearch(UIServerBrowser.SearchMode.Near, UIServerBrowser.SearchMode.Global, UIServerBrowser.SearchMode.LAN);
                        SFX.Play("rockHitGround", 0.8f);
                    }
                    else if (Input.Pressed("MENU2") || this.enteredPort != "")
                    {
                        if (this.enteredPort == "")
                        {
                            this._enteringPort = true;
                            this._portEntryMenu.SetValue(this.lanSearchPort.ToString());
                            new UIMenuActionOpenMenu(this, _portEntryMenu).Activate();
                        }
                        else
                        {
                            try
                            {
                                this.lanSearchPort = Convert.ToInt32(this.enteredPort);
                            }
                            catch (Exception)
                            {
                            }
                            this.enteredPort = "";
                            this.RefreshLobbySearch(UIServerBrowser.SearchMode.LAN);
                            SFX.Play("rockHitGround", 0.8f);
                        }
                    }
                    if (this._lobbies.Count > 0 && this._hoverIndex < this._lobbies.Count)
                    {
                        UIServerBrowser._selectedLobby = this._lobbies[this._hoverIndex];
                        if (Input.Pressed("SELECT") && this._pressWait == 0 && this._gamepadMode || !this._gamepadMode && Mouse.left == InputState.Pressed || this.enteredPassword != "")
                        {
                            if (!UIServerBrowser._selectedLobby.canJoin)
                            {
                                SFX.Play("consoleError");
                            }
                            else
                            {
                                SFX.Play("consoleSelect");
                                if (UIServerBrowser._selectedLobby.hasPassword && this.enteredPassword == "")
                                {
                                    this._passwordLobby = UIServerBrowser._selectedLobby;
                                    new UIMenuActionOpenMenu(this, _passwordEntryMenu).Activate();
                                }
                                else
                                {
                                    this.TryJoiningLobby(UIServerBrowser._selectedLobby);
                                    return;
                                }
                            }
                        }
                    }
                }
                else
                    UIServerBrowser._selectedLobby = null;
                if (this._gamepadMode)
                {
                    this._draggingScrollbar = false;
                    if (Input.Pressed("MENUDOWN"))
                        ++this._hoverIndex;
                    else if (Input.Pressed("MENUUP"))
                        --this._hoverIndex;
                    if (Input.Pressed("STRAFE"))
                        this._hoverIndex -= 10;
                    else if (Input.Pressed("RAGDOLL"))
                        this._hoverIndex += 10;
                    if (this._hoverIndex < 0)
                        this._hoverIndex = 0;
                    if ((double)(this._oldPos - Mouse.positionScreen).lengthSq > 200.0)
                        this._gamepadMode = false;
                }
                else
                {
                    if (!this._draggingScrollbar)
                    {
                        if (Mouse.left == InputState.Pressed && this.ScrollBarBox().Contains(Mouse.position))
                        {
                            this._draggingScrollbar = true;
                            this._oldPos = Mouse.position;
                        }
                        if ((double)Mouse.scroll > 0.0)
                        {
                            this._scrollItemOffset += 5;
                            this._hoverIndex += 5;
                        }
                        else if ((double)Mouse.scroll < 0.0)
                        {
                            this._scrollItemOffset -= 5;
                            this._hoverIndex -= 5;
                            if (this._hoverIndex < 0)
                                this._hoverIndex = 0;
                        }
                    }
                    else if (Mouse.left != InputState.Down)
                    {
                        this._draggingScrollbar = false;
                    }
                    else
                    {
                        Vec2 vec2 = Mouse.position - this._oldPos;
                        this._oldPos = Mouse.position;
                        this.scrollBarOffset += (int)vec2.y;
                        if (this.scrollBarOffset > this.scrollBarScrollableHeight)
                            this.scrollBarOffset = this.scrollBarScrollableHeight;
                        else if (this.scrollBarOffset < 0)
                            this.scrollBarOffset = 0;
                        this._scrollItemOffset = (int)((this._lobbies.Count - this._maxLobbiesToShow) * (double)(scrollBarOffset / (float)this.scrollBarScrollableHeight));
                    }
                    if (Input.Pressed("ANY"))
                    {
                        this._gamepadMode = true;
                        this._oldPos = Mouse.positionScreen;
                    }
                }
                if (this._scrollItemOffset < 0)
                    this._scrollItemOffset = 0;
                else if (this._scrollItemOffset > Math.Max(0, this._lobbies.Count - this._maxLobbiesToShow))
                    this._scrollItemOffset = Math.Max(0, this._lobbies.Count - this._maxLobbiesToShow);
                if (this._hoverIndex >= this._lobbies.Count)
                    this._hoverIndex = this._lobbies.Count - 1;
                else if (this._hoverIndex >= this._scrollItemOffset + this._maxLobbiesToShow)
                    this._scrollItemOffset += this._hoverIndex - (this._scrollItemOffset + this._maxLobbiesToShow) + 1;
                else if (this._hoverIndex >= 0 && this._hoverIndex < this._scrollItemOffset)
                    this._scrollItemOffset -= this._scrollItemOffset - this._hoverIndex;
                this.scrollBarOffset = this._scrollItemOffset == 0 ? 0 : (int)Lerp.FloatSmooth(0.0f, scrollBarScrollableHeight, _scrollItemOffset / (float)(this._lobbies.Count - this._maxLobbiesToShow));
                if (!Editor.hoverTextBox && !UIMenu.globalUILock && (Input.Pressed("CANCEL") || Keyboard.Pressed(Keys.Escape)))
                {
                    new UIMenuActionOpenMenu(this, _openOnClose).Activate();
                    return;
                }
            }
            if (this._showingMenu)
            {
                HUD.CloseAllCorners();
                this._showingMenu = false;
            }
            base.Update();
        }

        private Rectangle ScrollBarBox() => new Rectangle((float)((double)this._box.x + (double)this._box.halfWidth - 12.0 + 1.0), (float)((double)this._box.y - (double)this._box.halfHeight + 1.0) + scrollBarOffset, 10f, 32f);

        private void Completed(object sender, AsyncCompletedEventArgs e)
        {
            if (!UIServerBrowser._clientMap.ContainsKey(sender))
                return;
            ulong client = UIServerBrowser._clientMap[sender];
            UIServerBrowser._clientMap.Remove(sender);
            if (!UIServerBrowser._previewMap.ContainsKey(client))
                return;
            Texture2D texture2D = ContentPack.LoadTexture2D(UIServerBrowser.PreviewPathForWorkshopItem(client), false);
            if (texture2D == null)
                return;
            Tex2D tex2D = (Tex2D)texture2D;
            if (tex2D == null)
                return;
            UIServerBrowser._previewMap[client] = tex2D;
        }

        public static string PreviewPathForWorkshopItem(ulong id) => DuckFile.workshopDirectory + "/modPreview" + id.ToString() + "preview.png";

        public override void Draw()
        {
            if (this._downloadModsMenu.open)
                this._downloadModsMenu.DoDraw();
            if (this.open)
            {
                this.scrollBarTop = (int)((double)this._box.y - (double)this._box.halfHeight + 1.0 + 16.0);
                this.scrollBarBottom = (int)((double)this._box.y + (double)this._box.halfHeight - 1.0 - 16.0);
                this.scrollBarScrollableHeight = this.scrollBarBottom - this.scrollBarTop;
                if (this.fixView)
                {
                    Layer.HUD.camera.width *= 2f;
                    Layer.HUD.camera.height *= 2f;
                    this.fixView = false;
                }
                DuckGame.Graphics.DrawRect(new Vec2(this._box.x - this._box.halfWidth, this._box.y - this._box.halfHeight), new Vec2((float)((double)this._box.x + (double)this._box.halfWidth - 12.0 - 2.0), this._box.y + this._box.halfHeight), Color.Black, (Depth)0.4f);
                DuckGame.Graphics.DrawRect(new Vec2((float)((double)this._box.x + (double)this._box.halfWidth - 12.0), this._box.y - this._box.halfHeight), new Vec2(this._box.x + this._box.halfWidth, this._box.y + this._box.halfHeight), Color.Black, (Depth)0.4f);
                Rectangle r = this.ScrollBarBox();
                DuckGame.Graphics.DrawRect(r, this._draggingScrollbar || r.Contains(Mouse.position) ? Color.LightGray : Color.Gray, (Depth)0.5f);
                if (this._lobbies.Count == 0)
                {
                    float num1 = this._box.x - this._box.halfWidth;
                    float num2 = this._box.y - this._box.halfHeight;
                    if (this.mode == UIServerBrowser.SearchMode.None)
                        this._fancyFont.Draw("No games found!", new Vec2(num1 + 10f, num2 + 2f), Color.Yellow, (Depth)0.5f);
                    else
                        this._fancyFont.Draw("Waiting for game list.", new Vec2(num1 + 10f, num2 + 2f), Colors.DGGreen, (Depth)0.5f);
                }
                if (this.mode != UIServerBrowser.SearchMode.None)
                {
                    float x = (float)((double)this._box.x - (double)this._box.halfWidth + 116.0);
                    float y = this._splitter.topSection.y - 5f;
                    this._refreshingDots += 0.01f;
                    if (_refreshingDots > 1.0)
                        this._refreshingDots = 0.0f;
                    string str = "(REFRESHING";
                    for (int index = 0; index < 3; ++index)
                    {
                        if (_refreshingDots * 4.0 > index + 1)
                            str += ".";
                    }
                    this._fancyFont.Draw(str + ")", new Vec2(x, y), Colors.DGGreen, (Depth)0.5f);
                }
                this._lobbies.Sort();
                for (int index1 = 0; index1 < this._maxLobbiesToShow; ++index1)
                {
                    int index2 = this._scrollItemOffset + index1;
                    if (index2 < this._lobbies.Count)
                    {
                        float x1 = this._box.x - this._box.halfWidth;
                        float y = this._box.y - this._box.halfHeight + 36 * index1;
                        if (this._hoverIndex == index2)
                            DuckGame.Graphics.DrawRect(new Vec2(x1, y), new Vec2((float)((double)x1 + (double)this._box.width - 14.0), y + 36f), Color.White * 0.6f, (Depth)0.4f);
                        else if ((index2 & 1) != 0)
                            DuckGame.Graphics.DrawRect(new Vec2(x1, y), new Vec2((float)((double)x1 + (double)this._box.width - 14.0), y + 36f), Color.White * 0.1f, (Depth)0.4f);
                        UIServerBrowser.LobbyData lobby = this._lobbies[index2];
                        if (lobby != null)
                        {
                            this._noImage.texture = this.defaultImage;
                            if (lobby.lobby == null)
                                this._noImage.texture = this.defaultImageLan;
                            this._noImage.scale = new Vec2(1f, 1f);
                            List<Tex2D> tex2DList = new List<Tex2D>();
                            string titleString = lobby.name;
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
                                    lobby.workshopItems = lobby.workshopItems.OrderByDescending<WorkshopItem, int>(x => x.data == null ? 0 : x.data.votesUp).ToList<WorkshopItem>();
                                    if (!lobby.downloadedWorkshopItems)
                                    {
                                        lobby.hasFirstMod = true;
                                        lobby.hasRestOfMods = true;
                                        bool flag = true;
                                        foreach (WorkshopItem workshopItem2 in lobby.workshopItems)
                                        {
                                            ulong id = workshopItem2.id;
                                            if (ModLoader.accessibleMods.FirstOrDefault<Mod>(x => (long)x.configuration.workshopID == (long)id) == null)
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
                                    string str3 = lobby.hasRestOfMods ? "|DGGREEN|" : "|RED|";
                                    if (lobby.workshopItems.Count == 2)
                                        str2 = str2 + str3 + " +" + (lobby.workshopItems.Count - 1).ToString() + " other mod.";
                                    else if (lobby.workshopItems.Count > 2)
                                        str2 = str2 + str3 + " +" + (lobby.workshopItems.Count - 1).ToString() + " other mods.";
                                    text1 = str2 + "\n|GRAY|";
                                    if (!UIServerBrowser._previewMap.ContainsKey(workshopItem1.id))
                                    {
                                        if (workshopItem1.data.previewPath != null)
                                        {
                                            if (workshopItem1.data.previewPath != "")
                                            {
                                                try
                                                {
                                                    WebClient key = new WebClient();
                                                    string str4 = UIServerBrowser.PreviewPathForWorkshopItem(workshopItem1.id);
                                                    DuckFile.CreatePath(str4);
                                                    if (System.IO.File.Exists(str4))
                                                        DuckFile.Delete(str4);
                                                    key.DownloadFileAsync(new Uri(workshopItem1.data.previewPath), str4);
                                                    key.DownloadFileCompleted += new AsyncCompletedEventHandler(this.Completed);
                                                    UIServerBrowser._clientMap[key] = workshopItem1.id;
                                                }
                                                catch (Exception)
                                                {
                                                }
                                            }
                                        }
                                        UIServerBrowser._previewMap[workshopItem1.id] = null;
                                    }
                                    else
                                    {
                                        Tex2D preview = UIServerBrowser._previewMap[workshopItem1.id];
                                        if (preview != null)
                                            tex2DList.Add(preview);
                                    }
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
                            DuckGame.Graphics.DrawRect(new Vec2(x1 + 2f, y + 2f), new Vec2((float)((double)x1 + 36.0 - 2.0), (float)((double)y + 36.0 - 2.0)), Color.Gray, (Depth)0.5f, false, 2f);
                            if (tex2DList.Count > 0)
                            {
                                Vec2 zero = Vec2.Zero;
                                for (int index3 = 0; index3 < 4; ++index3)
                                {
                                    if (index3 < tex2DList.Count)
                                    {
                                        this._noImage.texture = tex2DList[index3];
                                        if (tex2DList.Count > 1)
                                            this._noImage.scale = new Vec2(16f / _noImage.texture.width);
                                        else
                                            this._noImage.scale = new Vec2(32f / _noImage.texture.width);
                                        if (this._noImage.texture.width != this._noImage.texture.height)
                                        {
                                            if (this._noImage.texture.width > this._noImage.texture.height)
                                            {
                                                this._noImage.scale = new Vec2(32f / _noImage.texture.height);
                                                DuckGame.Graphics.Draw(this._noImage, x1 + 2f + zero.x, y + 2f + zero.y, new Rectangle(this._noImage.texture.width / 2 - this._noImage.texture.height / 2, 0.0f, _noImage.texture.height, _noImage.texture.height), (Depth)0.5f);
                                            }
                                            else
                                                DuckGame.Graphics.Draw(this._noImage, x1 + 2f + zero.x, y + 2f + zero.y, new Rectangle(0.0f, 0.0f, _noImage.texture.width, _noImage.texture.width), (Depth)0.5f);
                                        }
                                        else
                                            DuckGame.Graphics.Draw(this._noImage, x1 + 2f + zero.x, y + 2f + zero.y, (Depth)0.5f);
                                        zero.x += 16f;
                                        if (zero.x >= 32.0)
                                        {
                                            zero.x = 0.0f;
                                            zero.y += 16f;
                                        }
                                    }
                                }
                            }
                            else
                                DuckGame.Graphics.Draw(this._noImage, x1 + 2f, y + 2f, (Depth)0.5f);
                            titleString += " (" + Math.Min(lobby.userCount - (lobby.dedicated ? 1 : 0), 8).ToString() + "/" + Math.Min(lobby.numSlots, 8).ToString() + ")";
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
                                    else if (lobby.userCount >= lobby.numSlots)
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
                                DuckGame.Graphics.DrawRect(new Vec2(x1, y), new Vec2((float)((double)x1 + (double)this._box.width - 14.0), y + 36f), Color.Black * 0.5f, (Depth)0.99f);
                            }
                            this._fancyFont.maxWidth = 1000;
                            float num = 0.0f;
                            if (lobby.hasPassword)
                            {
                                DuckGame.Graphics.Draw(this._lockedServer, (float)((double)x1 + 36.0 + 10.0), y + 2.5f, (Depth)0.5f);
                                num += 10f;
                            }
                            if (lobby.hasCustomName)
                            {
                                DuckGame.Graphics.Draw(this._namedServer, (float)((double)x1 + (double)num + 36.0 + 10.0), y + 2.5f, (Depth)0.5f);
                                num += 10f;
                            }
                            if (lobby.isGlobalLobby)
                            {
                                DuckGame.Graphics.Draw(this._globeIcon, (float)((double)x1 + (double)num + 36.0 + 10.0), y + 2.5f, (Depth)0.5f);
                                num += 10f;
                            }
                            this._fancyFont.Draw(titleString, new Vec2((float)((double)x1 + 36.0 + (double)num + 10.0), y + 2f), Color.Yellow, (Depth)0.5f);
                            if (lobby.version == DG.version)
                                this._fancyFont.Draw(lobby.version, new Vec2((float)((double)x1 + 430.0 + 10.0), y + 2f), Colors.DGGreen * 0.45f, (Depth)0.5f);
                            else
                                this._fancyFont.Draw(lobby.version, new Vec2((float)((double)x1 + 430.0 + 10.0), y + 2f), Colors.DGRed * 0.45f, (Depth)0.5f);
                            this._fancyFont.Draw("|WHITE|Ping:", new Vec2(x1 + 440f, y + 26f), Color.White * 0.45f, (Depth)0.5f);
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
                                this._fancyFont.Draw(lobby.estimatedPing.ToString() + "ms", new Vec2(x1 + 470f, y + 26f), color * 0.45f, (Depth)0.5f);
                            }
                            else
                                this._fancyFont.Draw("????ms", new Vec2(x1 + 470f, y + 26f), Colors.DGRed * 0.45f, (Depth)0.5f);
                            if (lobby.lobby != null)
                                DuckGame.Graphics.Draw(this._steamIcon, x1 + 36f, y + 2.5f, (Depth)0.5f);
                            else
                                DuckGame.Graphics.Draw(this._lanIcon, x1 + 36f, y + 2.5f, (Depth)0.5f);
                            this._fancyFont.Draw(text1, new Vec2(x1 + 36f, y + 6f + _fancyFont.characterHeight), Color.LightGray, (Depth)0.5f);
                        }
                    }
                    else
                        break;
                }
                if (Mouse.available && !this._gamepadMode)
                {
                    this._cursor.depth = (Depth)1f;
                    this._cursor.scale = new Vec2(1f, 1f);
                    this._cursor.position = Mouse.position;
                    this._cursor.frame = 0;
                    if (Editor.hoverTextBox)
                    {
                        this._cursor.frame = 5;
                        this._cursor.position.y -= 4f;
                        this._cursor.scale = new Vec2(0.5f, 1f);
                    }
                    this._cursor.Draw();
                }
            }
            base.Draw();
        }

        public enum SearchMode
        {
            None,
            Near,
            Global,
            LAN,
        }

        public class LobbyData : IComparable<UIServerBrowser.LobbyData>
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
                    return DG.version == this.version && (Network.gameDataHash == this.datahash || ModLoader.modHash != this.modHash) && this.started == "false" &&
                        (!this.hasLocalMods || ModLoader.modHash == this.modHash) && this.userCount < this.numSlots && (this.type == "2" || this.lobby == null);
                }
            }

            public int userCount => this.lobby != null ? this.lobby.users.Count : this._userCount;

            public int CompareTo(UIServerBrowser.LobbyData other)
            {
                if (this.isGlobalLobby && !other.isGlobalLobby)
                    return 10000;
                if (!this.isGlobalLobby && other.isGlobalLobby)
                    return -10000;
                if (this.canJoin && !other.canJoin)
                    return -500;
                if (!this.canJoin && other.canJoin)
                    return 500;
                if (this.hasFriends && !other.hasFriends)
                    return -100;
                if (!this.hasFriends && other.hasFriends)
                    return 100;
                if (!this.hasPassword && other.hasPassword)
                    return -10;
                if (this.hasPassword && !other.hasPassword)
                    return 10;
                if (this.lobby == null && other.lobby != null)
                    return -1000;
                if (this.lobby != null && other.lobby == null)
                    return 1000;
                if (this.dedicated && !other.dedicated)
                    return 1;
                if (!this.dedicated && other.dedicated)
                    return -1;
                if (this.estimatedPing < other.estimatedPing)
                    return -2;
                return this.estimatedPing > other.estimatedPing ? 2 : this.name.CompareTo(other.name);
            }
        }
    }
}