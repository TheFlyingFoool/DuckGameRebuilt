using AddedContent.Firebreak;
using DuckGame.ConsoleInterface;

namespace DuckGame.ConsoleEngine
{
    public static partial class Commands
    {
        [Marker.DSHCommand(Description = "Prints a string to the console")]
        public static void Echo(string s)
        {
            console.WriteLine(s, DSHConsoleLine.Significance.Neutral);
        }
    }
}