using Microsoft.Xna.Framework.Audio;
using System;

namespace DuckGame
{
    public class InvalidSound : Sound
    {
        private float _pitch;
        private float _pan;
        private bool _isLooped;

        public override float Pitch
        {
            get => _pitch;
            set => _pitch = value;
        }

        public override float Pan
        {
            get => _pan;
            set => _pan = value;
        }

        public override bool IsLooped
        {
            get => _isLooped;
            set => _isLooped = value;
        }

        public override SoundState State => SoundState.Stopped;

        public override float Volume
        {
            get => Math.Min(1f, Math.Max(0f, _volume));
            set => _volume = Math.Min(1f, Math.Max(0f, value));
        }

        public override void Play()
        {
        }

        public override void Stop()
        {
        }

        public override void Unpooled()
        {
        }

        public override void Pause()
        {
        }

        public override bool IsDisposed => false;

        public InvalidSound(string sound, float vol, float pitch, float pan, bool looped)
        {
            _name = sound;
            _volume = vol;
            _pitch = pitch;
            _pan = pan;
            _isLooped = looped;
        }
    }
}
