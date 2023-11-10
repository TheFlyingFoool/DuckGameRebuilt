using AddedContent.Firebreak;

namespace DuckGame
{

    public static partial class DevConsoleCommands
    {
        [Marker.DevConsoleCommand(Description = "Use the old code for handling angles")]
        public static void OldAngles()
        {
            Options.Data.oldAngleCode = !Options.Data.oldAngleCode;
            DevConsole.Log(new DCLine
            {
                line = $"Oldschool Angles have been {(Options.Data.oldAngleCode ? "enabled" : "disabled")}!",
                color = Options.Data.oldAngleCode ? Colors.DGGreen : Colors.DGRed
            });
            Options.Save();
            if (!Network.isActive || DuckNetwork.localProfile == null)
                return;
            Send.Message(new NMOldAngles(DuckNetwork.localProfile, Options.Data.oldAngleCode));
        }
    }
}