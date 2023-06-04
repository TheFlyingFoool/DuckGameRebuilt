// Decompiled with JetBrains decompiler
// Type: DuckGame.ConsoleScreen
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using Microsoft.Xna.Framework.Graphics;

namespace DuckGame
{
    public class ConsoleScreen : Thing
    {
        private Material _lcdMaterial;
        private RenderTarget2D _realScreenTarget;
        private RenderTarget2D _bloomTarget;
        private RenderTarget2D _finalTarget;
        private MTSpriteBatch _batch;
        private Material _blurMaterial;
        public float _darken = 1f;
        private HatSelector _selector;
        public bool _flashTransition;
        private Viewport _oldViewport;

        private RenderTarget2D _screenTarget
        {
            get => _selector._roomEditor.fade > 0f ? _finalTarget : _realScreenTarget;
            set => _realScreenTarget = value;
        }

        public RenderTarget2D target => _finalTarget;

        public float darken => _darken;

        public ConsoleScreen(float xpos, float ypos, HatSelector s)
          : base(xpos, ypos)
        {
            _lcdMaterial = new Material("Shaders/lcd");
            _blurMaterial = new Material("Shaders/lcdBlur");
            _screenTarget = new RenderTarget2D(134, 86);
            _bloomTarget = new RenderTarget2D(134, 86);
            _finalTarget = new RenderTarget2D(536, 344);
            _batch = new MTSpriteBatch(Graphics.device);
            _selector = s;
        }

        public bool transitioning => _flashTransition;

        public void DoFlashTransition() => _flashTransition = true;

        public void BeginDraw()
        {
            _oldViewport = Graphics.viewport;
            Graphics.SetRenderTarget(_screenTarget);
            Graphics.viewport = new Viewport(0, 0, _screenTarget.width, _screenTarget.height);
            Graphics.Clear(Color.Black);
            Graphics.screen = _batch;
            Camera camera = new Camera(3f, 4f, _screenTarget.width, _screenTarget.height);
            if (_selector._roomEditor.fade > 0f)
                camera = new Camera(3f, 4f, _screenTarget.width / 4, _screenTarget.height / 4);
            _batch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.DepthRead, RasterizerState.CullNone, null, camera.getMatrix());
        }

        public void EndDraw()
        {
            _batch.End();
            if (!_flashTransition)
            {
                Camera camera1 = new Camera(0f, 0f, _screenTarget.width, _screenTarget.height);
                if (_selector._roomEditor.fade <= 0f)
                {
                    Graphics.SetRenderTarget(_bloomTarget);
                    Graphics.viewport = new Viewport(0, 0, _bloomTarget.width, _bloomTarget.height);
                    Graphics.screen = _batch;
                    _batch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.DepthRead, RasterizerState.CullNone, (MTEffect)_blurMaterial, camera1.getMatrix());
                    Graphics.Draw(_screenTarget, Vec2.Zero, new Rectangle?(), Color.White, 0f, Vec2.Zero, new Vec2(1f, 1f), SpriteEffects.None, (Depth)1f);
                    _batch.End();
                    Graphics.SetRenderTarget(_finalTarget);
                    Graphics.viewport = new Viewport(0, 0, _finalTarget.width, _finalTarget.height);
                    Camera camera2 = new Camera(0f, 0f, _screenTarget.width, _screenTarget.height);
                    _batch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.DepthRead, RasterizerState.CullNone, (MTEffect)_lcdMaterial, camera2.getMatrix());
                    Graphics.device.Textures[1] = (Texture2D)_bloomTarget;
                    Graphics.device.SamplerStates[1] = SamplerState.LinearClamp;
                    Graphics.Draw(_screenTarget, Vec2.Zero, new Rectangle?(), Color.White, 0f, Vec2.Zero, new Vec2(1f, 1f), SpriteEffects.None, (Depth)0.82f);
                    Graphics.material = null;
                    _batch.End();
                }
            }
            Graphics.SetRenderTarget(null);
            Graphics.viewport = _oldViewport;
            Graphics.screen = null;
            Graphics.currentLayer = null;
        }

        public override void Update()
        {
            if (_flashTransition)
            {
                _darken -= 0.2f;
                if (_darken < 0.2f) _flashTransition = false;
            }
            if (_flashTransition) return;
            if (_darken < 1f) _darken += 0.2f;
            else _darken = 1f;
        }
    }
}
