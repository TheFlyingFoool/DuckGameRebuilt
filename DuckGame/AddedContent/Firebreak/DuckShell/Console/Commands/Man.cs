using System.Text;

namespace DuckGame.ConsoleEngine
{
    public static partial class Commands
    {
        [DSHCommand(Description = "Explains the usage of a command")]
        public static string Man(string commandName)
        {
            if (console.Shell.Commands.TryFirst(x => x.Name.CaselessEquals(commandName),
                    out DSHCommand commandAttribute))
            {
                StringBuilder functionSignature = commandAttribute.GetFunctionSignature();
                functionSignature.Append('\n');
                functionSignature.Append(commandAttribute.Description);
                
                return functionSignature.ToString();
            }
            else
            {
                return $"No command found with name: {commandName}";
            }
        }
    }
}