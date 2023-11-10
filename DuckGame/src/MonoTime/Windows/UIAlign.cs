using System;

namespace DuckGame
{
    [Flags]
    public enum UIAlign
    {
        Center = 0,
        Left = 2,
        Right = 4,
        Top = 8,
        Bottom = 16, // 0x00000010
    }
}
