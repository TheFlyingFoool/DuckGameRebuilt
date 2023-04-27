namespace DuckGame.ConsoleEngine
{
    public static partial class Commands
    {
        [MMCommand(Description = "Concatenates the provided items")]
        public static string Join(params string[] items)
        {
            return string.Join("", items);
        }
    }
}