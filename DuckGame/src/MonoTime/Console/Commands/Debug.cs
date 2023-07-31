using Newtonsoft.Json;
using System.Threading.Tasks;

namespace DuckGame
{
    public static partial class DevConsoleCommands
    {
        [DevConsoleCommand]
        public static void Debug()
        {
            DevConsole.RunCommand("eight");
            DevConsole.RunCommand("lev rockthrow");
        }
    }
}