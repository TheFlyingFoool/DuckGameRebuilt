using AddedContent.Firebreak;

namespace DuckGame.ConsoleEngine
{
    public static partial class Commands
    {
        [Marker.DevConsoleCommand(Description = "Repeats a command while the condition is true", To = ImplementTo.DuckShell)]
        public static void While(string conditionCommand, string command)
        {
            while ((bool) console.Shell.TypeInterpreterModulesMap[typeof(bool)].ParseString(Exec(conditionCommand).ToString(), typeof(bool), new(console.Shell, null)).Unpack())
            {
                console.Run(command, false);
            }
        }
    }
}