using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace DuckGame
{
    public static partial class DevConsoleCommands
    {
        [DevConsoleCommand(Description = "If this command still exists after release im gonna eat my shoes")]
        public static void Debug()
        {
            // new DynamicDCLine(i => i < 1000 ? i.ToString() : null).Log(); 
            foreach (var moji in Input._triggerImageMap)
            {
                DevConsole.Log($"{moji.Key}: @{moji.Key}@");
            }
        }
    }
}