using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DuckGame
{
    public class DuckGameTestArea : DeathmatchLevel
    {
        private Editor _editor;
        protected int _seed;
        protected RandomLevelData _center;
        protected LevGenType _genType;
        private string _levelValue;
        public static Editor currentEditor;
        private MenuBoolean _capture = new MenuBoolean();
        private MenuBoolean _quit = new MenuBoolean();
        private MenuBoolean _restart = new MenuBoolean();
        private MenuBoolean _startTestMode = new MenuBoolean();
        private UIComponent _pauseGroup;
        private UIMenu _pauseMenu;
        private UIMenu _confirmMenu;
        private UIMenu _testMode;
        public int numPlayers = 4;
        private bool _paused;
        private int wait;

        public DuckGameTestArea(
          Editor editor,
          string level,
          int seed = 0,
          RandomLevelData center = null,
          LevGenType genType = LevGenType.Any)
          : base(level)
        {
            _editor = editor;
            _started = true;
            _followCam.lerpMult = 1.1f;
            _seed = seed;
            _center = center;
            _genType = genType;
            _levelValue = level;
            currentEditor = editor;
        }

        public override void Initialize()
        {
            _pauseGroup = new UIComponent(Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 0f, 0f);
            _pauseMenu = new UIMenu("@LWING@PAUSE@RWING@", Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 160f, conString: "@CANCEL@CLOSE  @SELECT@SELECT");
            _confirmMenu = new UIMenu("REALLY QUIT?", Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 160f, conString: "@CANCEL@BACK  @SELECT@SELECT");
            _testMode = new UIMenu("TEST MODE", Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 160f, conString: "@CANCEL@BACK  @SELECT@SELECT");
            UIDivider component = new UIDivider(true, 0.75f);
            component.leftSection.Add(new UIMenuItem("RESTART", new UIMenuActionCloseMenuSetBoolean(_pauseGroup, _restart), UIAlign.Left), true);
            component.leftSection.Add(new UIMenuItem("OPTIONS", new UIMenuActionOpenMenu(_pauseMenu, Options.optionsMenu), UIAlign.Left), true);
            component.leftSection.Add(new UIMenuItem("TEST MODE", new UIMenuActionOpenMenu(_pauseMenu, _testMode), UIAlign.Left), true);
            component.leftSection.Add(new UIText("", Color.White), true);
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
            _testMode.Add(new UIMenuItemNumber("PLAYERS", field: new FieldBinding(this, "numPlayers", 2f, DG.FiftyPlayerMode?DG.MaxPlayers: 8f, 1f)), true);
            _testMode.Add(new UIMenuItem(Triggers.Start, new UIMenuActionCloseMenuSetBoolean(_pauseGroup, _startTestMode)), true);
            _testMode.SetBackFunction(new UIMenuActionOpenMenu(_testMode, _pauseMenu));
            _testMode.Close();
            _pauseGroup.Add(_testMode, false);
            _pauseGroup.isPauseMenu = true;
            _pauseGroup.Close();
            _pauseGroup.Update();
            _pauseGroup.Update();
            Add(_pauseGroup);
            if (_level == "RANDOM")
            {
                LevelGenerator.MakeLevel(_center, _center.left && _center.right, _seed, _genType, Editor._procTilesWide, Editor._procTilesHigh, Editor._procXPos, Editor._procYPos).LoadParts(0f, 0f, this, _seed);
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
                PyramidBackground pyramidBackground = new PyramidBackground(0f, 0f)
                {
                    visible = false
                };
                Add(pyramidBackground);
            }
            else
            {
                _level = _level.Replace(Directory.GetCurrentDirectory() + "\\", "");
                LevelData levelData = DuckFile.LoadLevel(_level);
                if (levelData != null)
                {
                    foreach (BinaryClassChunk node in levelData.objects.objects)
                    {
                        Thing t = Thing.LoadThing(node);
                        if (t != null)
                        {
                            if (!t.visibleInGame)
                                t.visible = false;
                            AddThing(t);
                        }
                    }
                }
            }
            things.RefreshState();
            foreach (Profile prof in Profiles.active)
            {
                if (prof.team != null)
                    prof.team.Leave(prof);
            }
            int num = 4;
            if (things[typeof(EightPlayer)].Count() > 0)
                num = 8;
            for (int index = 0; index < num; ++index)
            {
                Profiles.defaultProfiles[index].team = Teams.allStock[index];
                Profiles.defaultProfiles[index].persona = Profiles.defaultProfiles[index].defaultPersona;
                Profiles.defaultProfiles[index].UpdatePersona();
                Input.ApplyDefaultMapping(Profiles.defaultProfiles[index].inputProfile, Profiles.defaultProfiles[index]);
            }
            foreach (Duck spawnPlayer in new Deathmatch(this).SpawnPlayers(false))
            {
                Add(spawnPlayer);
                if (spawnPlayer.inputProfile == null && DGRSettings.EditorOnlinePhysics)
                {
                    current = _editor;
                    _editor.needInputRefresh = true;
                }
                followCam.Add(spawnPlayer);
            }
        }

        public void PauseLogic()
        {
            if (Input.Pressed(Triggers.Start))
            {
                _pauseGroup.Open();
                _pauseMenu.Open();
                MonoMain.pauseMenu = _pauseGroup;
                if (_paused)
                    return;
                SFX.Play("pause", 0.6f);
                _paused = true;
            }
            else
            {
                if (!_paused || MonoMain.pauseMenu != null)
                    return;
                _paused = false;
                SFX.Play("resume", 0.6f);
                _started = false;
            }
        }
        public int playtestTime;
        public override void PostDrawLayer(Layer layer)
        {
            if (DGRSettings.EditorTimer && layer == Layer.HUD)
            {
                if (layer.fade <= 0)
                    return;
                TimeSpan timeSpan = TimeSpan.FromSeconds((float)playtestTime / 60f);
                string formattedTime = string.Format("{0:D2}:{1:D2}:{2:D2}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
                Graphics.DrawString(formattedTime, new Vec2(Layer.HUD.width / 2f - formattedTime.Length * 4, 0), Color.White * 0.5f, 1);
            }
            base.PostDrawLayer(layer);
        }
        public override void Update()
        {
            playtestTime++;
            if (_startTestMode.value)
            {
                foreach (Profile profile in Profiles.active)
                    profile.team = null;
                for (int i = 1; i < numPlayers; i++)
                {
                    Profile profile = Profiles.GetProfile(i);
                    profile.team = Teams.core.teams[i];
                    if (profile.inputProfile == null)
                        profile.inputProfile = InputProfile.Get(InputProfile.MPPlayers[i]);
                }
                if (numPlayers > 0)
                    Profiles.experienceProfile.team = Teams.Player1;
                EditorTestLevel t = null;
                if (things[typeof(EditorTestLevel)].Count() > 0)
                    t = things[typeof(EditorTestLevel)].First() as EditorTestLevel;
                current = new GameLevel(_levelValue, editorTestMode: true);
                if (t == null)
                    return;
                current.AddThing(t);
            }
            else if (_restart.value)
            {
                transitionSpeedMultiplier = 2f;
                EditorTestLevel t = null;
                if (things[typeof(EditorTestLevel)].Count() > 0)
                    t = things[typeof(EditorTestLevel)].First() as EditorTestLevel;
                current = new DuckGameTestArea(_editor, _levelValue, _seed, _center, _genType)
                {
                    transitionSpeedMultiplier = 2f
                };
                if (t == null)
                    return;
                current.AddThing(t);
            }
            else
            {
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
                PauseLogic();
                if (_quit.value)
                    current = _editor;
                base.Update();
            }
        }
    }
}
