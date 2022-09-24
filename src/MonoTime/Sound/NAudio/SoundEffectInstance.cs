// Decompiled with JetBrains decompiler
// Type: DuckGame.SoundEffectInstance
//removed for regex reasons Culture=neutral, PublicKeyToken=null
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

        protected bool _IsDisposed;//dan
        public OggPlayer ogg;//dan
        public Microsoft.Xna.Framework.Audio.SoundEffectInstance soundEffectInstance;//dan
        public SoundState _State = SoundState.Stopped;//dan
        public void SetData(SoundEffect pData)
        {
            if (Program.IsLinuxD)
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
                _position = 0;
                lock (this)
                {
                    _data = pData;
                    RebuildChain();
                }
            }
        }

        public SoundEffectInstance(SoundEffect pData) => SetData(pData);

        private void RebuildChain()
        {
            if (Program.IsLinuxD)
                return;
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

        public bool IsDisposed
        {
            get
            {
                if (!Program.IsLinuxD)
                    return false;
                if (this.soundEffectInstance == null && (this.ogg == null || this.ogg.instance != null))
                {
                    return _IsDisposed;
                }
                if (this.ogg != null && this.ogg.instance != null)
                {
                    return this.ogg.instance.IsDisposed;
                }
                return this.soundEffectInstance.IsDisposed;
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
            if (Program.IsLinuxD)
            {
                if (this.soundEffectInstance == null && this.ogg == null)
                {
                    return;
                }
                if (this.ogg != null)
                {
                    this.ogg.Play();
                    return;
                }
                this.soundEffectInstance.Play();
                return;
            }
            if (_data == null)
                return;
            if (_inMixer)
                Stop();
            _inMixer = true;
            Windows_Audio.AddSound(_chainEnd, _isMusic);
        }

        public void Resume()
        {
            if (Program.IsLinuxD)
            {
                if (this.soundEffectInstance == null && this.ogg == null)
                {
                    return;
                }
                if (this.ogg != null)
                {
                    this.ogg.Resume();
                    return;
                }
                this.soundEffectInstance.Resume();
                return;
            }
            if (_data == null || _inMixer)
                return;
            _inMixer = true;
            Windows_Audio.AddSound(_chainEnd, _isMusic);
        }

        public void Stop()
        {
            if (Program.IsLinuxD)
            {
                if (this.soundEffectInstance == null && this.ogg == null)
                {
                    return;
                }
                if (this.ogg != null)
                {
                    this.ogg.Stop();
                    return;
                }
                this.soundEffectInstance.Stop();
                return;
            }
            if (_data == null)
                return;
            Pause();
            _position = 0;
        }

        public void Stop(bool immediate) => Stop();

        public void Pause()
        {
            if (Program.IsLinuxD)
            {
                if (this.soundEffectInstance == null && this.ogg == null)
                {
                    return;
                }
                if (this.ogg != null)
                {
                    this.ogg.Pause();
                    return;
                }
                this.soundEffectInstance.Pause();
                return;
            }
            if (_data == null)
                return;
            _inMixer = false;
        }

        public virtual bool IsLooped
        {
            //get => this._loop;
            //set => this._loop = value;
            get
            {
                if (!Program.IsLinuxD)
                {
                    return this._loop;
                }
                if (this.soundEffectInstance == null && this.ogg == null)
                {
                    return _loop;
                }
                if (this.ogg != null)
                {
                    return this.ogg.looped;
                }
                return this.soundEffectInstance.IsLooped;
            }
            set
            {
                this._loop = value;
                if (!Program.IsLinuxD)
                {
                    return;
                }
                if (this.soundEffectInstance == null && this.ogg == null)
                {
                    return;
                }
                if (this.ogg != null)
                {
                    this.ogg.looped = value;
                    return;
                }
                this.soundEffectInstance.IsLooped = value;
            }
        }
        public float Pitch
        {
            get
            {
                if (!Program.IsLinuxD)
                {
                    return this._pitch;
                }
                if (this.soundEffectInstance == null && this.ogg == null)
                {
                    return _pitch;
                }
                if (this.ogg != null)
                {
                    return this.ogg.pitch;
                }
                return this.soundEffectInstance.Pitch;
            }
            set
            {

                this._pitch = value;
                if (!Program.IsLinuxD)
                {
                    _pitch = value;
                    if (_pitch != 0.0 && _pitchChain == null)
                        RebuildChain();
                    if (_pitchChain == null)
                        return;
                    _pitchChain.pitch = _pitch;
                    return;
                }
                if (this.soundEffectInstance == null && this.ogg == null)
                {
                    return;
                }
                if (this.ogg != null)
                {
                    this.ogg.pitch = value;
                    return;
                }
                this.soundEffectInstance.Pitch = value;
            }
        }

        public float Volume
        {

            get
            {
                if (!Program.IsLinuxD)
                {
                    return this._volume;
                }
                if (this.soundEffectInstance == null && this.ogg == null)
                {
                    return _volume;
                }
                if (this.ogg != null)
                {
                    return this.ogg.volume;
                }
                return this.soundEffectInstance.Volume;
            }
            set
            {
                this._volume = value;
                if (!Program.IsLinuxD)
                {
                    if (_volume != 1.0 && _volumeChain == null)
                        RebuildChain();
                    if (_volumeChain == null || _data == null)
                        return;
                    _volumeChain.Volume = _volume * _data.replaygainModifier;
                    return;
                }
                if (this.soundEffectInstance == null && this.ogg == null)
                {
                    return;
                }
                if (this.ogg != null)
                {
                    this.ogg.volume = value;
                    return;
                }
                this.soundEffectInstance.Volume = value;
            }
        }
        public float Pan
        {
            get
            {
                if (!Program.IsLinuxD)
                {
                    return this._pan;
                }
                if (this.soundEffectInstance == null && this.ogg == null)
                {
                    return _pan;
                }
                if (this.ogg != null)
                {
                    return this.ogg.volume;
                }
                return this.soundEffectInstance.Pan;
            }
            set
            {
                this._pan = value;
                if (!Program.IsLinuxD)
                {
                    if (_pan != 0.0 && _panChain == null)
                        RebuildChain();
                    if (_panChain == null)
                        return;
                    _panChain.Pan = _pan;
                    return;
                }
                if (this.soundEffectInstance == null && this.ogg == null)
                {
                    return;
                }
                if (this.ogg != null)
                {
                    this.ogg.pan = value;
                    return;
                }
                this.soundEffectInstance.Pan = value;
            }
        }
        public SoundState State
        {
            get
            {
                if (!Program.IsLinuxD)
                {
                    return !_inMixer ? SoundState.Stopped : SoundState.Playing;
                }
                if (this.soundEffectInstance == null && this.ogg == null)
                {
                    return _State;
                }
                if (this.ogg != null)
                {
                    return this.ogg.state;
                }
                return this.soundEffectInstance.State;
            }
        }

        public virtual int Read(float[] buffer, int offset, int count)
        {
            if (!Program.IsLinuxD  || _data == null || !_inMixer)
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
            if (Program.IsLinuxD)
                return;
            if (_data == null)
                return;
            pProgress = Maths.Clamp(pProgress, 0f, 1f);
            _position = (int)(pProgress * _data.data.Length);
        }

        public float Platform_GetProgress()
        {
            if (Program.IsLinuxD)
                return 0f;
            return _data == null ? 1f : _position / (float)_data.data.Length;
        }

        public int Platform_GetLengthInMilliseconds()
        {
            if (Program.IsLinuxD)
                return 0;
            return _data == null ? 0 : (int)(_data.data.Length * 4 / WaveFormat.AverageBytesPerSecond) * 500;
        }

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
