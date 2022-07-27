// Decompiled with JetBrains decompiler
// Type: DuckGame.WdlResamplingSampleProvider
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using NAudio.Wave;

namespace DuckGame
{
    /// <summary>
    /// Fully managed resampling sample provider, based on the WDL Resampler
    /// </summary>
    public class WdlResamplingSampleProvider : ISampleProvider
    {
        private readonly WdlResampler resampler;
        private WaveFormat outFormat;
        private readonly ISampleProvider source;
        private readonly int channels;

        public WdlResamplingSampleProvider(ISampleProvider source)
          : this(source, source.WaveFormat.SampleRate)
        {
        }

        public WdlResamplingSampleProvider(ISampleProvider source, int pSampleRate)
        {
            this.resampler = new WdlResampler();
            this.resampler.SetMode(true, 0, false);
            this.resampler.SetFilterParms();
            this.resampler.SetFeedMode(false);
            this.channels = source.WaveFormat.Channels;
            this.source = source;
            this.sampleRate = pSampleRate;
        }

        public int sampleRate
        {
            get => this.outFormat.SampleRate;
            set
            {
                if (this.outFormat != null && this.outFormat.SampleRate == value)
                    return;
                this.outFormat = WaveFormat.CreateIeeeFloatWaveFormat(value, this.channels);
                this.resampler.SetRates(source.WaveFormat.SampleRate, outFormat.SampleRate);
            }
        }

        /// <summary>Reads from this sample provider</summary>
        public int Read(float[] buffer, int offset, int count)
        {
            int num1 = count / this.channels;
            float[] inbuffer;
            int inbufferOffset;
            int num2 = this.resampler.ResamplePrepare(num1, this.outFormat.Channels, out inbuffer, out inbufferOffset);
            int nsamples_in = this.source.Read(inbuffer, inbufferOffset, num2 * this.channels) / this.channels;
            return this.resampler.ResampleOut(buffer, offset, nsamples_in, num1, this.channels) * this.channels;
        }

        /// <summary>Output WaveFormat</summary>
        public WaveFormat WaveFormat => this.outFormat;
    }
}
