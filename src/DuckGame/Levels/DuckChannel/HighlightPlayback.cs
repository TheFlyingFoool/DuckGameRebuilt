// Decompiled with JetBrains decompiler
// Type: DuckGame.HighlightPlayback
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
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
            this._font = new BitmapFont("biosFont", 8);
            this._currentHighlight = highlight;
            this._highlights = Highlights.GetHighlights();
            while (this._currentHighlight >= this._highlights.Count)
                --this._currentHighlight;
            this._numbers = new SpriteMap("newscast/numberfont", 25, 22);
        }

        public override void Initialize()
        {
            this._tv = new Sprite("bigTV");
            Vote.OpenVoting("SKIP", "START");
        }

        public override void Update()
        {
            if (!this._skip && Vote.Passed(VoteType.Skip))
                this._skip = true;
            if (this._skip)
                this._fadeOut = true;
            Graphics.fade = Lerp.Float(Graphics.fade, this._fadeOut ? 0f : 1f, 0.02f);
            if ((double)Graphics.fade < 0.01f && this._skip)
            {
                HighlightLevel.didSkip = true;
                Vote.CloseVoting();
                Level.current = new RockScoreboard(RockScoreboard.returnLevel, ScoreBoardMode.ShowWinner, true); //!Main.isDemo ? new RockScoreboard(RockScoreboard.returnLevel, ScoreBoardMode.ShowWinner, true) : new HighlightLevel(true);
            }
            if (!this._showHighlight && Graphics.fade > 0.95f)
            {
                this._waitToShow -= 0.02f;
                if (_waitToShow <= 0f)
                {
                    this._waitToShow = 0f;
                    this._fadeOut = true;
                }
            }
            if (Graphics.fade < 0.01f && !this._showHighlight && this._fadeOut)
            {
                this._fadeOut = false;
                this._showHighlight = true;
            }
            if (this._showHighlight && (double)Graphics.fade > 0.95f)
                this._keepPaused -= 0.03f;
            if (this._currentHighlight >= 0 && !this._highlights[this._currentHighlight].finished)
                return;
            this._endWait -= 0.03f;
            if (_endWait > 0f)
                return;
            this._fadeOut = true;
            if ((double)Graphics.fade >= 0.01f)
                return;
            int highlight = this._currentHighlight - 1;
            if (this._currentHighlight <= 0)
                Level.current = new HighlightLevel(true);
            else
                Level.current = new HighlightPlayback(highlight);
        }

        public override void DoDraw()
        {
            DuckGame.Graphics.Clear(Color.Black);
            base.DoDraw();
        }

        public override void Draw()
        {
        }

        public override void BeforeDraw()
        {
            if (!this._showHighlight || this._currentHighlight < 0)
                return;
            if (_keepPaused > 0f)
                this._highlights[this._currentHighlight].frame = this._highlights[this._currentHighlight].startFrame + 5;
            this._highlights[this._currentHighlight].RenderFrame();
            if (_keepPaused > 0f || this._highlights[this._currentHighlight].finished)
                return;
            this._highlights[this._currentHighlight].UpdateFrame();
            this._highlights[this._currentHighlight].IncrementFrame();
        }

        public override void AfterDrawLayers()
        {
            if (_keepPaused <= 0f || this._currentHighlight < 0)
                return;
            DuckGame.Graphics.screen.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Resolution.getTransformationMatrix());
            this._font.scale = new Vec2(8f, 8f);
            double width = this._font.GetWidth(Change.ToString(this._currentHighlight + 1));
            double height = this._font.height;
            this._numbers.frame = 4 - this._currentHighlight;
            this._numbers.depth = (Depth)1f;
            this._numbers.scale = new Vec2(4f, 4f);
            DuckGame.Graphics.Draw(_numbers, 32f, 32f);
            DuckGame.Graphics.screen.End();
        }
    }
}
