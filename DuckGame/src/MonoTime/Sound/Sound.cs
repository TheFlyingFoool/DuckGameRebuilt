using Microsoft.Xna.Framework.Audio;
using System;

namespace DuckGame
{
    public class Sound
    {
        protected bool _killed;
        protected bool _cannotBeCancelled;
        public SoundEffectInstance _instance;
        protected bool _pooled;
        protected string _name = "";
        protected float _volume = 1f;

        public virtual void Kill()
        {
            Stop();
            _killed = true;
        }

        public bool cannotBeCancelled
        {
            get => _cannotBeCancelled;
            set => _cannotBeCancelled = value;
        }

        public virtual bool IsDisposed => _instance == null || _instance.IsDisposed;

        public virtual float Pitch
        {
            get => _instance.Pitch;
            set => _instance.Pitch = value;
        }

        public virtual float Pan
        {
            get => _instance.Pan;
            set => _instance.Pan = value;
        }

        public virtual bool IsLooped
        {
            get => _instance.IsLooped;
            set => _instance.IsLooped = value;
        }

        public virtual SoundState State => _instance != null && !_instance.IsDisposed ? _instance.State : SoundState.Stopped;

        public string name => _name;

        public virtual float Volume
        {
            get => Math.Min(1f, Math.Max(0f, _volume));
            set
            {
                _volume = value;
                if (_instance == null)
                    _instance = SFX.GetInstance(_name, _volume * SFX.volume);
                _instance.Volume = Math.Min(1f, Math.Max(0f, value * SFX.volume));
            }
        }

        public virtual void Play()
        {
            if (_killed || !SFX.PoolSound(this))
                return;
            if (Recorder.currentRecording != null)
                Recorder.currentRecording.LogSound(name, _volume, Pitch, Pan);
            _instance.Volume = Math.Min(1f, Math.Max(0f, _volume * SFX.volume));
            _instance.Play();
            _pooled = true;
        }

        public virtual void Stop()
        {
            if (_killed)
                return;
            _instance._position = 0;
            if (State == SoundState.Playing && !_instance.IsDisposed)
                _instance.Stop();
            _pooled = false;
            SFX.UnpoolSound(this);
        }

        public virtual void Unpooled()
        {
            if (State != SoundState.Stopped && !_instance.IsDisposed)
            {
                _instance.Volume = 0f;
                _instance.Stop();
            }
            _pooled = false;
        }

        public virtual void Pause()
        {
            if (_killed)
                return;
            _instance.Volume = 0f;
            _instance.Pause();
            _pooled = false;
            SFX.UnpoolSound(this);
        }

        public Sound(string sound, float vol, float pitch, float pan, bool looped)
        {
            _name = sound;
            _volume = vol;
            _instance = SFX.GetInstance(sound, _volume * SFX.volume, pitch, pan, looped);
        }

        public Sound()
        {
        }
    }
}
