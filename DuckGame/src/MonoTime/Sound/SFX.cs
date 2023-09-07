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
        //hi hello yes NiK0 here, this int is here so Recorderator can ignore certain SFX that dont need to be saved to the recorderator file
        //to be played back since an item might handle them on their own or whatever the circumstances are -NiK0 
        public static int DontSave = 0;
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
                if (_speech == null)
                {
                    _speech = new Speech();
                    _speech.Initialize();
                    _speech.SetOutputToDefaultAudioDevice();
                    _speech.ApplyTTSSettings();
                }
                return _speech;
            }
        }

        public static bool hasTTS
        {
            get
            {
                return !Program.IsLinuxD && !Program.isLinux && speech != null && speech.GetSayVoices().Count > 0;
            }
        }

        public static void Say(string pString)
        {
            if (Program.IsLinuxD || Program.isLinux || speech == null)
                return;
            speech.Say(pString);
        }

        public static void StopSaying()
        {
            if (Program.IsLinuxD || Program.isLinux || speech == null)
                return;
            speech.StopSaying();
        }

        public static void SetSayVoice(string pName)
        {
            if (Program.IsLinuxD || Program.isLinux)
                return;
            if (speech == null)
                return;
            try
            {
                speech.SetSayVoice(pName);
            }
            catch (Exception ex)
            {
                DevConsole.Log(DCSection.General, "|DGRED|SFX.SetSayVoice failed:" + ex.Message);
            }
        }

        public static List<string> GetSayVoices() => Program.isLinux || speech == null ? new List<string>() : speech.GetSayVoices();

        public static void ApplyTTSSettings()
        {
            if (Program.IsLinuxD || Program.isLinux || speech == null)
                return;
            speech.ApplyTTSSettings();
        }

        public static int RegisterSound(string pSound, SoundEffect pEffect)
        {
            int hashCode = pSound.GetHashCode();
            lock (_sounds)
            {
                _soundHashmap[pSound] = hashCode;
                _sounds[pSound] = pEffect;
            }
            return hashCode;
        }

        public static bool PoolSound(Sound s)
        {
            if (_soundPool.Count > 32)
            {
                bool flag = false;
                for (int index = 0; index < _soundPool.Count; ++index)
                {
                    if (!_soundPool[index].cannotBeCancelled)
                    {
                        UnpoolSound(_soundPool[index]);
                        flag = true;
                        break;
                    }
                }
                if (!flag)
                    return false;
            }
            _soundPool.Add(s);
            return true;
        }

        public static void UnpoolSound(Sound s)
        {
            _soundPool.Remove(s);
            s.Unpooled();
        }

        public static void Initialize()
        {
            _audio = new Windows_Audio();
            _audio.Platform_Initialize();
            if (!Windows_Audio.initialized)
            {
                NoSoundcard = true;
            }
            else
            {
                MonoMain.NloadMessage = "Loading SFX";
                SearchDir("Content/Audio/SFX");
                NetSoundEffect.Initialize();
            }

        }

        public static void Terminate() => _audio.Dispose();

        public static void Update()
        {
            if (Corderator.instance != null && !Recorderator.Playing)
            {
                List<SoundData> ls = new List<SoundData>();
                for (int i = 0; i < _playedThisFrame.Count; i++)
                {
                    Sound s = _playedThisFrame[i];
                    if (s.saveToRecording) ls.Add(new SoundData(SoundHash(s.name), s.Volume, s.Pitch));
                }
                if (Corderator.instance.toAddThisFrame.Count > 0) Corderator.instance.toAddThisFrame.AddRange(ls);
                else Corderator.instance.toAddThisFrame = ls;
            }
            _playedThisFrame.Clear();
            for (int index = 0; index < _soundPool.Count; ++index)
            {
                if (_soundPool[index].State != SoundState.Playing)
                {
                    _soundPool[index].Stop();
                    --index;
                }
            }
            foreach (KeyValuePair<string, MultiSoundUpdater> multiSound in _multiSounds)
                multiSound.Value.Update();
            _audio.Update();
        }

        public static float volume
        {
            get => Math.Min(1f, Math.Max(0f, _volume * _volume)) * 0.9f;
            set => _volume = Math.Min(1f, Math.Max(0f, value));
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
            return PlaySynchronized(sound, vol, pitch, pan, looped, false);
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
            if (!enabled)
                return new InvalidSound(sound, vol, pitch, pan, looped);
            if (Network.isActive)
                Send.Message(new NMSoundEffect(sound, louderForMe ? vol * 0.7f : vol, pitch));
            return Play(sound, vol, pitch, pan, looped);
        }

        public static Sound Play(string sound, float vol = 1f, float pitch = 0f, float pan = 0f, bool looped = false)
        {
            if (!enabled || skip)
                return new InvalidSound(sound, vol, pitch, pan, looped);
            Sound sound1 = _playedThisFrame.FirstOrDefault(x => x.name == sound);
            if (sound1 == null)
            {
                try
                {
                    sound1 = Get(sound, vol, pitch, pan, looped);
                    if (sound1 != null)
                    {
                        sound1.Play();
                        if (!Recorderator.Playing)
                        {
                            sound1.saveToRecording = DontSave == 0;
                            if (DontSave > 0) DontSave--;
                        }
                        _playedThisFrame.Add(sound1);
                    }
                }
                catch (Exception)
                {
                    return new Sound(_sounds.FirstOrDefault().Key, 0f, 0f, 0f, false);
                }
            }
            return sound1;
        }

        public static Sound Play(int sound, float vol = 1f, float pitch = 0f, float pan = 0f, bool looped = false)
        {
            string key;
            if (NoSoundcard)
            {
                return new InvalidSound("", vol, pitch, pan, looped);
            }
            return _soundHashmap.TryGetKey(sound, out key) ? Play(key, vol, pitch, pan, looped) : new Sound(_sounds.FirstOrDefault().Key, 0f, 0f, 0f, false);
        }

        public static int SoundHash(string pSound)
        {
            int num;
            _soundHashmap.TryGetValue(pSound, out num);
            return num;
        }

        public static bool HasSound(string sound)
        {
            if (NoSoundcard)
                return false;
            SoundEffect pEffect;
            if (!_sounds.TryGetValue(sound, out pEffect))
            {
                if (!sound.Contains(":"))
                    pEffect = Content.Load<SoundEffect>("Audio/SFX/" + sound);
                if (pEffect == null && MonoMain.moddingEnabled && ModLoader.modsEnabled)
                    pEffect = Content.Load<SoundEffect>(sound);
                RegisterSound(sound, pEffect);
            }
            return pEffect != null;
        }

        public static Sound Get(string sound, float vol = 1f, float pitch = 0f, float pan = 0f, bool looped = false)
        {
            try
            {
                float vol1 = Math.Min(1f, Math.Max(0f, vol));
                return HasSound(sound) ? new Sound(sound, vol1, pitch, pan, looped) : new InvalidSound(sound, vol1, pitch, pan, looped);
            }
            catch (Exception)
            {
                return new InvalidSound(sound, 0f, pitch, pan, looped);
            }
        }

        public static MultiSound GetMultiSound(string single, string multi)
        {
            if (_multiSounds.ContainsKey(single + multi))
                return _multiSounds[single + multi].GetInstance();
            if (HasSound(single) && HasSound(multi))
            {
                MultiSoundUpdater multiSoundUpdater = new MultiSoundUpdater(single + multi, single, multi);
                _multiSounds[single + multi] = multiSoundUpdater;
                return multiSoundUpdater.GetInstance();
            }
            MultiSoundUpdater multiSoundUpdater1 = new MultiSoundUpdater("", "", "");
            _multiSounds[single + multi] = multiSoundUpdater1;
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
            SoundEffectInstance instance = _sounds[sound].CreateInstance();
            instance.Volume = num;
            instance.Pitch = pitch;
            instance.Pan = pan;
            instance.IsLooped = looped;
            return instance;
        }

        private static void SearchDir(string dir)
        {
            foreach (string file in Content.GetFiles(dir))
                ProcessSoundEffect(file);
            foreach (string directory in Content.GetDirectories(dir))
                SearchDir(directory);
        }

        public static void StopAllSounds()
        {
            while (_soundPool.Count > 0)
                _soundPool[0].Stop();
        }

        public static void KillAllSounds()
        {
            while (_soundPool.Count > 0)
                _soundPool[0].Stop();
        }

        private static void ProcessSoundEffect(string path)
        {
            ++_numProcessed;
            path = path.Replace('\\', '/');
            int num = path.IndexOf("Content/Audio/", 0);
            string fileName = path.Substring(num + 8);
            fileName = fileName.Substring(0, fileName.Length - 4);
            MonoMain.lazyLoadActions.Enqueue(() =>
           {
               SoundEffect pEffect = Content.Load<SoundEffect>(fileName);
               if (pEffect == null)
                   return;
               RegisterSound(fileName.Substring(fileName.IndexOf("/SFX/") + 5), pEffect);
           });
            ++MonoMain.lazyLoadyBits;
        }
    }
}
