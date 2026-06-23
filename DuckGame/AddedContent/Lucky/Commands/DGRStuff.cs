using AddedContent.Firebreak;

namespace DuckGame.ConsoleEngine
{
    public static partial class Commands
    {
        [Marker.DevConsoleCommand(Description = "Toggles DGR stuff",
                                  To = ImplementTo.DuckShell, IsCheat = true)]
        public static void DGRStuff()
        {
            if (Editor.clientonlycontent)
            {
                DevConsole.Log("Disabled DGR stuff");
                Editor.DisableClientOnlyContent();
            }
            else
            {
                DevConsole.Log("Enabled DGR stuff");
                Editor.EnableClientOnlyContent();
            }
        }
    }
}