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

        public WaveFormat WaveFormat => this._data.format;

        public void SetData(SoundEffect pData)
        {
            this._position = 0;
            lock (this)
            {
                this._data = pData;
                this.RebuildChain();
            }
        }

        public SoundEffectInstance(SoundEffect pData) => this.SetData(pData);

        private void RebuildChain()
        {
            if (this._chainEnd != null)
                Windows_Audio.RemoveSound(this._chainEnd);
            this._chainEnd = this;
            if (this._data == null)
                return;
            if (this._data.format.Channels == 1 || _pan != 0.0)
            {
                this._panChain = new PanningSampleProvider(this._chainEnd);
                this._chainEnd = _panChain;
                this._panChain.Pan = this._pan;
            }
            if (_volume != 1.0)
            {
                this._volumeChain = new VolumeSampleProvider(this._chainEnd);
                this._chainEnd = _volumeChain;
                this._volumeChain.Volume = this._volume * this._data.replaygainModifier;
            }
            if (_pitch != 0.0)
            {
                this._pitchChain = new SoundEffectInstance.PitchShiftProvider(this._chainEnd);
                this._chainEnd = _pitchChain;
                this._pitchChain.pitch = this._pitch;
            }
            if (!this._inMixer)
                return;
            Windows_Audio.AddSound(this._chainEnd, this._isMusic);
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
            if (this._data == null)
                return;
            if (this._inMixer)
                this.Stop();
            this._inMixer = true;
            Windows_Audio.AddSound(this._chainEnd, this._isMusic);
        }

        public void Resume()
        {
            if (this._data == null || this._inMixer)
                return;
            this._inMixer = true;
            Windows_Audio.AddSound(this._chainEnd, this._isMusic);
        }

        public void Stop()
        {
            if (this._data == null)
                return;
            this.Pause();
            this._position = 0;
        }

        public void Stop(bool immediate) => this.Stop();

        public void Pause()
        {
            if (this._data == null)
                return;
            this._inMixer = false;
        }

        public virtual bool IsLooped
        {
            get => this._loop;
            set => this._loop = value;
        }

        public float Pitch
        {
            get => this._pitch;
            set
            {
                this._pitch = value;
                if (_pitch != 0.0 && this._pitchChain == null)
                    this.RebuildChain();
                if (this._pitchChain == null)
                    return;
                this._pitchChain.pitch = this._pitch;
            }
        }

        public float Volume
        {
            get => this._volume;
            set
            {
                this._volume = value;
                if (_volume != 1.0 && this._volumeChain == null)
                    this.RebuildChain();
                if (this._volumeChain == null || this._data == null)
                    return;
                this._volumeChain.Volume = this._volume * this._data.replaygainModifier;
            }
        }

        public float Pan
        {
            get => this._pan;
            set
            {
                this._pan = value;
                if (_pan != 0.0 && this._panChain == null)
                    this.RebuildChain();
                if (this._panChain == null)
                    return;
                this._panChain.Pan = this._pan;
            }
        }

        public SoundState State => !this._inMixer ? SoundState.Stopped : SoundState.Playing;

        public virtual int Read(float[] buffer, int offset, int count)
        {
            if (this._data == null || !this._inMixer)
                return 0;
            int length = 0;
            lock (this)
            {
                int val1 = this._data.dataSize - this._position;
                if (this._data.data == null)
                {
                    length = this._data.Decode(buffer, offset, count);
                }
                else
                {
                    length = Math.Min(val1, count);
                    Array.Copy(_data.data, this._position, buffer, offset, length);
                }
                this._position += length;
                if (length != count)
                {
                    if (this.SoundEndEvent != null)
                        this.SoundEndEvent();
                    if (this._loop)
                    {
                        this._position = 0;
                        offset += length;
                        if (this._data.data == null)
                        {
                            this._data.Rewind();
                            length = this._data.Decode(buffer, offset, count);
                        }
                        else
                        {
                            length = Math.Min(this._data.dataSize - this._position, count - length);
                            Array.Copy(_data.data, this._position, buffer, offset, length);
                        }
                        this._position += length;
                        length = count;
                    }
                }
            }
            this._inMixer = this._inMixer && length == count;
            return length;
        }

        protected void HandleLoop()
        {
        }

        public void Platform_SetProgress(float pProgress)
        {
            if (this._data == null)
                return;
            pProgress = Maths.Clamp(pProgress, 0f, 1f);
            this._position = (int)(pProgress * _data.data.Length);
        }

        public float Platform_GetProgress() => this._data == null ? 1f : _position / (float)this._data.data.Length;

        public int Platform_GetLengthInMilliseconds() => this._data == null ? 0 : (int)(this._data.data.Length * 4 / this.WaveFormat.AverageBytesPerSecond) * 500;

        public class PitchShiftProvider : ISampleProvider
        {
            private ISampleProvider _chain;
            public float pitch;
            public SoundEffectInstance instance;
            private WdlResamplingSampleProvider _resampler;

            public WaveFormat WaveFormat => this._chain.WaveFormat;

            public PitchShiftProvider(ISampleProvider pChain)
            {
                this._chain = pChain;
                this._resampler = new WdlResamplingSampleProvider(pChain);
            }

            public int Read(float[] buffer, int offset, int count)
            {
                if (pitch < 0.0)
                {
                }
                this._resampler.sampleRate = (int)(44100.0 * Math.Pow(2.0, -this.pitch));
                return this._resampler.Read(buffer, offset, count);
            }
        }
    }
}
