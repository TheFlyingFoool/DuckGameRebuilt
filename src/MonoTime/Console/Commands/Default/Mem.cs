using System;
using System.Diagnostics;
using System.Linq;

namespace DuckGame;

public static partial class DevConsoleCommands
{
    [DevConsoleCommand(Description = "Prints Duck Game's current memory usage")]
    public static string Mem()
    {
        long num = GC.GetTotalMemory(true) / 1000L;
        return $"Garbage Collector Has {num} KB Allocated ({num / 1000L} MB)";
    }
}