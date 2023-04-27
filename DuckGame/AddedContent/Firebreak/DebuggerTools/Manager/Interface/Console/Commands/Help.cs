using System;
using System.Linq;
using System.Text;

namespace DuckGame.ConsoleEngine
{
    public static partial class Commands
    {
        [MMCommand(Description = "Displays the function signatures of all public " +
                                 "commands, along with their description to explain " +
                                 "their usage.")]
        public static string Help(bool condensed = false)
        {
            StringBuilder builder = new();

            foreach (MMCommandAttribute commandAttribute in console.ShellInstance.Commands.Where(x => !x.Hidden))
            {
                builder.Append(commandAttribute.GetFunctionSignature());

                if (!condensed)
                {
                    builder.Append(" {\n");

                    foreach (string line in commandAttribute.Description?.SplitByLength(50) ?? Array.Empty<string>())
                    {
                        builder.Append($"    // {line}\n");
                    }

                    builder.Append('}');
                }
                else
                {
                    builder.Append(';');
                }

                builder.Append('\n');
            }

            if (builder.Length > 0)
                builder.Remove(builder.Length - 1, 1);

            return builder.ToString();
        }
    }
}