namespace DuckGame.ConsoleEngine
{
    public static partial class Commands
    {
        [DSHCommand(Description = "Clears the console.")]
        public static void Clear()
        {
            console.Clear();
        }
    }
}