// Decompiled with JetBrains decompiler
// Type: DuckGame.GameLevel
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DuckGame
{
    public class GameLevel : XMLLevel, IHaveAVirtualTransition
    {
        protected FollowCam _followCam;
        protected GameMode _mode;
        private RandomLevelNode _randomLevel;
        private bool _validityTest;
        private float _infoSlide;
        private float _infoWait;
        private bool _showInfo = true;
        public bool _editorTestMode;
        public string levelInputString;
        private static bool first;
        //private bool _startedMatch;
        private static int _numberOfDucksSpawned;
        private int wait;
        public override string networkIdentifier => level;

        public FollowCam followCam => _followCam;

        public bool isRandom => _randomLevel != null;

        public bool Raining;
        public void SkipMatch()
        {
            if (Network.isActive && Network.isServer)
                Send.Message(new NMSkipLevel());
            if (_mode == null)
                _mode = new DM();
            _mode.SkipMatch();
        }

        public GameLevel(string lev, int seedVal = 0, bool validityTest = false, bool editorTestMode = false)
          : base(lev)
        {
            levelInputString = lev;
            _followCam = new FollowCam
            {
                lerpMult = 1.2f
            };
            camera = _followCam;
            _validityTest = validityTest;
            if (Network.isActive)
                _readyForTransition = false;
            GameLevel.first = !GameLevel.first;
            if (seedVal != 0)
                seed = seedVal;
            _editorTestMode = editorTestMode;
        }

        public override string LevelNameData()
        {
            string str = base.LevelNameData();
            if (this != null)
                str = str + "," + (isCustomLevel ? "1" : "0");
            return str;
        }

        public bool matchOver => _mode == null || _mode.matchOver;

        public override void Initialize()
        {
            TeamSelect2.QUACK3 = TeamSelect2.Enabled("QUACK3");
            Vote.ClearVotes();
            if (level == "RANDOM")
            {
                _randomLevel = LevelGenerator.MakeLevel(seed: seed);
                seed = _randomLevel.seed;
            }
            else if (Rando.Int(3) == 0) Raining = true;
            base.Initialize();
            if (Network.isActive)
                Level.core.gameInProgress = true;
            if (_randomLevel != null)
            {
                GhostManager.context.ResetGhostIndex(networkIndex);
                _randomLevel.LoadParts(0f, 0f, this, seed);
                List<SpawnPoint> source1 = new List<SpawnPoint>();
                foreach (SpawnPoint spawnPoint in things[typeof(SpawnPoint)])
                    source1.Add(spawnPoint);
                List<SpawnPoint> chosenSpawns = new List<SpawnPoint>();
                for (int index = 0; index < 4; ++index)
                {
                    if (chosenSpawns.Count == 0)
                    {
                        chosenSpawns.Add(source1.ElementAt<SpawnPoint>(Rando.Int(source1.Count - 1)));
                    }
                    else
                    {
                        IOrderedEnumerable<SpawnPoint> source2 = source1.OrderByDescending<SpawnPoint, int>(x =>
                       {
                           int val2 = 9999999;
                           foreach (Transform transform in chosenSpawns)
                               val2 = (int)Math.Min((transform.position - x.position).length, val2);
                           return val2;
                       });
                        chosenSpawns.Add(source2.First<SpawnPoint>());
                    }
                }
                foreach (SpawnPoint spawnPoint in source1)
                {
                    if (!chosenSpawns.Contains(spawnPoint))
                        Level.Remove(spawnPoint);
                }
                foreach (Thing thing in things)
                {
                    if (Network.isActive && thing.isStateObject)
                    {
                        GhostManager.context.MakeGhost(thing, initLevel: true);
                        thing.ghostType = Editor.IDToType[thing.GetType()];
                    }
                }
                PyramidBackground pyramidBackground = new PyramidBackground(0f, 0f)
                {
                    visible = false
                };
                Level.Add(pyramidBackground);
                base.Initialize();
            }
            things.RefreshState();
            if (_mode == null)
                _mode = new DM(_validityTest, _editorTestMode);
            _mode.DoInitialize();
            if (!Network.isServer)
                return;
            foreach (Duck prepareSpawn in _mode.PrepareSpawns())
            {
                prepareSpawn.localSpawnVisible = false;
                prepareSpawn.immobilized = true;
                Level.Add(prepareSpawn);
            }
        }

        public virtual void MatchStart() { } //=> this._startedMatch = true;

        public static int NumberOfDucks
        {
            get => GameLevel._numberOfDucksSpawned < 2 ? 2 : GameLevel._numberOfDucksSpawned;
            set => GameLevel._numberOfDucksSpawned = value;
        }

        public override void Start()
        {
            _things.RefreshState();
            Vec2 vec2_1 = new Vec2(9999f, -9999f);
            Vec2 zero = Vec2.Zero;
            int num = 0;
            foreach (Duck t in things[typeof(Duck)])
            {
                followCam.Add(t);
                if (t.x < vec2_1.x)
                    vec2_1 = t.position;
                zero += t.position;
                ++num;
            }
            Vec2 vec2_2 = zero / num;
            followCam.Adjust();
        }

        protected override void OnTransferComplete(NetworkConnection c)
        {
            Level.current.things.RefreshState();
            Vec2 vec2_1 = new Vec2(9999f, -9999f);
            Vec2 zero = Vec2.Zero;
            int num = 0;
            List<Duck> duckList = new List<Duck>();
            foreach (Duck t in things[typeof(Duck)])
            {
                t.localSpawnVisible = false;
                followCam.Add(t);
                if (t.x < vec2_1.x)
                    vec2_1 = t.position;
                zero += t.position;
                ++num;
                duckList.Add(t);
            }
            Vec2 vec2_2 = zero / num;
            GameLevel._numberOfDucksSpawned = num;
            if (GameLevel._numberOfDucksSpawned > 4)
                TeamSelect2.eightPlayersActive = true;
            followCam.Adjust();
            _mode.pendingSpawns = duckList;
            base.OnTransferComplete(c);
        }

        protected override void OnAllClientsReady()
        {
            if (Network.isServer)
                Send.Message(new NMBeginLevel());
            base.OnAllClientsReady();
        }

        public override void Update()
        {
            if (Raining)
            {
                int f = DGRSettings.S_ParticleMultiplier;
                if (f > 2 || Rando.Int(f) > 0)
                {
                    f = Maths.Clamp(f - 2, 1, 100);
                    for (int i = 0; i < f; i++)
                    {
                        Level.Add(new RainParticel(new Vec2(Rando.Float(topLeft.x - 400, bottomRight.x + 300), topLeft.y - 200)));
                    }
                }
            }
            ++MonoMain.timeInMatches;
            if (_mode != null)
                _mode.DoUpdate();
            if (_level == "RANDOM")
            {
                if (wait < 4)
                    ++wait;
                if (wait == 4)
                {
                    ++wait;
                    foreach (AutoBlock autoBlock in things[typeof(AutoBlock)])
                        autoBlock.PlaceBlock();
                    foreach (AutoPlatform autoPlatform in things[typeof(AutoPlatform)])
                    {
                        autoPlatform.PlaceBlock();
                        autoPlatform.UpdateNubbers();
                    }
                    foreach (BlockGroup blockGroup in things[typeof(BlockGroup)])
                    {
                        foreach (Block block in blockGroup.blocks)
                        {
                            if (block is AutoBlock)
                                (block as AutoBlock).PlaceBlock();
                        }
                    }
                }
            }
            base.Update();
        }

        public string displayName
        {
            get
            {
                string displayName = null;
                if (data != null && data.workshopData != null && data.workshopData.name != null && data.workshopData.name != "")
                    displayName = data.workshopData.name;
                else if (data != null && data.GetPath() != "" && data.GetPath() != null)
                    displayName = Path.GetFileNameWithoutExtension(data.GetPath());
                return displayName;
            }
        }

        public override void PostDrawLayer(Layer layer)
        {
            if (_mode != null)
                _mode.PostDrawLayer(layer);
            if (layer == Layer.HUD && data != null && customLevel && !_waitingOnTransition)
            {
                drawsOverPauseMenu = true;
                if (_showInfo && !GameMode.started || MonoMain.pauseMenu != null)
                {
                    _infoSlide = Lerp.Float(_infoSlide, 1f, 0.06f);
                    if (_infoSlide > 0.95f)
                    {
                        _infoWait += Maths.IncFrameTimer();
                        if (_infoWait > 2.5)
                            _showInfo = false;
                    }
                }
                else
                    _infoSlide = Lerp.Float(_infoSlide, 0f, 0.1f);
                if (_infoSlide > 0.0f)
                {
                    float x = 10f;
                    string text1 = displayName;
                    if (synchronizedLevelName != null)
                        text1 = synchronizedLevelName;
                    else if (text1 == null)
                        text1 = "CUSTOM LEVEL";
                    float stringWidth1 = Graphics.GetStringWidth(text1);
                    float num1 = (float)((stringWidth1 + x + 12.0) * (1.0 - _infoSlide));
                    Vec2 p1 = new Vec2(-num1, x - 1f);
                    Vec2 p2 = new Vec2((float)(x + stringWidth1 + 4.0), x + 10f);
                    Graphics.DrawRect(p1, p2 + new Vec2(-num1, 0f), new Color(13, 130, 211), (Depth)0.95f);
                    Graphics.DrawRect(p1 + new Vec2(-2f, 2f), p2 + new Vec2((float)(-num1 + 2.0), 2f), Colors.BlueGray, (Depth)0.9f);
                    Graphics.DrawStringOutline(text1, p1 + new Vec2(x, 2f), Color.White, Color.Black, (Depth)1f);
                    if (data.workshopData != null && data.workshopData.author != null && data.workshopData.author != "")
                    {
                        string text2 = "BY " + data.workshopData.author;
                        float stringWidth2 = Graphics.GetStringWidth(text2);
                        float num2 = (float)((stringWidth2 + x + 12.0) * (1.0 - _infoSlide));
                        p1 = new Vec2((float)(Layer.HUD.width - stringWidth2 - x - 5.0) + num2, (float)(Layer.HUD.height - x - 10.0));
                        p2 = new Vec2(Layer.HUD.width + num2, (float)(Layer.HUD.height - x + 1.0));
                        Graphics.DrawRect(p1, p2, new Color(138, 38, 190), (Depth)0.95f);
                        Graphics.DrawRect(p1 + new Vec2(-2f, -2f), p2 + new Vec2(2f, -2f), Colors.BlueGray, (Depth)0.9f);
                        Graphics.DrawStringOutline(text2, new Vec2(Layer.HUD.width - stringWidth2 - x + num2, (float)(Layer.HUD.height - x - 8.0)), Color.White, Color.Black, (Depth)1f);
                    }
                }
            }
            base.PostDrawLayer(layer);
        }
    }
}
