// Decompiled with JetBrains decompiler
// Type: DuckGame.Music
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DuckGame
{
    public class Music
    {
        private static Dictionary<string, MemoryStream> _songs = new Dictionary<string, MemoryStream>();
        private static Dictionary<string, Queue<string>> _recentSongs = new Dictionary<string, Queue<string>>();
        private static float _fadeSpeed;
        private static float _volume = 1f;
        private static float _volumeMult = 1f;
        private static float _masterVolume = 0.65f;
        private static string _currentSong = "";
        private static string _pendingSong = "";
        private static string[] _songList;
        private static Random _musicPickGen = new Random();
        private static SoundEffect _currentMusic = (SoundEffect)null;
        public static MusicInstance _musicPlayer = (MusicInstance)null;
        private static bool _alternateLoop = false;
        private static string _alternateSong = "";
        private static HashSet<string> _processedSongs = new HashSet<string>();

        public static Dictionary<string, MemoryStream> songs => Music._songs;

        public static void Reset() => Music._recentSongs.Clear();

        public static bool stopped => Music._musicPlayer.State == SoundState.Stopped || Music._musicPlayer.State == SoundState.Paused;

        public static float volumeMult
        {
            get => Music._volumeMult;
            set
            {
                Music._volumeMult = value;
                Music.volume = Music._volume;
            }
        }

        public static float volume
        {
            get => Music._volume;
            set
            {
                Music._volume = value;
                if (Music._musicPlayer == null)
                    return;
                Music._musicPlayer.Volume = Music._volume * (Music._masterVolume * Music._masterVolume) * Music._volumeMult;
            }
        }

        public static float masterVolume
        {
            get => Music._masterVolume;
            set
            {
                Music._masterVolume = value;
                Music.volume = Music._volume;
            }
        }

        public static string currentSong => Music._currentSong;

        public static string pendingSong => Music._pendingSong;

        public static TimeSpan position => new TimeSpan(0, 0, 0, 0, (int)((double)Music._musicPlayer.Platform_GetProgress() * (double)Music._musicPlayer.Platform_GetLengthInMilliseconds()));

        public static bool finished => Music._musicPlayer.State == SoundState.Stopped;

        public static void Initialize()
        {
            Music._musicPlayer = new MusicInstance((SoundEffect)null);
            Music._songList = Content.GetFiles("Content/Audio/Music/InGame");
        }

        public static void PreloadSongs()
        {
        }

        public static void Terminate()
        {
            foreach (KeyValuePair<string, MemoryStream> song in Music._songs)
                song.Value.Close();
        }

        public static string RandomTrack(string folder, string ignore = "")
        {
            if (DevConsole.rhythmMode)
                return "InGame/comic.ogg";
            string[] strArray = Music._songList;
            if (ReskinPack.active.Count > 0)
            {
                List<string> stringList = new List<string>();
                foreach (ReskinPack reskinPack in ReskinPack.active)
                    stringList.AddRange((IEnumerable<string>)DuckFile.GetFiles(reskinPack.contentPath + "/Audio/Music/InGame"));
                if (stringList.Count > 0)
                    strArray = stringList.ToArray();
            }
            if (strArray.Length == 0)
                return "";
            Random generator = Rando.generator;
            Rando.generator = Music._musicPickGen;
            List<string> stringList1 = new List<string>();
            foreach (string path in strArray)
            {
                string str = folder + "/" + Path.GetFileNameWithoutExtension(path);
                if (str != ignore)
                    stringList1.Add(str);
            }
            if (stringList1.Count == 0)
                stringList1.Add(folder + "/" + Path.GetFileNameWithoutExtension(strArray[0]));
            Queue<string> stringQueue = (Queue<string>)null;
            if (!Music._recentSongs.TryGetValue(folder, out stringQueue))
            {
                stringQueue = new Queue<string>();
                Music._recentSongs[folder] = stringQueue;
            }
            if (stringQueue.Count > 0 && stringQueue.Count > stringList1.Count - 5)
                stringQueue.Dequeue();
            List<string> stringList2 = new List<string>();
            stringList2.AddRange((IEnumerable<string>)stringList1);
            string str1 = "";
            while (str1 == "")
            {
                if (stringList1.Count == 0 && stringQueue.Count > 0)
                {
                    str1 = stringQueue.Dequeue();
                    if (!stringList2.Contains(str1))
                        str1 = "";
                }
                else if (stringList1.Count == 0)
                {
                    str1 = stringList2[0];
                }
                else
                {
                    str1 = stringList1[Rando.Int(stringList1.Count<string>() - 1)];
                    if (str1 == ignore && stringList1.Count > 1)
                    {
                        stringList1.Remove(str1);
                        str1 = "";
                    }
                    else if (stringQueue.Contains(str1))
                    {
                        if ((double)Rando.Float(1f) > 0.25)
                        {
                            stringList1.Remove(str1);
                            if (stringList1.Count > 0)
                                str1 = "";
                        }
                        else
                            str1 = "";
                    }
                }
            }
            stringQueue.Enqueue(str1);
            Rando.generator = generator;
            return str1;
        }

        public static string FindSong(string song)
        {
            foreach (string song1 in Music._songList)
            {
                string withoutExtension = Path.GetFileNameWithoutExtension(song1);
                if (withoutExtension.ToLower() == song.ToLower())
                    return "InGame/" + withoutExtension;
            }
            return "Challenging";
        }

        public static void Play(string music, bool looping = true, float crossFadeTime = 0.0f)
        {
            if (!Music.Load(music))
                return;
            Music._musicPlayer.Play();
            Music._musicPlayer.IsLooped = looping;
        }

        public static void Play(Song music, bool looping = true)
        {
        }

        public static bool Load(string music, bool looping = true, float crossFadeTime = 0.0f)
        {
            Music._currentSong = music;
            Music._musicPlayer.Stop();
            if (!music.Contains(":"))
            {
                if (!music.EndsWith(".wav"))
                {
                    try
                    {
                        string str = "Audio/Music/" + music;
                        try
                        {
                            Music._currentMusic = ReskinPack.LoadAsset<SoundEffect>(str + ".ogg", true);
                            if (Music._currentMusic == null)
                                Music._currentMusic = ReskinPack.LoadAsset<SoundEffect>(str + ".mp3", true);
                        }
                        catch (Exception ex)
                        {
                        }
                        if (Music._currentMusic == null)
                        {
                            Music._currentMusic = new SoundEffect(DuckFile.contentDirectory + str + ".ogg");
                            goto label_10;
                        }
                        else
                            goto label_10;
                    }
                    catch (Exception ex)
                    {
                        DevConsole.Log(DCSection.General, "|DGRED|Failed to load music (" + music + "):");
                        DevConsole.Log(DCSection.General, "|DGRED|" + ex.Message);
                        goto label_10;
                    }
                }
            }
            Music._currentMusic = new SoundEffect(music);
        label_10:
            Music._musicPlayer.SetData(Music._currentMusic);
            return true;
        }

        public static void PlayLoaded() => Music._musicPlayer.Play();

        public static void CancelLooping() => Music._musicPlayer.IsLooped = false;

        public static void LoadAlternateSong(string music, bool looping = true, float crossFadeTime = 0.0f)
        {
            Music._alternateLoop = looping;
            Music._pendingSong = music;
            Music._alternateSong = music;
        }

        public static void SwitchSongs()
        {
            try
            {
                Music.Play(Music._pendingSong, Music._alternateLoop);
            }
            catch
            {
            }
            Music._pendingSong = (string)null;
        }

        public static void Pause() => Music._musicPlayer.Pause();

        public static void Resume() => Music._musicPlayer.Resume();

        public static void Stop()
        {
            Music._musicPlayer.Stop();
            Music._currentSong = "";
        }

        public static void FadeOut(float duration) => Music._fadeSpeed = duration / 60f;

        public static void FadeIn(float duration) => Music._fadeSpeed = (float)-((double)duration / 60.0);

        private static void SearchDir(string dir)
        {
            foreach (string file in Content.GetFiles(dir))
                Music.ProcessSong(file);
            foreach (string directory in Content.GetDirectories(dir))
                Music.SearchDir(directory);
        }

        private static void ProcessSong(string path)
        {
            if (ReskinPack.context != null)
            {
                if (ReskinPack.context.hasIngameMusic && !path.Contains(":") && path.Contains("Audio/Music/InGame"))
                    return;
                string str = path;
                if (str.StartsWith("Content"))
                    str = str.Substring(7, str.Length - 7);
                string pPath = ReskinPack.context.contentPath + str;
                if (DuckFile.FileExists(pPath))
                    path = pPath;
            }
            path = path.Replace('\\', '/');
            if (Music._processedSongs.Contains(path))
                return;
            Music._processedSongs.Add(path);
            try
            {
                MemoryStream memoryStream = OggSong.Load(path, !path.Contains(":"));
                path = path.Substring(0, path.Length - 4);
                string key = path.Substring(path.IndexOf("/Music/") + 7);
                Music._songs[key] = memoryStream;
            }
            catch (Exception ex)
            {
                DevConsole.Log(DCSection.General, "Failed to load song: " + path);
            }
            ++MonoMain.loadyBits;
        }

        public static void Update()
        {
        }
    }
}
