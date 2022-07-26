// Decompiled with JetBrains decompiler
// Type: DuckGame.PadButton
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;

namespace DuckGame
{
    /// <summary>A pad button collection.</summary>
    [Flags]
    public enum PadButton
    {
        DPadUp = 1,
        DPadDown = 2,
        DPadLeft = 4,
        DPadRight = 8,
        Start = 16, // 0x00000010
        Back = 32, // 0x00000020
        LeftStick = 64, // 0x00000040
        RightStick = 128, // 0x00000080
        LeftShoulder = 256, // 0x00000100
        RightShoulder = 512, // 0x00000200
        BigButton = 2048, // 0x00000800
        A = 4096, // 0x00001000
        B = 8192, // 0x00002000
        X = 16384, // 0x00004000
        Y = 32768, // 0x00008000
        LeftThumbstickLeft = 2097152, // 0x00200000
        RightTrigger = 4194304, // 0x00400000
        LeftTrigger = 8388608, // 0x00800000
        RightThumbstickUp = 16777216, // 0x01000000
        RightThumbstickDown = 33554432, // 0x02000000
        RightThumbstickRight = 67108864, // 0x04000000
        RightThumbstickLeft = 134217728, // 0x08000000
        LeftThumbstickUp = 268435456, // 0x10000000
        LeftThumbstickDown = 536870912, // 0x20000000
        LeftThumbstickRight = 1073741824, // 0x40000000
    }
}
