// Decompiled with JetBrains decompiler
// Type: DuckGame.GameLevel
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
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

        public override string networkIdentifier => this.level;

        public FollowCam followCam => this._followCam;

        public bool isRandom => this._randomLevel != null;

        public void SkipMatch()
        {
            if (Network.isActive && Network.isServer)
                Send.Message(new NMSkipLevel());
            if (this._mode == null)
                this._mode = new DM();
            this._mode.SkipMatch();
        }

        public GameLevel(string lev, int seedVal = 0, bool validityTest = false, bool editorTestMode = false)
          : base(lev)
        {
            this.levelInputString = lev;
            this._followCam = new FollowCam
            {
                lerpMult = 1.2f
            };
            this.camera = _followCam;
            this._validityTest = validityTest;
            if (Network.isActive)
                this._readyForTransition = false;
            GameLevel.first = !GameLevel.first;
            if (seedVal != 0)
                this.seed = seedVal;
            this._editorTestMode = editorTestMode;
        }

        public override string LevelNameData()
        {
            string str = base.LevelNameData();
            if (this != null)
                str = str + "," + (this.isCustomLevel ? "1" : "0");
            return str;
        }

        public bool matchOver => this._mode == null || this._mode.matchOver;

        public override void Initialize()
        {
            TeamSelect2.QUACK3 = TeamSelect2.Enabled("QUACK3");
            Vote.ClearVotes();
            if (this.level == "RANDOM")
            {
                this._randomLevel = LevelGenerator.MakeLevel(seed: this.seed);
                this.seed = this._randomLevel.seed;
            }
            base.Initialize();
            if (Network.isActive)
                Level.core.gameInProgress = true;
            if (this._randomLevel != null)
            {
                GhostManager.context.ResetGhostIndex(this.networkIndex);
                this._randomLevel.LoadParts(0.0f, 0.0f, this, this.seed);
                List<SpawnPoint> source1 = new List<SpawnPoint>();
                foreach (SpawnPoint spawnPoint in this.things[typeof(SpawnPoint)])
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
                foreach (Thing thing in this.things)
                {
                    if (Network.isActive && thing.isStateObject)
                    {
                        GhostManager.context.MakeGhost(thing, initLevel: true);
                        thing.ghostType = Editor.IDToType[thing.GetType()];
                    }
                }
                PyramidBackground pyramidBackground = new PyramidBackground(0.0f, 0.0f)
                {
                    visible = false
                };
                Level.Add(pyramidBackground);
                base.Initialize();
            }
            this.things.RefreshState();
            if (this._mode == null)
                this._mode = new DM(this._validityTest, this._editorTestMode);
            this._mode.DoInitialize();
            if (!Network.isServer)
                return;
            foreach (Duck prepareSpawn in this._mode.PrepareSpawns())
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
            this._things.RefreshState();
            Vec2 vec2_1 = new Vec2(9999f, -9999f);
            Vec2 zero = Vec2.Zero;
            int num = 0;
            foreach (Duck t in this.things[typeof(Duck)])
            {
                this.followCam.Add(t);
                if ((double)t.x < vec2_1.x)
                    vec2_1 = t.position;
                zero += t.position;
                ++num;
            }
            Vec2 vec2_2 = zero / num;
            this.followCam.Adjust();
        }

        protected override void OnTransferComplete(NetworkConnection c)
        {
            Level.current.things.RefreshState();
            Vec2 vec2_1 = new Vec2(9999f, -9999f);
            Vec2 zero = Vec2.Zero;
            int num = 0;
            List<Duck> duckList = new List<Duck>();
            foreach (Duck t in this.things[typeof(Duck)])
            {
                t.localSpawnVisible = false;
                this.followCam.Add(t);
                if ((double)t.x < vec2_1.x)
                    vec2_1 = t.position;
                zero += t.position;
                ++num;
                duckList.Add(t);
            }
            Vec2 vec2_2 = zero / num;
            GameLevel._numberOfDucksSpawned = num;
            if (GameLevel._numberOfDucksSpawned > 4)
                TeamSelect2.eightPlayersActive = true;
            this.followCam.Adjust();
            this._mode.pendingSpawns = duckList;
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
            ++MonoMain.timeInMatches;
            if (this._mode != null)
                this._mode.DoUpdate();
            if (this._level == "RANDOM")
            {
                if (this.wait < 4)
                    ++this.wait;
                if (this.wait == 4)
                {
                    ++this.wait;
                    foreach (AutoBlock autoBlock in this.things[typeof(AutoBlock)])
                        autoBlock.PlaceBlock();
                    foreach (AutoPlatform autoPlatform in this.things[typeof(AutoPlatform)])
                    {
                        autoPlatform.PlaceBlock();
                        autoPlatform.UpdateNubbers();
                    }
                    foreach (BlockGroup blockGroup in this.things[typeof(BlockGroup)])
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
                if (this.data.workshopData != null && this.data.workshopData.name != null && this.data.workshopData.name != "")
                    displayName = this.data.workshopData.name;
                else if (this.data.GetPath() != "" && this.data.GetPath() != null)
                    displayName = Path.GetFileNameWithoutExtension(this.data.GetPath());
                return displayName;
            }
        }

        public override void PostDrawLayer(Layer layer)
        {
            if (this._mode != null)
                this._mode.PostDrawLayer(layer);
            if (layer == Layer.HUD && this.data != null && this.customLevel && !this._waitingOnTransition)
            {
                this.drawsOverPauseMenu = true;
                if (this._showInfo && !GameMode.started || MonoMain.pauseMenu != null)
                {
                    this._infoSlide = Lerp.Float(this._infoSlide, 1f, 0.06f);
                    if (_infoSlide > 0.949999988079071)
                    {
                        this._infoWait += Maths.IncFrameTimer();
                        if (_infoWait > 2.5)
                            this._showInfo = false;
                    }
                }
                else
                    this._infoSlide = Lerp.Float(this._infoSlide, 0.0f, 0.1f);
                if (_infoSlide > 0.0)
                {
                    float x = 10f;
                    string text1 = this.displayName;
                    if (this.synchronizedLevelName != null)
                        text1 = this.synchronizedLevelName;
                    else if (text1 == null)
                        text1 = "CUSTOM LEVEL";
                    float stringWidth1 = Graphics.GetStringWidth(text1);
                    float num1 = (float)(((double)stringWidth1 + (double)x + 12.0) * (1.0 - _infoSlide));
                    Vec2 p1 = new Vec2(-num1, x - 1f);
                    Vec2 p2 = new Vec2((float)((double)x + (double)stringWidth1 + 4.0), x + 10f);
                    Graphics.DrawRect(p1, p2 + new Vec2(-num1, 0.0f), new Color(13, 130, 211), (Depth)0.95f);
                    Graphics.DrawRect(p1 + new Vec2(-2f, 2f), p2 + new Vec2((float)(-(double)num1 + 2.0), 2f), Colors.BlueGray, (Depth)0.9f);
                    Graphics.DrawStringOutline(text1, p1 + new Vec2(x, 2f), Color.White, Color.Black, (Depth)1f);
                    if (this.data.workshopData != null && this.data.workshopData.author != null && this.data.workshopData.author != "")
                    {
                        string text2 = "BY " + this.data.workshopData.author;
                        float stringWidth2 = Graphics.GetStringWidth(text2);
                        float num2 = (float)(((double)stringWidth2 + (double)x + 12.0) * (1.0 - _infoSlide));
                        p1 = new Vec2((float)((double)Layer.HUD.width - (double)stringWidth2 - (double)x - 5.0) + num2, (float)((double)Layer.HUD.height - (double)x - 10.0));
                        p2 = new Vec2(Layer.HUD.width + num2, (float)((double)Layer.HUD.height - (double)x + 1.0));
                        Graphics.DrawRect(p1, p2, new Color(138, 38, 190), (Depth)0.95f);
                        Graphics.DrawRect(p1 + new Vec2(-2f, -2f), p2 + new Vec2(2f, -2f), Colors.BlueGray, (Depth)0.9f);
                        Graphics.DrawStringOutline(text2, new Vec2(Layer.HUD.width - stringWidth2 - x + num2, (float)((double)Layer.HUD.height - (double)x - 8.0)), Color.White, Color.Black, (Depth)1f);
                    }
                }
            }
            base.PostDrawLayer(layer);
        }
    }
}
