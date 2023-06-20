using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace DuckGame
{
    public class GameMode
    {
        private static GameModeCore _core = new GameModeCore();
        public static List<Profile> lastWinners = new List<Profile>();
        protected UIComponent _pauseGroup;
        protected UIMenu _pauseMenu;
        protected UIMenu _confirmMenu;
        protected UIMenu _confirmBlacklistMenu;
        protected UIMenu _confirmReturnToLobby;
        protected MenuBoolean _returnToLobby = new MenuBoolean();
        protected bool _matchOver;
        protected bool _paused;
        protected MenuBoolean _quit = new MenuBoolean();
        protected float _wait;
        protected float _roundEndWait = 1f;
        protected bool _doScore;
        protected bool _addedPoints;
        protected bool _endedHighlights;
        protected bool _switchedLevel;
        protected bool _roundHadWinner;
        protected float _waitFade = 1f;
        protected float _waitSpawn = 1.8f;
        protected float _waitAfterSpawn;
        protected int _waitAfterSpawnDings;
        protected float _fontFade = 1f;
        protected List<Duck> _pendingSpawns;
        protected BitmapFont _font;
        protected bool _editorTestMode;
        protected bool _validityTest;
        private Stopwatch _watch;
        private long frames;
        public bool skippedLevel;
        protected float _pulse;

        public static GameModeCore core
        {
            get => _core;
            set => _core = value;
        }

        public static int roundsBetweenIntermission
        {
            get => _core.roundsBetweenIntermission;
            set => _core.roundsBetweenIntermission = value;
        }

        public static int winsPerSet
        {
            get => _core.winsPerSet;
            set => _core.winsPerSet = value;
        }

        protected static bool _started
        {
            get => _core._started;
            set => _core._started = value;
        }

        public static bool started => _started;

        public static bool getReady
        {
            get => _core.getReady;
            set => _core.getReady = value;
        }

        protected static int _numMatchesPlayed
        {
            get => _core._numMatchesPlayed;
            set => _core._numMatchesPlayed = value;
        }

        public static int numMatchesPlayed
        {
            get => _numMatchesPlayed;
            set => _numMatchesPlayed = value;
        }

        public static bool showdown
        {
            get => _core.showdown;
            set => _core.showdown = value;
        }

        public static string previousLevel
        {
            get => _core.previousLevel;
            set => _core.previousLevel = value;
        }

        protected static string _currentMusic
        {
            get => _core._currentMusic;
            set => _core._currentMusic = value;
        }

        public static bool firstDead
        {
            get => _core.firstDead;
            set => _core.firstDead = value;
        }

        public static bool playedGame
        {
            get => _core.playedGame;
            set => _core.playedGame = value;
        }

        public bool matchOver => _matchOver;

        public List<Duck> pendingSpawns
        {
            get => _pendingSpawns;
            set => _pendingSpawns = value;
        }

        public GameMode(bool validityTest = false, bool editorTestMode = false)
        {
            _validityTest = validityTest;
            _editorTestMode = editorTestMode;
        }

        public static void Subscribe()
        {
            if (!(Level.current is GameLevel) || (Level.current as GameLevel).data == null || (Level.current as GameLevel).data.metaData.workshopID == 0UL)
                return;
            WorkshopItem workshopItem = WorkshopItem.GetItem((Level.current as GameLevel).data.metaData.workshopID);
            if ((workshopItem.stateFlags & WorkshopItemState.Subscribed) != WorkshopItemState.None)
                Steam.WorkshopUnsubscribe(workshopItem.id);
            else
                Steam.WorkshopSubscribe(workshopItem.id);
        }

        public static void View()
        {
            if (!(Level.current is GameLevel) || (Level.current as GameLevel).data == null || (Level.current as GameLevel).data.metaData.workshopID == 0UL)
                return;
            Steam.OverlayOpenURL("https://steamcommunity.com/sharedfiles/filedetails/?id=" + (Level.current as GameLevel).data.metaData.workshopID.ToString());
        }

        public static void Blacklist()
        {
            if (Level.current is GameLevel && (Level.current as GameLevel).data != null && (Level.current as GameLevel).data.metaData.workshopID != 0UL)
                Global.data.blacklist.Add((Level.current as GameLevel).data.metaData.workshopID);
            Skip();
        }

        public static void Skip()
        {
            if (!(Level.current is GameLevel))
                return;
            (Level.current as GameLevel).SkipMatch();
        }

        public void DoInitialize()
        {
            _started = false;
            firstDead = false;
            playedGame = true;
            if (!_editorTestMode)
                Highlights.StartRound();
            _font = new BitmapFont("biosFont", 8);
            if (Network.isServer)
            {
                string str = Music.RandomTrack("InGame", Music.currentSong);
                Music.LoadAlternateSong(str);
                Music.CancelLooping();
                if (Network.isActive)
                    Send.Message(new NMSwitchMusic(str));
            }
            Initialize();
            if (Network.isActive)
                getReady = false;
            else
                getReady = true;
        }

        private void CreatePauseGroup()
        {
            if (_pauseGroup != null)
                Level.Remove(_pauseGroup);
            if (_editorTestMode)
            {
                _pauseGroup = new UIComponent(Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 0f, 0f);
                _pauseMenu = new UIMenu("@LWING@PAUSE@RWING@", Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 160f, conString: "@CANCEL@CLOSE @SELECT@SELECT");
                _confirmMenu = new UIMenu("REALLY QUIT?", Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 160f, conString: "@CANCEL@BACK @SELECT@SELECT");
                UIDivider component = new UIDivider(true, 0.8f);
                component.leftSection.Add(new UIMenuItem("RESUME", new UIMenuActionCloseMenu(_pauseGroup), UIAlign.Left), true);
                component.leftSection.Add(new UIMenuItem("OPTIONS", new UIMenuActionOpenMenu(_pauseMenu, Options.optionsMenu), UIAlign.Left), true);
                component.leftSection.Add(new UIText(" ", Color.White, UIAlign.Left), true);
                component.leftSection.Add(new UIMenuItem("|DGRED|QUIT", new UIMenuActionCloseMenuSetBoolean(_pauseGroup, _quit), UIAlign.Left), true);
                component.rightSection.Add(new UIImage("pauseIcons", UIAlign.Right), true);
                _pauseMenu.Add(component, true);
                _pauseMenu.Close();
                _pauseGroup.Add(_pauseMenu, false);
                Options.AddMenus(_pauseGroup);
                Options.openOnClose = _pauseMenu;
                _confirmMenu.Add(new UIMenuItem("NO!", new UIMenuActionOpenMenu(_confirmMenu, _pauseMenu), UIAlign.Left, backButton: true), true);
                _confirmMenu.Add(new UIMenuItem("YES!", new UIMenuActionCloseMenuSetBoolean(_pauseGroup, _quit)), true);
                _confirmMenu.Close();
                _pauseGroup.Add(_confirmMenu, false);
                _pauseGroup.isPauseMenu = true;
                Level.Add(_pauseGroup);
                _pauseGroup.Update();
                _pauseGroup.Update();
                _pauseGroup.Close();
                _confirmMenu.Update();
            }
            else
            {
                _pauseGroup = new UIComponent(Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 0f, 0f);
                _pauseMenu = new UIMenu("@LWING@PAUSE@RWING@", Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 160f, conString: "@CANCEL@CLOSE @SELECT@SELECT");
                _confirmMenu = new UIMenu("REALLY QUIT?", Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 160f, conString: "@CANCEL@BACK @SELECT@SELECT");
                _confirmBlacklistMenu = new UIMenu("AVOID LEVEL?", Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 160f, conString: "@CANCEL@BACK @SELECT@SELECT");
                _confirmReturnToLobby = new UIMenu("RETURN TO LOBBY?", Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 230f, conString: "@CANCEL@BACK @SELECT@SELECT");
                UIDivider component1 = new UIDivider(true, 0.8f);
                component1.leftSection.Add(new UIMenuItem("RESUME", new UIMenuActionCloseMenu(_pauseGroup), UIAlign.Left), true);
                component1.leftSection.Add(new UIMenuItem("OPTIONS", new UIMenuActionOpenMenu(_pauseMenu, Options.optionsMenu), UIAlign.Left), true);
                component1.leftSection.Add(new UIText(" ", Color.White, UIAlign.Left), true);
                component1.leftSection.Add(new UIMenuItem("|DGRED|BACK TO LOBBY", new UIMenuActionOpenMenu(_pauseMenu, _confirmReturnToLobby), UIAlign.Left), true);
                component1.leftSection.Add(new UIMenuItem("|DGRED|QUIT", new UIMenuActionOpenMenu(_pauseMenu, _confirmMenu), UIAlign.Left), true);
                _confirmReturnToLobby.Add(new UIText("YOUR CURRENT GAME", Color.White), true);
                _confirmReturnToLobby.Add(new UIText("WILL BE CANCELLED.", Color.White), true);
                _confirmReturnToLobby.Add(new UIMenuItem("NO!", new UIMenuActionOpenMenu(_confirmReturnToLobby, _pauseMenu), UIAlign.Left, backButton: true), true);
                _confirmReturnToLobby.Add(new UIMenuItem("YES!", new UIMenuActionCloseMenuSetBoolean(_pauseGroup, _returnToLobby)), true);
                _confirmReturnToLobby.Close();
                _pauseGroup.Add(_confirmReturnToLobby, false);
                if (Level.current.isCustomLevel || Level.current is RandomLevel)
                {
                    component1.leftSection.Add(new UIText(" ", Color.White), true);
                    if ((Level.current as GameLevel).data.metaData.workshopID != 0UL)
                    {
                        WorkshopItem workshopItem = WorkshopItem.GetItem((Level.current as GameLevel).data.metaData.workshopID);
                        if (workshopItem != null)
                        {
                            component1.leftSection.Add(new UIMenuItem("@STEAMICON@|DGGREEN|VIEW", new UIMenuActionCallFunction(new UIMenuActionCallFunction.Function(View)), UIAlign.Left), true);
                            if ((workshopItem.stateFlags & WorkshopItemState.Subscribed) != WorkshopItemState.None)
                            {
                                component1.leftSection.Add(new UIMenuItem("@STEAMICON@|DGRED|UNSUBSCRIBE", new UIMenuActionCloseMenuCallFunction(_pauseGroup, new UIMenuActionCloseMenuCallFunction.Function(Subscribe)), UIAlign.Left), true);
                            }
                            else
                            {
                                component1.leftSection.Add(new UIMenuItem("@STEAMICON@|DGGREEN|SUBSCRIBE", new UIMenuActionCloseMenuCallFunction(_pauseGroup, new UIMenuActionCloseMenuCallFunction.Function(Subscribe)), UIAlign.Left), true);
                                component1.leftSection.Add(new UIMenuItem("@blacklist@|DGRED|NEVER AGAIN", new UIMenuActionOpenMenu(_pauseMenu, _confirmBlacklistMenu), UIAlign.Left), true);
                            }
                        }
                    }
                    if (!_matchOver && Network.isServer)
                        component1.leftSection.Add(new UIMenuItem("@SKIPSPIN@|DGRED|SKIP", new UIMenuActionCloseMenuCallFunction(_pauseGroup, new UIMenuActionCloseMenuCallFunction.Function(Skip)), UIAlign.Left), true);
                }
                component1.rightSection.Add(new UIImage("pauseIcons", UIAlign.Right), true);
                _pauseMenu.Add(component1, true);
                _pauseMenu.Close();
                _pauseGroup.Add(_pauseMenu, false);
                Options.AddMenus(_pauseGroup);
                Options.openOnClose = _pauseMenu;
                _confirmMenu.Add(new UIMenuItem("NO!", new UIMenuActionOpenMenu(_confirmMenu, _pauseMenu), UIAlign.Left, backButton: true), true);
                _confirmMenu.Add(new UIMenuItem("YES!", new UIMenuActionCloseMenuSetBoolean(_pauseGroup, _quit)), true);
                _confirmMenu.Close();
                _pauseGroup.Add(_confirmMenu, false);
                UIMenu confirmBlacklistMenu1 = _confirmBlacklistMenu;
                UIText component2 = new UIText("", Color.White, heightAdd: -4f)
                {
                    scale = new Vec2(0.5f)
                };
                confirmBlacklistMenu1.Add(component2, true);
                UIMenu confirmBlacklistMenu2 = _confirmBlacklistMenu;
                UIText component3 = new UIText("Are you sure you want to avoid", Color.White, heightAdd: -4f)
                {
                    scale = new Vec2(0.5f)
                };
                confirmBlacklistMenu2.Add(component3, true);
                UIMenu confirmBlacklistMenu3 = _confirmBlacklistMenu;
                UIText component4 = new UIText("this level in the future?", Color.White, heightAdd: -4f)
                {
                    scale = new Vec2(0.5f)
                };
                confirmBlacklistMenu3.Add(component4, true);
                UIMenu confirmBlacklistMenu4 = _confirmBlacklistMenu;
                UIText component5 = new UIText("", Color.White, heightAdd: -4f)
                {
                    scale = new Vec2(0.5f)
                };
                confirmBlacklistMenu4.Add(component5, true);
                _confirmBlacklistMenu.Add(new UIMenuItem("|DGRED|@blacklist@YES!", new UIMenuActionCloseMenuCallFunction(_pauseGroup, new UIMenuActionCloseMenuCallFunction.Function(Blacklist))), true);
                _confirmBlacklistMenu.Add(new UIMenuItem("BACK", new UIMenuActionOpenMenu(_confirmBlacklistMenu, _pauseMenu), backButton: true), true);
                _confirmBlacklistMenu.Close();
                _pauseGroup.Add(_confirmBlacklistMenu, false);
                Level.Add(_pauseGroup);
                _pauseGroup.isPauseMenu = true;
                _pauseGroup.Close();
                _pauseGroup.Update();
                _pauseGroup.Update();
                _confirmBlacklistMenu.Update();
                _confirmMenu.Update();
                _pauseMenu.Update();
                _confirmReturnToLobby.Update();
                _confirmBlacklistMenu.Update();
            }
        }

        protected virtual void Initialize() => Graphics.fade = 1f;

        public void DoStart()
        {
            if (!_editorTestMode)
            {
                ++Deathmatch.levelsSinceRandom;
                ++Deathmatch.levelsSinceWorkshop;
                ++Deathmatch.levelsSinceCustom;
                foreach (Profile profile in Profiles.active)
                    ++profile.stats.timesSpawned;
                ++Global.data.timesSpawned.valueInt;
            }
            Start();
            _started = true;
        }

        protected virtual void Start()
        {
        }

        public void DoUpdate()
        {
            if (_validityTest)
            {
                if (_watch == null)
                    _watch = new Stopwatch();
                _watch.Start();
            }
            if (Graphics.fade > 0.9f && Input.Pressed(Triggers.Start) && (!Network.isActive || Network.isFakeActive))
            {
                if (_watch != null)
                    _watch.Stop();
                CreatePauseGroup();
                _pauseGroup.DoUpdate();
                _pauseGroup.DoUpdate();
                _pauseGroup.DoUpdate();
                _pauseMenu.DoUpdate();
                _pauseGroup.Open();
                _pauseMenu.Open();
                MonoMain.pauseMenu = _pauseGroup;
                if (_paused)
                    return;
                if (!_validityTest)
                    Music.Pause();
                SFX.Play("pause", 0.6f);
                _paused = true;
            }
            else
            {
                if (_paused && MonoMain.pauseMenu == null)
                {
                    _paused = false;
                    SFX.Play("resume", 0.6f);
                    if (!_validityTest)
                        Music.Resume();
                }
                if (_returnToLobby.value && !Network.isActive)
                {
                    Level.current = new TeamSelect2(true);
                    _returnToLobby.value = false;
                }
                if (_quit.value)
                {
                    if (_editorTestMode)
                        Level.current = DuckGameTestArea.currentEditor;
                    else if (_validityTest)
                        Level.current = DeathmatchTestDialogue.currentEditor;
                    else if (Network.isActive)
                    {
                        Level.current = new DisconnectFromGame();
                    }
                    else
                    {
                        Graphics.fade -= 0.04f;
                        if (Graphics.fade >= 0.01f)
                            return;
                        Level.current = new TitleScreen();
                    }
                }
                else
                {
                    Graphics.fade = 1f;
                    if (!_validityTest && Music.finished)
                    {
                        if (Music.pendingSong != null)
                            Music.SwitchSongs();
                        else
                            PlayMusic();
                    }
                    if (Music.finished)
                        _wait -= 0.0006f;
                    _waitFade -= 0.04f;
                    if (_waitFade > 0 || !getReady || Network.isActive && Network.isClient && !Level.current.transferCompleteCalled)
                        return;
                    _waitSpawn -= 0.06f;
                    if (_waitSpawn <= 0)
                    {
                        if (Network.isServer && _pendingSpawns == null)
                            _pendingSpawns = AssignSpawns();
                        if (_pendingSpawns != null && _pendingSpawns.Count > 0)
                        {
                            _waitSpawn = 1.1f;
                            if (_pendingSpawns.Count == 1)
                                _waitSpawn = 2f;
                            Duck pendingSpawn = _pendingSpawns[0];
                            pendingSpawn.respawnPos = pendingSpawn.position;
                            pendingSpawn.localSpawnVisible = true;
                            _pendingSpawns.RemoveAt(0);
                            Vec3 color = pendingSpawn.profile.persona.color;
                            Level.Add(new SpawnLine(pendingSpawn.x, pendingSpawn.y, 0, 0f, new Color((int)color.x, (int)color.y, (int)color.z), 32f));
                            Level.Add(new SpawnLine(pendingSpawn.x, pendingSpawn.y, 0, -4f, new Color((int)color.x, (int)color.y, (int)color.z), 4f));
                            Level.Add(new SpawnLine(pendingSpawn.x, pendingSpawn.y, 0, 4f, new Color((int)color.x, (int)color.y, (int)color.z), 4f));
                            Level.Add(new SpawnAimer(pendingSpawn.x, pendingSpawn.y, 0, 4f, new Color((int)color.x, (int)color.y, (int)color.z), pendingSpawn.persona, 4f)
                            {
                                dugg = pendingSpawn
                            });
                            SFX.Play("pullPin", 0.7f);
                            if (pendingSpawn.isServerForObject && !_editorTestMode)
                            {
                                if (!Network.isActive && pendingSpawn.profile.team.name == "ZEKE")
                                {
                                    Ragdoll ragdoll = new Ragdoll(pendingSpawn.x, pendingSpawn.y, null, false, 0f, 0, Vec2.Zero);
                                    Level.Add(ragdoll);
                                    ragdoll.RunInit();
                                    ragdoll.MakeZekeBear();
                                }
                                if (Party.HasPerk(pendingSpawn.profile, PartyPerks.Present) || TeamSelect2.Enabled("WINPRES") && lastWinners.Contains(pendingSpawn.profile))
                                {
                                    Present h = new Present(pendingSpawn.x, pendingSpawn.y);
                                    Level.Add(h);
                                    pendingSpawn.GiveHoldable(h);
                                }
                                if (Party.HasPerk(pendingSpawn.profile, PartyPerks.Jetpack) || TeamSelect2.Enabled("JETTY"))
                                {
                                    Jetpack e = new Jetpack(pendingSpawn.x, pendingSpawn.y);
                                    Level.Add(e);
                                    pendingSpawn.Equip(e);
                                }
                                if (TeamSelect2.Enabled("HELMY"))
                                {
                                    Helmet e = new Helmet(pendingSpawn.x, pendingSpawn.y);
                                    Level.Add(e);
                                    pendingSpawn.Equip(e);
                                }
                                if (TeamSelect2.Enabled("SHOESTAR"))
                                {
                                    Boots e = new Boots(pendingSpawn.x, pendingSpawn.y);
                                    Level.Add(e);
                                    pendingSpawn.Equip(e);
                                }
                                if (DevConsole.fancyMode)
                                {
                                    FancyShoes e = new FancyShoes(pendingSpawn.x, pendingSpawn.y);
                                    Level.Add(e);
                                    pendingSpawn.Equip(e);
                                }
                                if (TeamSelect2.Enabled("DILLY"))
                                {
                                    DuelingPistol h = new DuelingPistol(pendingSpawn.x, pendingSpawn.y);
                                    Level.Add(h);
                                    pendingSpawn.GiveHoldable(h);
                                }
                                if (TeamSelect2.Enabled("COOLBOOK"))
                                {
                                    GoodBook h = new GoodBook(pendingSpawn.x, pendingSpawn.y);
                                    Level.Add(h);
                                    pendingSpawn.GiveHoldable(h);
                                }
                                if (Party.HasPerk(pendingSpawn.profile, PartyPerks.Armor))
                                {
                                    Helmet e1 = new Helmet(pendingSpawn.x, pendingSpawn.y);
                                    Level.Add(e1);
                                    pendingSpawn.Equip(e1);
                                    ChestPlate e2 = new ChestPlate(pendingSpawn.x, pendingSpawn.y);
                                    Level.Add(e2);
                                    pendingSpawn.Equip(e2);
                                }
                                if (Party.HasPerk(pendingSpawn.profile, PartyPerks.Pistol))
                                {
                                    Pistol h = new Pistol(pendingSpawn.x, pendingSpawn.y);
                                    Level.Add(h);
                                    pendingSpawn.GiveHoldable(h);
                                }
                                if (Party.HasPerk(pendingSpawn.profile, PartyPerks.NetGun))
                                {
                                    NetGun h = new NetGun(pendingSpawn.x, pendingSpawn.y);
                                    Level.Add(h);
                                    pendingSpawn.GiveHoldable(h);
                                }
                                if (TeamSelect2.QUACK3)
                                {
                                    Helmet e3 = new Helmet(pendingSpawn.x, pendingSpawn.y);
                                    Level.Add(e3);
                                    pendingSpawn.Equip(e3);
                                    ChestPlate e4 = new ChestPlate(pendingSpawn.x, pendingSpawn.y);
                                    Level.Add(e4);
                                    pendingSpawn.Equip(e4);
                                    Holster e5 = new Holster(pendingSpawn.x, pendingSpawn.y);
                                    Level.Add(e5);
                                    pendingSpawn.Equip(e5);
                                    if (pendingSpawn.profile.carryOverObject != null)
                                    {
                                        Level.Add(pendingSpawn.profile.carryOverObject);
                                        e5.SetContainedObject(pendingSpawn.profile.carryOverObject);
                                    }
                                    else
                                    {
                                        DuelingPistol h = new DuelingPistol(pendingSpawn.x, pendingSpawn.y);
                                        Level.Add(h);
                                        e5.SetContainedObject(h);
                                    }
                                }
                            }
                        }
                        else if (!_started)
                        {
                            _waitAfterSpawn -= 0.05f;
                            if (_waitAfterSpawn <= 0)
                            {
                                ++_waitAfterSpawnDings;
                                if (_waitAfterSpawnDings > 2)
                                {
                                    Party.Clear();
                                    DoStart();
                                    SFX.Play("ding");
                                    Event.Log(new RoundStartEvent());
                                    lastWinners.Clear();
                                    if (Level.current is GameLevel)
                                        (Level.current as GameLevel).MatchStart();
                                    foreach (Duck duck in Level.current.things[typeof(Duck)])
                                    {
                                        if (duck.profile.localPlayer)
                                            duck.connection = DuckNetwork.localConnection;
                                        duck.immobilized = false;
                                    }
                                }
                                else
                                    SFX.Play("preStartDing");
                                _waitSpawn = 1.1f;
                            }
                        }
                        else
                        {
                            _fontFade -= 0.1f;
                            if (_fontFade < 0)
                                _fontFade = 0f;
                        }
                    }
                    if (Network.isClient)
                        return;
                    if (_started)
                        Update();
                    if (_matchOver)
                        _roundEndWait -= 0.005f;
                    if (skippedLevel)
                        _roundEndWait = -1f;
                    if (_roundEndWait < 0.5f && !_addedPoints && !skippedLevel)
                        DoAddPoints();
                    if (_roundEndWait < 0.1f && !_endedHighlights)
                    {
                        _endedHighlights = true;
                        if (!_editorTestMode)
                            Highlights.FinishRound();
                    }
                    if (_roundEndWait >= 0f || _switchedLevel)
                        return;
                    bool flag = false;
                    if (!Network.isActive && !skippedLevel)
                    {
                        int num = 0;
                        List<Team> winning = Teams.winning;
                        if (winning.Count > 0)
                        {
                            num = winning[0].score;
                            if (Teams.active.Count > 1)
                                Global.WinLevel(winning[0]);
                        }
                        else
                        {
                            flag = true;
                            if (Teams.active.Count > 1)
                                Global.WinLevel(null);
                        }
                        if (!_editorTestMode && num > 4)
                        {
                            foreach (Team team in Teams.active)
                            {
                                if (team.score != num)
                                {
                                    if (team.score < 1)
                                    {
                                        foreach (Profile activeProfile in team.activeProfiles)
                                            Party.AddRandomPerk(activeProfile);
                                    }
                                    else if (team.score < 2 && Rando.Float(1f) > 0.3f)
                                    {
                                        foreach (Profile activeProfile in team.activeProfiles)
                                            Party.AddRandomPerk(activeProfile);
                                    }
                                    else if (team.score < 5 && Rando.Float(1f) > 0.6f)
                                    {
                                        foreach (Profile activeProfile in team.activeProfiles)
                                            Party.AddRandomPerk(activeProfile);
                                    }
                                    else if (team.score < 7 && Rando.Float(1f) > 0.85f)
                                    {
                                        foreach (Profile activeProfile in team.activeProfiles)
                                            Party.AddRandomPerk(activeProfile);
                                    }
                                    else if (team.score < num && Rando.Float(1f) > 0.9f)
                                    {
                                        foreach (Profile activeProfile in team.activeProfiles)
                                            Party.AddRandomPerk(activeProfile);
                                    }
                                }
                            }
                        }
                    }
                    Level nextLevel = GetNextLevel();
                    previousLevel = nextLevel.level;
                    if (!skippedLevel)
                    {
                        if (Teams.active.Count > 1)
                        {
                            if (!_editorTestMode)
                                ++Global.data.levelsPlayed;
                            if (flag && Network.isServer)
                            {
                                if (!_editorTestMode)
                                    ++Global.data.littleDraws.valueInt;
                                if (Network.isActive)
                                    Send.Message(new NMAssignDraw());
                            }
                        }
                        if (Network.isServer)
                        {
                            List<int> scrs = new List<int>();
                            foreach (Profile profile in DuckNetwork.profiles)
                            {
                                profile.ready = true;
                                if (profile.team != null)
                                {
                                    scrs.Add(profile.team.score);
                                    if (profile.connection != null)
                                        profile.ready = false;
                                }
                                else
                                    scrs.Add(0);
                            }
                            Send.Message(new NMTransferScores(scrs));
                            RunPostRound(_editorTestMode);
                        }
                    }
                    if (_validityTest && _watch != null)
                    {
                        //long elapsedMilliseconds = _watch.ElapsedMilliseconds; what -NiK0
                        if (frames / (_watch.ElapsedMilliseconds / 1000) < 30)
                        {
                            DeathmatchTestDialogue.success = false;
                            DeathmatchTestDialogue.tooSlow = true;
                        }
                        else
                            DeathmatchTestDialogue.success = true;
                        Level.current = DeathmatchTestDialogue.currentEditor;
                    }
                    else
                    {
                        if (TeamSelect2.QUACK3)
                        {
                            foreach (Profile profile in Profiles.active)
                                profile.carryOverObject = null;
                            foreach (Duck duck in Level.current.things[typeof(Duck)])
                            {
                                if (duck.GetEquipment(typeof(Holster)) is Holster equipment && equipment.containedObject != null)
                                    duck.profile.carryOverObject = equipment.containedObject;
                            }
                        }
                        if (!DuckNetwork.TryPeacefulResolution())
                        {
                            if (_doScore && !skippedLevel)
                            {
                                _doScore = false;

                                if (showdown)
                                {
                                    if (_roundHadWinner)
                                    {
                                        showdown = false;
                                        Level.current = new RockScoreboard(nextLevel, ScoreBoardMode.ShowWinner);
                                        if (!_editorTestMode)
                                            ++Global.data.drawsPlayed.valueInt;
                                        if (Network.isActive)
                                            Send.Message(new NMDrawBroken());
                                    }
                                    else
                                    {
                                        _endedHighlights = false;
                                        Level.current = nextLevel;
                                    }
                                }
                                else
                                {
                                    Level.current = new RockIntro(nextLevel);
                                    _doScore = false;
                                }

                            }
                            else
                            {
                                _endedHighlights = false;
                                Level.current = !TeamSelect2.partyMode || skippedLevel ? nextLevel : new DrinkRoom(nextLevel);
                            }

                            if (Network.isActive && Network.isServer && DGRSettings.SkipExcessRounds)
                            {
                                bool teamWon = false;
                                int winsPerS = winsPerSet;
                                int difference = roundsBetweenIntermission - numMatchesPlayed;
                                int teamsWon = 0;
                                foreach (Team team in Teams.all)
                                {
                                    if (team.activeProfiles.Count > 0 && team.score >= winsPerS)
                                    {
                                        teamWon = true;
                                        winsPerS = team.score;
                                    }
                                }
                                if (teamWon)
                                {
                                    foreach (Team team2 in Teams.all)
                                    {
                                        if (team2.activeProfiles.Count > 0 && team2.score + difference >= winsPerS)
                                        {
                                            teamsWon++;
                                        }
                                    }
                                }
                                if (teamsWon == 1)
                                {
                                    GameLevel gameLevel = new GameLevel(Deathmatch.RandomLevelString(GameMode.previousLevel, "deathmatch"), 0, false, false);
                                    GameMode.previousLevel = gameLevel.level;
                                    if (Network.isServer)
                                    {
                                        List<int> list = new List<int>();
                                        foreach (Profile profile in DuckNetwork.profiles)
                                        {
                                            profile.ready = true;
                                            if (profile.team != null)
                                            {
                                                list.Add(profile.team.score);
                                                if (profile.connection != null)
                                                {
                                                    profile.ready = false;
                                                }
                                            }
                                            else
                                            {
                                                list.Add(0);
                                            }
                                        }
                                        Send.Message(new NMTransferScores(list));
                                        RunPostRound(false);
                                    }
                                    Level.current = new RockIntro(gameLevel);
                                }
                            }
                        }
                        _switchedLevel = true;
                    }
                }
            }
        }

        public static void RunPostRound(bool testMode)
        {
            if (Level.current == null | testMode)
                return;
            if (!Global.HasAchievement("mine"))
            {
                foreach (Mine mine in Level.current.things[typeof(Mine)])
                {
                    foreach (KeyValuePair<Duck, float> keyValuePair in mine.ducksOnMine)
                    {
                        if (!keyValuePair.Key.dead && keyValuePair.Value > 5 && keyValuePair.Key.profile != null && (!Network.isActive || keyValuePair.Key.profile.connection == DuckNetwork.localConnection))
                        {
                            Global.GiveAchievement("mine");
                            break;
                        }
                    }
                    if (Global.HasAchievement("mine"))
                        break;
                }
            }
            if (!Global.HasAchievement("book"))
            {
                int num = 0;
                foreach (Duck duck in Level.current.things[typeof(Duck)])
                {
                    if (duck.converted != null && duck.converted.profile != null && (!Network.isActive || duck.converted.profile.connection == DuckNetwork.localConnection))
                        ++num;
                }
                if (num > 2)
                    Global.GiveAchievement("book");
            }
            if (Teams.active.Count <= 1 || Profiles.experienceProfile == null || Network.isActive && (DuckNetwork.localProfile == null || DuckNetwork.localProfile.slotType == SlotType.Spectator))
                return;
            DuckNetwork.GiveXP("Rounds", 1, Rando.Int(6, 7), firstCap: 350, secondCap: 650);
            if (Profiles.experienceProfile.roundsSinceXP > 10)
                DuckNetwork.GiveXP("Participation", 0, 75, firstCap: 75, secondCap: 75, finalCap: 75);
            ++Profiles.experienceProfile.roundsSinceXP;
        }

        protected virtual void Update()
        {
        }

        public List<Duck> PrepareSpawns()
        {
            _pendingSpawns = AssignSpawns();
            return _pendingSpawns;
        }

        protected virtual List<Duck> AssignSpawns() => new List<Duck>();

        protected virtual void PlayMusic()
        {
            if (_validityTest)
                return;
            string music = Music.RandomTrack("InGame", _currentMusic);
            Music.Play(music, false);
            _currentMusic = music;
        }

        protected virtual Level GetNextLevel() => _editorTestMode ? new GameLevel((Level.current as GameLevel).levelInputString, editorTestMode: true) : (Level)new GameLevel(Deathmatch.RandomLevelString(previousLevel));

        protected void DoAddPoints()
        {
            _addedPoints = true;
            Event.Log(new RoundEndEvent());
            Highlights.highlightRatingMultiplier = 0f;
            if (AddPoints().Count > 0)
            {
                _roundHadWinner = true;
                SFX.Play("scoreDing", 0.9f);
            }
            if (skippedLevel || _editorTestMode)
                return;
            ++_numMatchesPlayed;
            if (_numMatchesPlayed < roundsBetweenIntermission && !showdown)
                return;
            _numMatchesPlayed = 0;
            _doScore = true;
        }

        protected virtual List<Profile> AddPoints() => new List<Profile>();

        public void SkipMatch()
        {
            skippedLevel = true;
            EndMatch();
        }

        protected void EndMatch() => _matchOver = true;

        public virtual void PostDrawLayer(Layer layer)
        {


            frames++;
            
            if (layer == Layer.Foreground && DGRSettings.NameTags)
            {
                drawNameTags();
                return;
            }
            else if (layer != Layer.HUD)
            {
                return;
            }
            
            if (DGRSettings.QOLScoreThingButWithoutScore)
            {
                drawNameDisplay();
            }

            if (_waitAfterSpawnDings > 0 && _fontFade > 0.01f)
            {
                _font.scale = new Vec2(2f, 2f);
                _font.alpha = _fontFade;
                string text = "GET";
                if (_waitAfterSpawnDings == 2)
                    text = "READY";
                else if (_waitAfterSpawnDings == 3)
                    text = "";
                float width = _font.GetWidth(text);
                _font.Draw(text, (float)(Layer.HUD.camera.width / 2 - width / 2), (float)(Layer.HUD.camera.height / 2 - _font.height / 2), Color.White);
            }
            if (!_validityTest || _waitAfterSpawnDings <= 0 || _fontFade >= 0.5)
                return;
            _pulse += 0.08f;
            string text1 = "WIN THE MATCH";
            float width1 = _font.GetWidth(text1);
            _font.alpha = (float)((Math.Sin(_pulse) + 1) / 2);
            _font.Draw(text1, (float)(Layer.HUD.camera.width / 2 - width1 / 2), (float)(Layer.HUD.camera.height - _font.height - 16), Color.Red);
        }

        private void drawNameDisplay()
        {
            NameDisplayConfig config = AdvancedConfig.Get<NameDisplayConfig>();

            float fontSize = config.Size;
            float vSpacing = config.VerticalSpacing;
            float hSpacing = config.HorizontalSpacing;
            float opacity = config.Opacity;
            float teamLineWidth = config.TeamLineWidth;
            NameDisplayConfig.DeadPlayerRemoval removeDeadPlayers = config.RemoveDeadPlayers;
            float xOffset = config.PositionOffset.X;
            float yOffset = config.PositionOffset.Y;
            NameDisplayConfig.ScoreShowing showScore = config.ShowScores;

            bool doTeams = Extensions.MultiPlayerTeamsExist() && teamLineWidth > 0;
            IEnumerable<Profile> profileList = Profiles.activeNonSpectators;
            Color[] teamColors = { // yoinked from hypixel bedwars teams
                    Color.Blue,
                    Color.Red,
                    Color.Green,
                    Color.Yellow,
                    Color.Purple,
                    Color.Cyan,
                    Color.Pink,
                    Color.Orange,
                };

            if (showScore != NameDisplayConfig.ScoreShowing.DontShow)
                profileList = profileList.OrderByDescending(x => x?.team.score);

            if (removeDeadPlayers == NameDisplayConfig.DeadPlayerRemoval.RemoveDead)
                profileList = profileList.Where(x => x?.duck?.dead == false);

            if (removeDeadPlayers == NameDisplayConfig.DeadPlayerRemoval.ShowAsGhosts)
                profileList = profileList.OrderByDescending(x => x?.duck?.dead == false);

            float xPos = xOffset + hSpacing;
            float yPos = yOffset + vSpacing;

            Dictionary<int, int> teamColorMapping = new();
            Dictionary<int, List<Profile>> profileTeamMembersMapping = new();
            if (doTeams)
            {
                int index = 0;
                foreach (Profile prof in Profiles.activeNonSpectators)
                {
                    int teamHashCode = prof.team.GetHashCode();

                    if (!profileTeamMembersMapping.ContainsKey(teamHashCode))
                        profileTeamMembersMapping.Add(teamHashCode, new List<Profile>());
                    profileTeamMembersMapping[teamHashCode].Add(prof);

                    if (teamColorMapping.ContainsKey(teamHashCode))
                        continue;

                    teamColorMapping[teamHashCode] = index;

                    // increment index by one
                    // makes sure if teams.length > 8 that colors overflow instead of crashing
                    index = (index + 1) % teamColors.Length;
                }

                profileList = profileList.OrderBy(x => x.team.GetHashCode());

                xPos += hSpacing * 2 + teamLineWidth;
            }

            float longestNameWidth = 0f;
            int highestScore = 0;
            
            Dictionary<Profile, Vec2> profileNameSizeMapping = new();
            foreach (Profile prof in profileList)
            {
                Vec2 size = Extensions.GetStringSize(prof.name.CleanFormatting(), fontSize);
                profileNameSizeMapping[prof] = size;

                if (size.x > longestNameWidth)
                    longestNameWidth = size.x;

                if (prof.team.score > highestScore)
                    highestScore = prof.team.score;
            }

            foreach (Profile prof in profileList)
            {
                int teamHashCode = prof.team.GetHashCode();

                (float nameW, float nameH) = profileNameSizeMapping[prof];
                Color duckColor = prof.persona.colorUsable * opacity;
                Color teamColor = doTeams ? (teamColors[teamColorMapping[teamHashCode]] * opacity) : Color.Transparent;
                Color borderColor = Color.Black * opacity;
                float addedHeight = nameH + vSpacing;
                bool isGhost = removeDeadPlayers == NameDisplayConfig.DeadPlayerRemoval.ShowAsGhosts &&
                               prof?.duck?.dead == true;
                
                Graphics.DrawStringOutline(prof.name, new Vec2(xPos + hSpacing + nameH, yPos), isGhost ? Color.DarkRed * 0.6f * opacity : duckColor, borderColor, 1.1f, scale: fontSize);

                Rectangle colorBox = new(xPos, yPos, nameH, nameH - 0.5f);
                Graphics.DrawOutlinedRect(colorBox, duckColor, borderColor, 1.1f, fontSize);

                if (showScore == NameDisplayConfig.ScoreShowing.ShowBar)
                {
                    for (int i = 0; i < prof.team.score; i++)
                    {
                        float barPartX = xPos + nameH / 2 * (i + 1) - fontSize * (i + 1) + longestNameWidth + hSpacing * 3;
                        float barPartY = yPos - 0.5f;
                        Graphics.DrawOutlinedRect(new Rectangle(barPartX, barPartY, nameH / 2, nameH + 0.5f), duckColor, borderColor, 1.1f, fontSize);
                    }
                }
                else if (showScore == NameDisplayConfig.ScoreShowing.ShowValue)
                {
                    float scoreTextPos = xPos + hSpacing * 3 + nameH + longestNameWidth;
                    string scoreText = $"{prof.team.score}";
                    
                    Graphics.DrawStringOutline(scoreText, new Vec2(scoreTextPos, yPos), duckColor, borderColor, 1.1f, scale: fontSize);
                }

                if (doTeams)
                {
                    Vec2 lineStartPos = new(xPos - (hSpacing + teamLineWidth / 2), yPos);
                    Vec2 lineEndOffset = new(0, nameH + vSpacing);

                    List<Profile> teamMembers = profileTeamMembersMapping[teamHashCode];
                    if (teamMembers.IndexOf(prof) == teamMembers.Count - 1)
                        lineEndOffset.y -= vSpacing;

                    Graphics.DrawLine(lineStartPos + lineEndOffset, lineStartPos, teamColor, teamLineWidth, 1.1f);
                }

                yPos += addedHeight;
            }
        }

        private void drawNameTags()
        {
            try // FIXED !!!!!!!!
            {
                if (Level.current is RockScoreboard) return;

                Profile me = Extensions.GetMe();

                if (me?.duck?.connection == null)
                    return;

                bool spectating = (Network.isActive && (me.duck.dead || me.spectator)) || matchOver;

                foreach (Profile prof in Profiles.activeNonSpectators)
                {
                    if (prof?.duck?.connection == null)
                        continue;

                    bool doDraw = spectating || (prof.duck.localSpawnVisible && !started);

                    if (!doDraw)
                        continue;

                    Vec2 tagPos = prof.duck.position - new Vec2(0, 24);
                    float depth = prof.connection == DuckNetwork.localConnection ? 2f : 1.95f;
                    Color duckColor = prof.persona.colorUsable;

                    Extensions.DrawCenteredOutlinedString(prof.name, tagPos, duckColor, Color.Black, depth, null, 0.7f);
                }
            }
            catch(Exception e)
            {
                DevConsole.Log(e.Message);
            }
        }
    }
}
