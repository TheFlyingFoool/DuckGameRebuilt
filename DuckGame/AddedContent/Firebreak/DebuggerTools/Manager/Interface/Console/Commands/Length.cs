namespace DuckGame.ConsoleEngine
{
    public static partial class Commands
    {
        [MMCommand(Description = "Returns the length of the provided string.")]
        public static int Length(string s)
        {
            return s.Length;
        }
    }
}