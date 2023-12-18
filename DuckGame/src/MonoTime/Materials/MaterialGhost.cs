using Microsoft.Xna.Framework.Graphics;

namespace DuckGame
{
    public class MaterialGhost : Material // unused
    {
        public MaterialGhost() => _effect = Content.Load<MTEffect>("Shaders/ghost");

        public override void Apply()
        {
            foreach (EffectPass pass in _effect.effect.CurrentTechnique.Passes)
                pass.Apply();
        }
    }
}
