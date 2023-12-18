using System;

namespace DuckGame
{
    [Flags]
    public enum TileConnection
    {
        None = 0,
        Left = 2,
        Right = 4,
        Up = 8,
        Down = 16, // 0x00000010
    }
}
