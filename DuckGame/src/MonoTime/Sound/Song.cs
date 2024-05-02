using System.IO;

namespace DuckGame
{
    public class Song
    {
        private MemoryStream _data;
        private string _name;

        public MemoryStream data => _data;

        public string name => _name;

        public Song(MemoryStream dat, string nam)
        {
            _data = dat;
            _name = nam;
        }

        public void Play(bool loop = true) => Music.Play(this, loop);

        public void Stop()
        {
            if (!(Music.currentSong == _name))
                return;
            Music.Stop();
        }
    }
}
