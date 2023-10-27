namespace DuckGame
{
    public class NMPlayMusic : NMEvent
    {
        public string song;

        public NMPlayMusic(string s) => song = s;

        public NMPlayMusic()
        {
        }

        public override void Activate() => Music.Play(song);
    }
}
