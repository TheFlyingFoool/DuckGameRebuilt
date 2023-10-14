using AddedContent.Firebreak;
using System.Linq;
using System.Text;

namespace DuckGame
{

    public static partial class DevConsoleCommands
    {
        public static readonly Color SyntaxColorKeywords = Color.FromHexString("FF6188"); // pink
        public static readonly Color SyntaxColorComments = Color.FromHexString("59DEDE"); // cyan
        public static readonly Color SyntaxColorBraces = Color.FromHexString("939293"); // grey
        public static readonly Color SyntaxColorMethods = Color.FromHexString("A9DC76"); // lime
        public static readonly Color SyntaxColorNumber = Color.FromHexString("AB9DF2"); // purple
        public static readonly Color SyntaxColorParams = Color.FromHexString("F59762"); // orange
        public static readonly Color SyntaxColorFields = Color.FromHexString("FCFCFA"); // white
        public static readonly Color SyntaxColorStrings = Color.FromHexString("FFD866"); // yellow

        [Marker.DevConsoleCommand(
            Aliases = new[] { "?" },
            Description = "Gives general help about every command")]
        public static void Help(bool includeDescriptions = false, bool verboseParameters = false)
        {
            string s1 = SyntaxColorBraces.ToDGColorString();
            string s2 = SyntaxColorMethods.ToDGColorString();
            string s3 = SyntaxColorParams.ToDGColorString();
            string s4 = SyntaxColorKeywords.ToDGColorString();
            string s5 = SyntaxColorComments.ToDGColorString();

            string fullHelpString = string.Join("\n",
                DevConsole.commands.Values.Select(x => generateHelpString(x[0])));

            DevConsole.LogComplexMessage(fullHelpString, Color.White);

            string generateHelpString(CMD cmd)
            {
                StringBuilder builder = new();
                builder.Append(s2);
                builder.Append(cmd.keyword);
                builder.Append(s1);
                builder.Append('(');
                if (cmd.arguments != null)
                    builder.Append(string.Join(", ",
                        cmd.arguments.Select(x =>
                        {
                            StringBuilder parameterBuilder = new();

                            if (x.optional) parameterBuilder.Append($"{s1}[");
                            if (verboseParameters) parameterBuilder.Append($"{s4}{x.type.Name} ");
                            parameterBuilder.Append($"{s3}{x.name}");
                            if (x.optional) parameterBuilder.Append($"{s1}]");

                            return parameterBuilder.ToString();
                        })));
                builder.Append(s1);
                builder.Append(')');

                if (string.IsNullOrEmpty(cmd.description) || !includeDescriptions)
                    return builder.ToString();

                builder.Append('\n');
                builder.Append(string.Join("\n", cmd.description.SplitByLength().Select(x => $"  {s5}// {x}")));

                return builder.ToString();
            }
        }
    }
}
