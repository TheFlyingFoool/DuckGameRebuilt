using Microsoft.Xna.Framework.Audio;
using System.Collections.Generic;

namespace DuckGame
{
    public class MultiSoundUpdater : Sound
    {
        private List<MultiSound> _instances = new List<MultiSound>();
        private SoundEffectInstance _single;
        private SoundEffectInstance _multi;
        private SoundState _state = SoundState.Stopped;
        private new string _name = "";
        private int _playCount;

        public override void Kill()
        {
            for (int index = 0; index < _playCount; ++index)
                Stop();
            _killed = true;
        }

        public MultiSound GetInstance() => new MultiSound(this);

        public override bool IsDisposed
        {
            get
            {
                if (_single == null)
                    return false;
                return _single.IsDisposed || _multi.IsDisposed;
            }
        }

        public override float Pitch
        {
            get => _single != null ? _single.Pitch : 0f;
            set
            {
                if (_single == null)
                    return;
                _single.Pitch = value;
                _multi.Pitch = value;
            }
        }

        public override float Pan
        {
            get => _single != null ? _single.Pan : 0f;
            set
            {
                if (_single == null)
                    return;
                _single.Pan = value;
                _single.Pan = value;
            }
        }

        public override bool IsLooped
        {
            get => _single != null && _single.IsLooped;
            set
            {
                if (_single == null)
                    return;
                _single.IsLooped = value;
                _multi.IsLooped = value;
            }
        }

        public override SoundState State => _state;

        public new string name => _name;

        public void Play(MultiSound who)
        {
            if (_killed || _single == null || _instances.Contains(who))
                return;
            if (_playCount == 0 && SFX.PoolSound(this))
            {
                _single.Volume = _volume * SFX.volume;
                _single.Play();
            }
            ++_playCount;
            _state = SoundState.Playing;
            _pooled = true;
            _instances.Add(who);
        }

        public override void Stop()
        {
            while (_instances.Count > 0)
                _instances[0].Stop();
            _volume = 0f;
        }

        public void Stop(MultiSound who)
        {
            if (_killed || _single == null || !_instances.Contains(who))
                return;
            if (_state == SoundState.Playing)
                --_playCount;
            if (_playCount == 0)
            {
                _single.Volume = 0f;
                _single.Stop();
                _multi.Volume = 0f;
                _multi.Stop();
                _pooled = false;
                SFX.UnpoolSound(this);
                _state = SoundState.Stopped;
            }
            _instances.Remove(who);
        }

        public override void Unpooled()
        {
            if (_single != null && _state != SoundState.Stopped)
            {
                _single.Volume = 0f;
                _single.Stop();
                _multi.Volume = 0f;
                _multi.Stop();
            }
            _pooled = false;
        }

        public void Update()
        {
            if (_single == null)
                return;
            float num1 = 0f;
            foreach (MultiSound instance in _instances)
                num1 += instance.Volume;
            int count = _instances.Count;
            float num2 = num1 / (count > 0 ? count : 1f);
            _volume = Lerp.Float(_volume, (float)(num2 * 0.7f + Maths.Clamp(_instances.Count, 0, 4) / 4f * num2 * 0.3f), 0.05f);
            if (_state != SoundState.Playing)
                return;
            if (_playCount > 1)
            {
                if (_multi.State == SoundState.Stopped)
                    _multi.Play();
                if (_single.State != SoundState.Stopped)
                {
                    _single.Volume = Lerp.Float(_single.Volume, 0f, 0.05f);
                    if (_single.Volume < 0.02f)
                    {
                        _single.Volume = 0f;
                        _single.Stop();
                    }
                }
                _multi.Volume = Lerp.Float(_multi.Volume, _volume * SFX.volume, 0.05f);
            }
            else
            {
                if (_playCount != 1)
                    return;
                if (_single.State == SoundState.Stopped)
                    _single.Play();
                if (_multi.State != SoundState.Stopped)
                {
                    _multi.Volume = Lerp.Float(_multi.Volume, 0f, 0.05f);
                    if (_multi.Volume < 0.02f)
                    {
                        _multi.Volume = 0f;
                        _multi.Stop();
                    }
                }
                _single.Volume = Lerp.Float(_single.Volume, _volume * SFX.volume, 0.05f);
            }
        }

        public MultiSoundUpdater(string id, string single, string multi)
        {
            _name = id;
            if (id != "")
            {
                _single = SFX.GetInstance(single, 0f, looped: true);
                _multi = SFX.GetInstance(multi, 0f, looped: true);
            }
            _cannotBeCancelled = true;
            _volume = 1f;
        }
    }
}
