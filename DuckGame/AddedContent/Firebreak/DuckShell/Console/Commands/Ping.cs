using AddedContent.Firebreak;

namespace DuckGame.ConsoleEngine
{
    public static partial class Commands
    {
        [Marker.DSHCommand(Description = "Replies with 'Pong!' as a way to reassure " +
                                  "the user that the program is running.")]
        public static string Ping() => "Pong!";
    }
}