using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DuckGame
{
    public static partial class DevConsoleCommands
    {
        [DevConsoleCommand(Description = "async in duckgame !!!!!!")]
        public static async Task Debug()
        {
            DevConsole.Log("Hello! What's your name?");
            string name = await DevConsole.GetResponse<string>();
            DevConsole.Log($"Well nice to meet you, {name}!");
            DevConsole.Log("..");
            DevConsole.Log("Should I write that down?");
            
            bool shouldWriteDown = await DevConsole.GetResponse<bool>();
            if (shouldWriteDown)
            {
                DevConsole.Log("Write it down I shall, then!");
            }
            else
            {
                DevConsole.Log("Oh, OK then.");
            }
        }
    }
}