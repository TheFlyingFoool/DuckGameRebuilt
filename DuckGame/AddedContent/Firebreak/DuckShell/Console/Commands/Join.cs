namespace DuckGame.ConsoleEngine
{
    public static partial class Commands
    {
        [DSHCommand(Description = "Concatenates the provided items")]
        public static string Join(params string[] items)
        {
            return JoinWith(string.Empty, items);
        }
    }
}