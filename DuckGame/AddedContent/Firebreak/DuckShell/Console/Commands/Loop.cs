using AddedContent.Firebreak;

namespace DuckGame.ConsoleEngine
{
    public static partial class Commands
    {
        [Marker.DSHCommand(Description = "Repeats an action X times")]
        public static void Loop(int times, string command)
        {
            for (int i = 0; i < times; i++)
            {
                console.Run(command, false);
            }
        }
    }
}