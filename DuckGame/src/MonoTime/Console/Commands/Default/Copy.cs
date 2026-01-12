using AddedContent.Firebreak;
using Microsoft.Xna.Framework;
using SDL2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace DuckGame
{

    public static partial class DevConsoleCommands
    {
        [Marker.DevConsoleCommand(Description = "Copies the console's last 750 lines of output to your clipboard")]
        public static void Copy()
        {
            StringBuilder currentPart = new();
            Queue<DCLine> lines = DevConsole.core.lines;

            for (int index = Math.Max(lines.Count - 750, 0); index < lines.Count; ++index)
            {
                currentPart.Append(lines.ElementAt(index).ToShortString());
                // NOTE .ToShortString() ends with '\n' so no need to add it manually
                // currentPart.Append('\n');
            }

            Thread thread = new(() => FNAPlatform.SetClipboardText(currentPart.ToString()));
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();

            DevConsole.Log("Log was copied to clipboard!");
        }
    }
}