using AddedContent.Firebreak;

namespace DuckGame.ConsoleEngine
{
    public static partial class Commands
    {
        [Marker.DSHCommand(Name = "!", Description = "Returns true if false, and false if true", Hidden = true)]
        public static bool StrEqualTo(bool b) => !b;
    }
}