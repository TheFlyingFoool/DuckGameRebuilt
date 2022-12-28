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
        protected PitchShiftProvider _pitchChain;
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
                _pitchChain = new PitchShiftProvider(_chainEnd);
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
            if (Program.IsLinuxD)
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
            if (_data == null || _inMixer)
                return;
            _inMixer = true;
            Windows_Audio.AddSound(_chainEnd, _isMusic);
        }

        public void Stop()
        {
            if (Program.IsLinuxD)
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
                    return _loop;
                }
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
                if (!Program.IsLinuxD)
                {
                    return;
                }
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
                if (!Program.IsLinuxD)
                {
                    return _pitch;
                }
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
                if (!Program.IsLinuxD)
                {
                    return _volume;
                }
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
                if (!Program.IsLinuxD)
                {
                    if (_volume != 1.0f && _volumeChain == null)
                        RebuildChain();
                    if (_volumeChain == null || _data == null)
                        return;
                    _volumeChain.Volume = _volume * _data.replaygainModifier;
                    return;
                }
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
                if (!Program.IsLinuxD)
                {
                    return _pan;
                }
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
                if (!Program.IsLinuxD)
                {
                    if (_pan != 0.0 && _panChain == null)
                        RebuildChain();
                    if (_panChain == null)
                        return;
                    _panChain.Pan = _pan;
                    return;
                }
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
                if (!Program.IsLinuxD)
                {
                    return !_inMixer ? SoundState.Stopped : SoundState.Playing;
                }
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
            if (Program.IsLinuxD || _data == null || !_inMixer)
            {
                return 0;
            }
            int samplesToCopy = 0;
            lock (this)
            {
                int availableSamples = _data.dataSize - _position;
                if (_data.data == null)
                {
                    samplesToCopy = _data.Decode(buffer, offset, count);
                }
                else
                {
                    samplesToCopy = Math.Min(availableSamples, count);
                    Array.Copy(_data.data, _position, buffer, offset, samplesToCopy);
                }
                _position += samplesToCopy;
                if (samplesToCopy != count)
                {
                    if (SoundEndEvent != null)
                    {
                        SoundEndEvent();
                    }
                    if (_loop)
                    {
                        _position = 0;
                        offset += samplesToCopy;
                        if (_data.data == null)
                        {
                            _data.Rewind();
                            samplesToCopy = _data.Decode(buffer, offset, count);
                        }
                        else
                        {
                            availableSamples = _data.dataSize - _position;
                            samplesToCopy = Math.Min(availableSamples, count - samplesToCopy);
                            Array.Copy(_data.data, _position, buffer, offset, samplesToCopy);
                        }
                        _position += samplesToCopy;
                        samplesToCopy = count;
                    }
                }
            }
            _inMixer = (_inMixer && samplesToCopy == count);
            return samplesToCopy;
        }
        protected void HandleLoop()
        {
        }
        public void Platform_SetProgress(float pProgress)
        {
            if (Program.IsLinuxD || _data == null)
            {
                return;
            }
            pProgress = Maths.Clamp(pProgress, 0f, 1f);
            _position = (int)(pProgress * _data.data.Length);
        }
        public float Platform_GetProgress()
        {
            if (Program.IsLinuxD)
            {
                return 0f;
            }
            if (_data == null)
            {
                return 1f;
            }
            return _position / (float)_data.data.Length;
        }
        public int Platform_GetLengthInMilliseconds()
        {
            if (Program.IsLinuxD || _data == null)
            {
                return 0;
            }
            return (int)(_data.data.Length * 4 / (float)WaveFormat.AverageBytesPerSecond) * 500;
        }
        public class PitchShiftProvider : ISampleProvider
        {
            public WaveFormat WaveFormat
            {
                get
                {
                    return _chain.WaveFormat;
                }
            }

            public PitchShiftProvider(ISampleProvider pChain)
            {
                _chain = pChain;
                _resampler = new WdlResamplingSampleProvider(pChain);
            }

            public int Read(float[] buffer, int offset, int count)
            {
                //float num = this.pitch;
                //if (this.pitch < 0f)
                //{
                //    float num2 = this.pitch;
                //}
                _resampler.sampleRate = (int)(44100.0 * Math.Pow(2.0, (double)(-(double)pitch)));
                return _resampler.Read(buffer, offset, count);
            }

            private ISampleProvider _chain;

            public float pitch;

            public SoundEffectInstance instance;

            private WdlResamplingSampleProvider _resampler;
        }
    }
}
