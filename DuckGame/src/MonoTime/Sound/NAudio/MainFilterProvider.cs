//using NAudio.Dsp;
//using NAudio.Wave;

//namespace DuckGame
//{
//    internal class MainFilterProvider : ISampleProvider
//    {
//        private ISampleProvider _chain;
//        private BiQuadFilter _filter;

//        public WaveFormat WaveFormat => this._chain.WaveFormat;

//        public MainFilterProvider(ISampleProvider pChain)
//        {
//            this._chain = pChain;
//            this._filter = BiQuadFilter.LowPassFilter(44100f, 12000f, 1f);
//        }

//        public int Read(float[] buffer, int offset, int count)
//        {
//            this._chain.Read(buffer, offset, count);
//            int num;
//            for (num = 0; num < count; ++num)
//                buffer[offset + num] = this._filter.Transform(buffer[offset + num]);
//            return num;
//        }
//    }
//}
