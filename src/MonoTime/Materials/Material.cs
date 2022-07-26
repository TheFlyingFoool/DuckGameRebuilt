// Decompiled with JetBrains decompiler
// Type: DuckGame.Material
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DuckGame
{
    public class Material
    {
        protected MTEffect _effect;

        public MTEffect effect => this._effect;

        public Material()
        {
        }

        public Material(string mat) => this._effect = Content.Load<MTEffect>(mat);

        public Material(Effect e) => this._effect = (MTEffect)e;

        public virtual void SetValue(string name, float value) => this._effect.effect.Parameters[name]?.SetValue(value);

        public virtual void SetValue(string name, Vec2 value) => this._effect.effect.Parameters[name]?.SetValue((Vector2)value);

        public virtual void SetValue(string name, Vec3 value) => this._effect.effect.Parameters[name]?.SetValue((Vector3)value);

        public virtual void SetValue(string name, Color value) => this._effect.effect.Parameters[name]?.SetValue((Vector4)value.ToVector4());

        public virtual void SetValue(string name, Matrix value) => this._effect.effect.Parameters[name]?.SetValue((Microsoft.Xna.Framework.Matrix)value);

        public virtual void SetValue(string name, Texture2D value) => this._effect.effect.Parameters[name]?.SetValue((Texture)value);

        public virtual void Update()
        {
        }

        public virtual void Apply()
        {
            foreach (EffectPass pass in this._effect.effect.CurrentTechnique.Passes)
                pass.Apply();
        }

        public static implicit operator MTEffect(Material val) => val.effect;
    }
}
