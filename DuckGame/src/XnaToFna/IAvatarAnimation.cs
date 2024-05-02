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
