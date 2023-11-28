using AddedContent.Firebreak;

namespace DuckGame
{

    public static partial class DevConsoleCommands
    {
        [Marker.DevConsoleCommand(Description = "Zips your cloud data to your save directory", IsCheat = true)]
        public static void ZipCloud()
        {
            string pFile = $"{DuckFile.saveDirectory}cloud_zip.zip";
            Cloud.ZipUpCloudData(pFile);
            DevConsole.Log(new DCLine
            {
                line = $"Zipped up to: {pFile}",
                color = Colors.DGBlue
            });
        }
    }
}