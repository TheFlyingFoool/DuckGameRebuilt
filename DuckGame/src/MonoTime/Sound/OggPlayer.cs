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

        private float _pitch; //dan
        private float _pan; //dan

        public SoundState state => _instance == null || !_valid || _iSaidStop ? SoundState.Stopped : _instance.State;
        public DynamicSoundEffectInstance instance
        {
            get { return _instance; }
            set { _instance = value; }
        }
        public float volume
        {
            get => _volume;
            set
            {
                _volume = value;
                if (_instance == null)
                    return;
                lock (_instance)
                    ApplyVolume();
            }
        }

        private void ApplyVolume()
        {
            if (!_valid || _instance == null || _instance.State != SoundState.Playing)
                return;
            _instance.Volume = MathHelper.Clamp(_volume, 0f, 1f) * _replaygainModifier;
        }


        private void ApplyPitch() //dan
        {
            if (!_valid || _instance == null || _instance.State != SoundState.Playing)
                return;
            _instance.Pitch = _pitch;
        }
        private void ApplyPan() //dan
        {
            if (!_valid || _instance == null || _instance.State != SoundState.Playing)
                return;
            _instance.Pan = _pan;
        }
        public float pitch //dan
        {
            get => _pitch;
            set
            {
                bool changed = _pitch != value;
                _pitch = value;
                if (_instance == null)
                    return;
                if (changed)
                {
                    lock (_instance)
                        ApplyPitch();
                }

            }
        }
        public float pan //dan
        {
            get => _pan;
            set
            {
                bool changed = _pan != value;
                _pan = value;
                if (_instance == null)
                    return;
                if (changed)
                {
                    lock (_instance)
                        ApplyPan();
                }
            }
        }

        public bool looped
        {
            get => _shouldLoop;
            set => _shouldLoop = value;
        }

        public TimeSpan position => _activeSong != null && _valid && _totalSamplesToDecode > 0 && _decodedSamplePosition < _totalSamplesToDecode ? new TimeSpan(0, 0, 0, 0, (int)(_decodedSamplePosition / _totalSamplesToDecode / 44100.0) * 500) : new TimeSpan();

        public void Terminate()
        {
            if (_valid)
                _instance.Dispose();
            try
            {
                if (_decoderThread != null)
                    _decoderThread.Abort();
            }
            catch (Exception)
            {
            }
            _killDecodingThread = true;
        }

        public void Initialize()
        {
            if (_initialized)
                return;
            try
            {
                _instance = new DynamicSoundEffectInstance(44100, AudioChannels.Stereo);
                _buffer = new byte[_instance.GetSampleSizeInBytes(TimeSpan.FromMilliseconds(500.0))];
                _floatBuffer = new float[_buffer.Length / 2];
                _instance.BufferNeeded += new EventHandler<EventArgs>(Thread_Stream);
                _decoderThread = new Thread(new ThreadStart(Thread_Decoder))
                {
                    CurrentCulture = CultureInfo.InvariantCulture,
                    Priority = ThreadPriority.BelowNormal,
                    IsBackground = true
                };
                _decoderThread.Start();
            }
            catch
            {
                DevConsole.Log(DCSection.General, "Music player failed to initialize.");
                _valid = false;
            }
            _initialized = true;
        }

        private void Thread_Decoder_LoadNewSong()
        {
            if (_decoderSong == _activeSong)
                return;
            lock (_decoderDataMutex)
            {
                lock (_decoderMutex)
                {
                    if (_decoderSong == _activeSong)
                        return;
                    if (_decoderSong != null)
                        _decoderSong.Dispose();
                    _decoderSong = _activeSong;
                    _streamerSong = _activeSong;
                    _decodedSamplePosition = 0;
                    _samplesDecoded = 0;
                    if (_decoderSong == null)
                        return;
                    _totalSamplesToDecode = (int)(_decoderSong.TotalSamples * 2L);
                    _decodedData = new float[_totalSamplesToDecode];
                }
            }
        }

        private bool Thread_Decoder_DecodeChunk()
        {
            lock (_decoderMutex)
            {
                if (_decoderSong != null)
                {
                    if (volume != 0.0)
                    {
                        int count = Math.Min(176400, _totalSamplesToDecode - _samplesDecoded);
                        if (count > 0)
                        {
                            _decoderSong.ReadSamples(_decodedData, _samplesDecoded, count);
                            _samplesDecoded += count;
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private void Thread_Decoder()
        {
            while (!_killDecodingThread)
            {
                Thread_Decoder_LoadNewSong();
                if (!Thread_Decoder_DecodeChunk())
                    Thread.Sleep(200);
                else
                    Thread.Sleep(20);
            }
        }

        public void SetOgg(MemoryStream ogg)
        {
            if (!_valid)
                return;
            try
            {
                lock (_streamingMutex)
                {
                    Stop();
                    float num = 0f;
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
                        num = 0f;
                    }
                    _activeSong = new VorbisReader(ogg, false);
                    _replaygainModifier = Math.Max(0f, Math.Min(1f, (float)((100f * (float)Math.Pow(10.0, num / 20.0)) / 100.0 * 1.9f)));
                    Thread_Decoder_LoadNewSong();
                    Thread_Decoder_DecodeChunk();
                }
            }
            catch (Exception ex)
            {
                DevConsole.Log(DCSection.General, "OggPlayer.SetOgg failed with exception:");
                DevConsole.Log(DCSection.General, ex.Message);
                _activeSong = null;
            }
        }

        public void Play()
        {
            if (_instance == null)
                return;
            lock (_instance)
            {
                if (!_valid)
                    return;
                _instance.Play();
                ApplyVolume();
                _iSaidStop = false;
            }
        }

        public void Pause()
        {
            if (_instance == null)
                return;
            lock (_instance)
            {
                if (!_valid)
                    return;
                _instance.Pause();
            }
        }

        public void Resume()
        {
            if (_instance == null)
                return;
            lock (_instance)
            {
                if (!_valid)
                    return;
                _instance.Resume();
                ApplyVolume();
                _iSaidStop = false;
            }
        }

        public void Stop()
        {
            if (_instance == null)
                return;
            lock (_instance)
            {
                if (!_valid)
                    return;
                _instance.Stop();
                _iSaidStop = true;
            }
        }

        public void Update()
        {
        }

        private void Thread_Stream(object sender, EventArgs e)
        {
            lock (_streamingMutex)
            {
                int length = 0;
                lock (_decoderDataMutex)
                {
                    Thread_Decoder_LoadNewSong();
                    if (volume == 0.0 || !_valid || _decoderSong == null)
                    {
                        for (int index = 0; index < _buffer.Length; ++index)
                            _buffer[index] = 0;
                        _instance.SubmitBuffer(_buffer, 0, _buffer.Length);
                        return;
                    }
                    do
                        ;
                    while (_samplesDecoded - _decodedSamplePosition < _floatBuffer.Length && Thread_Decoder_DecodeChunk());
                    length = Math.Min(_totalSamplesToDecode - _decodedSamplePosition, _floatBuffer.Length);
                    if (length > 0)
                    {
                        Array.Copy(_decodedData, _decodedSamplePosition, _floatBuffer, 0, length);
                        _decodedSamplePosition += length;
                    }
                    if (length == 0)
                    {
                        if (_shouldLoop)
                        {
                            _decodedSamplePosition = 0;
                            Array.Copy(_decodedData, _decodedSamplePosition, _floatBuffer, 0, _floatBuffer.Length);
                            _decodedSamplePosition += _floatBuffer.Length;
                            length = _floatBuffer.Length;
                        }
                        else
                        {
                            for (int index = 0; index < _floatBuffer.Length / 2; ++index)
                            {
                                _floatBuffer[index * 2] = 0f;
                                _floatBuffer[index * 2 + 1] = 0f;
                            }
                            length = _floatBuffer.Length;
                            Stop();
                        }
                    }
                }
                if (length <= 0)
                    return;
                for (int index = 0; index < length; ++index)
                {
                    short num = (short)Math.Max(Math.Min(short.MaxValue * _floatBuffer[index], short.MaxValue), short.MinValue);
                    _buffer[index * 2] = (byte)((uint)num & byte.MaxValue);
                    _buffer[index * 2 + 1] = (byte)(num >> 8 & byte.MaxValue);
                }
                _instance.SubmitBuffer(_buffer, 0, length * 2);
            }
        }
    }
}
