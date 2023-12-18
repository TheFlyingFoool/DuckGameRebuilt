namespace DuckGame
{
    public class NMSwitchMusic : NMEvent
    {
        public string song;

        public NMSwitchMusic(string s) => song = s;

        public NMSwitchMusic()
        {
        }

        public override void Activate()
        {
            Music.LoadAlternateSong(song);
            Music.CancelLooping();
        }
    }
}
