using System;

namespace DuckGame.ConsoleEngine
{
    public static partial class Commands
    {
        [DSHCommand(Description = "Returns an array of information about the time specified")]
        public static string Time(string from = "now")
        {
            DateTimeOffset time = from == "now" ? DateTimeOffset.Now : DateTimeOffset.Parse(from);

            return string.Join(",", time.ToUnixTimeMilliseconds(), $"{time:yyyy,MM,dd,HH,mm,ss,tt,zz}");
        }
    }
}