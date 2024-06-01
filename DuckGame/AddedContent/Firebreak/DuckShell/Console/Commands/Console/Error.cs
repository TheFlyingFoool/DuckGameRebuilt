using AddedContent.Firebreak;
using System;

namespace DuckGame.ConsoleEngine
{
    public static partial class Commands
    {
        [Marker.DevConsoleCommand(Description = "Throws an error with the provided message", To = ImplementTo.DuckShell)]
        public static object Error(string errorMessage)
        {
            throw new Exception($"Error: {errorMessage}");
        }
    }
}