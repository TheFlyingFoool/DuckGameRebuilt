using Microsoft.Xna.Framework.Graphics;

namespace DuckGame
{
    public class MaterialDustSparkle : Material //test
    {
        private Tex2D _cone;
        public Vec2 position;
        public Vec2 size;
        public float fade;

        public MaterialDustSparkle(Vec2 pos, Vec2 s, bool wide, bool lit)
        {
            _effect = Content.Load<MTEffect>("Shaders/dustsparkle");
            if (!lit)
            {
                _cone = Content.Load<Tex2D>("arcade/lightSphere");
                pos.y += 10f;
            }
            else
                _cone = !wide ? Content.Load<Tex2D>("arcade/lightCone") : Content.Load<Tex2D>("arcade/bigLightCone");
            position = pos;
            size = s;
        }

        public override void Apply()
        {
            Graphics.device.Textures[1] = (Texture2D)_cone;
            Graphics.device.SamplerStates[1] = SamplerState.PointClamp;
            SetValue("topLeft", position);
            SetValue("size", size);
            SetValue("fade", Layer.Game.fade * fade);
            SetValue("viewMatrix", Graphics.screen.viewMatrix);
            SetValue("projMatrix", Graphics.screen.projMatrix);
            foreach (EffectPass pass in _effect.effect.CurrentTechnique.Passes)
                pass.Apply();
        }
    }
}
