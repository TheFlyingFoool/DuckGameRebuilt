// Decompiled with JetBrains decompiler
// Type: DuckGame.PlasmaLayer
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using Microsoft.Xna.Framework.Graphics;

namespace DuckGame
{
    public class PlasmaLayer : Layer
    {
        private Sprite _plasma;
        private MaterialPlasma _plasmaMaterial;
        private new RenderTarget2D _target;
        public float alpha;

        public PlasmaLayer(string nameval, int depthval = 0)
          : base(nameval, depthval)
        {
            _plasma = new Sprite("arcade/plasma");
            camera = new Camera();
            _plasmaMaterial = new MaterialPlasma();
            _target = new RenderTarget2D(320, (int)(320.0 * DuckGame.Graphics.aspect));
            visible = false;
        }

        public override void Update()
        {
            visible = alpha > 0.01f;
            if (!visible)
                return;
            _plasmaMaterial.offset = 0.5f;
            _plasmaMaterial.offset2 = 0.3f;
            _plasmaMaterial.scroll += 0.0009f;
            _plasmaMaterial.scroll2 += 0.0006f;
            _plasmaMaterial.gradientOffset -= 0.0009f;
            _plasmaMaterial.gradientOffset2 += 0.0007f;
            _plasmaMaterial.color1 = Color.DeepPink;
            _plasmaMaterial.color2 = Color.HotPink;
            _plasmaMaterial.Apply();
            Graphics.currentLayer = this;
            Graphics.SetRenderTarget(_target);
            Viewport viewport = Graphics.viewport;
            Graphics.viewport = new Viewport(0, 0, 320, (int)(320f * Graphics.aspect));
            Begin(false, false);
            _plasma.depth = -0.9f;
            _plasma.alpha = 1f;
            Graphics.device.SamplerStates[0] = SamplerState.PointWrap;
            Graphics.Draw(_plasma, -30f, -30f);
            _batch.End();
            Graphics.SetRenderTarget(null);
            Graphics.viewport = viewport;
            Graphics.screen = null;
            Graphics.currentLayer = null;
        }

        public override void Begin(bool transparent, bool isTargetDraw = false)
        {
            Graphics.screen = _batch;
            _batch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.DepthRead, _state, (MTEffect)_plasmaMaterial, camera.getMatrix());
        }

        public override void Draw(bool transparent, bool isTargetDraw = false)
        {
            if (!visible)
                return;
            Graphics.currentLayer = this;
            Graphics.screen = _batch;
            _batch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.DepthRead, _state, null, camera.getMatrix());
            Graphics.Draw(_target, Vec2.Zero, new Rectangle?(), Color.White * alpha, 0f, Vec2.Zero, new Vec2(1f, 1f), SpriteEffects.None, (Depth)1f);
            _batch.End();
            Graphics.screen = null;
            Graphics.currentLayer = null;
        }
    }
}
