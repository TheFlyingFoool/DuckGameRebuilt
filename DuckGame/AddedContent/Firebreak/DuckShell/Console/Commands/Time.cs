using AddedContent.Firebreak;
using System;

namespace DuckGame.ConsoleEngine
{
    public static partial class Commands
    {
        [Marker.DevConsoleCommand(Description = "Returns an array of information about the time specified", To = ImplementTo.DuckShell)]
        public static string Time(string from = "now")
        {
            DateTimeOffset time = from == "now" ? DateTimeOffset.Now : DateTimeOffset.Parse(from);

            return string.Join(",", time.ToUnixTimeMilliseconds(), $"{time:yyyy,MM,dd,HH,mm,ss,tt,zz}");
        }
    }
}