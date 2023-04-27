namespace DuckGame.ConsoleEngine
{
    public static partial class Commands
    {
        [MMCommand(Description = "Prints a string to the console")]
        public static string Echo(string s) => s;
    }
}