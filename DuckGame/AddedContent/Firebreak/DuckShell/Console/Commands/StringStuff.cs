using AddedContent.Firebreak;

namespace DuckGame.ConsoleEngine
{
    public static partial class Commands
    {
        [Marker.DevConsoleCommand(Name = "@=", Description = "Returns True if a = b, else False.", To = ImplementTo.DuckShell)]
        public static bool StrEqualTo(string a, string b) => a == b;
        
        [Marker.DevConsoleCommand(Name = "replace", Description = "Replaces all FROM with TO in the given string", To = ImplementTo.DuckShell)]
        public static string StrEqualTo(string givenString, string from, string to) => givenString.Replace(from, to);
        
        [Marker.DevConsoleCommand(Name = "@!=", Description = "Returns False if a = b, else True.", To = ImplementTo.DuckShell)]
        public static bool StrNotEqualTo(string a, string b) => a != b;
    }
}