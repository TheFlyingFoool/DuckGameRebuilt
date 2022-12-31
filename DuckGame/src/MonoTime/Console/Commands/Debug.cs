using Newtonsoft.Json;
using System.Threading.Tasks;

namespace DuckGame
{
    public static partial class DevConsoleCommands
    {
        [DevConsoleCommand]
        public static string Debug()
        {
            return JsonConvert.SerializeObject(new Rectangle(0, 0, 24, 24), Formatting.Indented);
        }
    }
}