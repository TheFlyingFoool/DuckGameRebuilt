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

        public bool activated => _activated;

        public static Layer boardLayer
        {
            get => MonoMain.core.ginormoBoardLayer;
            set => MonoMain.core.ginormoBoardLayer = value;
        }

        public GinormoBoard(float xpos, float ypos, BoardMode mode, bool smallMode)
          : base(xpos, ypos)
        {
            _smallMode = smallMode;
            _board = new Sprite("rockThrow/boardMiddle");
            _board.center = new Vec2(_board.w / 2, _board.h / 2 - 30);
            _lighting = new SpriteMap("rockThrow/lighting", 191, 23)
            {
                frame = 1
            };
            boardLightingLayer = new Layer("LIGHTING", -85)
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
            Layer.Add(boardLightingLayer);
            BoardLighting boardLighting = new BoardLighting(x + 0.5f, y - 125f)
            {
                layer = boardLightingLayer
            };
            Level.Add(boardLighting);
            if (RockWeather.weather == Weather.Snowing)
            {
                _boardTop = new Sprite("rockThrow/boardTopSnow");
                _boardBottom = new Sprite("rockThrow/boardBottomSnow");
            }
            else
            {
                _boardTop = new Sprite("rockThrow/boardTop");
                _boardBottom = new Sprite("rockThrow/boardBottom");
            }
            _boardTop.center = new Vec2(_boardTop.w / 2, _boardTop.h / 2 - 30);
            _boardBottom.center = new Vec2(_boardBottom.w / 2, _boardBottom.h / 2 - 30);
            layer = Layer.Background;
            _pos = new Vec2(xpos, ypos);
            _mode = mode;
            GinormoBoard.boardLayer = new Layer("BOARD", -85, targetLayer: true, targetSize: new Vec2(GinormoScreen.GetSize(_smallMode).x, GinormoScreen.GetSize(_smallMode).y))
            {
                camera = new Camera(0f, 0f, GinormoScreen.GetSize(_smallMode).x, GinormoScreen.GetSize(_smallMode).y),
                targetOnly = true,
                targetClearColor = new Color(0.05f, 0.05f, 0.05f)
            };
            Layer.Add(GinormoBoard.boardLayer);
            overlayLayer = new Layer("OVERLAY", 10);
            Layer.Add(overlayLayer);
            GinormoOverlay ginormoOverlay = new GinormoOverlay(x - 182f, y - 65f, _smallMode)
            {
                z = -130f,
                position = position,
                layer = overlayLayer
            };
            Level.Add(ginormoOverlay);
        }

        public void Activate()
        {
            if (_activated)
                return;
            _screen = new GinormoScreen(0f, 0f, _mode);
            Level.Add(_screen);
            _activated = true;
        }

        public override void Draw()
        {
            boardLightingLayer.perspective = true;
            boardLightingLayer.projection = Layer.Background.projection;
            boardLightingLayer.view = Layer.Background.view;
            overlayLayer.perspective = true;
            overlayLayer.projection = Layer.Game.projection;
            overlayLayer.view = Layer.Game.view;
            overlayLayer.camera = Layer.Game.camera;
            boardLightingLayer.colorAdd = new Vec3(1f - RockWeather.lightOpacity);
            _lighting.frame = RockWeather.lightOpacity <= 0.01f ? 0 : 1;
            _board.depth = depth;
            DuckGame.Graphics.Draw(_board, x, y - 12f);
            DuckGame.Graphics.Draw(_boardBottom, x, y + 58f);
            DuckGame.Graphics.Draw(_boardTop, x, y - 68f);
            if (!RockScoreboard._sunEnabled)
                return;
            DuckGame.Graphics.Draw(_lighting, x - 95f, y - 67f);
        }
    }
}
