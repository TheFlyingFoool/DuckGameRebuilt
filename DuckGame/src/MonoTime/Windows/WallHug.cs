using System;

namespace DuckGame
{
    [Flags]
    public enum WallHug
    {
        None = 0,
        Left = 1,
        Right = 2,
        Ceiling = 4,
        Floor = 8,
    }
}
