using AddedContent.Firebreak;
using System.Text.RegularExpressions;

namespace DuckGame.ConsoleEngine
{
    public static partial class Commands
    {
        [Marker.DevConsoleCommand(Name = "replaceregex", Description = "Replace using regex. \nKeep in mind that you need to backslash backslashes", To = ImplementTo.DuckShell)]
        public static string ReplaceRegex(string givenString, string pattern, string to)
        {
            return Regex.Replace(givenString, pattern, to);
        }
    }
}