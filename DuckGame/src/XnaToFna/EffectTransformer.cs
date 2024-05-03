using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Runtime.Serialization;

namespace XnaToFna.ContentTransformers
{
    public class EffectTransformer : ContentTypeReader<Effect>
    {
        private Type t_Effect = typeof(Effect);

        protected override Effect Read(ContentReader input, Effect existing)
        {
            input.ReadBytes(input.ReadInt32());
            return (Effect)FormatterServices.GetUninitializedObject(t_Effect);
        }
    }
}
