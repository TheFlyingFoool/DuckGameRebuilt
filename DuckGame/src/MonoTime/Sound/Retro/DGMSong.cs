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
        private string[] _songList;
        List<VGMSong> vgmList = new List<VGMSong>();

        private float _volume = 1f;
        private bool _looped = true;
        public SoundState state => vgmList[0].state;

        public DGMSong(string file)
        {
            //yipee the .dgm is real -othello7
            using (ZipArchive archive = ZipFile.OpenRead(file))
            {
                _songList = archive.Entries.Select(entry => entry.FullName).ToArray();

                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    DevConsole.Log("|DGBLUE|LoadDGM: |WHITE|" + entry.FullName);
                    using (Stream entryStream = entry.Open())
                    {
                        // Extract the entry to a temporary location -ChatGPT
                        string tempFilePath = Path.Combine(Path.GetTempPath(), entry.FullName);
                        using (FileStream tempFileStream = File.Create(tempFilePath))
                        {
                            entryStream.CopyTo(tempFileStream);
                        }

                        // Create VGMSong instance from the extracted file -ChatGPT
                        VGMSong vs = new VGMSong(tempFilePath);
                        vgmList.Add(vs);
                    }
                }
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
                for (int i = 0; i < vgmList.Count; i++)
                {
                    VGMSong player = vgmList[i];
                    player.looped = _looped;
                }
            }
        }

        public void Play()
        {
            for (int i = 0; i < vgmList.Count; i++)
            {
                VGMSong player = vgmList[i];
                player.Play();
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
