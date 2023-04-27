namespace DuckGame.ConsoleEngine
{
    public static partial class Commands
    {
        [MMCommand(Description = "Repeats an action X times")]
        public static void Loop(int x, [Autocomplete(AutocompleteTag.Command)] string command)
        {
            for (int i = 0; i < x; i++)
            {
                console.ExecuteCommand(command);
            }
        }
    }
}