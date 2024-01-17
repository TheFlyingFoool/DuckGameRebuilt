using AddedContent.Firebreak;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using AddedContent.Hyeve;

namespace DuckGame
{
    public class Level
    {
        public List<NMLevel> levelMessages = new List<NMLevel>();
        public bool isCustomLevel;
        public static bool flipH = false;
        public static bool symmetry = false;
        public static bool leftSymmetry = true;
        public static bool loadingOppositeSymmetry = false;
        public string _level = "";
        private static LevelCore _core = new LevelCore();
        public static bool skipInitialize = false;
        public bool isPreview;
        private static Queue<List<object>> _collisionLists = new Queue<List<object>>();
        private bool _simulatePhysics = true;
        private Color _backgroundColor = Color.Black;
        protected QuadTreeObjectList _things = new QuadTreeObjectList();
        protected string _id = "";
        protected Camera _camera = new Camera();
        protected NetLevelStatus _networkStatus;
        public float transitionSpeedMultiplier = 1f;
        public float lowestPoint = 1000f;
        private bool _lowestPointInitialized;
        public float highestPoint = -1000f;
        protected bool _initialized;
        protected bool _levelStart;
        public bool _startCalled;
        protected bool _centeredView;
        private bool _waitingOnNewData;
        public byte networkIndex;
        public int seed;
        private bool _notifiedReady;
        private bool _initializeLater;
        public bool bareInitialize;
        protected Vec2 _topLeft = new Vec2(99999f, 99999f);
        protected Vec2 _bottomRight = new Vec2(-99999f, -99999f);
        protected bool _readyForTransition = true;
        public bool _waitingOnTransition;
        public bool cold;
        public bool suppressLevelMessage;
        private static int collectionCount;
        protected int _updateWaitFrames;
        private bool _sentLevelChange;
        private bool _calledAllClientsReady;
        public bool transferCompleteCalled = true;
        private bool _aiInitialized;
        private bool _refreshState;
        //private bool initPaths;
        private Dictionary<NetworkConnection, bool> checksumReplies = new Dictionary<NetworkConnection, bool>();
        public static bool doingOnLoadedMessage = false;
        public float flashDissipationSpeed = 0.15f;
        public bool skipCurrentLevelReset;
        //private int wait = 60;
        private bool _clearScreen = true;
        public bool drawsOverPauseMenu;
        private Sprite _burnGlow;
        private Sprite _burnGlowWide;
        private Sprite _burnGlowWideLeft;
        private Sprite _burnGlowWideRight;

        public string level => _level;

        public static LevelCore core
        {
            get => _core;
            set => _core = value;
        }

        public static void InitializeCollisionLists()
        {
            MonoMain.NloadMessage = "Loading Collision Lists";
            for (int index = 0; index < 10; ++index)
                _collisionLists.Enqueue(new List<object>());
        }

        public static List<object> GetNextCollisionList() => new List<object>();

        public static bool PassedChanceGroup(int group, float val) => group == -1 ? Rando.Float(1f) < val : _core._chanceGroups[group] < val;

        public static bool PassedChanceGroup2(int group, float val) => group == -1 ? Rando.Float(1f) < val : _core._chanceGroups2[group] < val;

        public static float GetChanceGroup2(int group) => group == -1 ? Rando.Float(1f) : _core._chanceGroups2[group];

        public bool simulatePhysics
        {
            get => _simulatePhysics;
            set => _simulatePhysics = value;
        }

        public static bool sendCustomLevels
        {
            get => _core.sendCustomLevels;
            set => _core.sendCustomLevels = value;
        }

        public static Level current
        {
            get => _core.nextLevel == null ? _core.currentLevel : _core.nextLevel;
            set => _core.nextLevel = value;
        }

        public static Level activeLevel
        {
            get => _core.currentLevel;
            set => _core.currentLevel = value;
        }

        public Color backgroundColor
        {
            get => _backgroundColor;
            set => _backgroundColor = value;
        }

        public static void Add(Thing thing)
        {
            if (_core.currentLevel == null)
                return;
            _core.currentLevel.AddThing(thing);
        }

        public static void Remove(Thing thing)
        {
            if (Corderator.instance != null && !Recorderator.Playing && thing.shouldhavevessel)
            {
                if (Corderator.instance.somethingMap.Contains(thing))
                {
                    int z = Corderator.instance.somethingMap[thing];
                    Corderator.instance.somethingMapped[z].deleteTime = Corderator.instance.cFrame;
                }
            }
            if (_core.currentLevel == null)
                return;
            _core.currentLevel.RemoveThing(thing);
        }

        public static void ClearThings()
        {
            if (_core.currentLevel == null)
                return;
            _core.currentLevel.Clear();
        }

        public static void UpdateCurrentLevel()
        {
            if (_core.currentLevel == null)
                return;
            _core.currentLevel.DoUpdate();
        }

        public static void DrawCurrentLevel()
        {
            if (_core.currentLevel == null)
                return;
            _core.currentLevel.DoDraw();
        }

        public static T First<T>()
        {
            IEnumerable<Thing> thing = current.things[typeof(T)];
            return thing.Count() > 0 ? (T)(object)thing.First() : default(T);
        }

        public T FirstOfType<T>()
        {
            IEnumerable<Thing> thing = things[typeof(T)];
            return thing.Count() > 0 ? (T)(object)thing.First() : default(T);
        }

        public QuadTreeObjectList things => _things;

        public string id => _id;

        public Camera camera
        {
            get => _camera;
            set => _camera = value;
        }

        public NetLevelStatus networkStatus => _networkStatus;

        public bool initialized => _initialized;

        public bool initializeFunctionHasBeenRun => _initialized && !_initializeLater;

        public bool waitingOnNewData
        {
            get => _waitingOnNewData;
            set => _waitingOnNewData = value;
        }

        public virtual void DoInitialize()
        {
            if (waitingOnNewData)
            {
                _initializeLater = true;
                _initialized = true;
            }
            else if (!_initialized)
            {
                GhostManager.context.TransferPendingGhosts();
                Random generator = Rando.generator;
                Rando.generator = new Random(seed + 2500);
                InitChanceGroups();
                Rando.generator = generator;
                Initialize();
                if (!Network.isActive || Network.inLobby)
                    DoStart();
                _things.RefreshState();
                CalculateBounds();
                _initialized = true;
                if (_centeredView)
                    camera.centerY -= (float)(((Graphics.aspect * camera.width) - (9f / 16f * camera.width)) / 2);
                if (!VirtualTransition.active)
                    StaticRenderer.Update();
                foreach (BlockGroup block in _things[typeof(BlockGroup)])
                {
                    block.PreLevelInitialize();
                }
                foreach (AutoBlock block in _things[typeof(AutoBlock)])
                {
                    block.PreLevelInitialize();
                }
                foreach (AutoPlatform autoPlatform in _things[typeof(AutoPlatform)])
                {
                    autoPlatform.PreLevelInitialize();
                }
                if (!Network.isActive)
                    return;
                ClientReady(DuckNetwork.localConnection);
            }
            else
            {
                foreach (Thing thing in _things)
                    thing.AddToLayer();
                foreach (BlockGroup block in _things[typeof(BlockGroup)])
                {
                    block.PreLevelInitialize();
                }
                foreach (AutoBlock block in _things[typeof(AutoBlock)])
                {
                    block.PreLevelInitialize();
                }
                foreach (AutoPlatform autoPlatform in _things[typeof(AutoPlatform)])
                {
                    autoPlatform.PreLevelInitialize();
                }
            }
        }

        public virtual void LevelLoaded()
        {
        }

        public virtual void Initialize()
        {
            _levelStart = true;
            Vote.ClearVotes();
        }

        private void DoStart()
        {
            if (_startCalled)
                return;
            Start();
            _startCalled = true;
        }

        public void SkipStart() => _startCalled = true;

        public virtual void Start()
        {
        }

        public virtual void Terminate() => Clear();

        public virtual void AddThing(Thing t)
        {
            if (Corderator.instance != null && !Recorderator.Playing && Corderator.instance.cFrame > 0 && t.shouldhavevessel)
            {
                Type typeoid = t.GetType();
                //if (t is TeamHat) DevConsole.Log("what1");
                if (SomethingSomethingVessel.tatchedVessels.ContainsKey(typeoid))
                {
                    //if (t is TeamHat) DevConsole.Log("what2");
                    object[] args = new object[] { t };
                    SomethingSomethingVessel vs = (SomethingSomethingVessel)Activator.CreateInstance(SomethingSomethingVessel.tatchedVessels[typeoid], args);
                    vs.addTime = Corderator.instance.cFrame;
                    Corderator.instance.MapSomeSomeVessel(vs);
                    goto FF;
                }
                /*if (t is Bullet b)
                {
                    BulletVessel bv = new BulletVessel(b) { addTime = Corderator.instance.cFrame };
                    Corderator.instance.MapSomeSomeVessel(bv);
                }*/
                if (t is Equipment e)
                {
                    EquipmentVessel ev = new EquipmentVessel(e) { addTime = Corderator.instance.cFrame };
                    Corderator.instance.MapSomeSomeVessel(ev);
                }
                else if (t is Holdable h)
                {
                    if (t is IAmADuck || t is Equipment) goto FF;
                    if (t is Gun)
                    {
                        GunVessel gv = new GunVessel(t) { addTime = Corderator.instance.cFrame };
                        Corderator.instance.MapSomeSomeVessel(gv);
                        goto FF;
                    }
                    HoldableVessel hv = new HoldableVessel(h) { addTime = Corderator.instance.cFrame };
                    Corderator.instance.MapSomeSomeVessel(hv);
                }
            }
            FF:

            Main.SpecialCode = "vay";
            if (Thread.CurrentThread == Content.previewThread && this != Content.previewLevel) Content.previewLevel.AddThing(t);
            else if (t is ThingContainer)
            {
                ThingContainer thingContainer = t as ThingContainer;
                if (thingContainer.bozocheck)
                {
                    foreach (Thing thing in thingContainer.things)
                    {
                        if (!Thing.CheckForBozoData(thing))
                            AddThing(thing);
                    }
                }
                else
                {
                    foreach (Thing thing in thingContainer.things)
                        AddThing(thing);
                }
            }
            else
            {
            Main.SpecialCode = "lay";
                if (t.level != this)
                {
                    _things.Add(t);
                    if (!skipInitialize)
                        t.Added(this, !bareInitialize, false);
                }
                if (!Network.isActive || t.connection != null)
                    return;
                t.connection = DuckNetwork.localConnection;
            }
        }

        public virtual void RemoveThing(Thing t)
        {
            if (t == null)
                return;
            t.DoTerminate();
            t.Removed();
            _things.Remove(t);
            if (t.ghostObject == null || !t.isServerForObject)
                return;
            GhostManager.context.RemoveLater(t.ghostObject);
        }

        public void Clear()
        {
            foreach (Thing thing in _things)
                thing.Removed();
            Layer.ClearLayers();
            _things.Clear();
        }

        public Vec2 topLeft => _topLeft;

        public Vec2 bottomRight => _bottomRight;

        public static void InitChanceGroups()
        {
            for (int index = 0; index < _core._chanceGroups.Count; ++index)
                _core._chanceGroups[index] = Rando.Float(1f);
            for (int index = 0; index < _core._chanceGroups2.Count; ++index)
                _core._chanceGroups2[index] = Rando.Float(1f);
        }

        public virtual string LevelNameData() => GetType().Name;

        public virtual string networkIdentifier => "";

        public static void UpdateLevelChange()
        {
            if (_core.nextLevel != null)
            {
                RumbleManager.ClearRumbles(new RumbleType?());
                if (_core.currentLevel is IHaveAVirtualTransition && _core.nextLevel is IHaveAVirtualTransition && !(_core.nextLevel is TeamSelect2))
                    VirtualTransition.GoVirtual();
                if (Network.isActive && activeLevel != null && !_core.nextLevel._sentLevelChange)
                {
                    byte levelIndex = DuckNetwork.levelIndex;
                    DevConsole.Log(DCSection.GhostMan, "|DGYELLOW|Performing level swap (" + levelIndex.ToString() + ")");
                    if (_core.currentLevel is TeamSelect2 && !(_core.nextLevel is TeamSelect2))
                        DuckNetwork.ClosePauseMenu();
                    if (!(_core.currentLevel is TeamSelect2) && _core.nextLevel is TeamSelect2)
                        DuckNetwork.ClosePauseMenu();
                    if (Network.isServer && !(_core.nextLevel is IConnectionScreen))
                    {
                        if (DuckNetwork.levelIndex > 250)
                            DuckNetwork.levelIndex = 1;
                        if (_core.nextLevel is TeamSelect2)
                            Network.ContextSwitch(0);
                        else
                            Network.ContextSwitch((byte)(DuckNetwork.levelIndex + 1U));
                        DuckNetwork.compressedLevelData = null;
                        string[] strArray = new string[5]
                        {
                          "|DGYELLOW|Incrementing level index (",
                          ( DuckNetwork.levelIndex - 1).ToString(),
                          "->",
                          null,
                          null
                        };
                        levelIndex = DuckNetwork.levelIndex;
                        strArray[3] = levelIndex.ToString();
                        strArray[4] = ")";
                        DevConsole.Log(DCSection.GhostMan, string.Concat(strArray));
                        if (!_core.nextLevel.suppressLevelMessage)
                            Send.Message(new NMLevel(_core.nextLevel));
                        _core.nextLevel.networkIndex = DuckNetwork.levelIndex;
                    }
                    else if (_core.nextLevel is IConnectionScreen)
                        Network.ContextSwitch(byte.MaxValue);
                    _core.nextLevel._sentLevelChange = true;
                }
                if (!VirtualTransition.active)
                {
                    if (NetworkDebugger.enabled && NetworkDebugger.Recorder.active != null)
                        Rando.generator = new Random(NetworkDebugger.Recorder.active.seed);
                    DamageManager.ClearHits();
                    Layer.ResetLayers();
                    HUD.ClearCorners();
                    if (_core.currentLevel != null)
                        _core.currentLevel.Terminate();
                    string str1 = _core.currentLevel != null ? _core.currentLevel.LevelNameData() : "null";
                    string str2 = _core.nextLevel != null ? _core.nextLevel.LevelNameData() : "null";
                    if (_core.nextLevel is XMLLevel && (_core.nextLevel as XMLLevel).level == "RANDOM")
                        DevConsole.Log(DCSection.General, "Level Switch (" + str1 + " -> Random Level(" + (_core.nextLevel as XMLLevel).seed.ToString() + "))");
                    else
                        DevConsole.Log(DCSection.General, "Level Switch (" + str1 + " -> " + str2 + ")");
                    _core.currentLevel = _core.nextLevel;
                    _core.nextLevel = null;
                    Layer.lighting = false;
                    VirtualTransition.core._transitionLevel = null;
                    AutoUpdatables.ClearSounds();
                    SequenceItem.sequenceItems.Clear();
                    Graphics.GarbageDisposal(true);
                    GC.Collect(1, GCCollectionMode.Optimized);
                    ++collectionCount;
                    if (!(_core.currentLevel is GameLevel))
                    {
                        if (MonoMain.timeInMatches > 0)
                        {
                            Global.data.timeInMatches.valueInt += MonoMain.timeInMatches / 60;
                            MonoMain.timeInMatches = 0;
                        }
                        if (MonoMain.timeInArcade > 0)
                        {
                            Global.data.timeInArcade.valueInt += MonoMain.timeInArcade / 60;
                            MonoMain.timeInArcade = 0;
                        }
                        if (MonoMain.timeInEditor > 0)
                        {
                            Global.data.timeInEditor.valueInt += MonoMain.timeInEditor / 60;
                            MonoMain.timeInEditor = 0;
                        }
                        if (!(_core.currentLevel is HighlightLevel))
                            Graphics.fadeAdd = 0f;
                        Steam.StoreStats();
                    }
                    foreach (Profile profile in Profiles.active)
                        profile.duck = null;
                    SFX.StopAllSounds();
                    _core.currentLevel.DoInitialize();
                    if (_core.currentLevel is XMLLevel && (_core.currentLevel as XMLLevel).data != null)
                    {
                        string path = (_core.currentLevel as XMLLevel).data.GetPath();
                        if (path != null)
                            DevConsole.Log(DCSection.General, "Level Initialized(" + path + ")");
                    }
                    if (MonoMain.pauseMenu != null && MonoMain.pauseMenu.inWorld)
                        _core.currentLevel.AddThing(MonoMain.pauseMenu);
                    if (Network.isActive && DuckNetwork.duckNetUIGroup != null && DuckNetwork.duckNetUIGroup.open)
                        _core.currentLevel.AddThing(DuckNetwork.duckNetUIGroup);
                    current._networkStatus = NetLevelStatus.WaitingForDataTransfer;
                    if (!(_core.currentLevel is IOnlyTransitionIn) && _core.currentLevel is IHaveAVirtualTransition && !(_core.currentLevel is TeamSelect2) && VirtualTransition.isVirtual)
                    {
                        if (current._readyForTransition)
                        {
                            VirtualTransition.GoUnVirtual();
                            Graphics.fade = 1f;
                        }
                        else
                        {
                            current._waitingOnTransition = true;
                            if (Network.isActive)
                                ConnectionStatusUI.Show();
                        }
                    }
                }
            }
            if (!current._waitingOnTransition || !current._readyForTransition)
                return;
            current._waitingOnTransition = false;
            VirtualTransition.GoUnVirtual();
            if (!Network.isActive)
                return;
            ConnectionStatusUI.Hide();
        }

        public virtual void OnMessage(NetMessage message)
        {
        }

        public virtual void OnNetworkConnecting(Profile p)
        {
        }

        public virtual void OnNetworkConnected(Profile p)
        {
        }

        public virtual void OnNetworkDisconnected(Profile p)
        {
        }

        public virtual void OnSessionEnded(DuckNetErrorInfo error)
        {
            current = error == null ? new ConnectionError("|RED|Disconnected from game.") : (Level)new ConnectionError(error.message);
            DuckNetwork.core.stopEnteringText = true;
        }

        public virtual void OnDisconnect(NetworkConnection n)
        {
        }

        public virtual void ClientReady(NetworkConnection c)
        {
            if (!initializeFunctionHasBeenRun)
                return;
            bool flag = true;
            foreach (Profile profile in DuckNetwork.profiles)
            {
                if (profile.connection != null && profile.connection.levelIndex != DuckNetwork.levelIndex)
                {
                    flag = false;
                    break;
                }
            }
            if (!flag)
                return;
            DevConsole.Log(DCSection.DuckNet, "|DGGREEN|All Clients ready! The level can begin...");
            Send.Message(new NMAllClientsReady());
        }

        public bool calledAllClientsReady => _calledAllClientsReady;

        public virtual void DoAllClientsReady()
        {
            if (_calledAllClientsReady)
                return;
            _calledAllClientsReady = true;
            OnAllClientsReady();
        }

        protected virtual void OnAllClientsReady()
        {
            _networkStatus = NetLevelStatus.Ready;
            current._readyForTransition = true;
            DoStart();
        }

        public void TransferComplete(NetworkConnection c)
        {
            transferCompleteCalled = true;
            _networkStatus = NetLevelStatus.WaitingForTransition;
            OnTransferComplete(c);
        }

        protected virtual void OnTransferComplete(NetworkConnection c)
        {
        }

        public virtual void SendLevelData(NetworkConnection c)
        {
        }

        public void IgnoreLowestPoint()
        {
            _lowestPointInitialized = true;
            lowestPoint = 999999f;
            _topLeft = new Vec2(-99999f, -99999f);
            _bottomRight = new Vec2(99999f, 99999f);
        }

        public void CalculateBounds()
        {
            _lowestPointInitialized = true;
            CameraBounds cameraBounds = FirstOfType<CameraBounds>();
            if (cameraBounds != null)
            {
                _topLeft = new Vec2(cameraBounds.x - (int)cameraBounds.wide / 2, cameraBounds.y - (int)cameraBounds.high / 2);
                _bottomRight = new Vec2(cameraBounds.x + (int)cameraBounds.wide / 2, cameraBounds.y + (int)cameraBounds.high / 2);
                lowestPoint = _bottomRight.y;
                highestPoint = _topLeft.y;
            }
            else
            {
                _topLeft = new Vec2(99999f, 99999f);
                _bottomRight = new Vec2(-99999f, -99999f);
                foreach (Block block in _things[typeof(Block)])
                {
                    if (!(block is RockWall) && block.y <= 7500)
                    {
                        if (block.right > _bottomRight.x)
                            _bottomRight.x = block.right;
                        if (block.left < _topLeft.x)
                            _topLeft.x = block.left;
                        if (block.bottom > _bottomRight.y)
                            _bottomRight.y = block.bottom;
                        if (block.top < _topLeft.y)
                            _topLeft.y = block.top;
                    }
                }
                foreach (AutoPlatform autoPlatform in _things[typeof(AutoPlatform)])
                {
                    if (autoPlatform.y <= 7500)
                    {
                        if (autoPlatform.right > _bottomRight.x)
                            _bottomRight.x = autoPlatform.right;
                        if (autoPlatform.left < _topLeft.x)
                            _topLeft.x = autoPlatform.left;
                        if (autoPlatform.bottom > _bottomRight.y)
                            _bottomRight.y = autoPlatform.bottom;
                        if (autoPlatform.top < _topLeft.y)
                            _topLeft.y = autoPlatform.top;
                    }
                }
                lowestPoint = _bottomRight.y;
                highestPoint = topLeft.y;
            }
        }

        public bool HasChecksumReply(NetworkConnection pConnection)
        {
            bool flag;
            checksumReplies.TryGetValue(pConnection, out flag);
            return flag;
        }

        public void ChecksumReplied(NetworkConnection pConnection) => checksumReplies[pConnection] = true;

        public bool levelIsUpdating => _core.nextLevel == null && (!Network.isActive || _startCalled) && !_waitingOnTransition && transferCompleteCalled;

        public virtual void DoUpdate()
        {
            if (_updateWaitFrames > 0)
            {
                if (!_refreshState)
                {
                    _things.RefreshState();
                    VirtualTransition.Update();
                    _refreshState = true;
                }
                --_updateWaitFrames;
                if (_lowestPointInitialized)
                    return;
                CalculateBounds();
            }
            else
            {
                Level currentLevel = _core.currentLevel;
                _core.currentLevel = this;
                if (Graphics.flashAdd > 0)
                    Graphics.flashAdd -= flashDissipationSpeed;
                else
                    Graphics.flashAdd = 0f;
                if (_levelStart)
                {
                    Graphics.fade = Lerp.Float(Graphics.fade, 1f, 0.05f);
                    if (Graphics.fade == 1)
                        _levelStart = false;
                }
                if (_core.nextLevel == null && initializeFunctionHasBeenRun && levelMessages.Count > 0)
                {
                    for (int index = 0; index < levelMessages.Count; ++index)
                    {
                        doingOnLoadedMessage = true;
                        if (levelMessages[index].OnLevelLoaded())
                        {
                            levelMessages.RemoveAt(index);
                            --index;
                        }
                        doingOnLoadedMessage = false;
                    }
                }
                if (levelIsUpdating)
                {
                    if (_camera != null)
                        _camera.DoUpdate();
                    Update();
                    Layer.UpdateLayers();
                    UpdateThings();
                    PostUpdate();
                    _things.RefreshState();
                    Vote.Update();
                    HUD.Update();
                }
                else
                    _things.RefreshState();
                if (!_notifiedReady && _initialized && !waitingOnNewData)
                {
                    DevConsole.Log(DCSection.GhostMan, "Initializing level (" + DuckNetwork.levelIndex.ToString() + ")");
                    if (_initializeLater)
                    {
                        _initialized = false;
                        _initializeLater = false;
                        DoInitialize();
                    }
                    _notifiedReady = true;
                }
                VirtualTransition.Update();
                ConnectionStatusUI.Update();
                //if (!this._aiInitialized)
                //{
                //    AI.InitializeLevelPaths();
                //    this._aiInitialized = true;
                //}
                if (skipCurrentLevelReset)
                    return;
                _core.currentLevel = currentLevel;
            }
        }

        public virtual void PostUpdate()
        {
        }

        public virtual void NetworkDebuggerPrepare()
        {
        }
        public void AddUpdateOnce(Thing T)
        {
            occasionalupdatethingspending.Add(T);
        }
        public List<Thing> occasionalupdatethingspending = new List<Thing>();
        public List<Thing> occasionalupdatethings = new List<Thing>();
        public virtual void UpdateThings()
        {
            Network.PostDraw();
            IEnumerable<Thing> thing1 = things[typeof(IComplexUpdate)];
            if (Network.isActive)
            {
                foreach (Thing thing2 in thing1)
                {
                    if (thing2.shouldRunUpdateLocally)
                        (thing2 as IComplexUpdate).OnPreUpdate();
                }
                foreach (Thing update in _things.RealupdateList)//_things.updateList)
                {
                    if (update.active)
                    {
                        if (update.shouldRunUpdateLocally)
                            update.DoUpdate();
                    }
                    else
                        update.InactiveUpdate();
                    if (_core.nextLevel != null)
                        break;
                }
                occasionalupdatethings = new List<Thing>(occasionalupdatethingspending);
                occasionalupdatethingspending.Clear();
                foreach (Thing update in occasionalupdatethings)
                {
                    if (update.active)
                    {
                        if (update.shouldRunUpdateLocally)
                            update.DoUpdate();
                    }
                    else
                        update.InactiveUpdate();
                    if (_core.nextLevel != null)
                        break;
                }
                foreach (Thing thing3 in thing1)
                {
                    if (thing3.shouldRunUpdateLocally)
                        (thing3 as IComplexUpdate).OnPostUpdate();
                }
            }
            else
            {
                foreach (Thing thing4 in thing1)
                    (thing4 as IComplexUpdate).OnPreUpdate();
                foreach (Thing update in _things.RealupdateList)//_things.updateList)
                {
                    if (update.active && update.level != null)
                        update.DoUpdate();
                    if (_core.nextLevel != null)
                        break;
                }
                occasionalupdatethings = new List<Thing>(occasionalupdatethingspending);
                occasionalupdatethingspending.Clear();
                foreach (Thing update in occasionalupdatethings)
                {
                    if (update.active && update.level != null)
                        update.DoUpdate();
                    if (_core.nextLevel != null)
                        break;
                }
                foreach (Thing thing5 in thing1)
                    (thing5 as IComplexUpdate).OnPostUpdate();
            }
            occasionalupdatethings.Clear();
            //occasionalupdatethingspending.Clear();
        }

        public virtual void Update()
        {
        }

        public bool clearScreen
        {
            get => _clearScreen;
            set => _clearScreen = value;
        }

        public virtual void StartDrawing()
        {
        }
        public RenderTarget2D rd2;
        public List<FullscreenMaterial> fullscreenShaders = new List<FullscreenMaterial>();
        public static void AddFullscreenMaterial(FullscreenMaterial material)
        {
            current.fullscreenShaders.Add(material);
            material.added = true;
        }
        public static void RemoveFullscreenMaterial(FullscreenMaterial material)
        {
            current.fullscreenShaders.Remove(material);
            material.added = false;
        }
        public virtual void DoDraw()
        {
            if (DGRSettings.UncappedFPS)
                camera.LerpCamera();

            bool currentlyShadering = false;

            bool cameraD = false;
            List<FluidPuddle> lavaPuddles = new List<FluidPuddle>();
            List<FullscreenMaterial> activeFS = new List<FullscreenMaterial>();
            if (fullscreenShaders.Count > 0)
            {
                fullscreenShaders.RemoveAll(item => item.removed);
                for (int i = 0; i < fullscreenShaders.Count; i++)
                {
                    FullscreenMaterial mt = fullscreenShaders[i];
                    if (mt.active && (mt.thing == null || (!mt.thing.removeFromLevel && mt.thing.currentlyDrawing)))
                    {
                        activeFS.Add(mt);
                    }
                }
            }

            if (Graphics.currentRenderTarget == null && !VirtualTransition.active && activeFS.Count > 0)
            {
                if (rd2 != null) rd2.Dispose();
                int width = (int)Resolution._device.PreferredBackBufferWidth;
                int height = (int)Resolution._device.PreferredBackBufferHeight;

                if (Math.Abs(((float)width / (float)height) - camera.aspect) > 0.01f)
                {
                    cameraD = true;
                    height = (int)(width / camera.aspect) + 1;
                }

                Layer.Console.visible = false;
                Layer.HUD.visible = false;
                rd2 = new RenderTarget2D(width, height);
                Graphics.SettingForShader = true;
                Graphics.SetRenderTarget(rd2);
                Graphics.SettingForShader = false;
                currentlyShadering = true;
            }
            StartDrawing();
            foreach (IDrawToDifferentLayers toDifferentLayers in things[typeof(IDrawToDifferentLayers)])
                toDifferentLayers.OnDrawLayer(Layer.PreDrawLayer);
            Layer.DrawTargetLayers();
            Vec3 vec = backgroundColor.ToVector3() * Graphics.fade;
            vec.x += Graphics.flashAddRenderValue;
            vec.y += Graphics.flashAddRenderValue;
            vec.z += Graphics.flashAddRenderValue;
            vec = new Vec3(vec.x + Graphics.fadeAddRenderValue, vec.y + Graphics.fadeAddRenderValue, vec.z + Graphics.fadeAddRenderValue);
            Color color = new Color(vec)
            {
                a = backgroundColor.a
            };
            color *= Layer.Parallax.fade;
            if (clearScreen)
            {
                if (!Options.Data.fillBackground)
                {
                    Graphics.Clear(color);
                }
                else
                {
                    Graphics.Clear(Color.Black);
                    Graphics.SetFullViewport();
                    Material material = Graphics.material;
                    Graphics.material = null;
                    Graphics.screen.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Matrix.Identity);
                    Graphics.DrawRect(new Vec2(0f, 0f), new Vec2(Resolution.current.x, Resolution.current.y), color, -1f);
                    Graphics.screen.End();
                    Graphics.material = material;
                    Graphics.RestoreOldViewport();
                }
            }
            if (Recorder.currentRecording != null) Recorder.currentRecording.LogBackgroundColor(backgroundColor);
            BeforeDraw();
            Graphics.screen.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, camera.getMatrix());
            Draw();
            things.Draw();

            Graphics.screen.End();
            if (DevConsole.splitScreen && this is GameLevel) SplitScreen.Draw();
            else Layer.DrawLayers();
            if (DevConsole.rhythmMode && this is GameLevel)
            {
                Graphics.screen.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Layer.HUD.camera.getMatrix());
                RhythmMode.Draw();
                Graphics.screen.End();
            }
            AfterDrawLayers();

            if (currentlyShadering)
            {

                if (!cameraD) Graphics.SettingForShader = true;
                Graphics.SetRenderTarget(null);
                Graphics.SettingForShader = false;

                bool multiLayering = false;
                if (activeFS.Count > 1) multiLayering = true;
                List<RenderTarget2D> dispose = new List<RenderTarget2D>() { rd2 };
                for (int i = 0; i < activeFS.Count; i++)
                {
                    RenderTarget2D rd3 = null;
                    if (multiLayering && i != activeFS.Count - 1)
                    {
                        int width = (int)Resolution._device.PreferredBackBufferWidth;
                        int height = (int)Resolution._device.PreferredBackBufferHeight;
                        if (Math.Abs(((float)width / (float)height) - camera.aspect) > 0.01f)
                        {
                            cameraD = true;
                            height = (int)(width / camera.aspect) + 1;
                        }
                        rd3 = new RenderTarget2D(width, height);

                        dispose.Add(rd3);
                        Graphics.SettingForShader = true;
                        Graphics.SetRenderTarget(rd3);
                        Graphics.SettingForShader = false;
                    }

                    Graphics.screen.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Matrix.CreateScale(1, 1, 1));
                    Graphics.Clear(backgroundColor);
#if DEBUG
                    if (!Network.isActive)
                    {
                        Graphics.DrawString("this stuff only shows in debug builds and offline -NiK0", new Vec2(0, 0), Color.White, 1, null, 2);
                        Graphics.DrawString(Resolution.size.ToString(), new Vec2(0, 16), Color.White, 1, null, 2);
                        Graphics.DrawString(Resolution.current.dimensions.ToString(), new Vec2(0, 32), Color.White, 1, null, 2);
                        if (Graphics._screenViewport != null) Graphics.DrawString(Graphics._screenViewport.Value.Bounds.ToString(), new Vec2(0, 48), Color.White, 1, null, 2);
                        int width = (int)Resolution._device.PreferredBackBufferWidth;
                        int height = (int)Resolution._device.PreferredBackBufferHeight;
                        Graphics.DrawString($"{width}:{height} didRSVP:{cameraD}", new Vec2(0, 64), Color.White, 1, null, 2);

                    }
#endif
                    Graphics.material = activeFS[i];
                    Graphics.Draw(rd2, 0, 0, 1, 1);
                    Graphics.material = null;
                    Graphics.screen.End();
                    if (multiLayering)
                    {
                        if (!cameraD) Graphics.SettingForShader = true;
                        Graphics.SetRenderTarget(null);
                        Graphics.SettingForShader = true;
                        rd2 = rd3;
                    }
                }
                Layer.Console.visible = true;
                Layer.Console.Draw(true);
                Layer.HUD.visible = true;
                Layer.HUD.Draw(true);
                for (int i = 0; i < dispose.Count; i++)
                {
                    dispose[i].Dispose();
                }
            }
        }

        public virtual void InitializeDraw(Layer l)
        {
            if (l != Layer.HUD || !_centeredView)
                return;
            float num = (float)(Resolution.size.x * Graphics.aspect - Resolution.size.x * (9f / 16f)); //for the love of jod keep the "f" on these otherwise reality breaks -NiK0
            if (num <= 0)
                return;
            Graphics.screen.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Matrix.Identity);
            Graphics.DrawRect(Vec2.Zero, new Vec2(Resolution.size.x, num / 2f), Color.Black, (Depth)0.9f);
            Graphics.DrawRect(new Vec2(0f, Resolution.size.y - num / 2f), new Vec2(Resolution.size.x, Resolution.size.y), Color.Black, (Depth)0.9f);
            Graphics.screen.End();
        }

        public virtual void BeforeDraw()
        {
        }

        public virtual void AfterDrawLayers()
        {
        }

        public virtual void Draw()
        {
        }

        public virtual void PreDrawLayer(Layer layer)
        {
        }

        public virtual void PostDrawLayer(Layer layer)
        {
            foreach (IEngineUpdatable engineUpdatable in MonoMain.core.engineUpdatables)
                engineUpdatable.OnDrawLayer(layer);
            foreach (IDrawToDifferentLayers toDifferentLayers in things[typeof(IDrawToDifferentLayers)])
                toDifferentLayers.OnDrawLayer(layer);

            DrawingLayer? drawingLayer = Marker.DrawingContextAttribute.DrawingLayerFromLayer(layer);

            if (drawingLayer != null)
            {
                Marker.DrawingContextAttribute.ExecuteAll(drawingLayer.Value);
                Marker.ComplexDrawingContextAttribute.ExecuteAll(layer);
            }
            
            RenderDelegates.Layers.InvokeFor(layer);
            
            
            if (layer == Layer.Console)
            {
                DevConsole.Draw();
                if (!Network.isActive)
                    return;
                DuckNetwork.Draw();
            }
            else if (layer == Layer.Foreground)
            {
                if (layer.fade <= 0)
                    return;
                HUD.DrawForeground();
            }
            else if (layer == Layer.HUD)
            {
                if (layer.fade <= 0)
                    return;
                Vote.Draw();
                HUD.Draw();
                ConnectionStatusUI.Draw();
            }
            else
            {
                if (layer == Layer.Lighting)
                    return;
                if (layer == Layer.Glow && Options.Data.fireGlow)
                {
                    foreach (MaterialThing materialThing in things[typeof(MaterialThing)])
                    {
                        switch (materialThing)
                        {
                            case Holdable _ when materialThing.heat > 0.3f && materialThing.physicsMaterial == PhysicsMaterial.Metal:
                                if (_burnGlow == null)
                                {
                                    _burnGlow = new Sprite("redHotGlow");
                                    _burnGlow.CenterOrigin();
                                }
                                _burnGlow.alpha = (Math.Min(materialThing.heat, 1f) / 1f - 0.2f);
                                _burnGlow.scale = new Vec2((materialThing.width + 22f) / _burnGlow.width, (materialThing.height + 22f) / _burnGlow.height);
                                Vec2 center = materialThing.rectangle.Center;
                                Graphics.Draw(_burnGlow, center.x, center.y);
                                Graphics.Draw(_burnGlow, center.x, center.y);
                                break;
                            case FluidPuddle _:
                                FluidPuddle fluidPuddle = materialThing as FluidPuddle;
                                if ((fluidPuddle.onFire || fluidPuddle.data.heat > 0.5f) && fluidPuddle.alpha > 0.5f)
                                {
                                    double num1 = fluidPuddle.right - fluidPuddle.left;
                                    float num2 = 16f;
                                    Math.Sin(fluidPuddle.fluidWave);
                                    if (_burnGlowWide == null)
                                    {
                                        _burnGlowWide = new Sprite("redGlowWideSharp");
                                        _burnGlowWide.CenterOrigin();
                                        _burnGlowWide.alpha = 0.75f;
                                        _burnGlowWideLeft = new Sprite("redGlowWideLeft");
                                        _burnGlowWideLeft.center = new Vec2(_burnGlowWideLeft.width, _burnGlowWideLeft.height / 2);
                                        _burnGlowWideLeft.alpha = 0.75f;
                                        _burnGlowWideRight = new Sprite("redGlowWideRight");
                                        _burnGlowWideRight.center = new Vec2(0f, _burnGlowWideRight.height / 2);
                                        _burnGlowWideRight.alpha = 0.75f;
                                    }
                                    double num3 = num2;
                                    int num4 = (int)Math.Floor(num1 / num3);
                                    if (fluidPuddle.collisionSize.y > 8f)
                                    {
                                        _burnGlowWide.xscale = 16f;
                                        for (int index = 0; index < num4; ++index)
                                        {
                                            float x = (fluidPuddle.bottomLeft.x + index * num2 + 11f - 8f);
                                            float y = fluidPuddle.top - 1f + (float)Math.Sin(fluidPuddle.fluidWave + index * 0.7f);
                                            Graphics.Draw(_burnGlowWide, x, y);
                                            if (index == 0)
                                                Graphics.Draw(_burnGlowWideLeft, x, y);
                                            else if (index == num4 - 1)
                                                Graphics.Draw(_burnGlowWideRight, x + 16f, y);
                                        }
                                        break;
                                    }
                                    Graphics.doSnap = false;
                                    _burnGlowWide.xscale = fluidPuddle.collisionSize.x;
                                    Graphics.Draw(_burnGlowWide, fluidPuddle.left, fluidPuddle.bottom - 2f);
                                    Graphics.Draw(_burnGlowWideLeft, fluidPuddle.left, fluidPuddle.bottom - 2f);
                                    Graphics.Draw(_burnGlowWideRight, fluidPuddle.right, fluidPuddle.bottom - 2f);
                                    Graphics.doSnap = true;
                                    break;
                                }
                                break;
                        }
                        materialThing.DrawGlow();
                    }
                    foreach (SmallFire smallFire in things[typeof(SmallFire)])
                    {
                        if (_burnGlow == null)
                        {
                            _burnGlow = new Sprite("redGlow");
                            _burnGlow.CenterOrigin();
                        }
                        _burnGlow.alpha = 0.65f * smallFire.alpha;
                        Graphics.Draw(_burnGlow, smallFire.x, smallFire.y - 4f);
                    }
                }
                else if (layer == Layer.Virtual)
                {
                    VirtualTransition.Draw();
                }
                else
                {
                    if (layer != Layer.Game || !NetworkDebugger.enabled || VirtualTransition.active || this is NetworkDebugger)
                        return;
                    NetworkDebugger.DrawInstanceGameDebug();
                }
            }
        }

        public static T Nearest<T>(float x, float y, Thing ignore, Layer layer) => current.NearestThing<T>(new Vec2(x, y), ignore, layer);

        public static T Nearest<T>(float x, float y, Thing ignore) => current.NearestThing<T>(new Vec2(x, y), ignore);

        public static T Nearest<T>(float x, float y) => current.NearestThing<T>(new Vec2(x, y));
        public static T Nearest<T>(Vec2 point, float maxdistance) => current.NearestThing<T>(point, maxdistance);

        public static T Nearest<T>(Vec2 point, float maxdistance, Thing ignore) => current.NearestThing<T>(point, maxdistance, ignore);
        public static T Nearest<T>(Vec2 p) => current.NearestThing<T>(p);

        public static T Nearest<T>(Vec2 point, Thing ignore, int nearIndex, Layer layer) => current.NearestThing<T>(point, ignore, nearIndex, layer);

        public static T Nearest<T>(Vec2 point, Thing ignore, int nearIndex) => current.NearestThing<T>(point, ignore, nearIndex);

        public static T CheckCircle<T>(float p1x, float p1y, float radius, Thing ignore) => current.CollisionCircle<T>(new Vec2(p1x, p1y), radius, ignore);

        public static T CheckCircle<T>(float p1x, float p1y, float radius) => current.CollisionCircle<T>(new Vec2(p1x, p1y), radius);

        public static T CheckCircle<T>(Vec2 p1, float radius, Thing ignore) => current.CollisionCircle<T>(p1, radius, ignore);

        public static T CheckCircle<T>(Vec2 p1, float radius) => current.CollisionCircle<T>(p1, radius);

        public static IEnumerable<T> CheckCircleAll<T>(Vec2 p1, float radius)
        {
            if (radius > 100000) // for JadeMod MegaBuster, Why the fuck do they use 2.1474836E+09f for the radius. *pain
                return CheckCircleAllOld<T>(p1, radius);
            return current.CollisionCircleAll<T>(p1, radius);
        }

        public static IEnumerable<T> CheckCircleAllOld<T>(Vec2 p1, float radius) => current.CollisionCircleAllOld<T>(p1, radius);

        public T CollisionCircle<T>(float p1x, float p1y, float radius, Thing ignore) => CollisionCircle<T>(new Vec2(p1x, p1y), radius, ignore);

        public T CollisionCircle<T>(float p1x, float p1y, float radius) => CollisionCircle<T>(new Vec2(p1x, p1y), radius);

        public static T CheckRect<T>(float p1x, float p1y, float p2x, float p2y, Thing ignore) => current.CollisionRect<T>(new Vec2(p1x, p1y), new Vec2(p2x, p2y), ignore);

        public static T CheckRect<T>(float p1x, float p1y, float p2x, float p2y) => current.CollisionRect<T>(new Vec2(p1x, p1y), new Vec2(p2x, p2y));

        public static T CheckRectFilter<T>(Vec2 p1, Vec2 p2, Predicate<T> filter) => current.CollisionRectFilter(p1, p2, filter);

        public static T CheckRect<T>(Vec2 p1, Vec2 p2, Thing ignore) => current.CollisionRect<T>(p1, p2, ignore);

        public static T CheckRect<T>(Vec2 p1, Vec2 p2) => current.CollisionRect<T>(p1, p2);

        public static List<T> CheckRectAll<T>(Vec2 p1, Vec2 p2, List<T> outList) => current.CollisionRectAllDan(p1, p2, outList);

        public static IEnumerable<T> CheckRectAll<T>(Vec2 p1, Vec2 p2)
        {
            if (p1.Distance(p2) > 100000)
            {
                return current.CollisionRectAll<T>(p1, p2, null);
            }
            return current.CollisionRectAllDan<T>(p1, p2, null); // spooky time
            //return Level.current.CollisionRectAll<T>(p1, p2, null);
        }
        public static IEnumerable<T> OldCheckRectAll<T>(Vec2 p1, Vec2 p2)
        {
            return current.CollisionRectAll<T>(p1, p2, null);
        }
        public static IEnumerable<T> CheckRectAllDan<T>(Vec2 p1, Vec2 p2)
        {
            return current.CollisionRectAllDan<T>(p1, p2, null);
        }
        public static IEnumerable<T> CheckRectAllNiK0<T>(Vec2 p1, Vec2 p2)
        {
            return current.CollisionRectAllNiK0<T>(p1, p2, null);
        }

        public T CollisionRect<T>(float p1x, float p1y, float p2x, float p2y, Thing ignore) => CollisionRect<T>(new Vec2(p1x, p1y), new Vec2(p2x, p2y), ignore);

        public T CollisionRect<T>(float p1x, float p1y, float p2x, float p2y) => CollisionRect<T>(new Vec2(p1x, p1y), new Vec2(p2x, p2y));

        public static T CheckLine<T>(float p1x, float p1y, float p2x, float p2y, Thing ignore) => current.CollisionLine<T>(new Vec2(p1x, p1y), new Vec2(p2x, p2y), ignore);

        public static T CheckLine<T>(float p1x, float p1y, float p2x, float p2y) => current.CollisionLine<T>(new Vec2(p1x, p1y), new Vec2(p2x, p2y));

        public static T CheckLine<T>(
          float p1x,
          float p1y,
          float p2x,
          float p2y,
          out Vec2 position,
          Thing ignore)
        {
            return current.CollisionLine<T>(new Vec2(p1x, p1y), new Vec2(p2x, p2y), out position, ignore);
        }

        public static T CheckLine<T>(float p1x, float p1y, float p2x, float p2y, out Vec2 position) => current.CollisionLine<T>(new Vec2(p1x, p1y), new Vec2(p2x, p2y), out position);

        public static T CheckLine<T>(Vec2 p1, Vec2 p2, Thing ignore) => current.CollisionLine<T>(p1, p2, ignore);

        public static T CheckLine<T>(Vec2 p1, Vec2 p2) => current.CollisionLine<T>(p1, p2);
        public static T OldCheckLine<T>(Vec2 p1, Vec2 p2) => current.OldCollisionLine<T>(p1, p2);

        public static T CheckLine<T>(Vec2 p1, Vec2 p2, out Vec2 position, Thing ignore) => current.CollisionLine<T>(p1, p2, out position, ignore);

        public static T CheckLine<T>(Vec2 p1, Vec2 p2, out Vec2 position) => current.CollisionLine<T>(p1, p2, out position);

        public T CollisionLine<T>(float p1x, float p1y, float p2x, float p2y, Thing ignore) => CollisionLine<T>(new Vec2(p1x, p1y), new Vec2(p2x, p2y), ignore);

        public T CollisionLine<T>(float p1x, float p1y, float p2x, float p2y) => CollisionLine<T>(new Vec2(p1x, p1y), new Vec2(p2x, p2y));

        public static IEnumerable<T> CheckLineAll<T>(Vec2 p1, Vec2 p2) => current.CollisionLineAll<T>(p1, p2);

        public IEnumerable<T> CheckLineAll<T>(float p1x, float p1y, float p2x, float p2y) => CollisionLineAll<T>(new Vec2(p1x, p1y), new Vec2(p2x, p2y));

        public static T CheckPoint<T>(float x, float y, Thing ignore, Layer layer) => current.CollisionPoint<T>(new Vec2(x, y), ignore, layer);

        public static T CheckPoint<T>(float x, float y, Thing ignore) => current.CollisionPoint<T>(new Vec2(x, y), ignore);

        public static Thing CheckPoint(Type pType, float x, float y, Thing ignore) => current.CollisionPoint(pType, new Vec2(x, y), ignore);

        public static T CheckPoint<T>(float x, float y) => current.CollisionPoint<T>(new Vec2(x, y));

        public static T CheckPointFast<T>(float x, float y) => current.CollisionPointFast<T>(new Vec2(x, y));

        public static T CheckPointPlacementLayer<T>(float x, float y, Thing ignore = null, Layer layer = null) => current.CollisionPointPlacementLayer<T>(new Vec2(x, y), ignore, layer);

        public static T CheckPoint<T>(Vec2 point, Thing ignore, Layer layer) => current.CollisionPoint<T>(point, ignore, layer);

        public static T CheckPoint<T>(Vec2 point, Thing ignore) => current.CollisionPoint<T>(point, ignore);

        public static T CheckPoint<T>(Vec2 point) => current.CollisionPoint<T>(point);

        public static T CheckPointPlacementLayer<T>(Vec2 point, Thing ignore = null, Layer layer = null) => current.CollisionPointPlacementLayer<T>(point, ignore, layer);

        public static IEnumerable<T> CheckPointAll<T>(float x, float y, Layer layer) => current.CollisionPointAll<T>(new Vec2(x, y), layer);

        public static IEnumerable<T> CheckPointAll<T>(float x, float y) => current.CollisionPointAll<T>(new Vec2(x, y));

        public static IEnumerable<T> CheckPointAll<T>(Vec2 point, Layer layer) => current.CollisionPointAll<T>(point, layer);

        public static IEnumerable<T> CheckPointAll<T>(Vec2 point) => current.CollisionPointAll<T>(point);

        public T CollisionPoint<T>(float x, float y, Thing ignore, Layer layer) => CollisionPoint<T>(new Vec2(x, y), ignore, layer);

        public T CollisionPoint<T>(float x, float y, Thing ignore) => CollisionPoint<T>(new Vec2(x, y), ignore);

        public T CollisionPoint<T>(float x, float y) => CollisionPoint<T>(new Vec2(x, y));

        public Thing nearest_single(
          Vec2 point,
          HashSet<Thing> things,
          Thing ignore,
          Layer layer,
          bool placementLayer = false)
        {
            Thing thing1 = null;
            float num = float.MaxValue;
            foreach (Thing thing2 in things)
            {
                if (!thing2.removeFromLevel && thing2 != ignore && (layer == null || (placementLayer || thing2.layer == layer) && thing2.placementLayer == layer))
                {
                    float lengthSq = (point - thing2.position).lengthSq;
                    if (lengthSq < num)
                    {
                        num = lengthSq;
                        thing1 = thing2;
                    }
                }
            }
            return thing1;
        }

        public Thing nearest_single(Vec2 point, HashSet<Thing> things, Thing ignore)
        {
            Thing thing1 = null;
            float num = float.MaxValue;
            foreach (Thing thing2 in things)
            {
                if (!thing2.removeFromLevel && thing2 != ignore)
                {
                    float lengthSq = (point - thing2.position).lengthSq;
                    if (lengthSq < num)
                    {
                        num = lengthSq;
                        thing1 = thing2;
                    }
                }
            }
            return thing1;
        }

        public Thing nearest_single(Vec2 point, HashSet<Thing> things)
        {
            Thing thing1 = null;
            float num = float.MaxValue;
            foreach (Thing thing2 in things)
            {
                if (!thing2.removeFromLevel)
                {
                    float lengthSq = (point - thing2.position).lengthSq;
                    if (lengthSq < num)
                    {
                        num = lengthSq;
                        thing1 = thing2;
                    }
                }
            }
            return thing1;
        }

        public Thing nearest_single(
          Vec2 point,
          IEnumerable<Thing> things,
          Thing ignore,
          Layer layer,
          bool placementLayer = false)
        {
            Thing thing1 = null;
            float num = float.MaxValue;
            foreach (Thing thing2 in things)
            {
                if (!thing2.removeFromLevel && thing2 != ignore && (layer == null || (placementLayer || thing2.layer == layer) && thing2.placementLayer == layer))
                {
                    float lengthSq = (point - thing2.position).lengthSq;
                    if (lengthSq < num)
                    {
                        num = lengthSq;
                        thing1 = thing2;
                    }
                }
            }
            return thing1;
        }

        public Thing nearest_single(Vec2 point, IEnumerable<Thing> things, Thing ignore)
        {
            Thing thing1 = null;
            float num = float.MaxValue;
            foreach (Thing thing2 in things)
            {
                if (!thing2.removeFromLevel && thing2 != ignore)
                {
                    float lengthSq = (point - thing2.position).lengthSq;
                    if (lengthSq < num)
                    {
                        num = lengthSq;
                        thing1 = thing2;
                    }
                }
            }
            return thing1;
        }

        public Thing nearest_single(Vec2 point, IEnumerable<Thing> things)
        {
            Thing thing1 = null;
            float num = float.MaxValue;
            foreach (Thing thing2 in things)
            {
                if (!thing2.removeFromLevel)
                {
                    float lengthSq = (point - thing2.position).lengthSq;
                    if (lengthSq < num)
                    {
                        num = lengthSq;
                        thing1 = thing2;
                    }
                }
            }
            return thing1;
        }

        public List<KeyValuePair<float, Thing>> nearest(
          Vec2 point,
          IEnumerable<Thing> things,
          Thing ignore,
          Layer layer,
          bool placementLayer = false)
        {
            List<KeyValuePair<float, Thing>> keyValuePairList = new List<KeyValuePair<float, Thing>>();
            foreach (Thing thing in things)
            {
                if (!thing.removeFromLevel && thing != ignore && (layer == null || (placementLayer || thing.layer == layer) && thing.placementLayer == layer))
                    keyValuePairList.Add(new KeyValuePair<float, Thing>((point - thing.position).lengthSq, thing));
            }
            keyValuePairList.Sort((x, y) => x.Key >= y.Key ? 1 : -1);
            return keyValuePairList;
        }

        public List<KeyValuePair<float, Thing>> nearest(
          Vec2 point,
          IEnumerable<Thing> things,
          Thing ignore)
        {
            List<KeyValuePair<float, Thing>> keyValuePairList = new List<KeyValuePair<float, Thing>>();
            foreach (Thing thing in things)
            {
                if (!thing.removeFromLevel && thing != ignore)
                    keyValuePairList.Add(new KeyValuePair<float, Thing>((point - thing.position).lengthSq, thing));
            }
            keyValuePairList.Sort((x, y) => x.Key >= y.Key ? 1 : -1);
            return keyValuePairList;
        }

        public List<KeyValuePair<float, Thing>> nearest(
          Vec2 point,
          IEnumerable<Thing> things)
        {
            List<KeyValuePair<float, Thing>> keyValuePairList = new List<KeyValuePair<float, Thing>>();
            foreach (Thing thing in things)
            {
                if (!thing.removeFromLevel)
                    keyValuePairList.Add(new KeyValuePair<float, Thing>((point - thing.position).lengthSq, thing));
            }
            keyValuePairList.Sort((x, y) => x.Key >= y.Key ? 1 : -1);
            return keyValuePairList;
        }

        public T NearestThing<T>(Vec2 point, Thing ignore, Layer layer)
        {
            Type key = typeof(T);
            Thing thing = !(key == typeof(Thing)) ? nearest_single(point, _things[key], ignore, layer) : nearest_single(point, _things[typeof(Thing)], ignore, layer);
            return thing == null ? default(T) : (T)(object)thing;
        }

        public T NearestThing<T>(Vec2 point, Thing ignore)
        {
            Type key = typeof(T);
            Thing thing = !(key == typeof(Thing)) ? nearest_single(point, _things[key], ignore) : nearest_single(point, _things[typeof(Thing)], ignore);
            return thing == null ? default(T) : (T)(object)thing;
        }

        public T NearestThing<T>(Vec2 point)
        {
            Type key = typeof(T);
            Thing thing = !(key == typeof(Thing)) ? nearest_single(point, _things[key]) : nearest_single(point, _things[typeof(Thing)]);
            return thing == null ? default(T) : (T)(object)thing;
        }

        public T NearestThing<T>(Vec2 point, Thing ignore, int nearIndex, Layer layer)
        {
            Type key = typeof(T);
            if (key == typeof(Thing))
            {
                List<KeyValuePair<float, Thing>> keyValuePairList = nearest(point, _things[typeof(Thing)], ignore, layer);
                if (keyValuePairList.Count > nearIndex)
                    return (T)(object)keyValuePairList[nearIndex].Value;
            }
            List<KeyValuePair<float, Thing>> keyValuePairList1 = nearest(point, _things[key], ignore, layer);
            return keyValuePairList1.Count > nearIndex ? (T)(object)keyValuePairList1[nearIndex].Value : default(T);
        }

        public T NearestThing<T>(Vec2 point, Thing ignore, int nearIndex)
        {
            Type key = typeof(T);
            if (key == typeof(Thing))
            {
                List<KeyValuePair<float, Thing>> keyValuePairList = nearest(point, _things[typeof(Thing)], ignore);
                if (keyValuePairList.Count > nearIndex)
                    return (T)(object)keyValuePairList[nearIndex].Value;
            }
            List<KeyValuePair<float, Thing>> keyValuePairList1 = nearest(point, _things[key], ignore);
            return keyValuePairList1.Count > nearIndex ? (T)(object)keyValuePairList1[nearIndex].Value : default(T);
        }

        public T NearestThingFilter<T>(Vec2 point, Predicate<Thing> filter)
        {
            Thing thing1 = null;
            float num = float.MaxValue;
            foreach (Thing thing2 in things[typeof(T)])
            {
                if (!thing2.removeFromLevel)
                {
                    float lengthSq = (point - thing2.position).lengthSq;
                    if (lengthSq < num && filter(thing2))
                    {
                        num = lengthSq;
                        thing1 = thing2;
                    }
                }
            }
            return thing1 == null ? default(T) : (T)(object)thing1;
        }

        public T NearestThingFilter<T>(Vec2 point, Predicate<Thing> filter, float maxDistance) // IMPROVEME Yea this just checks an ok sized area instead of being related to the maxdistance ¯\_(ツ)_/¯
        {
            maxDistance *= maxDistance;
            int hashcode = typeof(T).GetHashCode();

            Thing thing1 = null;
            float num = float.MaxValue;
            int positionx = (int)((point.x + QuadTreeObjectList.offset) / QuadTreeObjectList.cellsize);
            int positiony = (int)((point.y + QuadTreeObjectList.offset) / QuadTreeObjectList.cellsize);
            for (int x = -1; x < 2; x++)
            {
                for (int y = -1; y < 2; y++)
                {
                    if (_things.Buckets.TryGetValue(new Vec2(positionx + x, positiony + y), out Dictionary<int, List<Thing>> output))
                    {
                        if (output.TryGetValue(hashcode, out List<Thing> output2))
                        {
                            foreach (Thing thing2 in output2)
                            {
                                if (!thing2.removeFromLevel)
                                {
                                    float lengthSq = (point - thing2.position).lengthSq;
                                    if (lengthSq < num && lengthSq < maxDistance && filter(thing2))
                                    {
                                        num = lengthSq;
                                        thing1 = thing2;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return thing1 == null ? default(T) : (T)(object)thing1;
        }
        public T NearestThing<T>(Vec2 point, float maxDistance) // IMPROVEME Yea this just checks an ok sized area instead of being related to the maxdistance ¯\_(ツ)_/¯
        {
            maxDistance *= maxDistance;
            int hashcode = typeof(T).GetHashCode();
            Thing thing1 = null;
            float num = float.MaxValue;
            int positionx = (int)((point.x + QuadTreeObjectList.offset) / QuadTreeObjectList.cellsize);
            int positiony = (int)((point.y + QuadTreeObjectList.offset) / QuadTreeObjectList.cellsize);
            for (int x = -1; x < 2; x++)
            {
                for (int y = -1; y < 2; y++)
                {
                    if (_things.Buckets.TryGetValue(new Vec2(positionx + x, positiony + y), out Dictionary<int, List<Thing>> output))
                    {
                        if (output.TryGetValue(hashcode, out List<Thing> output2))
                        {
                            foreach (Thing thing2 in output2)
                            {
                                if (!thing2.removeFromLevel)
                                {
                                    float lengthSq = (point - thing2.position).lengthSq;
                                    if (lengthSq < num && lengthSq < maxDistance)
                                    {
                                        num = lengthSq;
                                        thing1 = thing2;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return thing1 == null ? default(T) : (T)(object)thing1;
        }
        public T NearestThing<T>(Vec2 point, float maxDistance, Thing ignore) // IMPROVEME Yea this just checks an ok sized area instead of being related to the maxdistance ¯\_(ツ)_/¯
        {
            maxDistance *= maxDistance;
            int hashcode = typeof(T).GetHashCode();
            Thing thing1 = null;
            float num = float.MaxValue;
            int positionx = (int)((point.x + QuadTreeObjectList.offset) / QuadTreeObjectList.cellsize);
            int positiony = (int)((point.y + QuadTreeObjectList.offset) / QuadTreeObjectList.cellsize);
            for (int x = -1; x < 2; x++)
            {
                for (int y = -1; y < 2; y++)
                {
                    if (_things.Buckets.TryGetValue(new Vec2(positionx + x, positiony + y), out Dictionary<int, List<Thing>> output))
                    {
                        if (output.TryGetValue(hashcode, out List<Thing> output2))
                        {
                            foreach (Thing thing2 in output2)
                            {
                                if (!thing2.removeFromLevel)
                                {
                                    float lengthSq = (point - thing2.position).lengthSq;
                                    if (thing2 != ignore && lengthSq < num && lengthSq < maxDistance)
                                    {
                                        num = lengthSq;
                                        thing1 = thing2;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return thing1 == null ? default(T) : (T)(object)thing1;
        }

        public T CollisionCircle<T>(Vec2 p1, float radius, Thing ignore)
        {
            foreach (Thing thing in things.CollisionCircleAll(p1, radius, typeof(T)))
            {
                if (!thing.removeFromLevel && thing != ignore && Collision.Circle(p1, radius, thing))
                {
                    return (T)(object)thing;
                }
            }
            return default(T);
        }

        //public T CollisionCircle<T>(Vec2 p1, float radius)
        //{
        //    System.Type key = typeof(T);
        //    foreach (Thing dynamicObject in _things.GetDynamicObjects(key))
        //    {
        //        if (!dynamicObject.removeFromLevel && Collision.Circle(p1, radius, dynamicObject))
        //            return (T)(object)dynamicObject;
        //    }
        //    return _things.HasStaticObjects(key) ? _things.quadTree.CheckCircle<T>(p1, radius) : default(T);
        //}
        public T CollisionCircle<T>(Vec2 p1, float radius)
        {
            foreach (Thing thing in things.CollisionCircleAll(p1, radius, typeof(T)))
            {
                if (!thing.removeFromLevel && Collision.Circle(p1, radius, thing))
                {
                    return (T)(object)thing;
                }
            }
            return default(T);
        }
        public IEnumerable<T> CollisionCircleAll<T>(Vec2 p1, float radius) //ban
        {
            List<T> outList1 = new List<T>();
            foreach (Thing thing in things.CollisionCircleAll(p1, radius, typeof(T)))
            {
                if (!thing.removeFromLevel && Collision.Circle(p1, radius, thing))
                {
                    outList1.Add((T)(object)thing);
                }
            }
            return outList1;//nextCollisionList.AsEnumerable<object>().Cast<T>();
        }
        public IEnumerable<T> CollisionCircleAllOld<T>(Vec2 p1, float radius) //old //brought it back to fix collision issue with the postitron shooter -NiK0
        {

            List<object> nextCollisionList = GetNextCollisionList();
            Type key = typeof(T);
            foreach (Thing dynamicObject in _things.GetDynamicObjects(key))
            {
                if (!dynamicObject.removeFromLevel && Collision.Circle(p1, radius, dynamicObject))
                    nextCollisionList.Add(dynamicObject);
            }
            if (_things.HasStaticObjects(key))
                _things.quadTree.CheckCircleAll<T>(p1, radius, nextCollisionList);
            return nextCollisionList.AsEnumerable<object>().Cast<T>();
        }

        public T CollisionRectFilter<T>(Vec2 p1, Vec2 p2, Predicate<T> filter)
        {
            Type key = typeof(T);
            //System.Type key = typeof(T);
            //foreach (Thing dynamicObject in _things.GetDynamicObjects(key))
            //{
            //    if (!dynamicObject.removeFromLevel && Collision.Rect(p1, p2, dynamicObject) && filter((T)(object)dynamicObject))
            //        return (T)(object)dynamicObject;
            //}
            //return _things.HasStaticObjects(key) ? _things.quadTree.CheckRectangleFilter<T>(p1, p2, filter) : default(T);
            foreach (Thing thing in things.CollisionRectAll(p1, p2, key))
            {
                if (!thing.removeFromLevel && Collision.Rect(p1, p2, thing) && filter((T)(object)thing))
                {
                    return (T)(object)thing;
                }
            }
            return default(T);
        }

        public T CollisionRect<T>(Vec2 p1, Vec2 p2, Thing ignore)
        {
            //System.Type key = typeof(T);
            //foreach (Thing dynamicObject in _things.GetDynamicObjects(key))
            //{
            //    if (!dynamicObject.removeFromLevel && dynamicObject != ignore && Collision.Rect(p1, p2, dynamicObject))
            //        return (T)(object)dynamicObject;
            //}
            //return _things.HasStaticObjects(key) ? _things.quadTree.CheckRectangle<T>(p1, p2, ignore) : default(T);
            foreach (Thing thing in things.CollisionRectAll(p1, p2, typeof(T)))
            {
                if (!thing.removeFromLevel && thing != ignore && Collision.Rect(p1, p2, thing))
                {
                    return (T)(object)thing;
                }
            }
            return default(T);
        }

        public T CollisionRect<T>(Vec2 p1, Vec2 p2)
        {
            foreach (Thing thing in things.CollisionRectAll(p1, p2, typeof(T)))
            {
                if (!thing.removeFromLevel && Collision.Rect(p1, p2, thing))
                {
                    return (T)(object)thing;
                }
            }
            return default(T);
            //System.Type key = typeof(T);
            //foreach (Thing dynamicObject in _things.GetDynamicObjects(key))
            //{
            //    if (!dynamicObject.removeFromLevel && Collision.Rect(p1, p2, dynamicObject))
            //        return (T)(object)dynamicObject;
            //}
            //return _things.HasStaticObjects(key) ? _things.quadTree.CheckRectangle<T>(p1, p2) : default(T);
        }
        //GetThings
        public List<T> CollisionRectAll<T>(Vec2 p1, Vec2 p2, List<T> outList) // old DG
        {
            List<T> outList1 = outList == null ? new List<T>() : outList;
            Type key = typeof(T);
            foreach (Thing dynamicObject in _things.GetDynamicObjects(key))
            {
                if (!dynamicObject.removeFromLevel && Collision.Rect(p1, p2, dynamicObject))
                    outList1.Add((T)(object)dynamicObject);
            }
            if (_things.HasStaticObjects(key))
                _things.quadTree.CheckRectangleAll(p1, p2, outList1);
            return outList1;
        }
        public List<T> CollisionRectAllNiK0<T>(Vec2 p1, Vec2 p2, List<T> outList)
        {
            List<T> list = (outList == null) ? new List<T>() : outList;
            Type t = typeof(T);
            foreach (Thing thing in _things.GetDynamicObjects(t))
            {
                if (!thing.removeFromLevel && Collision.Rect(p1, p2, thing))
                {
                    list.Add((T)((object)thing));
                }
            }
            if (_things.HasStaticObjects(t))
            {
                _things.quadTree.CheckRectangleAll<T>(p1, p2, list);
            }
            return list;
        }
        public List<T> CollisionRectAllDan<T>(Vec2 p1, Vec2 p2, List<T> outList)
        {
            List<T> outList1 = outList == null ? new List<T>() : outList;
            foreach (Thing thing in things.CollisionRectAll(p1, p2, typeof(T)))
            {
                if (!thing.removeFromLevel && Collision.Rect(p1, p2, thing))
                {
                    outList1.Add((T)(object)thing);
                }
            }
            //bool flag2 = !t.removeFromLevel && Collision.Rect(p1, p2, t);
            //System.Type key = typeof(T);
            //foreach (Thing dynamicObject in _things.GetDynamicObjects(key))
            //{
            //    if (!dynamicObject.removeFromLevel && Collision.Rect(p1, p2, dynamicObject))
            //        outList1.Add((T)(object)dynamicObject);
            //}
            //if (_things.HasStaticObjects(key))
            //    _things.quadTree.CheckRectangleAll<T>(p1, p2, outList1);
            return outList1;
        }

        public T CollisionLine<T>(Vec2 p1, Vec2 p2, Thing ignore)
        {
            Type key = typeof(T);
            foreach (Thing dynamicObject in _things.GetDynamicObjects(key))
            {
                if (!dynamicObject.removeFromLevel && dynamicObject != ignore && Collision.Line(p1, p2, dynamicObject))
                    return (T)(object)dynamicObject;
            }
            return _things.HasStaticObjects(key) ? _things.quadTree.CheckLine<T>(p1, p2, ignore) : default(T);
        }

        public T CollisionLine<T>(Vec2 p1, Vec2 p2)
        {
            Type key = typeof(T);
            foreach (Thing thing in things.CollisionLineAll(p1, p2, key))
            {
                if (!thing.removeFromLevel && Collision.Line(p1, p2, thing))
                    return (T)(object)thing;
            }
            return default(T);
        }
        public T OldCollisionLine<T>(Vec2 p1, Vec2 p2)
        {
            Type t = typeof(T);
            foreach (Thing thing in _things.GetDynamicObjects(t))
            {
                if (!thing.removeFromLevel && Collision.Line(p1, p2, thing))
                {
                    return (T)((object)thing);
                }
            }
            if (_things.HasStaticObjects(t))
            {
                return _things.quadTree.CheckLine<T>(p1, p2);
            }
            return default(T);
        }


        public T CollisionLine<T>(Vec2 p1, Vec2 p2, out Vec2 position, Thing ignore)
        {
            position = new Vec2(0f, 0f);
            Type key = typeof(T);
            foreach (Thing dynamicObject in _things.GetDynamicObjects(key))
            {
                if (!dynamicObject.removeFromLevel && dynamicObject != ignore)
                {
                    Vec2 vec2 = Collision.LinePoint(p1, p2, dynamicObject);
                    if (vec2 != Vec2.Zero)
                    {
                        position = vec2;
                        return (T)(object)dynamicObject;
                    }
                }
            }
            return _things.HasStaticObjects(key) ? _things.quadTree.CheckLinePoint<T>(p1, p2, out position, ignore) : default(T);
        }

        public T CollisionLine<T>(Vec2 p1, Vec2 p2, out Vec2 position)
        {
            position = new Vec2(0f, 0f);
            Type key = typeof(T);

            foreach (Thing dynamicObject in _things.GetDynamicObjects(key))
            {
                if (!dynamicObject.removeFromLevel)
                {
                    Vec2 vec2 = Collision.LinePoint(p1, p2, dynamicObject);
                    if (vec2 != Vec2.Zero)
                    {
                        position = vec2;
                        return (T)(object)dynamicObject;
                    }
                }
            }
            return _things.HasStaticObjects(key) ? _things.quadTree.CheckLinePoint<T>(p1, p2, out position) : default(T);
        }

        public IEnumerable<T> CollisionLineAll<T>(Vec2 p1, Vec2 p2)
        {
            List<object> nextCollisionList = GetNextCollisionList();
            Type key = typeof(T);
            foreach (Thing thing in things.CollisionLineAll(p1, p2, key))
            {
                if (!thing.removeFromLevel && Collision.Line(p1, p2, thing))
                    nextCollisionList.Add(thing);
            }
            return nextCollisionList.Cast<T>();
            //foreach (Thing dynamicObject in _things.GetDynamicObjects(key))
            //{
            //    if (!dynamicObject.removeFromLevel && Collision.Line(p1, p2, dynamicObject))
            //        nextCollisionList.Add(dynamicObject);
            //}
            //if (_things.HasStaticObjects(key))
            //{
            //    List<T> source = _things.quadTree.CheckLineAll<T>(p1, p2);
            //    nextCollisionList.AddRange(source.Cast<object>());
            //}
            //return nextCollisionList.AsEnumerable().Cast<T>();
        }

        public T CollisionPoint<T>(Vec2 point, Thing ignore, Layer layer)
        {
            Type key = typeof(T);
            if (key == typeof(Thing))
            {
                foreach (Thing thing in _things)
                {
                    if (!thing.removeFromLevel && thing != ignore && Collision.Point(point, thing) && (layer == null || layer == thing.layer))
                        return (T)(object)thing;
                }
            }
            foreach (Thing dynamicObject in _things.GetDynamicObjects(key))
            {
                if (!dynamicObject.removeFromLevel && dynamicObject != ignore && Collision.Point(point, dynamicObject) && (layer == null || layer == dynamicObject.layer))
                    return (T)(object)dynamicObject;
            }
            return _things.HasStaticObjects(key) ? _things.quadTree.CheckPoint<T>(point, ignore, layer) : default(T);
        }

        public T CollisionPoint<T>(Vec2 point, Thing ignore)
        {
            Type key = typeof(T);
            if (key == typeof(Thing))
            {
                foreach (Thing thing in _things)
                {
                    if (!thing.removeFromLevel && thing != ignore && Collision.Point(point, thing))
                        return (T)(object)thing;
                }
            }
            foreach (Thing dynamicObject in _things.GetDynamicObjects(key))
            {
                if (!dynamicObject.removeFromLevel && dynamicObject != ignore && Collision.Point(point, dynamicObject))
                    return (T)(object)dynamicObject;
            }
            return _things.HasStaticObjects(key) ? _things.quadTree.CheckPoint<T>(point, ignore) : default(T);
        }

        public Thing CollisionPoint(Type pType, Vec2 point, Thing ignore)
        {
            if (pType == typeof(Thing))
            {
                foreach (Thing thing in _things)
                {
                    if (!thing.removeFromLevel && thing != ignore && Collision.Point(point, thing))
                        return thing;
                }
            }
            foreach (Thing dynamicObject in _things.GetDynamicObjects(pType))
            {
                if (!dynamicObject.removeFromLevel && dynamicObject != ignore && Collision.Point(point, dynamicObject))
                    return dynamicObject;
            }
            return _things.HasStaticObjects(pType) ? _things.quadTree.CheckPoint(pType, point, ignore) : null;
        }

        //public T CollisionPoint<T>(Vec2 point)
        //{
        //    System.Type key = typeof(T);
        //    if (key == typeof(Thing))
        //    {
        //        foreach (Thing thing in _things)
        //        {
        //            if (!thing.removeFromLevel && Collision.Point(point, thing))
        //                return (T)(object)thing;
        //        }
        //    }
        //    foreach (Thing dynamicObject in _things.GetDynamicObjects(key))
        //    {
        //        if (!dynamicObject.removeFromLevel && Collision.Point(point, dynamicObject))
        //            return (T)(object)dynamicObject;
        //    }
        //    return _things.HasStaticObjects(key) ? _things.quadTree.CheckPoint<T>(point) : default(T);
        //}
        public T CollisionPoint<T>(Vec2 point)
        {
            foreach (Thing thing in things.CollisionPointAll(point, typeof(T)))
            {
                if (!thing.removeFromLevel && Collision.Point(point, thing))
                    return (T)(object)thing;
            }
            return default(T);
        }
        public T CollisionPointFast<T>(Vec2 point)
        {
            List<Thing> t = things.CollisionPointAllFast(point, typeof(T));
            for (int i = 0; i < t.Count; i++)
            {
                Thing thing = t[i];
                if (!thing.removeFromLevel && Collision.Point(point, thing))
                    return (T)(object)thing;
            }
            return default(T);
        }
        public T QuadTreePointFilter<T>(Vec2 point, Func<Thing, bool> pFilter) => _things.HasStaticObjects(typeof(T)) ? _things.quadTree.CheckPointFilter<T>(point, pFilter) : default(T);

        public Thing CollisionPoint(Vec2 point, Type t, Thing ignore, Layer layer)
        {
            if (t == typeof(Thing))
            {
                foreach (Thing thing in _things)
                {
                    if (!thing.removeFromLevel && thing != ignore && Collision.Point(point, thing) && (layer == null || layer == thing.layer))
                        return thing;
                }
            }
            foreach (Thing dynamicObject in _things.GetDynamicObjects(t))
            {
                if (!dynamicObject.removeFromLevel && dynamicObject != ignore && Collision.Point(point, dynamicObject) && (layer == null || layer == dynamicObject.layer))
                    return dynamicObject;
            }
            return _things.HasStaticObjects(t) ? _things.quadTree.CheckPoint(point, t, ignore, layer) : null;
        }

        public Thing CollisionPoint(Vec2 point, Type t, Thing ignore)
        {
            if (t == typeof(Thing))
            {
                foreach (Thing thing in _things)
                {
                    if (!thing.removeFromLevel && thing != ignore && Collision.Point(point, thing))
                        return thing;
                }
            }
            foreach (Thing dynamicObject in _things.GetDynamicObjects(t))
            {
                if (!dynamicObject.removeFromLevel && dynamicObject != ignore && Collision.Point(point, dynamicObject))
                    return dynamicObject;
            }
            return _things.HasStaticObjects(t) ? _things.quadTree.CheckPoint(point, t, ignore) : null;
        }

        public Thing CollisionPoint(Vec2 point, Type t)
        {
            if (t == typeof(Thing))
            {
                foreach (Thing thing in _things)
                {
                    if (!thing.removeFromLevel && Collision.Point(point, thing))
                        return thing;
                }
            }
            foreach (Thing dynamicObject in _things.GetDynamicObjects(t))
            {
                if (!dynamicObject.removeFromLevel && Collision.Point(point, dynamicObject))
                    return dynamicObject;
            }
            return _things.HasStaticObjects(t) ? _things.quadTree.CheckPoint(point, t) : null;
        }

        public T CollisionPointPlacementLayer<T>(Vec2 point, Thing ignore = null, Layer layer = null)
        {
            Type key = typeof(T);
            if (key == typeof(Thing))
            {
                foreach (Thing thing in _things)
                {
                    if (!thing.removeFromLevel && thing != ignore && Collision.Point(point, thing) && (layer == null || layer == thing.placementLayer))
                        return (T)(object)thing;
                }
            }
            foreach (Thing dynamicObject in _things.GetDynamicObjects(key))
            {
                if (!dynamicObject.removeFromLevel && dynamicObject != ignore && Collision.Point(point, dynamicObject) && (layer == null || layer == dynamicObject.placementLayer))
                    return (T)(object)dynamicObject;
            }
            return _things.HasStaticObjects(key) ? _things.quadTree.CheckPointPlacementLayer<T>(point, ignore, layer) : default(T);
        }

        public T CollisionPointFilter<T>(Vec2 point, Predicate<Thing> filter)
        {
            Type key = typeof(T);
            if (key == typeof(Thing))
            {
                foreach (Thing thing in _things)
                {
                    if (!thing.removeFromLevel && filter(thing) && Collision.Point(point, thing))
                        return (T)(object)thing;
                }
            }
            foreach (Thing dynamicObject in _things.GetDynamicObjects(key))
            {
                if (!dynamicObject.removeFromLevel && filter(dynamicObject) && Collision.Point(point, dynamicObject))
                    return (T)(object)dynamicObject;
            }
            return _things.HasStaticObjects(key) ? _things.quadTree.CheckPointFilter<T>(point, filter) : default(T);
        }

        public IEnumerable<T> CollisionPointAll<T>(Vec2 point, Layer layer)
        {
            List<object> nextCollisionList = GetNextCollisionList();
            Type key = typeof(T);
            foreach (Thing dynamicObject in _things.GetDynamicObjects(key))
            {
                if (!dynamicObject.removeFromLevel && Collision.Point(point, dynamicObject) && (layer == null || layer == dynamicObject.layer))
                    nextCollisionList.Add(dynamicObject);
            }
            if (_things.HasStaticObjects(key))
            {
                T obj = _things.quadTree.CheckPoint<T>(point, null, layer);
                if (obj != null)
                    nextCollisionList.Add(obj);
            }
            return nextCollisionList.AsEnumerable().Cast<T>();
        }

        //public IEnumerable<T> CollisionPointAll<T>(Vec2 point)
        //{
        //    List<object> nextCollisionList = Level.GetNextCollisionList();
        //    System.Type key = typeof(T);
        //    foreach (Thing dynamicObject in _things.GetDynamicObjects(key))
        //    {
        //        if (!dynamicObject.removeFromLevel && Collision.Point(point, dynamicObject))
        //            nextCollisionList.Add(dynamicObject);
        //    }
        //    if (_things.HasStaticObjects(key))
        //    {
        //        T obj = _things.quadTree.CheckPoint<T>(point);
        //        if (obj != null)
        //            nextCollisionList.Add(obj);
        //    }
        //    return nextCollisionList.AsEnumerable<object>().Cast<T>();
        //}
        public IEnumerable<T> CollisionPointAll<T>(Vec2 point)
        {
            List<object> nextCollisionList = GetNextCollisionList();
            foreach (Thing thing in things.CollisionPointAll(point, typeof(T)))
            {
                if (!thing.removeFromLevel && Collision.Point(point, thing))
                    nextCollisionList.Add(thing);

            }
            return nextCollisionList.AsEnumerable().Cast<T>();
        }
        public IEnumerable<T> OldCollisionPointAll<T>(Vec2 point)
        {
            List<object> list = Level.GetNextCollisionList();
            Type t = typeof(T);
            foreach (Thing thing in this._things.GetDynamicObjects(t))
            {
                if (!thing.removeFromLevel && Collision.Point(point, thing))
                {
                    list.Add(thing);
                }
            }
            if (this._things.HasStaticObjects(t))
            {
                T thing2 = (T)((object)this._things.quadTree.CheckPoint<T>(point));
                if (thing2 != null)
                {
                    list.Add(thing2);
                }
            }
            return list.AsEnumerable<object>().Cast<T>();
        }
        //public ICollection<Thing> CollisionPointAll(Vec2 point)
        //{
        //    if (Buckets.TryGetValue(new Vec2((int)((point.x + offset) / cellsize), (int)((point.y + offset) / cellsize)), out List<Thing> output))
        //    {
        //        return output;
        //    }
        //    return new List<Thing>();
        //}
        public void CollisionBullet(Vec2 point, List<MaterialThing> output)
        {
            //TODO: Re-optimize this code, this is currently DG's collision code because the code that uses the Bucket system makes certain bullet collisions
            //work differently from vanilla, this is unoptimal because as with normal DG it checks every thing in the entire map and not just a sector like
            //what we do with buckets so if someone can figure out a way to make it use buckets and work in vanilla go for it -NiK0
            /*foreach (Thing thing in things.CollisionPointAll(point, typeof(MaterialThing)))
            {
                if (!thing.removeFromLevel && Collision.Point(point, thing))
                    output.Add(thing as MaterialThing);
            }*/
            Type t = typeof(MaterialThing);
            foreach (Thing thing in _things.GetDynamicObjects(t))
            {
                if (!thing.removeFromLevel && Collision.Point(point, thing))
                {
                    output.Add(thing as MaterialThing);
                }
            }
            if (_things.HasStaticObjects(t))
            {
                MaterialThing thing2 = _things.quadTree.CheckPoint<MaterialThing>(point);
                if (thing2 != null)
                {
                    output.Add(thing2);
                }
            }
        }


        public static T CheckRay<T>(Vec2 start, Vec2 end) => current.CollisionRay<T>(start, end);

        public T CollisionRay<T>(Vec2 start, Vec2 end) => CheckRay<T>(start, end, out Vec2 _);

        public static T CheckRay<T>(Vec2 start, Vec2 end, out Vec2 hitPos) => current.CollisionRay<T>(start, end, out hitPos);

        public static T CheckRay<T>(Vec2 start, Vec2 end, Thing ignore, out Vec2 hitPos) => current.CollisionRay<T>(start, end, ignore, out hitPos);

        public T CollisionRay<T>(Vec2 start, Vec2 end, out Vec2 hitPos)
        {
            Vec2 dir = end - start;
            float length = dir.length;
            dir.Normalize();
            Math.Ceiling(length);
            Stack<TravelInfo> travelInfoStack = new Stack<TravelInfo>();
            travelInfoStack.Push(new TravelInfo(start, end, length));
            while (travelInfoStack.Count > 0)
            {
                TravelInfo travelInfo = travelInfoStack.Pop();
                if (current.CollisionLine<T>(travelInfo.p1, travelInfo.p2) != null)
                {
                    if (travelInfo.length < 8)
                    {
                        T obj = Raycast<T>(travelInfo.p1, dir, travelInfo.length, out hitPos);
                        if (obj != null)
                            return obj;
                    }
                    else
                    {
                        float len = travelInfo.length * 0.5f;
                        Vec2 vec2 = travelInfo.p1 + dir * len;
                        travelInfoStack.Push(new TravelInfo(vec2, travelInfo.p2, len));
                        travelInfoStack.Push(new TravelInfo(travelInfo.p1, vec2, len));
                    }
                }
            }
            hitPos = end;
            return default(T);
        }

        public T CollisionRay<T>(Vec2 start, Vec2 end, Thing ignore, out Vec2 hitPos)
        {
            Vec2 dir = end - start;
            float length = dir.length;
            dir.Normalize();
            Math.Ceiling(length);
            Stack<TravelInfo> travelInfoStack = new Stack<TravelInfo>();
            travelInfoStack.Push(new TravelInfo(start, end, length));
            while (travelInfoStack.Count > 0)
            {
                TravelInfo travelInfo = travelInfoStack.Pop();
                if (current.CollisionLine<T>(travelInfo.p1, travelInfo.p2, ignore) != null)
                {
                    if (travelInfo.length < 8)
                    {
                        T obj = Raycast<T>(travelInfo.p1, dir, ignore, travelInfo.length, out hitPos);
                        if (obj != null)
                            return obj;
                    }
                    else
                    {
                        float len = travelInfo.length * 0.5f;
                        Vec2 vec2 = travelInfo.p1 + dir * len;
                        travelInfoStack.Push(new TravelInfo(vec2, travelInfo.p2, len));
                        travelInfoStack.Push(new TravelInfo(travelInfo.p1, vec2, len));
                    }
                }
            }
            hitPos = end;
            return default(T);
        }

        private T Raycast<T>(Vec2 p1, Vec2 dir, float length, out Vec2 hit)
        {
            int num = (int)Math.Ceiling(length);
            Vec2 point = p1;
            do
            {
                --num;
                T obj = current.CollisionPoint<T>(point);
                if (obj != null)
                {
                    hit = point;
                    return obj;
                }
                point += dir;
            }
            while (num > 0);
            hit = point;
            return default(T);
        }

        private T Raycast<T>(Vec2 p1, Vec2 dir, Thing ignore, float length, out Vec2 hit)
        {
            int num = (int)Math.Ceiling(length);
            Vec2 point = p1;
            do
            {
                --num;
                T obj = current.CollisionPoint<T>(point, ignore);
                if (obj != null)
                {
                    hit = point;
                    return obj;
                }
                point += dir;
            }
            while (num > 0);
            hit = point;
            return default(T);
        }

        //private T Rectcast<T>(Vec2 p1, Vec2 p2, Rectangle rect, out Vec2 hit)
        //{
        //    Vec2 vec2_1 = p2 - p1;
        //    int num = (int)Math.Ceiling(vec2_1.length);
        //    vec2_1.Normalize();
        //    Vec2 vec2_2 = p1;
        //    do
        //    {
        //        --num;
        //        T obj = Level.current.CollisionRect<T>(vec2_2 + new Vec2(rect.Top, rect.Left), vec2_2 + new Vec2(rect.Bottom, rect.Right));
        //        if ((object)obj != null)
        //        {
        //            hit = vec2_2;
        //            return obj;
        //        }
        //        vec2_2 += vec2_1;
        //    }
        //    while (num > 0);
        //    hit = vec2_2;
        //    return default(T);
        //}
    }
}
