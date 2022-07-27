// Decompiled with JetBrains decompiler
// Type: DuckGame.SoundEffect
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
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

        public static SoundEffect FromStream(Stream stream) => SoundEffect.FromStream(stream, "wav");

        public static SoundEffect FromStream(Stream stream, string extension)
        {
            SoundEffect soundEffect = new SoundEffect();
            if (soundEffect.Platform_Construct(stream, extension))
                return soundEffect;
            DevConsole.Log(DCSection.General, "|DGRED|SoundEffect.FromStream Failed!");
            return (SoundEffect)null;
        }

        public static SoundEffect CreateStreaming(string pPath)
        {
            if (System.IO.File.Exists(pPath))
                return new SoundEffect()
                {
                    streaming = true,
                    file = pPath
                };
            DevConsole.Log(DCSection.General, "|DGRED|SoundEffect.CreateStreaming Failed (file not found)!");
            return (SoundEffect)null;
        }

        public SoundEffect(string pPath)
        {
            this.file = pPath;
            this.Platform_Construct(pPath);
        }

        public SoundEffect()
        {
        }

        public SoundEffectInstance CreateInstance() => new SoundEffectInstance(this);

        public float[] data => this._waveBuffer;

        public WaveFormat format { get; private set; }

        public int decodedSamples => this._decodedSamples;

        public int totalSamples => this._totalSamples;

        public int Decode(float[] pBuffer, int pOffset, int pCount) => this._decoderReader.Read(pBuffer, pOffset, pCount);

        public void Rewind() => this._decode.Seek(0L, SeekOrigin.Begin);

        public bool Decoder_DecodeChunk()
        {
            if (this._decoderReader == null)
                return false;
            lock (this._decoderReader)
            {
                try
                {
                    if (this._decodedSamples + this.kDecoderChunkSize > SoundEffect._songBuffer.Length)
                    {
                        float[] destinationArray = new float[SoundEffect._songBuffer.Length * 2];
                        Array.Copy((Array)SoundEffect._songBuffer, (Array)destinationArray, SoundEffect._songBuffer.Length);
                        SoundEffect._songBuffer = destinationArray;
                    }
                    int num = this._decoderReader.Read(SoundEffect._songBuffer, this._decodedSamples, this.kDecoderChunkSize);
                    if (num > 0)
                    {
                        this._decodedSamples += num;
                    }
                    else
                    {
                        this.dataSize = this._decodedSamples;
                        this._decode.Dispose();
                        this._decode = (WaveStream)null;
                        this._decoderReader = (ISampleProvider)null;
                    }
                    return num > 0;
                }
                catch (Exception ex)
                {
                }
                return false;
            }
        }

        private void Thread_Decoder()
        {
            while (true)
            {
                lock (SoundEffect.kDecoderHandle)
                {
                    if (!this.Decoder_DecodeChunk())
                        break;
                    if (this._decoderIndex != SoundEffect.kDecoderIndex)
                        break;
                }
                Thread.Sleep(10);
            }
        }

        public void Dispose()
        {
            if (this._decoderReader == null)
                return;
            lock (this._decoderReader)
            {
                this._decoderReader = (ISampleProvider)null;
                this._decode.Dispose();
                if (this._stream == null)
                    return;
                this._stream.Close();
                this._stream.Dispose();
            }
        }

        public bool Platform_Construct(Stream pStream, string pExtension)
        {
            pExtension = pExtension.Replace(".", "");
            this._stream = pStream;
            WaveStream waveStream = (WaveStream)null;
            if (pExtension == "wav")
            {
                waveStream = (WaveStream)new WaveFileReader(pStream);
                if (waveStream.WaveFormat.Encoding != WaveFormatEncoding.Pcm && waveStream.WaveFormat.Encoding != WaveFormatEncoding.IeeeFloat)
                    waveStream = (WaveStream)new BlockAlignReductionStream(WaveFormatConversionStream.CreatePcmStream(waveStream));
            }
            else if (pExtension == "mp3")
                waveStream = (WaveStream)new Mp3FileReader(pStream);
            else if (pExtension == "aiff")
                waveStream = (WaveStream)new AiffFileReader(pStream);
            else if (pExtension == "ogg")
            {
                waveStream = (WaveStream)new VorbisWaveReader(pStream);
                float num = 0.0f;
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
                catch (Exception ex)
                {
                    num = 0.0f;
                }
                this.replaygainModifier = Math.Max(0.0f, Math.Min(1f, (float)((double)(100f * (float)Math.Pow(10.0, (double)num / 20.0)) / 100.0 * 1.89999997615814)));
            }
            if (waveStream == null)
                return false;
            this.PrepareReader(waveStream, pStream);
            return true;
        }

        private void PrepareReader(WaveStream reader, Stream pStream)
        {
            this._decode = reader;
            this._totalSamples = (int)(this._decode.Length * 8L / (long)this._decode.WaveFormat.BitsPerSample);
            this._decoderReader = (ISampleProvider)new SampleChannel((IWaveProvider)this._decode);
            if (this._decoderReader.WaveFormat.SampleRate != 44100)
            {
                this._decoderReader = (ISampleProvider)new WdlResamplingSampleProvider(this._decoderReader, 44100);
                this._totalSamples *= this._decoderReader.WaveFormat.BitsPerSample / this._decode.WaveFormat.BitsPerSample;
            }
            this.format = this._decoderReader.WaveFormat;
            this.dataSize = this._totalSamples;
            if (reader is WaveFileReader)
            {
                if (pStream is FileStream)
                {
                    this.streaming = true;
                }
                else
                {
                    this._waveBuffer = new float[this._totalSamples];
                    this._decoderReader.Read(this._waveBuffer, 0, this._totalSamples);
                    this._decode.Dispose();
                    this._decoderReader = (ISampleProvider)null;
                    if (this._stream != null)
                    {
                        this._stream.Dispose();
                        this._stream = (Stream)null;
                    }
                    int num = this._totalSamples * 4 / 1000;
                    ContentPack.kTotalKilobytesAllocated += (long)num;
                    if (ContentPack.currentPreloadPack == null)
                        return;
                    ContentPack.currentPreloadPack.kilobytesPreAllocated += (long)num;
                }
            }
            else
            {
                if (!MonoMain.enableThreadedLoading)
                    return;
                lock (SoundEffect.kDecoderHandle)
                {
                    if (SoundEffect._songBuffer == null)
                        SoundEffect._songBuffer = new float[this._totalSamples];
                    this._waveBuffer = SoundEffect._songBuffer;
                    ++SoundEffect.kDecoderIndex;
                    this._decoderIndex = SoundEffect.kDecoderIndex;
                    Task.Factory.StartNew(new Action(this.Thread_Decoder));
                }
            }
        }

        public void Platform_Construct(string pPath)
        {
            byte[] buffer = System.IO.File.ReadAllBytes(pPath);
            if (buffer == null)
            {
                this.PrepareReader((WaveStream)new AudioFileReader(pPath), (Stream)null);
            }
            else
            {
                if (this.Platform_Construct((Stream)new MemoryStream(buffer), Path.GetExtension(pPath)))
                    return;
                DevConsole.Log(DCSection.General, "Tried to read invalid sound format (" + pPath + ")");
            }
        }

        public float[] Platform_GetData() => this.data;
    }
}
