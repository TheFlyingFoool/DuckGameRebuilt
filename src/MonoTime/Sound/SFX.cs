// Decompiled with JetBrains decompiler
// Type: DuckGame.SFX
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DuckGame
{
    public static class SFX
    {
        private static Speech _speech;
        private static Dictionary<string, SoundEffect> _sounds = new Dictionary<string, SoundEffect>();
        private static Map<string, int> _soundHashmap = new Map<string, int>();
        private static Dictionary<string, MultiSoundUpdater> _multiSounds = new Dictionary<string, MultiSoundUpdater>();
        private static List<Sound> _soundPool = new List<Sound>();
        //private const int kMaxSounds = 32;
        private static List<Sound> _playedThisFrame = new List<Sound>();
        public static Windows_Audio _audio;
        public static bool NoSoundcard = false;
        private static float _volume = 1f;
        public static bool enabled = true;
        public static bool skip = false;
        private static int _numProcessed = 0;

        public static Speech speech
        {
            get
            {
                if (Program.IsLinuxD || Program.isLinux)
                    return null;
                if (SFX._speech == null)
                {
                    SFX._speech = new Speech();
                    SFX._speech.Initialize();
                    SFX._speech.SetOutputToDefaultAudioDevice();
                    SFX._speech.ApplyTTSSettings();
                }
                return SFX._speech;
            }
        }

        public static bool hasTTS
        {
            get
            {
                return !Program.IsLinuxD && !Program.isLinux && SFX.speech != null && SFX.speech.GetSayVoices().Count > 0;
            }
        }

        public static void Say(string pString)
        {
            if (Program.IsLinuxD || Program.isLinux || SFX.speech == null)
                return;
            SFX.speech.Say(pString);
        }

        public static void StopSaying()
        {
            if (Program.IsLinuxD || Program.isLinux || SFX.speech == null)
                return;
            SFX.speech.StopSaying();
        }

        public static void SetSayVoice(string pName)
        {
            if (Program.IsLinuxD || Program.isLinux)
                return;
            if (SFX.speech == null)
                return;
            try
            {
                SFX.speech.SetSayVoice(pName);
            }
            catch (Exception ex)
            {
                DevConsole.Log(DCSection.General, "|DGRED|SFX.SetSayVoice failed:" + ex.Message);
            }
        }

        public static List<string> GetSayVoices() => Program.isLinux || SFX.speech == null ? new List<string>() : SFX.speech.GetSayVoices();

        public static void ApplyTTSSettings()
        {
            if (Program.IsLinuxD || Program.isLinux || SFX.speech == null)
                return;
            SFX.speech.ApplyTTSSettings();
        }

        public static int RegisterSound(string pSound, SoundEffect pEffect)
        {
            int hashCode = pSound.GetHashCode();
            lock (SFX._sounds)
            {
                SFX._soundHashmap[pSound] = hashCode;
                SFX._sounds[pSound] = pEffect;
            }
            return hashCode;
        }

        public static bool PoolSound(Sound s)
        {
            if (SFX._soundPool.Count > 32)
            {
                bool flag = false;
                for (int index = 0; index < SFX._soundPool.Count; ++index)
                {
                    if (!SFX._soundPool[index].cannotBeCancelled)
                    {
                        SFX.UnpoolSound(SFX._soundPool[index]);
                        flag = true;
                        break;
                    }
                }
                if (!flag)
                    return false;
            }
            SFX._soundPool.Add(s);
            return true;
        }

        public static void UnpoolSound(Sound s)
        {
            SFX._soundPool.Remove(s);
            s.Unpooled();
        }

        public static void Initialize()
        {
            SFX._audio = new Windows_Audio();
            SFX._audio.Platform_Initialize();
            if (!Windows_Audio.initialized)
            {
                SFX.NoSoundcard = true;
            }
            else
            {
                MonoMain.loadMessage = "Loading SFX";
                SFX.SearchDir("Content/Audio/SFX");
                NetSoundEffect.Initialize();
            }

        }

        public static void Terminate() => SFX._audio.Dispose();

        public static void Update()
        {
            SFX._playedThisFrame.Clear();
            for (int index = 0; index < SFX._soundPool.Count; ++index)
            {
                if (SFX._soundPool[index].State != SoundState.Playing)
                {
                    SFX._soundPool[index].Stop();
                    --index;
                }
            }
            foreach (KeyValuePair<string, MultiSoundUpdater> multiSound in SFX._multiSounds)
                multiSound.Value.Update();
            SFX._audio.Update();
        }

        public static float volume
        {
            get => Math.Min(1f, Math.Max(0f, SFX._volume * SFX._volume)) * 0.9f;
            set => SFX._volume = Math.Min(1f, Math.Max(0f, value));
        }

        /// <summary>
        /// Plays a sound effect, synchronized over the network (if the network is active)
        /// </summary>
        public static Sound PlaySynchronized(
          string sound,
          float vol = 1f,
          float pitch = 0f,
          float pan = 0f,
          bool looped = false)
        {
            return SFX.PlaySynchronized(sound, vol, pitch, pan, looped, false);
        }

        /// <summary>
        /// Plays a sound effect, synchronized over the network (if the network is active)
        /// </summary>
        public static Sound PlaySynchronized(
          string sound,
          float vol,
          float pitch,
          float pan,
          bool looped,
          bool louderForMe)
        {
            if (!SFX.enabled)
                return new InvalidSound(sound, vol, pitch, pan, looped);
            if (Network.isActive)
                Send.Message(new NMSoundEffect(sound, louderForMe ? vol * 0.7f : vol, pitch));
            return SFX.Play(sound, vol, pitch, pan, looped);
        }

        public static Sound Play(string sound, float vol = 1f, float pitch = 0f, float pan = 0f, bool looped = false)
        {
            if (!SFX.enabled || SFX.skip)
                return new InvalidSound(sound, vol, pitch, pan, looped);
            Sound sound1 = SFX._playedThisFrame.FirstOrDefault<Sound>(x => x.name == sound);
            if (sound1 == null)
            {
                try
                {
                    sound1 = SFX.Get(sound, vol, pitch, pan, looped);
                    if (sound1 != null)
                    {
                        sound1.Play();
                        SFX._playedThisFrame.Add(sound1);
                    }
                }
                catch (Exception)
                {
                    return new Sound(SFX._sounds.FirstOrDefault<KeyValuePair<string, SoundEffect>>().Key, 0f, 0f, 0f, false);
                }
            }
            return sound1;
        }

        public static Sound Play(int sound, float vol = 1f, float pitch = 0f, float pan = 0f, bool looped = false)
        {
            string key;
            if (SFX.NoSoundcard)
            {
                return new InvalidSound("", vol, pitch, pan, looped);
            }
            return SFX._soundHashmap.TryGetKey(sound, out key) ? SFX.Play(key, vol, pitch, pan, looped) : new Sound(SFX._sounds.FirstOrDefault<KeyValuePair<string, SoundEffect>>().Key, 0f, 0f, 0f, false);
        }

        public static int SoundHash(string pSound)
        {
            int num;
            SFX._soundHashmap.TryGetValue(pSound, out num);
            return num;
        }

        public static bool HasSound(string sound)
        {
            if (SFX.NoSoundcard)
                return false;
            SoundEffect pEffect;
            if (!SFX._sounds.TryGetValue(sound, out pEffect))
            {
                if (!sound.Contains(":"))
                    pEffect = Content.Load<SoundEffect>("Audio/SFX/" + sound);
                if (pEffect == null && MonoMain.moddingEnabled && ModLoader.modsEnabled)
                    pEffect = Content.Load<SoundEffect>(sound);
                SFX.RegisterSound(sound, pEffect);
            }
            return pEffect != null;
        }

        public static Sound Get(string sound, float vol = 1f, float pitch = 0f, float pan = 0f, bool looped = false)
        {
            try
            {
                float vol1 = Math.Min(1f, Math.Max(0f, vol));
                return SFX.HasSound(sound) ? new Sound(sound, vol1, pitch, pan, looped) : new InvalidSound(sound, vol1, pitch, pan, looped);
            }
            catch (Exception)
            {
                return new InvalidSound(sound, 0f, pitch, pan, looped);
            }
        }

        public static MultiSound GetMultiSound(string single, string multi)
        {
            if (SFX._multiSounds.ContainsKey(single + multi))
                return SFX._multiSounds[single + multi].GetInstance();
            if (SFX.HasSound(single) && SFX.HasSound(multi))
            {
                MultiSoundUpdater multiSoundUpdater = new MultiSoundUpdater(single + multi, single, multi);
                SFX._multiSounds[single + multi] = multiSoundUpdater;
                return multiSoundUpdater.GetInstance();
            }
            MultiSoundUpdater multiSoundUpdater1 = new MultiSoundUpdater("", "", "");
            SFX._multiSounds[single + multi] = multiSoundUpdater1;
            return multiSoundUpdater1.GetInstance();
        }

        public static SoundEffectInstance GetInstance(
          string sound,
          float vol = 1f,
          float pitch = 0f,
          float pan = 0f,
          bool looped = false)
        {
            float num = Math.Min(1f, Math.Max(0f, vol));
            SoundEffectInstance instance = SFX._sounds[sound].CreateInstance();
            instance.Volume = num;
            instance.Pitch = pitch;
            instance.Pan = pan;
            instance.IsLooped = looped;
            return instance;
        }

        private static void SearchDir(string dir)
        {
            foreach (string file in Content.GetFiles(dir))
                SFX.ProcessSoundEffect(file);
            foreach (string directory in Content.GetDirectories(dir))
                SFX.SearchDir(directory);
        }

        public static void StopAllSounds()
        {
            while (SFX._soundPool.Count > 0)
                SFX._soundPool[0].Stop();
        }

        public static void KillAllSounds()
        {
            while (SFX._soundPool.Count > 0)
                SFX._soundPool[0].Stop();
        }

        private static void ProcessSoundEffect(string path)
        {
            ++SFX._numProcessed;
            path = path.Replace('\\', '/');
            int num = path.IndexOf("Content/Audio/", 0);
            string fileName = path.Substring(num + 8);
            fileName = fileName.Substring(0, fileName.Length - 4);
            MonoMain.lazyLoadActions.Enqueue(() =>
           {
               SoundEffect pEffect = Content.Load<SoundEffect>(fileName);
               if (pEffect == null)
                   return;
               SFX.RegisterSound(fileName.Substring(fileName.IndexOf("/SFX/") + 5), pEffect);
           });
            ++MonoMain.lazyLoadyBits;
        }
    }
}
