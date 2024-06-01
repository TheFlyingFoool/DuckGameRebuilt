using AddedContent.Firebreak;
using System.Text.RegularExpressions;

namespace DuckGame.ConsoleEngine
{
    public static partial class Commands
    {
        [Marker.DevConsoleCommand(Name = "regex", Description = "Match using regex", To = ImplementTo.DuckShell)]
        public static string RegexMatch(string givenString, string pattern)
        {
            Match match = Regex.Match(givenString, pattern);

            if (match.Groups.Count > 1)
                return match.Groups[1].Value;

            return match.Value;
        }
    }
}