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

        public bool cityRaining;
        public bool Raining;
        public bool heavyRain;
        public float rainDarken;
        public float rainwind;
        public float rainwindto;
        public bool Snowing;
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
            first = !first;
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
            base.Initialize();
            if (Network.isActive)
                core.gameInProgress = true;
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
                        chosenSpawns.Add(source1.ElementAt(Rando.Int(source1.Count - 1)));
                    }
                    else
                    {
                        IOrderedEnumerable<SpawnPoint> source2 = source1.OrderByDescending(x =>
                       {
                           int val2 = 9999999;
                           foreach (Transform transform in chosenSpawns)
                               val2 = (int)Math.Min((transform.position - x.position).length, val2);
                           return val2;
                       });
                        chosenSpawns.Add(source2.First());
                    }
                }
                foreach (SpawnPoint spawnPoint in source1)
                {
                    if (!chosenSpawns.Contains(spawnPoint))
                        Remove(spawnPoint);
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
                Add(pyramidBackground);
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
                Add(prepareSpawn);
            }
        }

        public virtual void MatchStart() { } //=> this._startedMatch = true;

        public static int NumberOfDucks
        {
            get => _numberOfDucksSpawned < 2 ? 2 : _numberOfDucksSpawned;
            set => _numberOfDucksSpawned = value;
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

            if (level != "RANDOM" && Rando.Float(1) <= DGRSettings.S_RandomWeather)
            {
                RainParticel.c = new Color(0, 112, 168);
                RainParticel.flud = Fluid.Water;
                if (cold)
                {
                    Snowing = true;
                }
                else if (First<NatureTileset>() != null)
                {
                    Raining = true;
                    NatureBackground ng = First<NatureBackground>();
                    if (ng != null)
                    {
                        Remove(ng._parallax);
                        ng.Initialize();
                    }
                    rainSound = new LoopingSound("sizzle", 1, -3);
                    rainSound.volume = 0.2f;
                    darkenRainer = 0.8f;
                    rainwind = Rando.Float(-2, 2);
                    lightningRNG = Rando.Int(1200, 2400);
                    if (Rando.Int(2) == 0)
                    {
                        darkenRainer = 0.8f;
                        rainSound.volume = 0.5f;
                        rainwind = Rando.Float(4, 5) * Rando.ChooseInt(-1, 1);
                        lightningRNG = (int)Math.Floor(0.2f * lightningRNG);
                        heavyRain = true;
                    }
                    rainDarken = darkenRainer;
                    rainwindto = rainwind;
                }
                else if (First<OfficeTileset>() != null && (Rando.Int(1) == 1 || DGRSettings.S_RandomWeather > 9.9f))
                {
                    rainSound = new LoopingSound("sizzle", 1, -3);
                    rainSound.volume = 0.2f;
                    darkenRainer = 0.8f;
                    Raining = true;
                    rainwind = Rando.Float(-2, 2);
                    lightningRNG = Rando.Int(2400, 4800);
                    if (Rando.Int(2) == 0)
                    {
                        darkenRainer = 0.8f;
                        rainSound.volume = 0.5f;
                        rainwind = Rando.Float(4, 5) * Rando.ChooseInt(-1, 1);
                        lightningRNG = (int)Math.Floor(0.2f * lightningRNG);
                        heavyRain = true;
                    }
                    rainDarken = darkenRainer;
                    rainwindto = rainwind;
                }
                else if (First<CityTileset>() != null)
                {
                    CityBackground cbg = First<CityBackground>();
                    string forecast = "RAINY";
                    if (Rando.Int(1) == 0)
                    {
                        rainSound = new LoopingSound("sizzle", 1, -3);
                        rainSound.volume = 0.2f;
                        darkenRainer = 0.8f;
                        cityRaining = true;
                        rainwind = Rando.Float(-2, 2);
                        lightningRNG = Rando.Int(1200, 2400);
                        if (Rando.Int(2) == 0)
                        {
                            forecast = "HEAVY RAIN";
                            darkenRainer = 0.8f;
                            rainSound.volume = 0.5f;
                            rainwind = Rando.Float(4, 5) * Rando.ChooseInt(-1, 1);
                            lightningRNG = (int)Math.Floor(0.4f * lightningRNG);
                            if (Rando.Int(1) == 0)
                            {
                                lightningRNG = (int)Math.Floor(0.3f * lightningRNG);
                                forecast = "THUNDERSTORM";
                                darkenRainer = 0.55f;
                            }
                            heavyRain = true;
                        }
                        rainDarken = darkenRainer;
                    }
                    else
                    {
                        forecast = "SNOW";
                        Snowing = true;
                    }
                    if (cbg != null)
                    {
                        cbg.SkySay("TODAYS FORECAST", new Vec2(-20, 60));
                        cbg.SkySay(forecast, new Vec2(-20, 70));
                    }
                    rainwindto = rainwind;
                }
            }
        }

        protected override void OnTransferComplete(NetworkConnection c)
        {
            current.things.RefreshState();
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
            _numberOfDucksSpawned = num;
            if (_numberOfDucksSpawned > 4)
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

        public float snowTimer;
        public int lightningRNG;
        public float rainTimer;
        public float darkenRainer;
        public LoopingSound rainSound;
        public bool unrain;
        public int acidTimer;
        public bool acider;
        public override void Update()
        {
            if (Raining)
            {
                if (Rando.Int(60000) == 0 && DGRSettings.S_RandomWeather < 0.49f)
                {
                    if (heavyRain && Rando.Int(4) != 4)
                    {
                        heavyRain = false;
                        rainwindto /= 2;
                        darkenRainer = 0.8f;
                        lightningRNG *= 5;
                    }
                    else
                    {
                        Raining = false;
                        unrain = true;
                    }
                }
                rainwind = Lerp.Float(rainwind, rainwindto, 0.1f);
                if (Rando.Int(6000) == 0)
                {
                    if (!heavyRain && Rando.Int(1) == 0)
                    {
                        rainwindto *= 2;
                        darkenRainer = 0.8f;
                        heavyRain = true;
                        lightningRNG = (int)Math.Floor(0.2f * lightningRNG);
                    }
                    rainwindto *= -1;
                }


                if (heavyRain) rainSound.volume = Lerp.Float(rainSound.volume, 0.5f, 0.01f);
                else rainSound.volume = Lerp.Float(rainSound.volume, 0.2f, 0.01f);

                rainSound.pitch = Rando.Float(-3f, -3.1f);

                if (rainSound._effect._instance.Platform_GetProgress() > 0.5f) rainSound._effect._instance._position = 0;

                //ignore this mess im just quickly assembling this if you wanna make it better go ahead
                //-NiK0
                if (DGRSettings.S_WeatherLighting > 0 && (int)Math.Round((float)Rando.Int(lightningRNG) / DGRSettings.S_WeatherLighting) == 0)
                {
                    rainDarken = 1.2f;
                    Add(new BGLightning(Rando.Float(-30, 270), 0));
                    SFX.Play("balloonPop", 1, Rando.Float(-3, -4));
                }
                rainDarken = Lerp.Float(rainDarken, darkenRainer, 0.005f);
                Layer.Game.fade = rainDarken;
                Layer.Glow.fade = rainDarken;
                Layer.Blocks.fade = rainDarken;
                Layer.Virtual.fade = rainDarken;
                Layer.Parallax.fade = rainDarken;
                Layer.Foreground.fade = rainDarken;
                Layer.Background.fade = rainDarken;
                rainTimer += DGRSettings.S_WeatherMultiplier / (heavyRain ? 2 : 1.5f);
                if (rainTimer > 1)
                {
                    for (int i = 0; i < rainTimer; i++)
                    {
                        rainTimer -= 1;
                        Add(new RainParticel(new Vec2(Rando.Float(topLeft.x - 400, bottomRight.x + 400), topLeft.y - 200), rainwind));
                    }
                }
            }
            else if (cityRaining)
            {
                rainwind = Lerp.Float(rainwind, rainwindto, 0.1f);
                if (Rando.Int(100000) == 0 && !acider)
                {
                    CityBackground cbg = First<CityBackground>();
                    if (cbg != null)
                    {
                        acidTimer = 240;
                        acider = true;
                        cbg.SkySay("NOW INCOMING", new Vec2(-20, 60));
                        cbg.SkySay("ACID RAIN", new Vec2(-20, 70));
                    }
                }
                if (acidTimer > 0)
                {
                    acidTimer--;
                    if (acidTimer < 60)
                    {
                        RainParticel.c = Lerp.Color(RainParticel.c, Color.Yellow, 0.1f);
                        RainParticel.flud = new FluidData(0, RainParticel.c.ToVector4(), 0, "acid");
                        rainwindto *= 1.01f;
                    }
                }
                if (Rando.Int(10000) == 0)
                {
                    if (!heavyRain && Rando.Int(1) == 0)
                    {
                        rainwindto *= 2;
                        darkenRainer = 0.8f;
                        heavyRain = true;
                        lightningRNG = (int)Math.Floor(0.2f * lightningRNG);
                    }
                    rainwindto *= -1;
                }


                if (heavyRain) rainSound.volume = Lerp.Float(rainSound.volume, 0.5f, 0.01f);
                else rainSound.volume = Lerp.Float(rainSound.volume, 0.2f, 0.01f);

                rainSound.pitch = Rando.Float(-3f, -3.1f);

                if (rainSound._effect._instance.Platform_GetProgress() > 0.5f) rainSound._effect._instance._position = 0;

                //ignore this mess im just quickly assembling this if you wanna make it better go ahead
                //-NiK0
                if (DGRSettings.S_WeatherLighting > 0 && (int)Math.Round((float)Rando.Int(lightningRNG) / DGRSettings.S_WeatherLighting) == 0)
                {
                    rainDarken = 1.2f;
                    Add(new BGLightning(Rando.Float(-30, 270), 0));
                    SFX.Play("balloonPop", 1, Rando.Float(-3, -4));
                }
                rainDarken = Lerp.Float(rainDarken, darkenRainer, 0.005f);
                Layer.Game.fade = rainDarken;
                Layer.Glow.fade = rainDarken;
                Layer.Blocks.fade = rainDarken;
                Layer.Virtual.fade = rainDarken;
                Layer.Parallax.fade = rainDarken;
                Layer.Foreground.fade = rainDarken;
                Layer.Background.fade = rainDarken;
                rainTimer += DGRSettings.S_WeatherMultiplier / (heavyRain ? 2 : 1.5f);
                if (rainTimer > 1)
                {
                    for (int i = 0; i < rainTimer; i++)
                    {
                        rainTimer -= 1;
                        Add(new RainParticel(new Vec2(Rando.Float(topLeft.x - 400, bottomRight.x + 400), topLeft.y - 200), rainwind));
                    }
                }
            }
            else if (unrain)
            {
                rainSound.volume = Lerp.Float(rainSound.volume, 0, 0.01f);
                rainDarken = Lerp.Float(rainDarken, 1, 0.005f);
                Layer.Game.fade = rainDarken;
                Layer.Glow.fade = rainDarken;
                Layer.Blocks.fade = rainDarken;
                Layer.Virtual.fade = rainDarken;
                Layer.Parallax.fade = rainDarken;
                Layer.Foreground.fade = rainDarken;
                Layer.Background.fade = rainDarken;
            }
            else if (Snowing)
            {
                snowTimer += 0.1f * DGRSettings.S_WeatherMultiplier;
                if (snowTimer > 1)//lol
                {
                    for (int i = 0; i < snowTimer; i++)
                    {
                        snowTimer -= 1;
                        Vec2 v = new Vec2(Rando.Float(topLeft.x - 128, bottomRight.x + 128), topLeft.y - 100);
                        SnowFallParticle sn = new SnowFallParticle(v.x, v.y, new Vec2(0, 1), Rando.Int(2) == 0);
                        //sn.life = Rando.Float(1, 2);
                        //sn._size = Rando.Float(1, 2);
                        Add(sn);
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
