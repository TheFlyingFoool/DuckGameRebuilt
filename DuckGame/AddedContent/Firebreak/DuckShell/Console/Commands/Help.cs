using AddedContent.Firebreak;
using System;
using System.Linq;
using System.Text;

namespace DuckGame.ConsoleEngine
{
    public static partial class Commands
    {
        [Marker.DSHCommand(Description = "Displays the function signatures of all public " +
                                  "commands, along with their description to explain " +
                                  "their usage.")]
        public static string Help()
        {
            StringBuilder builder = new();

            foreach (Marker.DSHCommandAttribute commandAttribute in console.Shell.Commands.Where(x => !x.Hidden))
            {
                builder.Append(Man(commandAttribute.Name));

                builder.Append('\n');
            }

            if (builder.Length > 0)
                builder.Remove(builder.Length - 1, 1);

            return builder.ToString();
        }
    }
}