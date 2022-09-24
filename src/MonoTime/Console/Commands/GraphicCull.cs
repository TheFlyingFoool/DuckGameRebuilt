using System;
using System.Diagnostics;
using System.Linq;

namespace DuckGame
{

    public static partial class DevConsoleCommands
    {
        [AutoConfigField]
        public static bool GraphicsCulling = true;

        [DevConsoleCommand]
        public static void GraphicCull()
        {
            GraphicsCulling ^= true;

            string modeString = GraphicsCulling
                ? "|DGGREEN|Enabled"
                : "|DGRED|Disabled";

            DevConsole.Log($"Graphics Culling {modeString}");
        }
    }
}