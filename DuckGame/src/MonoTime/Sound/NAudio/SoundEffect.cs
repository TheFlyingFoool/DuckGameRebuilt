// Decompiled with JetBrains decompiler
// Type: DuckGame.SoundEffect
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using NVorbis.NAudioSupport;
using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DuckGame
{
    public class SoundEffect
    {
        public bool IsOgg;//dan
        public OggPlayer oggPlayer;//dan
        public Microsoft.Xna.Framework.Audio.SoundEffect soundEffect;//dan

        public string file;
        public bool streaming;
        public static float[] _songBuffer;
        private float[] _waveBuffer;
        public int dataSize;
        public WaveStream _decode;
        //private Thread _decoderThread;
        private ISampleProvider _decoderReader;
        private int _decodedSamples;
        private int _totalSamples;
        private int kDecoderChunkSize = 22050;
        private static int kDecoderIndex = 0;
        private static object kDecoderHandle = new object();
        private int _decoderIndex;
        public float replaygainModifier = 1f;
        private Stream _stream;

        public static float DistanceScale { get; set; }

        public static float DopplerScale { get; set; }

        public static float MasterVolume { get; set; }

        public static float SpeedOfSound { get; set; }

        public TimeSpan Duration { get; }

        public bool IsDisposed { get; }

        public string Name { get; set; }

        public static SoundEffect FromStream(Stream stream) => FromStream(stream, "wav");
        public SoundEffect(Stream stream)
        {
            soundEffect = Microsoft.Xna.Framework.Audio.SoundEffect.FromStream(stream);
        }
        public static SoundEffect FromStream(Stream stream, string extension)
        {
            if (Program.IsLinuxD)
            {
                try
                {
                    return new SoundEffect(stream);
                }
                catch (Exception)
                { }
                DevConsole.Log(DCSection.General, "|DGRED|SoundEffect.FromStream Failed!", -1);
                return null;
            }
            SoundEffect soundEffect = new SoundEffect();
            if (soundEffect.Platform_Construct(stream, extension))
                return soundEffect;
            DevConsole.Log(DCSection.General, "|DGRED|SoundEffect.FromStream Failed!");
            return null;
        }

        public static SoundEffect CreateStreaming(string pPath)
        {
            if (File.Exists(pPath))
                return new SoundEffect()
                {
                    streaming = true,
                    file = pPath
                };
            DevConsole.Log(DCSection.General, "|DGRED|SoundEffect.CreateStreaming Failed (file not found)!");
            return null;
        }

        public SoundEffect(string pPath)
        {
            file = pPath;
            Platform_Construct(pPath);
        }

        public SoundEffect()
        {
        }

        public SoundEffectInstance CreateInstance() => new SoundEffectInstance(this);

        public float[] data => _waveBuffer;

        public WaveFormat format { get; private set; }

        public int decodedSamples => _decodedSamples;

        public int totalSamples => _totalSamples;

        public int Decode(float[] pBuffer, int pOffset, int pCount) => _decoderReader.Read(pBuffer, pOffset, pCount);

        public void Rewind() => _decode.Seek(0L, SeekOrigin.Begin);

        public bool Decoder_DecodeChunk()
        {
            if (_decoderReader == null)
                return false;
            lock (_decoderReader)
            {
                try
                {
                    if (_decodedSamples + kDecoderChunkSize > _songBuffer.Length)
                    {
                        float[] destinationArray = new float[_songBuffer.Length * 2];
                        Array.Copy(_songBuffer, destinationArray, _songBuffer.Length);
                        _songBuffer = destinationArray;
                    }
                    int num = _decoderReader.Read(_songBuffer, _decodedSamples, kDecoderChunkSize);
                    if (num > 0)
                    {
                        _decodedSamples += num;
                    }
                    else
                    {
                        dataSize = _decodedSamples;
                        _decode.Dispose();
                        _decode = null;
                        _decoderReader = null;
                    }
                    return num > 0;
                }
                catch (Exception)
                {
                }
                return false;
            }
        }

        private void Thread_Decoder()
        {
            while (true)
            {
                lock (kDecoderHandle)
                {
                    if (!Decoder_DecodeChunk())
                        break;
                    if (_decoderIndex != kDecoderIndex)
                        break;
                }
                Thread.Sleep(10);
            }
        }

        public void Dispose()
        {
            if (_decoderReader == null)
                return;
            lock (_decoderReader)
            {
                _decoderReader = null;
                _decode.Dispose();
                if (_stream == null)
                    return;
                _stream.Close();
                _stream.Dispose();
            }
        }

        public bool Platform_Construct(Stream pStream, string pExtension)
        {
            pExtension = pExtension.Replace(".", "");
            _stream = pStream;
            WaveStream waveStream = null;
            if (pExtension == "wav")
            {
                waveStream = new WaveFileReader(pStream);
                if (waveStream.WaveFormat.Encoding != WaveFormatEncoding.Pcm && waveStream.WaveFormat.Encoding != WaveFormatEncoding.IeeeFloat)
                    waveStream = new BlockAlignReductionStream(WaveFormatConversionStream.CreatePcmStream(waveStream));
            }
            else if (pExtension == "mp3")
                waveStream = new Mp3FileReader(pStream);
            else if (pExtension == "aiff")
                waveStream = new AiffFileReader(pStream);
            else if (pExtension == "ogg")
            {
                waveStream = new VorbisWaveReader(pStream);
                float num = 0f;
                try
                {
                    byte[] numArray = new byte[1000];
                    pStream.Position = 0L;
                    pStream.Read(numArray, 0, 1000);
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
                    pStream.Position = 0L;
                }
                catch (Exception)
                {
                    num = 0f;
                }
                replaygainModifier = Math.Max(0f, Math.Min(1f, (float)((100f * (float)Math.Pow(10.0, num / 20.0)) / 100.0 * 1.9f)));
            }
            if (waveStream == null)
                return false;
            PrepareReader(waveStream, pStream);
            return true;
        }

        private void PrepareReader(WaveStream reader, Stream pStream)
        {
            _decode = reader;
            _totalSamples = (int)(_decode.Length * 8L / _decode.WaveFormat.BitsPerSample);
            _decoderReader = new SampleChannel(_decode);
            if (_decoderReader.WaveFormat.SampleRate != 44100)
            {
                _decoderReader = new WdlResamplingSampleProvider(_decoderReader, 44100);
                _totalSamples *= _decoderReader.WaveFormat.BitsPerSample / _decode.WaveFormat.BitsPerSample;
            }
            format = _decoderReader.WaveFormat;
            dataSize = _totalSamples;
            if (reader is WaveFileReader)
            {
                if (pStream is FileStream)
                {
                    streaming = true;
                }
                else
                {
                    _waveBuffer = new float[_totalSamples];
                    _decoderReader.Read(_waveBuffer, 0, _totalSamples);
                    _decode.Dispose();
                    _decoderReader = null;
                    if (_stream != null)
                    {
                        _stream.Dispose();
                        _stream = null;
                    }
                    int num = _totalSamples * 4 / 1000;
                    ContentPack.kTotalKilobytesAllocated += num;
                    if (ContentPack.currentPreloadPack == null)
                        return;
                    ContentPack.currentPreloadPack.kilobytesPreAllocated += num;
                }
            }
            else
            {
                if (!MonoMain.enableThreadedLoading)
                    return;
                lock (kDecoderHandle)
                {
                    if (_songBuffer == null)
                        _songBuffer = new float[_totalSamples];
                    _waveBuffer = _songBuffer;
                    ++kDecoderIndex;
                    _decoderIndex = kDecoderIndex;
                    //Task.Factory.StartNew(new Action(Thread_Decoder));
                }
            }
        }

        public void Platform_Construct(string pPath)
        {
            if (Program.IsLinuxD)
            {
                int index = pPath.LastIndexOf(".");
                byte[] data = File.ReadAllBytes(pPath);
                if (index != -1 && pPath.Substring(index + 1).ToLower() == "ogg")
                {
                    IsOgg = true;
                    oggPlayer = new OggPlayer();
                    oggPlayer.SetOgg(new MemoryStream(data));
                }
                else
                {

                    soundEffect = Microsoft.Xna.Framework.Audio.SoundEffect.FromStream(new MemoryStream(data));
                }
                return;
            }
            byte[] buffer = File.ReadAllBytes(pPath);
            if (buffer == null)
            {
                PrepareReader(new AudioFileReader(pPath), null);
            }
            else
            {
                if (Platform_Construct(new MemoryStream(buffer), Path.GetExtension(pPath)))
                    return;
                DevConsole.Log(DCSection.General, "Tried to read invalid sound format (" + pPath + ")");
            }
        }

        public float[] Platform_GetData() => data;
    }
}
