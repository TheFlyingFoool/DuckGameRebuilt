// Decompiled with JetBrains decompiler
// Type: DuckGame.ContentPack
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using XnaToFna;

namespace DuckGame
{
    public class ContentPack
    {
        public static long kTotalKilobytesAllocated;
        public long kilobytesPreAllocated;
        public List<string> levels = new List<string>();
        protected Dictionary<string, Texture2D> _textures = new Dictionary<string, Texture2D>();
        protected Dictionary<string, SoundEffect> _sounds = new Dictionary<string, SoundEffect>();
        protected Dictionary<string, Song> _songs = new Dictionary<string, Song>();
        protected ModConfiguration _modConfig;
        public static ContentPack currentPreloadPack;
        //private long _beginCalculatingAllocatedBytes;

        public ContentPack(ModConfiguration modConfiguration) => _modConfig = modConfiguration;

        public void ImportAsset(string path, byte[] data)
        {
            try
            {
                if (Program.IsLinuxD || Program.isLinux)
                {
                    path = XnaToFnaHelper.GetActualCaseForFileName(XnaToFnaHelper.FixPath(path), true);
                }
                string str = path.Substring(0, path.Length - 4);
                if (path.EndsWith(".png"))
                {
                    Texture2D texture2D = TextureConverter.LoadPNGWithPinkAwesomeness(Graphics.device, new Bitmap(new MemoryStream(data)), true);
                    _textures[str] = texture2D;
                    Content.textures[str] = (Tex2D)texture2D;
                }
                else
                {
                    if (!path.EndsWith(".wav"))
                        return;
                    SoundEffect pEffect = SoundEffect.FromStream(new MemoryStream(data));
                    if (pEffect != null)
                    {
                        pEffect.file = path;
                        _sounds[str] = pEffect;
                        SFX.RegisterSound(str, pEffect);
                    }
                    else
                        DevConsole.Log(DCSection.General, "|DGRED|Failed to load sound effect! (" + path + ")");
                }
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// Called when the mod is loaded to preload content. This is only called if preload is set to true.
        /// </summary>
        public virtual void PreloadContent() => PreloadContentPaths();

        /// <summary>
        /// Called when the mod is loaded to preload the paths to all content. Does not actually load content, and is only called if PreloadContent is disabled (looks like that's a lie, this function loads the content).
        /// </summary>
        public virtual void PreloadContentPaths()
        {
            List<string> files = Content.GetFiles<Texture2D>(_modConfig.contentDirectory);
            int length = _modConfig.contentDirectory.Length;
            foreach (string file in files)
            {
                Texture2D texture2D = LoadTexture2DInternal(file);
                string key = file.Substring(0, file.Length - 4);
                _textures[key] = texture2D;
                Content.textures[key] = (Tex2D)texture2D;
            }
            foreach (string file in Content.GetFiles<SoundEffect>(_modConfig.contentDirectory))
            {
                string s = file;
                MonoMain.currentActionQueue.Enqueue(new LoadingAction(() =>
               {
                   currentPreloadPack = this;
                   SoundEffect pEffect = LoadSoundInternal(s);
                   string str = s.Substring(0, s.Length - 4);
                   _sounds[str] = pEffect;
                   SFX.RegisterSound(str, pEffect);
               }, null, "Loading SoundEffect"));
            }
            string str1 = _modConfig.contentDirectory + "/Levels";
            if (DuckFile.DirectoryExists(str1))
                levels = Content.GetFiles<Level>(str1);
            MonoMain.currentActionQueue.Enqueue(new LoadingAction(() =>
           {
               currentPreloadPack = null;
               if (kilobytesPreAllocated / 1000L <= 20L)
                   return;
               MonoMain.CalculateModMemoryOffendersList();
           }, null, "Memory stuff"));
        }

        private static Texture2D LoadTexture2DInternal(string file, bool processPink = true)
        {
            try
            {
                return TextureConverter.LoadPNGWithPinkAwesomeness(Graphics.device, file, processPink);
            }
            catch (Exception ex)
            {
                throw new Exception("PNG Load Fail(" + Path.GetFileName(file) + "): " + ex.Message, ex);
            }
        }

        public static Texture2D LoadTexture2DFromStream(Stream data, bool processPink = true)
        {
            try
            {
                return TextureConverter.LoadPNGWithPinkAwesomeness(Graphics.device, data, processPink);
            }
            catch (Exception ex)
            {
                throw new Exception("PNG Load Fail: " + ex.Message, ex);
            }
        }

        public static PNGData LoadPNGDataFromStream(Stream data, bool processPink = true)
        {
            try
            {
                return TextureConverter.LoadPNGDataWithPinkAwesomeness(data, processPink);
            }
            catch (Exception ex)
            {
                throw new Exception("PNG Load Fail: " + ex.Message, ex);
            }
        }

        public static Texture2D LoadTexture2D(string name, bool processPink = true)
        {
            if (Program.IsLinuxD || Program.isLinux)
            {
                name = XnaToFnaHelper.GetActualCaseForFileName(XnaToFnaHelper.FixPath(name), true);
            }
            Texture2D texture2D = null;
            if (!name.EndsWith(".png"))
                name += ".png";
            if (File.Exists(name))
            {
                try
                {
                    texture2D = LoadTexture2DInternal(name);
                }
                catch (Exception ex)
                {
                    DevConsole.Log(DCSection.General, "LoadTexure2D Error (" + name + "): " + ex.Message);
                }
            }
            return texture2D;
        }

        internal SoundEffect LoadSoundInternal(string file)
        {
            SoundEffect soundEffect = null;
            try
            {
                soundEffect = new FileInfo(file).Length <= 5000000L ? SoundEffect.FromStream(new MemoryStream(File.ReadAllBytes(file)), Path.GetExtension(file)) : SoundEffect.FromStream(new FileStream(file, FileMode.Open), Path.GetExtension(file));
                if (soundEffect != null)
                    soundEffect.file = file;
            }
            catch (Exception)
            {
            }
            return soundEffect;
        }

        internal SoundEffect LoadSoundEffect(string name)
        {
            if (Program.IsLinuxD || Program.isLinux)
            {
                name = XnaToFnaHelper.GetActualCaseForFileName(XnaToFnaHelper.FixPath(name), true);
            }
            SoundEffect soundEffect = null;
            if (Path.GetExtension(name) == "")
                name += ".wav";
            if (File.Exists(name))
                soundEffect = LoadSoundInternal(name);
            return soundEffect;
        }

        internal Song LoadSongInternal(string file)
        {
            Song song = null;
            try
            {
                MemoryStream dat = OggSong.Load(file, false);
                if (dat != null)
                    song = new Song(dat, file);
            }
            catch
            {
            }
            return song;
        }

        internal Song LoadSong(string name)
        {
            if (Program.IsLinuxD || Program.isLinux)
            {
                name = XnaToFnaHelper.GetActualCaseForFileName(XnaToFnaHelper.FixPath(name), true);
            }
            Song song = null;
            if (!name.EndsWith(".ogg"))
                name += ".ogg";
            if (File.Exists(name))
                song = LoadSongInternal(name);
            return song;
        }

        /// <summary>
        /// Loads content from the content pack. Currently supports Texture2D(png) and SoundEffect(wav) in
        /// "mySound" "customSounds/mySound" path format. You should usually use Content.Load&lt;&gt;().
        /// </summary>
        public virtual T Load<T>(string name)
        {
            if (typeof(T) == typeof(Texture2D))
            {
                Texture2D texture2D1;
                if (_textures.TryGetValue(name, out texture2D1))
                    return (T)(object)texture2D1;
                Texture2D texture2D2 = LoadTexture2D(name, _modConfig == null || _modConfig.processPinkTransparency);
                _textures[name] = texture2D2;
                return (T)(object)texture2D2;
            }
            if (typeof(T) == typeof(SoundEffect))
            {
                SoundEffect soundEffect1;
                if (_sounds.TryGetValue(name, out soundEffect1))
                    return (T)(object)soundEffect1;
                SoundEffect soundEffect2 = LoadSoundEffect(name);
                _sounds[name] = soundEffect2;
                return (T)(object)soundEffect2;
            }
            if (!(typeof(T) == typeof(Song)))
                return default(T);
            Song song1;
            if (_songs.TryGetValue(name, out song1))
                return (T)(object)song1;
            Song song2 = LoadSong(name);
            _songs[name] = song2;
            return (T)(object)song2;
        }
    }
}
