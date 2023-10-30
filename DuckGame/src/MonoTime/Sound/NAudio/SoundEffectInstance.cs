using Microsoft.VisualBasic;
using Microsoft.Xna.Framework.Audio;
using System;
using NAudio2.Wave;
namespace DuckGame
{
    public class SoundEffectInstance : ISampleProvider
    {
        protected bool _isMusic;
        public ISampleProvider _chainEnd;
        protected SoundEffect _data;
        protected bool _inMixer;
        protected bool _loop;
        protected float _pitch;
        protected float _volume = 1f;
        protected float _pan;
        public int _position;

        public Action SoundEndEvent { get; set; }

        protected bool _IsDisposed;//dan
        public OggPlayer ogg;//dan
        public Microsoft.Xna.Framework.Audio.SoundEffectInstance soundEffectInstance;//dan
        public SoundState _State = SoundState.Stopped;//dan
        public void SetData(SoundEffect pData)
        {
            if (true)
            {
                if (pData == null)
                {
                    return;
                }
                if (pData.IsOgg)
                {
                    ogg = pData.oggPlayer;
                    ogg.Initialize();
                    ogg.looped = _loop;
                    ogg.pitch = _pitch;
                    ogg.volume = _volume;
                    ogg.pan = _pan;
                }
                else
                {
                    soundEffectInstance = pData.soundEffect.CreateInstance();
                    soundEffectInstance.IsLooped = _loop;
                    soundEffectInstance.Pitch = _pitch;
                    soundEffectInstance.Volume = _volume;
                    soundEffectInstance.Pan = _pan;
                }
            }
            else
            {
            }
        }

        public SoundEffectInstance(SoundEffect pData) => SetData(pData);

        private void RebuildChain()
        {
            if (true)
                return;
        }

        public bool IsDisposed
        {
            get
            {
                if (!true)
                {
                }

                if (soundEffectInstance == null && (ogg == null || ogg.instance != null))
                {
                    return _IsDisposed;
                }
                if (ogg != null && ogg.instance != null)
                {
                    return ogg.instance.IsDisposed;
                }
                return soundEffectInstance.IsDisposed;
            }
        }

        public void Apply3D(AudioListener listener, AudioEmitter emitter)
        {
        }

        public void Apply3D(AudioListener[] listeners, AudioEmitter emitter)
        {
        }

        public void Dispose(bool disposing)
        {
        }

        public void Dispose()
        {
        }

        public void Play()
        {
            if (true)
            {
                if (soundEffectInstance == null && ogg == null)
                {
                    return;
                }
                if (ogg != null)
                {
                    ogg.Play();
                    return;
                }
                soundEffectInstance.Play();
                return;
            }
        }

        public void Resume()
        {
            if (true)
            {
                if (soundEffectInstance == null && ogg == null)
                {
                    return;
                }
                if (ogg != null)
                {
                    ogg.Resume();
                    return;
                }
                soundEffectInstance.Resume();
                return;
            }
        }

        public void Stop()
        {
            if (true)
            {
                if (soundEffectInstance == null && ogg == null)
                {
                    return;
                }
                if (ogg != null)
                {
                    ogg.Stop();
                    return;
                }
                soundEffectInstance.Stop();
                return;
            }
        }

        public void Stop(bool immediate) => Stop();

        public void Pause()
        {
            if (true)
            {
                if (soundEffectInstance == null && ogg == null)
                {
                    return;
                }
                if (ogg != null)
                {
                    ogg.Pause();
                    return;
                }
                soundEffectInstance.Pause();
                return;
            }
        }

        public virtual bool IsLooped
        {
            //get => this._loop;
            //set => this._loop = value;
            get
            {
                if (soundEffectInstance == null && ogg == null)
                {
                    return _loop;
                }
                if (ogg != null)
                {
                    return ogg.looped;
                }
                return soundEffectInstance.IsLooped;
            }
            set
            {
                _loop = value;
                if (soundEffectInstance == null && ogg == null)
                {
                    return;
                }
                if (ogg != null)
                {
                    ogg.looped = value;
                    return;
                }
                soundEffectInstance.IsLooped = value;
            }
        }
        public float Pitch
        {
            get
            {
                if (soundEffectInstance == null && ogg == null)
                {
                    return _pitch;
                }
                if (ogg != null)
                {
                    return ogg.pitch;
                }
                return soundEffectInstance.Pitch;
            }
            set
            {

                _pitch = value;
                if (soundEffectInstance == null && ogg == null)
                {
                    return;
                }
                if (ogg != null)
                {
                    ogg.pitch = value;
                    return;
                }
                soundEffectInstance.Pitch = value;
            }
        }

        public float Volume
        {

            get
            {
                if (soundEffectInstance == null && ogg == null)
                {
                    return _volume;
                }
                if (ogg != null)
                {
                    return ogg.volume;
                }
                return soundEffectInstance.Volume;
            }
            set
            {
                _volume = value;
                if (soundEffectInstance == null && ogg == null)
                {
                    return;
                }
                if (ogg != null)
                {
                    ogg.volume = value;
                    return;
                }
                soundEffectInstance.Volume = value;
            }
        }
        public float Pan
        {
            get
            {
                if (soundEffectInstance == null && ogg == null)
                {
                    return _pan;
                }
                if (ogg != null)
                {
                    return ogg.volume;
                }
                return soundEffectInstance.Pan;
            }
            set
            {
                _pan = value;
                if (soundEffectInstance == null && ogg == null)
                {
                    return;
                }
                if (ogg != null)
                {
                    ogg.pan = value;
                    return;
                }
                soundEffectInstance.Pan = value;
            }
        }
        public SoundState State
        {
            get
            {
                if (soundEffectInstance == null && ogg == null)
                {
                    return _State;
                }
                if (ogg != null)
                {
                    return ogg.state;
                }
                return soundEffectInstance.State;
            }
        }
        public virtual int Read(float[] buffer, int offset, int count)
        {
            return 0;
        }
        protected void HandleLoop()
        {
        }
        public void Platform_SetProgress(float pProgress)
        {
            return;

        }
        public float Platform_GetProgress()
        {
            return 0f;
        }
        public int Platform_GetLengthInMilliseconds()
        {
            return 0;
        }
    }
}

namespace NAudio2.Wave
{
    public interface ISampleProvider
    {

        int Read(float[] buffer, int offset, int count);
    }
}