// Decompiled with JetBrains decompiler
// Type: DuckGame.ChallengeLevel
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DuckGame
{
    public class ChallengeLevel : XMLLevel, IHaveAVirtualTransition
    {
        protected FollowCam _followCam;
        private BitmapFont _font;
        private LevelData _levelData;
        public bool _validityTest;
        private List<Duck> _pendingSpawns;
        public static int targetsShot;
        public static int goodiesGot;
        public static bool running;
        public static bool allTargetsShot;
        private static ChallengeTimer _timer;
        public static bool random;
        private MenuBoolean _capture = new MenuBoolean();
        private MenuBoolean _quit = new MenuBoolean();
        private MenuBoolean _restart = new MenuBoolean();
        private UIComponent _pauseGroup;
        private UIComponent _trophyGroup;
        private UIMenu _pauseMenu;
        private UIMenu _confirmMenu;
        private UIMenu _trophyMenu;
        private UIMenu _captureMenu;
        private bool _firstStart = true;
        private float _finishWait = 0.75f;
        private bool _finished;
        private bool _playedEndMusic;
        private float _restartMessageWait = 1f;
        private bool _win;
        private bool _developer;
        public ChallengeMode _challenge;
        private bool _doRestart;
        private float _waitForRestart = 1f;
        private float _waitFade;
        protected float _waitSpawn = 2f;
        private float _showResultsWait = 1f;
        private float _waitAfterSpawn = 1f;
        private int _waitAfterSpawnDings;
        private bool _didFade;
        private bool _started;
        private float _fontFade = 1f;
        private bool _paused;
        private bool _restarting;
        private static Duck _duck;
        private bool _showedEndMenu;
        private float _showEndTextWait = 1f;
        private bool _fading;
        private RenderTarget2D _captureTarget;

        public FollowCam followCam => _followCam;

        public ChallengeLevel(string name)
          : base(name)
        {
            _followCam = new FollowCam
            {
                lerpMult = 1f,
                startCentered = false
            };
            camera = _followCam;
            simulatePhysics = false;
        }

        public ChallengeLevel(LevelData data, bool validityTest)
          : base(data)
        {
            _followCam = new FollowCam
            {
                lerpMult = 1f,
                startCentered = false
            };
            camera = _followCam;
            simulatePhysics = false;
            _levelData = data;
            _validityTest = validityTest;
        }

        public static ChallengeTimer timer => ChallengeLevel._timer;

        public override void Initialize()
        {
            MonoMain.FinishLazyLoad();
            ChallengeLevel.targetsShot = 0;
            ChallengeLevel.goodiesGot = 0;
            ChallengeLevel.allTargetsShot = true;
            ChallengeLevel.running = false;
            ChallengeLevel._timer = new ChallengeTimer();
            if (DuckNetwork.core.speedrunMode)
                ChallengeRando.generator = new Random(1337);
            base.Initialize();
            _font = new BitmapFont("biosFont", 8);
            foreach (Team team in Teams.all)
            {
                int num1;
                int num2 = num1 = 0;
                team.score = num1;
                team.prevScoreboardScore = num2;
            }
            bool flag = true;
            foreach (Profile prof in Profiles.active)
            {
                if (flag)
                {
                    flag = false;
                }
                else
                {
                    if (prof.team != null)
                        prof.team.Leave(prof);
                    prof.inputProfile = null;
                }
            }
            _pendingSpawns = new Deathmatch(this).SpawnPlayers(false);
            _pendingSpawns = _pendingSpawns.OrderBy<Duck, float>(sp => sp.x).ToList<Duck>();
            foreach (Thing pendingSpawn in _pendingSpawns)
                followCam.Add(pendingSpawn);
            _pauseGroup = new UIComponent(Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 0f, 0f);
            _pauseMenu = new UIMenu("@LWING@CHALLENGE@RWING@", Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 160f, conString: "@CANCEL@CLOSE @SELECT@SELECT");
            _confirmMenu = new UIMenu("REALLY QUIT?", Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 160f, conString: "@CANCEL@BACK @SELECT@SELECT");
            _captureMenu = new UICaptureBox(_pauseMenu, Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 160f);
            _captureMenu.Close();
            _pauseGroup.Add(_captureMenu, false);
            UIDivider component = new UIDivider(true, 0.8f);
            component.leftSection.Add(new UIMenuItem("RESTART!", new UIMenuActionCloseMenuSetBoolean(_pauseGroup, _restart), UIAlign.Left), true);
            component.leftSection.Add(new UIMenuItem("RESUME", new UIMenuActionCloseMenu(_pauseGroup), UIAlign.Left), true);
            component.leftSection.Add(new UIMenuItem("OPTIONS", new UIMenuActionOpenMenu(_pauseMenu, Options.optionsMenu), UIAlign.Left), true);
            component.leftSection.Add(new UIText("", Color.White), true);
            component.leftSection.Add(new UIMenuItem("|DGRED|QUIT", new UIMenuActionOpenMenu(_pauseMenu, _confirmMenu), UIAlign.Left), true);
            if (things[typeof(EditorTestLevel)].Count<Thing>() > 0)
            {
                component.leftSection.Add(new UIText("", Color.White), true);
                component.leftSection.Add(new UIMenuItem("CAPTURE ICON", new UIMenuActionOpenMenu(_pauseMenu, _captureMenu), UIAlign.Left), true);
            }
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
            _pauseGroup.isPauseMenu = true;
            _pauseGroup.Close();
            Level.Add(_pauseGroup);
            Music.volume = 1f;
            followCam.Adjust();
        }

        public void ChallengeEnded(ChallengeMode challenge)
        {
            Music.Stop();
            _developer = false;
            if (challenge.wonTrophies.Count > 0)
            {
                SFX.Play("scoreDing");
                _win = true;
                if (challenge.wonTrophies[0].type == TrophyType.Developer)
                {
                    Options.Data.gotDevMedal = true;
                    _developer = true;
                }
            }
            else
            {
                SFX.Play("recordStop");
                _win = false;
            }
            _finished = true;
            _challenge = challenge;
        }

        public void RestartChallenge() => _doRestart = true;

        public static Duck duck => ChallengeLevel._duck;

        private string FormatResultString(
          string captionColor,
          string caption,
          string resultColor,
          string result,
          string trophyColor,
          string trophy,
          int wide = 26)
        {
            if (trophy == null)
                trophy = "";
            if (trophyColor == null)
                trophyColor = "";
            int num = wide - (caption + result + trophy).Length;
            bool flag = false;
            while (num > 0)
            {
                --num;
                if (flag || trophy == "")
                    caption += " ";
                else
                    result += " ";
                flag = !flag;
            }
            return captionColor + caption + resultColor + result + trophyColor + trophy;
        }

        public override void Update()
        {
            ++MonoMain.timeInArcade;
            ChallengeLevel._timer.Update();
            if (_fading)
            {
                DuckGame.Graphics.fade = Lerp.Float(DuckGame.Graphics.fade, 0f, 0.05f);
                if (Graphics.fade >= 0.01f)
                    return;
                if (_validityTest)
                {
                    ArcadeTestDialogue.success = _challenge.wonTrophies.Count > 0 && _challenge.wonTrophies.Count > 0 && (_challenge.wonTrophies[0].type == TrophyType.Platinum || _challenge.wonTrophies[0].type == TrophyType.Developer);
                    Level.current = ArcadeTestDialogue.currentEditor;
                    DuckGame.Graphics.fade = 1f;
                }
                else
                {
                    if (things[typeof(EditorTestLevel)].Count<Thing>() > 0)
                    {
                        Level.current = (things[typeof(EditorTestLevel)].First() as EditorTestLevel).editor;
                        Music.Stop();
                    }
                    else
                        Level.current = Arcade.currentArcade == null ? ArcadeLevel.currentArcade : Arcade.currentArcade;
                    _fading = false;
                }
            }
            else
            {
                if (_restartMessageWait > 0f)
                    _restartMessageWait -= 0.008f;
                else
                    HUD.CloseCorner(HUDCorner.TopLeft);
                if (_doRestart)
                {
                    ChallengeLevel.running = false;
                    _waitForRestart -= 0.04f;
                    if (_waitForRestart <= 0f)
                        _restarting = true;
                }
                _waitFade -= 0.04f;
                if (!_didFade && _waitFade <= 0f && DuckGame.Graphics.fade < 1f)
                    DuckGame.Graphics.fade = Lerp.Float(DuckGame.Graphics.fade, 1f, 0.04f);
                else if (_restarting)
                {
                    ChallengeLevel.running = false;
                    transitionSpeedMultiplier = 2f;
                    EditorTestLevel t = null;
                    if (things[typeof(EditorTestLevel)].Count<Thing>() > 0)
                        t = things[typeof(EditorTestLevel)].First<Thing>() as EditorTestLevel;
                    Level.current = !(_level != "") ? new ChallengeLevel(_levelData, _validityTest) : (Level)new ChallengeLevel(_level);
                    Level.current.transitionSpeedMultiplier = 2f;
                    ((ChallengeLevel)Level.current)._waitSpawn = 0f;
                    if (t == null)
                        return;
                    Level.current.AddThing(t);
                }
                else
                {
                    if (_waitFade > 0f)
                        return;
                    _didFade = true;
                    if (_finished)
                    {
                        ChallengeLevel.running = false;
                        PauseLogic();
                        if (_finishWait > 0f)
                        {
                            _finishWait -= 0.01f;
                        }
                        else
                        {
                            if (!_playedEndMusic)
                            {
                                _playedEndMusic = true;
                                Level.current.simulatePhysics = false;
                                ArcadeFrame arcadeFrame = null;
                                if (_win)
                                {
                                    if (ArcadeLevel.currentArcade != null)
                                    {
                                        arcadeFrame = ArcadeLevel.currentArcade.GetFrame();
                                        if (arcadeFrame != null)
                                        {
                                            Vec2 renderTargetSize = arcadeFrame.GetRenderTargetSize();
                                            float renderTargetZoom = arcadeFrame.GetRenderTargetZoom();
                                            if (_captureTarget == null)
                                                _captureTarget = new RenderTarget2D((int)(renderTargetSize.x * 6f), (int)(renderTargetSize.y * 6f));
                                            int num = DuckGame.Graphics.width / 320;
                                            Camera camera = new Camera(0f, 0f, _captureTarget.width * renderTargetZoom, _captureTarget.height * renderTargetZoom);
                                            if (ChallengeLevel._duck != null)
                                            {
                                                Layer.HUD.visible = false;
                                                MonoMain.RenderGame(MonoMain.screenCapture);
                                                Layer.HUD.visible = true;
                                                Matrix result;
                                                Matrix.CreateOrthographicOffCenter(0f, MonoMain.screenCapture.width, MonoMain.screenCapture.height, 0f, 0f, -1f, out result);
                                                result.M41 += -0.5f * result.M11;
                                                result.M42 += -0.5f * result.M22;
                                                Matrix matrix = Level.current.camera.getMatrix();
                                                Vec3 vec3 = (Vec3)DuckGame.Graphics.viewport.Project((Vector3)new Vec3(ChallengeLevel._duck.cameraPosition.x, ChallengeLevel._duck.cameraPosition.y, 0f), (Microsoft.Xna.Framework.Matrix)result, (Microsoft.Xna.Framework.Matrix)matrix, (Microsoft.Xna.Framework.Matrix)Matrix.Identity);
                                                DuckGame.Graphics.SetRenderTarget(_captureTarget);
                                                camera.center = new Vec2(vec3.x, vec3.y);
                                                if (camera.bottom > MonoMain.screenCapture.height)
                                                    camera.centerY = MonoMain.screenCapture.height - camera.height / 2f;
                                                if (camera.top < 0f)
                                                    camera.centerY = camera.height / 2f;
                                                if (camera.right > MonoMain.screenCapture.width)
                                                    camera.centerX = MonoMain.screenCapture.width - camera.width / 2f;
                                                if (camera.left < 0f)
                                                    camera.centerX = camera.width / 2f;
                                                DuckGame.Graphics.Clear(Color.Black);
                                                DuckGame.Graphics.screen.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.DepthRead, RasterizerState.CullNone, null, camera.getMatrix());
                                                DuckGame.Graphics.Draw(MonoMain.screenCapture, 0f, 0f);
                                                DuckGame.Graphics.screen.End();
                                                DuckGame.Graphics.SetRenderTarget(null);
                                            }
                                        }
                                    }
                                    if (_challenge.wonTrophies.Count > 0 && _challenge.wonTrophies[0].type == TrophyType.Developer)
                                        SFX.Play("developerWin");
                                    else
                                        SFX.Play("challengeWin");
                                    _showEndTextWait = 1f;
                                }
                                else
                                {
                                    SFX.Play("challengeLose");
                                    _showEndTextWait = 1f;
                                }
                                _trophyGroup = new UIComponent(Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 0f, 0f);
                                _trophyMenu = !_validityTest || _challenge.wonTrophies.Count <= 0 || _challenge.wonTrophies[0].type != TrophyType.Platinum && _challenge.wonTrophies[0].type != TrophyType.Developer ? new UIMenu("@LWING@" + _challenge.challenge.GetNameForDisplay() + "@RWING@", Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 230f, conString: "@CANCEL@RETRY @SELECT@CONTINUE") : new UIMenu("@LWING@" + _challenge.challenge.GetNameForDisplay() + "@RWING@", Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 230f, conString: "@SELECT@CONTINUE");
                                UIDivider component1 = new UIDivider(false, 0f, 6f);
                                UIDivider component2 = new UIDivider(true, 0f);
                                SpriteMap imageVal = new SpriteMap("challengeTrophy", 60, 58);
                                component1.leftSection.vertical = false;
                                component1.leftSection.borderSize.y = 2f;
                                bool flag1 = false;
                                bool flag2 = false;
                                ChallengeSaveData saveData = ChallengeLevel._duck.profile.GetSaveData(id);
                                if (_challenge.wonTrophies.Count > 0 && _challenge.wonTrophies[0].type > saveData.trophy)
                                {
                                    saveData.trophy = _challenge.wonTrophies[0].type;
                                    flag2 = true;
                                }
                                string captionColor = "|DGRED|";
                                if (saveData.trophy == TrophyType.Bronze)
                                    captionColor = "|CBRONZE|";
                                else if (saveData.trophy == TrophyType.Silver)
                                    captionColor = "|CSILVER|";
                                else if (saveData.trophy == TrophyType.Gold)
                                    captionColor = "|CGOLD|";
                                else if (saveData.trophy == TrophyType.Platinum)
                                    captionColor = "|CPLATINUM|";
                                else if (saveData.trophy == TrophyType.Developer)
                                    captionColor = "|CDEV|";
                                int bestTime = saveData.bestTime;
                                TimeSpan elapsed;
                                if (saveData.bestTime != 0)
                                {
                                    elapsed = ChallengeLevel.timer.elapsed;
                                    if ((int)(elapsed.TotalSeconds * 1000f) >= saveData.bestTime)
                                        goto label_64;
                                }
                                ChallengeSaveData challengeSaveData = saveData;
                                elapsed = ChallengeLevel.timer.elapsed;
                                int num1 = (int)(elapsed.TotalSeconds * 1000f);
                                challengeSaveData.bestTime = num1;
                            label_64:
                                int targets1 = saveData.targets;
                                if (ChallengeLevel.targetsShot > saveData.targets)
                                    saveData.targets = ChallengeLevel.targetsShot;
                                int targets2 = saveData.targets;
                                if (ChallengeLevel.goodiesGot > saveData.goodies)
                                    saveData.goodies = ChallengeLevel.goodiesGot;
                                bool flag3 = false;
                                if (_challenge.challenge.hasTimeRequirements)
                                {
                                    int num2;
                                    if (bestTime <= 0)
                                    {
                                        num2 = 1;
                                    }
                                    else
                                    {
                                        elapsed = ChallengeLevel.timer.elapsed;
                                        num2 = elapsed.TotalSeconds * 1000f < bestTime ? 1 : 0;
                                    }
                                    bool flag4 = num2 != 0;
                                    string textVal = FormatResultString("|WHITE|", "TIME", flag4 ? "|TIMELIME|" : "|DGRED|", MonoMain.TimeString(ChallengeLevel.timer.elapsed, small: true), _challenge.wonTrophies.Count > 0 ? _challenge.wonTrophies[0].colorString : "|DGRED|", _challenge.wonTrophies.Count > 0 ? _challenge.wonTrophies[0].name : "FAILED!");
                                    component1.leftSection.Add(new UIText(textVal, Color.White, UIAlign.Left), true);
                                    if (flag4)
                                        flag1 = true;
                                    string result = MonoMain.TimeString(TimeSpan.FromMilliseconds(saveData.bestTime), small: true);
                                    component2.leftSection.Add(new UIText(FormatResultString(captionColor, "BEST", "|WHITE|", result, null, null, 19), Color.White, UIAlign.Left), true);
                                    component2.leftSection.Add(new UIText("               ", Color.White, UIAlign.Left), true);
                                    flag3 = true;
                                }
                                if (!flag3 && _challenge.challenge.countTargets)
                                {
                                    string caption = "TARGETS";
                                    if (_challenge.challenge.prefix != "" && _challenge.challenge.prefix != null)
                                        caption = _challenge.challenge.prefix;
                                    string result = ChallengeLevel.targetsShot.ToString();
                                    if (targets1 < ChallengeLevel.targetsShot)
                                        flag1 = true;
                                    component2.leftSection.Add(new UIText(FormatResultString(captionColor, "BEST", "|WHITE|", saveData.targets.ToString(), null, null, 19), Color.White, UIAlign.Left), true);
                                    component2.leftSection.Add(new UIText("               ", Color.White, UIAlign.Left), true);
                                    string textVal = FormatResultString("|WHITE|", caption, ChallengeLevel.targetsShot > targets1 ? "|TIMELIME|" : "|DGRED|", result, _challenge.wonTrophies.Count > 0 ? _challenge.wonTrophies[0].colorString : "|DGRED|", _challenge.wonTrophies.Count > 0 ? _challenge.wonTrophies[0].name : "FAILED!");
                                    component1.leftSection.Add(new UIText(textVal, Color.White, UIAlign.Left), true);
                                    flag3 = true;
                                }
                                if (!flag3 && _challenge.challenge.countGoodies)
                                {
                                    string caption = "NUMBER";
                                    if (_challenge.challenge.prefix != "" && _challenge.challenge.prefix != null)
                                        caption = _challenge.challenge.prefix;
                                    string result = ChallengeLevel.goodiesGot.ToString();
                                    if (targets2 < ChallengeLevel.goodiesGot)
                                        flag1 = true;
                                    component2.leftSection.Add(new UIText(FormatResultString(captionColor, "BEST", "|WHITE|", saveData.goodies.ToString(), null, null, 19), Color.White, UIAlign.Left), true);
                                    component2.leftSection.Add(new UIText("               ", Color.White, UIAlign.Left), true);
                                    string textVal = FormatResultString("|WHITE|", caption, ChallengeLevel.goodiesGot > targets2 ? "|TIMELIME|" : "|DGRED|", result, _challenge.wonTrophies.Count > 0 ? _challenge.wonTrophies[0].colorString : "|DGRED|", _challenge.wonTrophies.Count > 0 ? _challenge.wonTrophies[0].name : "FAILED!");
                                    component1.leftSection.Add(new UIText(textVal, Color.White, UIAlign.Left), true);
                                }
                                if (saveData.trophy < TrophyType.Gold || !flag2)
                                    flag1 = false;
                                int wide = 19;
                                foreach (ChallengeTrophy challengeTrophy in (IEnumerable<ChallengeTrophy>)_challenge.challenge.trophies.OrderBy<ChallengeTrophy, int>(x => -(int)x.type))
                                {
                                    if (challengeTrophy.type == TrophyType.Gold || challengeTrophy.type == TrophyType.Silver || challengeTrophy.type == TrophyType.Bronze)
                                    {
                                        string result = "";
                                        int num3;
                                        if (_challenge.challenge.hasTimeRequirements)
                                        {
                                            if (challengeTrophy.timeRequirement == 0 && _challenge.challenge.trophies[0].timeRequirement != 0)
                                                challengeTrophy.timeRequirement = _challenge.challenge.trophies[0].timeRequirement;
                                            result = challengeTrophy.timeRequirement == 0 ? "ANY TIME" : MonoMain.TimeString(TimeSpan.FromSeconds(challengeTrophy.timeRequirement), small: true);
                                        }
                                        else if (_challenge.challenge.countGoodies)
                                        {
                                            num3 = challengeTrophy.goodies;
                                            result = num3.ToString();
                                        }
                                        else if (_challenge.challenge.countTargets)
                                        {
                                            num3 = challengeTrophy.targets;
                                            result = num3.ToString();
                                        }
                                        component2.leftSection.Add(new UIText(FormatResultString(challengeTrophy.colorString, challengeTrophy.name, "|WHITE|", result, null, null, wide), Color.White, UIAlign.Left), true);
                                        component2.leftSection.Add(new UIText("               ", Color.White, UIAlign.Left), true);
                                    }
                                }
                                if (_challenge.wonTrophies.Count > 0)
                                    imageVal.frame = (int)_challenge.wonTrophies[0].type;
                                component2.rightSection.Add(new UIImage(imageVal, UIAlign.Right), true);
                                if (_validityTest && _challenge.wonTrophies.Count > 0 && (_challenge.wonTrophies[0].type == TrophyType.Platinum || _challenge.wonTrophies[0].type == TrophyType.Developer))
                                {
                                    _trophyMenu.SetBackFunction(new UIMenuActionCloseMenuSetBoolean(_trophyGroup, _quit));
                                    _trophyMenu.SetAcceptFunction(new UIMenuActionCloseMenuSetBoolean(_trophyGroup, _quit));
                                }
                                else
                                {
                                    _trophyMenu.SetBackFunction(new UIMenuActionCloseMenuSetBoolean(_trophyGroup, _restart));
                                    _trophyMenu.SetAcceptFunction(new UIMenuActionCloseMenuSetBoolean(_trophyGroup, _quit));
                                }
                                component1.rightSection.Add(component2, true);
                                _trophyMenu.Add(component1, true);
                                _trophyMenu.Close();
                                _trophyGroup.Add(_trophyMenu, false);
                                _trophyGroup.Close();
                                Level.Add(_trophyGroup);
                                if (arcadeFrame != null & flag1 && saveData != null)
                                {
                                    saveData.frameID = arcadeFrame._identifier;
                                    saveData.frameImage = Editor.TextureToString((Texture2D)_captureTarget);
                                    arcadeFrame.saveData = saveData;
                                }
                                Profiles.Save(ChallengeLevel._duck.profile);
                            }
                            if (_showEndTextWait > 0.0)
                            {
                                _showEndTextWait -= 0.01f;
                            }
                            else
                            {
                                _fontFade = 1f;
                                if (_showResultsWait > 0.0)
                                    _showResultsWait -= 0.01f;
                                else if (!_showedEndMenu)
                                {
                                    _trophyGroup.Open();
                                    _trophyMenu.Open();
                                    MonoMain.pauseMenu = _trophyGroup;
                                    SFX.Play("pause", 0.6f, -0.2f);
                                    _showedEndMenu = true;
                                }
                                if (_restart.value)
                                {
                                    _restarting = true;
                                    SFX.Play("resume", 0.6f);
                                }
                                else
                                {
                                    if (!_quit.value)
                                        return;
                                    _fading = true;
                                    SFX.Play("resume", 0.6f);
                                }
                            }
                        }
                    }
                    else
                    {
                        _waitSpawn -= 0.06f;
                        if (_waitSpawn > 0.0)
                            return;
                        if (_pendingSpawns != null && _pendingSpawns.Count > 0)
                        {
                            _waitSpawn = 0.5f;
                            Duck pendingSpawn = _pendingSpawns[0];
                            AddThing(pendingSpawn);
                            _pendingSpawns.RemoveAt(0);
                            Vec3 color = pendingSpawn.profile.persona.color;
                            Level.Add(new SpawnLine(pendingSpawn.x, pendingSpawn.y, 0, 0f, new Color((int)color.x, (int)color.z, (int)color.z), 32f));
                            Level.Add(new SpawnLine(pendingSpawn.x, pendingSpawn.y, 0, -4f, new Color((int)color.x, (int)color.y, (int)color.z), 4f));
                            Level.Add(new SpawnLine(pendingSpawn.x, pendingSpawn.y, 0, 4f, new Color((int)color.x, (int)color.y, (int)color.z), 4f));
                            SFX.Play("pullPin", 0.7f);
                            ChallengeLevel._duck = pendingSpawn;
                            _challenge = things[typeof(ChallengeMode)].First<Thing>() as ChallengeMode;
                            if (_challenge == null)
                                return;
                            _challenge.PrepareCounts();
                            ChallengeLevel.random = _challenge.random.value;
                            _challenge.duck = pendingSpawn;
                            ChallengeLevel._timer.maxTime = TimeSpan.FromSeconds(_challenge.challenge.trophies[0].timeRequirement);
                            HUD.core._cornerDisplays.Add(new CornerDisplay() { corner = HUDCorner.BottomRight, text = "", timer = _timer, ischallenge = true }); //       HUD.AddCornerTimer(HUDCorner.BottomRight, "", _timer);
                            if (_challenge.challenge.countTargets)
                            {
                                int num = _challenge.challenge.trophies[0].targets;
                                if (num < 0 && _challenge.goalTypes != null && _challenge.goalTypes.Count > 0)
                                {
                                    num = 0;
                                    foreach (GoalType goalType in _challenge.goalTypes)
                                        num += goalType.numObjectsRemaining;
                                }
                                FieldBinding _counter = new FieldBinding(this, "targetsShot");
                                HUD.core._cornerDisplays.Add(new CornerDisplay() { corner = HUDCorner.BottomLeft, text = "@RETICULE@", counter = _counter, maxCount = num > 0 ? num : 0, animateCount = false, curCount = (int)_counter.value, realCount = (int)_counter.value, ischallenge = true }); // HUD.AddCornerCounter(HUDCorner.BottomLeft, "@RETICULE@", new FieldBinding(this, "targetsShot"), num > 0 ? num : 0);

                            }
                            if (_challenge.challenge.countGoodies)
                            {
                                MultiMap<System.Type, ISequenceItem> multiMap = new MultiMap<System.Type, ISequenceItem>();
                                foreach (ISequenceItem element in Level.current.things[typeof(ISequenceItem)])
                                {
                                    System.Type type = element.GetType();
                                    SequenceItem sequence = (element as Thing).sequence;
                                    if (sequence.isValid && sequence.type == SequenceItemType.Goody)
                                        multiMap.Add(type, element);
                                }
                                System.Type key = null;
                                int num = 0;
                                foreach (KeyValuePair<System.Type, List<ISequenceItem>> keyValuePair in (MultiMap<System.Type, ISequenceItem, List<ISequenceItem>>)multiMap)
                                {
                                    if (keyValuePair.Value.Count > num)
                                    {
                                        key = keyValuePair.Key;
                                        num = keyValuePair.Value.Count;
                                    }
                                }
                                if (key != null)
                                {
                                    ISequenceItem sequenceItem = multiMap[key][0];
                                    string text = "@STARGOODY@";
                                    switch (sequenceItem)
                                    {
                                        case LapGoody _:
                                        case InvisiGoody _:
                                            text = "@LAPGOODY@";
                                            break;
                                        case SuitcaseGoody _:
                                            text = "@SUITCASEGOODY@";
                                            break;
                                        case Window _:
                                        case YellowBarrel _:
                                        case Door _:
                                            text = "@RETICULE@";
                                            break;
                                    }
                                    int goodies = _challenge.challenge.trophies[0].goodies;
                                    FieldBinding _counter = new FieldBinding(this, "goodiesGot");
                                    HUD.core._cornerDisplays.Add(new CornerDisplay() { corner = HUDCorner.BottomLeft, text = text, counter = _counter, maxCount = goodies > 0 ? goodies : 0, animateCount = false, curCount = (int)_counter.value, realCount = (int)_counter.value , ischallenge = true}); //                    HUD.AddCornerCounter(HUDCorner.BottomLeft, text, new FieldBinding(this, "goodiesGot"), goodies > 0 ? goodies : 0);

                                }
                            }
                            if (_firstStart)
                            {
                                int _max = -1;
                                foreach (TargetDuck targetDuck in things[typeof(TargetDuck)])
                                {
                                    if (targetDuck.sequence.order > _max)
                                        _max = targetDuck.sequence.order;
                                }
                                foreach (TargetDuck targetDuck in things[typeof(TargetDuck)])
                                {
                                    if (targetDuck.sequence.order == -1)
                                    {
                                        if (_max <= 0)
                                            targetDuck.sequence.order = Rando.Int(byte.MaxValue);
                                        else
                                            targetDuck.sequence.order = Rando.Int(_max);
                                    }
                                }
                                if (things[typeof(RandomControllerNew)].Count<Thing>() == 0)
                                {
                                    if (ChallengeLevel.random)
                                    {
                                        IEnumerable<Thing> thing = things[typeof(ISequenceItem)];
                                        if (thing.Count<Thing>() > 0)
                                            thing.ElementAt<Thing>(ChallengeRando.Int(thing.Count<Thing>() - 1)).sequence.BeginRandomSequence();
                                    }
                                    else
                                    {
                                        foreach (TargetDuck targetDuck in things[typeof(TargetDuck)])
                                        {
                                            if (targetDuck.sequence.order == 0)
                                                targetDuck.sequence.Activate();
                                        }
                                    }
                                }
                                _firstStart = false;
                            }
                            if (!Music.stopped)
                                return;
                            if ((string)_challenge.music == "")
                                Music.Load("Challenging");
                            else if ((string)_challenge.music == "donutmystery")
                                Music.Load("spacemystery");
                            else
                                Music.Load(Music.FindSong((string)_challenge.music));
                        }
                        else if (!_started)
                        {
                            _waitAfterSpawn -= 0.06f;
                            if (_waitAfterSpawn > 0.0)
                                return;
                            ++_waitAfterSpawnDings;
                            if (_waitAfterSpawnDings > 2)
                            {
                                _started = true;
                                simulatePhysics = true;
                                ChallengeLevel.running = true;
                                SFX.Play("ding");
                                ChallengeLevel._timer.Start();
                                if (Music.stopped)
                                    Music.PlayLoaded();
                            }
                            else
                                SFX.Play("preStartDing");
                            _waitSpawn = 1.1f;
                        }
                        else
                        {
                            _fontFade -= 0.1f;
                            if (_fontFade < 0.0)
                                _fontFade = 0f;
                            PauseLogic();
                        }
                    }
                }
            }
        }

        public void PauseLogic()
        {
            if (Input.Pressed("START"))
            {
                _pauseGroup.Open();
                _pauseMenu.Open();
                MonoMain.pauseMenu = _pauseGroup;
                if (!_paused)
                {
                    SFX.Play("pause", 0.6f);
                    ChallengeLevel._timer.Stop();
                    _paused = true;
                }
                simulatePhysics = false;
            }
            else
            {
                if (!_paused || MonoMain.pauseMenu != null)
                    return;
                _paused = false;
                SFX.Play("resume", 0.6f);
                _waitAfterSpawn = 1f;
                _waitAfterSpawnDings = 0;
                _started = false;
                _fontFade = 1f;
                if (_restart.value)
                {
                    _restarting = true;
                }
                else
                {
                    if (!_quit.value)
                        return;
                    _fading = true;
                }
            }
        }

        public override void PostDrawLayer(Layer layer)
        {
            if (layer == Layer.HUD && (!_started || _finished) && _waitAfterSpawnDings > 0 && _fontFade > 0.01f)
            {
                _font.scale = new Vec2(2f, 2f);
                _font.alpha = _fontFade;
                string text = "GET";
                if (_finished)
                    text = !_win ? "LOSE!" : (!_developer ? "WIN!" : "WOAH!");
                else if (_waitAfterSpawnDings == 2)
                    text = "READY";
                else if (_waitAfterSpawnDings == 3)
                    text = "";
                float width = _font.GetWidth(text);
                float num = 1f;
                _font.Draw(text, (Layer.HUD.camera.width / 2f - width / 2f) - num, (Layer.HUD.camera.height / 2f - _font.height / 2f) - num, Color.Black, (Depth)0.9f);
                _font.Draw(text, (Layer.HUD.camera.width / 2f - width / 2f) - num, (Layer.HUD.camera.height / 2f - _font.height / 2f) + num, Color.Black, (Depth)0.9f);
                _font.Draw(text, (Layer.HUD.camera.width / 2f - width / 2f) + num, (Layer.HUD.camera.height / 2f - _font.height / 2f) - num, Color.Black, (Depth)0.9f);
                _font.Draw(text, (Layer.HUD.camera.width / 2f - width / 2f) + num, (Layer.HUD.camera.height / 2f - _font.height / 2f) + num, Color.Black, (Depth)0.9f);
                _font.Draw(text, (Layer.HUD.camera.width / 2f - width / 2f), (Layer.HUD.camera.height / 2f - _font.height / 2f), Color.White, (Depth)1f);
            }
            base.PostDrawLayer(layer);
        }
    }
}
