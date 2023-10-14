using AddedContent.Firebreak;

namespace DuckGame.ConsoleEngine
{
    public static partial class Commands
    {
        [Marker.DSHCommand(Description = "Repeats a command while the condition is true")]
        public static void While(string conditionCommand, string command)
        {
            while ((bool) console.Shell.TypeInterpreterModulesMap[typeof(bool)].ParseString(Exec(conditionCommand).ToString(), typeof(bool), console.Shell).Unpack())
            {
                console.Run(command, false);
            }
        }
    }
}