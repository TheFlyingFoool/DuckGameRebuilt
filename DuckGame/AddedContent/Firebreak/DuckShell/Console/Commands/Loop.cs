using AddedContent.Firebreak;

namespace DuckGame.ConsoleEngine
{
    public static partial class Commands
    {
        [Marker.DevConsoleCommand(Description = "Repeats an action X times", To = ImplementTo.DuckShell)]
        public static void Loop(int times, string command)
        {
            for (int i = 0; i < times; i++)
            {
                console.Run(command, false);
            }
        }
    }
}