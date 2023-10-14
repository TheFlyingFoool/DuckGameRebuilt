namespace DuckGame.ConsoleEngine
{
    public static partial class Commands
    {
        [DSHCommand(Name = "!", Description = "Returns true if false, and false if true", Hidden = true)]
        public static bool StrEqualTo(bool b) => !b;
    }
}