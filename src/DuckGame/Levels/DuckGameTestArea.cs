// Decompiled with JetBrains decompiler
// Type: DuckGame.DuckGameTestArea
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
            this._editor = editor;
            DeathmatchLevel._started = true;
            this._followCam.lerpMult = 1.1f;
            this._seed = seed;
            this._center = center;
            this._genType = genType;
            this._levelValue = level;
            DuckGameTestArea.currentEditor = editor;
        }

        public override void Initialize()
        {
            this._pauseGroup = new UIComponent(Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 0f, 0f);
            this._pauseMenu = new UIMenu("@LWING@PAUSE@RWING@", Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 160f, conString: "@CANCEL@CLOSE  @SELECT@SELECT");
            this._confirmMenu = new UIMenu("REALLY QUIT?", Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 160f, conString: "@CANCEL@BACK  @SELECT@SELECT");
            this._testMode = new UIMenu("TEST MODE", Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 160f, conString: "@CANCEL@BACK  @SELECT@SELECT");
            UIDivider component = new UIDivider(true, 0.8f);
            component.leftSection.Add(new UIMenuItem("RESTART", new UIMenuActionCloseMenuSetBoolean(this._pauseGroup, this._restart), UIAlign.Left), true);
            component.leftSection.Add(new UIMenuItem("OPTIONS", new UIMenuActionOpenMenu(_pauseMenu, Options.optionsMenu), UIAlign.Left), true);
            component.leftSection.Add(new UIMenuItem("TEST MODE", new UIMenuActionOpenMenu(_pauseMenu, _testMode), UIAlign.Left), true);
            component.leftSection.Add(new UIText("", Color.White), true);
            component.leftSection.Add(new UIMenuItem("|DGRED|QUIT", new UIMenuActionCloseMenuSetBoolean(this._pauseGroup, this._quit), UIAlign.Left), true);
            component.rightSection.Add(new UIImage("pauseIcons", UIAlign.Right), true);
            this._pauseMenu.Add(component, true);
            this._pauseMenu.Close();
            this._pauseGroup.Add(_pauseMenu, false);
            Options.AddMenus(this._pauseGroup);
            Options.openOnClose = this._pauseMenu;
            this._confirmMenu.Add(new UIMenuItem("NO!", new UIMenuActionOpenMenu(_confirmMenu, _pauseMenu), UIAlign.Left, backButton: true), true);
            this._confirmMenu.Add(new UIMenuItem("YES!", new UIMenuActionCloseMenuSetBoolean(this._pauseGroup, this._quit)), true);
            this._confirmMenu.Close();
            this._pauseGroup.Add(_confirmMenu, false);
            this._testMode.Add(new UIMenuItemNumber("PLAYERS", field: new FieldBinding(this, "numPlayers", 2f, 8f, 1f)), true);
            this._testMode.Add(new UIMenuItem("START", new UIMenuActionCloseMenuSetBoolean(this._pauseGroup, this._startTestMode)), true);
            this._testMode.SetBackFunction(new UIMenuActionOpenMenu(_testMode, _pauseMenu));
            this._testMode.Close();
            this._pauseGroup.Add(_testMode, false);
            this._pauseGroup.isPauseMenu = true;
            this._pauseGroup.Close();
            this._pauseGroup.Update();
            this._pauseGroup.Update();
            Level.Add(_pauseGroup);
            if (this._level == "RANDOM")
            {
                LevelGenerator.MakeLevel(this._center, this._center.left && this._center.right, this._seed, this._genType, Editor._procTilesWide, Editor._procTilesHigh, Editor._procXPos, Editor._procYPos).LoadParts(0f, 0f, this, this._seed);
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
                PyramidBackground pyramidBackground = new PyramidBackground(0f, 0f)
                {
                    visible = false
                };
                Level.Add(pyramidBackground);
            }
            else
            {
                this._level = this._level.Replace(Directory.GetCurrentDirectory() + "\\", "");
                LevelData levelData = DuckFile.LoadLevel(this._level);
                if (levelData != null)
                {
                    foreach (BinaryClassChunk node in levelData.objects.objects)
                    {
                        Thing t = Thing.LoadThing(node);
                        if (t != null)
                        {
                            if (!t.visibleInGame)
                                t.visible = false;
                            this.AddThing(t);
                        }
                    }
                }
            }
            this.things.RefreshState();
            foreach (Profile prof in Profiles.active)
            {
                if (prof.team != null)
                    prof.team.Leave(prof);
            }
            int num = 4;
            if (this.things[typeof(EightPlayer)].Count<Thing>() > 0)
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
                Level.Add(spawnPlayer);
                this.followCam.Add(spawnPlayer);
            }
        }

        public void PauseLogic()
        {
            if (Input.Pressed("START"))
            {
                this._pauseGroup.Open();
                this._pauseMenu.Open();
                MonoMain.pauseMenu = this._pauseGroup;
                if (this._paused)
                    return;
                SFX.Play("pause", 0.6f);
                this._paused = true;
            }
            else
            {
                if (!this._paused || MonoMain.pauseMenu != null)
                    return;
                this._paused = false;
                SFX.Play("resume", 0.6f);
                DeathmatchLevel._started = false;
            }
        }

        public override void Update()
        {
            if (this._startTestMode.value)
            {
                foreach (Profile profile in Profiles.active)
                    profile.team = null;
                if (this.numPlayers > 7)
                {
                    Profiles.DefaultPlayer8.team = Teams.Player8;
                    if (Profiles.DefaultPlayer8.inputProfile == null)
                        Profiles.DefaultPlayer8.inputProfile = InputProfile.DefaultPlayer8;
                }
                if (this.numPlayers > 6)
                {
                    Profiles.DefaultPlayer7.team = Teams.Player7;
                    if (Profiles.DefaultPlayer7.inputProfile == null)
                        Profiles.DefaultPlayer7.inputProfile = InputProfile.DefaultPlayer7;
                }
                if (this.numPlayers > 5)
                {
                    Profiles.DefaultPlayer6.team = Teams.Player6;
                    if (Profiles.DefaultPlayer6.inputProfile == null)
                        Profiles.DefaultPlayer6.inputProfile = InputProfile.DefaultPlayer6;
                }
                if (this.numPlayers > 4)
                {
                    Profiles.DefaultPlayer5.team = Teams.Player5;
                    if (Profiles.DefaultPlayer5.inputProfile == null)
                        Profiles.DefaultPlayer5.inputProfile = InputProfile.DefaultPlayer5;
                }
                if (this.numPlayers > 3)
                {
                    Profiles.DefaultPlayer4.team = Teams.Player4;
                    if (Profiles.DefaultPlayer4.inputProfile == null)
                        Profiles.DefaultPlayer4.inputProfile = InputProfile.DefaultPlayer4;
                }
                if (this.numPlayers > 2)
                {
                    Profiles.DefaultPlayer3.team = Teams.Player3;
                    if (Profiles.DefaultPlayer3.inputProfile == null)
                        Profiles.DefaultPlayer3.inputProfile = InputProfile.DefaultPlayer3;
                }
                if (this.numPlayers > 1)
                {
                    Profiles.DefaultPlayer2.team = Teams.Player2;
                    if (Profiles.DefaultPlayer2.inputProfile == null)
                        Profiles.DefaultPlayer2.inputProfile = InputProfile.DefaultPlayer2;
                }
                if (this.numPlayers > 0)
                    Profiles.experienceProfile.team = Teams.Player1;
                EditorTestLevel t = null;
                if (this.things[typeof(EditorTestLevel)].Count<Thing>() > 0)
                    t = this.things[typeof(EditorTestLevel)].First<Thing>() as EditorTestLevel;
                Level.current = new GameLevel(this._levelValue, editorTestMode: true);
                if (t == null)
                    return;
                Level.current.AddThing(t);
            }
            else if (this._restart.value)
            {
                this.transitionSpeedMultiplier = 2f;
                EditorTestLevel t = null;
                if (this.things[typeof(EditorTestLevel)].Count<Thing>() > 0)
                    t = this.things[typeof(EditorTestLevel)].First<Thing>() as EditorTestLevel;
                Level.current = new DuckGameTestArea(this._editor, this._levelValue, this._seed, this._center, this._genType);
                Level.current.transitionSpeedMultiplier = 2f;
                if (t == null)
                    return;
                Level.current.AddThing(t);
            }
            else
            {
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
                this.PauseLogic();
                if (this._quit.value)
                    Level.current = _editor;
                base.Update();
            }
        }
    }
}
