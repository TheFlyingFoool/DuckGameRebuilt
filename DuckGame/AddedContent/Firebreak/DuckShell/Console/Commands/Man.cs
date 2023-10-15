using AddedContent.Firebreak;
using System;
using System.Text;

namespace DuckGame.ConsoleEngine
{
    public static partial class Commands
    {
        [Marker.DevConsoleCommand(Description = "Explains the usage of a command", To = ImplementTo.DuckShell)]
        public static StringBuilder Man(string commandName)
        {
            if (console.Shell.Commands.TryFirst(x => x.Name.CaselessEquals(commandName),
                    out Marker.DevConsoleCommandAttribute commandAttribute))
            {
                StringBuilder builder = new();
        
                builder.Append(commandAttribute.Name);
                builder.Append('(');

                ShellCommand.Parameter[] parameterInfos = commandAttribute.Command.Parameters;
                for (int i = 0; i < parameterInfos.Length; i++)
                {
                    ShellCommand.Parameter pInfo = parameterInfos[i];

                    Type pType = pInfo.ParameterType;
                    string pName = pInfo.Name;
                    bool isOptional = pInfo.IsOptional;

                    if (i > 0)
                        builder.Append(", ");

                    if (isOptional)
                        builder.Append('[');

                    builder.Append(pType.Name);
                    builder.Append(' ');
                    builder.Append(pName);

                    if (isOptional)
                        builder.Append(']');
                }

                builder.Append(')');
                
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