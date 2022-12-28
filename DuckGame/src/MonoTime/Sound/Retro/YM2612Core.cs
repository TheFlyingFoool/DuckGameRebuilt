// Decompiled with JetBrains decompiler
// Type: DuckGame.YM2612Core
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Linq;

namespace DuckGame
{
    public class YM2612Core
    {
        private const int FREQ_SH = 16;
        private const int EG_SH = 16;
        private const int LFO_SH = 24;
        private const int TIMER_SH = 16;
        private const int FREQ_MASK = 65535;
        private const int ENV_BITS = 10;
        private const int ENV_LEN = 1024;
        private const float ENV_STEP_GX = 0.125f;
        private const int MAX_ATT_INDEX = 1023;
        private const int MIN_ATT_INDEX = 0;
        private const int EG_ATT = 4;
        private const int EG_DEC = 3;
        private const int EG_SUS = 2;
        private const int EG_REL = 1;
        private const int EG_OFF = 0;
        private const int SIN_BITS = 10;
        private const int SIN_LEN = 1024;
        private const int SIN_MASK_GX = 1023;
        private const int TL_RES_LEN = 256;
        private static uint[] sl_table = new uint[16]
        {
      SC(0),
      SC(1),
      SC(2),
      SC(3),
      SC(4),
      SC(5),
      SC(6),
      SC(7),
      SC(8),
      SC(9),
      SC(10),
      SC(11),
      SC(12),
      SC(13),
      SC(14),
      SC(31)
        };
        private const int RATE_STEPS = 8;
        private static byte[] eg_inc = new byte[152]
        {
       0,
       1,
       0,
       1,
       0,
       1,
       0,
       1,
       0,
       1,
       0,
       1,
       1,
       1,
       0,
       1,
       0,
       1,
       1,
       1,
       0,
       1,
       1,
       1,
       0,
       1,
       1,
       1,
       1,
       1,
       1,
       1,
       1,
       1,
       1,
       1,
       1,
       1,
       1,
       1,
       1,
       1,
       1,
       2,
       1,
       1,
       1,
       2,
       1,
       2,
       1,
       2,
       1,
       2,
       1,
       2,
       1,
       2,
       2,
       2,
       1,
       2,
       2,
       2,
       2,
       2,
       2,
       2,
       2,
       2,
       2,
       2,
       2,
       2,
       2,
       4,
       2,
       2,
       2,
       4,
       2,
       4,
       2,
       4,
       2,
       4,
       2,
       4,
       2,
       4,
       4,
       4,
       2,
       4,
       4,
       4,
       4,
       4,
       4,
       4,
       4,
       4,
       4,
       4,
       4,
       4,
       4,
       8,
       4,
       4,
       4,
       8,
       4,
       8,
       4,
       8,
       4,
       8,
       4,
       8,
       4,
       8,
       8,
       8,
       4,
       8,
       8,
       8,
       8,
       8,
       8,
       8,
       8,
       8,
       8,
       8,
       16,
       16,
       16,
       16,
       16,
       16,
       16,
       16,
       0,
       0,
       0,
       0,
       0,
       0,
       0,
       0
        };
        private static byte[] eg_rate_select = new byte[128]
        {
      eg_rate_selectO( 18),
      eg_rate_selectO( 18),
      eg_rate_selectO( 18),
      eg_rate_selectO( 18),
      eg_rate_selectO( 18),
      eg_rate_selectO( 18),
      eg_rate_selectO( 18),
      eg_rate_selectO( 18),
      eg_rate_selectO( 18),
      eg_rate_selectO( 18),
      eg_rate_selectO( 18),
      eg_rate_selectO( 18),
      eg_rate_selectO( 18),
      eg_rate_selectO( 18),
      eg_rate_selectO( 18),
      eg_rate_selectO( 18),
      eg_rate_selectO( 18),
      eg_rate_selectO( 18),
      eg_rate_selectO( 18),
      eg_rate_selectO( 18),
      eg_rate_selectO( 18),
      eg_rate_selectO( 18),
      eg_rate_selectO( 18),
      eg_rate_selectO( 18),
      eg_rate_selectO( 18),
      eg_rate_selectO( 18),
      eg_rate_selectO( 18),
      eg_rate_selectO( 18),
      eg_rate_selectO( 18),
      eg_rate_selectO( 18),
      eg_rate_selectO( 18),
      eg_rate_selectO( 18),
      eg_rate_selectO( 18),
      eg_rate_selectO( 18),
      eg_rate_selectO( 0),
      eg_rate_selectO( 0),
      eg_rate_selectO( 0),
      eg_rate_selectO( 0),
      eg_rate_selectO( 2),
      eg_rate_selectO( 2),
      eg_rate_selectO( 0),
      eg_rate_selectO( 1),
      eg_rate_selectO( 2),
      eg_rate_selectO( 3),
      eg_rate_selectO( 0),
      eg_rate_selectO( 1),
      eg_rate_selectO( 2),
      eg_rate_selectO( 3),
      eg_rate_selectO( 0),
      eg_rate_selectO( 1),
      eg_rate_selectO( 2),
      eg_rate_selectO( 3),
      eg_rate_selectO( 0),
      eg_rate_selectO( 1),
      eg_rate_selectO( 2),
      eg_rate_selectO( 3),
      eg_rate_selectO( 0),
      eg_rate_selectO( 1),
      eg_rate_selectO( 2),
      eg_rate_selectO( 3),
      eg_rate_selectO( 0),
      eg_rate_selectO( 1),
      eg_rate_selectO( 2),
      eg_rate_selectO( 3),
      eg_rate_selectO( 0),
      eg_rate_selectO( 1),
      eg_rate_selectO( 2),
      eg_rate_selectO( 3),
      eg_rate_selectO( 0),
      eg_rate_selectO( 1),
      eg_rate_selectO( 2),
      eg_rate_selectO( 3),
      eg_rate_selectO( 0),
      eg_rate_selectO( 1),
      eg_rate_selectO( 2),
      eg_rate_selectO( 3),
      eg_rate_selectO( 0),
      eg_rate_selectO( 1),
      eg_rate_selectO( 2),
      eg_rate_selectO( 3),
      eg_rate_selectO( 4),
      eg_rate_selectO( 5),
      eg_rate_selectO( 6),
      eg_rate_selectO( 7),
      eg_rate_selectO( 8),
      eg_rate_selectO( 9),
      eg_rate_selectO( 10),
      eg_rate_selectO( 11),
      eg_rate_selectO( 12),
      eg_rate_selectO( 13),
      eg_rate_selectO( 14),
      eg_rate_selectO( 15),
      eg_rate_selectO( 16),
      eg_rate_selectO( 16),
      eg_rate_selectO( 16),
      eg_rate_selectO( 16),
      eg_rate_selectO( 16),
      eg_rate_selectO( 16),
      eg_rate_selectO( 16),
      eg_rate_selectO( 16),
      eg_rate_selectO( 16),
      eg_rate_selectO( 16),
      eg_rate_selectO( 16),
      eg_rate_selectO( 16),
      eg_rate_selectO( 16),
      eg_rate_selectO( 16),
      eg_rate_selectO( 16),
      eg_rate_selectO( 16),
      eg_rate_selectO( 16),
      eg_rate_selectO( 16),
      eg_rate_selectO( 16),
      eg_rate_selectO( 16),
      eg_rate_selectO( 16),
      eg_rate_selectO( 16),
      eg_rate_selectO( 16),
      eg_rate_selectO( 16),
      eg_rate_selectO( 16),
      eg_rate_selectO( 16),
      eg_rate_selectO( 16),
      eg_rate_selectO( 16),
      eg_rate_selectO( 16),
      eg_rate_selectO( 16),
      eg_rate_selectO( 16),
      eg_rate_selectO( 16),
      eg_rate_selectO( 16),
      eg_rate_selectO( 16),
      eg_rate_selectO( 16),
      eg_rate_selectO( 16)
        };
        private static byte[] eg_rate_shift = new byte[128]
        {
      eg_rate_shiftO( 11),
      eg_rate_shiftO( 11),
      eg_rate_shiftO( 11),
      eg_rate_shiftO( 11),
      eg_rate_shiftO( 11),
      eg_rate_shiftO( 11),
      eg_rate_shiftO( 11),
      eg_rate_shiftO( 11),
      eg_rate_shiftO( 11),
      eg_rate_shiftO( 11),
      eg_rate_shiftO( 11),
      eg_rate_shiftO( 11),
      eg_rate_shiftO( 11),
      eg_rate_shiftO( 11),
      eg_rate_shiftO( 11),
      eg_rate_shiftO( 11),
      eg_rate_shiftO( 11),
      eg_rate_shiftO( 11),
      eg_rate_shiftO( 11),
      eg_rate_shiftO( 11),
      eg_rate_shiftO( 11),
      eg_rate_shiftO( 11),
      eg_rate_shiftO( 11),
      eg_rate_shiftO( 11),
      eg_rate_shiftO( 11),
      eg_rate_shiftO( 11),
      eg_rate_shiftO( 11),
      eg_rate_shiftO( 11),
      eg_rate_shiftO( 11),
      eg_rate_shiftO( 11),
      eg_rate_shiftO( 11),
      eg_rate_shiftO( 11),
      eg_rate_shiftO( 11),
      eg_rate_shiftO( 11),
      eg_rate_shiftO( 11),
      eg_rate_shiftO( 11),
      eg_rate_shiftO( 10),
      eg_rate_shiftO( 10),
      eg_rate_shiftO( 10),
      eg_rate_shiftO( 10),
      eg_rate_shiftO( 9),
      eg_rate_shiftO( 9),
      eg_rate_shiftO( 9),
      eg_rate_shiftO( 9),
      eg_rate_shiftO( 8),
      eg_rate_shiftO( 8),
      eg_rate_shiftO( 8),
      eg_rate_shiftO( 8),
      eg_rate_shiftO( 7),
      eg_rate_shiftO( 7),
      eg_rate_shiftO( 7),
      eg_rate_shiftO( 7),
      eg_rate_shiftO( 6),
      eg_rate_shiftO( 6),
      eg_rate_shiftO( 6),
      eg_rate_shiftO( 6),
      eg_rate_shiftO( 5),
      eg_rate_shiftO( 5),
      eg_rate_shiftO( 5),
      eg_rate_shiftO( 5),
      eg_rate_shiftO( 4),
      eg_rate_shiftO( 4),
      eg_rate_shiftO( 4),
      eg_rate_shiftO( 4),
      eg_rate_shiftO( 3),
      eg_rate_shiftO( 3),
      eg_rate_shiftO( 3),
      eg_rate_shiftO( 3),
      eg_rate_shiftO( 2),
      eg_rate_shiftO( 2),
      eg_rate_shiftO( 2),
      eg_rate_shiftO( 2),
      eg_rate_shiftO( 1),
      eg_rate_shiftO( 1),
      eg_rate_shiftO( 1),
      eg_rate_shiftO( 1),
      eg_rate_shiftO( 0),
      eg_rate_shiftO( 0),
      eg_rate_shiftO( 0),
      eg_rate_shiftO( 0),
      eg_rate_shiftO( 0),
      eg_rate_shiftO( 0),
      eg_rate_shiftO( 0),
      eg_rate_shiftO( 0),
      eg_rate_shiftO( 0),
      eg_rate_shiftO( 0),
      eg_rate_shiftO( 0),
      eg_rate_shiftO( 0),
      eg_rate_shiftO( 0),
      eg_rate_shiftO( 0),
      eg_rate_shiftO( 0),
      eg_rate_shiftO( 0),
      eg_rate_shiftO( 0),
      eg_rate_shiftO( 0),
      eg_rate_shiftO( 0),
      eg_rate_shiftO( 0),
      eg_rate_shiftO( 0),
      eg_rate_shiftO( 0),
      eg_rate_shiftO( 0),
      eg_rate_shiftO( 0),
      eg_rate_shiftO( 0),
      eg_rate_shiftO( 0),
      eg_rate_shiftO( 0),
      eg_rate_shiftO( 0),
      eg_rate_shiftO( 0),
      eg_rate_shiftO( 0),
      eg_rate_shiftO( 0),
      eg_rate_shiftO( 0),
      eg_rate_shiftO( 0),
      eg_rate_shiftO( 0),
      eg_rate_shiftO( 0),
      eg_rate_shiftO( 0),
      eg_rate_shiftO( 0),
      eg_rate_shiftO( 0),
      eg_rate_shiftO( 0),
      eg_rate_shiftO( 0),
      eg_rate_shiftO( 0),
      eg_rate_shiftO( 0),
      eg_rate_shiftO( 0),
      eg_rate_shiftO( 0),
      eg_rate_shiftO( 0),
      eg_rate_shiftO( 0),
      eg_rate_shiftO( 0),
      eg_rate_shiftO( 0),
      eg_rate_shiftO( 0),
      eg_rate_shiftO( 0),
      eg_rate_shiftO( 0),
      eg_rate_shiftO( 0)
        };
        private static byte[] dt_tab = new byte[128]
        {
       0,
       0,
       0,
       0,
       0,
       0,
       0,
       0,
       0,
       0,
       0,
       0,
       0,
       0,
       0,
       0,
       0,
       0,
       0,
       0,
       0,
       0,
       0,
       0,
       0,
       0,
       0,
       0,
       0,
       0,
       0,
       0,
       0,
       0,
       0,
       0,
       1,
       1,
       1,
       1,
       1,
       1,
       1,
       1,
       2,
       2,
       2,
       2,
       2,
       3,
       3,
       3,
       4,
       4,
       4,
       5,
       5,
       6,
       6,
       7,
       8,
       8,
       8,
       8,
       1,
       1,
       1,
       1,
       2,
       2,
       2,
       2,
       2,
       3,
       3,
       3,
       4,
       4,
       4,
       5,
       5,
       6,
       6,
       7,
       8,
       8,
       9,
       10,
       11,
       12,
       13,
       14,
       16,
       16,
       16,
       16,
       2,
       2,
       2,
       2,
       2,
       3,
       3,
       3,
       4,
       4,
       4,
       5,
       5,
       6,
       6,
       7,
       8,
       8,
       9,
       10,
       11,
       12,
       13,
       14,
       16,
       17,
       19,
       20,
       22,
       22,
       22,
       22
        };
        private static byte[] opn_fktable = new byte[16]
        {
       0,
       0,
       0,
       0,
       0,
       0,
       0,
       1,
       2,
       3,
       3,
       3,
       3,
       3,
       3,
       3
        };
        private static uint[] lfo_samples_per_step = new uint[8]
        {
      108U,
      77U,
      71U,
      67U,
      62U,
      44U,
      8U,
      5U
        };
        private static byte[] lfo_ams_depth_shift = new byte[4]
        {
       8,
       3,
       1,
       0
        };
        private static byte[,] lfo_pm_output = new byte[56, 8]
        {
      {
         0,
         0,
         0,
         0,
         0,
         0,
         0,
         0
      },
      {
         0,
         0,
         0,
         0,
         0,
         0,
         0,
         0
      },
      {
         0,
         0,
         0,
         0,
         0,
         0,
         0,
         0
      },
      {
         0,
         0,
         0,
         0,
         0,
         0,
         0,
         0
      },
      {
         0,
         0,
         0,
         0,
         0,
         0,
         0,
         0
      },
      {
         0,
         0,
         0,
         0,
         0,
         0,
         0,
         0
      },
      {
         0,
         0,
         0,
         0,
         0,
         0,
         0,
         0
      },
      {
         0,
         0,
         0,
         0,
         1,
         1,
         1,
         1
      },
      {
         0,
         0,
         0,
         0,
         0,
         0,
         0,
         0
      },
      {
         0,
         0,
         0,
         0,
         0,
         0,
         0,
         0
      },
      {
         0,
         0,
         0,
         0,
         0,
         0,
         0,
         0
      },
      {
         0,
         0,
         0,
         0,
         0,
         0,
         0,
         0
      },
      {
         0,
         0,
         0,
         0,
         0,
         0,
         0,
         0
      },
      {
         0,
         0,
         0,
         0,
         0,
         0,
         0,
         0
      },
      {
         0,
         0,
         0,
         0,
         1,
         1,
         1,
         1
      },
      {
         0,
         0,
         1,
         1,
         2,
         2,
         2,
         3
      },
      {
         0,
         0,
         0,
         0,
         0,
         0,
         0,
         0
      },
      {
         0,
         0,
         0,
         0,
         0,
         0,
         0,
         0
      },
      {
         0,
         0,
         0,
         0,
         0,
         0,
         0,
         0
      },
      {
         0,
         0,
         0,
         0,
         0,
         0,
         0,
         0
      },
      {
         0,
         0,
         0,
         0,
         0,
         0,
         0,
         1
      },
      {
         0,
         0,
         0,
         0,
         1,
         1,
         1,
         1
      },
      {
         0,
         0,
         1,
         1,
         2,
         2,
         2,
         3
      },
      {
         0,
         0,
         2,
         3,
         4,
         4,
         5,
         6
      },
      {
         0,
         0,
         0,
         0,
         0,
         0,
         0,
         0
      },
      {
         0,
         0,
         0,
         0,
         0,
         0,
         0,
         0
      },
      {
         0,
         0,
         0,
         0,
         0,
         0,
         1,
         1
      },
      {
         0,
         0,
         0,
         0,
         1,
         1,
         1,
         1
      },
      {
         0,
         0,
         0,
         1,
         1,
         1,
         1,
         2
      },
      {
         0,
         0,
         1,
         1,
         2,
         2,
         2,
         3
      },
      {
         0,
         0,
         2,
         3,
         4,
         4,
         5,
         6
      },
      {
         0,
         0,
         4,
         6,
         8,
         8,
         10,
         12
      },
      {
         0,
         0,
         0,
         0,
         0,
         0,
         0,
         0
      },
      {
         0,
         0,
         0,
         0,
         1,
         1,
         1,
         1
      },
      {
         0,
         0,
         0,
         1,
         1,
         1,
         2,
         2
      },
      {
         0,
         0,
         1,
         1,
         2,
         2,
         3,
         3
      },
      {
         0,
         0,
         1,
         2,
         2,
         2,
         3,
         4
      },
      {
         0,
         0,
         2,
         3,
         4,
         4,
         5,
         6
      },
      {
         0,
         0,
         4,
         6,
         8,
         8,
         10,
         12
      },
      {
         0,
         0,
         8,
         12,
         16,
         16,
         20,
         24
      },
      {
         0,
         0,
         0,
         0,
         0,
         0,
         0,
         0
      },
      {
         0,
         0,
         0,
         0,
         2,
         2,
         2,
         2
      },
      {
         0,
         0,
         0,
         2,
         2,
         2,
         4,
         4
      },
      {
         0,
         0,
         2,
         2,
         4,
         4,
         6,
         6
      },
      {
         0,
         0,
         2,
         4,
         4,
         4,
         6,
         8
      },
      {
         0,
         0,
         4,
         6,
         8,
         8,
         10,
         12
      },
      {
         0,
         0,
         8,
         12,
         16,
         16,
         20,
         24
      },
      {
         0,
         0,
         16,
         24,
         32,
         32,
         40,
         48
      },
      {
         0,
         0,
         0,
         0,
         0,
         0,
         0,
         0
      },
      {
         0,
         0,
         0,
         0,
         4,
         4,
         4,
         4
      },
      {
         0,
         0,
         0,
         4,
         4,
         4,
         8,
         8
      },
      {
         0,
         0,
         4,
         4,
         8,
         8,
         12,
         12
      },
      {
         0,
         0,
         4,
         8,
         8,
         8,
         12,
         16
      },
      {
         0,
         0,
         8,
         12,
         16,
         16,
         20,
         24
      },
      {
         0,
         0,
         16,
         24,
         32,
         32,
         40,
         48
      },
      {
         0,
         0,
         32,
         48,
         64,
         64,
         80,
         96
      }
        };
        private const int STATE_SIZE = 295168;
        private const string STATE_VERSION = "GENPLUS-GX 1.6.1";
        private const int ENV_QUIET = 832;
        private const int SLOT1 = 0;
        private const int SLOT2 = 2;
        private const int SLOT3 = 1;
        private const int SLOT4 = 3;
        private const int TL_TAB_LEN = 6656;
        private int[] tl_tab = new int[6656];
        private uint[] sin_tab = new uint[1024];
        private long[] lfo_pm_table = new long[32768];
        private t_config config = new t_config();
        private _YM2612_data ym2612 = new _YM2612_data();
        private LongPointer m2 = new LongPointer();
        private LongPointer c1 = new LongPointer();
        private LongPointer c2 = new LongPointer();
        private LongPointer mem = new LongPointer();
        private LongPointer[] out_fm = new LongPointer[8];

        private static uint SC(int db) => (uint)(db * 32.0);

        private static byte eg_rate_selectO(byte a) => (byte)(a * 8U);

        private static byte eg_rate_shiftO(byte a) => a;

        public YM2612Core()
        {
            for (int index = 0; index < 8; ++index)
                out_fm[index] = new LongPointer();
        }

        private void FM_KEYON(FM_CH CH, int s)
        {
            FM_SLOT fmSlot = CH.SLOT[s];
            if (fmSlot.key == 0 && ym2612.OPN.SL3.key_csm == 0)
            {
                fmSlot.phase = 0U;
                fmSlot.ssgn = 0;
                if (fmSlot.ar + fmSlot.ksr < 94U)
                {
                    fmSlot.state = fmSlot.volume <= 0L ? (fmSlot.sl == 0U ? (byte)2 : (byte)3) : (byte)4;
                }
                else
                {
                    fmSlot.volume = 0L;
                    fmSlot.state = fmSlot.sl == 0U ? (byte)2 : (byte)3;
                }
                fmSlot.vol_out = (fmSlot.ssg & 8) == 0 || (fmSlot.ssgn ^ fmSlot.ssg & 4) == 0 ? (uint)fmSlot.volume + fmSlot.tl : ((uint)(512UL - (ulong)fmSlot.volume) & 1023U) + fmSlot.tl;
            }
            fmSlot.key = 1;
        }

        private void FM_KEYOFF(FM_CH CH, int s)
        {
            FM_SLOT fmSlot = CH.SLOT[s];
            if (fmSlot.key > 0 && ym2612.OPN.SL3.key_csm == 0 && fmSlot.state > 1)
            {
                fmSlot.state = 1;
                if ((fmSlot.ssg & 8) > 0)
                {
                    if ((fmSlot.ssgn ^ fmSlot.ssg & 4) != 0)
                        fmSlot.volume = 512L - fmSlot.volume;
                    if (fmSlot.volume >= 512L)
                    {
                        fmSlot.volume = 1023L;
                        fmSlot.state = 0;
                    }
                    fmSlot.vol_out = (uint)fmSlot.volume + fmSlot.tl;
                }
            }
            fmSlot.key = 0;
        }

        private void FM_KEYON_CSM(FM_CH CH, int s)
        {
            FM_SLOT fmSlot = CH.SLOT[s];
            if (fmSlot.key != 0 || ym2612.OPN.SL3.key_csm != 0)
                return;
            fmSlot.phase = 0U;
            fmSlot.ssgn = 0;
            if (fmSlot.ar + fmSlot.ksr < 94U)
            {
                fmSlot.state = fmSlot.volume <= 0L ? (fmSlot.sl == 0U ? (byte)2 : (byte)3) : (byte)4;
            }
            else
            {
                fmSlot.volume = 0L;
                fmSlot.state = fmSlot.sl == 0U ? (byte)2 : (byte)3;
            }
            if ((fmSlot.ssg & 8) != 0 && (fmSlot.ssgn ^ fmSlot.ssg & 4) != 0)
                fmSlot.vol_out = ((uint)(512UL - (ulong)fmSlot.volume) & 1023U) + fmSlot.tl;
            else
                fmSlot.vol_out = (uint)fmSlot.volume + fmSlot.tl;
        }

        private void FM_KEYOFF_CSM(FM_CH CH, int s)
        {
            FM_SLOT fmSlot = CH.SLOT[s];
            if (fmSlot.key != 0 || fmSlot.state <= 1)
                return;
            fmSlot.state = 1;
            if ((fmSlot.ssg & 8) <= 0)
                return;
            if ((fmSlot.ssgn ^ fmSlot.ssg & 4) > 0)
                fmSlot.volume = 512L - fmSlot.volume;
            if (fmSlot.volume >= 512L)
            {
                fmSlot.volume = 1023L;
                fmSlot.state = 0;
            }
            fmSlot.vol_out = (uint)fmSlot.volume + fmSlot.tl;
        }

        private void CSMKeyControll(FM_CH CH)
        {
            FM_KEYON_CSM(CH, 0);
            FM_KEYON_CSM(CH, 2);
            FM_KEYON_CSM(CH, 1);
            FM_KEYON_CSM(CH, 3);
            ym2612.OPN.SL3.key_csm = 1;
        }

        private void INTERNAL_TIMER_A()
        {
            if (((int)ym2612.OPN.ST.mode & 1) == 0 || (ym2612.OPN.ST.TAC -= ym2612.OPN.ST.TimerBase) > 0L)
                return;
            if (((int)ym2612.OPN.ST.mode & 4) != 0)
                ym2612.OPN.ST.status |= 1;
            if (ym2612.OPN.ST.TAL != 0L)
                ym2612.OPN.ST.TAC += ym2612.OPN.ST.TAL;
            else
                ym2612.OPN.ST.TAC = ym2612.OPN.ST.TAL;
            if (((int)ym2612.OPN.ST.mode & 192) != 128)
                return;
            CSMKeyControll(ym2612.CH[2]);
        }

        private void INTERNAL_TIMER_B(int step)
        {
            if (((int)ym2612.OPN.ST.mode & 2) == 0 || (ym2612.OPN.ST.TBC -= ym2612.OPN.ST.TimerBase * step) > 0L)
                return;
            if (((int)ym2612.OPN.ST.mode & 8) != 0)
                ym2612.OPN.ST.status |= 2;
            if (ym2612.OPN.ST.TBL != 0L)
                ym2612.OPN.ST.TBC += ym2612.OPN.ST.TBL;
            else
                ym2612.OPN.ST.TBC = ym2612.OPN.ST.TBL;
        }

        private void set_timers(int v)
        {
            if (((ym2612.OPN.ST.mode ^ v) & 192L) != 0L)
            {
                ym2612.CH[2].SLOT[0].Incr = -1L;
                if ((v & 192) != 128 && ym2612.OPN.SL3.key_csm != 0)
                {
                    FM_KEYOFF_CSM(ym2612.CH[2], 0);
                    FM_KEYOFF_CSM(ym2612.CH[2], 2);
                    FM_KEYOFF_CSM(ym2612.CH[2], 1);
                    FM_KEYOFF_CSM(ym2612.CH[2], 3);
                    ym2612.OPN.SL3.key_csm = 0;
                }
            }
            if ((v & 1) != 0 && ((int)ym2612.OPN.ST.mode & 1) == 0)
                ym2612.OPN.ST.TAC = ym2612.OPN.ST.TAL;
            if ((v & 2) != 0 && ((int)ym2612.OPN.ST.mode & 2) == 0)
                ym2612.OPN.ST.TBC = ym2612.OPN.ST.TBL;
            ym2612.OPN.ST.status &= (byte)(~v >> 4);
            ym2612.OPN.ST.mode = (uint)v;
        }

        private void setup_connection(FM_CH CH, int ch)
        {
            LongPointer longPointer = out_fm[ch];
            switch (CH.ALGO)
            {
                case 0:
                    CH.connect1 = c1;
                    CH.connect2 = mem;
                    CH.connect3 = c2;
                    CH.mem_connect = m2;
                    break;
                case 1:
                    CH.connect1 = mem;
                    CH.connect2 = mem;
                    CH.connect3 = c2;
                    CH.mem_connect = m2;
                    break;
                case 2:
                    CH.connect1 = c2;
                    CH.connect2 = mem;
                    CH.connect3 = c2;
                    CH.mem_connect = m2;
                    break;
                case 3:
                    CH.connect1 = c1;
                    CH.connect2 = mem;
                    CH.connect3 = c2;
                    CH.mem_connect = c2;
                    break;
                case 4:
                    CH.connect1 = c1;
                    CH.connect2 = longPointer;
                    CH.connect3 = c2;
                    CH.mem_connect = mem;
                    break;
                case 5:
                    CH.connect1 = null;
                    CH.connect2 = longPointer;
                    CH.connect3 = longPointer;
                    CH.mem_connect = m2;
                    break;
                case 6:
                    CH.connect1 = c1;
                    CH.connect2 = longPointer;
                    CH.connect3 = longPointer;
                    CH.mem_connect = mem;
                    break;
                case 7:
                    CH.connect1 = longPointer;
                    CH.connect2 = longPointer;
                    CH.connect3 = longPointer;
                    CH.mem_connect = mem;
                    break;
            }
            CH.connect4 = longPointer;
        }

        private void set_det_mul(FM_CH CH, FM_SLOT SLOT, int v)
        {
            SLOT.mul = (v & 15) != 0 ? (uint)((v & 15) * 2) : 1U;
            SLOT.DT = ym2612.OPN.ST.dt_tab[v >> 4 & 7];
            CH.SLOT[0].Incr = -1L;
        }

        private void set_tl(FM_SLOT SLOT, int v)
        {
            SLOT.tl = (uint)((v & sbyte.MaxValue) << 3);
            if ((SLOT.ssg & 8) != 0 && (SLOT.ssgn ^ SLOT.ssg & 4) != 0 && SLOT.state > 1)
                SLOT.vol_out = ((uint)(512UL - (ulong)SLOT.volume) & 1023U) + SLOT.tl;
            else
                SLOT.vol_out = (uint)SLOT.volume + SLOT.tl;
        }

        private void set_ar_ksr(FM_CH CH, FM_SLOT SLOT, int v)
        {
            byte ksr = SLOT.KSR;
            SLOT.ar = (v & 31) != 0 ? (uint)(32 + ((v & 31) << 1)) : 0U;
            SLOT.KSR = (byte)(3 - (v >> 6));
            if (SLOT.KSR != ksr)
                CH.SLOT[0].Incr = -1L;
            if (SLOT.ar + SLOT.ksr < 94U)
            {
                SLOT.eg_sh_ar = eg_rate_shift[(int)SLOT.ar + SLOT.ksr];
                SLOT.eg_sel_ar = eg_rate_select[(int)SLOT.ar + SLOT.ksr];
            }
            else
            {
                SLOT.eg_sh_ar = 0;
                SLOT.eg_sel_ar = 144;
            }
        }

        private void set_dr(FM_SLOT SLOT, int v)
        {
            SLOT.d1r = (v & 31) != 0 ? (uint)(32 + ((v & 31) << 1)) : 0U;
            SLOT.eg_sh_d1r = eg_rate_shift[(int)SLOT.d1r + SLOT.ksr];
            SLOT.eg_sel_d1r = eg_rate_select[(int)SLOT.d1r + SLOT.ksr];
        }

        private void set_sr(FM_SLOT SLOT, int v)
        {
            SLOT.d2r = (v & 31) != 0 ? (uint)(32 + ((v & 31) << 1)) : 0U;
            SLOT.eg_sh_d2r = eg_rate_shift[(int)SLOT.d2r + SLOT.ksr];
            SLOT.eg_sel_d2r = eg_rate_select[(int)SLOT.d2r + SLOT.ksr];
        }

        private void set_sl_rr(FM_SLOT SLOT, int v)
        {
            SLOT.sl = sl_table[v >> 4];
            if (SLOT.state == 3 && SLOT.volume >= (int)SLOT.sl)
                SLOT.state = 2;
            SLOT.rr = (uint)(34 + ((v & 15) << 2));
            SLOT.eg_sh_rr = eg_rate_shift[(int)SLOT.rr + SLOT.ksr];
            SLOT.eg_sel_rr = eg_rate_select[(int)SLOT.rr + SLOT.ksr];
        }

        private void advance_lfo()
        {
            if (ym2612.OPN.lfo_timer_overflow == 0U)
                return;
            ym2612.OPN.lfo_timer += ym2612.OPN.lfo_timer_add;
            while (ym2612.OPN.lfo_timer >= ym2612.OPN.lfo_timer_overflow)
            {
                ym2612.OPN.lfo_timer -= ym2612.OPN.lfo_timer_overflow;
                ym2612.OPN.lfo_cnt = (byte)(ym2612.OPN.lfo_cnt + 1 & sbyte.MaxValue);
                ym2612.OPN.LFO_AM = ym2612.OPN.lfo_cnt >= 64 ? (uint)(126 - (ym2612.OPN.lfo_cnt & 63) * 2) : ym2612.OPN.lfo_cnt * 2U;
                ym2612.OPN.LFO_PM = (uint)ym2612.OPN.lfo_cnt >> 2;
            }
        }

        private void advance_eg_channels()
        {
            uint egCnt = ym2612.OPN.eg_cnt;
            uint index1 = 0;
            do
            {
                int index2 = 0;
                FM_SLOT fmSlot = ym2612.CH[(int)index1].SLOT[index2];
                uint num = 4;
                do
                {
                    switch (fmSlot.state)
                    {
                        case 1:
                            if ((egCnt & (1 << fmSlot.eg_sh_rr) - 1) == 0L)
                            {
                                if ((fmSlot.ssg & 8) != 0)
                                {
                                    if (fmSlot.volume < 512L)
                                        fmSlot.volume += 4 * eg_inc[fmSlot.eg_sel_rr + ((int)(egCnt >> fmSlot.eg_sh_rr) & 7)];
                                    if (fmSlot.volume >= 512L)
                                    {
                                        fmSlot.volume = 1023L;
                                        fmSlot.state = 0;
                                    }
                                }
                                else
                                {
                                    fmSlot.volume += eg_inc[fmSlot.eg_sel_rr + ((int)(egCnt >> fmSlot.eg_sh_rr) & 7)];
                                    if (fmSlot.volume >= 1023L)
                                    {
                                        fmSlot.volume = 1023L;
                                        fmSlot.state = 0;
                                    }
                                }
                                fmSlot.vol_out = (uint)fmSlot.volume + fmSlot.tl;
                                break;
                            }
                            break;
                        case 2:
                            if ((egCnt & (1 << fmSlot.eg_sh_d2r) - 1) == 0L)
                            {
                                if ((fmSlot.ssg & 8) != 0)
                                {
                                    if (fmSlot.volume < 512L)
                                    {
                                        fmSlot.volume += 4 * eg_inc[fmSlot.eg_sel_d2r + ((int)(egCnt >> fmSlot.eg_sh_d2r) & 7)];
                                        fmSlot.vol_out = (fmSlot.ssgn ^ fmSlot.ssg & 4) == 0 ? (uint)fmSlot.volume + fmSlot.tl : ((uint)(512UL - (ulong)fmSlot.volume) & 1023U) + fmSlot.tl;
                                        break;
                                    }
                                    break;
                                }
                                fmSlot.volume += eg_inc[fmSlot.eg_sel_d2r + ((int)(egCnt >> fmSlot.eg_sh_d2r) & 7)];
                                if (fmSlot.volume >= 1023L)
                                    fmSlot.volume = 1023L;
                                fmSlot.vol_out = (uint)fmSlot.volume + fmSlot.tl;
                                break;
                            }
                            break;
                        case 3:
                            if ((egCnt & (1 << fmSlot.eg_sh_d1r) - 1) == 0L)
                            {
                                if ((fmSlot.ssg & 8) != 0)
                                {
                                    if (fmSlot.volume < 512L)
                                    {
                                        fmSlot.volume += 4 * eg_inc[fmSlot.eg_sel_d1r + ((int)(egCnt >> fmSlot.eg_sh_d1r) & 7)];
                                        fmSlot.vol_out = (fmSlot.ssgn ^ fmSlot.ssg & 4) == 0 ? (uint)fmSlot.volume + fmSlot.tl : ((uint)(512UL - (ulong)fmSlot.volume) & 1023U) + fmSlot.tl;
                                    }
                                }
                                else
                                {
                                    fmSlot.volume += eg_inc[fmSlot.eg_sel_d1r + ((int)(egCnt >> fmSlot.eg_sh_d1r) & 7)];
                                    fmSlot.vol_out = (uint)fmSlot.volume + fmSlot.tl;
                                }
                                if (fmSlot.volume >= (int)fmSlot.sl)
                                {
                                    fmSlot.state = 2;
                                    break;
                                }
                                break;
                            }
                            break;
                        case 4:
                            if ((egCnt & (1 << fmSlot.eg_sh_ar) - 1) == 0L)
                            {
                                fmSlot.volume += ~fmSlot.volume * eg_inc[fmSlot.eg_sel_ar + ((int)(egCnt >> fmSlot.eg_sh_ar) & 7)] >> 4;
                                if (fmSlot.volume <= 0L)
                                {
                                    fmSlot.volume = 0L;
                                    fmSlot.state = fmSlot.sl == 0U ? (byte)2 : (byte)3;
                                }
                                fmSlot.vol_out = (fmSlot.ssg & 8) == 0 || (fmSlot.ssgn ^ fmSlot.ssg & 4) == 0 ? (uint)fmSlot.volume + fmSlot.tl : ((uint)(512UL - (ulong)fmSlot.volume) & 1023U) + fmSlot.tl;
                                break;
                            }
                            break;
                    }
                    ++index2;
                    if (index2 < ym2612.CH[(int)index1].SLOT.Count())
                        fmSlot = ym2612.CH[(int)index1].SLOT[index2];
                    --num;
                }
                while (num != 0U);
                ++index1;
            }
            while (index1 < 6U);
        }

        private void update_ssg_eg_channel(FM_SLOT[] SLOTS)
        {
            uint num = 4;
            int index = 0;
            FM_SLOT fmSlot = SLOTS[index];
            do
            {
                if ((fmSlot.ssg & 8) != 0 && fmSlot.volume >= 512L && fmSlot.state > 1)
                {
                    if ((fmSlot.ssg & 1) != 0)
                    {
                        if ((fmSlot.ssg & 2) != 0)
                            fmSlot.ssgn = 4;
                        if (fmSlot.state != 4 && (fmSlot.ssgn ^ fmSlot.ssg & 4) == 0)
                            fmSlot.volume = 1023L;
                    }
                    else
                    {
                        if ((fmSlot.ssg & 2) != 0)
                            fmSlot.ssgn ^= 4;
                        else
                            fmSlot.phase = 0U;
                        if (fmSlot.state != 4)
                        {
                            if (fmSlot.ar + fmSlot.ksr < 94U)
                            {
                                fmSlot.state = fmSlot.volume <= 0L ? (fmSlot.sl == 0U ? (byte)2 : (byte)3) : (byte)4;
                            }
                            else
                            {
                                fmSlot.volume = 0L;
                                fmSlot.state = fmSlot.sl == 0U ? (byte)2 : (byte)3;
                            }
                        }
                    }
                    fmSlot.vol_out = (fmSlot.ssgn ^ fmSlot.ssg & 4) == 0 ? (uint)fmSlot.volume + fmSlot.tl : ((uint)(512UL - (ulong)fmSlot.volume) & 1023U) + fmSlot.tl;
                }
                ++index;
                if (index < SLOTS.Count())
                    fmSlot = SLOTS[index];
                --num;
            }
            while (num != 0U);
        }

        private void update_phase_lfo_slot(FM_SLOT SLOT, long pms, uint block_fnum)
        {
            long num1 = lfo_pm_table[((block_fnum & 2032U) >> 4 << 8) + pms + ym2612.OPN.LFO_PM];
            if (num1 != 0L)
            {
                block_fnum = (uint)(block_fnum * 2U + (ulong)num1);
                byte num2 = (byte)((block_fnum & 28672U) >> 12);
                block_fnum &= 4095U;
                int index = num2 << 2 | opn_fktable[(int)(block_fnum >> 8)];
                int num3 = (int)((ym2612.OPN.fn_table[(int)block_fnum] >> 7 - num2) + SLOT.DT.value[index]);
                if (num3 < 0)
                    num3 += (int)ym2612.OPN.fn_max;
                SLOT.phase += (uint)(num3 * SLOT.mul >> 1);
            }
            else
                SLOT.phase += (uint)SLOT.Incr;
        }

        private void update_phase_lfo_channel(FM_CH CH)
        {
            uint blockFnum = CH.block_fnum;
            long num1 = lfo_pm_table[((blockFnum & 2032U) >> 4 << 8) + CH.pms + ym2612.OPN.LFO_PM];
            if (num1 != 0L)
            {
                uint num2 = (uint)(blockFnum * 2U + (ulong)num1);
                byte num3 = (byte)((num2 & 28672U) >> 12);
                uint index1 = num2 & 4095U;
                int index2 = num3 << 2 | opn_fktable[(int)(index1 >> 8)];
                int num4;
                int num5 = (int)((num4 = (int)(ym2612.OPN.fn_table[(int)index1] >> 7 - num3)) + CH.SLOT[0].DT.value[index2]);
                if (num5 < 0)
                    num5 += (int)ym2612.OPN.fn_max;
                CH.SLOT[0].phase += (uint)(num5 * CH.SLOT[0].mul >> 1);
                int num6 = (int)(num4 + CH.SLOT[2].DT.value[index2]);
                if (num6 < 0)
                    num6 += (int)ym2612.OPN.fn_max;
                CH.SLOT[2].phase += (uint)(num6 * CH.SLOT[2].mul >> 1);
                int num7 = (int)(num4 + CH.SLOT[1].DT.value[index2]);
                if (num7 < 0)
                    num7 += (int)ym2612.OPN.fn_max;
                CH.SLOT[1].phase += (uint)(num7 * CH.SLOT[1].mul >> 1);
                int num8 = (int)(num4 + CH.SLOT[3].DT.value[index2]);
                if (num8 < 0)
                    num8 += (int)ym2612.OPN.fn_max;
                CH.SLOT[3].phase += (uint)(num8 * CH.SLOT[3].mul >> 1);
            }
            else
            {
                CH.SLOT[0].phase += (uint)CH.SLOT[0].Incr;
                CH.SLOT[2].phase += (uint)CH.SLOT[2].Incr;
                CH.SLOT[1].phase += (uint)CH.SLOT[1].Incr;
                CH.SLOT[3].phase += (uint)CH.SLOT[3].Incr;
            }
        }

        private void refresh_fc_eg_slot(FM_SLOT SLOT, int fc, int kc)
        {
            fc += (int)SLOT.DT.value[kc];
            if (fc < 0)
                fc += (int)ym2612.OPN.fn_max;
            SLOT.Incr = fc * SLOT.mul >> 1;
            kc >>= SLOT.KSR;
            if (SLOT.ksr == kc)
                return;
            SLOT.ksr = (byte)kc;
            if (SLOT.ar + kc < 94L)
            {
                SLOT.eg_sh_ar = eg_rate_shift[SLOT.ar + kc];
                SLOT.eg_sel_ar = eg_rate_select[SLOT.ar + kc];
            }
            else
            {
                SLOT.eg_sh_ar = 0;
                SLOT.eg_sel_ar = 144;
            }
            SLOT.eg_sh_d1r = eg_rate_shift[SLOT.d1r + kc];
            SLOT.eg_sel_d1r = eg_rate_select[SLOT.d1r + kc];
            SLOT.eg_sh_d2r = eg_rate_shift[SLOT.d2r + kc];
            SLOT.eg_sel_d2r = eg_rate_select[SLOT.d2r + kc];
            SLOT.eg_sh_rr = eg_rate_shift[SLOT.rr + kc];
            SLOT.eg_sel_rr = eg_rate_select[SLOT.rr + kc];
        }

        private void refresh_fc_eg_chan(FM_CH CH)
        {
            if (CH.SLOT[0].Incr != -1L)
                return;
            int fc = (int)CH.fc;
            int kcode = CH.kcode;
            refresh_fc_eg_slot(CH.SLOT[0], fc, kcode);
            refresh_fc_eg_slot(CH.SLOT[2], fc, kcode);
            refresh_fc_eg_slot(CH.SLOT[1], fc, kcode);
            refresh_fc_eg_slot(CH.SLOT[3], fc, kcode);
        }

        private uint volume_calc(FM_SLOT slot, uint AM) => slot.vol_out + (AM & slot.AMmask);

        private int op_calc(uint phase, uint env, int pm)
        {
            uint index = (env << 3) + sin_tab[(int)((phase & -65536L) + (pm << 15)) >> 16 & 1023];
            return index >= 6656U ? 0 : tl_tab[(int)index];
        }

        private int op_calc1(uint phase, uint env, int pm)
        {
            uint index = (env << 3) + sin_tab[(int)((phase & -65536L) + pm) >> 16 & 1023];
            return index >= 6656U ? 0 : tl_tab[(int)index];
        }

        private void chan_calc(FM_CH CH)
        {
            uint AM = ym2612.OPN.LFO_AM >> CH.ams;
            uint env1 = volume_calc(CH.SLOT[0], AM);
            m2.value = c1.value = c2.value = mem.value = 0L;
            CH.mem_connect.value = CH.mem_value;
            long num = CH.op1_out[0] + CH.op1_out[1];
            CH.op1_out[0] = CH.op1_out[1];
            if (CH.connect1 == null)
                mem.value = c1.value = c2.value = CH.op1_out[0];
            else
                CH.connect1.value += CH.op1_out[0];
            CH.op1_out[1] = 0L;
            if (env1 < 832U)
            {
                if (CH.FB == 0)
                    num = 0L;
                CH.op1_out[1] = op_calc1(CH.SLOT[0].phase, env1, (int)(num << CH.FB));
            }
            uint env2 = volume_calc(CH.SLOT[1], AM);
            if (env2 < 832U)
                CH.connect3.value += op_calc(CH.SLOT[1].phase, env2, (int)m2.value);
            uint env3 = volume_calc(CH.SLOT[2], AM);
            if (env3 < 832U)
                CH.connect2.value += op_calc(CH.SLOT[2].phase, env3, (int)c1.value);
            uint env4 = volume_calc(CH.SLOT[3], AM);
            if (env4 < 832U)
                CH.connect4.value += op_calc(CH.SLOT[3].phase, env4, (int)c2.value);
            CH.mem_value = mem.value;
            if (CH.pms != 0L)
            {
                if ((ym2612.OPN.ST.mode & 192U) > 0U && CH == ym2612.CH[2])
                {
                    update_phase_lfo_slot(CH.SLOT[0], CH.pms, ym2612.OPN.SL3.block_fnum[1]);
                    update_phase_lfo_slot(CH.SLOT[2], CH.pms, ym2612.OPN.SL3.block_fnum[2]);
                    update_phase_lfo_slot(CH.SLOT[1], CH.pms, ym2612.OPN.SL3.block_fnum[0]);
                    update_phase_lfo_slot(CH.SLOT[3], CH.pms, CH.block_fnum);
                }
                else
                    update_phase_lfo_channel(CH);
            }
            else
            {
                CH.SLOT[0].phase += (uint)CH.SLOT[0].Incr;
                CH.SLOT[2].phase += (uint)CH.SLOT[2].Incr;
                CH.SLOT[1].phase += (uint)CH.SLOT[1].Incr;
                CH.SLOT[3].phase += (uint)CH.SLOT[3].Incr;
            }
        }

        private void OPNWriteMode(int r, int v)
        {
            switch (r)
            {
                case 34:
                    if ((v & 8) != 0)
                    {
                        if (ym2612.OPN.lfo_timer_overflow == 0U)
                        {
                            ym2612.OPN.lfo_cnt = 0;
                            ym2612.OPN.lfo_timer = 0U;
                            ym2612.OPN.LFO_AM = 0U;
                            ym2612.OPN.LFO_PM = 0U;
                        }
                        ym2612.OPN.lfo_timer_overflow = lfo_samples_per_step[v & 7] << 24;
                        break;
                    }
                    ym2612.OPN.lfo_timer_overflow = 0U;
                    break;
                case 36:
                    ym2612.OPN.ST.TA = ym2612.OPN.ST.TA & 3L | (long)v << 2;
                    ym2612.OPN.ST.TAL = 1024L - ym2612.OPN.ST.TA << 16;
                    break;
                case 37:
                    ym2612.OPN.ST.TA = ym2612.OPN.ST.TA & 1020L | v & 3L;
                    ym2612.OPN.ST.TAL = 1024L - ym2612.OPN.ST.TA << 16;
                    break;
                case 38:
                    ym2612.OPN.ST.TB = v;
                    ym2612.OPN.ST.TBL = 256L - ym2612.OPN.ST.TB << 20;
                    break;
                case 39:
                    set_timers(v);
                    break;
                case 40:
                    byte index = (byte)(v & 3);
                    if (index == 3)
                        break;
                    if ((v & 4) != 0)
                        index += 3;
                    FM_CH CH = ym2612.CH[index];
                    if ((v & 16) != 0)
                        FM_KEYON(CH, 0);
                    else
                        FM_KEYOFF(CH, 0);
                    if ((v & 32) != 0)
                        FM_KEYON(CH, 2);
                    else
                        FM_KEYOFF(CH, 2);
                    if ((v & 64) != 0)
                        FM_KEYON(CH, 1);
                    else
                        FM_KEYOFF(CH, 1);
                    if ((v & 128) != 0)
                    {
                        FM_KEYON(CH, 3);
                        break;
                    }
                    FM_KEYOFF(CH, 3);
                    break;
            }
        }

        private byte OPN_CHAN(int N) => (byte)(N & 3);

        private int OPN_SLOT(int N) => N >> 2 & 3;

        private void OPNWriteReg(int r, int v)
        {
            byte ch = OPN_CHAN(r);
            if (ch == 3)
                return;
            if (r >= 256)
                ch += 3;
            FM_CH CH = ym2612.CH[ch];
            FM_SLOT SLOT = CH.SLOT[OPN_SLOT(r)];
            switch (r & 240)
            {
                case 48:
                    set_det_mul(CH, SLOT, v);
                    break;
                case 64:
                    set_tl(SLOT, v);
                    break;
                case 80:
                    set_ar_ksr(CH, SLOT, v);
                    break;
                case 96:
                    set_dr(SLOT, v);
                    SLOT.AMmask = (v & 128) != 0 ? uint.MaxValue : 0U;
                    break;
                case 112:
                    set_sr(SLOT, v);
                    break;
                case 128:
                    set_sl_rr(SLOT, v);
                    break;
                case 144:
                    SLOT.ssg = (byte)(v & 15);
                    if (SLOT.state <= 1)
                        break;
                    if ((SLOT.ssg & 8) != 0 && (SLOT.ssgn ^ SLOT.ssg & 4) != 0)
                    {
                        SLOT.vol_out = ((uint)(512UL - (ulong)SLOT.volume) & 1023U) + SLOT.tl;
                        break;
                    }
                    SLOT.vol_out = (uint)SLOT.volume + SLOT.tl;
                    break;
                case 160:
                    switch (OPN_SLOT(r))
                    {
                        case 0:
                            uint num1 = (uint)((uint)((ym2612.OPN.ST.fn_h & 7) << 8) + (ulong)v);
                            byte num2 = (byte)((uint)ym2612.OPN.ST.fn_h >> 3);
                            CH.kcode = (byte)((uint)num2 << 2 | opn_fktable[(int)(num1 >> 7)]);
                            CH.fc = ym2612.OPN.fn_table[(int)num1 * 2] >> 7 - num2;
                            CH.block_fnum = (uint)num2 << 11 | num1;
                            CH.SLOT[0].Incr = -1L;
                            return;
                        case 1:
                            ym2612.OPN.ST.fn_h = (byte)(v & 63);
                            return;
                        case 2:
                            if (r >= 256)
                                return;
                            uint num3 = (uint)((uint)((ym2612.OPN.SL3.fn_h & 7) << 8) + (ulong)v);
                            byte num4 = (byte)((uint)ym2612.OPN.SL3.fn_h >> 3);
                            ym2612.OPN.SL3.kcode[ch] = (byte)((uint)num4 << 2 | opn_fktable[(int)(num3 >> 7)]);
                            ym2612.OPN.SL3.fc[ch] = ym2612.OPN.fn_table[(int)num3 * 2] >> 7 - num4;
                            ym2612.OPN.SL3.block_fnum[ch] = (uint)num4 << 11 | num3;
                            ym2612.CH[2].SLOT[0].Incr = -1L;
                            return;
                        case 3:
                            if (r >= 256)
                                return;
                            ym2612.OPN.SL3.fn_h = (byte)(v & 63);
                            return;
                        default:
                            return;
                    }
                case 176:
                    switch (OPN_SLOT(r))
                    {
                        case 0:
                            int num5 = v >> 3 & 7;
                            CH.ALGO = (byte)(v & 7);
                            CH.FB = num5 != 0 ? (byte)(num5 + 6) : (byte)0;
                            setup_connection(CH, ch);
                            return;
                        case 1:
                            CH.pms = (v & 7) * 32;
                            CH.ams = lfo_ams_depth_shift[v >> 4 & 3];
                            ym2612.OPN.pan[ch * 2] = (v & 128) != 0 ? uint.MaxValue : 0U;
                            ym2612.OPN.pan[ch * 2 + 1] = (v & 64) != 0 ? uint.MaxValue : 0U;
                            return;
                        default:
                            return;
                    }
            }
        }

        private void init_timetables(double freqbase)
        {
            for (int index1 = 0; index1 <= 3; ++index1)
            {
                for (int index2 = 0; index2 <= 31; ++index2)
                {
                    double num = dt_tab[index1 * 32 + index2] * freqbase * 64.0;
                    ym2612.OPN.ST.dt_tab[index1].value[index2] = (int)num;
                    ym2612.OPN.ST.dt_tab[index1 + 4].value[index2] = -ym2612.OPN.ST.dt_tab[index1].value[index2];
                }
            }
            for (int index = 0; index < 4096; ++index)
                ym2612.OPN.fn_table[index] = (uint)(index * 32.0 * freqbase * 64.0);
            ym2612.OPN.fn_max = (uint)(131072.0 * freqbase * 64.0);
        }

        private void OPNSetPres(int pres)
        {
            double freqbase = ym2612.OPN.ST.clock / ym2612.OPN.ST.rate / pres;
            if (config.hq_fm != 0)
                freqbase = 1.0;
            ym2612.OPN.eg_timer_add = (uint)(65536.0 * freqbase);
            ym2612.OPN.eg_timer_overflow = 196608U;
            ym2612.OPN.lfo_timer_add = (uint)(16777216.0 * freqbase);
            ym2612.OPN.ST.TimerBase = (int)(65536.0 * freqbase);
            init_timetables(freqbase);
        }

        private void reset_channels(FM_CH[] CH, int num)
        {
            for (int index1 = 0; index1 < num; ++index1)
            {
                CH[index1].mem_value = 0L;
                CH[index1].op1_out[0] = 0L;
                CH[index1].op1_out[1] = 0L;
                for (int index2 = 0; index2 < 4; ++index2)
                {
                    CH[index1].SLOT[index2].Incr = -1L;
                    CH[index1].SLOT[index2].key = 0;
                    CH[index1].SLOT[index2].phase = 0U;
                    CH[index1].SLOT[index2].ssgn = 0;
                    CH[index1].SLOT[index2].state = 0;
                    CH[index1].SLOT[index2].volume = 1023L;
                    CH[index1].SLOT[index2].vol_out = 1023U;
                }
            }
        }

        private void init_tables()
        {
            uint num1 = (uint)~((1 << 14 - config.dac_bits) - 1);
            for (int index1 = 0; index1 < 256; ++index1)
            {
                int num2 = (int)Math.Floor(65536.0 / Math.Pow(2.0, (index1 + 1) * (1.0 / 32.0) / 8.0)) >> 4;
                int num3 = ((num2 & 1) == 0 ? num2 >> 1 : (num2 >> 1) + 1) << 2;
                tl_tab[index1 * 2] = (int)(num3 & num1);
                tl_tab[index1 * 2 + 1] = (int)(-tl_tab[index1 * 2] & num1);
                for (int index2 = 1; index2 < 13; ++index2)
                {
                    tl_tab[index1 * 2 + index2 * 2 * 256] = (int)(tl_tab[index1 * 2] >> index2 & num1);
                    tl_tab[index1 * 2 + 1 + index2 * 2 * 256] = (int)(-tl_tab[index1 * 2 + index2 * 2 * 256] & num1);
                }
            }
            for (int index = 0; index < 1024; ++index)
            {
                double num4 = Math.Sin((index * 2 + 1) * Math.PI / 1024.0);
                int num5 = (int)(2.0 * ((num4 <= 0.0 ? 8.0 * Math.Log(-1.0 / num4) / Math.Log(2.0) : 8.0 * Math.Log(1.0 / num4) / Math.Log(2.0)) / (1.0 / 32.0)));
                int num6 = (num5 & 1) == 0 ? num5 >> 1 : (num5 >> 1) + 1;
                sin_tab[index] = (uint)(num6 * 2 + (num4 >= 0.0 ? 0 : 1));
            }
            for (int index3 = 0; index3 < 8; ++index3)
            {
                for (byte index4 = 0; index4 < 128; ++index4)
                {
                    uint num7 = (uint)index3;
                    for (byte index5 = 0; index5 < 8; ++index5)
                    {
                        byte num8 = 0;
                        for (uint index6 = 0; index6 < 7U; ++index6)
                        {
                            if ((index4 & 1 << (int)index6) != 0)
                            {
                                uint num9 = index6 * 8U;
                                num8 += lfo_pm_output[(int)num9 + (int)num7, index5];
                            }
                        }
                        lfo_pm_table[index4 * 32 * 8 + index3 * 32 + index5] = num8;
                        lfo_pm_table[index4 * 32 * 8 + index3 * 32 + (index5 ^ 7) + 8] = num8;
                        lfo_pm_table[index4 * 32 * 8 + index3 * 32 + index5 + 16] = -num8;
                        lfo_pm_table[index4 * 32 * 8 + index3 * 32 + (index5 ^ 7) + 24] = -num8;
                    }
                }
            }
        }

        public void YM2612Init(double clock, int rate)
        {
            config.psg_preamp = 150;
            config.fm_preamp = 100;
            config.hq_fm = 0;
            config.psgBoostNoise = 0;
            config.filter = 0;
            config.lp_range = 50;
            config.low_freq = 880;
            config.high_freq = 5000;
            config.lg = 1;
            config.mg = 1;
            config.hg = 1;
            config.rolloff = 0.99f;
            config.dac_bits = 14;
            config.ym2413 = 2;
            config.system = 0;
            config.region_detect = 0;
            config.vdp_mode = 0;
            config.master_clock = 0;
            config.force_dtack = 0;
            config.addr_error = 1;
            config.bios = 0;
            config.lock_on = 0;
            config.hot_swap = 0;
            config.xshift = 0;
            config.yshift = 0;
            config.xscale = 0;
            config.yscale = 0;
            config.aspect = 1;
            config.overscan = 3;
            config.ntsc = 0;
            config.vsync = 1;
            config.render = 0;
            config.bilinear = 0;
            config.tv_mode = 1;
            config.gun_cursor[0] = 1;
            config.gun_cursor[1] = 1;
            config.invert_mouse = 0;
            config.autoload = 0;
            config.autocheat = 0;
            config.s_auto = 0;
            config.s_default = 1;
            config.s_device = 0;
            config.l_device = 0;
            config.bg_overlay = 0;
            config.screen_w = 658;
            config.bgm_volume = 100f;
            config.sfx_volume = 100f;
            config.hot_swap &= 1;
            init_tables();
            ym2612.OPN.ST.clock = clock;
            ym2612.OPN.ST.rate = (uint)rate;
            OPNSetPres(144);
        }

        public void YM2612ResetChip()
        {
            ym2612.OPN.eg_timer = 0U;
            ym2612.OPN.eg_cnt = 0U;
            ym2612.OPN.lfo_timer_overflow = 0U;
            ym2612.OPN.lfo_timer = 0U;
            ym2612.OPN.lfo_cnt = 0;
            ym2612.OPN.LFO_AM = 0U;
            ym2612.OPN.LFO_PM = 0U;
            ym2612.OPN.ST.TAC = 0L;
            ym2612.OPN.ST.TBC = 0L;
            ym2612.OPN.SL3.key_csm = 0;
            ym2612.dacen = 0;
            ym2612.dacout = 0L;
            set_timers(48);
            ym2612.OPN.ST.TB = 0L;
            ym2612.OPN.ST.TBL = 268435456L;
            ym2612.OPN.ST.TA = 0L;
            ym2612.OPN.ST.TAL = 67108864L;
            reset_channels(ym2612.CH, 6);
            for (int r = 182; r >= 180; --r)
            {
                OPNWriteReg(r, 192);
                OPNWriteReg(r | 256, 192);
            }
            for (int r = 178; r >= 48; --r)
            {
                OPNWriteReg(r, 0);
                OPNWriteReg(r | 256, 0);
            }
        }

        public void YM2612Write(uint a, uint v)
        {
            v &= byte.MaxValue;
            if (a != 0U)
            {
                if (a == 2U)
                {
                    ym2612.OPN.ST.address = (ushort)(v | 256U);
                }
                else
                {
                    int address = ym2612.OPN.ST.address;
                    if ((address & 496) == 32)
                    {
                        if (address != 42)
                        {
                            if (address == 43)
                                ym2612.dacen = (byte)(v & 128U);
                            else
                                OPNWriteMode(address, (int)v);
                        }
                        else
                            ym2612.dacout = (int)v - 128 << 6;
                    }
                    else
                        OPNWriteReg(address, (int)v);
                }
            }
            else
                ym2612.OPN.ST.address = (ushort)v;
        }

        public uint YM2612Read() => ym2612.OPN.ST.status & (uint)byte.MaxValue;

        public void YM2612Update(int[] buffer, int length)
        {
            refresh_fc_eg_chan(ym2612.CH[0]);
            refresh_fc_eg_chan(ym2612.CH[1]);
            if (((int)ym2612.OPN.ST.mode & 192) == 0)
                refresh_fc_eg_chan(ym2612.CH[2]);
            else if (ym2612.CH[2].SLOT[0].Incr == -1L)
            {
                refresh_fc_eg_slot(ym2612.CH[2].SLOT[0], (int)ym2612.OPN.SL3.fc[1], ym2612.OPN.SL3.kcode[1]);
                refresh_fc_eg_slot(ym2612.CH[2].SLOT[2], (int)ym2612.OPN.SL3.fc[2], ym2612.OPN.SL3.kcode[2]);
                refresh_fc_eg_slot(ym2612.CH[2].SLOT[1], (int)ym2612.OPN.SL3.fc[0], ym2612.OPN.SL3.kcode[0]);
                refresh_fc_eg_slot(ym2612.CH[2].SLOT[3], (int)ym2612.CH[2].fc, ym2612.CH[2].kcode);
            }
            refresh_fc_eg_chan(ym2612.CH[3]);
            refresh_fc_eg_chan(ym2612.CH[4]);
            refresh_fc_eg_chan(ym2612.CH[5]);
            int index1 = 0;
            for (int index2 = 0; index2 < length; ++index2)
            {
                out_fm[0].value = 0L;
                out_fm[1].value = 0L;
                out_fm[2].value = 0L;
                out_fm[3].value = 0L;
                out_fm[4].value = 0L;
                out_fm[5].value = 0L;
                update_ssg_eg_channel(ym2612.CH[0].SLOT);
                update_ssg_eg_channel(ym2612.CH[1].SLOT);
                update_ssg_eg_channel(ym2612.CH[2].SLOT);
                update_ssg_eg_channel(ym2612.CH[3].SLOT);
                update_ssg_eg_channel(ym2612.CH[4].SLOT);
                update_ssg_eg_channel(ym2612.CH[5].SLOT);
                chan_calc(ym2612.CH[0]);
                chan_calc(ym2612.CH[1]);
                chan_calc(ym2612.CH[2]);
                chan_calc(ym2612.CH[3]);
                chan_calc(ym2612.CH[4]);
                if (ym2612.dacen == 0)
                    chan_calc(ym2612.CH[5]);
                else
                    out_fm[5].value = ym2612.dacout;
                advance_lfo();
                ym2612.OPN.eg_timer += ym2612.OPN.eg_timer_add;
                while (ym2612.OPN.eg_timer >= ym2612.OPN.eg_timer_overflow)
                {
                    ym2612.OPN.eg_timer -= ym2612.OPN.eg_timer_overflow;
                    ++ym2612.OPN.eg_cnt;
                    advance_eg_channels();
                }
                if (out_fm[0].value > 8192L)
                    out_fm[0].value = 8192L;
                else if (out_fm[0].value < -8192L)
                    out_fm[0].value = -8192L;
                if (out_fm[1].value > 8192L)
                    out_fm[1].value = 8192L;
                else if (out_fm[1].value < -8192L)
                    out_fm[1].value = -8192L;
                if (out_fm[2].value > 8192L)
                    out_fm[2].value = 8192L;
                else if (out_fm[2].value < -8192L)
                    out_fm[2].value = -8192L;
                if (out_fm[3].value > 8192L)
                    out_fm[3].value = 8192L;
                else if (out_fm[3].value < -8192L)
                    out_fm[3].value = -8192L;
                if (out_fm[4].value > 8192L)
                    out_fm[4].value = 8192L;
                else if (out_fm[4].value < -8192L)
                    out_fm[4].value = -8192L;
                if (out_fm[5].value > 8192L)
                    out_fm[5].value = 8192L;
                else if (out_fm[5].value < -8192L)
                    out_fm[5].value = -8192L;
                int num1 = (int)(out_fm[0].value & ym2612.OPN.pan[0]);
                int num2 = (int)(out_fm[0].value & ym2612.OPN.pan[1]);
                int num3 = num1 + (int)(out_fm[1].value & ym2612.OPN.pan[2]);
                int num4 = num2 + (int)(out_fm[1].value & ym2612.OPN.pan[3]);
                int num5 = num3 + (int)(out_fm[2].value & ym2612.OPN.pan[4]);
                int num6 = num4 + (int)(out_fm[2].value & ym2612.OPN.pan[5]);
                int num7 = num5 + (int)(out_fm[3].value & ym2612.OPN.pan[6]);
                int num8 = num6 + (int)(out_fm[3].value & ym2612.OPN.pan[7]);
                int num9 = num7 + (int)(out_fm[4].value & ym2612.OPN.pan[8]);
                int num10 = num8 + (int)(out_fm[4].value & ym2612.OPN.pan[9]);
                int num11 = num9 + (int)(out_fm[5].value & ym2612.OPN.pan[10]);
                int num12 = num10 + (int)(out_fm[5].value & ym2612.OPN.pan[11]);
                buffer[index1] = num11;
                int index3 = index1 + 1;
                buffer[index3] = num12;
                index1 = index3 + 1;
                ym2612.OPN.SL3.key_csm <<= 1;
                INTERNAL_TIMER_A();
                if ((ym2612.OPN.SL3.key_csm & 2) != 0)
                {
                    FM_KEYOFF_CSM(ym2612.CH[2], 0);
                    FM_KEYOFF_CSM(ym2612.CH[2], 2);
                    FM_KEYOFF_CSM(ym2612.CH[2], 1);
                    FM_KEYOFF_CSM(ym2612.CH[2], 3);
                    ym2612.OPN.SL3.key_csm = 0;
                }
            }
            INTERNAL_TIMER_B(length);
        }

        public class FM_SLOT
        {
            public LongPointerArray32 DT = new LongPointerArray32();
            public byte KSR;
            public uint ar;
            public uint d1r;
            public uint d2r;
            public uint rr;
            public byte ksr;
            public uint mul;
            public uint phase;
            public long Incr;
            public byte state;
            public uint tl;
            public long volume;
            public uint sl;
            public uint vol_out;
            public byte eg_sh_ar;
            public byte eg_sel_ar;
            public byte eg_sh_d1r;
            public byte eg_sel_d1r;
            public byte eg_sh_d2r;
            public byte eg_sel_d2r;
            public byte eg_sh_rr;
            public byte eg_sel_rr;
            public byte ssg;
            public byte ssgn;
            public byte key;
            public uint AMmask;
        }

        public class FM_CH
        {
            public FM_SLOT[] SLOT = new FM_SLOT[4];
            public byte ALGO;
            public byte FB;
            public long[] op1_out = new long[2];
            public LongPointer connect1;
            public LongPointer connect3;
            public LongPointer connect2;
            public LongPointer connect4;
            public LongPointer mem_connect;
            public long mem_value;
            public long pms;
            public byte ams;
            public uint fc;
            public byte kcode;
            public uint block_fnum;

            public FM_CH()
            {
                SLOT[0] = new FM_SLOT();
                SLOT[1] = new FM_SLOT();
                SLOT[2] = new FM_SLOT();
                SLOT[3] = new FM_SLOT();
            }
        }

        public class FM_ST
        {
            public double clock;
            public uint rate;
            public ushort address;
            public byte status;
            public uint mode;
            public byte fn_h;
            public long TimerBase;
            public long TA;
            public long TAL;
            public long TAC;
            public long TB;
            public long TBL;
            public long TBC;
            public LongPointerArray32[] dt_tab = new LongPointerArray32[8];

            public FM_ST()
            {
                for (int index = 0; index < 8; ++index)
                    dt_tab[index] = new LongPointerArray32();
            }
        }

        public class FM_3SLOT
        {
            public uint[] fc = new uint[3];
            public byte fn_h;
            public byte[] kcode = new byte[3];
            public uint[] block_fnum = new uint[3];
            public byte key_csm;
        }

        public class FM_OPN
        {
            public FM_ST ST = new FM_ST();
            public FM_3SLOT SL3 = new FM_3SLOT();
            public uint[] pan = new uint[12];
            public uint eg_cnt;
            public uint eg_timer;
            public uint eg_timer_add;
            public uint eg_timer_overflow;
            public uint[] fn_table = new uint[4096];
            public uint fn_max;
            public byte lfo_cnt;
            public uint lfo_timer;
            public uint lfo_timer_add;
            public uint lfo_timer_overflow;
            public uint LFO_AM;
            public uint LFO_PM;
        }

        public class _YM2612_data
        {
            public FM_CH[] CH = new FM_CH[6];
            public byte dacen;
            public long dacout;
            public FM_OPN OPN = new FM_OPN();

            public _YM2612_data()
            {
                for (int index = 0; index < 6; ++index)
                    CH[index] = new FM_CH();
            }
        }
    }
}
