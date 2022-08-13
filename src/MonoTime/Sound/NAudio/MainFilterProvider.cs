//// Decompiled with JetBrains decompiler
//// Type: DuckGame.MainFilterProvider
////removed for regex reasons Culture=neutral, PublicKeyToken=null
//// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
//// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
//// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

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
