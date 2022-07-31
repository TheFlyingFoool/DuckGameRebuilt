// Decompiled with JetBrains decompiler
// Type: DuckGame.GinormoBoard
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using Microsoft.Xna.Framework.Graphics;

namespace DuckGame
{
    public class GinormoBoard : Thing
    {
        private Sprite _board;
        private Sprite _boardTop;
        private Sprite _boardBottom;
        private GinormoScreen _screen;
        private SpriteMap _lighting;
        private bool _activated;
        private BoardMode _mode;
        private Vec2 _pos;
        private Layer boardLightingLayer;
        private Layer overlayLayer;
        private bool _smallMode;

        public bool activated => this._activated;

        public static Layer boardLayer
        {
            get => MonoMain.core.ginormoBoardLayer;
            set => MonoMain.core.ginormoBoardLayer = value;
        }

        public GinormoBoard(float xpos, float ypos, BoardMode mode, bool smallMode)
          : base(xpos, ypos)
        {
            this._smallMode = smallMode;
            this._board = new Sprite("rockThrow/boardMiddle");
            this._board.center = new Vec2(this._board.w / 2, this._board.h / 2 - 30);
            this._lighting = new SpriteMap("rockThrow/lighting", 191, 23)
            {
                frame = 1
            };
            this.boardLightingLayer = new Layer("LIGHTING", -85)
            {
                blend = new BlendState()
                {
                    ColorSourceBlend = Blend.Zero,
                    ColorDestinationBlend = Blend.SourceColor,
                    ColorBlendFunction = BlendFunction.Add,
                    AlphaSourceBlend = Blend.Zero,
                    AlphaDestinationBlend = Blend.SourceColor,
                    AlphaBlendFunction = BlendFunction.Add
                }
            };
            Layer.Add(this.boardLightingLayer);
            BoardLighting boardLighting = new BoardLighting(this.x + 0.5f, this.y - 125f)
            {
                layer = this.boardLightingLayer
            };
            Level.Add(boardLighting);
            if (RockWeather.weather == Weather.Snowing)
            {
                this._boardTop = new Sprite("rockThrow/boardTopSnow");
                this._boardBottom = new Sprite("rockThrow/boardBottomSnow");
            }
            else
            {
                this._boardTop = new Sprite("rockThrow/boardTop");
                this._boardBottom = new Sprite("rockThrow/boardBottom");
            }
            this._boardTop.center = new Vec2(this._boardTop.w / 2, this._boardTop.h / 2 - 30);
            this._boardBottom.center = new Vec2(this._boardBottom.w / 2, this._boardBottom.h / 2 - 30);
            this.layer = Layer.Background;
            this._pos = new Vec2(xpos, ypos);
            this._mode = mode;
            GinormoBoard.boardLayer = new Layer("BOARD", -85, targetLayer: true, targetSize: new Vec2(GinormoScreen.GetSize(this._smallMode).x, GinormoScreen.GetSize(this._smallMode).y))
            {
                camera = new Camera(0f, 0f, GinormoScreen.GetSize(this._smallMode).x, GinormoScreen.GetSize(this._smallMode).y),
                targetOnly = true,
                targetClearColor = new Color(0.05f, 0.05f, 0.05f)
            };
            Layer.Add(GinormoBoard.boardLayer);
            this.overlayLayer = new Layer("OVERLAY", 10);
            Layer.Add(this.overlayLayer);
            GinormoOverlay ginormoOverlay = new GinormoOverlay(this.x - 182f, this.y - 65f, this._smallMode)
            {
                z = -130f,
                position = this.position,
                layer = this.overlayLayer
            };
            Level.Add(ginormoOverlay);
        }

        public void Activate()
        {
            if (this._activated)
                return;
            this._screen = new GinormoScreen(0f, 0f, this._mode);
            Level.Add(_screen);
            this._activated = true;
        }

        public override void Draw()
        {
            this.boardLightingLayer.perspective = true;
            this.boardLightingLayer.projection = Layer.Background.projection;
            this.boardLightingLayer.view = Layer.Background.view;
            this.overlayLayer.perspective = true;
            this.overlayLayer.projection = Layer.Game.projection;
            this.overlayLayer.view = Layer.Game.view;
            this.overlayLayer.camera = Layer.Game.camera;
            this.boardLightingLayer.colorAdd = new Vec3(1f - RockWeather.lightOpacity);
            this._lighting.frame = RockWeather.lightOpacity <= 0.01f ? 0 : 1;
            this._board.depth = this.depth;
            DuckGame.Graphics.Draw(this._board, this.x, this.y - 12f);
            DuckGame.Graphics.Draw(this._boardBottom, this.x, this.y + 58f);
            DuckGame.Graphics.Draw(this._boardTop, this.x, this.y - 68f);
            if (!RockScoreboard._sunEnabled)
                return;
            DuckGame.Graphics.Draw(_lighting, this.x - 95f, this.y - 67f);
        }
    }
}
