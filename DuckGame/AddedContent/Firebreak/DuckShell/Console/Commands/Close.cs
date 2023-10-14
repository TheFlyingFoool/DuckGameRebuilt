using AddedContent.Firebreak;

namespace DuckGame.ConsoleEngine
{
    public static partial class Commands
    {
        [Marker.DSHCommand(Description = "Closes MallardManager.")]
        public static void Close()
        {
            console.Active = false;
        }
    }
}