using AddedContent.Firebreak;

namespace DuckGame.ConsoleEngine
{
    public static partial class Commands
    {
        [Marker.DevConsoleCommand(Description = "Does an action for every element in a collection", To = ImplementTo.DuckShell)]
        public static void For(string variableName, string[] collection, [CommandAutoCompl(false)] string command)
        {
            foreach (string item in collection)
            {
                Set(variableName, item);
                console.Run(command, false);
            }

            VariableRegister.Remove(variableName);
        }
    }
}