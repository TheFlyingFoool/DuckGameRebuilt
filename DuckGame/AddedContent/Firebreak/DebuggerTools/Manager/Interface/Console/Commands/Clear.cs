namespace DuckGame.ConsoleEngine
{
    public static partial class Commands
    {
        [MMCommand(Description = "Clears the console.")]
        public static void Clear()
        {
            console.Clear();
        }
    }
}