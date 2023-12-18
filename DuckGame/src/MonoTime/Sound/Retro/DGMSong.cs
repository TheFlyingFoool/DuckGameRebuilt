using System.IO;
using System.Linq;
using System.IO.Compression;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;
//added by othello7

namespace DuckGame
{
    public class DGMSong
    {
        List<VGMSong> vgmList = new List<VGMSong>();

        private float _volume = 1f;
        private bool _looped = true;
        public SoundState state => vgmList[0].state;

        public DGMSong(string folder)
        {
            //yipee the .dgm is real -othello7, i made it use a folder instead -dan
            foreach (string file in DuckFile.GetFiles(folder, "*.vgm"))
            {
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
            }
        }

        public void Play()
        {
            for (int i = 0; i < vgmList.Count; i++)
            {
                VGMSong player = vgmList[i];
                player.Play();
                player.looped = false;
            }
        }
        public void Update()
        {
            if (looped)
            {
                for (int i = 0; i < vgmList.Count; i++)
                {
                    VGMSong player = vgmList[i];
                    if (player.state != SoundState.Stopped)
                    {
                        return;
                    }
                }
                for (int i = 0; i < vgmList.Count; i++)
                {
                    VGMSong player = vgmList[i];
                    player.Play();
                }
            }
        }

        public void Pause()
        {
            for (int i = 0; i < vgmList.Count; i++)
            {
                VGMSong player = vgmList[i];
                player.Pause();
            }
        }

        public void Resume()
        {
            for (int i = 0; i < vgmList.Count; i++)
            {
                VGMSong player = vgmList[i];
                player.Resume();
            }
        }

        public void Stop()
        {
            for (int i = 0; i < vgmList.Count; i++)
            {
                VGMSong player = vgmList[i];
                player.Stop();
            }
        }

    }
}
