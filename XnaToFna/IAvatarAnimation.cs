// Decompiled with JetBrains decompiler
// Type: XnaToFna.StubXDK.GamerServices.IAvatarAnimation
// Assembly: XnaToFna, Version=18.5.1.29483, Culture=neutral, PublicKeyToken=null
// MVID: C1D3521D-C7E9-4C43-B430-D28CC69450A3
// Assembly location: C:\Users\daniel\Desktop\Release\XnaToFna.exe

using Microsoft.Xna.Framework;
using System;
using System.Collections.ObjectModel;

namespace XnaToFna.StubXDK.GamerServices
{
  public interface IAvatarAnimation
  {
    ReadOnlyCollection<Matrix> BoneTransforms { get; }

    TimeSpan CurrentPosition { get; set; }

    AvatarExpression Expression { get; }

    TimeSpan Length { get; }

    void Update(TimeSpan elapsedAnimationTime, bool loop);
  }
}
