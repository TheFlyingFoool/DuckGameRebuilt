using Newtonsoft.Json;
using System.Threading.Tasks;

namespace DuckGame
{
    public static partial class DevConsoleCommands
    {
        [DevConsoleCommand(IsCheat = true)]
        public static void Debug()
        {
            DevConsole.RunCommand("eight");
            DevConsole.RunCommand("lev rockthrow");
        }
    }
}