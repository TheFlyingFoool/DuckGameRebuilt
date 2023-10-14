using AddedContent.Firebreak;

namespace DuckGame.ConsoleEngine
{
    public static partial class Commands
    {
        [Marker.DSHCommand(Name = "@=", Description = "Returns True if a = b, else False.", Hidden = true)]
        public static bool StrEqualTo(string a, string b) => a == b;
        
        [Marker.DSHCommand(Name = "replace", Description = "Replaces all FROM with TO in the given string", Hidden = true)]
        public static string StrEqualTo(string givenString, string from, string to) => givenString.Replace(from, to);
        
        [Marker.DSHCommand(Name = "@!=", Description = "Returns False if a = b, else True.", Hidden = true)]
        public static bool StrNotEqualTo(string a, string b) => a != b;
    }
}