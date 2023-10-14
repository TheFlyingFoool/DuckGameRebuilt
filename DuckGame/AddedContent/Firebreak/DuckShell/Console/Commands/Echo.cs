namespace DuckGame.ConsoleEngine
{
    public static partial class Commands
    {
        [DSHCommand(Description = "Prints a string to the console")]
        public static string Echo(string s) => s;
    }
}