using System;
using System.Collections.Generic;
//added by othello7

namespace DuckGame
{
    public class DGMSong
    {
        private string[] _songList;
        List<VGMSong> vgmList = new List<VGMSong>();

        private float _volume = 1f;
        private bool _looped = true;

        //would be cool if it could be a zipped format or something so we dont just have 18 quintillion folders -othello7
        public DGMSong(string fileorfolderidk)
        {
            _songList = Content.GetFiles(fileorfolderidk);

            foreach (string file in _songList)
            {
                DevConsole.Log("manyfiles: " + file);
                VGMSong vs = new VGMSong(file);
                vgmList.Add(vs);
            }          
        }


        public float volume
        {
            get => _volume;
            set
            {
                _volume = MathHelper.Clamp(value, 0f, 1f);
                foreach (VGMSong player in vgmList)
                {
                    player.volume = _volume;
                }
            }
        }

        public bool looped
        {
            get => _looped;
            set
            {
                _looped = value;
                foreach (VGMSong player in vgmList)
                {
                    player.looped = _looped;
                }
            }
        }

        public void Play()
        {
            foreach (VGMSong player in vgmList)
            {
                player.Play();
            }
        }

        public void Pause()
        {
            foreach (VGMSong player in vgmList)
            {
                player.Pause();
            }
        }

        public void Resume()
        {
            foreach (VGMSong player in vgmList)
            {
                player.Resume();
            }
        }

        public void Stop()
        {
            foreach (VGMSong player in vgmList)
            {
                player.Stop();
            }
        }

    }
}
