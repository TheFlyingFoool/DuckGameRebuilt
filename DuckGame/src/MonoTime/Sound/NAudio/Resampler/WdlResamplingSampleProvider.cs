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
            resampler = new WdlResampler();
            resampler.SetMode(true, 0, false);
            resampler.SetFilterParms();
            resampler.SetFeedMode(false);
            channels = source.WaveFormat.Channels;
            this.source = source;
            sampleRate = pSampleRate;
        }

        public int sampleRate
        {
            get => outFormat.SampleRate;
            set
            {
                if (outFormat != null && outFormat.SampleRate == value)
                    return;
                outFormat = WaveFormat.CreateIeeeFloatWaveFormat(value, channels);
                resampler.SetRates(source.WaveFormat.SampleRate, outFormat.SampleRate);
            }
        }

        /// <summary>Reads from this sample provider</summary>
        public int Read(float[] buffer, int offset, int count)
        {
            int num1 = count / channels;
            float[] inbuffer;
            int inbufferOffset;
            int num2 = resampler.ResamplePrepare(num1, outFormat.Channels, out inbuffer, out inbufferOffset);
            int nsamples_in = source.Read(inbuffer, inbufferOffset, num2 * channels) / channels;
            return resampler.ResampleOut(buffer, offset, nsamples_in, num1, channels) * channels;
        }

        /// <summary>Output WaveFormat</summary>
        public WaveFormat WaveFormat => outFormat;
    }
}