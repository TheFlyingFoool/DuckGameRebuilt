// Decompiled with JetBrains decompiler
// Type: DuckGame.Deathmatch
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections.Generic;
using System.Linq;

namespace DuckGame
{
    public class Deathmatch : Thing
    {
        private bool _matchOver;
        private float _deadTimer = 1f;
        public static bool showdown = false;
        private static int numMatches = 0;
        private static Queue<string> _recentLevels = new Queue<string>();
        //    private static List<string> _demoLevels = new List<string>()
        //{
        //  "deathmatch/forest02",
        //  "deathmatch/office02",
        //  "deathmatch/forest04",
        //  "deathmatch/office07",
        //  "deathmatch/office10",
        //  "deathmatch/office05"
        //};
        private static int _winsPerSet = 5;
        private static int _roundsBetweenIntermission = 5;
        private static int _userMapsPercent = 0;
        private static bool _enableRandom = true;
        private static bool _randomMapsOnly = false;
        public static List<Profile> lastWinners = new List<Profile>();
        private static float _wait = 0f;
        private static bool _endedHighlights = false;
        private static string _currentSong = "";
        //private Sprite _bottomWedge;
        private bool _addedPoints;
        private UIComponent _pauseGroup;
        private UIMenu _pauseMenu;
        private UIMenu _confirmMenu;
        private new Level _level;
        private MenuBoolean _quit = new MenuBoolean();
        //private static List<string> _networkLevels = (List<string>)null;
        public static int levelsSinceRandom = 0;
        public static int levelsSinceWorkshop = 0;
        public static int levelsSinceCustom = 0;
        public static int clientLevelRoundRobin;
        private static bool _prevNetworkSetting;
        private static bool _prevEightPlayerSetting;
        private static List<string> _fourPlayerLevels;
        private static List<string> _eightPlayerNonRestrictedLevels;
        private static List<string> _eightPlayerAllLevels;
        private static List<string> _rareLevels;
        private static bool _lastLevelWasPyramid = false;
        private bool _paused;
        private bool switched;

        public static int winsPerSet
        {
            get => Deathmatch._winsPerSet;
            set => Deathmatch._winsPerSet = value;
        }

        public static int roundsBetweenIntermission
        {
            get => Deathmatch._roundsBetweenIntermission;
            set => Deathmatch._roundsBetweenIntermission = value;
        }

        public static int userMapsPercent
        {
            get => Deathmatch._userMapsPercent;
            set => Deathmatch._userMapsPercent = value;
        }

        public static bool enableRandom
        {
            get => Deathmatch._enableRandom;
            set => Deathmatch._enableRandom = value;
        }

        public static bool randomMapsOnly
        {
            get => Deathmatch._randomMapsOnly;
            set => Deathmatch._randomMapsOnly = value;
        }

        public Deathmatch(Level l)
          : base()
        {
            _level = l;
            layer = Layer.HUD;
            //this._bottomWedge = new Sprite("bottomWedge");
        }

        public override void Initialize()
        {
            _pauseGroup = new UIComponent(Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 0f, 0f);
            _pauseMenu = new UIMenu("@LWING@PAUSE@RWING@", Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 160f, conString: "@CANCEL@CLOSE @SELECT@SELECT");
            _confirmMenu = new UIMenu("REALLY QUIT?", Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 160f, conString: "@CANCEL@BACK @SELECT@SELECT");
            UIDivider component = new UIDivider(true, 0.8f);
            component.leftSection.Add(new UIMenuItem("RESUME", new UIMenuActionCloseMenu(_pauseGroup), UIAlign.Left), true);
            component.leftSection.Add(new UIMenuItem("OPTIONS", new UIMenuActionOpenMenu(_pauseMenu, Options.optionsMenu), UIAlign.Left), true);
            component.leftSection.Add(new UIText(" ", Color.White, UIAlign.Left), true);
            component.leftSection.Add(new UIMenuItem("|DGRED|QUIT", new UIMenuActionOpenMenu(_pauseMenu, _confirmMenu), UIAlign.Left), true);
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
            _pauseGroup.Close();
            _pauseGroup.Update();
            _pauseGroup.Update();
            Level.Add(_pauseGroup);
            Highlights.StartRound();
        }

        public override void Terminate() => Options.openOnClose = null;

        public static string RandomLevelString(string ignore = "", string folder = "deathmatch") => Deathmatch.RandomLevelString(ignore, folder, false);

        public static string RandomLevelString(string ignore, string folder, bool forceCustom)
        {
            List<string> stringList1 = new List<string>();
            if (Deathmatch._fourPlayerLevels == null || Network.isActive != Deathmatch._prevNetworkSetting || TeamSelect2.eightPlayersActive != Deathmatch._prevEightPlayerSetting)
            {
                Deathmatch._prevNetworkSetting = Network.isActive;
                Deathmatch._prevEightPlayerSetting = TeamSelect2.eightPlayersActive;
                Deathmatch._fourPlayerLevels = Content.GetLevels(folder, LevelLocation.Content, false, Network.isActive, false);
                Deathmatch._eightPlayerNonRestrictedLevels = Content.GetLevels(folder, LevelLocation.Content, false, Network.isActive, true, true);
                Deathmatch._eightPlayerAllLevels = Content.GetLevels(folder, LevelLocation.Content, false, Network.isActive, true);
                Deathmatch._rareLevels = Content.GetLevels(folder + "/rare", LevelLocation.Content, false, Network.isActive, TeamSelect2.eightPlayersActive);
                if (Network.isActive)
                {
                    Deathmatch._fourPlayerLevels.AddRange(Content.GetLevels(folder + "/online_only", LevelLocation.Content, false, Network.isActive, false));
                    Deathmatch._eightPlayerNonRestrictedLevels.AddRange(Content.GetLevels(folder + "/online_only", LevelLocation.Content, false, Network.isActive, false, true));
                    Deathmatch._eightPlayerAllLevels.AddRange(Content.GetLevels(folder + "/online_only", LevelLocation.Content, false, Network.isActive, true));
                }
            }
            if (TeamSelect2.eightPlayersActive)
            {
                stringList1.AddRange(_eightPlayerAllLevels);
            }
            else
            {
                stringList1.AddRange(_fourPlayerLevels);
                stringList1.AddRange(_eightPlayerNonRestrictedLevels);
            }
            DateTime localTime = MonoMain.GetLocalTime();
            if (DateTime.Now.Month == 12)
            {
                if (localTime.Day <= 25)
                    stringList1.Add("23ec9c56-dbcc-4384-9507-5b0f80cb0111");
                else if (localTime.Day == 24 || localTime.Day == 25)
                {
                    stringList1.Add("23ec9c56-dbcc-4384-9507-5b0f80cb0111");
                    stringList1.Add("23ec9c56-dbcc-4384-9507-5b0f80cb0111");
                    stringList1.Add("23ec9c56-dbcc-4384-9507-5b0f80cb0111");
                }
            }
            if (ignore != "")
                stringList1.AddRange(_rareLevels);
            if (TeamSelect2.normalMapPercent != 100 | forceCustom)
            {
                MapRollGroup mapRollGroup1 = null;
                MapRollGroup mapRollGroup2 = null;
                if (forceCustom)
                {
                    mapRollGroup2 = new MapRollGroup()
                    {
                        type = MapRollType.Custom
                    };
                }
                else
                {
                    int num = TeamSelect2.randomMapPercent;
                    if (Deathmatch._lastLevelWasPyramid)
                        num = (int)(num * 0.5);
                    foreach (MapRollGroup mapRollGroup3 in (IEnumerable<MapRollGroup>)new List<MapRollGroup>()
          {
            new MapRollGroup()
            {
              type = MapRollType.Normal,
              chance = TeamSelect2.normalMapPercent
            },
            new MapRollGroup()
            {
              type = MapRollType.Random,
              chance = num
            },
            new MapRollGroup()
            {
              type = MapRollType.Custom,
              chance = Deathmatch.userMapsPercent
            },
            new MapRollGroup()
            {
              type = MapRollType.Internet,
              chance = TeamSelect2.workshopMapPercent
            }
          }.OrderBy<MapRollGroup, int>(x => Rando.Int(2147483646)))
                    {
                        if ((mapRollGroup3.type != MapRollType.Custom || Editor.customLevelCount != 0) && (mapRollGroup3.type != MapRollType.Internet || RandomLevelDownloader.PeekNextLevel() != null))
                        {
                            if (mapRollGroup1 == null || mapRollGroup3.chance > mapRollGroup1.chance || mapRollGroup1.chance == 0 && mapRollGroup3.chance == 0 && mapRollGroup3.type == MapRollType.Normal)
                                mapRollGroup1 = mapRollGroup3;
                            if (Rando.Int(100) < mapRollGroup3.chance && (mapRollGroup2 == null || mapRollGroup3.chance < mapRollGroup2.chance))
                                mapRollGroup2 = mapRollGroup3;
                        }
                    }
                    if (mapRollGroup2 == null)
                        mapRollGroup2 = mapRollGroup1;
                }
                if (mapRollGroup2.type == MapRollType.Custom && Editor.customLevelCount == 0)
                    mapRollGroup2.type = MapRollType.Normal;
                if (mapRollGroup2.type == MapRollType.Random)
                    return "RANDOM";
                if (mapRollGroup2.type == MapRollType.Internet)
                    return "WORKSHOP";
                if (mapRollGroup2.type == MapRollType.Custom)
                {
                    stringList1.Clear();
                    if (Network.isActive && Network.isServer && (bool)TeamSelect2.GetMatchSetting("clientlevelsenabled").value)
                    {
                        Profile profile1 = null;
                        int num = 0;
                        do
                        {
                            ++Deathmatch.clientLevelRoundRobin;
                            foreach (Profile profile2 in DuckNetwork.profiles)
                            {
                                if (profile2.connection != null && profile2.slotType != SlotType.Local && profile2.networkIndex == Deathmatch.clientLevelRoundRobin % GameLevel.NumberOfDucks && (profile2.connection != DuckNetwork.localConnection || Editor.activatedLevels.Count != 0) && (profile2.connection == DuckNetwork.localConnection || profile2.numClientCustomLevels != 0))
                                {
                                    profile1 = profile2;
                                    break;
                                }
                            }
                            ++num;
                            if (num > 10)
                                return Deathmatch.RandomLevelString(ignore, folder);
                        }
                        while (profile1 == null);
                        if (profile1.connection != DuckNetwork.localConnection)
                            return profile1.networkIndex.ToString() + ".client";
                    }
                    foreach (string activatedLevel in Editor.activatedLevels)
                        stringList1.Add(activatedLevel + ".custom");
                    if (stringList1.Count == 0 & forceCustom)
                        return "";
                }
            }
            if (_recentLevels.Count > stringList1.Count * 0.8f)
                Deathmatch._recentLevels.Clear();
            List<string> stringList2 = new List<string>();
            stringList2.AddRange(stringList1);
            Deathmatch._lastLevelWasPyramid = false;
            string str = "";
            while (str == "")
            {
                if (stringList2.Count == 0)
                {
                    str = "RANDOM";
                    break;
                }
                if (stringList1.Count == 0 && Deathmatch._recentLevels.Count > 0)
                {
                    str = Deathmatch._recentLevels.Dequeue();
                    if (!stringList2.Contains(str))
                        str = "";
                }
                else if (stringList1.Count == 0)
                {
                    str = stringList2[0];
                }
                else
                {
                    str = stringList1[Rando.Int(stringList1.Count<string>() - 1)];
                    if (str == ignore && stringList1.Count > 1)
                    {
                        stringList1.Remove(str);
                        str = "";
                    }
                    else if (!TeamSelect2.eightPlayersActive && Deathmatch._eightPlayerNonRestrictedLevels.Contains(str) && Rando.Float(1f) > 0.2f)
                    {
                        stringList1.Remove(str);
                        str = "";
                    }
                    else if (!Deathmatch._rareLevels.Contains(str) || Rando.Float(1f) > 0.75f)
                    {
                        if (Deathmatch._recentLevels.Contains(str))
                        {
                            if (Deathmatch._recentLevels.LastOrDefault<string>() == str)
                            {
                                stringList1.Remove(str);
                                str = "";
                            }
                            else if (Rando.Float(1f) < 0.95f)
                            {
                                stringList1.Remove(str);
                                str = "";
                            }
                        }
                    }
                    else
                    {
                        stringList1.Remove(str);
                        str = "";
                    }
                }
            }
            if (str != "RANDOM")
                Deathmatch._recentLevels.Enqueue(str);
            else
                Deathmatch._lastLevelWasPyramid = true;
            if (str.EndsWith(".custom"))
            {
                LevelData dat = DuckFile.LoadLevel(str.Substring(0, str.Length - 7));
                if (dat != null)
                {
                    str = dat.metaData.guid + ".custom";
                    if (Content.GetLevel(dat.metaData.guid, LevelLocation.Custom) == null)
                        Content.MapLevel(dat.metaData.guid, dat, LevelLocation.Custom);
                }
            }
            return string.IsNullOrWhiteSpace(str) ? "RANDOM" : str;
        }

        public override void Update()
        {
            if (Graphics.fade > 0.9f && Input.Pressed("START") && !NetworkDebugger.enabled)
            {
                _pauseGroup.Open();
                _pauseMenu.Open();
                MonoMain.pauseMenu = _pauseGroup;
                if (_paused)
                    return;
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
                    Music.Resume();
                }
                if (_quit.value)
                {
                    Graphics.fade -= 0.04f;
                    if (Graphics.fade >= 0.01f)
                        return;
                    Level.current = new TitleScreen();
                }
                else
                {
                    if (Music.finished)
                        Deathmatch._wait -= 0.0006f;
                    if (!_matchOver)
                    {
                        List<Team> teamList = new List<Team>();
                        foreach (Team team in Teams.all)
                        {
                            foreach (Profile activeProfile in team.activeProfiles)
                            {
                                if (activeProfile.duck != null && !activeProfile.duck.dead)
                                {
                                    if (activeProfile.duck.converted != null && activeProfile.duck.converted.profile.team != activeProfile.team)
                                    {
                                        if (!teamList.Contains(activeProfile.duck.converted.profile.team))
                                        {
                                            teamList.Add(activeProfile.duck.converted.profile.team);
                                            break;
                                        }
                                        break;
                                    }
                                    if (!teamList.Contains(team))
                                    {
                                        teamList.Add(team);
                                        break;
                                    }
                                    break;
                                }
                            }
                        }
                        if (teamList.Count <= 1)
                        {
                            _matchOver = true;
                            ++Deathmatch.numMatches;
                            if (Deathmatch.numMatches >= Deathmatch.roundsBetweenIntermission || Deathmatch.showdown)
                                Deathmatch.numMatches = 0;
                        }
                    }
                    if (_matchOver)
                        _deadTimer -= 0.005f;
                    if (_deadTimer < 0.5 && !_addedPoints)
                    {
                        List<Team> collection = new List<Team>();
                        List<Team> source = new List<Team>();
                        foreach (Team team in Teams.all)
                        {
                            foreach (Profile activeProfile in team.activeProfiles)
                            {
                                if (activeProfile.duck != null && !activeProfile.duck.dead)
                                {
                                    if (activeProfile.duck.converted != null && activeProfile.duck.converted.profile.team != activeProfile.team)
                                    {
                                        if (!source.Contains(activeProfile.duck.converted.profile.team))
                                            source.Add(activeProfile.duck.converted.profile.team);
                                        if (!collection.Contains(activeProfile.duck.profile.team))
                                        {
                                            collection.Add(activeProfile.duck.profile.team);
                                            break;
                                        }
                                        break;
                                    }
                                    if (!source.Contains(team))
                                    {
                                        source.Add(team);
                                        break;
                                    }
                                    break;
                                }
                            }
                        }
                        if (source.Count <= 1)
                        {
                            Highlights.highlightRatingMultiplier = 0f;
                            Deathmatch.lastWinners.Clear();
                            if (source.Count > 0)
                            {
                                Event.Log(new RoundEndEvent());
                                SFX.Play("scoreDing", 0.8f);
                                if (!TeamSelect2.KillsForPoints)
                                {
                                    source.AddRange(collection);
                                    foreach (Team team in source)
                                    {
                                        foreach (Profile activeProfile in team.activeProfiles)
                                        {
                                            if (activeProfile.duck != null && !activeProfile.duck.dead)
                                            {
                                                Deathmatch.lastWinners.Add(activeProfile);
                                                activeProfile.stats.lastWon = DateTime.Now;
                                                ++activeProfile.stats.matchesWon;
                                                Profile p = activeProfile;
                                                if (activeProfile.duck.converted != null)
                                                    p = activeProfile.duck.converted.profile;
                                                PlusOne plusOne = new PlusOne(0f, 0f, p)
                                                {
                                                    _duck = activeProfile.duck,
                                                    anchor = (Anchor)activeProfile.duck
                                                };
                                                plusOne.anchor.offset = new Vec2(0f, -16f);
                                                Level.Add(plusOne);
                                            }
                                        }
                                    }
                                    if (Network.isActive && Network.isServer)
                                        Send.Message(new NMAssignWin(Deathmatch.lastWinners, null));
                                    ++source.First<Team>().score;
                                }
                            }
                        }
                        _addedPoints = true;
                    }
                    if (_deadTimer < 0.1f && !Deathmatch._endedHighlights)
                    {
                        Deathmatch._endedHighlights = true;
                        Highlights.FinishRound();
                    }
                    if (_deadTimer >= 0.0 || switched || Network.isActive)
                        return;
                    foreach (Team team in Teams.all)
                    {
                        foreach (Profile activeProfile in team.activeProfiles)
                            Profiles.Save(activeProfile);
                    }
                    int num = 0;
                    List<Team> winning = Teams.winning;
                    if (winning.Count > 0)
                        num = winning[0].score;
                    if (num <= 4)
                        return;
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
        }

        public void PlayMusic()
        {
            string music = Music.RandomTrack("InGame", Deathmatch._currentSong);
            Music.Play(music, false);
            Deathmatch._currentSong = music;
            Deathmatch._wait = 1f;
        }

        //private SpawnPoint AttemptTeamSpawn(
        //  Team team,
        //  List<SpawnPoint> usedSpawns,
        //  List<Duck> spawned)
        //{
        //    Level level = this._level;
        //    List<TeamSpawn> teamSpawnList = new List<TeamSpawn>();
        //    foreach (TeamSpawn teamSpawn in this._level.things[typeof(TeamSpawn)])
        //    {
        //        if (!usedSpawns.Contains((SpawnPoint)teamSpawn))
        //            teamSpawnList.Add(teamSpawn);
        //    }
        //    if (teamSpawnList.Count <= 0)
        //        return (SpawnPoint)null;
        //    TeamSpawn teamSpawn1 = teamSpawnList[Rando.Int(teamSpawnList.Count - 1)];
        //    usedSpawns.Add((SpawnPoint)teamSpawn1);
        //    for (int index = 0; index < team.numMembers; ++index)
        //    {
        //        Vec2 position = teamSpawn1.position;
        //        if (team.numMembers == 2)
        //        {
        //            float num = 18.82353f;
        //            position.x = (float)(teamSpawn1.position.x - 16.0 + num * index);
        //        }
        //        else if (team.numMembers == 3)
        //        {
        //            float num = 9.411764f;
        //            position.x = (float)(teamSpawn1.position.x - 16.0 + num * index);
        //        }
        //        Duck duck = new Duck(position.x, position.y - 7f, team.activeProfiles[index]);
        //        duck.offDir = teamSpawn1.offDir;
        //        spawned.Add(duck);
        //    }
        //    return (SpawnPoint)teamSpawn1;
        //}

        //private SpawnPoint AttemptFreeSpawn(
        //  Profile profile,
        //  List<SpawnPoint> usedSpawns,
        //  List<Duck> spawned)
        //{
        //    Level level = this._level;
        //    List<SpawnPoint> spawnPointList = new List<SpawnPoint>();
        //    foreach (FreeSpawn freeSpawn in this._level.things[typeof(FreeSpawn)])
        //    {
        //        if (!usedSpawns.Contains((SpawnPoint)freeSpawn))
        //            spawnPointList.Add((SpawnPoint)freeSpawn);
        //    }
        //    if (spawnPointList.Count == 0)
        //        return (SpawnPoint)null;
        //    SpawnPoint spawnPoint = spawnPointList[Rando.Int(spawnPointList.Count - 1)];
        //    usedSpawns.Add(spawnPoint);
        //    Duck duck = new Duck(spawnPoint.x, spawnPoint.y - 7f, profile);
        //    duck.offDir = spawnPoint.offDir;
        //    spawned.Add(duck);
        //    return spawnPoint;
        //}

        //private SpawnPoint AttemptAnySpawn(
        //  Profile profile,
        //  List<SpawnPoint> usedSpawns,
        //  List<Duck> spawned)
        //{
        //    Level level = this._level;
        //    List<SpawnPoint> spawnPointList = new List<SpawnPoint>();
        //    foreach (SpawnPoint spawnPoint in this._level.things[typeof(SpawnPoint)])
        //    {
        //        if (!usedSpawns.Contains(spawnPoint))
        //            spawnPointList.Add(spawnPoint);
        //    }
        //    if (spawnPointList.Count == 0)
        //    {
        //        if (usedSpawns.Count <= 0)
        //            return (SpawnPoint)null;
        //        spawnPointList.AddRange((IEnumerable<SpawnPoint>)usedSpawns);
        //    }
        //    SpawnPoint spawnPoint1 = spawnPointList[Rando.Int(spawnPointList.Count - 1)];
        //    usedSpawns.Add(spawnPoint1);
        //    Duck duck = new Duck(spawnPoint1.x, spawnPoint1.y - 7f, profile);
        //    duck.offDir = spawnPoint1.offDir;
        //    spawned.Add(duck);
        //    return spawnPoint1;
        //}

        public List<Duck> SpawnPlayers(bool recordStats) => Spawn.SpawnPlayers(recordStats);

        public override void Draw()
        {
        }
    }
}
