using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DuckGame
{
    public class NetworkDebugger : Level
    {
        public const int kNumDebuggers = 4;
        public static List<ulong> profileIDs = new List<ulong>()
        {
          0UL,
          1UL,
          2UL,
          3UL,
          5UL,
          6UL,
          7UL,
          8UL
        };
        public static bool enableFrameTimeDebugging = false;
        private static int _currentIndex = 0;
        public static List<InputProfile> inputProfiles = new List<InputProfile>();
        private static bool _enabled = false;
        public static List<NetworkInstance> _instances = new List<NetworkInstance>();
        private Level _startLevel;
        private LayerCore _startLayer;
        //private bool _ghostDebugger;
        private MultiMap<string, int> _controlsMapGamepad;
        private MultiMap<string, int> _controlsMapKeyboard;
        private MultiMap<string, int> _controlsMapKeyboard2;
        //private InputProfile _defaultInput;
        public static NetworkDebugger instance;
        private static int _lastRect = 0;
        public static bool letJoin = false;
        public static bool startJoin = false;
        public static bool hoveringInstance = true;
        public static int fullscreenIndex = 0;
        //private bool eightPlayerMode = true;
        private bool logTimes = true;
        private bool logSections = true;
        private int logSwitchIndex;
        public static Dictionary<GhostObject, GhostDebugData> _ghostDebug = new Dictionary<GhostObject, GhostDebugData>();
        public bool lefpres;
        public static Dictionary<string, Dictionary<string, float>> _sentPulse = new Dictionary<string, Dictionary<string, float>>();
        public static Dictionary<string, Dictionary<string, float>> _receivedPulse = new Dictionary<string, Dictionary<string, float>>();
        public static bool showLogs = false;
        public static int[] showLogPage = new int[4]
        {
      0,
      -1,
      -1,
      -1
        };
        public static int[] logsScroll = new int[4];
        private Vec2[] mouseClickPos = new Vec2[4];
        private Vec2[] mouseClickTop = new Vec2[4];
        private bool[] scrollerDrag = new bool[4];
        private float wheel;
        private Dictionary<DCSection, bool> logFilters = new Dictionary<DCSection, bool>()
    {
      {
        DCSection.General,
        true
      },
      {
        DCSection.NetCore,
        true
      },
      {
        DCSection.DuckNet,
        true
      },
      {
        DCSection.GhostMan,
        true
      },
      {
        DCSection.Steam,
        true
      },
      {
        DCSection.Mod,
        true
      },
      {
        DCSection.Connection,
        true
      },
      {
        DCSection.Ack,
        true
      }
    };
        private bool showFilters;
        //private SpriteMap _connectionArrow;
        // private Sprite _connectionX;
        // private Sprite _connectionWall;
        public static int[] ghostsReceived = new int[8];
        private static Network oldNetwork;
        private static DuckNetworkCore oldDuckNetworkCore;
        private static VirtualTransitionCore oldVirtualCore;
        private static LevelCore oldLevelCore;
        private static ProfilesCore oldProfileCore;
        private static TeamsCore oldTeamCore;
        private static LayerCore oldLayerCore;
        private static InputProfileCore oldInputCore;
        private static DevConsoleCore oDevCore;
        private static CrowdCore oldCrowdCore;
        private static GameModeCore oldGameModeCore;
        private static ConnectionStatusUICore oldConnectionUICore;
        private static MonoMainCore oldMonoCore;
        private static HUDCore oldHUDCore;
        private static MatchmakingBoxCore oldMatchmakingCore;
        private static AutoUpdatables.Core oldAUCore;
        private static Random oldRando;
        private static List<NetworkInstance.Core> _registeredCores = new List<NetworkInstance.Core>();

        public static int currentIndex => _currentIndex;

        public static bool enabled => _enabled;

        public NetworkDebugger(Level level = null, LayerCore startLayer = null, bool pGhostDebugger = false)
        {
            _startLevel = level;
            _startLayer = startLayer;
            if (level == null)
            {
                foreach (Profile profile in Profiles.all)
                    profile.team = null;
            }
            for (int index = 0; index < 8; ++index)
                inputProfiles.Add(new InputProfile());
            //this._ghostDebugger = pGhostDebugger;
        }

        public static int CurrentServerIndex()
        {
            int num = 0;
            foreach (NetworkInstance instance in _instances)
            {
                if (instance.network.core.isActive && instance.network.core.isServer)
                    return num;
                ++num;
            }
            return -1;
        }

        public void RefreshRectSizes()
        {
            for (int index = 0; index < 4; ++index)
                RefreshRectSize(_instances[index], index);
        }

        public void RefreshRectSize(NetworkInstance host, int index)
        {
            switch (index)
            {
                case 0:
                    host.rect = new Rectangle(0f, 0f, Resolution.current.x / 2, Resolution.current.y / 2);
                    break;
                case 1:
                    host.rect = new Rectangle(Resolution.current.x / 2, 0f, Resolution.current.x / 2, Resolution.current.y / 2);
                    break;
                case 2:
                    host.rect = new Rectangle(0f, Resolution.current.y / 2, Resolution.current.x / 2, Resolution.current.y / 2);
                    break;
                case 3:
                    host.rect = new Rectangle(Resolution.current.x / 2, Resolution.current.y / 2, Resolution.current.x / 2, Resolution.current.y / 2);
                    break;
            }
        }

        public void CreateInstance(int init, bool isHost)
        {
            _currentIndex = init;
            NetworkInstance networkInstance = new NetworkInstance()
            {
                network = new Network(_currentIndex)
            };
            RefreshRectSize(networkInstance, _currentIndex);
            List<Team> teamList = new List<Team>();
            foreach (Team extraTeam in Teams.core.extraTeams)
                teamList.Add(extraTeam.Clone());
            if (_startLevel == null)
            {
                networkInstance.teamsCore = new TeamsCore();
            }
            else
            {
                networkInstance.teamsCore = Teams.core;
                Teams.core = new TeamsCore();
                Teams.core.Initialize();
            }
            if (_startLayer != null)
            {
                networkInstance.layerCore = _startLayer;
                _startLayer = null;
            }
            else
            {
                networkInstance.layerCore = new LayerCore();
                networkInstance.layerCore.InitializeLayers();
            }
            networkInstance.profileCore = Profiles.core;
            Profiles.core = new ProfilesCore();
            Profiles.core.Initialize();
            networkInstance.virtualCore = new VirtualTransitionCore();
            networkInstance.inputProfile = new InputProfileCore();
            networkInstance.levelCore = new LevelCore();
            networkInstance.crowdCore = new CrowdCore();
            networkInstance.duckNetworkCore = new DuckNetworkCore(true);
            networkInstance.gameModeCore = new GameModeCore();
            networkInstance.connectionUICore = new ConnectionStatusUICore();
            networkInstance.consoleCore = new DevConsoleCore();
            networkInstance.monoCore = new MonoMainCore();
            networkInstance.hudCore = new HUDCore();
            networkInstance.matchmakingCore = new MatchmakingBoxCore();
            networkInstance.auCore = new AutoUpdatables.Core();
            networkInstance.rando = new Random();
            LockInstance(networkInstance);
            networkInstance.duckNetworkCore.RecreateProfiles();
            Teams.core.Initialize();
            if (init == 0 || init == 1)
                Teams.core.extraTeams = teamList;
            Profiles.core.Initialize();
            networkInstance.virtualCore.Initialize();
            Input.InitDefaultProfiles();
            networkInstance.network.DoInitialize();
            DuckNetwork.Initialize();
            foreach (Team team in Teams.all)
                team.ClearProfiles();
            current = new TeamSelect2();
            networkInstance.joined = true;
            if (init >= _instances.Count)
                _instances.Add(networkInstance);
            else
                _instances[init] = networkInstance;
            base.Initialize();
            switch (init)
            {
                case 1:
                    Profiles.experienceProfile.name = "DAN RANDO";
                    break;
                case 2:
                    Profiles.experienceProfile.name = "Zoo Tycoon 2";
                    Profiles.experienceProfile.preferredColor = 7;
                    break;
                case 3:
                    Profiles.experienceProfile.name = "xXspandeXx";
                    break;
                case 4:
                    Profiles.experienceProfile.name = "boloBoy";
                    break;
                case 5:
                    Profiles.experienceProfile.name = "MINTY TASTE";
                    break;
                case 6:
                    Profiles.experienceProfile.name = "r_b_sprinkles";
                    break;
                case 7:
                    Profiles.experienceProfile.name = "darren";
                    break;

            }
            if (init != 0)
            {
                Profiles.experienceProfile.keepSetName = true;
                Profiles.experienceProfile.furniturePositions.Clear();
            }
            networkInstance.debugInterface = new NetDebugInterface(_instances[init]);
            UnlockInstance(_instances[init]);
            using (List<NetworkInstance.Core>.Enumerator enumerator = _registeredCores.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    NetworkInstance.Core current = enumerator.Current;
                    _instances[init].extraCores.Add(new NetworkInstance.Core()
                    {
                        member = current.member,
                        originalInstance = current.originalInstance,
                        instance = Activator.CreateInstance(current.member.FieldType, null),
                        firstLockAction = current.firstLockAction
                    });
                }
            }
        }

        public override void Initialize()
        {
            _enabled = true;
            _currentIndex = 0;
            _controlsMapGamepad = InputProfile.DefaultPlayer1.GetControllerMap<GenericController>();
            _controlsMapKeyboard = InputProfile.defaultProfiles[Options.Data.keyboard1PlayerIndex].GetControllerMap<Keyboard>();
            _controlsMapKeyboard2 = InputProfile.defaultProfiles[Options.Data.keyboard2PlayerIndex].GetControllerMap<Keyboard>();
            for (int init = 0; init < 4; ++init)
                CreateInstance(init, true);
            activeLevel = this;
            base.Initialize();
        }

        public static void TerminateThreads()
        {
            foreach (NetworkInstance instance in _instances)
            {
                LockInstance(instance);
                instance.network.core.Terminate();
                UnlockInstance(instance);
            }
        }

        public override void DoUpdate()
        {
            instance = this;
            if (Keyboard.Down(Keys.LeftShift) && Keyboard.Pressed(Keys.L))
                showLogs = !showLogs;
            MonoMain.instance.IsMouseVisible = true;
            lefpres = Mouse.left == InputState.Pressed;
            List<DCLine> dcLineList = null;
            lock (DevConsole.debuggerLines)
            {
                dcLineList = DevConsole.debuggerLines;
                DevConsole.debuggerLines = new List<DCLine>();
            }
            //this._defaultInput = InputProfile.DefaultPlayer1;
            for (int index1 = 0; index1 < 4; ++index1)
            {
                foreach (NetworkInstance instance in _instances)
                {
                    if (instance.inputProfile.DefaultPlayer1.JoinGamePressed())
                        letJoin = true;
                }
                NetworkInstance instance1 = _instances[index1];
                _currentIndex = index1;
                LockInstance(instance1);
                bool flag1 = false;
                if (_lastRect == _currentIndex || instance1.rect.Contains(Mouse.mousePos) || Math.Abs(Graphics.width / 2 - Mouse.mousePos.x) < 32 && Math.Abs(Graphics.height / 2 - Mouse.mousePos.y) < 32)
                {
                    _lastRect = _currentIndex;
                    InputProfile.active = InputProfile.DefaultPlayer1;
                    InputProfile.DefaultPlayer1.SetGenericControllerMapIndex<GenericController>(0, _controlsMapGamepad);
                    InputProfile.DefaultPlayer2.SetGenericControllerMapIndex<GenericController>(1, _controlsMapGamepad);
                    InputProfile.DefaultPlayer3.SetGenericControllerMapIndex<GenericController>(2, _controlsMapGamepad);
                    InputProfile.DefaultPlayer4.SetGenericControllerMapIndex<GenericController>(3, _controlsMapGamepad);
                    InputProfile.Get(InputProfile.MPPlayer1).SetGenericControllerMapIndex<GenericController>(0, _controlsMapGamepad);
                    InputProfile.Get(InputProfile.MPPlayer2).SetGenericControllerMapIndex<GenericController>(1, _controlsMapGamepad);
                    InputProfile.Get(InputProfile.MPPlayer3).SetGenericControllerMapIndex<GenericController>(2, _controlsMapGamepad);
                    InputProfile.Get(InputProfile.MPPlayer4).SetGenericControllerMapIndex<GenericController>(3, _controlsMapGamepad);
                    InputProfile.DefaultPlayer1.SetGenericControllerMapIndex<Keyboard>(0, null);
                    InputProfile.DefaultPlayer2.SetGenericControllerMapIndex<Keyboard>(1, null);
                    InputProfile.DefaultPlayer3.SetGenericControllerMapIndex<Keyboard>(2, null);
                    InputProfile.DefaultPlayer4.SetGenericControllerMapIndex<Keyboard>(3, null);
                    InputProfile.Get(InputProfile.MPPlayer1).SetGenericControllerMapIndex<Keyboard>(0, null);
                    InputProfile.Get(InputProfile.MPPlayer2).SetGenericControllerMapIndex<Keyboard>(1, null);
                    InputProfile.Get(InputProfile.MPPlayer3).SetGenericControllerMapIndex<Keyboard>(2, null);
                    InputProfile.Get(InputProfile.MPPlayer4).SetGenericControllerMapIndex<Keyboard>(3, null);
                    InputProfile.defaultProfiles[Options.Data.keyboard1PlayerIndex].SetGenericControllerMapIndex<Keyboard>(0, _controlsMapKeyboard);
                    InputProfile.Get(InputProfile.MPPlayers[Options.Data.keyboard1PlayerIndex]).SetGenericControllerMapIndex<Keyboard>(0, _controlsMapKeyboard);
                    InputProfile.defaultProfiles[Options.Data.keyboard2PlayerIndex].SetGenericControllerMapIndex<Keyboard>(1, _controlsMapKeyboard2);
                    InputProfile.Get(InputProfile.MPPlayers[Options.Data.keyboard2PlayerIndex]).SetGenericControllerMapIndex<Keyboard>(1, _controlsMapKeyboard2);
                    hoveringInstance = true;
                    flag1 = true;
                }
                else
                {
                    InputProfile.active = InputProfile.DefaultPlayer4;
                    InputProfile.DefaultPlayer1.SetGenericControllerMapIndex<GenericController>(0, null);
                    InputProfile.DefaultPlayer2.SetGenericControllerMapIndex<GenericController>(1, null);
                    InputProfile.DefaultPlayer3.SetGenericControllerMapIndex<GenericController>(2, null);
                    InputProfile.DefaultPlayer4.SetGenericControllerMapIndex<GenericController>(3, null);
                    InputProfile.Get(InputProfile.MPPlayer1).SetGenericControllerMapIndex<GenericController>(0, null);
                    InputProfile.Get(InputProfile.MPPlayer2).SetGenericControllerMapIndex<GenericController>(1, null);
                    InputProfile.Get(InputProfile.MPPlayer3).SetGenericControllerMapIndex<GenericController>(2, null);
                    InputProfile.Get(InputProfile.MPPlayer4).SetGenericControllerMapIndex<GenericController>(3, null);
                    InputProfile.DefaultPlayer1.SetGenericControllerMapIndex<Keyboard>(0, null);
                    InputProfile.DefaultPlayer2.SetGenericControllerMapIndex<Keyboard>(1, null);
                    InputProfile.DefaultPlayer3.SetGenericControllerMapIndex<Keyboard>(2, null);
                    InputProfile.DefaultPlayer4.SetGenericControllerMapIndex<Keyboard>(3, null);
                    InputProfile.Get(InputProfile.MPPlayer1).SetGenericControllerMapIndex<Keyboard>(0, null);
                    InputProfile.Get(InputProfile.MPPlayer2).SetGenericControllerMapIndex<Keyboard>(1, null);
                    InputProfile.Get(InputProfile.MPPlayer3).SetGenericControllerMapIndex<Keyboard>(2, null);
                    InputProfile.Get(InputProfile.MPPlayer4).SetGenericControllerMapIndex<Keyboard>(3, null);
                    hoveringInstance = false;
                }
                InputProfile.Update();
                if (Recorder.active != null)
                {
                    if (currentIndex == Recorder.active.activeIndex)
                    {
                        if (InputProfile.DefaultPlayer1.virtualDevice != null)
                        {
                            InputProfile.DefaultPlayer1.virtualDevice.SetState(0);
                            InputProfile.DefaultPlayer1.virtualDevice.SetState(0);
                            InputProfile.DefaultPlayer1.virtualDevice = null;
                        }
                        Recorder.active.Log(InputProfile.DefaultPlayer1.state);
                    }
                    else
                    {
                        if (InputProfile.DefaultPlayer1.virtualDevice == null)
                        {
                            InputProfile.DefaultPlayer1.virtualDevice = VirtualInput.debuggerInputs[currentIndex];
                            for (int index2 = 0; index2 < Network.synchronizedTriggers.Count; ++index2)
                                InputProfile.DefaultPlayer1.Map(VirtualInput.debuggerInputs[currentIndex], Network.synchronizedTriggers[index2], index2);
                            VirtualInput.debuggerInputs[currentIndex].availableTriggers = Network.synchronizedTriggers;
                        }
                        InputProfile.DefaultPlayer1.virtualDevice.SetState(Recorder.active.Get());
                    }
                }
                foreach (DCLine dcLine in dcLineList)
                {
                    if (dcLine.threadIndex == index1)
                        DevConsole.core.lines.Enqueue(dcLine);
                }
                bool flag2 = false;
                if (flag1 && Keyboard.Pressed(Keys.OemMinus))
                    flag2 = true;
                if (!flag2)
                {
                    Network.netGraph.PreUpdate();
                    DevConsole.Update();
                    Network.PreUpdate();
                    MonoMain.UpdatePauseMenu();
                    if (!MonoMain.shouldPauseGameplay)
                    {
                        foreach (IEngineUpdatable engineUpdatable in MonoMain.core.engineUpdatables)
                            engineUpdatable.PreUpdate();
                        AutoUpdatables.Update();
                        FireManager.Update();
                        UpdateLevelChange();
                        UpdateCurrentLevel();
                        foreach (IEngineUpdatable engineUpdatable in MonoMain.core.engineUpdatables)
                            engineUpdatable.Update();
                    }
                    if (!showLogs)
                        instance1.debugInterface.Update();
                    Network.PostUpdate();
                    foreach (IEngineUpdatable engineUpdatable in MonoMain.core.engineUpdatables)
                        engineUpdatable.PostUpdate();
                    instance1.network.core.Thread_Loop();
                }
                UnlockInstance(instance1);
            }
            if (Recorder.active != null)
                ++Recorder.active.frame;
            if (showLogs)
            {
                showFilters = false;
                if (Keyboard.Down(Keys.LeftControl))
                {
                    showFilters = true;
                    for (int key = 49; key <= 56; ++key)
                    {
                        if (Keyboard.Pressed((Keys)key))
                        {
                            int index3 = key - 49;
                            if (Keyboard.Down(Keys.LeftShift))
                            {
                                for (int index4 = 0; index4 < 8; ++index4)
                                    logFilters[logFilters.ElementAt(index4).Key] = index4 == index3;
                            }
                            else
                                logFilters[logFilters.ElementAt(index3).Key] = !logFilters.ElementAt(index3).Value;
                        }
                    }
                    if (Keyboard.Pressed(Keys.D9))
                    {
                        if (logFilters[logFilters.ElementAt(0).Key] && logFilters[logFilters.ElementAt(1).Key] && logFilters[logFilters.ElementAt(2).Key] && logFilters[logFilters.ElementAt(3).Key] && logFilters[logFilters.ElementAt(4).Key] && logFilters[logFilters.ElementAt(5).Key] && logFilters[logFilters.ElementAt(6).Key])
                        {
                            for (int index = 0; index < 8; ++index)
                                logFilters[logFilters.ElementAt(index).Key] = false;
                        }
                        else
                        {
                            for (int index = 0; index < 8; ++index)
                                logFilters[logFilters.ElementAt(index).Key] = true;
                        }
                    }
                }
                else if (Keyboard.Down(Keys.LeftShift))
                {
                    if (Keyboard.Pressed(Keys.D1))
                    {
                        logSwitchIndex = 0;
                        if (showLogPage[logSwitchIndex] < 0)
                            showLogPage[logSwitchIndex] = logSwitchIndex;
                    }
                    else if (Keyboard.Pressed(Keys.D2))
                    {
                        logSwitchIndex = 1;
                        if (showLogPage[logSwitchIndex] < 0)
                            showLogPage[logSwitchIndex] = logSwitchIndex;
                    }
                    else if (Keyboard.Pressed(Keys.D3))
                    {
                        logSwitchIndex = 2;
                        if (showLogPage[logSwitchIndex] < 0)
                            showLogPage[logSwitchIndex] = logSwitchIndex;
                    }
                    else if (Keyboard.Pressed(Keys.D4))
                    {
                        logSwitchIndex = 3;
                        if (showLogPage[logSwitchIndex] < 0)
                            showLogPage[logSwitchIndex] = logSwitchIndex;
                    }
                    if (Keyboard.Pressed(Keys.T))
                        logTimes = !logTimes;
                    else if (Keyboard.Pressed(Keys.S))
                        logSections = !logSections;
                }
                else
                {
                    if (Keyboard.Pressed(Keys.D1))
                        showLogPage[logSwitchIndex] = 0;
                    else if (Keyboard.Pressed(Keys.D2))
                        showLogPage[logSwitchIndex] = 1;
                    else if (Keyboard.Pressed(Keys.D3))
                        showLogPage[logSwitchIndex] = 2;
                    else if (Keyboard.Pressed(Keys.D4))
                        showLogPage[logSwitchIndex] = 3;
                    else if (Keyboard.Pressed(Keys.D0))
                        showLogPage[logSwitchIndex] = -1;
                    while (showLogPage[logSwitchIndex] < 0 && logSwitchIndex > 0)
                        --logSwitchIndex;
                }
            }
            wheel = Mouse.scroll;
            for (int index = 0; index < 4; ++index)
            {
                LockInstance(_instances[index]);
                _currentIndex = index;
                UnlockInstance(_instances[index]);
            }
            if (Keyboard.Pressed(Keys.F11))
            {
                foreach (NetworkInstance instance in _instances)
                    instance.network.core.ForcefulTermination();
                Network.activeNetwork.core.ForcefulTermination();
                _instances.Clear();
                inputProfiles.Clear();
                current = new NetworkDebugger();
            }
            if (Keyboard.shift)
            {
                if (Keyboard.Pressed(Keys.D0))
                    fullscreenIndex = 0;
                if (Keyboard.Pressed(Keys.D1))
                    fullscreenIndex = 1;
                if (Keyboard.Pressed(Keys.D2))
                    fullscreenIndex = 2;
                if (Keyboard.Pressed(Keys.D3))
                    fullscreenIndex = 3;
                if (Keyboard.Pressed(Keys.D4))
                    fullscreenIndex = 4;
                if (Keyboard.Pressed(Keys.D5))
                    fullscreenIndex = 5;
                if (Keyboard.Pressed(Keys.D6))
                    fullscreenIndex = 6;
                if (Keyboard.Pressed(Keys.D7))
                    fullscreenIndex = 7;
                if (Keyboard.Pressed(Keys.D8))
                    fullscreenIndex = 8;
            }
            things.RefreshState();
        }

        public override void DoDraw()
        {
            if (_instances.Count == 0)
                return;
            int num = -1;
            foreach (NetworkInstance instance in _instances)
            {
                ++num;
                if (instance.active)
                {
                    _currentIndex = num;
                    LockInstance(instance);
                    Viewport viewport = Graphics.viewport;
                    Graphics.viewport = fullscreenIndex <= 0 ? new Viewport((int)instance.rect.x, (int)instance.rect.y, (int)instance.rect.width, (int)instance.rect.height) : (num + 1 != fullscreenIndex ? new Viewport(0, 0, 1, 1) : new Viewport(0, 0, viewport.Width, viewport.Height));
                    current.clearScreen = num == 0;
                    if (MonoMain.pauseMenu != null)
                    {
                        if (current.clearScreen)
                            Graphics.Clear(current.backgroundColor);
                        Layer.HUD.Begin(true);
                        MonoMain.pauseMenu.Draw();
                        foreach (Thing thing in MonoMain.closeMenuUpdate)
                            thing.Draw();
                        HUD.Draw();
                        Layer.HUD.End(true);
                        Layer.Console.Begin(true);
                        DevConsole.Draw();
                        Layer.Console.End(true);
                    }
                    else
                        DrawCurrentLevel();
                    Network.netGraph.Draw();
                    Graphics.viewport = viewport;
                    UnlockInstance(instance);
                }
            }
            clearScreen = false;
            base.DoDraw();
        }

        public static GhostDebugData GetGhost(GhostObject pGhost)
        {
            GhostDebugData ghost;
            if (!_ghostDebug.TryGetValue(pGhost, out ghost))
                ghost = _ghostDebug[pGhost] = new GhostDebugData();
            return ghost;
        }

        public static void ClearGhostDebug() => _ghostDebug.Clear();

        public static void DrawInstanceGameDebug()
        {
            foreach (GhostObject ghost1 in _instances[currentIndex].network.core.ghostManager._ghosts)
            {
                GhostDebugData ghost2 = GetGhost(ghost1);
                if (ghost1.thing != null)
                {
                    if (ghost1.thing.connection == DuckNetwork.localConnection || ghost1.thing.connection == null)
                        Graphics.DrawRect(ghost1.thing.topLeft, ghost1.thing.bottomRight, Color.Red * 0.8f, (Depth)1f, false);
                    if (ghost1.thing.ghostObject != null && !ghost1.thing.ghostObject.IsInitialized())
                        Graphics.DrawRect(ghost1.thing.topLeft + new Vec2(-1f, -1f), ghost1.thing.bottomRight + new Vec2(1f, 1f), Color.Orange * 0.8f, (Depth)1f, false);
                    foreach (KeyValuePair<DuckPersona, long> dataReceivedFrame in ghost2.dataReceivedFrames)
                    {
                        if (dataReceivedFrame.Value == Graphics.frame)
                        {
                            if (dataReceivedFrame.Key == Persona.Duck1)
                                Graphics.DrawRect(ghost1.thing.topLeft + new Vec2(-4f, -4f), ghost1.thing.topLeft + new Vec2(-2f, -2f), dataReceivedFrame.Key.colorUsable, (Depth)1f, false);
                            else if (dataReceivedFrame.Key == Persona.Duck2)
                                Graphics.DrawRect(ghost1.thing.topRight + new Vec2(4f, -4f), ghost1.thing.topRight + new Vec2(2f, -2f), dataReceivedFrame.Key.colorUsable, (Depth)1f, false);
                            else if (dataReceivedFrame.Key == Persona.Duck3)
                                Graphics.DrawRect(ghost1.thing.bottomLeft + new Vec2(-4f, 2f), ghost1.thing.bottomLeft + new Vec2(-2f, 4f), dataReceivedFrame.Key.colorUsable, (Depth)1f, false);
                            else if (dataReceivedFrame.Key == Persona.Duck4)
                                Graphics.DrawRect(ghost1.thing.bottomRight + new Vec2(4f, 2f), ghost1.thing.bottomRight + new Vec2(2f, 4f), dataReceivedFrame.Key.colorUsable, (Depth)1f, false);
                        }
                    }
                }
            }
        }

        public static void StartRecording(string pLevel)
        {
            Recorder.active = new Recorder
            {
                level = pLevel
            };
            StartRecording(0);
        }

        public static void StartRecording(int pIndex)
        {
            if (Recorder.active == null)
                return;
            Recorder.active.activeIndex = pIndex;
            Recorder.active.frame = 0;
            DevConsole.RunCommand("level " + Recorder.active.level);
        }

        public static void LogSend(string from, string to)
        {
            if (!_sentPulse.ContainsKey(from))
                _sentPulse[from] = new Dictionary<string, float>();
            if (!_sentPulse[from].ContainsKey(to))
                _sentPulse[from][to] = 0f;
            ++_sentPulse[from][to];
        }

        public static float GetSent(string key, string to)
        {
            if (!_sentPulse.ContainsKey(key) || !_sentPulse[key].ContainsKey(to))
                return 0f;
            if (_sentPulse[key][to] > 1)
                _sentPulse[key][to] = 1f;
            _sentPulse[key][to] -= 0.1f;
            if (_sentPulse[key][to] < 0)
                _sentPulse[key][to] = 0f;
            return _sentPulse[key][to];
        }

        public static void LogReceive(string to, string from)
        {
            if (!_receivedPulse.ContainsKey(to))
                _receivedPulse[to] = new Dictionary<string, float>();
            if (!_receivedPulse[to].ContainsKey(from))
                _receivedPulse[to][from] = 0f;
            ++_receivedPulse[to][from];
        }

        public static float GetReceived(string key, string from)
        {
            if (!_receivedPulse.ContainsKey(key) || !_receivedPulse[key].ContainsKey(from))
                return 0f;
            if (_receivedPulse[key][from] > 1)
                _receivedPulse[key][from] = 1f;
            _receivedPulse[key][from] -= 0.1f;
            if (_receivedPulse[key][from] < 0)
                _receivedPulse[key][from] = 0f;
            return _receivedPulse[key][from];
        }

        public static string GetID(int index)
        {
            switch (index)
            {
                case 0:
                    return "127.0.0.1:1337";
                case 1:
                    return "127.0.0.1:1338";
                case 2:
                    return "127.0.0.1:1339";
                case 3:
                    return "127.0.0.1:1340";
                default:
                    return "";
            }
        }

        private void DrawLogWindow(Vec2 pos, Vec2 size, int page, int index)
        {
            if (_instances.Count <= page)
                return;
            int num1 = 97;
            if (size.y < 300)
                num1 = num1 / 2 - 2;
            Queue<DCLine> lines = _instances[page].consoleCore.lines;
            Vec2 p1_1 = pos;
            Vec2 p2_1 = pos + size;
            Graphics.DrawRect(p1_1, p2_1, Color.Black, (Depth)0.8f);
            if (logSwitchIndex == index)
                Graphics.DrawRect(p1_1, p2_1, Color.White * 0.5f, (Depth)0.88f, false);
            Graphics.DrawRect(p1_1 + new Vec2(0f, -14f), p1_1 + new Vec2(100f, 0f), Color.Black, (Depth)0.8f);
            Color color = Colors.Duck1;
            switch (page)
            {
                case 1:
                    color = Colors.Duck2;
                    break;
                case 2:
                    color = Colors.Duck3;
                    break;
                case 3:
                    color = Colors.Duck4;
                    break;
            }
            Graphics.DrawString("Player " + (page + 1).ToString(), p1_1 + new Vec2(4f, -12f), color * (logSwitchIndex == index ? 1f : 0.6f), (Depth)0.81f);
            int num2 = 0;
            foreach (DCLine dcLine in lines)
            {
                if (logFilters[dcLine.section] || dcLine.line.Contains("@error"))
                    ++num2;
            }
            Graphics.DrawRect(new Vec2(p1_1.x + (size.x - 12f), p1_1.y), p2_1, Color.Gray * 0.5f, (Depth)0.81f);
            float num3 = logsScroll[index] / (float)num2;
            float num4 = 300f;
            float num5 = Math.Max(num4 - num2, 20f) / num4;
            float num6 = size.y * num5;
            Vec2 p1_2 = new Vec2(p1_1.x + (size.x - 12f), p1_1.y + num3 * (size.y - num6));
            Vec2 p2_2 = new Vec2(p1_1.x + size.x, p1_1.y + num3 * (size.y - num6) + num6);
            bool flag = false;
            if (Mouse.xConsole > p1_2.x && Mouse.xConsole < p2_2.x && Mouse.yConsole > p1_2.y && Mouse.yConsole < p2_2.y)
            {
                if (Mouse.left == InputState.Pressed)
                {
                    scrollerDrag[index] = true;
                    mouseClickPos[index] = Mouse.positionConsole;
                    mouseClickTop[index] = p1_2;
                }
                flag = true;
            }
            if (scrollerDrag[index])
            {
                Vec2 vec2_1 = mouseClickPos[index] - Mouse.positionConsole;
                Vec2 vec2_2 = mouseClickTop[index] - vec2_1;
                if (vec2_2.y < p1_1.y)
                    vec2_2.y = p1_1.y;
                if (vec2_2.y > p2_1.y - num6)
                    vec2_2.y = p2_1.y - num6;
                logsScroll[index] = (int)Math.Round((vec2_2.y - p1_1.y) / (size.y - num6) * num2);
            }
            if (Mouse.left == InputState.Released)
                scrollerDrag[index] = false;
            Graphics.DrawRect(p1_2, p2_2, Color.White * (flag || scrollerDrag[index] ? 0.8f : 0.5f), (Depth)0.82f);
            if (Mouse.xConsole > p1_1.x && Mouse.xConsole < p2_1.x && Mouse.yConsole > p1_1.y && Mouse.yConsole < p2_1.y)
            {
                if (Mouse.scroll > 0)
                    logsScroll[index] += 5;
                else if (Mouse.scroll < 0)
                    logsScroll[index] -= 5;
            }
            if (logsScroll[index] < 0)
                logsScroll[index] = 0;
            if (logsScroll[index] > num2 - 1)
                logsScroll[index] = num2 - 1;
            if (num2 < num1)
                logsScroll[index] = 0;
            Vec2 pos1 = p1_1 + new Vec2(8f, 8f);
            int num7 = 0;
            for (int index1 = 0; index1 < num1; ++index1)
            {
                int num8 = index1 + logsScroll[index];
                if (num8 < num2)
                {
                    int num9 = 0;
                    for (; num8 + num7 < lines.Count && !logFilters[lines.ElementAt(num8 + num7).section] && !lines.ElementAt(num8 + num7).line.Contains("@error"); ++num7)
                        num9 += lines.ElementAt(num8 + num7).frames;
                    if (num8 + num7 < lines.Count)
                    {
                        DCLine line = lines.ElementAt(num8 + num7);
                        DevConsole.DrawLine(pos1, line, logTimes, logSections);
                        Color col = DCLine.ColorForSection(line.section);
                        col.r = (byte)(col.r * 0.1f);
                        col.g = (byte)(col.g * 0.1f);
                        col.b = (byte)(col.b * 0.1f);
                        if (line.line.Contains("@error"))
                        {
                            col = Color.Red;
                            col.r = (byte)(col.r * 0.3f);
                            col.g = (byte)(col.g * 0.3f);
                            col.b = (byte)(col.b * 0.3f);
                        }
                        Graphics.DrawRect(pos1 + new Vec2(-4f, -1f), new Vec2(p2_1.x - 14f, pos1.y + 9f), col, (Depth)0.85f);
                        if (line.frames + num9 > 0)
                        {
                            ++pos1.y;
                            Graphics.DrawLine(pos1 + new Vec2(-4f, 10f), new Vec2(p2_1.x - 14f, pos1.y + 10f), Color.White * 0.24f, depth: ((Depth)0.9f));
                            pos1.y += 2f;
                            if (line.frames + num9 > 30)
                            {
                                Graphics.DrawString("~" + (line.frames + num9).ToString() + " frames~", pos1 + new Vec2(80f, 10f), Color.White * 0.2f, (Depth)0.9f);
                                pos1.y += 10f;
                                --num1;
                            }
                        }
                    }
                    pos1.y += 10f;
                }
            }
        }

        public override void PostDrawLayer(Layer layer)
        {
            if (_instances.Count == 0 || fullscreenIndex != 0)
                return;
            if (layer == Layer.Console)
            {
                Graphics.fade = 1f;
                if (showLogs)
                {
                    int num = 0;
                    for (int index = 0; index < 4; ++index)
                    {
                        if (showLogPage[index] >= 0)
                            ++num;
                    }
                    Vec2[] vec2Array = new Vec2[4];
                    Vec2 vec2 = new Vec2(20f, 80f);
                    if (showFilters)
                    {
                        Graphics.DrawRect(vec2 + new Vec2(0f, -42f), vec2 + new Vec2(890f, -30f), Color.Black * 0.9f, (Depth)0.8f);
                        for (int index = 0; index < 9; ++index)
                        {
                            if (index == 8)
                            {
                                Graphics.DrawString("ALL (9)", vec2 + new Vec2(index * 110, -40f), Color.White, (Depth)0.82f);
                            }
                            else
                            {
                                DCSection dcSection = (DCSection)Enum.GetValues(typeof(DCSection)).GetValue(index);
                                string str = DCLine.StringForSection(dcSection, true, false, false);
                                if (dcSection == DCSection.General)
                                    str = "GENERAL";
                                Graphics.DrawString(str + " (" + (index + 1).ToString() + ")", vec2 + new Vec2(index * 110, -40f), Color.White * (logFilters[dcSection] ? 1f : 0.5f), (Depth)0.82f);
                            }
                        }
                    }
                    vec2Array[0] = vec2;
                    Vec2 size = new Vec2(Layer.Console.width - 40f, Layer.Console.height - 100f);
                    if (num > 1)
                    {
                        size = new Vec2((float)(size.x / 2 - 4), Layer.Console.height - 100f);
                        vec2Array[1] = vec2 + new Vec2(size.x + 4f, 0f);
                    }
                    if (num > 2)
                    {
                        size = new Vec2(size.x, (float)(size.y / 2 - 16));
                        vec2Array[2] = vec2 + new Vec2(0f, size.y + 16f);
                        vec2Array[3] = vec2 + new Vec2(size.x + 4f, size.y + 16f);
                    }
                    for (int index = 0; index < 4; ++index)
                    {
                        if (showLogPage[index] >= 0)
                            DrawLogWindow(vec2Array[index], size, showLogPage[index], index);
                    }
                    return;
                }
                foreach (NetworkInstance instance in _instances.ToList())
                {
                    LockInstance(instance);
                    instance.debugInterface.Draw();
                    if (instance.debugInterface.visible)
                        Network.netGraph.DrawChart(instance.consoleSize.tl + new Vec2(10f, 300f));
                    UnlockInstance(instance);
                }
            }
            base.PostDrawLayer(layer);
        }

        public static void LockInstance(NetworkInstance instance)
        {
            oldNetwork = Network.activeNetwork;
            Network.activeNetwork = instance.network;
            oldDuckNetworkCore = DuckNetwork.core;
            DuckNetwork.core = instance.duckNetworkCore;
            oldVirtualCore = VirtualTransition.core;
            VirtualTransition.core = instance.virtualCore;
            oldLevelCore = core;
            core = instance.levelCore;
            oldProfileCore = Profiles.core;
            Profiles.core = instance.profileCore;
            oldTeamCore = Teams.core;
            Teams.core = instance.teamsCore;
            oldLayerCore = Layer.core;
            Layer.core = instance.layerCore;
            oldInputCore = InputProfile.core;
            InputProfile.core = instance.inputProfile;
            oDevCore = DevConsole.core;
            DevConsole.core = instance.consoleCore;
            oldCrowdCore = Crowd.core;
            Crowd.core = instance.crowdCore;
            oldGameModeCore = GameMode.core;
            GameMode.core = instance.gameModeCore;
            oldConnectionUICore = ConnectionStatusUI.core;
            ConnectionStatusUI.core = instance.connectionUICore;
            oldMonoCore = MonoMain.core;
            MonoMain.core = instance.monoCore;
            oldHUDCore = HUD.core;
            HUD.core = instance.hudCore;
            oldMatchmakingCore = UIMatchmakingBox.core;
            UIMatchmakingBox.core = instance.matchmakingCore;
            oldAUCore = AutoUpdatables.core;
            AutoUpdatables.core = instance.auCore;
            oldRando = Rando.generator;
            Rando.generator = instance.rando;
            foreach (NetworkInstance.Core extraCore in instance.extraCores)
                extraCore.Lock();
        }

        public static void UnlockInstance(NetworkInstance instance)
        {
            Network.activeNetwork = oldNetwork;
            DuckNetwork.core = oldDuckNetworkCore;
            Teams.core = oldTeamCore;
            Layer.core = oldLayerCore;
            VirtualTransition.core = oldVirtualCore;
            core = oldLevelCore;
            Profiles.core = oldProfileCore;
            InputProfile.core = oldInputCore;
            DevConsole.core = oDevCore;
            Crowd.core = oldCrowdCore;
            GameMode.core = oldGameModeCore;
            ConnectionStatusUI.core = oldConnectionUICore;
            MonoMain.core = oldMonoCore;
            HUD.core = oldHUDCore;
            UIMatchmakingBox.core = oldMatchmakingCore;
            AutoUpdatables.core = oldAUCore;
            Rando.generator = oldRando;
            foreach (NetworkInstance.Core extraCore in instance.extraCores)
                extraCore.Unlock();
        }

        public static NetworkInstance Reboot(NetworkInstance pInstance)
        {
            UnlockInstance(pInstance);
            int num = _instances.IndexOf(pInstance);
            NetworkDebugger.instance.CreateInstance(num, false);
            NetworkInstance instance = _instances[num];
            LockInstance(pInstance);
            return instance;
        }

        public static void RegisterCore<T>(string pCoreMemberName, Action pRunOnFirstLock)
        {
            FieldInfo field = typeof(T).GetField(pCoreMemberName, BindingFlags.Static | BindingFlags.Public);
            if (!(field != null))
                return;
            _registeredCores.Add(new NetworkInstance.Core()
            {
                member = field,
                originalInstance = field.GetValue(null),
                firstLockAction = pRunOnFirstLock
            });
        }

        public class GhostDebugData
        {
            public bool hover;
            public Dictionary<DuckPersona, long> dataReceivedFrames = new Dictionary<DuckPersona, long>();
        }

        public class Recorder
        {
            public static Recorder active;
            private Dictionary<int, List<ushort>> inputs = new Dictionary<int, List<ushort>>();
            public int seed = 1337;
            public string level = "";
            public int activeIndex;
            public int frame;

            public void Log(ushort pInputState)
            {
                List<ushort> ushortList;
                if (!inputs.TryGetValue(currentIndex, out ushortList))
                    inputs[currentIndex] = ushortList = new List<ushort>();
                ushortList.Add(pInputState);
            }

            public ushort Get()
            {
                List<ushort> ushortList;
                return inputs.TryGetValue(currentIndex, out ushortList) && frame < ushortList.Count ? ushortList[frame] : (ushort)0;
            }
        }
    }
}
