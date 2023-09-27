// Decompiled with JetBrains decompiler
// Type: DuckGame.ConstantSound
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using Microsoft.Xna.Framework.Audio;

namespace DuckGame
{
    public class ConstantSound : IAutoUpdate
    {
        private Sound _effect;
        public Sound effect
        {
            get
            {
                return _effect;
            }
            set
            {
                _effect = value;
            }
        }
        private float _lerpVolume;
        private float _lerpSpeed = 0.1f;
        private bool _killSound;
        private Level _startLevel;

        public float volume
        {
            get => _effect == null ? 1f : _effect.Volume;
            set
            {
                if (_effect != null)
                    _effect.Volume = value;
                _lerpVolume = value;
            }
        }

        public float lerpVolume
        {
            get => _lerpVolume;
            set => _lerpVolume = value;
        }

        public float lerpSpeed
        {
            get => _lerpSpeed;
            set => _lerpSpeed = value;
        }

        public float pitch
        {
            get => _effect == null ? 0f : _effect.Pitch;
            set
            {
                if (_effect == null)
                    return;
                _effect.Pitch = value;
            }
        }

        public ConstantSound(string sound, float startVolume = 0f, float startPitch = 0f, string multiSound = null)
        {
            AutoUpdatables.Add(this);
            _effect = startVolume <= 0f ? (multiSound == null ? SFX.Get(sound, startVolume * SFX.volume, startPitch, looped: true) : SFX.GetMultiSound(sound, multiSound)) : SFX.Play(sound, startVolume * SFX.volume, startPitch, looped: true);
            if (_effect != null)
                return;
            DevConsole.Log("ConstantSound not found! (" + sound + ")");
            _effect = new InvalidSound(sound, startVolume, startPitch, 0f, true);
        }

        ~ConstantSound()
        {
            _lerpSpeed = 0f;
            _lerpVolume = 0f;
        }

        public void Kill() => _killSound = true;

        public void Mute()
        {
            if (_effect == null || _effect.IsDisposed)
                return;
            _effect.Volume = 0f;
        }

        public void Update()
        {
            if (_effect != null && _effect.IsDisposed)
                _effect = null;
            else if (_effect == null || _startLevel != null && Level.current != _startLevel)
            {
                if (_effect == null)
                    return;
                _effect.Kill();
            }
            else if (_killSound)
            {
                _effect.Stop();
            }
            else
            {
                if (_effect.Volume > 0.01f && _effect.State != SoundState.Playing)
                {
                    _effect.Play();
                    _startLevel = Level.current;
                }
                else if (_effect.Volume < 0.01f && _effect.State == SoundState.Playing)
                    _effect.Stop();
                _effect.Volume = Maths.LerpTowards(_effect.Volume, _lerpVolume, _lerpSpeed);
            }
        }
    }
}
