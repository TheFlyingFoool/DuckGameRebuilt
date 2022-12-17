namespace DuckGame
{

    public static partial class DevConsoleCommands
    {
        public static string DCSectionFilter = "all";
        
        [DevConsoleCommand(Description = "Filters the DevConsole's lines based on their section(s)")]
        public static void Filter(string filter)
        {
            DCSectionFilter = filter;
        }
    }
}