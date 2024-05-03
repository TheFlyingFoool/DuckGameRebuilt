using System;

namespace XnaToFna.ProxyForms
{
    [Flags]
    public enum MouseButtons
    {
        Left = 1048576, // 0x00100000
        None = 0,
        Right = 2097152, // 0x00200000
        Middle = 4194304, // 0x00400000
        XButton1 = 8388608, // 0x00800000
        XButton2 = 16777216, // 0x01000000
    }
}
