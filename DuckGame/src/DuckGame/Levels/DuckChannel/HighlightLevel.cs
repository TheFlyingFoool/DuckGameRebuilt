// Decompiled with JetBrains decompiler
// Type: DuckGame.HighlightLevel
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace DuckGame
{
    public class HighlightLevel : Level
    {
        private Sprite _pumpkin;
        private Sprite _tv;
        private Sprite _logo;
        private Sprite _rockImage2;
        private Sprite _background;
        private Sprite _newsTable;
        private SpriteMap _duck;
        private SpriteMap _duckBeak;
        private SpriteMap _tie;
        private float _done = 1f;
        //private Vec2 _tl;
        //private Vec2 _size;
        private float _waitZoom = 1f;
        //private Vec2 _imageDraw = Vec2.Zero;
        private float _tvFade = 1f;
        private TVState _state;
        private TVState _desiredState;
        private Vec2 _cameraOffset = new Vec2(0f, 0f);
        private Teleprompter _talker;
        public static bool didSkip;
        private bool _firedSkipLogic;
        private bool _endOfHighlights;
        private bool _testMode;
        public static int currentTie;
        public static List<DuckStory> _stories;
        private Layer _blurLayer;
        private static Sprite _image;
        private DuckChannelLogo _transition;
        private HotnessAnimation _hotness;
        private int _interviewIndex;
        private bool _skip;
        public static bool _cancelSkip;
        private bool _askedQuestion;
        private float _interviewWait = 1f;
        private float _wait;

        public HighlightLevel(bool endOfHighlights = false, bool testMode = false)
        {
            _centeredView = true;
            _endOfHighlights = endOfHighlights;
            _testMode = testMode;
        }

        public void OnHotnessImage(DuckStory story)
        {
            story.OnStoryBegin -= new DuckStory.OnStoryBeginDelegate(OnHotnessImage);
            _image = new Sprite("newscast/hotnessImage");
        }

        public void OnInterviewImage(DuckStory story)
        {
            story.OnStoryBegin -= new DuckStory.OnStoryBeginDelegate(OnInterviewImage);
            _image = new SpriteMap("interviewBox", 63, 47);
        }

        public void OnHotnessStory(DuckStory story)
        {
            story.OnStoryBegin -= new DuckStory.OnStoryBeginDelegate(OnHotnessStory);
            _desiredState = TVState.ShowHotness;
            _talker.Pause();
        }

        public void OnHotnessEnd(DuckStory story)
        {
            story.OnStoryBegin -= new DuckStory.OnStoryBeginDelegate(OnHotnessEnd);
            _desiredState = TVState.ShowNewscaster;
            _image = null;
        }

        public void OnInterview(DuckStory story)
        {
            story.OnStoryBegin -= new DuckStory.OnStoryBeginDelegate(OnHotnessStory);
            _desiredState = TVState.ShowInterview;
            _talker.Pause();
        }

        public override void Initialize()
        {
            if (_testMode)
            {
                _endOfHighlights = true;
                Options.Data.sfxVolume = 0f;
                DuckStory duckStory = new DuckStory
                {
                    text = "|SUAVE||RED|John Mallard|WHITE| here dancing|CALM| for you |EXCITED|and wearing ties!"
                };
                _stories = new List<DuckStory>();
                for (int index = 0; index < 9999; ++index)
                    _stories.Add(duckStory);
            }
            _cancelSkip = false;
            _tv = new Sprite("bigTV");
            _duck = new SpriteMap("newsDuck", 140, 100);
            _duckBeak = new SpriteMap("newsDuckBeak", 140, 100);
            _tie = new SpriteMap("ties", 12, 21);
            _pumpkin = new Sprite("pump");
            _pumpkin.CenterOrigin();
            _newsTable = new Sprite("newsTable");
            _logo = new Sprite("duckGameTitle");
            _logo.CenterOrigin();
            _background = new Sprite("duckChannelBackground");
            _blurLayer = new Layer("BLUR", Layer.HUD.depth + 5, Layer.HUD.camera);
            Layer.Add(_blurLayer);
            _blurLayer.effect = (Effect)Content.Load<MTEffect>("Shaders/blur");
            _transition = new DuckChannelLogo();
            Add(_transition);
            //this._tl = new Vec2(30f, 32f);
            //this._size = new Vec2(207f, 141f);
            _rockImage2 = new Sprite(RockScoreboard.finalImage, 0f, 0f);
            _talker = new Teleprompter(0f, 0f, _duck);
            _talker.active = _talker.visible = false;
            Add(_talker);
            if (didSkip)
                _skip = true;
            if (_endOfHighlights)
            {
                _state = TVState.ShowNewscaster;
                _desiredState = _state;
            }
            else
            {
                _image = null;
                currentTie = Rando.Int(15);
                Music.Play("SportsCap");
                _stories = DuckNews.CalculateStories();
            }
            _hotness = new HotnessAnimation();
            _tie.frame = currentTie;
            for (int index = 0; index < _stories.Count; index = index - 1 + 1)
            {
                int num = _stories[index].text == "%CUEHIGHLIGHTS%" ? 1 : 0;
                if (_stories[index].text == "CUE%HOTNESSIMAGE%")
                    _stories[index].OnStoryBegin += new DuckStory.OnStoryBeginDelegate(OnHotnessImage);
                if (_stories[index].text == "CUE%CUEHOTNESS%")
                    _stories[index].OnStoryBegin += new DuckStory.OnStoryBeginDelegate(OnHotnessStory);
                if (_stories[index].text == "CUE%ENDHOTNESS%")
                    _stories[index].OnStoryBegin += new DuckStory.OnStoryBeginDelegate(OnHotnessEnd);
                if (_stories[index].text == "CUE%INTERVIEWIMAGE%")
                    _stories[index].OnStoryBegin += new DuckStory.OnStoryBeginDelegate(OnInterviewImage);
                if (_stories[index].text == "CUE%CUEINTERVIEW%")
                {
                    _interviewIndex = index;
                    _stories[index].OnStoryBegin += new DuckStory.OnStoryBeginDelegate(OnInterview);
                }
                if (num == 0)
                    _talker.ReadLine(_stories[index]);
                _stories.RemoveAt(index);
                if (num != 0)
                    break;
            }
            Vote.OpenVoting("SKIP", Triggers.Start);
            List<Team> teamList = new List<Team>();
            foreach (Team team in Teams.all)
            {
                if (team.activeProfiles.Count > 0)
                    teamList.Add(team);
            }
            foreach (Team team in teamList)
            {
                foreach (Profile activeProfile in team.activeProfiles)
                    Vote.RegisterVote(activeProfile, VoteType.None);
            }
        }

        public void DoSkip()
        {
            if (!_endOfHighlights)
            {
                _talker.ClearLines();
                for (int index = 0; index < _stories.Count; index = index - 1 + 1)
                {
                    int num = _stories[index].text == "%CUEHIGHLIGHTS%" ? 1 : 0;
                    if (num == 0)
                        _talker.ReadLine(_stories[index]);
                    _stories.RemoveAt(index);
                    if (num != 0)
                        break;
                }
            }
            _talker.SkipToClose();
        }

        public override void Update()
        {
            if (_testMode)
            {
                _wait += Maths.IncFrameTimer();
                if (Keyboard.Pressed(Keys.F5) || _wait > 0.1f)
                {
                    _wait = 0f;
                    try
                    {
                        _tie = new SpriteMap((Tex2D)ContentPack.LoadTexture2D("tieTest.png"), 64, 64)
                        {
                            center = new Vec2(26f, 27f)
                        };
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            Graphics.fadeAdd = Lerp.Float(Graphics.fadeAdd, 0f, 0.01f);
            //if (Main.isDemo && this._skip && !this._firedSkipLogic)
            //{
            //    this._firedSkipLogic = true;
            //    Vote.CloseVoting();
            //    HUD.CloseAllCorners();
            //    this.DoSkip();
            //}
            if (Graphics.fade > 0.99f && !_skip && Vote.Passed(VoteType.Skip))
                _skip = true;
            if (_talker.finished || !_cancelSkip && _skip)// && !Main.isDemo
                _done -= 0.04f;
            Graphics.fade = Lerp.Float(Graphics.fade, _done < 0f ? 0f : 1f, 0.02f);
            if (Graphics.fade < 0.01f && (_talker.finished || _skip))
            {
                if (_endOfHighlights || _skip)
                {
                    Vote.CloseVoting();
                    current = new RockScoreboard(RockScoreboard.returnLevel, ScoreBoardMode.ShowWinner, true);
                }
                else
                    current = new HighlightPlayback(4);
            }
            if (_state == TVState.ShowPedestals)
            {
                _waitZoom -= 0.008f;
                if (_waitZoom < 0.01f)
                {
                    _waitZoom = 0f;
                    _desiredState = TVState.ShowNewscaster;
                }
            }
            if (_state == TVState.ShowHotness && _hotness.ready)
                _talker.Resume();
            if (_state == TVState.ShowInterview)
            {
                _interviewWait -= 0.02f;
                if (_interviewWait < 0f && !_askedQuestion)
                {
                    _talker.InsertLine(Script.winner() + "! To what do you attribute your success?", _interviewIndex);
                    _talker.Resume();
                    _askedQuestion = true;
                }
            }
            _cameraOffset.x = Lerp.Float(_cameraOffset.x, _image != null ? 20f : 0f, 2f);
            _talker.active = _talker.visible = _state != 0;
            if (_state == _desiredState)
                return;
            _talker.active = false;
            _transition.PlaySwipe();
            if (!_transition.doTransition)
                return;
            _state = _desiredState;
        }

        public override void PostDrawLayer(Layer layer)
        {
            float num1 = -20f;
            float y = -5f;
            if (layer == Layer.Game)
                Graphics.Clear(Color.Black);
            if (layer == _blurLayer)
            {
                if (_state != TVState.ShowPedestals)
                    ;
            }
            else if (layer == Layer.HUD)
            {
                if (_state == TVState.ShowPedestals)
                {
                    if (_rockImage2.texture != null)
                    {
                        float num2 = (Layer.HUD.camera.width - 0f) / _rockImage2.texture.width;
                        _rockImage2.color = new Color(_tvFade, _tvFade, _tvFade);
                        _rockImage2.scale = new Vec2(num2, num2);
                        Graphics.Draw(_rockImage2, num1 - 10f, y, (Depth)0.8f);
                    }
                }
                else if (_state == TVState.ShowNewscaster)
                {
                    _background.color = new Color(_tvFade, _tvFade, _tvFade);
                    _duck.color = new Color(_tvFade, _tvFade, _tvFade);
                    _tie.color = new Color(_tvFade, _tvFade, _tvFade);
                    _newsTable.color = new Color(_tvFade, _tvFade, _tvFade);
                    Graphics.Draw(_background, 0f + _cameraOffset.x, 3f + _cameraOffset.y, (Depth)0.5f);
                    Graphics.Draw(_newsTable, 0f + _cameraOffset.x, 116f + _cameraOffset.y, (Depth)0.6f);
                    _duck.depth = (Depth)0.8f;
                    Vec2 vec2 = new Vec2(63f + _cameraOffset.x, 35f + _cameraOffset.y);
                    Graphics.Draw(_duck, vec2.x, vec2.y);
                    if (_duck.frame == 6)
                        vec2.x -= 3f;
                    else if (_duck.frame == 7)
                        vec2.x += 3f;
                    else if (_duck.frame == 8)
                        ++vec2.x;
                    if (DG.isHalloween)
                    {
                        _pumpkin.depth = (Depth)0.81f;
                        Graphics.Draw(_pumpkin, vec2.x + 69f, vec2.y + 22f);
                    }
                    _tie.depth = (Depth)0.805f;
                    float num3 = 0f;
                    if (_duck.frame == 7)
                        num3 += 2f;
                    else if (_duck.frame == 8)
                        ++num3;
                    Graphics.Draw(_tie, 130f + _cameraOffset.x + num3, 96f + _cameraOffset.y);
                    if (!DG.isHalloween)
                    {
                        _duckBeak.depth = (Depth)0.81f;
                        _duckBeak.frame = _duck.frame;
                        Graphics.Draw(_duckBeak, 63f + _cameraOffset.x, 35f + _cameraOffset.y);
                    }
                    if (_image != null)
                    {
                        _image.depth = (Depth)0.65f;
                        if (_cameraOffset.x > 19.0)
                            Graphics.Draw(_image, 50f, 40f);
                    }
                }
                else if (_state == TVState.ShowHotness)
                    _hotness.Draw();
                else if (_state == TVState.ShowInterview)
                {
                    _image.scale = new Vec2(2f);
                    Graphics.Draw(_image, 40f, 30f);
                }
                Graphics.Draw(_tv, 0f, -10f, (Depth)0.9f);
            }
            base.PostDrawLayer(layer);
        }
    }
}
