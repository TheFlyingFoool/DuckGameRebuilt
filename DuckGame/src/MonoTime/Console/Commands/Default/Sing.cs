using AddedContent.Firebreak;

namespace DuckGame
{

    public static partial class DevConsoleCommands
    {
        [Marker.DevConsoleCommand(
            Description = "Plays a specified music track",
            IsCheat = true)]
        public static void Sing(string trackName)
        {
            Music.Play(trackName);
            
            if (Network.isActive) 
                Send.Message(new NMSwitchMusic(trackName));
        }
    }
}