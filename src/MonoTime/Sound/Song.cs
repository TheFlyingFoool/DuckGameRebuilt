// Decompiled with JetBrains decompiler
// Type: DuckGame.Song
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

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
