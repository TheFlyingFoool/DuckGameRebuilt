// Decompiled with JetBrains decompiler
// Type: DuckGame.OggPlayer
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using Microsoft.Xna.Framework.Audio;
using NVorbis;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace DuckGame
{
    public class OggPlayer
    {
        private DynamicSoundEffectInstance _instance;
        private byte[] _buffer;
        private float[] _floatBuffer;
        private bool _iSaidStop;
        private float _replaygainModifier = 1f;
        private float _volume = 1f;
        private bool _shouldLoop;
        private bool _valid = true;
        private Thread _decoderThread;
        private bool _killDecodingThread;
        private bool _initialized;
        private VorbisReader _activeSong;
        private VorbisReader _decoderSong;
        private VorbisReader _streamerSong;
        private object _decoderDataMutex = new object();
        private object _decoderMutex = new object();
        private object _streamingMutex = new object();
        private float[] _decodedData;
        private int _samplesDecoded;
        private int _totalSamplesToDecode;
        private int _decodedSamplePosition;
        //private const int kDecoderChunkSize = 176400;

        public SoundState state => this._instance == null || !this._valid || this._iSaidStop ? SoundState.Stopped : this._instance.State;

        public float volume
        {
            get => this._volume;
            set
            {
                this._volume = value;
                if (this._instance == null)
                    return;
                lock (this._instance)
                    this.ApplyVolume();
            }
        }

        private void ApplyVolume()
        {
            if (!this._valid || this._instance == null || this._instance.State != SoundState.Playing)
                return;
            this._instance.Volume = MathHelper.Clamp(this._volume, 0.0f, 1f) * this._replaygainModifier;
        }

        public bool looped
        {
            get => this._shouldLoop;
            set => this._shouldLoop = value;
        }

        public TimeSpan position => this._activeSong != null && this._valid && this._totalSamplesToDecode > 0 && this._decodedSamplePosition < this._totalSamplesToDecode ? new TimeSpan(0, 0, 0, 0, (int)(this._decodedSamplePosition / this._totalSamplesToDecode / 44100.0) * 500) : new TimeSpan();

        public void Terminate()
        {
            if (this._valid)
                this._instance.Dispose();
            try
            {
                if (this._decoderThread != null)
                    this._decoderThread.Abort();
            }
            catch (Exception)
            {
            }
            this._killDecodingThread = true;
        }

        public void Initialize()
        {
            if (this._initialized)
                return;
            try
            {
                this._instance = new DynamicSoundEffectInstance(44100, AudioChannels.Stereo);
                this._buffer = new byte[this._instance.GetSampleSizeInBytes(TimeSpan.FromMilliseconds(500.0))];
                this._floatBuffer = new float[this._buffer.Length / 2];
                this._instance.BufferNeeded += new EventHandler<EventArgs>(this.Thread_Stream);
                this._decoderThread = new Thread(new ThreadStart(this.Thread_Decoder))
                {
                    CurrentCulture = CultureInfo.InvariantCulture,
                    Priority = ThreadPriority.BelowNormal,
                    IsBackground = true
                };
                this._decoderThread.Start();
            }
            catch
            {
                DevConsole.Log(DCSection.General, "Music player failed to initialize.");
                this._valid = false;
            }
            this._initialized = true;
        }

        private void Thread_Decoder_LoadNewSong()
        {
            if (this._decoderSong == this._activeSong)
                return;
            lock (this._decoderDataMutex)
            {
                lock (this._decoderMutex)
                {
                    if (this._decoderSong == this._activeSong)
                        return;
                    if (this._decoderSong != null)
                        this._decoderSong.Dispose();
                    this._decoderSong = this._activeSong;
                    this._streamerSong = this._activeSong;
                    this._decodedSamplePosition = 0;
                    this._samplesDecoded = 0;
                    if (this._decoderSong == null)
                        return;
                    this._totalSamplesToDecode = (int)(this._decoderSong.TotalSamples * 2L);
                    this._decodedData = new float[this._totalSamplesToDecode];
                }
            }
        }

        private bool Thread_Decoder_DecodeChunk()
        {
            lock (this._decoderMutex)
            {
                if (this._decoderSong != null)
                {
                    if ((double)this.volume != 0.0)
                    {
                        int count = Math.Min(176400, this._totalSamplesToDecode - this._samplesDecoded);
                        if (count > 0)
                        {
                            this._decoderSong.ReadSamples(this._decodedData, this._samplesDecoded, count);
                            this._samplesDecoded += count;
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private void Thread_Decoder()
        {
            while (!this._killDecodingThread)
            {
                this.Thread_Decoder_LoadNewSong();
                if (!this.Thread_Decoder_DecodeChunk())
                    Thread.Sleep(200);
                else
                    Thread.Sleep(20);
            }
        }

        public void SetOgg(MemoryStream ogg)
        {
            if (!this._valid)
                return;
            try
            {
                lock (this._streamingMutex)
                {
                    this.Stop();
                    float num = 0.0f;
                    try
                    {
                        byte[] numArray = new byte[1000];
                        ogg.Position = 0L;
                        ogg.Read(numArray, 0, 1000);
                        string str1 = Encoding.ASCII.GetString(numArray);
                        int index1 = str1.IndexOf("replaygain_track_gain");
                        if (index1 >= 0)
                        {
                            while (str1[index1] != '=' && index1 < str1.Length)
                                ++index1;
                            int index2 = index1 + 1;
                            string str2 = "";
                            for (; str1[index2] != 'd' && index2 < str1.Length; ++index2)
                                str2 += str1[index2].ToString();
                            num = Convert.ToSingle(str2);
                        }
                    }
                    catch (Exception)
                    {
                        num = 0.0f;
                    }
                    this._activeSong = new VorbisReader(ogg, false);
                    this._replaygainModifier = Math.Max(0.0f, Math.Min(1f, (float)((double)(100f * (float)Math.Pow(10.0, (double)num / 20.0)) / 100.0 * 1.89999997615814)));
                    this.Thread_Decoder_LoadNewSong();
                    this.Thread_Decoder_DecodeChunk();
                }
            }
            catch (Exception ex)
            {
                DevConsole.Log(DCSection.General, "OggPlayer.SetOgg failed with exception:");
                DevConsole.Log(DCSection.General, ex.Message);
                this._activeSong = null;
            }
        }

        public void Play()
        {
            if (this._instance == null)
                return;
            lock (this._instance)
            {
                if (!this._valid)
                    return;
                this._instance.Play();
                this.ApplyVolume();
                this._iSaidStop = false;
            }
        }

        public void Pause()
        {
            if (this._instance == null)
                return;
            lock (this._instance)
            {
                if (!this._valid)
                    return;
                this._instance.Pause();
            }
        }

        public void Resume()
        {
            if (this._instance == null)
                return;
            lock (this._instance)
            {
                if (!this._valid)
                    return;
                this._instance.Resume();
                this.ApplyVolume();
                this._iSaidStop = false;
            }
        }

        public void Stop()
        {
            if (this._instance == null)
                return;
            lock (this._instance)
            {
                if (!this._valid)
                    return;
                this._instance.Stop();
                this._iSaidStop = true;
            }
        }

        public void Update()
        {
        }

        private void Thread_Stream(object sender, EventArgs e)
        {
            lock (this._streamingMutex)
            {
                int length = 0;
                lock (this._decoderDataMutex)
                {
                    this.Thread_Decoder_LoadNewSong();
                    if ((double)this.volume == 0.0 || !this._valid || this._decoderSong == null)
                    {
                        for (int index = 0; index < _buffer.Count<byte>(); ++index)
                            this._buffer[index] = 0;
                        this._instance.SubmitBuffer(this._buffer, 0, _buffer.Count<byte>());
                        return;
                    }
                    do
                        ;
                    while (this._samplesDecoded - this._decodedSamplePosition < this._floatBuffer.Length && this.Thread_Decoder_DecodeChunk());
                    length = Math.Min(this._totalSamplesToDecode - this._decodedSamplePosition, this._floatBuffer.Length);
                    if (length > 0)
                    {
                        Array.Copy(_decodedData, this._decodedSamplePosition, _floatBuffer, 0, length);
                        this._decodedSamplePosition += length;
                    }
                    if (length == 0)
                    {
                        if (this._shouldLoop)
                        {
                            this._decodedSamplePosition = 0;
                            Array.Copy(_decodedData, this._decodedSamplePosition, _floatBuffer, 0, this._floatBuffer.Length);
                            this._decodedSamplePosition += this._floatBuffer.Length;
                            length = this._floatBuffer.Length;
                        }
                        else
                        {
                            for (int index = 0; index < this._floatBuffer.Length / 2; ++index)
                            {
                                this._floatBuffer[index * 2] = 0.0f;
                                this._floatBuffer[index * 2 + 1] = 0.0f;
                            }
                            length = this._floatBuffer.Length;
                            this.Stop();
                        }
                    }
                }
                if (length <= 0)
                    return;
                for (int index = 0; index < length; ++index)
                {
                    short num = (short)Math.Max(Math.Min(short.MaxValue * this._floatBuffer[index], short.MaxValue), short.MinValue);
                    this._buffer[index * 2] = (byte)((uint)num & byte.MaxValue);
                    this._buffer[index * 2 + 1] = (byte)(num >> 8 & byte.MaxValue);
                }
                this._instance.SubmitBuffer(this._buffer, 0, length * 2);
            }
        }
    }
}
