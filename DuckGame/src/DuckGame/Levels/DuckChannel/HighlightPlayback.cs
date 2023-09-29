// Decompiled with JetBrains decompiler
// Type: DuckGame.HighlightPlayback
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace DuckGame
{
    public class HighlightPlayback : Level
    {
        private BitmapFont _font;
        private bool _fadeOut;
        private float _waitToShow = 1f;
        private int _currentHighlight;
        private float _endWait = 1f;
        private bool _showHighlight = true;
        private float _keepPaused = 1f;
        private List<Recording> _highlights;
        private Sprite _tv;
        private SpriteMap _numbers;
        private bool _skip;

        public HighlightPlayback(int highlight)
        {
            _font = new BitmapFont("biosFont", 8);
            _currentHighlight = highlight;
            _highlights = Highlights.GetHighlights();
            while (_currentHighlight >= _highlights.Count)
                --_currentHighlight;
            _numbers = new SpriteMap("newscast/numberfont", 25, 22);
        }

        public override void Initialize()
        {
            _tv = new Sprite("bigTV");
            Vote.OpenVoting("SKIP", Triggers.Start);
        }

        public override void Update()
        {
            if (!_skip && Vote.Passed(VoteType.Skip))
                _skip = true;
            if (_skip)
                _fadeOut = true;
            Graphics.fade = Lerp.Float(Graphics.fade, _fadeOut ? 0f : 1f, 0.02f);
            if (Graphics.fade < 0.01f && _skip)
            {
                HighlightLevel.didSkip = true;
                Vote.CloseVoting();
                current = new RockScoreboard(RockScoreboard.returnLevel, ScoreBoardMode.ShowWinner, true); //!Main.isDemo ? new RockScoreboard(RockScoreboard.returnLevel, ScoreBoardMode.ShowWinner, true) : new HighlightLevel(true);
            }
            if (!_showHighlight && Graphics.fade > 0.95f)
            {
                _waitToShow -= 0.02f;
                if (_waitToShow <= 0f)
                {
                    _waitToShow = 0f;
                    _fadeOut = true;
                }
            }
            if (Graphics.fade < 0.01f && !_showHighlight && _fadeOut)
            {
                _fadeOut = false;
                _showHighlight = true;
            }
            if (_showHighlight && Graphics.fade > 0.95f)
                _keepPaused -= 0.03f;
            if (_currentHighlight >= 0 && !_highlights[_currentHighlight].finished)
                return;
            _endWait -= 0.03f;
            if (_endWait > 0f)
                return;
            _fadeOut = true;
            if (Graphics.fade >= 0.01f)
                return;
            int highlight = _currentHighlight - 1;
            if (_currentHighlight <= 0)
                current = new HighlightLevel(true);
            else
                current = new HighlightPlayback(highlight);
        }

        public override void DoDraw()
        {
            Graphics.Clear(Color.Black);
            base.DoDraw();
        }

        public override void Draw()
        {
        }

        public override void BeforeDraw()
        {
            if (!_showHighlight || _currentHighlight < 0)
                return;
            if (_keepPaused > 0f)
                _highlights[_currentHighlight].frame = _highlights[_currentHighlight].startFrame + 5;
            _highlights[_currentHighlight].RenderFrame();
            if (_keepPaused > 0f || _highlights[_currentHighlight].finished)
                return;
            _highlights[_currentHighlight].UpdateFrame();
            _highlights[_currentHighlight].IncrementFrame();
        }

        public override void AfterDrawLayers()
        {
            if (_keepPaused <= 0f || _currentHighlight < 0)
                return;
            Graphics.screen.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Resolution.getTransformationMatrix());
            _font.scale = new Vec2(8f, 8f);
            double width = _font.GetWidth(Change.ToString(_currentHighlight + 1));
            double height = _font.height;
            _numbers.frame = 4 - _currentHighlight;
            _numbers.depth = (Depth)1f;
            _numbers.scale = new Vec2(4f, 4f);
            Graphics.Draw(ref _numbers, 32f, 32f);
            Graphics.screen.End();
        }
    }
}
