namespace DuckGame.ConsoleEngine
{
    public static partial class Commands
    {
        [DSHCommand(Description = "Does an action for every element in a collection")]
        public static void For(string variableName, string[] collection, string command)
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