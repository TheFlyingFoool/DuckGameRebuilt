// Decompiled with JetBrains decompiler
// Type: DuckGame.HighlightLevel
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
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
        private Vec2 _cameraOffset = new Vec2(0.0f, 0.0f);
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
            this._centeredView = true;
            this._endOfHighlights = endOfHighlights;
            this._testMode = testMode;
        }

        public void OnHotnessImage(DuckStory story)
        {
            story.OnStoryBegin -= new DuckStory.OnStoryBeginDelegate(this.OnHotnessImage);
            HighlightLevel._image = new Sprite("newscast/hotnessImage");
        }

        public void OnInterviewImage(DuckStory story)
        {
            story.OnStoryBegin -= new DuckStory.OnStoryBeginDelegate(this.OnInterviewImage);
            HighlightLevel._image = new SpriteMap("interviewBox", 63, 47);
        }

        public void OnHotnessStory(DuckStory story)
        {
            story.OnStoryBegin -= new DuckStory.OnStoryBeginDelegate(this.OnHotnessStory);
            this._desiredState = TVState.ShowHotness;
            this._talker.Pause();
        }

        public void OnHotnessEnd(DuckStory story)
        {
            story.OnStoryBegin -= new DuckStory.OnStoryBeginDelegate(this.OnHotnessEnd);
            this._desiredState = TVState.ShowNewscaster;
            HighlightLevel._image = null;
        }

        public void OnInterview(DuckStory story)
        {
            story.OnStoryBegin -= new DuckStory.OnStoryBeginDelegate(this.OnHotnessStory);
            this._desiredState = TVState.ShowInterview;
            this._talker.Pause();
        }

        public override void Initialize()
        {
            if (this._testMode)
            {
                this._endOfHighlights = true;
                Options.Data.sfxVolume = 0.0f;
                DuckStory duckStory = new DuckStory
                {
                    text = "|SUAVE||RED|John Mallard|WHITE| here dancing|CALM| for you |EXCITED|and wearing ties!"
                };
                HighlightLevel._stories = new List<DuckStory>();
                for (int index = 0; index < 9999; ++index)
                    HighlightLevel._stories.Add(duckStory);
            }
            HighlightLevel._cancelSkip = false;
            this._tv = new Sprite("bigTV");
            this._duck = new SpriteMap("newsDuck", 140, 100);
            this._duckBeak = new SpriteMap("newsDuckBeak", 140, 100);
            this._tie = new SpriteMap("ties", 12, 21);
            this._pumpkin = new Sprite("pump");
            this._pumpkin.CenterOrigin();
            this._newsTable = new Sprite("newsTable");
            this._logo = new Sprite("duckGameTitle");
            this._logo.CenterOrigin();
            this._background = new Sprite("duckChannelBackground");
            this._blurLayer = new Layer("BLUR", Layer.HUD.depth + 5, Layer.HUD.camera);
            Layer.Add(this._blurLayer);
            this._blurLayer.effect = (Effect)Content.Load<MTEffect>("Shaders/blur");
            this._transition = new DuckChannelLogo();
            Level.Add(_transition);
            //this._tl = new Vec2(30f, 32f);
            //this._size = new Vec2(207f, 141f);
            this._rockImage2 = new Sprite(RockScoreboard.finalImage, 0.0f, 0.0f);
            this._talker = new Teleprompter(0.0f, 0.0f, this._duck);
            this._talker.active = this._talker.visible = false;
            Level.Add(_talker);
            if (HighlightLevel.didSkip)
                this._skip = true;
            if (this._endOfHighlights)
            {
                this._state = TVState.ShowNewscaster;
                this._desiredState = this._state;
            }
            else
            {
                HighlightLevel._image = null;
                HighlightLevel.currentTie = Rando.Int(15);
                Music.Play("SportsCap");
                HighlightLevel._stories = DuckNews.CalculateStories();
            }
            this._hotness = new HotnessAnimation();
            this._tie.frame = HighlightLevel.currentTie;
            for (int index = 0; index < HighlightLevel._stories.Count; index = index - 1 + 1)
            {
                int num = HighlightLevel._stories[index].text == "%CUEHIGHLIGHTS%" ? 1 : 0;
                if (HighlightLevel._stories[index].text == "CUE%HOTNESSIMAGE%")
                    HighlightLevel._stories[index].OnStoryBegin += new DuckStory.OnStoryBeginDelegate(this.OnHotnessImage);
                if (HighlightLevel._stories[index].text == "CUE%CUEHOTNESS%")
                    HighlightLevel._stories[index].OnStoryBegin += new DuckStory.OnStoryBeginDelegate(this.OnHotnessStory);
                if (HighlightLevel._stories[index].text == "CUE%ENDHOTNESS%")
                    HighlightLevel._stories[index].OnStoryBegin += new DuckStory.OnStoryBeginDelegate(this.OnHotnessEnd);
                if (HighlightLevel._stories[index].text == "CUE%INTERVIEWIMAGE%")
                    HighlightLevel._stories[index].OnStoryBegin += new DuckStory.OnStoryBeginDelegate(this.OnInterviewImage);
                if (HighlightLevel._stories[index].text == "CUE%CUEINTERVIEW%")
                {
                    this._interviewIndex = index;
                    HighlightLevel._stories[index].OnStoryBegin += new DuckStory.OnStoryBeginDelegate(this.OnInterview);
                }
                if (num == 0)
                    this._talker.ReadLine(HighlightLevel._stories[index]);
                HighlightLevel._stories.RemoveAt(index);
                if (num != 0)
                    break;
            }
            Vote.OpenVoting("SKIP", "START");
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
            if (!this._endOfHighlights)
            {
                this._talker.ClearLines();
                for (int index = 0; index < HighlightLevel._stories.Count; index = index - 1 + 1)
                {
                    int num = HighlightLevel._stories[index].text == "%CUEHIGHLIGHTS%" ? 1 : 0;
                    if (num == 0)
                        this._talker.ReadLine(HighlightLevel._stories[index]);
                    HighlightLevel._stories.RemoveAt(index);
                    if (num != 0)
                        break;
                }
            }
            this._talker.SkipToClose();
        }

        public override void Update()
        {
            if (this._testMode)
            {
                this._wait += Maths.IncFrameTimer();
                if (Keyboard.Pressed(Keys.F5) || _wait > 0.1)
                {
                    this._wait = 0.0f;
                    try
                    {
                        this._tie = new SpriteMap((Tex2D)ContentPack.LoadTexture2D("tieTest.png"), 64, 64)
                        {
                            center = new Vec2(26f, 27f)
                        };
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            DuckGame.Graphics.fadeAdd = Lerp.Float(DuckGame.Graphics.fadeAdd, 0.0f, 0.01f);
            if (Main.isDemo && this._skip && !this._firedSkipLogic)
            {
                this._firedSkipLogic = true;
                Vote.CloseVoting();
                HUD.CloseAllCorners();
                this.DoSkip();
            }
            if ((double)DuckGame.Graphics.fade > 0.990000009536743 && !this._skip && Vote.Passed(VoteType.Skip))
                this._skip = true;
            if (this._talker.finished || !HighlightLevel._cancelSkip && this._skip && !Main.isDemo)
                this._done -= 0.04f;
            DuckGame.Graphics.fade = Lerp.Float(DuckGame.Graphics.fade, _done < 0.0 ? 0.0f : 1f, 0.02f);
            if ((double)DuckGame.Graphics.fade < 0.00999999977648258 && (this._talker.finished || this._skip))
            {
                if (this._endOfHighlights || this._skip)
                {
                    Vote.CloseVoting();
                    Level.current = new RockScoreboard(RockScoreboard.returnLevel, ScoreBoardMode.ShowWinner, true);
                }
                else
                    Level.current = new HighlightPlayback(4);
            }
            if (this._state == TVState.ShowPedestals)
            {
                this._waitZoom -= 0.008f;
                if (_waitZoom < 0.00999999977648258)
                {
                    this._waitZoom = 0.0f;
                    this._desiredState = TVState.ShowNewscaster;
                }
            }
            if (this._state == TVState.ShowHotness && this._hotness.ready)
                this._talker.Resume();
            if (this._state == TVState.ShowInterview)
            {
                this._interviewWait -= 0.02f;
                if (_interviewWait < 0.0 && !this._askedQuestion)
                {
                    this._talker.InsertLine(Script.winner() + "! To what do you attribute your success?", this._interviewIndex);
                    this._talker.Resume();
                    this._askedQuestion = true;
                }
            }
            this._cameraOffset.x = Lerp.Float(this._cameraOffset.x, HighlightLevel._image != null ? 20f : 0.0f, 2f);
            this._talker.active = this._talker.visible = this._state != 0;
            if (this._state == this._desiredState)
                return;
            this._talker.active = false;
            this._transition.PlaySwipe();
            if (!this._transition.doTransition)
                return;
            this._state = this._desiredState;
        }

        public override void PostDrawLayer(Layer layer)
        {
            float num1 = -20f;
            float y = -5f;
            if (layer == Layer.Game)
                DuckGame.Graphics.Clear(Color.Black);
            if (layer == this._blurLayer)
            {
                if (this._state != TVState.ShowPedestals)
                    ;
            }
            else if (layer == Layer.HUD)
            {
                if (this._state == TVState.ShowPedestals)
                {
                    if (this._rockImage2.texture != null)
                    {
                        float num2 = (Layer.HUD.camera.width - 0.0f) / _rockImage2.texture.width;
                        this._rockImage2.color = new Color(this._tvFade, this._tvFade, this._tvFade);
                        this._rockImage2.scale = new Vec2(num2, num2);
                        DuckGame.Graphics.Draw(this._rockImage2, num1 - 10f, y, (Depth)0.8f);
                    }
                }
                else if (this._state == TVState.ShowNewscaster)
                {
                    this._background.color = new Color(this._tvFade, this._tvFade, this._tvFade);
                    this._duck.color = new Color(this._tvFade, this._tvFade, this._tvFade);
                    this._tie.color = new Color(this._tvFade, this._tvFade, this._tvFade);
                    this._newsTable.color = new Color(this._tvFade, this._tvFade, this._tvFade);
                    DuckGame.Graphics.Draw(this._background, 0.0f + this._cameraOffset.x, 3f + this._cameraOffset.y, (Depth)0.5f);
                    DuckGame.Graphics.Draw(this._newsTable, 0.0f + this._cameraOffset.x, 116f + this._cameraOffset.y, (Depth)0.6f);
                    this._duck.depth = (Depth)0.8f;
                    Vec2 vec2 = new Vec2(63f + this._cameraOffset.x, 35f + this._cameraOffset.y);
                    DuckGame.Graphics.Draw(_duck, vec2.x, vec2.y);
                    if (this._duck.frame == 6)
                        vec2.x -= 3f;
                    else if (this._duck.frame == 7)
                        vec2.x += 3f;
                    else if (this._duck.frame == 8)
                        ++vec2.x;
                    if (DG.isHalloween)
                    {
                        this._pumpkin.depth = (Depth)0.81f;
                        DuckGame.Graphics.Draw(this._pumpkin, vec2.x + 69f, vec2.y + 22f);
                    }
                    this._tie.depth = (Depth)0.805f;
                    float num3 = 0.0f;
                    if (this._duck.frame == 7)
                        num3 += 2f;
                    else if (this._duck.frame == 8)
                        ++num3;
                    DuckGame.Graphics.Draw(_tie, 130f + this._cameraOffset.x + num3, 96f + this._cameraOffset.y);
                    if (!DG.isHalloween)
                    {
                        this._duckBeak.depth = (Depth)0.81f;
                        this._duckBeak.frame = this._duck.frame;
                        DuckGame.Graphics.Draw(_duckBeak, 63f + this._cameraOffset.x, 35f + this._cameraOffset.y);
                    }
                    if (HighlightLevel._image != null)
                    {
                        HighlightLevel._image.depth = (Depth)0.65f;
                        if (_cameraOffset.x > 19.0)
                            DuckGame.Graphics.Draw(HighlightLevel._image, 50f, 40f);
                    }
                }
                else if (this._state == TVState.ShowHotness)
                    this._hotness.Draw();
                else if (this._state == TVState.ShowInterview)
                {
                    HighlightLevel._image.scale = new Vec2(2f);
                    DuckGame.Graphics.Draw(HighlightLevel._image, 40f, 30f);
                }
                DuckGame.Graphics.Draw(this._tv, 0.0f, -10f, (Depth)0.9f);
            }
            base.PostDrawLayer(layer);
        }
    }
}
