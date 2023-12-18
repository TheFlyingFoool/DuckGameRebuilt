using System;

namespace DuckGame
{
    [Flags]
    public enum FrameFlipFlags : byte
    {
        Horizontal = 1,
        Vertical = 2,
    }
}
