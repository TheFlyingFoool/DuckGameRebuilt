using AddedContent.Firebreak;
using System;
using System.Text;

namespace DuckGame.ConsoleEngine
{
    public static partial class Commands
    {
        [Marker.DSHCommand(Description = "Explains the usage of a command")]
        public static StringBuilder Man(string commandName)
        {
            if (console.Shell.Commands.TryFirst(x => x.Name.CaselessEquals(commandName),
                    out Marker.DSHCommandAttribute commandAttribute))
            {
                StringBuilder builder = commandAttribute.GetFunctionSignature();
                
                builder.Append(" {\n");

                foreach (string line in commandAttribute.Description?.SplitByLength(50) ?? Array.Empty<string>())
                {
                    builder.Append($"  // {line}\n");
                }

                builder.Append('}');

                return builder;
            }
            else
            {
                throw new Exception($"No command found with name: {commandName}");
            }
        }
    }
}