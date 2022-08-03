// Decompiled with JetBrains decompiler
// Type: DuckGame.GinormoCard
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace DuckGame
{
    public class GinormoCard : Thing
    {
        private float _slideWait;
        private Vec2 _start;
        private Vec2 _end;
        private List<SpriteMap> _sprites = new List<SpriteMap>();
        private Team _team;
        private BitmapFont _font;
        private BitmapFont _smallFont;
        private BoardMode _mode;
        private Sprite _trophy;
        private RenderTarget2D _faceTarget;
        private Sprite _targetSprite;
        private Sprite _gradient;
        private Sprite _edgeOverlay;
        private int index;
        private bool _smallMode;

        public GinormoCard(
          float slideWait,
          Vec2 start,
          Vec2 end,
          Team team,
          BoardMode mode,
          int idx,
          bool smallMode)
          : base()
        {
            _smallMode = smallMode;
            layer = GinormoBoard.boardLayer;
            _start = start;
            _end = end;
            _slideWait = slideWait;
            position = _start;
            _team = team;
            depth = (Depth)0.98f;
            index = idx;
            _font = new BitmapFont("biosFont", 8);
            _smallFont = new BitmapFont("smallBiosFont", 7, 6);
            _mode = mode;
            _faceTarget = !_smallMode ? new RenderTarget2D(104, 24) : new RenderTarget2D(104, 12);
            _targetSprite = new Sprite(_faceTarget, 0f, 0f);
            _gradient = new Sprite("rockThrow/headGradient2");
            if (_smallMode)
            {
                _edgeOverlay = new Sprite("rockThrow/edgeOverlayShort");
                _trophy = new Sprite("tinyTrophy");
            }
            else
            {
                _edgeOverlay = new Sprite("rockThrow/edgeOverlay");
                _trophy = new Sprite("littleTrophy");
            }
            _trophy.CenterOrigin();
        }

        public override void Update()
        {
            if (_slideWait < 0.0)
                position = Vec2.Lerp(position, _end, 0.15f);
            _slideWait -= 0.4f;
            DuckGame.Graphics.SetRenderTarget(_faceTarget);
            DuckGame.Graphics.Clear(Color.Transparent);
            DuckGame.Graphics.screen.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.DepthRead, RasterizerState.CullNone, null, Matrix.Identity);
            _gradient.depth = -0.6f;
            _gradient.alpha = 0.5f;
            if (_team.activeProfiles.Count == 1)
            {
                _gradient.color = _team.activeProfiles[0].persona.colorUsable;
            }
            else
            {
                switch (Teams.CurrentGameTeamIndex(_team))
                {
                    case 0:
                        _gradient.color = Color.Red;
                        break;
                    case 1:
                        _gradient.color = Color.Blue;
                        break;
                    case 2:
                        _gradient.color = Color.LimeGreen;
                        break;
                }
            }
            if (_smallMode)
                _gradient.yscale = 0.5f;
            DuckGame.Graphics.Draw(_gradient, 0f, 0f);
            _edgeOverlay.depth = (Depth)0.9f;
            _edgeOverlay.alpha = 0.5f;
            DuckGame.Graphics.Draw(_edgeOverlay, 0f, 0f);
            int num = 0;
            foreach (Profile activeProfile in _team.activeProfiles)
            {
                float x = (num * 8 + 8) * 2;
                float y = 16f;
                activeProfile.persona.quackSprite.depth = (Depth)0.7f;
                activeProfile.persona.quackSprite.scale = new Vec2(2f, 2f);
                if (_smallMode)
                    DuckGame.Graphics.Draw(activeProfile.persona.quackSprite, 0, x - 8f, y - 8f);
                else
                    DuckGame.Graphics.Draw(activeProfile.persona.quackSprite, 0, x, y, 2f, 2f);
                activeProfile.persona.quackSprite.color = Color.White;
                activeProfile.persona.quackSprite.scale = new Vec2(1f, 1f);
                Vec2 hatPoint = DuckRig.GetHatPoint(activeProfile.persona.sprite.imageIndex);
                SpriteMap hat = activeProfile.team.GetHat(activeProfile.persona);
                hat.depth = (Depth)0.8f;
                hat.center = new Vec2(16f, 16f) + activeProfile.team.hatOffset;
                hat.scale = new Vec2(2f, 2f);
                if (hat.texture.width > 16.0)
                    hat.frame = 1;
                if (_smallMode)
                    DuckGame.Graphics.Draw(hat, hat.frame, (float)(x + hatPoint.x - 8.0), (float)(y + hatPoint.y - 8.0));
                else
                    DuckGame.Graphics.Draw(hat, hat.frame, x + hatPoint.x * 2f, y + hatPoint.y * 2f, 2f, 2f);
                hat.color = Color.White;
                hat.scale = new Vec2(1f, 1f);
                hat.frame = 0;
                ++num;
            }
            DuckGame.Graphics.screen.End();
            DuckGame.Graphics.SetRenderTarget(null);
            base.Update();
        }

        public override void Draw()
        {
            _font.scale = new Vec2(1f, 1f);
            string str = _team.currentDisplayName;
            float num1 = 0f;
            float num2 = 0f;
            if (str.Length > 16)
                str = str.Substring(0, 16);
            string text1 = "@ICONGRADIENT@" + str;
            if (_team != null && _team.activeProfiles != null && _team.activeProfiles.Count > 0)
            {
                BitmapFont bitmapFont = _team.activeProfiles.Count <= 1 ? _team.activeProfiles[0].font : Profiles.EnvironmentProfile.font;
                bitmapFont.scale = new Vec2(1f, 1f);
                bitmapFont.Draw(text1, x + 182f + num1 - bitmapFont.GetWidth(text1), y + 2f + num2, Color.White, depth);
            }
            _font.scale = new Vec2(1f, 1f);
            _targetSprite.scale = new Vec2(1f, 1f);
            DuckGame.Graphics.Draw(_targetSprite, x, y);
            if (_mode == BoardMode.Points)
            {
                string text2 = Change.ToString(_team.score);
                if (_smallMode)
                {
                    _smallFont.scale = new Vec2(1f, 1f);
                    _smallFont.Draw(text2, x + 32f - _smallFont.GetWidth(text2), y + 2f, Color.White, depth);
                }
                else
                {
                    _smallFont.scale = new Vec2(2f, 2f);
                    _smallFont.Draw(text2, x + 183f - _smallFont.GetWidth(text2), y + 10f, Color.White, depth);
                }
            }
            else
            {
                int wins = _team.wins;
                if (_team.activeProfiles.Count == 1)
                    wins = _team.activeProfiles[0].wins;
                for (int index = 0; index < wins; ++index)
                {
                    if (_smallMode)
                    {
                        _trophy.depth = (Depth)(0.8f + index * 0.01f);
                        DuckGame.Graphics.Draw(_trophy, x + 24f + index * 6, y + 6f);
                    }
                    else
                    {
                        _trophy.depth = (Depth)(0.8f - index * 0.01f);
                        DuckGame.Graphics.Draw(_trophy, x + 175f - index * 8, y + 18f);
                    }
                }
            }
        }
    }
}
