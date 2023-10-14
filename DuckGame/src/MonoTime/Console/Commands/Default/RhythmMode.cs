using AddedContent.Firebreak;

namespace DuckGame
{

    public static partial class DevConsoleCommands
    {
        [Marker.DevConsoleCommand(Description = "Toggles rhythm mode (groovy)")]
        public static bool RhythmMode()
        {
            if (!DevConsole.core.rhythmMode)
                Music.Stop();
            DevConsole.core.rhythmMode ^= true;
            if (!DevConsole.core.rhythmMode)
                return false;
            Music.Play(Music.RandomTrack("i can write whatever i want here because for some reason" +
                                         "landon decided to hardcode the music for rhythm mode but" +
                                         "still decided to use the general method regardless... :I"));

            return true;
        }
    }
}