// Decompiled with JetBrains decompiler
// Type: DuckGame.SN76489Core
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public sealed class SN76489Core
    {
        private static float[] volumeTable = new float[64]
        {
      0.25f,
      0.2442f,
      0.194f,
      0.1541f,
      0.1224f,
      0.0972f,
      0.0772f,
      0.0613f,
      0.0487f,
      0.0386f,
      0.0307f,
      0.0244f,
      0.0193f,
      0.0154f,
      0.0122f,
      0f,
      -0.25f,
      -0.2442f,
      -0.194f,
      -0.1541f,
      -0.1224f,
      -0.0972f,
      -0.0772f,
      -0.0613f,
      -0.0487f,
      -0.0386f,
      -0.0307f,
      -0.0244f,
      -0.0193f,
      -0.0154f,
      -0.0122f,
      0f,
      0.25f,
      0.2442f,
      0.194f,
      0.1541f,
      0.1224f,
      0.0972f,
      0.0772f,
      0.0613f,
      0.0487f,
      0.0386f,
      0.0307f,
      0.0244f,
      0.0193f,
      0.0154f,
      0.0122f,
      0f,
      0f,
      0f,
      0f,
      0f,
      0f,
      0f,
      0f,
      0f,
      0f,
      0f,
      0f,
      0f,
      0f,
      0f,
      0f,
      0f
        };
        private uint volA;
        private uint volB;
        private uint volC;
        private uint volD;
        private int divA;
        private int divB;
        private int divC;
        private int divD;
        private int cntA;
        private int cntB;
        private int cntC;
        private int cntD;
        private float outA;
        private float outB;
        private float outC;
        private float outD;
        private uint noiseLFSR;
        private uint noiseTap;
        private uint latchedChan;
        private bool latchedVolume;
        private float ticksPerSample;
        private float ticksCount;

        public SN76489Core()
        {
            clock(3500000f);
            reset();
        }

        public void clock(float f) => ticksPerSample = (float)(f / 16.0 / 44100.0);

        public void reset()
        {
            volA = 15U;
            volB = 15U;
            volC = 15U;
            volD = 15U;
            outA = 0f;
            outB = 0f;
            outC = 0f;
            outD = 0f;
            latchedChan = 0U;
            latchedVolume = false;
            noiseLFSR = 32768U;
            ticksCount = ticksPerSample;
        }

        public uint getDivByNumber(uint chan)
        {
            switch (chan)
            {
                case 0:
                    return (uint)divA;
                case 1:
                    return (uint)divB;
                case 2:
                    return (uint)divC;
                case 3:
                    return (uint)divD;
                default:
                    return 0;
            }
        }

        public void setDivByNumber(uint chan, uint div)
        {
            switch (chan)
            {
                case 0:
                    divA = (int)div;
                    break;
                case 1:
                    divB = (int)div;
                    break;
                case 2:
                    divC = (int)div;
                    break;
                case 3:
                    divD = (int)div;
                    break;
            }
        }

        public uint getVolByNumber(uint chan)
        {
            switch (chan)
            {
                case 0:
                    return volA;
                case 1:
                    return volB;
                case 2:
                    return volC;
                case 3:
                    return volD;
                default:
                    return 0;
            }
        }

        public void setVolByNumber(uint chan, uint vol)
        {
            switch (chan)
            {
                case 0:
                    volA = vol;
                    break;
                case 1:
                    volB = vol;
                    break;
                case 2:
                    volC = vol;
                    break;
                case 3:
                    volD = vol;
                    break;
            }
        }

        public void write(int val)
        {
            int chan;
            int div;
            if ((val & 128) != 0)
            {
                chan = val >> 5 & 3;
                div = (int)getDivByNumber((uint)chan) & 65520 | val & 15;
                latchedChan = (uint)chan;
                latchedVolume = (val & 16) != 0;
            }
            else
            {
                chan = (int)latchedChan;
                div = (int)getDivByNumber((uint)chan) & 15 | (val & 63) << 4;
            }
            if (latchedVolume)
            {
                setVolByNumber((uint)chan, (uint)((int)getVolByNumber((uint)chan) & 16 | val & 15));
            }
            else
            {
                setDivByNumber((uint)chan, (uint)div);
                if (chan != 3)
                    return;
                noiseTap = (div >> 2 & 1) != 0 ? 9U : 1U;
                noiseLFSR = 32768U;
            }
        }

        public float render()
        {
            if (0U >= 1U)
                return 0f;
            for (; ticksCount > 0.0; --ticksCount)
            {
                --cntA;
                if (cntA < 0)
                {
                    if (divA > 1)
                    {
                        volA ^= 16U;
                        outA = volumeTable[(int)volA];
                    }
                    cntA = divA;
                }
                --cntB;
                if (cntB < 0)
                {
                    if (divB > 1)
                    {
                        volB ^= 16U;
                        outB = volumeTable[(int)volB];
                    }
                    cntB = divB;
                }
                --cntC;
                if (cntC < 0)
                {
                    if (divC > 1)
                    {
                        volC ^= 16U;
                        outC = volumeTable[(int)volC];
                    }
                    cntC = divC;
                }
                --cntD;
                if (cntD < 0)
                {
                    uint num1 = (uint)(divD & 3);
                    cntD = num1 >= 3U ? divC << 1 : 16 << (int)num1;
                    uint num2;
                    if (noiseTap == 9U)
                    {
                        uint num3 = noiseLFSR & noiseTap;
                        uint num4 = num3 ^ num3 >> 8;
                        uint num5 = num4 ^ num4 >> 4;
                        uint num6 = num5 ^ num5 >> 2;
                        num2 = (num6 ^ num6 >> 1) & 1U;
                    }
                    else
                        num2 = noiseLFSR & 1U;
                    noiseLFSR = noiseLFSR >> 1 | num2 << 15;
                    volD = (uint)((int)volD & 15 | ((int)noiseLFSR & 1 ^ 1) << 4);
                    outD = volumeTable[(int)volD];
                }
            }
            ticksCount += ticksPerSample;
            return outA + outB + outC + outD;
        }
    }
}
