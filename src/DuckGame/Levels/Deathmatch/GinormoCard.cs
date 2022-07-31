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
            this._smallMode = smallMode;
            this.layer = GinormoBoard.boardLayer;
            this._start = start;
            this._end = end;
            this._slideWait = slideWait;
            this.position = this._start;
            this._team = team;
            this.depth = (Depth)0.98f;
            this.index = idx;
            this._font = new BitmapFont("biosFont", 8);
            this._smallFont = new BitmapFont("smallBiosFont", 7, 6);
            this._mode = mode;
            this._faceTarget = !this._smallMode ? new RenderTarget2D(104, 24) : new RenderTarget2D(104, 12);
            this._targetSprite = new Sprite(this._faceTarget, 0f, 0f);
            this._gradient = new Sprite("rockThrow/headGradient2");
            if (this._smallMode)
            {
                this._edgeOverlay = new Sprite("rockThrow/edgeOverlayShort");
                this._trophy = new Sprite("tinyTrophy");
            }
            else
            {
                this._edgeOverlay = new Sprite("rockThrow/edgeOverlay");
                this._trophy = new Sprite("littleTrophy");
            }
            this._trophy.CenterOrigin();
        }

        public override void Update()
        {
            if (_slideWait < 0.0)
                this.position = Vec2.Lerp(this.position, this._end, 0.15f);
            this._slideWait -= 0.4f;
            DuckGame.Graphics.SetRenderTarget(this._faceTarget);
            DuckGame.Graphics.Clear(Color.Transparent);
            DuckGame.Graphics.screen.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.DepthRead, RasterizerState.CullNone, null, Matrix.Identity);
            this._gradient.depth = -0.6f;
            this._gradient.alpha = 0.5f;
            if (this._team.activeProfiles.Count == 1)
            {
                this._gradient.color = this._team.activeProfiles[0].persona.colorUsable;
            }
            else
            {
                switch (Teams.CurrentGameTeamIndex(this._team))
                {
                    case 0:
                        this._gradient.color = Color.Red;
                        break;
                    case 1:
                        this._gradient.color = Color.Blue;
                        break;
                    case 2:
                        this._gradient.color = Color.LimeGreen;
                        break;
                }
            }
            if (this._smallMode)
                this._gradient.yscale = 0.5f;
            DuckGame.Graphics.Draw(this._gradient, 0f, 0f);
            this._edgeOverlay.depth = (Depth)0.9f;
            this._edgeOverlay.alpha = 0.5f;
            DuckGame.Graphics.Draw(this._edgeOverlay, 0f, 0f);
            int num = 0;
            foreach (Profile activeProfile in this._team.activeProfiles)
            {
                float x = (num * 8 + 8) * 2;
                float y = 16f;
                activeProfile.persona.quackSprite.depth = (Depth)0.7f;
                activeProfile.persona.quackSprite.scale = new Vec2(2f, 2f);
                if (this._smallMode)
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
                if (this._smallMode)
                    DuckGame.Graphics.Draw(hat, hat.frame, (float)((double)x + hatPoint.x - 8.0), (float)((double)y + hatPoint.y - 8.0));
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
            this._font.scale = new Vec2(1f, 1f);
            string str = this._team.currentDisplayName;
            float num1 = 0f;
            float num2 = 0f;
            if (str.Length > 16)
                str = str.Substring(0, 16);
            string text1 = "@ICONGRADIENT@" + str;
            if (this._team != null && this._team.activeProfiles != null && this._team.activeProfiles.Count > 0)
            {
                BitmapFont bitmapFont = this._team.activeProfiles.Count <= 1 ? this._team.activeProfiles[0].font : Profiles.EnvironmentProfile.font;
                bitmapFont.scale = new Vec2(1f, 1f);
                bitmapFont.Draw(text1, this.x + 182f + num1 - bitmapFont.GetWidth(text1), this.y + 2f + num2, Color.White, this.depth);
            }
            this._font.scale = new Vec2(1f, 1f);
            this._targetSprite.scale = new Vec2(1f, 1f);
            DuckGame.Graphics.Draw(this._targetSprite, this.x, this.y);
            if (this._mode == BoardMode.Points)
            {
                string text2 = Change.ToString(_team.score);
                if (this._smallMode)
                {
                    this._smallFont.scale = new Vec2(1f, 1f);
                    this._smallFont.Draw(text2, this.x + 32f - this._smallFont.GetWidth(text2), this.y + 2f, Color.White, this.depth);
                }
                else
                {
                    this._smallFont.scale = new Vec2(2f, 2f);
                    this._smallFont.Draw(text2, this.x + 183f - this._smallFont.GetWidth(text2), this.y + 10f, Color.White, this.depth);
                }
            }
            else
            {
                int wins = this._team.wins;
                if (this._team.activeProfiles.Count == 1)
                    wins = this._team.activeProfiles[0].wins;
                for (int index = 0; index < wins; ++index)
                {
                    if (this._smallMode)
                    {
                        this._trophy.depth = (Depth)(0.8f + index * 0.01f);
                        DuckGame.Graphics.Draw(this._trophy, this.x + 24f + index * 6, this.y + 6f);
                    }
                    else
                    {
                        this._trophy.depth = (Depth)(0.8f - index * 0.01f);
                        DuckGame.Graphics.Draw(this._trophy, this.x + 175f - index * 8, this.y + 18f);
                    }
                }
            }
        }
    }
}
