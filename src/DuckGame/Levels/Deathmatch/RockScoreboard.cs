// Decompiled with JetBrains decompiler
// Type: DuckGame.RockScoreboard
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DuckGame
{
    public class RockScoreboard : Level
    {
        private static Level _returnLevel;
        private bool _afterHighlights;
        private ScoreBoardMode _mode;
        private ScoreBoardState _state = ScoreBoardState.Intro;
        private float _throwWait = 1f;
        private float _afterThrowWait = 1f;
        private Sprite _bleachers;
        private Sprite _bleacherSeats;
        private Sprite _intermissionText;
        private Sprite _winnerPost;
        private Sprite _winnerBanner;
        private BitmapFont _font;
        private FieldBackground _field;
        private WallLayer _wall;
        private FieldBackground _fieldForeground;
        private FieldBackground _fieldForeground2;
        private ContinueCountdown _netCountdown;
        private GinormoBoard _scoreBoard;
        private bool _shiftCamera;
        private bool _finished;
        private bool _getScreenshot;
        private Sprite _finalSprite;
        private float _intermissionSlide = 1f;
        private float _controlSlide;
        private int _controlMessage = -1;
        private CornerDisplay _continueHUD;
        private float _desiredScroll;
        private float _animWait = 1f;
        private float _backWait = 1f;
        private float _showScoreWait = 1f;
        private float _fieldWidth = 680f;
        private bool _skipFade;
        private float _winnerWait = 1f;
        private bool _matchOver;
        private bool _tie;
        private bool _viewBoard;
        private bool _quit;
        private bool _misfire;
        private float _cameraWait = 1f;
        private bool _takePicture;
        private int _playedBeeps;
        private bool _playedFlash;
        private float _cameraFadeVel;
        private int _flashSkipFrames;
        private float _fieldScroll;
        private SinWave _sin = (SinWave)0.01f;
        private Crowd _crowd;
        public List<Slot3D> _slots = new List<Slot3D>();
        private Slot3D _highestSlot;
        private Team _winningTeam;
        public static RenderTarget2D finalImage;
        public static RenderTarget2D finalImage2;
        private float _backgroundFade;
        private Layer _sunLayer;
        private RenderTarget2D _sunshineTarget;
        private RenderTarget2D _screenTarget;
        private RenderTarget2D _pixelTarget;
        public static bool _sunEnabled = true;
        private RockWeather _weather;
        private List<InputObject> _inputs = new List<InputObject>();
        public static bool initializingDucks = false;
        public static bool wallMode = false;
        private Thing sunThing;
        private Thing rainbowThing;
        private Thing rainbowThing2;
        private static bool _drawingSunTarget = false;
        private static bool _drawingLighting = false;
        private static bool _drawingNormalTarget = false;
        private bool _hatSelect;
        private bool _focusRock;
        private bool _droppedConfetti;
        private float _confettiDrop;
        private bool _returnToScoreboard;
        private Material _sunshineMaterial;
        private Material _sunshineMaterialBare;

        public override string networkIdentifier
        {
            get
            {
                if (mode == ScoreBoardMode.ShowScores)
                    return "@ROCKTHROW|SHOWSCORE";
                return afterHighlights ? "@ROCKTHROW|SHOWEND" : "@ROCKTHROW|SHOWWINNER";
            }
        }

        public static Level returnLevel
        {
            get
            {
                if (RockScoreboard._returnLevel == null)
                    RockScoreboard._returnLevel = new GameLevel(Deathmatch.RandomLevelString());
                return RockScoreboard._returnLevel;
            }
        }

        public bool afterHighlights
        {
            get => _afterHighlights;
            set => _afterHighlights = value;
        }

        public RockScoreboard(Level r = null, ScoreBoardMode mode = ScoreBoardMode.ShowScores, bool afterHighlights = false)
        {
            _afterHighlights = afterHighlights;
            if (Network.isServer)
                RockScoreboard._returnLevel = r;
            _mode = mode;
            if (mode != ScoreBoardMode.ShowWinner)
                return;
            _state = ScoreBoardState.None;
        }

        public ScoreBoardMode mode => _mode;

        public Vec3 fieldAddColor
        {
            set
            {
                if (_field == null)
                    return;
                _field.colorAdd = value;
                _fieldForeground.colorAdd = value;
                _fieldForeground2.colorAdd = value;
                _wall.colorAdd = value;
            }
        }

        public Vec3 fieldMulColor
        {
            set
            {
                if (_field == null)
                    return;
                _field.colorMul = value;
                _fieldForeground.colorMul = value;
                _fieldForeground2.colorMul = value;
                _wall.colorMul = value;
            }
        }

        public ContinueCountdown netCountdown => _netCountdown;

        public float cameraY
        {
            get => camera.y;
            set
            {
                camera.y = value;
                _field.ypos = camera.y * 1.4f;
            }
        }

        public int controlMessage
        {
            get => _controlMessage;
            set
            {
                if (_controlMessage != value)
                {
                    HUD.CloseAllCorners();
                    if (value == 0)
                        HUD.AddCornerControl(HUDCorner.BottomRight, "@START@SKIP");
                    else if (value > 0)
                    {
                        if (!Network.isServer)
                        {
                            _continueHUD = HUD.AddCornerMessage(HUDCorner.BottomRight, "WAITING");
                        }
                        else
                        {
                            _continueHUD = HUD.AddCornerControl(HUDCorner.BottomRight, "@START@CONTINUE");
                            if (value > 1)
                            {
                                HUD.AddCornerControl(HUDCorner.BottomLeft, "@CANCEL@QUIT");
                                HUD.AddCornerControl(HUDCorner.TopRight, "@MENU2@LOBBY");
                            }
                        }
                    }
                }
                _controlMessage = value;
            }
        }

        public void SetWeather(Weather w)
        {
            if (_weather == null)
                return;
            _weather.SetWeather(w);
        }

        public override void SendLevelData(NetworkConnection c)
        {
            if (!Network.isServer)
                return;
            Send.Message(new NMCrowdData(_crowd.NetSerialize()), c);
            Send.Message(new NMWeatherData(_weather.NetSerialize()), c);
        }

        public override void OnMessage(NetMessage message)
        {
            if (message is NMCrowdData && Network.isClient)
                _crowd.NetDeserialize((message as NMCrowdData).data);
            if (!(message is NMWeatherData) || !Network.isClient)
                return;
            _weather.NetDeserialize((message as NMWeatherData).data);
        }

        public InputProfile GetNetInput(sbyte index) => index >= _inputs.Count || _inputs[index].duckProfile == null || _inputs[index].duckProfile.inputProfile == null ? new InputProfile() : _inputs[index].duckProfile.inputProfile;

        public override void Initialize()
        {
            if (Network.isActive && Network.isServer && _mode == ScoreBoardMode.ShowScores)
            {
                int num = 0;
                foreach (Profile profile in DuckNetwork.profiles)
                {
                    if (profile.connection != null && profile.slotType != SlotType.Spectator)
                    {
                        InputObject inputObject = new InputObject
                        {
                            profileNumber = (sbyte)num
                        };
                        Level.Add(inputObject);
                        _inputs.Add(inputObject);
                        ++num;
                    }
                }
            }
            HighlightLevel.didSkip = false;
            if (_afterHighlights)
                _skipFade = true;
            _weather = new RockWeather(this);
            _weather.Start();
            Level.Add(_weather);
            for (int index = 0; index < 350; ++index)
                _weather.Update();
            if (RockScoreboard._sunEnabled)
            {
                float num = 9f / 16f;
                _sunshineTarget = new RenderTarget2D(DuckGame.Graphics.width / 12, (int)(Graphics.width * num) / 12);
                _screenTarget = new RenderTarget2D(DuckGame.Graphics.width, (int)(Graphics.width * num));
                _pixelTarget = new RenderTarget2D(160, (int)(320.0 * num / 2.0));
                _sunLayer = new Layer("SUN LAYER", 99999);
                Layer.Add(_sunLayer);
                Thing thing = new SpriteThing(150f, 120f, new Sprite("sun"));
                thing.z = -9999f;
                thing.depth = -0.99f;
                thing.layer = _sunLayer;
                thing.xscale = 1f;
                thing.yscale = 1f;
                thing.collisionSize = new Vec2(1f, 1f);
                thing.collisionOffset = new Vec2(0f, 0f);
                Level.Add(thing);
                sunThing = thing;
                SpriteThing spriteThing1 = new SpriteThing(150f, 80f, new Sprite("rainbow"))
                {
                    alpha = 0.15f,
                    z = -9999f,
                    depth = -0.99f,
                    layer = _sunLayer,
                    xscale = 1f,
                    yscale = 1f,
                    color = new Color(100, 100, 100),
                    collisionSize = new Vec2(1f, 1f),
                    collisionOffset = new Vec2(0f, 0f)
                };
                Level.Add(spriteThing1);
                rainbowThing = spriteThing1;
                rainbowThing.visible = false;
                SpriteThing spriteThing2 = new SpriteThing(150f, 80f, new Sprite("rainbow"))
                {
                    z = -9999f,
                    depth = -0.99f,
                    layer = _sunLayer,
                    xscale = 1f,
                    yscale = 1f,
                    color = new Color((int)byte.MaxValue, (int)byte.MaxValue, (int)byte.MaxValue, 90),
                    collisionSize = new Vec2(1f, 1f),
                    collisionOffset = new Vec2(0f, 0f)
                };
                Level.Add(spriteThing2);
                rainbowThing2 = spriteThing2;
                rainbowThing2.visible = false;
            }
            List<Team> allRandomized = Teams.allRandomized;
            if (!Network.isActive && RockScoreboard.returnLevel == null)
            {
                allRandomized[0].Join(Profiles.DefaultPlayer1);
                allRandomized[1].Join(Profiles.DefaultPlayer2);
                allRandomized[0].score = 10;
                allRandomized[1].score = 2;
                Teams.Player3.score = 3;
                Teams.Player4.score = 4;
            }
            _crowd = new Crowd();
            Level.Add(_crowd);
            Crowd.mood = Mood.Calm;
            _field = new FieldBackground("FIELD", 9999);
            Layer.Add(_field);
            _bleacherSeats = new Sprite("bleacherSeats");
            _bleachers = RockWeather.weather != Weather.Snowing ? new Sprite("bleacherBack") : new Sprite("bleacherBackSnow");
            _bleachers.center = new Vec2(_bleachers.w / 2, _bleachers.height - 3);
            _intermissionText = new Sprite("rockThrow/intermission");
            _winnerPost = new Sprite("rockThrow/winnerPost");
            _winnerBanner = new Sprite("rockThrow/winnerBanner");
            _font = new BitmapFont("biosFont", 8);
            List<Team> teamList1 = new List<Team>();
            foreach (Team team in Teams.all)
            {
                if (team.activeProfiles.Count > 0)
                    teamList1.Add(team);
            }
            int num1 = 0;
            foreach (Team team in Teams.all)
                num1 += team.activeProfiles.Count;
            if (_mode == ScoreBoardMode.ShowWinner)
                Vote.ClearVotes();
            foreach (Team team in teamList1)
            {
                team.rockScore = team.score;
                if (RockScoreboard.wallMode && _mode == ScoreBoardMode.ShowScores)
                    team.score = Math.Min(team.score, GameMode.winsPerSet);
                if (_mode != ScoreBoardMode.ShowWinner && !_afterHighlights)
                {
                    foreach (Profile activeProfile in team.activeProfiles)
                        Vote.RegisterVote(activeProfile, VoteType.None);
                }
            }
            if (Network.isActive)
                Level.Add(new HostTable(160f, 170f));
            bool smallMode = teamList1.Count > 4;
            if (_mode == ScoreBoardMode.ShowScores)
            {
                _intermissionSlide = 1f;
                DuckGame.Graphics.fade = 1f;
                Layer.Game.fade = 0f;
                Layer.Background.fade = 0f;
                Crowd.UpdateFans();
                int num2 = 0;
                int num3 = 0;
                foreach (Team team in teamList1)
                {
                    float num4 = 223f;
                    float ypos = 0f;
                    float num5 = 26f;
                    if (num2 % 4 == 1)
                        num5 = 24f;
                    else if (num2 % 4 == 2)
                        num5 = 27f;
                    else if (num2 % 4 == 3)
                        num5 = 30f;
                    float num6 = (float)(158.0 - num2 % 4 * num5);
                    if (num2 > 3)
                        num6 -= 12f;
                    Depth depth = (Depth)(num6 / 200f);
                    int prevScoreboardScore = team.prevScoreboardScore;
                    int num7 = GameMode.winsPerSet * 2;
                    int num8 = team.score;
                    if (RockScoreboard.wallMode && num8 > GameMode.winsPerSet)
                        num8 = GameMode.winsPerSet;
                    _slots.Add(new Slot3D());
                    if (num8 >= GameMode.winsPerSet && num8 == num3)
                        _tie = true;
                    else if (num8 >= GameMode.winsPerSet && num8 > num3)
                    {
                        _tie = false;
                        num3 = num8;
                        _highestSlot = _slots[_slots.Count - 1];
                    }
                    List<Profile> profileList = new List<Profile>();
                    Profile profile1 = null;
                    bool flag = false;
                    foreach (Profile activeProfile in team.activeProfiles)
                    {
                        if (flag)
                        {
                            profile1 = activeProfile;
                            flag = false;
                        }
                        if (activeProfile.wasRockThrower)
                        {
                            activeProfile.wasRockThrower = false;
                            flag = true;
                        }
                        profileList.Add(activeProfile);
                    }
                    if (profile1 == null)
                        profile1 = team.activeProfiles[0];
                    profileList.Remove(profile1);
                    profileList.Insert(0, profile1);
                    profile1.wasRockThrower = true;
                    byte index = (byte)(_slots.Count - 1);
                    int num9 = 0;
                    foreach (Profile profile2 in profileList)
                    {
                        if (profile2 == profile1)
                        {
                            RockScoreboard.initializingDucks = true;
                            _slots[index].duck = new RockThrowDuck(num4 - num9 * 10, ypos - 16f, profile2);
                            _slots[index].duck.planeOfExistence = index;
                            _slots[index].duck.ignoreGhosting = true;
                            _slots[index].duck.forceMindControl = true;
                            Level.Add(_slots[index].duck);
                            _slots[index].duck.connection = DuckNetwork.localConnection;
                            RockScoreboard.initializingDucks = false;
                            if (_slots[_slots.Count - 1].duck.GetEquipment(typeof(TeamHat)) is TeamHat equipment)
                                equipment.ignoreGhosting = true;
                            _slots[_slots.Count - 1].duck.z = num6;
                            _slots[_slots.Count - 1].duck.depth = depth;
                            _slots[_slots.Count - 1].ai = new DuckAI(profile2.inputProfile);
                            if (Network.isActive && profile2.connection != DuckNetwork.localConnection)
                                _slots[_slots.Count - 1].ai._manualQuack = GetNetInput((sbyte)profile2.networkIndex);
                            _slots[_slots.Count - 1].duck.derpMindControl = false;
                            _slots[_slots.Count - 1].duck.mindControl = _slots[_slots.Count - 1].ai;
                            _slots[_slots.Count - 1].rock = new ScoreRock((float)(num4 + 18.0 + prevScoreboardScore / num7 * _fieldWidth), ypos, profile2)
                            {
                                planeOfExistence = index,
                                ignoreGhosting = true
                            };
                            Level.Add(_slots[_slots.Count - 1].rock);
                            _slots[_slots.Count - 1].rock.z = num6;
                            _slots[_slots.Count - 1].rock.depth = _slots[_slots.Count - 1].duck.depth + 1;
                            _slots[_slots.Count - 1].rock.grounded = true;
                            _slots[_slots.Count - 1].duck.isRockThrowDuck = true;
                        }
                        else
                        {
                            RockScoreboard.initializingDucks = true;
                            Duck duck = new RockThrowDuck(num4 - num9 * 12, ypos - 16f, profile2);
                            duck.forceMindControl = true;
                            duck.planeOfExistence = index;
                            duck.ignoreGhosting = true;
                            Level.Add(duck);
                            RockScoreboard.initializingDucks = false;
                            duck.depth = depth;
                            duck.z = num6;
                            duck.derpMindControl = false;
                            DuckAI duckAi = new DuckAI(profile2.inputProfile);
                            if (Network.isActive && profile2.connection != DuckNetwork.localConnection)
                                duckAi._manualQuack = GetNetInput((sbyte)profile2.networkIndex);
                            duck.mindControl = duckAi;
                            duck.isRockThrowDuck = true;
                            duck.connection = DuckNetwork.localConnection;
                            _slots[_slots.Count - 1].subDucks.Add(duck);
                            _slots[_slots.Count - 1].subAIs.Add(duckAi);
                        }
                        ++num9;
                    }
                    _slots[_slots.Count - 1].slotIndex = num2;
                    _slots[_slots.Count - 1].startX = num4;
                    ++num2;
                }
                for (int index = 0; index < DG.MaxPlayers; ++index)
                {
                    Block block = new Block(-50f, 0f, 1200f, 32f)
                    {
                        planeOfExistence = (byte)index
                    };
                    Level.Add(block);
                }
                if (!_tie && num3 > 0)
                    _matchOver = true;
                if (_tie)
                    GameMode.showdown = true;
            }
            else if (_mode == ScoreBoardMode.ShowWinner)
            {
                Level.core.gameFinished = true;
                PurpleBlock.Reset();
                Level.core.gameInProgress = false;
                if (Teams.active.Count > 1 && !_afterHighlights)
                {
                    Global.data.matchesPlayed += 1;
                    Global.WinMatch(Teams.winning[0]);
                    if (Network.isActive)
                    {
                        foreach (Profile activeProfile in Teams.winning[0].activeProfiles)
                        {
                            if (activeProfile.connection == DuckNetwork.localConnection)
                            {
                                DuckNetwork.GiveXP("Won Match", 0, 150);
                                break;
                            }
                        }
                        if (DuckNetwork.localProfile != null && DuckNetwork.localProfile.slotType == SlotType.Spectator && DuckNetwork.profiles.Where<Profile>(x => x.connection == DuckNetwork.localConnection).Count<Profile>() == 1 && DuckNetwork._xpEarned.Count == 0)
                            DuckNetwork.GiveXP("Observer Bonus", 0, 50);
                    }
                    DuckNetwork.finishedMatch = true;
                    if (GameMode.winsPerSet > (int)Global.data.longestMatchPlayed)
                        Global.data.longestMatchPlayed.valueInt = GameMode.winsPerSet;
                }
                _intermissionSlide = 0f;
                teamList1.Sort((a, b) =>
               {
                   if (a.score == b.score)
                       return 0;
                   return a.score >= b.score ? -1 : 1;
               });
                float num10 = (float)(160.0 - teamList1.Count * 42 / 2 + 21.0);
                if (smallMode)
                    num10 = (float)(160.0 - teamList1.Count * 24 / 2 + 12.0);
                foreach (Team team in Teams.all)
                    team.prevScoreboardScore = 0;
                List<List<Team>> source = new List<List<Team>>();
                foreach (Team team in teamList1)
                {
                    int score = team.score;
                    bool flag = false;
                    for (int index = 0; index < source.Count; ++index)
                    {
                        if (source[index][0].score < score)
                        {
                            source.Insert(index, new List<Team>());
                            source[index].Add(team);
                            flag = true;
                            break;
                        }
                        if (source[index][0].score == score)
                        {
                            source[index].Add(team);
                            flag = true;
                            break;
                        }
                    }
                    if (!flag)
                    {
                        source.Add(new List<Team>());
                        source.Last<List<Team>>().Add(team);
                    }
                }
                _winningTeam = teamList1[0];
                controlMessage = 1;
                _state = ScoreBoardState.None;
                Crowd.mood = Mood.Dead;
                bool flag1 = false;
                if (!_afterHighlights)
                {
                    if (Network.isServer)
                    {
                        Level.Add(new FloorWindow(10f, -5f));
                        Level.Add(new Trombone(10f, -15f));
                        Level.Add(new Saxaphone(14f, -15f));
                        Level.Add(new Trumpet(6f, -15f));
                        Level.Add(new Trumpet(8f, -15f));
                    }
                    if (Network.isActive)
                        ++Global.data.onlineMatches.valueInt;
                    int place = 0;
                    int num11 = 0;
                    foreach (List<Team> teamList2 in source)
                    {
                        foreach (Team team in teamList2)
                        {
                            Level.Add(new Pedestal(num10 + num11 * (smallMode ? 24 : 42), 150f, team, place, smallMode));
                            ++num11;
                        }
                        ++place;
                    }
                    if (_winningTeam.activeProfiles.Count > 1)
                        ++_winningTeam.wins;
                    else
                        ++_winningTeam.activeProfiles[0].wins;
                    foreach (Profile activeProfile in _winningTeam.activeProfiles)
                    {
                        ++activeProfile.stats.trophiesWon;
                        activeProfile.stats.trophiesSinceLastWin = activeProfile.stats.trophiesSinceLastWinCounter;
                        activeProfile.stats.trophiesSinceLastWinCounter = 0;
                        if ((!Network.isActive || activeProfile.connection == DuckNetwork.localConnection) && !flag1)
                        {
                            flag1 = true;
                            if (Network.isActive)
                                ++Global.data.onlineWins.valueInt;
                            if (activeProfile.team.name == "SWACK")
                                ++Global.data.winsAsSwack.valueInt;
                            if (activeProfile.team.isHair)
                                ++Global.data.winsAsHair.valueInt;
                        }
                        if (!Network.isActive && activeProfile.team.name == "SWACK")
                            ++Global.data.winsAsSwack.valueInt;
                    }
                    foreach (Team team in teamList1)
                    {
                        foreach (Profile activeProfile in team.activeProfiles)
                        {
                            ++activeProfile.stats.trophiesSinceLastWinCounter;
                            ++activeProfile.stats.gamesPlayed;
                        }
                    }
                    Main.lastLevel = "";
                }
            }
            _bottomRight = new Vec2(1000f, 1000f);
            lowestPoint = 1000f;
            _scoreBoard = new GinormoBoard(300f, -320f, _mode == ScoreBoardMode.ShowScores ? BoardMode.Points : BoardMode.Wins, teamList1.Count > 4)
            {
                z = -130f
            };
            Level.Add(_scoreBoard);
            backgroundColor = new Color(0, 0, 0);
            Music.volume = 1f;
            if (_mode != ScoreBoardMode.ShowWinner && !_afterHighlights)
                Music.Play("SportsTime");
            cameraY = 0f;
            Sprite s1;
            switch (RockWeather.weather)
            {
                case Weather.Snowing:
                    s1 = new Sprite("fieldNoiseSnow");
                    break;
                case Weather.Raining:
                    s1 = new Sprite("fieldNoiseRain");
                    break;
                default:
                    s1 = new Sprite("fieldNoise");
                    break;
            }
            s1.scale = new Vec2(4f, 4f);
            s1.depth = (Depth)0.5f;
            s1.y -= 16f;
            _field.AddSprite(s1);
            Sprite s2 = new Sprite("fieldWall")
            {
                scale = new Vec2(4f, 4f),
                depth = (Depth)0.5f
            };
            s2.y -= 16f;
            _wall = new WallLayer("FIELDWALL", 80);
            if (RockScoreboard.wallMode)
                _wall.AddWallSprite(s2);
            Layer.Add(_wall);
            _fieldForeground = new FieldBackground("FIELDFOREGROUND", 80)
            {
                fieldHeight = -13f
            };
            Layer.Add(_fieldForeground);
            _fieldForeground2 = new FieldBackground("FIELDFOREGROUND2", 70)
            {
                fieldHeight = -15f
            };
            Layer.Add(_fieldForeground2);
            if (_mode != ScoreBoardMode.ShowWinner)
            {
                Sprite s3 = new Sprite("rockThrow/chairSeat");
                s3.CenterOrigin();
                s3.x = 300f;
                s3.y = 20f;
                s3.scale = new Vec2(1.2f, 1.2f);
                _fieldForeground.AddSprite(s3);
                Sprite s4 = new Sprite("rockThrow/tableTop");
                s4.CenterOrigin();
                s4.x = 450f;
                s4.y = 14f;
                s4.scale = new Vec2(1.2f, 1.4f);
                _fieldForeground2.AddSprite(s4);
                int num12 = -95;
                Sprite spr1 = new Sprite("rockThrow/chairBottomBack");
                SpriteThing spriteThing3 = new SpriteThing(300f, -10f, spr1)
                {
                    center = new Vec2(spr1.w / 2, spr1.h / 2),
                    z = 106 + num12,
                    depth = (Depth)0.5f,
                    layer = Layer.Background
                };
                Level.Add(spriteThing3);
                Sprite spr2 = new Sprite("rockThrow/chairBottom");
                SpriteThing spriteThing4 = new SpriteThing(300f, -6f, spr2)
                {
                    center = new Vec2(spr2.w / 2, spr2.h / 2),
                    z = 120 + num12,
                    depth = (Depth)0.8f,
                    layer = Layer.Background
                };
                Level.Add(spriteThing4);
                Sprite spr3 = new Sprite("rockThrow/chairFront");
                SpriteThing spriteThing5 = new SpriteThing(300f, -9f, spr3)
                {
                    center = new Vec2(spr3.w / 2, spr3.h / 2),
                    z = 122 + num12,
                    depth = (Depth)0.9f,
                    layer = Layer.Background
                };
                Level.Add(spriteThing5);
                Sprite spr4 = new Sprite("rockThrow/tableBottomBack");
                SpriteThing spriteThing6 = new SpriteThing(450f, -7f, spr4)
                {
                    center = new Vec2(spr4.w / 2, spr4.h / 2),
                    z = 106 + num12,
                    depth = (Depth)0.5f,
                    layer = Layer.Background
                };
                Level.Add(spriteThing6);
                Sprite spr5 = new Sprite("rockThrow/tableBottom");
                SpriteThing spriteThing7 = new SpriteThing(450f, -7f, spr5)
                {
                    center = new Vec2(spr5.w / 2, spr5.h / 2),
                    z = 120 + num12,
                    depth = (Depth)0.8f,
                    layer = Layer.Background
                };
                Level.Add(spriteThing7);
                Sprite spr6 = new Sprite("rockThrow/keg");
                SpriteThing spriteThing8 = new SpriteThing(460f, -24f, spr6)
                {
                    center = new Vec2(spr6.w / 2, spr6.h / 2),
                    z = 120 + num12 - 4,
                    depth = -0.4f,
                    layer = Layer.Game
                };
                Level.Add(spriteThing8);
                Sprite spr7 = new Sprite("rockThrow/cup");
                SpriteThing spriteThing9 = new SpriteThing(445f, -21f, spr7)
                {
                    center = new Vec2(spr7.w / 2, spr7.h / 2),
                    z = 120 + num12 - 6,
                    depth = -0.5f,
                    layer = Layer.Game
                };
                Level.Add(spriteThing9);
                Sprite spr8 = new Sprite("rockThrow/cup");
                SpriteThing spriteThing10 = new SpriteThing(437f, -20f, spr8)
                {
                    center = new Vec2(spr8.w / 2, spr8.h / 2),
                    z = 120 + num12,
                    depth = -0.3f,
                    layer = Layer.Game
                };
                Level.Add(spriteThing10);
                Sprite spr9 = new Sprite("rockThrow/cup");
                SpriteThing spriteThing11 = new SpriteThing(472f, -20f, spr9)
                {
                    center = new Vec2(spr9.w / 2, spr9.h / 2),
                    z = 120 + num12 - 7,
                    depth = -0.5f,
                    layer = Layer.Game,
                    angleDegrees = 80f
                };
                Level.Add(spriteThing11);
            }
            for (int index = 0; index < 3; ++index)
            {
                DistanceMarker distanceMarker = new DistanceMarker(230 + index * 175, -25f, (int)Math.Round(index * GameMode.winsPerSet / 2.0))
                {
                    z = 0f,
                    depth = (Depth)0.34f,
                    layer = Layer.Background
                };
                Level.Add(distanceMarker);
            }
            Sprite spr = RockWeather.weather != Weather.Snowing ? new Sprite("bleacherBack") : new Sprite("bleacherBackSnow");
            for (int index = 0; index < 24; ++index)
            {
                SpriteThing spriteThing = new SpriteThing(100 + index * (spr.w + 13), spr.h + 15, spr)
                {
                    center = new Vec2(spr.w / 2, spr.h - 1)
                };
                spriteThing.collisionOffset = new Vec2(spriteThing.collisionOffset.x, -spr.h);
                spriteThing.z = 0f;
                spriteThing.depth = (Depth)0.33f;
                spriteThing.layer = Layer.Background;
                Level.Add(spriteThing);
            }
            SpriteThing spriteThing12 = new SpriteThing(600f, 0f, new Sprite("blackSquare"))
            {
                z = -90f,
                centery = 7f,
                depth = (Depth)0.1f,
                layer = Layer.Background,
                xscale = 100f,
                yscale = 7f
            };
            Level.Add(spriteThing12);
            _weather.Update();
        }

        public Vec2 sunPos => sunThing.position;

        public Layer sunLayer => _sunLayer;

        public static bool drawingSunTarget => RockScoreboard._drawingSunTarget;

        public static bool drawingLighting => RockScoreboard._drawingLighting;

        public static bool drawingNormalTarget => RockScoreboard._drawingNormalTarget;

        public void DoRender()
        {
            Color backgroundColor = this.backgroundColor;
            if (NetworkDebugger.enabled)
            {
                RockScoreboard._drawingSunTarget = true;
                Layer.Game.camera.width = 320f;
                Layer.Game.camera.height = 180f;
                _field.fade = Layer.Game.fade;
                _fieldForeground.fade = Layer.Game.fade;
                _wall.fade = Layer.Game.fade;
                _fieldForeground2.fade = Layer.Game.fade;
                this.backgroundColor = backgroundColor;
                MonoMain.RenderGame(_screenTarget);
                RockScoreboard._drawingSunTarget = false;
            }
            else
            {
                this.backgroundColor = Color.Black;
                RockScoreboard._drawingSunTarget = true;
                float fade1 = Layer.HUD.fade;
                float fade2 = Layer.Console.fade;
                float fade3 = Layer.Game.fade;
                float fade4 = Layer.Background.fade;
                float fade5 = _field.fade;
                Layer.Game.fade = 0f;
                Layer.Background.fade = 0f;
                Layer.Foreground.fade = 0f;
                _field.fade = 0f;
                _fieldForeground.fade = 0f;
                _wall.fade = 0f;
                _fieldForeground2.fade = 0f;
                Vec3 colorMul1 = Layer.Game.colorMul;
                Vec3 colorMul2 = Layer.Background.colorMul;
                Layer.Game.colorMul = Vec3.One;
                Layer.Background.colorMul = Vec3.One;
                Layer.HUD.fade = 0f;
                Layer.Console.fade = 0f;
                fieldMulColor = Vec3.One;
                Vec3 colorAdd = Layer.Game.colorAdd;
                Layer.Game.colorAdd = Vec3.Zero;
                Layer.Background.colorAdd = Vec3.Zero;
                fieldAddColor = Vec3.Zero;
                Layer.blurry = true;
                sunThing.alpha = RockWeather.sunOpacity;
                rainbowThing2.alpha = 0f;
                RockScoreboard._drawingLighting = true;
                MonoMain.RenderGame(_sunshineTarget);
                RockScoreboard._drawingLighting = false;
                if (_sunshineMaterialBare == null)
                    _sunshineMaterialBare = new MaterialSunshineBare();
                Vec2 sunPos = this.sunPos;
                Vec3 source = new Vec3(sunPos.x, -9999f, sunPos.y);
                Viewport viewport1 = new Viewport(0, 0, (int)Layer.HUD.width, (int)Layer.HUD.height);
                Vec3 vec3 = (Vec3)viewport1.Project((Vector3)source, (Microsoft.Xna.Framework.Matrix)sunLayer.projection, (Microsoft.Xna.Framework.Matrix)sunLayer.view, (Microsoft.Xna.Framework.Matrix)Matrix.Identity);
                vec3.y -= 256f;
                vec3.x /= viewport1.Width;
                vec3.y /= viewport1.Height;
                _sunshineMaterialBare.effect.effect.Parameters["lightPos"].SetValue((Vector2)new Vec2(vec3.x, vec3.y));
                _sunshineMaterialBare.effect.effect.Parameters["weight"].SetValue(1f);
                _sunshineMaterialBare.effect.effect.Parameters["density"].SetValue(0.4f);
                _sunshineMaterialBare.effect.effect.Parameters["decay"].SetValue(0.68f + RockWeather.sunGlow);
                _sunshineMaterialBare.effect.effect.Parameters["exposure"].SetValue(1f);
                Viewport viewport2 = DuckGame.Graphics.viewport;
                DuckGame.Graphics.SetRenderTarget(_pixelTarget);
                DuckGame.Graphics.viewport = new Viewport(0, 0, _pixelTarget.width, _pixelTarget.height);
                DuckGame.Graphics.screen.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, camera.getMatrix());
                DuckGame.Graphics.material = _sunshineMaterialBare;
                float num = _pixelTarget.width * 2 / (float)_sunshineTarget.width;
                DuckGame.Graphics.Draw(_sunshineTarget, Vec2.Zero, new Rectangle?(), Color.White, 0f, Vec2.Zero, new Vec2(num), SpriteEffects.None);
                DuckGame.Graphics.material = null;
                DuckGame.Graphics.screen.End();
                DuckGame.Graphics.SetRenderTarget(null);
                DuckGame.Graphics.viewport = viewport2;
                Layer.blurry = false;
                Layer.HUD.fade = fade1;
                Layer.Console.fade = fade2;
                Layer.Game.fade = fade3;
                Layer.Foreground.fade = fade3;
                Layer.Background.fade = fade4;
                _field.fade = fade5;
                _fieldForeground.fade = fade5;
                _fieldForeground2.fade = fade5;
                _wall.fade = fade5;
                Layer.Game.colorMul = colorMul1;
                Layer.Background.colorMul = colorMul2;
                fieldMulColor = colorMul2;
                Layer.Game.colorAdd = colorAdd;
                Layer.Background.colorAdd = colorAdd;
                fieldAddColor = colorAdd;
                RockScoreboard._drawingSunTarget = false;
                sunThing.x = (float)(290.0 + RockWeather.sunPos.x * 8000.0);
                sunThing.y = (float)(10000.0 - RockWeather.sunPos.y * 8000.0);
                rainbowThing.y = rainbowThing2.y = (float)(2000.0 + _fieldScroll * 12.0);
                rainbowThing.x = rainbowThing2.x = (float)(-_field.scroll * 15.0 + 6800.0);
                rainbowThing.alpha = _weather.rainbowLight;
                rainbowThing2.alpha = _weather.rainbowLight2;
                rainbowThing.visible = rainbowThing.alpha > 0.00999999977648258;
                rainbowThing2.visible = rainbowThing2.alpha > 0.00999999977648258;
                RockScoreboard._drawingSunTarget = true;
                Layer.Game.camera.width = 320f;
                Layer.Game.camera.height = 180f;
                _field.fade = Layer.Game.fade;
                _fieldForeground.fade = Layer.Game.fade;
                _fieldForeground2.fade = Layer.Game.fade;
                _wall.fade = Layer.Game.fade;
                this.backgroundColor = backgroundColor;
                RockScoreboard._drawingNormalTarget = true;
                MonoMain.RenderGame(_screenTarget);
                RockScoreboard._drawingNormalTarget = false;
                RockScoreboard._drawingSunTarget = false;
            }
        }

        public override void Update()
        {
            if (Network.isActive)
            {
                if (_netCountdown == null)
                {
                    if (Network.isServer)
                    {
                        _netCountdown = !DuckNetwork.isDedicatedServer ? new ContinueCountdown(_mode == ScoreBoardMode.ShowScores ? 5f : 15f) : new ContinueCountdown(_mode == ScoreBoardMode.ShowScores ? 4f : (_afterHighlights ? 5f : 10f));
                        Level.Add(_netCountdown);
                    }
                    else
                    {
                        IEnumerable<Thing> thing = Level.current.things[typeof(ContinueCountdown)];
                        if (thing.Count<Thing>() > 0)
                            _netCountdown = thing.ElementAt<Thing>(0) as ContinueCountdown;
                    }
                }
                else if (_continueHUD != null)
                {
                    if (Network.isServer)
                    {
                        _continueHUD.text = "@START@CONTINUE(" + ((int)Math.Ceiling(_netCountdown.timer)).ToString() + ")";
                        _netCountdown.UpdateTimer();
                    }
                    else
                        _continueHUD.text = "WAITING(" + ((int)Math.Ceiling(_netCountdown.timer)).ToString() + ")";
                }
                if (Network.isServer && netCountdown != null && !netCountdown.isServerForObject)
                {
                    int controlMessage = this.controlMessage;
                    if (controlMessage > 0)
                    {
                        this.controlMessage = -1;
                        this.controlMessage = controlMessage;
                    }
                    Thing.Fondle(netCountdown, DuckNetwork.localConnection);
                }
            }
            bool isServer = Network.isServer;
            Network.isServer = true;
            backgroundColor = new Color(139, 204, 248) * _backgroundFade;
            Layer.Game.fade = _backgroundFade;
            Layer.Background.fade = _backgroundFade;
            _backgroundFade = Lerp.Float(_backgroundFade, 1f, 0.02f);
            _field.rise = _fieldScroll;
            _fieldForeground.rise = _fieldScroll;
            _fieldForeground2.rise = _fieldScroll;
            _wall.rise = _fieldScroll;
            _bottomRight = new Vec2(1000f, 1000f);
            lowestPoint = 1000f;
            bool flag1 = false;
            _field.scroll = Lerp.Float(_field.scroll, _desiredScroll, 6f);
            if (_field.scroll < 297.0)
            {
                _field.scroll = 0f;
                flag1 = true;
            }
            if (_field.scroll < 302.0)
                _field.scroll = 302f;
            _fieldForeground.scroll = _field.scroll;
            _fieldForeground2.scroll = _field.scroll;
            _wall.scroll = _field.scroll;
            if (_state != ScoreBoardState.Transition)
            {
                if (_state == ScoreBoardState.Intro)
                {
                    if (_animWait > 0.0)
                    {
                        _animWait -= 0.021f;
                    }
                    else
                    {
                        Crowd.mood = Mood.Silent;
                        _intermissionSlide = Lerp.FloatSmooth(_intermissionSlide, 2.1f, 0.1f, 1.05f);
                        if (_intermissionSlide > 2.08999991416931)
                        {
                            controlMessage = 0;
                            Vote.OpenVoting("", "", false);
                            _state = ScoreBoardState.ThrowRocks;
                        }
                    }
                }
                else if (_state == ScoreBoardState.MatchOver)
                {
                    if (_highestSlot.duck.position.x < _highestSlot.rock.x - 16.0)
                    {
                        _highestSlot.ai.Release("LEFT");
                        _highestSlot.ai.Press("RIGHT");
                    }
                    if (_highestSlot.duck.position.x > _highestSlot.rock.x + 16.0)
                    {
                        _highestSlot.ai.Release("RIGHT");
                        _highestSlot.ai.Press("LEFT");
                    }
                    if (_highestSlot.duck.position.x > _highestSlot.rock.position.x - 16.0 || _highestSlot.duck.profile.team == null)
                        _focusRock = true;
                    for (int index = 0; index < _highestSlot.subAIs.Count; ++index)
                    {
                        DuckAI subAi = _highestSlot.subAIs[index];
                        Duck subDuck = _highestSlot.subDucks[index];
                        if (subDuck.position.x < _highestSlot.rock.x - 16.0)
                        {
                            subAi.Release("LEFT");
                            subAi.Press("RIGHT");
                        }
                        if (subDuck.position.x > _highestSlot.rock.x + 16.0)
                        {
                            subAi.Release("RIGHT");
                            subAi.Press("LEFT");
                        }
                    }
                    if (_focusRock)
                    {
                        _highestSlot.ai.Release("JUMP");
                        if (Rando.Float(1f) > 0.980000019073486)
                            _highestSlot.ai.Press("JUMP");
                        for (int index = 0; index < _highestSlot.subAIs.Count; ++index)
                        {
                            DuckAI subAi = _highestSlot.subAIs[index];
                            Duck subDuck = _highestSlot.subDucks[index];
                            subAi.Release("JUMP");
                            if (Rando.Float(1f) > 0.980000019073486)
                                subAi.Press("JUMP");
                        }
                        if (!_droppedConfetti)
                        {
                            _desiredScroll = _highestSlot.duck.position.x;
                            if (_desiredScroll >= _highestSlot.rock.position.x)
                            {
                                _desiredScroll = _highestSlot.rock.position.x;
                                Crowd.mood = Mood.Extatic;
                                _droppedConfetti = true;
                                for (int index = 0; index < 64; ++index)
                                    Level.Add(new Confetti(_confettiDrop + Rando.Float(-32f, 32f), _highestSlot.rock.y - 220f - Rando.Float(50f)));
                            }
                        }
                        if (Network.isServer && (Input.Pressed("START") || _netCountdown != null && _netCountdown.timer <= 0.0))
                            _finished = true;
                        _winnerWait -= 0.007f;
                        if (_winnerWait < 0.0)
                            _finished = true;
                    }
                    else
                    {
                        _desiredScroll = _highestSlot.duck.position.x;
                        Crowd.mood = Mood.Excited;
                    }
                }
            }
            if (_state == ScoreBoardState.ThrowRocks)
            {
                if (!_shiftCamera)
                    _controlSlide = Lerp.FloatSmooth(_controlSlide, 1f, 0.1f, 1.05f);
                bool flag2 = true;
                foreach (Slot3D slot3D in _slots)
                {
                    slot3D.follow = false;
                    if (flag2)
                    {
                        if (slot3D.state != RockThrow.Finished)
                        {
                            flag2 = false;
                            slot3D.follow = true;
                        }
                        else if (slot3D == _slots[_slots.Count - 1])
                        {
                            if (_matchOver)
                                _skipFade = true;
                            else
                                _state = ScoreBoardState.ShowBoard;
                        }
                        if (slot3D.state == RockThrow.Idle)
                            slot3D.state = RockThrow.PickUpRock;
                        if (slot3D.state == RockThrow.PickUpRock)
                        {
                            if (slot3D.duck.position.x < slot3D.rock.position.x)
                            {
                                slot3D.ai.Press("RIGHT");
                            }
                            else
                            {
                                slot3D.state = RockThrow.ThrowRock;
                                slot3D.duck.position.x = slot3D.rock.position.x;
                                slot3D.duck.hSpeed = 0f;
                                _throwWait = !TeamSelect2.eightPlayersActive ? 0.9f : 0.5f;
                            }
                        }
                        if (slot3D.state == RockThrow.ThrowRock)
                        {
                            if (_throwWait > 0.0)
                            {
                                _throwWait -= 0.08f;
                                slot3D.ai.Release("RIGHT");
                                slot3D.duck.GiveHoldable(slot3D.rock);
                                _afterThrowWait = !TeamSelect2.eightPlayersActive ? 0.7f : 0.5f;
                            }
                            else
                            {
                                if (slot3D.duck.holdObject != null)
                                {
                                    if (slot3D.duck.profile.team == null)
                                    {
                                        slot3D.duck.Kill(new DTDisconnect(slot3D.duck));
                                    }
                                    else
                                    {
                                        this._misfire = false;
                                        slot3D.duck.ThrowItem(true);
                                        float num = slot3D.duck.profile.team.rockScore;
                                        int num2 = GameMode.winsPerSet * 2;
                                        if (num > (num2 - 2))
                                        {
                                            num = (num2 - 2) + Math.Min((float)(slot3D.duck.profile.team.rockScore - GameMode.winsPerSet * 2) / 16f, 1f);
                                        }
                                        float num3 = slot3D.startX + 30f + num / (float)num2 * this._fieldWidth - slot3D.rock.x;
                                        slot3D.rock.vSpeed = -2f - Maths.Clamp(num3 / 300f, 0f, 1f) * 4f;
                                        float num4 = Math.Abs(2f * slot3D.rock.vSpeed) / slot3D.rock.currentGravity;
                                        float currentFriction = slot3D.rock.currentFriction;
                                        float num5 = num3 / num4;
                                        slot3D.rock.frictionMult = 0f;
                                        slot3D.rock.grounded = false;
                                        slot3D.rock.hMax = 100f;
                                        slot3D.rock.vMax = 100f;
                                        if (slot3D.duck.profile.team.rockScore == slot3D.duck.profile.team.prevScoreboardScore)
                                        {
                                            num5 = 0.3f;
                                            slot3D.rock.vSpeed = -0.6f;
                                            this._misfire = true;
                                        }
                                        slot3D.rock.hSpeed = num5 * 0.88f;
                                        if (RockScoreboard.wallMode && slot3D.duck.profile.team.rockScore > GameMode.winsPerSet)
                                        {
                                            slot3D.rock.hSpeed += 1f;
                                        }
                                    }
                                }
                                if (slot3D.rock.grounded)
                                {
                                    if (slot3D.duck.profile.team == null)
                                        slot3D.duck.Kill(new DTDisconnect(slot3D.duck));
                                    float num6 = 0.015f;
                                    if (slot3D.duck.profile.team != null)
                                    {
                                        int num7 = slot3D.duck.profile.team.rockScore - slot3D.duck.profile.team.prevScoreboardScore;
                                        if (num7 == 0)
                                            Crowd.mood = Mood.Dead;
                                        else if (num7 < 2)
                                            Crowd.mood = Mood.Calm;
                                        else if (num7 < 5)
                                        {
                                            Crowd.mood = Mood.Excited;
                                            num6 = 0.013f;
                                        }
                                        else if (num7 < 99)
                                        {
                                            Crowd.mood = Mood.Extatic;
                                            num6 = 0.01f;
                                        }
                                    }
                                    int winsPerSet = GameMode.winsPerSet;
                                    if (slot3D.rock.frictionMult == 0.0)
                                    {
                                        Sprite s;
                                        switch (RockWeather.weather)
                                        {
                                            case Weather.Snowing:
                                                s = new Sprite("rockThrow/rockSmudgeSnow");
                                                break;
                                            case Weather.Raining:
                                                s = new Sprite("rockThrow/rockSmudgeMud");
                                                break;
                                            default:
                                                s = new Sprite("rockThrow/rockSmudge");
                                                break;
                                        }
                                        s.position = new Vec2(slot3D.rock.x - 12f, slot3D.rock.z - 10f);
                                        s.depth = (Depth)0.9f;
                                        s.xscale = 0.8f;
                                        s.yscale = 1.4f;
                                        s.alpha = 0.9f;
                                        _field.AddSprite(s);
                                    }
                                    ++slot3D.slideWait;
                                    if (slot3D.slideWait > 3 && slot3D.rock.hSpeed > 0.0)
                                    {
                                        Sprite s;
                                        switch (RockWeather.weather)
                                        {
                                            case Weather.Snowing:
                                                s = new Sprite("rockThrow/rockSmearSnow");
                                                break;
                                            case Weather.Raining:
                                                s = new Sprite("rockThrow/rockSmearMud");
                                                break;
                                            default:
                                                s = new Sprite("rockThrow/rockSmear");
                                                break;
                                        }
                                        s.position = new Vec2(slot3D.rock.x - 5f, slot3D.rock.z - 10f);
                                        s.depth = (Depth)0.9f;
                                        s.xscale = 0.6f;
                                        s.yscale = 1.4f;
                                        s.alpha = 0.9f;
                                        slot3D.slideWait = 0;
                                        _field.AddSprite(s);
                                    }
                                    slot3D.rock.frictionMult = 4f;
                                    _afterThrowWait -= num6;
                                    if (_afterThrowWait < 0.400000005960464)
                                    {
                                        slot3D.state = RockThrow.ShowScore;
                                        SFX.Play("scoreDing");
                                        if (slot3D.duck.profile.team != null && RockScoreboard.wallMode && slot3D.duck.profile.team.rockScore > GameMode.winsPerSet)
                                            slot3D.duck.profile.team.rockScore = GameMode.winsPerSet;
                                        _showScoreWait = !TeamSelect2.eightPlayersActive ? 0.6f : 0.5f;
                                        Crowd.ThrowHats(slot3D.duck.profile);
                                        if (!slot3D.showScore)
                                        {
                                            slot3D.showScore = true;
                                            PointBoard pointBoard = new PointBoard(slot3D.rock, slot3D.duck.profile.team)
                                            {
                                                depth = slot3D.rock.depth + 1,
                                                z = slot3D.rock.z
                                            };
                                            Level.Add(pointBoard);
                                        }
                                    }
                                }
                                else if (slot3D.duck.profile.team == null)
                                {
                                    slot3D.duck.Kill(new DTDisconnect(slot3D.duck));
                                }
                                else
                                {
                                    int rockScore = slot3D.duck.profile.team.rockScore;
                                    int num = GameMode.winsPerSet * 2;
                                    if (!_misfire && slot3D.rock.x > slot3D.startX + 30.0 + rockScore / num * _fieldWidth)
                                        slot3D.rock.x = (float)(slot3D.startX + 30.0 + rockScore / num * _fieldWidth);
                                }
                            }
                        }
                        if (slot3D.state == RockThrow.ShowScore)
                        {
                            _showScoreWait -= 0.016f;
                            if (_showScoreWait < 0.0)
                            {
                                if (slot3D.duck.profile.team == null)
                                {
                                    slot3D.state = RockThrow.Finished;
                                    _backWait = !TeamSelect2.eightPlayersActive ? 0.9f : 0.5f;
                                }
                                else
                                    slot3D.state = RockThrow.RunBack;
                            }
                        }
                        if (slot3D.state == RockThrow.RunBack)
                        {
                            if (slot3D == _slots[_slots.Count - 1])
                                slot3D.follow = false;
                            if (slot3D.duck.position.x > slot3D.startX)
                            {
                                slot3D.ai.Press("LEFT");
                            }
                            else
                            {
                                slot3D.duck.position.x = slot3D.startX;
                                slot3D.duck.hSpeed = 0f;
                                slot3D.duck.offDir = 1;
                                slot3D.ai.Release("LEFT");
                                _backWait -= 0.05f;
                                Crowd.mood = Mood.Silent;
                                if (_backWait < 0.0 && (flag1 || slot3D == _slots[_slots.Count - 1]))
                                {
                                    slot3D.state = RockThrow.Finished;
                                    _backWait = !TeamSelect2.eightPlayersActive ? 0.9f : 0.5f;
                                }
                            }
                        }
                    }
                    if (slot3D.follow)
                        _desiredScroll = slot3D.state == RockThrow.ThrowRock || slot3D.state == RockThrow.ShowScore ? slot3D.rock.position.x : slot3D.duck.position.x;
                    if (Input.Pressed("START"))
                    {
                        foreach (Profile profile in Profiles.active)
                        {
                            if (profile.inputProfile != null && profile.inputProfile.Pressed("START") && (!Network.isActive || profile.connection == DuckNetwork.localConnection))
                            {
                                Vote.RegisterVote(profile, VoteType.Skip);
                                if (Network.isActive)
                                    Send.Message(new NMVoteToSkip(profile));
                            }
                        }
                    }
                    if (Vote.Passed(VoteType.Skip))
                        _skipFade = true;
                }
            }
            else
                Vote.CloseVoting();
            if (_state == ScoreBoardState.MatchOver)
            {
                Network.isServer = isServer;
                _controlSlide = Lerp.FloatSmooth(_controlSlide, controlMessage == 1 ? 1f : 0f, 0.1f, 1.05f);
                if (_controlSlide < 0.00999999977648258)
                    controlMessage = -1;
            }
            if (_state == ScoreBoardState.ShowBoard)
            {
                Network.isServer = isServer;
                _shiftCamera = true;
                _controlSlide = Lerp.FloatSmooth(_controlSlide, controlMessage == 1 ? 1f : 0f, 0.1f, 1.05f);
                if (_controlSlide < 0.00999999977648258)
                    controlMessage = 1;
            }
            if (_shiftCamera)
            {
                if (_state == ScoreBoardState.ThrowRocks)
                    _controlSlide = Lerp.FloatSmooth(_controlSlide, 0f, 0.1f, 1.05f);
                _desiredScroll = -79f;
                if (_fieldScroll < 220.0)
                {
                    _fieldScroll += 4f;
                }
                else
                {
                    if (_state == ScoreBoardState.ThrowRocks)
                        _state = ScoreBoardState.ShowBoard;
                    if (!_scoreBoard.activated)
                        _scoreBoard.Activate();
                    if (!_finished && isServer && (Input.Pressed("START") || _netCountdown != null && _netCountdown.timer <= 0.0))
                        _finished = true;
                    Crowd.mood = Mood.Dead;
                }
            }
            if (_skipFade)
            {
                Network.isServer = isServer;
                controlMessage = -1;
                DuckGame.Graphics.fade = Lerp.Float(DuckGame.Graphics.fade, 0f, 0.02f);
                if (DuckGame.Graphics.fade < 0.00999999977648258)
                {
                    _skipFade = false;
                    if (_mode == ScoreBoardMode.ShowScores)
                    {
                        if (!_matchOver)
                        {
                            _state = ScoreBoardState.ShowBoard;
                            _fieldScroll = 220f;
                            _desiredScroll = -79f;
                            _field.scroll = _desiredScroll;
                        }
                        else
                        {
                            _state = ScoreBoardState.MatchOver;
                            _field.scroll = 0f;
                            foreach (Slot3D slot in _slots)
                            {
                                if (slot.duck.profile.team != null)
                                {
                                    float num8 = slot.duck.profile.team.rockScore;
                                    int num9 = GameMode.winsPerSet * 2;
                                    if (num8 > num9 - 2)
                                        num8 = num9 - 2 + Math.Min((slot.duck.profile.team.rockScore - GameMode.winsPerSet * 2) / 16f, 1f);
                                    slot.rock.x = (float)(slot.startX + 30.0 + num8 / num9 * _fieldWidth);
                                    if (RockScoreboard.wallMode && slot.duck.profile.team.rockScore >= GameMode.winsPerSet)
                                        slot.rock.x -= 10f;
                                }
                                slot.rock.hSpeed = 0f;
                            }
                        }
                    }
                    else if (_afterHighlights)
                    {
                        _fieldScroll = 220f;
                        _desiredScroll = -79f;
                        _field.scroll = _desiredScroll;
                        _scoreBoard.Activate();
                        _viewBoard = true;
                    }
                    else
                        Level.current = !isServer || !Network.isActive ? new HighlightLevel() : new RockScoreboard(RockScoreboard.returnLevel, ScoreBoardMode.ShowWinner, true);
                }
            }
            if (_finished)
            {
                DuckGame.Graphics.fade = Lerp.Float(DuckGame.Graphics.fade, 0f, 0.03f);
                if (DuckGame.Graphics.fade < 0.00999999977648258)
                {
                    foreach (Team team in Teams.all)
                        team.prevScoreboardScore = team.score;
                    if (isServer)
                    {
                        if (_mode == ScoreBoardMode.ShowWinner)
                        {
                            if (_returnToScoreboard)
                            {
                                Level.current = new RockScoreboard(RockScoreboard.returnLevel, ScoreBoardMode.ShowWinner, true);
                            }
                            else
                            {
                                Main.ResetMatchStuff();
                                if (_hatSelect)
                                    Level.current = new TeamSelect2(true);
                                else if (!_quit)
                                {
                                    Music.Stop();
                                    Level.current = RockScoreboard.returnLevel;
                                    DuckGame.Graphics.fade = 1f;
                                }
                                else
                                {
                                    if (Network.isActive)
                                        Network.EndNetworkingSession(new DuckNetErrorInfo(DuckNetError.ControlledDisconnect, "Game Over!"));
                                    Level.current = new TitleScreen();
                                }
                            }
                        }
                        else if (_state != ScoreBoardState.MatchOver)
                        {
                            Music.Stop();
                            Level.current = RockScoreboard.returnLevel;
                            DuckGame.Graphics.fade = 1f;
                        }
                        else
                            Level.current = new RockScoreboard(RockScoreboard.returnLevel, ScoreBoardMode.ShowWinner);
                    }
                }
            }
            else if (!_skipFade && !_finished)
                DuckGame.Graphics.fade = Lerp.Float(DuckGame.Graphics.fade, 1f, 0.03f);
            Network.isServer = isServer;
            if (_mode == ScoreBoardMode.ShowWinner)
            {
                _controlSlide = Lerp.FloatSmooth(_controlSlide, controlMessage == 1 ? 1f : 0f, 0.1f, 1.05f);
                if (_controlSlide < 0.00999999977648258)
                    controlMessage = 1;
                if (_viewBoard)
                {
                    controlMessage = 2;
                    _controlSlide = 1f;
                }
                if (!_scoreBoard.activated)
                {
                    if (isServer && (Input.Pressed("START") || _netCountdown != null && _netCountdown.timer <= 0.0))
                    {
                        if (Network.isActive)
                        {
                            _finished = true;
                            _returnToScoreboard = true;
                        }
                        else
                        {
                            _takePicture = true;
                            HUD.CloseAllCorners();
                        }
                    }
                    if (_takePicture && _flashSkipFrames == 0)
                    {
                        _cameraWait -= 0.01f;
                        if (_cameraWait < 0.600000023841858 && _playedBeeps == 0)
                        {
                            _playedBeeps = 1;
                            SFX.Play("cameraBeep", pitch: -0.5f);
                        }
                        else if (_cameraWait < 0.300000011920929 && _playedBeeps == 1)
                        {
                            _playedBeeps = 2;
                            SFX.Play("cameraBeep", pitch: -0.5f);
                        }
                        if (_cameraWait < 0.0 && !_playedFlash)
                        {
                            _playedFlash = true;
                            SFX.Play("cameraFlash", 0.8f, 1f);
                        }
                        if (_cameraWait < 0.100000001490116)
                        {
                            _cameraFadeVel += 3f / 1000f;
                            if (_cameraWait < 0.0399999991059303)
                                _cameraFadeVel += 0.01f;
                        }
                        DuckGame.Graphics.fadeAdd += _cameraFadeVel;
                        if (DuckGame.Graphics.fadeAdd > 1.0)
                        {
                            int width1 = DuckGame.Graphics.width;
                            int height1 = DuckGame.Graphics.height;
                            if (!RockScoreboard._sunEnabled)
                            {
                                int height2 = DuckGame.Graphics.height / 4 * 3 + 30;
                                DuckGame.Graphics.fadeAdd = 0f;
                                Layer.Background.fade = 0.8f;
                                ++_flashSkipFrames;
                                RockScoreboard.finalImage = new RenderTarget2D(width1, height1);
                                Layer.Game.visible = false;
                                Rectangle scissor = _field.scissor;
                                _field.scissor = new Rectangle(0f, 0f, Resolution.size.x, height2);
                                _field.visible = true;
                                MonoMain.RenderGame(RockScoreboard.finalImage);
                                Layer.Game.visible = true;
                                Color backgroundColor = Level.current.backgroundColor;
                                Level.current.backgroundColor = Color.Transparent;
                                RockScoreboard.finalImage2 = new RenderTarget2D(width1, height1);
                                Layer.allVisible = false;
                                Layer.Game.visible = true;
                                int y = height2 - 5;
                                _field.scissor = new Rectangle(0f, y, width1, height1 - y);
                                _field.visible = true;
                                MonoMain.RenderGame(RockScoreboard.finalImage2);
                                _field.scissor = scissor;
                                Layer.allVisible = true;
                                Level.current.backgroundColor = backgroundColor;
                                _getScreenshot = true;
                                _finalSprite = new Sprite(RockScoreboard.finalImage, 0f, 0f);
                                Stream stream = DuckFile.Create(DuckFile.albumDirectory + "album" + DateTime.Now.ToString("MM-dd-yy H;mm;ss") + ".png");
                                ((Texture2D)_finalSprite.texture.nativeObject).SaveAsPng(stream, width1, height1);
                                stream.Dispose();
                            }
                            else
                            {
                                DuckGame.Graphics.fadeAdd = 0f;
                                Layer.Background.fade = 0.8f;
                                _weather.Update();
                                DoRender();
                                RockScoreboard.finalImage = new RenderTarget2D(width1, height1);
                                RenderFinalImage(RockScoreboard.finalImage, false);
                                _finalSprite = new Sprite(RockScoreboard.finalImage, 0f, 0f);
                                _getScreenshot = true;
                                DuckGame.Graphics.fadeAdd = 1f;
                                int width2 = 320;
                                int height3 = 180;
                                RenderTarget2D image = new RenderTarget2D(width2, height3);
                                RenderFinalImage(image, true);
                                DuckGame.Graphics.fadeAdd = 1f;
                                Stream stream = DuckFile.Create(DuckFile.albumDirectory + DateTime.Now.ToString("MM-dd-yy H;mm") + ".png");
                                (image.nativeObject as Microsoft.Xna.Framework.Graphics.RenderTarget2D).SaveAsPng(stream, width2, height3);
                                stream.Dispose();
                                DoRender();
                            }
                        }
                    }
                    if (_getScreenshot && DuckGame.Graphics.screenCapture == null)
                    {
                        Level.current.simulatePhysics = false;
                        ++_flashSkipFrames;
                        if (_flashSkipFrames > 2)
                            DuckGame.Graphics.fadeAdd = 1f;
                        if (_flashSkipFrames > 20)
                            Level.current = new HighlightLevel();
                    }
                }
                else if (!_finished && isServer)
                {
                    if (Input.Pressed("START") || _netCountdown != null && _netCountdown.timer <= 0.0)
                    {
                        _finished = true;
                        _hatSelect = DuckNetwork.isDedicatedServer;
                    }
                    if (Input.Pressed("CANCEL"))
                    {
                        _finished = true;
                        _quit = true;
                    }
                    if (Input.Pressed("MENU2"))
                    {
                        _finished = true;
                        _hatSelect = true;
                    }
                }
            }
            Network.isServer = isServer;
            base.Update();
        }

        public override void Terminate()
        {
            if (_mode == ScoreBoardMode.ShowWinner)
            {
                foreach (Team team in Teams.all)
                    team.prevScoreboardScore = 0;
            }
            else
            {
                foreach (Team team in Teams.all)
                    team.prevScoreboardScore = team.score;
            }
            Vote.CloseVoting();
        }

        public void RenderFinalImage(RenderTarget2D image, bool shrink)
        {
            if (_sunshineMaterial == null)
                _sunshineMaterial = new MaterialSunshine(_screenTarget);
            DuckGame.Graphics.SetRenderTarget(image);
            DuckGame.Graphics.screen.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, camera.getMatrix());
            DuckGame.Graphics.material = _sunshineMaterial;
            float num = Graphics.width / (_pixelTarget.width * (Graphics.width / 320f));
            if (shrink)
                num = 2f;
            DuckGame.Graphics.Draw(_pixelTarget, Vec2.Zero, new Rectangle?(), Color.White, 0f, Vec2.Zero, new Vec2(num), SpriteEffects.None);
            DuckGame.Graphics.material = null;
            DuckGame.Graphics.screen.End();
            DuckGame.Graphics.SetRenderTarget(null);
        }

        public override void DoDraw()
        {
            if (NetworkDebugger.enabled)
                base.DoDraw();
            else if (!RockScoreboard._drawingSunTarget && RockScoreboard._sunEnabled)
            {
                if (RockScoreboard._sunEnabled)
                    DoRender();
                DuckGame.Graphics.Clear(Color.Black);
                if (_sunshineMaterial == null)
                    _sunshineMaterial = new MaterialSunshine(_screenTarget);
                if (NetworkDebugger.enabled)
                {
                    DuckGame.Graphics.screen.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, camera.getMatrix());
                    float num = Graphics.width / (_screenTarget.width * (Graphics.width / 320f));
                    DuckGame.Graphics.Draw(_screenTarget, Vec2.Zero, new Rectangle?(), Color.White, 0f, Vec2.Zero, new Vec2(num), SpriteEffects.None);
                    DuckGame.Graphics.material = null;
                    DuckGame.Graphics.screen.End();
                }
                else
                {
                    DuckGame.Graphics.screen.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, camera.getMatrix());
                    DuckGame.Graphics.material = _sunshineMaterial;
                    float num = Graphics.width / (_pixelTarget.width * (Graphics.width / 320f));
                    DuckGame.Graphics.Draw(_pixelTarget, Vec2.Zero, new Rectangle?(), Color.White, 0f, Vec2.Zero, new Vec2(num), SpriteEffects.None);
                    DuckGame.Graphics.material = null;
                    DuckGame.Graphics.screen.End();
                }
            }
            else
                base.DoDraw();
        }

        public override void Draw()
        {
            Layer.Game.perspective = _mode == ScoreBoardMode.ShowScores;
            Layer.Game.projection = _field.projection;
            Layer.Game.view = _field.view;
            Layer.Background.perspective = true;
            Layer.Background.projection = _field.projection;
            Layer.Background.view = _field.view;
            Layer.Foreground.perspective = true;
            Layer.Foreground.projection = _field.projection;
            Layer.Foreground.view = _field.view;
            if (!RockScoreboard._sunEnabled)
                return;
            _sunLayer.perspective = true;
            _sunLayer.projection = _field.projection;
            _sunLayer.view = _field.view;
        }

        public override void PostDrawLayer(Layer layer)
        {
            if (layer == Layer.HUD)
            {
                if (_getScreenshot && DuckGame.Graphics.screenCapture == null)
                {
                    _finalSprite.scale = new Vec2(0.25f, 0.25f);
                    DuckGame.Graphics.Draw(_finalSprite, 0f, 0f);
                }
                if (_intermissionSlide > 0.00999999977648258)
                {
                    _intermissionText.depth = (Depth)0.91f;
                    float x1 = (float)(_intermissionSlide * 320.0 - 320.0);
                    float y = 60f;
                    DuckGame.Graphics.DrawRect(new Vec2(x1, y), new Vec2(x1 + 320f, y + 30f), Color.Black, (Depth)0.9f);
                    float x2 = (float)(320.0 - _intermissionSlide * 320.0);
                    float num = 60f;
                    DuckGame.Graphics.DrawRect(new Vec2(x2, num + 30f), new Vec2(x2 + 320f, num + 60f), Color.Black, (Depth)0.9f);
                    DuckGame.Graphics.Draw(_intermissionText, (float)(_intermissionSlide * 336.0 - 320.0), num + 18f);
                }
            }
            else if (layer == Layer.Game)
            {
                if (_mode == ScoreBoardMode.ShowWinner && !_afterHighlights)
                {
                    _winnerPost.depth = -0.962f;
                    _winnerBanner.depth = -0.858f;
                    float num1 = -10f;
                    DuckGame.Graphics.Draw(_winnerPost, 63f, 40f + num1);
                    DuckGame.Graphics.Draw(_winnerPost, 248f, 40f + num1);
                    DuckGame.Graphics.Draw(_winnerBanner, 70f, 43f + num1);
                    string name = Results.winner.name;
                    BitmapFont font = Results.winner.font;
                    font.scale = new Vec2(2f, 2f);
                    float num2 = 0f;
                    float num3 = 0f;
                    if (name.Length > 12)
                    {
                        font.scale = new Vec2(1f);
                        num3 = 3f;
                    }
                    else if (name.Length > 9)
                    {
                        font.scale = new Vec2(1.5f);
                        num2 = 2f;
                        num3 = 1f;
                    }
                    font.Draw(name, (float)(160.0 - font.GetWidth(name) / 2.0) + num2, 50f + num1 + num3, Color.Black, _winnerBanner.depth + 1);
                    font.scale = new Vec2(1f, 1f);
                }
            }
            else if (layer == Layer.Foreground)
            {
                int num4 = RockScoreboard._drawingSunTarget ? 1 : 0;
            }
            base.PostDrawLayer(layer);
        }
    }
}
