using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using ManagedBass;
using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DuckGame
{
    public class SoundEffectInstance
    {
        protected bool _isMusic;
        protected bool _inMixer;
        protected bool _loop;
        protected float _pitch;
        protected float _volume = 1f;
        protected float _pan;
        protected SoundEffect _data;

        public int _position;

        public Action SoundEndEvent { get; set; }

        public void SetData(SoundEffect pData)
        {
            _data = pData;
        }

        public SoundEffectInstance(SoundEffect data)
        {
            _data = data;
        }

        private void RebuildChain()
        {

        }

        public bool IsDisposed { get; }

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
            if (this._inMixer)
            {
                this.Stop();
            }
            this._inMixer = true;
            if (_isMusic)
            {
                AndroidAudio.Play(_data.Handle, true);
                return;
            }
            var h = AndroidAudio.GetSample(_data.Handle);
            AndroidAudio.SetVolume(h, _volume);
            //AndroidAudio.Pitch(h, _pitch - 1);
            AndroidAudio.SetPan(h, _pan);
            AndroidAudio.Loop(h, _isMusic ? true : _loop);
            AndroidAudio.Play(h, false);
        }

        public void Resume()
        {
            if (this._data == null)
            {
                return;
            }
            if (!this._inMixer)
            {
                this._inMixer = true;
                AndroidAudio.Play(_data.Handle, _isMusic);
            }
        }

        public void Stop()
        {
            if (this._data == null)
            {
                return;
            }
            this.Pause();
            this._position = 0;
        }

        public void Stop(bool immediate)
        {
            this.Stop();
        }

        public void Pause()
        {
            if (this._data == null) return;
            if (!_inMixer) return;
            this._inMixer = false;
            if (!_isMusic) return;
            AndroidAudio.Pause(_data.Handle);
        }

        public virtual bool IsLooped
        {
            get {
                return this._loop;
            }
            set {
                this._loop = value;
                if (this._data == null) return;
                if (!_isMusic) return;
                AndroidAudio.Loop(_data.Handle, value);
            }
        }

        public float Pitch
        {
            get {
                return this._pitch;
            }
            set {
                this._pitch = value;
                if (this._data == null) return;
                if (!_isMusic) return;
                AndroidAudio.Pitch(_data.Handle, value);
            }
        }

        public float Volume
        {
            get {
                return this._volume;
            }
            set {
                this._volume = value;
                if (this._data == null) return;
                if (!_isMusic) return;
                AndroidAudio.SetVolume(_data.Handle, value);
            }
        }

        public float Pan
        {
            get {
                return this._pan;
            }
            set {
                this._pan = value;
                if (this._data == null) return;
                if (!_isMusic) return;
                AndroidAudio.SetPan(_data.Handle, value);
            }
        }

        public SoundState State
        {
            get {
                if (!this._inMixer)
                {
                    return SoundState.Stopped;
                }
                return SoundState.Playing;
            }
        }

        public virtual int Read(float[] buffer, int offset, int count)
        {
            return default;
            //if (this._data == null || !this._inMixer)
            //{
            //    return 0;
            //}
            //int samplesToCopy = 0;
            //lock (this)
            //{
            //    int availableSamples = this._data.dataSize - this._position;
            //    if (this._data.data == null)
            //    {
            //        samplesToCopy = this._data.Decode(buffer, offset, count);
            //    }
            //    else
            //    {
            //        samplesToCopy = Math.Min(availableSamples, count);
            //        Array.Copy(this._data.data, this._position, buffer, offset, samplesToCopy);
            //    }
            //    this._position += samplesToCopy;
            //    if (samplesToCopy != count)
            //    {
            //        if (this.SoundEndEvent != null)
            //        {
            //            this.SoundEndEvent();
            //        }
            //        if (this._loop)
            //        {
            //            this._position = 0;
            //            offset += samplesToCopy;
            //            if (this._data.data == null)
            //            {
            //                this._data.Rewind();
            //                samplesToCopy = this._data.Decode(buffer, offset, count);
            //            }
            //            else
            //            {
            //                availableSamples = this._data.dataSize - this._position;
            //                samplesToCopy = Math.Min(availableSamples, count - samplesToCopy);
            //                Array.Copy(this._data.data, this._position, buffer, offset, samplesToCopy);
            //            }
            //            this._position += samplesToCopy;
            //            samplesToCopy = count;
            //        }
            //    }
            //}
            //this._inMixer = (this._inMixer && samplesToCopy == count);
            //return samplesToCopy;
        }

        protected void HandleLoop()
        {
        }

        public void Platform_SetProgress(float pProgress)
        {
            if (this._data == null)
            {
                return;
            }
            pProgress = Maths.Clamp(pProgress, 0f, 1f);
            this._position = (int)(pProgress * (float)this._data.data.Length);
        }

        public float Platform_GetProgress()
        {
            if (this._data == null)
            {
                return 1f;
            }
            _position = (int)AndroidAudio.GetPostion(_data.Handle);
            return (float)this._position / AndroidAudio.GetLength(_data.Handle);
        }

        public int Platform_GetLengthInMilliseconds()
        {
            if (this._data == null)
            {
                return 0;
            }
            return (int)(AndroidAudio.GetLength(_data.Handle, true) / 1000);
        }
    }
}