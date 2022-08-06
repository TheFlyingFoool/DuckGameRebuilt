// Decompiled with JetBrains decompiler
// Type: XnaToFna.ContentTransformers.EffectTransformer
// Assembly: XnaToFna, Version=18.5.1.29483, Culture=neutral, PublicKeyToken=null
// MVID: C1D3521D-C7E9-4C43-B430-D28CC69450A3
// Assembly location: C:\Users\daniel\Desktop\Release\XnaToFna.exe

using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Runtime.Serialization;

namespace XnaToFna.ContentTransformers
{
  public class EffectTransformer : ContentTypeReader<Effect>
  {
    private Type t_Effect = typeof (Effect);

    protected override Effect Read(ContentReader input, Effect existing)
    {
      input.ReadBytes(input.ReadInt32());
      return (Effect) FormatterServices.GetUninitializedObject(this.t_Effect);
    }
  }
}
