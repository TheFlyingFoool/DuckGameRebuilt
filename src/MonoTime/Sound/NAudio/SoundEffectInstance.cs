// Decompiled with JetBrains decompiler
// Type: DuckGame.SoundEffectInstance
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using Microsoft.Xna.Framework.Audio;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System;

namespace DuckGame
{
    public class SoundEffectInstance : ISampleProvider
    {
        protected bool _isMusic;
        public ISampleProvider _chainEnd;
        protected VolumeSampleProvider _volumeChain;
        protected SoundEffectInstance.PitchShiftProvider _pitchChain;
        protected PanningSampleProvider _panChain;
        protected SoundEffect _data;
        protected bool _inMixer;
        protected bool _loop;
        protected float _pitch;
        protected float _volume = 1f;
        protected float _pan;
        public int _position;

        public Action SoundEndEvent { get; set; }

        public WaveFormat WaveFormat => _data.format;

        public void SetData(SoundEffect pData)
        {
            _position = 0;
            lock (this)
            {
                _data = pData;
                RebuildChain();
            }
        }

        public SoundEffectInstance(SoundEffect pData) => SetData(pData);

        private void RebuildChain()
        {
            if (_chainEnd != null)
                Windows_Audio.RemoveSound(_chainEnd);
            _chainEnd = this;
            if (_data == null)
                return;
            if (_data.format.Channels == 1 || _pan != 0.0)
            {
                _panChain = new PanningSampleProvider(_chainEnd);
                _chainEnd = _panChain;
                _panChain.Pan = _pan;
            }
            if (_volume != 1.0)
            {
                _volumeChain = new VolumeSampleProvider(_chainEnd);
                _chainEnd = _volumeChain;
                _volumeChain.Volume = _volume * _data.replaygainModifier;
            }
            if (_pitch != 0.0)
            {
                _pitchChain = new SoundEffectInstance.PitchShiftProvider(_chainEnd);
                _chainEnd = _pitchChain;
                _pitchChain.pitch = _pitch;
            }
            if (!_inMixer)
                return;
            Windows_Audio.AddSound(_chainEnd, _isMusic);
        }

        public bool IsDisposed { get; }

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
            if (_data == null)
                return;
            if (_inMixer)
                Stop();
            _inMixer = true;
            Windows_Audio.AddSound(_chainEnd, _isMusic);
        }

        public void Resume()
        {
            if (_data == null || _inMixer)
                return;
            _inMixer = true;
            Windows_Audio.AddSound(_chainEnd, _isMusic);
        }

        public void Stop()
        {
            if (_data == null)
                return;
            Pause();
            _position = 0;
        }

        public void Stop(bool immediate) => Stop();

        public void Pause()
        {
            if (_data == null)
                return;
            _inMixer = false;
        }

        public virtual bool IsLooped
        {
            get => _loop;
            set => _loop = value;
        }

        public float Pitch
        {
            get => _pitch;
            set
            {
                _pitch = value;
                if (_pitch != 0.0 && _pitchChain == null)
                    RebuildChain();
                if (_pitchChain == null)
                    return;
                _pitchChain.pitch = _pitch;
            }
        }

        public float Volume
        {
            get => _volume;
            set
            {
                _volume = value;
                if (_volume != 1.0 && _volumeChain == null)
                    RebuildChain();
                if (_volumeChain == null || _data == null)
                    return;
                _volumeChain.Volume = _volume * _data.replaygainModifier;
            }
        }

        public float Pan
        {
            get => _pan;
            set
            {
                _pan = value;
                if (_pan != 0.0 && _panChain == null)
                    RebuildChain();
                if (_panChain == null)
                    return;
                _panChain.Pan = _pan;
            }
        }

        public SoundState State => !_inMixer ? SoundState.Stopped : SoundState.Playing;

        public virtual int Read(float[] buffer, int offset, int count)
        {
            if (_data == null || !_inMixer)
                return 0;
            int length = 0;
            lock (this)
            {
                int val1 = _data.dataSize - _position;
                if (_data.data == null)
                {
                    length = _data.Decode(buffer, offset, count);
                }
                else
                {
                    length = Math.Min(val1, count);
                    Array.Copy(_data.data, _position, buffer, offset, length);
                }
                _position += length;
                if (length != count)
                {
                    if (SoundEndEvent != null)
                        SoundEndEvent();
                    if (_loop)
                    {
                        _position = 0;
                        offset += length;
                        if (_data.data == null)
                        {
                            _data.Rewind();
                            length = _data.Decode(buffer, offset, count);
                        }
                        else
                        {
                            length = Math.Min(_data.dataSize - _position, count - length);
                            Array.Copy(_data.data, _position, buffer, offset, length);
                        }
                        _position += length;
                        length = count;
                    }
                }
            }
            _inMixer = _inMixer && length == count;
            return length;
        }

        protected void HandleLoop()
        {
        }

        public void Platform_SetProgress(float pProgress)
        {
            if (_data == null)
                return;
            pProgress = Maths.Clamp(pProgress, 0f, 1f);
            _position = (int)(pProgress * _data.data.Length);
        }

        public float Platform_GetProgress() => _data == null ? 1f : _position / (float)_data.data.Length;

        public int Platform_GetLengthInMilliseconds() => _data == null ? 0 : (int)(_data.data.Length * 4 / WaveFormat.AverageBytesPerSecond) * 500;

        public class PitchShiftProvider : ISampleProvider
        {
            private ISampleProvider _chain;
            public float pitch;
            public SoundEffectInstance instance;
            private WdlResamplingSampleProvider _resampler;

            public WaveFormat WaveFormat => _chain.WaveFormat;

            public PitchShiftProvider(ISampleProvider pChain)
            {
                _chain = pChain;
                _resampler = new WdlResamplingSampleProvider(pChain);
            }

            public int Read(float[] buffer, int offset, int count)
            {
                if (pitch < 0.0)
                {
                }
                _resampler.sampleRate = (int)(44100.0 * Math.Pow(2.0, -pitch));
                return _resampler.Read(buffer, offset, count);
            }
        }
    }
}
