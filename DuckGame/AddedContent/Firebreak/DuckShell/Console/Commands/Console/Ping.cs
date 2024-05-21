using AddedContent.Firebreak;

namespace DuckGame.ConsoleEngine
{
    public static partial class Commands
    {
        [Marker.DevConsoleCommand(Description = "Replies with 'Pong!' as a way to reassure " +
                                  "the user that the program is running.")]
        public static string Ping() => "Pong!";
    }
}