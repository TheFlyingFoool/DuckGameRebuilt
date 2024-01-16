using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DuckGame
{
    public class Material
    {
        public bool spsupport;
        protected MTEffect _effect;

        public MTEffect effect => _effect;

        public Material()
        {
        }
        public MTSpriteBatchItem batchItem;
        public Material(string mat) => _effect = Content.Load<MTEffect>(mat);

        public Material(Effect e) => _effect = (MTEffect)e;

        public virtual void SetValue(string name, float value) => _effect?.effect.Parameters[name]?.SetValue(value);

        public virtual void SetValue(string name, Vec2 value) => _effect?.effect.Parameters[name]?.SetValue((Vector2)value);

        public virtual void SetValue(string name, Vec3 value) => _effect?.effect.Parameters[name]?.SetValue((Vector3)value);

        public virtual void SetValue(string name, Color value) => _effect?.effect.Parameters[name]?.SetValue((Vector4)value.ToVector4());

        public virtual void SetValue(string name, Matrix value) => _effect?.effect.Parameters[name]?.SetValue((Microsoft.Xna.Framework.Matrix)value);

        public virtual void SetValue(string name, Texture2D value) => _effect?.effect.Parameters[name]?.SetValue(value);

        public virtual void Update()
        {
        }

        public virtual void Apply()
        {
            if (_effect == null)
                return;
            foreach (EffectPass pass in _effect.effect.CurrentTechnique.Passes)
                pass.Apply();
        }

        public static implicit operator MTEffect(Material val) => val.effect;
    }
}
