// Decompiled with JetBrains decompiler
// Type: DuckGame.WdlResampler
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;

namespace DuckGame
{
    /// <summary>
    /// Fully managed resampler, based on Cockos WDL Resampler
    /// </summary>
    internal class WdlResampler
    {
        private const int WDL_RESAMPLE_MAX_FILTERS = 4;
        private const int WDL_RESAMPLE_MAX_NCH = 64;
        private const double PI = 3.14159265358979;
        private double m_sratein;
        private double m_srateout;
        private double m_fracpos;
        private double m_ratio;
        private double m_filter_ratio;
        private float m_filterq;
        private float m_filterpos;
        private float[] m_rsinbuf;
        private float[] m_filter_coeffs;
        private WdlResampler.WDL_Resampler_IIRFilter m_iirfilter;
        private int m_filter_coeffs_size;
        private int m_last_requested;
        private int m_filtlatency;
        private int m_samples_in_rsinbuf;
        private int m_lp_oversize;
        private int m_sincsize;
        private int m_filtercnt;
        private int m_sincoversize;
        private bool m_interp;
        private bool m_feedmode;

        /// <summary>Creates a new Resampler</summary>
        public WdlResampler()
        {
            this.m_filterq = 0.707f;
            this.m_filterpos = 0.693f;
            this.m_sincoversize = 0;
            this.m_lp_oversize = 1;
            this.m_sincsize = 0;
            this.m_filtercnt = 1;
            this.m_interp = true;
            this.m_feedmode = false;
            this.m_filter_coeffs_size = 0;
            this.m_sratein = 44100.0;
            this.m_srateout = 44100.0;
            this.m_ratio = 1.0;
            this.m_filter_ratio = -1.0;
            this.Reset();
        }

        /// <summary>
        /// sets the mode
        /// if sinc set, it overrides interp or filtercnt
        /// </summary>
        public void SetMode(
          bool interp,
          int filtercnt,
          bool sinc,
          int sinc_size = 64,
          int sinc_interpsize = 32)
        {
            this.m_sincsize = !sinc || sinc_size < 4 ? 0 : (sinc_size > 8192 ? 8192 : sinc_size);
            this.m_sincoversize = this.m_sincsize != 0 ? (sinc_interpsize <= 1 ? 1 : (sinc_interpsize >= 4096 ? 4096 : sinc_interpsize)) : 1;
            this.m_filtercnt = this.m_sincsize != 0 ? 0 : (filtercnt <= 0 ? 0 : (filtercnt >= 4 ? 4 : filtercnt));
            this.m_interp = interp && this.m_sincsize == 0;
            if (this.m_sincsize == 0)
            {
                this.m_filter_coeffs = new float[0];
                this.m_filter_coeffs_size = 0;
            }
            if (this.m_filtercnt != 0)
                return;
            this.m_iirfilter = (WdlResampler.WDL_Resampler_IIRFilter)null;
        }

        /// <summary>
        /// Sets the filter parameters
        /// used for filtercnt&gt;0 but not sinc
        /// </summary>
        public void SetFilterParms(float filterpos = 0.693f, float filterq = 0.707f)
        {
            this.m_filterpos = filterpos;
            this.m_filterq = filterq;
        }

        /// <summary>Set feed mode</summary>
        /// <param name="wantInputDriven">if true, that means the first parameter to ResamplePrepare will specify however much input you have, not how much you want</param>
        public void SetFeedMode(bool wantInputDriven) => this.m_feedmode = wantInputDriven;

        /// <summary>Reset</summary>
        public void Reset(double fracpos = 0.0)
        {
            this.m_last_requested = 0;
            this.m_filtlatency = 0;
            this.m_fracpos = fracpos;
            this.m_samples_in_rsinbuf = 0;
            if (this.m_iirfilter == null)
                return;
            this.m_iirfilter.Reset();
        }

        public void SetRates(double rate_in, double rate_out)
        {
            if (rate_in < 1.0)
                rate_in = 1.0;
            if (rate_out < 1.0)
                rate_out = 1.0;
            if (rate_in == this.m_sratein && rate_out == this.m_srateout)
                return;
            this.m_sratein = rate_in;
            this.m_srateout = rate_out;
            this.m_ratio = this.m_sratein / this.m_srateout;
        }

        public double GetCurrentLatency()
        {
            double currentLatency = ((double)this.m_samples_in_rsinbuf - (double)this.m_filtlatency) / this.m_sratein;
            if (currentLatency < 0.0)
                currentLatency = 0.0;
            return currentLatency;
        }

        /// <summary>
        /// Prepare
        /// note that it is safe to call ResamplePrepare without calling ResampleOut (the next call of ResamplePrepare will function as normal)
        /// nb inbuffer was WDL_ResampleSample **, returning a place to put the in buffer, so we return a buffer and offset
        /// </summary>
        /// <param name="out_samples">req_samples is output samples desired if !wantInputDriven, or if wantInputDriven is input samples that we have</param>
        /// <param name="nch"></param>
        /// <param name="inbuffer"></param>
        /// <param name="inbufferOffset"></param>
        /// <returns>returns number of samples desired (put these into *inbuffer)</returns>
        public int ResamplePrepare(
          int out_samples,
          int nch,
          out float[] inbuffer,
          out int inbufferOffset)
        {
            if (nch > 64 || nch < 1)
            {
                inbuffer = (float[])null;
                inbufferOffset = 0;
                return 0;
            }
            int num1 = 0;
            if (this.m_sincsize > 1)
                num1 = this.m_sincsize;
            int num2 = num1 / 2;
            if (num2 > 1 && this.m_samples_in_rsinbuf < num2 - 1)
            {
                this.m_filtlatency += num2 - 1 - this.m_samples_in_rsinbuf;
                this.m_samples_in_rsinbuf = num2 - 1;
                if (this.m_samples_in_rsinbuf > 0)
                    this.m_rsinbuf = new float[this.m_samples_in_rsinbuf * nch];
            }
            int num3 = this.m_feedmode ? out_samples : (int)(this.m_ratio * (double)out_samples) + 4 + num1 - this.m_samples_in_rsinbuf;
            if (num3 < 0)
                num3 = 0;
            int num4;
            while (true)
            {
                Array.Resize<float>(ref this.m_rsinbuf, (this.m_samples_in_rsinbuf + num3) * nch);
                num4 = this.m_rsinbuf.Length / (nch != 0 ? nch : 1) - this.m_samples_in_rsinbuf;
                if (num4 != num3)
                {
                    if (num3 > 4 && num4 == 0)
                        num3 /= 2;
                    else
                        break;
                }
                else
                    goto label_13;
            }
            num3 = num4;
        label_13:
            inbuffer = this.m_rsinbuf;
            inbufferOffset = this.m_samples_in_rsinbuf * nch;
            this.m_last_requested = num3;
            return num3;
        }

        public int ResampleOut(
          float[] outBuffer,
          int outBufferIndex,
          int nsamples_in,
          int nsamples_out,
          int nch)
        {
            if (nch > 64 || nch < 1)
                return 0;
            if (this.m_filtercnt > 0 && this.m_ratio > 1.0 && nsamples_in > 0)
            {
                if (this.m_iirfilter == null)
                    this.m_iirfilter = new WdlResampler.WDL_Resampler_IIRFilter();
                int filtercnt = this.m_filtercnt;
                this.m_iirfilter.setParms(1.0 / this.m_ratio * (double)this.m_filterpos, (double)this.m_filterq);
                int num1 = this.m_samples_in_rsinbuf * nch;
                int num2 = 0;
                for (int index1 = 0; index1 < nch; ++index1)
                {
                    for (int index2 = 0; index2 < filtercnt; ++index2)
                        this.m_iirfilter.Apply(this.m_rsinbuf, num1 + index1, this.m_rsinbuf, num1 + index1, nsamples_in, nch, num2++);
                }
            }
            this.m_samples_in_rsinbuf += Math.Min(nsamples_in, this.m_last_requested);
            int num3 = this.m_samples_in_rsinbuf;
            if (nsamples_in < this.m_last_requested)
            {
                int num4 = (this.m_last_requested - nsamples_in) * 2 + this.m_sincsize * 2;
                int newSize = (this.m_samples_in_rsinbuf + num4) * nch;
                Array.Resize<float>(ref this.m_rsinbuf, newSize);
                if (this.m_rsinbuf.Length == newSize)
                {
                    Array.Clear((Array)this.m_rsinbuf, this.m_samples_in_rsinbuf * nch, num4 * nch);
                    num3 = this.m_samples_in_rsinbuf + num4;
                }
            }
            int ns = 0;
            double fracpos = this.m_fracpos;
            double ratio = this.m_ratio;
            int destinationIndex = 0;
            int index3 = outBufferIndex;
            int num5 = nsamples_out;
            int num6 = 0;
            if (this.m_sincsize != 0)
            {
                if (this.m_ratio > 1.0)
                    this.BuildLowPass(1.0 / (this.m_ratio * 1.03));
                else
                    this.BuildLowPass(1.0);
                int filterCoeffsSize = this.m_filter_coeffs_size;
                int num7 = num3 - filterCoeffsSize;
                num6 = filterCoeffsSize / 2 - 1;
                int filterIndex = 0;
                switch (nch)
                {
                    case 1:
                        while (num5-- != 0)
                        {
                            int num8 = (int)fracpos;
                            if (num8 < num7 - 1)
                            {
                                this.SincSample1(outBuffer, index3, this.m_rsinbuf, destinationIndex + num8, fracpos - (double)num8, this.m_filter_coeffs, filterIndex, filterCoeffsSize);
                                ++index3;
                                fracpos += ratio;
                                ++ns;
                            }
                            else
                                break;
                        }
                        break;
                    case 2:
                        while (num5-- != 0)
                        {
                            int num9 = (int)fracpos;
                            if (num9 < num7 - 1)
                            {
                                this.SincSample2(outBuffer, index3, this.m_rsinbuf, destinationIndex + num9 * 2, fracpos - (double)num9, this.m_filter_coeffs, filterIndex, filterCoeffsSize);
                                index3 += 2;
                                fracpos += ratio;
                                ++ns;
                            }
                            else
                                break;
                        }
                        break;
                    default:
                        while (num5-- != 0)
                        {
                            int num10 = (int)fracpos;
                            if (num10 < num7 - 1)
                            {
                                this.SincSample(outBuffer, index3, this.m_rsinbuf, destinationIndex + num10 * nch, fracpos - (double)num10, nch, this.m_filter_coeffs, filterIndex, filterCoeffsSize);
                                index3 += nch;
                                fracpos += ratio;
                                ++ns;
                            }
                            else
                                break;
                        }
                        break;
                }
            }
            else if (!this.m_interp)
            {
                switch (nch)
                {
                    case 1:
                        while (num5-- != 0)
                        {
                            int num11 = (int)fracpos;
                            if (num11 < num3)
                            {
                                outBuffer[index3++] = this.m_rsinbuf[destinationIndex + num11];
                                fracpos += ratio;
                                ++ns;
                            }
                            else
                                break;
                        }
                        break;
                    case 2:
                        while (num5-- != 0)
                        {
                            int num12 = (int)fracpos;
                            if (num12 < num3)
                            {
                                int num13 = num12 + num12;
                                outBuffer[index3] = this.m_rsinbuf[destinationIndex + num13];
                                outBuffer[index3 + 1] = this.m_rsinbuf[destinationIndex + num13 + 1];
                                index3 += 2;
                                fracpos += ratio;
                                ++ns;
                            }
                            else
                                break;
                        }
                        break;
                    default:
                        while (num5-- != 0)
                        {
                            int num14 = (int)fracpos;
                            if (num14 < num3)
                            {
                                Array.Copy((Array)this.m_rsinbuf, destinationIndex + num14 * nch, (Array)outBuffer, index3, nch);
                                index3 += nch;
                                fracpos += ratio;
                                ++ns;
                            }
                            else
                                break;
                        }
                        break;
                }
            }
            else
            {
                switch (nch)
                {
                    case 1:
                        while (num5-- != 0)
                        {
                            int num15 = (int)fracpos;
                            double num16 = fracpos - (double)num15;
                            if (num15 < num3 - 1)
                            {
                                double num17 = 1.0 - num16;
                                int index4 = destinationIndex + num15;
                                outBuffer[index3++] = (float)((double)this.m_rsinbuf[index4] * num17 + (double)this.m_rsinbuf[index4 + 1] * num16);
                                fracpos += ratio;
                                ++ns;
                            }
                            else
                                break;
                        }
                        break;
                    case 2:
                        while (num5-- != 0)
                        {
                            int num18 = (int)fracpos;
                            double num19 = fracpos - (double)num18;
                            if (num18 < num3 - 1)
                            {
                                double num20 = 1.0 - num19;
                                int index5 = destinationIndex + num18 * 2;
                                outBuffer[index3] = (float)((double)this.m_rsinbuf[index5] * num20 + (double)this.m_rsinbuf[index5 + 2] * num19);
                                outBuffer[index3 + 1] = (float)((double)this.m_rsinbuf[index5 + 1] * num20 + (double)this.m_rsinbuf[index5 + 3] * num19);
                                index3 += 2;
                                fracpos += ratio;
                                ++ns;
                            }
                            else
                                break;
                        }
                        break;
                    default:
                        while (num5-- != 0)
                        {
                            int num21 = (int)fracpos;
                            double num22 = fracpos - (double)num21;
                            if (num21 < num3 - 1)
                            {
                                double num23 = 1.0 - num22;
                                int num24 = nch;
                                int index6 = destinationIndex + num21 * nch;
                                while (num24-- != 0)
                                {
                                    outBuffer[index3++] = (float)((double)this.m_rsinbuf[index6] * num23 + (double)this.m_rsinbuf[index6 + nch] * num22);
                                    ++index6;
                                }
                                fracpos += ratio;
                                ++ns;
                            }
                            else
                                break;
                        }
                        break;
                }
            }
            if (this.m_filtercnt > 0 && this.m_ratio < 1.0 && ns > 0)
            {
                if (this.m_iirfilter == null)
                    this.m_iirfilter = new WdlResampler.WDL_Resampler_IIRFilter();
                int filtercnt = this.m_filtercnt;
                this.m_iirfilter.setParms(this.m_ratio * (double)this.m_filterpos, (double)this.m_filterq);
                int num25 = 0;
                for (int index7 = 0; index7 < nch; ++index7)
                {
                    for (int index8 = 0; index8 < filtercnt; ++index8)
                        this.m_iirfilter.Apply(outBuffer, index7, outBuffer, index7, ns, nch, num25++);
                }
            }
            if (ns > 0 && num3 > this.m_samples_in_rsinbuf)
            {
                double num26 = (fracpos - (double)this.m_samples_in_rsinbuf + (double)num6) / ratio;
                if (num26 > 0.0)
                {
                    ns -= (int)(num26 + 0.5);
                    if (ns < 0)
                        ns = 0;
                }
            }
            int num27 = (int)fracpos;
            this.m_fracpos = fracpos - (double)num27;
            this.m_samples_in_rsinbuf -= num27;
            if (this.m_samples_in_rsinbuf <= 0)
                this.m_samples_in_rsinbuf = 0;
            else
                Array.Copy((Array)this.m_rsinbuf, destinationIndex + num27 * nch, (Array)this.m_rsinbuf, destinationIndex, this.m_samples_in_rsinbuf * nch);
            return ns;
        }

        private void BuildLowPass(double filtpos)
        {
            int sincsize = this.m_sincsize;
            int sincoversize = this.m_sincoversize;
            if (this.m_filter_ratio == filtpos && this.m_filter_coeffs_size == sincsize && this.m_lp_oversize == sincoversize)
                return;
            this.m_lp_oversize = sincoversize;
            this.m_filter_ratio = filtpos;
            int newSize = (sincsize + 1) * this.m_lp_oversize;
            Array.Resize<float>(ref this.m_filter_coeffs, newSize);
            if (this.m_filter_coeffs.Length == newSize)
            {
                this.m_filter_coeffs_size = sincsize;
                int num1 = sincsize * this.m_lp_oversize;
                int num2 = num1 / 2;
                double num3 = 0.0;
                double d = 0.0;
                double num4 = 2.0 * Math.PI / (double)num1;
                double num5 = Math.PI / (double)this.m_lp_oversize * filtpos;
                double a = num5 * (double)-num2;
                for (int index = -num2; index < num2 + this.m_lp_oversize; ++index)
                {
                    double num6 = 287.0 / 800.0 - 0.48829 * Math.Cos(d) + 0.14128 * Math.Cos(2.0 * d) - 0.01168 * Math.Cos(6.0 * d);
                    if (index != 0)
                        num6 *= Math.Sin(a) / a;
                    d += num4;
                    a += num5;
                    this.m_filter_coeffs[num2 + index] = (float)num6;
                    if (index < num2)
                        num3 += num6;
                }
                double num7 = (double)this.m_lp_oversize / num3;
                for (int index = 0; index < num1 + this.m_lp_oversize; ++index)
                    this.m_filter_coeffs[index] = this.m_filter_coeffs[index] * (float)num7;
            }
            else
                this.m_filter_coeffs_size = 0;
        }

        private void SincSample(
          float[] outBuffer,
          int outBufferIndex,
          float[] inBuffer,
          int inBufferIndex,
          double fracpos,
          int nch,
          float[] filter,
          int filterIndex,
          int filtsz)
        {
            int lpOversize = this.m_lp_oversize;
            fracpos *= (double)lpOversize;
            int num1 = (int)fracpos;
            filterIndex += lpOversize - 1 - num1;
            fracpos -= (double)num1;
            for (int index1 = 0; index1 < nch; ++index1)
            {
                double num2 = 0.0;
                double num3 = 0.0;
                int index2 = filterIndex;
                int index3 = inBufferIndex + index1;
                int num4 = filtsz;
                while (num4-- != 0)
                {
                    num2 += (double)filter[index2] * (double)inBuffer[index3];
                    num3 += (double)filter[index2 + 1] * (double)inBuffer[index3];
                    index3 += nch;
                    index2 += lpOversize;
                }
                outBuffer[outBufferIndex + index1] = (float)(num2 * fracpos + num3 * (1.0 - fracpos));
            }
        }

        private void SincSample1(
          float[] outBuffer,
          int outBufferIndex,
          float[] inBuffer,
          int inBufferIndex,
          double fracpos,
          float[] filter,
          int filterIndex,
          int filtsz)
        {
            int lpOversize = this.m_lp_oversize;
            fracpos *= (double)lpOversize;
            int num1 = (int)fracpos;
            filterIndex += lpOversize - 1 - num1;
            fracpos -= (double)num1;
            double num2 = 0.0;
            double num3 = 0.0;
            int index1 = filterIndex;
            int index2 = inBufferIndex;
            int num4 = filtsz;
            while (num4-- != 0)
            {
                num2 += (double)filter[index1] * (double)inBuffer[index2];
                num3 += (double)filter[index1 + 1] * (double)inBuffer[index2];
                ++index2;
                index1 += lpOversize;
            }
            outBuffer[outBufferIndex] = (float)(num2 * fracpos + num3 * (1.0 - fracpos));
        }

        private void SincSample2(
          float[] outptr,
          int outBufferIndex,
          float[] inBuffer,
          int inBufferIndex,
          double fracpos,
          float[] filter,
          int filterIndex,
          int filtsz)
        {
            int lpOversize = this.m_lp_oversize;
            fracpos *= (double)lpOversize;
            int num1 = (int)fracpos;
            filterIndex += lpOversize - 1 - num1;
            fracpos -= (double)num1;
            double num2 = 0.0;
            double num3 = 0.0;
            double num4 = 0.0;
            double num5 = 0.0;
            int index1 = filterIndex;
            int index2 = inBufferIndex;
            int num6 = filtsz / 2;
            while (num6-- != 0)
            {
                double num7 = num2 + (double)filter[index1] * (double)inBuffer[index2];
                double num8 = num3 + (double)filter[index1] * (double)inBuffer[index2 + 1];
                double num9 = num4 + (double)filter[index1 + 1] * (double)inBuffer[index2];
                double num10 = num5 + (double)filter[index1 + 1] * (double)inBuffer[index2 + 1];
                num2 = num7 + (double)filter[index1 + lpOversize] * (double)inBuffer[index2 + 2];
                num3 = num8 + (double)filter[index1 + lpOversize] * (double)inBuffer[index2 + 3];
                num4 = num9 + (double)filter[index1 + lpOversize + 1] * (double)inBuffer[index2 + 2];
                num5 = num10 + (double)filter[index1 + lpOversize + 1] * (double)inBuffer[index2 + 3];
                index2 += 4;
                index1 += lpOversize * 2;
            }
            outptr[outBufferIndex] = (float)(num2 * fracpos + num4 * (1.0 - fracpos));
            outptr[outBufferIndex + 1] = (float)(num3 * fracpos + num5 * (1.0 - fracpos));
        }

        private class WDL_Resampler_IIRFilter
        {
            private double m_fpos;
            private double m_a1;
            private double m_a2;
            private double m_b0;
            private double m_b1;
            private double m_b2;
            private double[,] m_hist;

            public WDL_Resampler_IIRFilter()
            {
                this.m_fpos = -1.0;
                this.Reset();
            }

            public void Reset() => this.m_hist = new double[256, 4];

            public void setParms(double fpos, double Q)
            {
                if (Math.Abs(fpos - this.m_fpos) < 1E-06)
                    return;
                this.m_fpos = fpos;
                double num1 = fpos * Math.PI;
                double num2 = Math.Cos(num1);
                double num3 = Math.Sin(num1) / (2.0 * Q);
                double num4 = 1.0 / (1.0 + num3);
                this.m_b1 = (1.0 - num2) * num4;
                this.m_b2 = this.m_b0 = this.m_b1 * 0.5;
                this.m_a1 = -2.0 * num2 * num4;
                this.m_a2 = (1.0 - num3) * num4;
            }

            public void Apply(
              float[] inBuffer,
              int inIndex,
              float[] outBuffer,
              int outIndex,
              int ns,
              int span,
              int w)
            {
                double b0 = this.m_b0;
                double b1 = this.m_b1;
                double b2 = this.m_b2;
                double a1 = this.m_a1;
                double a2 = this.m_a2;
                while (ns-- != 0)
                {
                    double num = (double)inBuffer[inIndex];
                    inIndex += span;
                    double x = num * b0 + this.m_hist[w, 0] * b1 + this.m_hist[w, 1] * b2 - this.m_hist[w, 2] * a1 - this.m_hist[w, 3] * a2;
                    this.m_hist[w, 1] = this.m_hist[w, 0];
                    this.m_hist[w, 0] = num;
                    this.m_hist[w, 3] = this.m_hist[w, 2];
                    this.m_hist[w, 2] = this.denormal_filter(x);
                    outBuffer[outIndex] = (float)this.m_hist[w, 2];
                    outIndex += span;
                }
            }

            private double denormal_filter(float x) => (double)x;

            private double denormal_filter(double x) => x;
        }
    }
}
