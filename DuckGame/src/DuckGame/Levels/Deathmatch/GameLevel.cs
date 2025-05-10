﻿using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using System.Web.Profile;
using System.Windows.Forms;

namespace DuckGame
{
    public class GameLevel : XMLLevel, IHaveAVirtualTransition
    {
        protected FollowCam _followCam;
        protected GameMode _mode;
        public GameMode mode
        {
            get
            {
                return _mode;
            }
        }
        private RandomLevelNode _randomLevel;
        private bool _validityTest;
        private float _infoSlide;
        private float _infoWait;
        private bool _showInfo = true;
        public bool _editorTestMode;
        public string levelInputString;
        private static bool first;
        private bool _startedMatch;
        private static int _numberOfDucksSpawned;
        private int wait;
        public override string networkIdentifier => level;

        public FollowCam followCam => _followCam;

        public bool isRandom => _randomLevel != null;

        public bool cityRaining;
        public bool Raining;
        public bool heavyRain;
        public float rainDarken;
        public static float rainwind;
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
            rainwind = 0;
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
        
        public virtual void MatchStart() => this._startedMatch = true; 

        public static int NumberOfDucks
        {
            get => _numberOfDucksSpawned < 2 ? 2 : _numberOfDucksSpawned;
            set => _numberOfDucksSpawned = value;
        }


        public int acidity = -1;
        public override void Start()
        {
            _things.RefreshState();
            foreach (Duck t in things[typeof(Duck)])
            {
                followCam.Add(t);
            }
            followCam.Adjust();



            if (level != "RANDOM" && First<RainTile>() == null && First<SnowTile>() == null && DGRSettings.Weather)
            {
                Random rnd = Rando.generator;

                if (Network.isActive)
                {
                    int rand = DuckNetwork.levelIndex;
                    for (int i = 0; i < Profiles.active.Count; i++)
                    {
                        Profile p = Profiles.active[i];
                        rand += p.flagIndex;
                        rand += (int)(p.funslider * 100);
                        rand += p.customTeams.Count;
                        if (!Network.lanMode) rand += p.preferredColor;
                        if (p.team != null) rand += Teams.IndexOf(p.team);
                    }
                    Rando.generator = new Random(rand);
                }
                else Rando.generator = new Random(Rando.Int(int.MaxValue - 1));

                if (Rando.Int(4) == 0) //20% chance
                {
                    rainRando = new Random(Rando.Int(int.MaxValue - 1));
                    acidity = -1;
                    if (Program.BirthdayDGR)
                    {
                        DGRBirthday = true;

                        CityBackground cbg = First<CityBackground>();
                        if (cbg != null)
                        {
                            cbg.SkySay("HAPPY BIRTHDAY DUCK GAME REBUILT!", new Vec2(-20, 60));
                        }
                    }
                    else
                    {
                        RainParticel.c = Color.White * 0.8f;
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
                            rainSound = new LoopingSound("sizzle", 1, -3)
                            {
                                volume = 0.2f
                            };
                            darkenRainer = 0.9f;
                            rainwind = Rando.Float(-2, 2);
                            lightningRNG = Rando.Int(4000, 6000);
                            if (Rando.Int(2) == 0)
                            {
                                darkenRainer = 0.85f;
                                rainSound.volume = 0.5f;
                                rainwind = Rando.Float(4, 5) * Rando.ChooseInt(-1, 1);
                                lightningRNG = (int)Math.Floor(0.75f * lightningRNG);
                                heavyRain = true;
                            }
                            rainDarken = darkenRainer;
                            rainwindto = rainwind;
                        }
                        else if (First<OfficeTileset>() != null)
                        {
                            rainSound = new LoopingSound("sizzle", 1, -3)
                            {
                                volume = 0.2f
                            };
                            darkenRainer = 0.9f;
                            Raining = true;
                            rainwind = Rando.Float(-2, 2);
                            lightningRNG = Rando.Int(4000, 6000);
                            if (Rando.Int(2) == 0)
                            {
                                darkenRainer = 0.85f;
                                rainSound.volume = 0.5f;
                                rainwind = Rando.Float(4, 5) * Rando.ChooseInt(-1, 1);
                                lightningRNG = (int)Math.Floor(0.75f * lightningRNG);
                                heavyRain = true;
                            }
                            rainDarken = darkenRainer;
                            rainwindto = rainwind;
                        }
                        else if (First<CityTileset>() != null)
                        {
                            if (Rando.Int(15000) == 0) //ACIDD GRAHHH -NiK0
                            {
                                acidTimer = Rando.Int(1200, 3000);
                            }
                            CityBackground cbg = First<CityBackground>();
                            string forecast = "RAINY";
                            if (Rando.Int(1) == 0)
                            {
                                rainSound = new LoopingSound("sizzle", 1, -3)
                                {
                                    volume = 0.2f
                                };
                                darkenRainer = 0.9f;
                                cityRaining = true;
                                rainwind = Rando.Float(-2, 2);
                                lightningRNG = Rando.Int(4000, 6000);
                                if (Rando.Int(2) == 0)
                                {
                                    forecast = "HEAVY RAIN";
                                    darkenRainer = 0.85f;
                                    rainSound.volume = 0.5f;
                                    rainwind = Rando.Float(4, 5) * Rando.ChooseInt(-1, 1);
                                    lightningRNG = (int)Math.Floor(0.7f * lightningRNG);
                                    if (Rando.Int(1) == 0)
                                    {
                                        lightningRNG = (int)Math.Floor(0.7f * lightningRNG);
                                        forecast = "THUNDERSTORM";
                                        darkenRainer = 0.8f;
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
                else if (First<SpaceTileset>() == null)
                {
                    //have wind happen anyways
                    rainwind = Rando.Float(-1, 1);
                    if (Rando.Int(1) == 0)
                    {
                        rainwind = Rando.Float(1, 2) * Rando.ChooseInt(-1, 1);
                        if (Rando.Int(3) == 0) rainwind = Rando.Float(3, 5) * Rando.ChooseInt(-1, 1);
                    }
                    rainwindto = rainwind;
                }
                Rando.generator = rnd;
            }
        }

        protected override void OnTransferComplete(NetworkConnection c)
        {
            current.things.RefreshState();
            int num = 0;
            List<Duck> duckList = new List<Duck>();
            foreach (Duck t in things[typeof(Duck)])
            {
                t.localSpawnVisible = false;
                followCam.Add(t);
                num++;
                duckList.Add(t);
            }
            _numberOfDucksSpawned = num;
            if (_numberOfDucksSpawned > 4) TeamSelect2.eightPlayersActive = true;
            followCam.Adjust();
            _mode.pendingSpawns = duckList;
            base.OnTransferComplete(c);
        }

        protected override void OnAllClientsReady()
        {
            if (Network.isServer) Send.Message(new NMBeginLevel());
            base.OnAllClientsReady();
        }
        public List<Profile> toSend = new List<Profile>();
        public override void OnNetworkConnecting(Profile p)
        {
            toSend.Add(p);
            toSendDelay = 30;
            base.OnNetworkConnecting(p);
        }
        public override void OnNetworkConnected(Profile p)
        {
            base.OnNetworkConnected(p);
        }

        public float snowTimer;
        public int lightningRNG;
        public float rainTimer;
        public float darkenRainer;
        public LoopingSound rainSound;
        public bool unrain;
        public int acidTimer = -1;
        //DGR was made on the 3rd of august, if weather is enabled and its currently the date all weather will be replaced by
        //confetti falling from the sky -NiK0
        public bool DGRBirthday;

        public int toSendDelay;
        public Random rainRando;
        public override void Update()
        {
            if (toSend.Count > 0)
            {
                toSendDelay--;
                if (toSendDelay <= 0)
                {
                    for (int i = 0; i < toSend.Count; i++)
                    {
                        Profile p = toSend[i];
                        if (p != null && p.connection != null) Send.Message(new NMBeginLevel(), p.connection);
                    }
                    if (toSendDelay <= -20) toSend.Clear();
                }
            }
            Random realRando = Rando.generator;
            Rando.generator = rainRando;
            if (DGRBirthday)
            {
                rainTimer += 8f * DGRSettings.ActualParticleMultiplier;
                if (rainTimer > 1)
                {
                    for (int i = 0; i < rainTimer; i++)
                    {
                        rainTimer -= 1;
                        Vec2 pPosition = new Vec2(Rando.Float(ExtendedLeft, ExtendedRight), ExtendedTop);
                        ConfettiParticle confettiParticle = new ConfettiParticle();
                        confettiParticle.Init(pPosition.x + Rando.Float(-4f, 0f), pPosition.y + Rando.Float(-4f, 6f), new Vec2(Rando.Float(-1f, 0f), Rando.Float(-1f, 1f)), 0.01f);
                        confettiParticle._color = Color.Pink;
                        Add(confettiParticle);
                        confettiParticle = new ConfettiParticle();
                        confettiParticle.Init(pPosition.x + Rando.Float(-4f, 0f), pPosition.y + Rando.Float(-4f, 6f), new Vec2(Rando.Float(-1f, 0f), Rando.Float(-1f, 1f)), 0.01f);
                        confettiParticle._color = Color.DeepPink;
                        Add(confettiParticle);
                    }
                }
            }
            else if (Raining)
            {

                if (rainSound != null)
                {
                    if (heavyRain) rainSound.volume = Lerp.Float(rainSound.volume, 0.5f, 0.01f);
                    else rainSound.volume = Lerp.Float(rainSound.volume, 0.2f, 0.01f);

                    rainSound.pitch = Rando.Float(-3f, -3.1f);

                    if (rainSound._effect != null && rainSound._effect._instance != null && rainSound._effect._instance.Platform_GetProgress() > 0.5f) rainSound._effect._instance._position = 0;
                }

                if (Rando.Int(lightningRNG) == 0)
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
                rainTimer += (heavyRain ? 2 : 1.5f) * DGRSettings.ActualParticleMultiplier;
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

                if (acidTimer > 0)
                {
                    acidTimer--;
                    if (acidTimer == 300)
                    {
                        CityBackground cbg = First<CityBackground>();
                        if (cbg != null)
                        {
                            cbg.SkySay("NOW INCOMING", new Vec2(-20, 60));
                            cbg.SkySay("ACID RAIN", new Vec2(-20, 70));
                        }
                    }
                    if (acidTimer < 60)
                    {
                        RainParticel.c = Lerp.Color(RainParticel.c, Color.Yellow, 0.1f);
                        rainwindto *= 1.01f;
                    }
                }
                if (Rando.Int(100000) == 0)
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


                if (rainSound != null)
                {
                    if (heavyRain) rainSound.volume = Lerp.Float(rainSound.volume, 0.5f, 0.01f);
                    else rainSound.volume = Lerp.Float(rainSound.volume, 0.2f, 0.01f);

                    rainSound.pitch = Rando.Float(-3f, -3.1f);

                    if (rainSound._effect != null && rainSound._effect._instance != null && rainSound._effect._instance.Platform_GetProgress() > 0.5f) rainSound._effect._instance._position = 0;
                }

                //ignore this mess im just quickly assembling this if you wanna make it better go ahead
                //-NiK0
                if (Rando.Int(lightningRNG) == 0)
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
                rainTimer += (heavyRain ? 2 : 1.5f) * DGRSettings.ActualParticleMultiplier;
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
                snowTimer += 0.1f * DGRSettings.ActualParticleMultiplier;
                if (snowTimer > 1)//lol
                {
                    snowTimer = 0;
                    Vec2 v = new Vec2(Rando.Float(topLeft.x - 128, bottomRight.x + 128), topLeft.y - 100);
                    SnowFallParticle sn = new SnowFallParticle(v.x, v.y, new Vec2(0, 1), Rando.Int(2) == 0);
                    sn.life = Rando.Float(1, 2);
                    //sn._size = Rando.Float(1, 2);
                    Add(sn);
                }
            }
            Rando.generator = realRando;

            MonoMain.timeInMatches++;
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
            if (layer == Layer.Console && DGRSettings.QOLScoreThingButWithoutScore)
            {
                IEnumerable<Profile> profileList = Profiles.activeNonSpectators;
                float opacity = DGRSettings.HSDOpacity / 100f;
                float fontScale = DGRSettings.HSDFontScale / 50f;
                float spacing = DGRSettings.HSDSpacing;

                if (DGRSettings.HSDSorting == 1 || DGRSettings.HSDSorting == 3) profileList = profileList.OrderByDescending(x => x?.team.score);
                if (DGRSettings.HSDSorting == 2 || DGRSettings.HSDSorting == 3) profileList = profileList.OrderByDescending(x => x?.duck?.dead);

                int highestScore = 0;
                Team t = null;
                if (DGRSettings.HSDStarForHighestScore)
                {
                    foreach (Profile p in profileList)
                    {
                        if (p.team.score > highestScore)
                        {
                            highestScore = p.team.score;
                            t = p.team;
                        }
                    }
                }

                int i = 0;
                float xs = 0;
                float ys = 0;
                Vec2 vAlign = DGRSettings.HSDHorizontal? new Vec2(DGRSettings.HSDRightToLeft?-spacing:spacing, 0): new Vec2(0, spacing);

                Vec2 generalOffset = new Vec2(DGRSettings.HSDXoffset * 4, DGRSettings.HSDYoffset * 4);

                foreach (Profile p in profileList)
                {
                    string s = DGRSettings.HSDRightToLeft?
                        ((p.team.score == t?.score?"@STARGOODY@":"") + (DGRSettings.HSDShowScore ? p.team.score + " " : "") + (DGRSettings.HSDClearNames ? Extensions.CleanFormatting(p.name) : p.name)):
                        ((DGRSettings.HSDClearNames ? Extensions.CleanFormatting(p.name) : p.name) + (DGRSettings.HSDShowScore ? " " + p.team.score : "") + (p.team.score == t?.score ? "@STARGOODY@" : ""));
                    Color c = DGRSettings.HSDShowColors?p.persona.colorUsable:Color.White;

                    if (DGRSettings.HSDRightToLeft)
                    {
                        if (DGRSettings.HSDBlackOutline) Graphics.DrawRightAllignedOutlinedString(s, new Vec2(generalOffset.x + xs + vAlign.x * i, generalOffset.y + ys + vAlign.y * i), c * opacity, Color.Black, 1, null, fontScale);
                        else Graphics.DrawRightAlignedString(s, new Vec2(generalOffset.x + xs + vAlign.x * i, generalOffset.y + ys + vAlign.y * i), c * opacity, 1, null, fontScale);
                    }
                    else
                    {
                        if (DGRSettings.HSDBlackOutline) Graphics.DrawStringOutline(s, new Vec2(generalOffset.x + xs + vAlign.x * i, generalOffset.y + ys + vAlign.y * i), c * opacity, Color.Black, 1, null, fontScale);
                        else Graphics.DrawString(s, new Vec2(generalOffset.x + xs + vAlign.x * i, generalOffset.y + ys + vAlign.y * i), c * opacity, 1, null, fontScale);
                    }

                    if (DGRSettings.HSDHorizontal) xs += Graphics.GetStringWidth(s, false, fontScale) * (DGRSettings.HSDRightToLeft ? -1 : 1);
                    else ys += Graphics.GetStringHeight(s) * fontScale;
                    i++;
                }
                if (DGRSettings.HSDShowRoundsLeft)
                {
                    i+= 2;
                    int num = GameMode.roundsBetweenIntermission - GameMode.numMatchesPlayed;
                    string s = DGRSettings.HSDRightToLeft?(num.ToString() + " :rounds left") :("rounds left: " + num.ToString());
                    Color c = Color.White;
                    if (DGRSettings.HSDRightToLeft)
                    {
                        if (DGRSettings.HSDBlackOutline) Graphics.DrawRightAllignedOutlinedString(s, new Vec2(generalOffset.x + xs + vAlign.x * i, generalOffset.y + ys + vAlign.y * i), c * opacity, Color.Black, 1, null, fontScale);
                        else Graphics.DrawRightAlignedString(s, new Vec2(generalOffset.x + xs + vAlign.x * i, generalOffset.y + ys + vAlign.y * i), c * opacity, 1, null, fontScale);
                    }
                    else
                    {
                        if (DGRSettings.HSDBlackOutline) Graphics.DrawStringOutline(s, new Vec2(generalOffset.x + xs + vAlign.x * i, generalOffset.y + ys + vAlign.y * i), c * opacity, Color.Black, 1, null, fontScale);
                        else Graphics.DrawString(s, new Vec2(generalOffset.x + xs + vAlign.x * i, generalOffset.y + ys + vAlign.y * i), c * opacity, 1, null, fontScale);
                    }
                }
            }
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
                    float num1 = (float)((stringWidth1 + x + 12f) * (1f - _infoSlide));
                    Vec2 p1 = new Vec2(-num1, x - 1f);
                    Vec2 p2 = new Vec2((float)(x + stringWidth1 + 4f), x + 10f);
                    Graphics.DrawRect(p1, p2 + new Vec2(-num1, 0f), new Color(13, 130, 211), (Depth)0.95f);
                    Graphics.DrawRect(p1 + new Vec2(-2f, 2f), p2 + new Vec2((float)(-num1 + 2f), 2f), Colors.BlueGray, (Depth)0.9f);
                    Graphics.DrawStringOutline(text1, p1 + new Vec2(x, 2f), Color.White, Color.Black, (Depth)1f);
                    if (data.workshopData != null && data.workshopData.author != null && data.workshopData.author != "")
                    {
                        string text2 = "BY " + data.workshopData.author;
                        float stringWidth2 = Graphics.GetStringWidth(text2);
                        float num2 = (float)((stringWidth2 + x + 12f) * (1f - _infoSlide));
                        p1 = new Vec2((float)(Layer.HUD.width - stringWidth2 - x - 5f) + num2, (float)(Layer.HUD.height - x - 10f));
                        p2 = new Vec2(Layer.HUD.width + num2, (float)(Layer.HUD.height - x + 1f));
                        Graphics.DrawRect(p1, p2, new Color(138, 38, 190), (Depth)0.95f);
                        Graphics.DrawRect(p1 + new Vec2(-2f, -2f), p2 + new Vec2(2f, -2f), Colors.BlueGray, (Depth)0.9f);
                        Graphics.DrawStringOutline(text2, new Vec2(Layer.HUD.width - stringWidth2 - x + num2, (float)(Layer.HUD.height - x - 8f)), Color.White, Color.Black, (Depth)1f);
                    }
                }
            }
            base.PostDrawLayer(layer);
        }
    }
}
