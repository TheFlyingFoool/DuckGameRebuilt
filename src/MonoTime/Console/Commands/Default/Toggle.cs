using System;
using System.Diagnostics;
using System.Linq;

namespace DuckGame;

public static partial class DevConsoleCommands
{
    [DevConsoleCommand(Description = "Toggles whether or not a layer is visible. Some options include 'game', 'background', 'blocks' and 'parallax'")]
    public static bool Toggle(Layer layer)
    {
        return layer.visible ^= true;
    }
}