// Decompiled with JetBrains decompiler
// Type: DuckGame.UICaptureBox
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using Microsoft.Xna.Framework.Graphics;

namespace DuckGame
{
    public class UICaptureBox : UIMenu
    {
        private Vec2 _capturePosition;
        private Vec2 _captureSize;
        private RenderTarget2D _captureTarget;
        private UIMenu _closeMenu;
        private bool _resizable;
        public bool finished;

        public UICaptureBox(
          UIMenu closeMenu,
          float xpos,
          float ypos,
          float wide = -1f,
          float high = -1f,
          bool resizable = false)
          : base("", xpos, ypos, wide, high)
        {
            float num = 38f;
            _capturePosition = new Vec2((float)(Layer.HUD.camera.width / 2.0 - num / 2.0), (float)(Layer.HUD.camera.height / 2.0 - num / 2.0));
            _captureSize = new Vec2(num, num);
            if (resizable)
                _captureSize = new Vec2(320f, 180f);
            _closeMenu = closeMenu;
            _resizable = resizable;
        }

        public override void Update()
        {
            if (open)
            {
                if (_captureTarget == null)
                    _captureTarget = !_resizable ? new RenderTarget2D(152, 152, true) : new RenderTarget2D(1280, 720, true);
                MonoMain.autoPauseFade = false;
                if (Input.Down("MENULEFT"))
                    --_capturePosition.x;
                if (Input.Down("MENURIGHT"))
                    ++_capturePosition.x;
                if (Input.Down("MENUUP"))
                    --_capturePosition.y;
                if (Input.Down("MENUDOWN"))
                    ++_capturePosition.y;
                float num = DuckGame.Graphics.width / 320;
                if (_resizable)
                {
                    _captureSize += _captureSize * ((float)-(InputProfile.DefaultPlayer1.leftTrigger - InputProfile.DefaultPlayer1.rightTrigger) * 0.1f);
                    if (_captureSize.x > 1280.0)
                        _captureSize.x = 1280f;
                    if (_captureSize.y > 720.0)
                        _captureSize.y = 720f;
                    Vec2 vec2 = _capturePosition * num;
                    if (vec2.x < 0.0)
                        vec2.x = 0f;
                    if (vec2.y < 0.0)
                        vec2.y = 0f;
                    _capturePosition = vec2 / num;
                }
                DuckGame.Graphics.SetRenderTarget(_captureTarget);
                Camera camera = new Camera(_capturePosition.x * num, _capturePosition.y * num, (int)_captureSize.x * num, (int)_captureSize.y * num);
                DuckGame.Graphics.Clear(Color.Black);
                Viewport viewport = DuckGame.Graphics.viewport;
                DuckGame.Graphics.viewport = new Viewport(0, 0, (int)(_captureSize.x * num), (int)(_captureSize.y * num));
                DuckGame.Graphics.screen.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.DepthRead, RasterizerState.CullNone, null, camera.getMatrix());
                DuckGame.Graphics.Draw(MonoMain.screenCapture, 0f, 0f);
                DuckGame.Graphics.screen.End();
                DuckGame.Graphics.viewport = viewport;
                DuckGame.Graphics.SetRenderTarget(null);
                if (Input.Pressed("SELECT"))
                {
                    SFX.Play("cameraFlash");
                    Editor.previewCapture = (Texture2D)_captureTarget;
                    _captureTarget = null;
                    new UIMenuActionOpenMenu(this, _closeMenu).Activate();
                }
                else if (Input.Pressed("CANCEL"))
                {
                    SFX.Play("consoleCancel");
                    new UIMenuActionOpenMenu(this, _closeMenu).Activate();
                }
            }
            base.Update();
        }

        public override void Draw()
        {
            if (!open)
                return;
            DuckGame.Graphics.DrawRect(new Vec2(_capturePosition.x - 1f, _capturePosition.y - 1f), new Vec2((float)(_capturePosition.x + (int)_captureSize.x + 1.0), (float)(_capturePosition.y + (int)_captureSize.y + 1.0)), Color.White, (Depth)1f, false);
            if (_captureTarget == null)
                return;
            DuckGame.Graphics.Draw(_captureTarget, _capturePosition, new Rectangle?(new Rectangle(0f, 0f, (int)_captureSize.x * 4, (int)_captureSize.y * 4)), Color.White, 0f, Vec2.Zero, new Vec2(0.25f, 0.25f), SpriteEffects.None, (Depth)1f);
        }
    }
}
