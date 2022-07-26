// Decompiled with JetBrains decompiler
// Type: DuckGame.YM2612Core
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections.Generic;
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
      YM2612Core.SC(0),
      YM2612Core.SC(1),
      YM2612Core.SC(2),
      YM2612Core.SC(3),
      YM2612Core.SC(4),
      YM2612Core.SC(5),
      YM2612Core.SC(6),
      YM2612Core.SC(7),
      YM2612Core.SC(8),
      YM2612Core.SC(9),
      YM2612Core.SC(10),
      YM2612Core.SC(11),
      YM2612Core.SC(12),
      YM2612Core.SC(13),
      YM2612Core.SC(14),
      YM2612Core.SC(31)
        };
        private const int RATE_STEPS = 8;
        private static byte[] eg_inc = new byte[152]
        {
      (byte) 0,
      (byte) 1,
      (byte) 0,
      (byte) 1,
      (byte) 0,
      (byte) 1,
      (byte) 0,
      (byte) 1,
      (byte) 0,
      (byte) 1,
      (byte) 0,
      (byte) 1,
      (byte) 1,
      (byte) 1,
      (byte) 0,
      (byte) 1,
      (byte) 0,
      (byte) 1,
      (byte) 1,
      (byte) 1,
      (byte) 0,
      (byte) 1,
      (byte) 1,
      (byte) 1,
      (byte) 0,
      (byte) 1,
      (byte) 1,
      (byte) 1,
      (byte) 1,
      (byte) 1,
      (byte) 1,
      (byte) 1,
      (byte) 1,
      (byte) 1,
      (byte) 1,
      (byte) 1,
      (byte) 1,
      (byte) 1,
      (byte) 1,
      (byte) 1,
      (byte) 1,
      (byte) 1,
      (byte) 1,
      (byte) 2,
      (byte) 1,
      (byte) 1,
      (byte) 1,
      (byte) 2,
      (byte) 1,
      (byte) 2,
      (byte) 1,
      (byte) 2,
      (byte) 1,
      (byte) 2,
      (byte) 1,
      (byte) 2,
      (byte) 1,
      (byte) 2,
      (byte) 2,
      (byte) 2,
      (byte) 1,
      (byte) 2,
      (byte) 2,
      (byte) 2,
      (byte) 2,
      (byte) 2,
      (byte) 2,
      (byte) 2,
      (byte) 2,
      (byte) 2,
      (byte) 2,
      (byte) 2,
      (byte) 2,
      (byte) 2,
      (byte) 2,
      (byte) 4,
      (byte) 2,
      (byte) 2,
      (byte) 2,
      (byte) 4,
      (byte) 2,
      (byte) 4,
      (byte) 2,
      (byte) 4,
      (byte) 2,
      (byte) 4,
      (byte) 2,
      (byte) 4,
      (byte) 2,
      (byte) 4,
      (byte) 4,
      (byte) 4,
      (byte) 2,
      (byte) 4,
      (byte) 4,
      (byte) 4,
      (byte) 4,
      (byte) 4,
      (byte) 4,
      (byte) 4,
      (byte) 4,
      (byte) 4,
      (byte) 4,
      (byte) 4,
      (byte) 4,
      (byte) 4,
      (byte) 4,
      (byte) 8,
      (byte) 4,
      (byte) 4,
      (byte) 4,
      (byte) 8,
      (byte) 4,
      (byte) 8,
      (byte) 4,
      (byte) 8,
      (byte) 4,
      (byte) 8,
      (byte) 4,
      (byte) 8,
      (byte) 4,
      (byte) 8,
      (byte) 8,
      (byte) 8,
      (byte) 4,
      (byte) 8,
      (byte) 8,
      (byte) 8,
      (byte) 8,
      (byte) 8,
      (byte) 8,
      (byte) 8,
      (byte) 8,
      (byte) 8,
      (byte) 8,
      (byte) 8,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0
        };
        private static byte[] eg_rate_select = new byte[128]
        {
      YM2612Core.eg_rate_selectO((byte) 18),
      YM2612Core.eg_rate_selectO((byte) 18),
      YM2612Core.eg_rate_selectO((byte) 18),
      YM2612Core.eg_rate_selectO((byte) 18),
      YM2612Core.eg_rate_selectO((byte) 18),
      YM2612Core.eg_rate_selectO((byte) 18),
      YM2612Core.eg_rate_selectO((byte) 18),
      YM2612Core.eg_rate_selectO((byte) 18),
      YM2612Core.eg_rate_selectO((byte) 18),
      YM2612Core.eg_rate_selectO((byte) 18),
      YM2612Core.eg_rate_selectO((byte) 18),
      YM2612Core.eg_rate_selectO((byte) 18),
      YM2612Core.eg_rate_selectO((byte) 18),
      YM2612Core.eg_rate_selectO((byte) 18),
      YM2612Core.eg_rate_selectO((byte) 18),
      YM2612Core.eg_rate_selectO((byte) 18),
      YM2612Core.eg_rate_selectO((byte) 18),
      YM2612Core.eg_rate_selectO((byte) 18),
      YM2612Core.eg_rate_selectO((byte) 18),
      YM2612Core.eg_rate_selectO((byte) 18),
      YM2612Core.eg_rate_selectO((byte) 18),
      YM2612Core.eg_rate_selectO((byte) 18),
      YM2612Core.eg_rate_selectO((byte) 18),
      YM2612Core.eg_rate_selectO((byte) 18),
      YM2612Core.eg_rate_selectO((byte) 18),
      YM2612Core.eg_rate_selectO((byte) 18),
      YM2612Core.eg_rate_selectO((byte) 18),
      YM2612Core.eg_rate_selectO((byte) 18),
      YM2612Core.eg_rate_selectO((byte) 18),
      YM2612Core.eg_rate_selectO((byte) 18),
      YM2612Core.eg_rate_selectO((byte) 18),
      YM2612Core.eg_rate_selectO((byte) 18),
      YM2612Core.eg_rate_selectO((byte) 18),
      YM2612Core.eg_rate_selectO((byte) 18),
      YM2612Core.eg_rate_selectO((byte) 0),
      YM2612Core.eg_rate_selectO((byte) 0),
      YM2612Core.eg_rate_selectO((byte) 0),
      YM2612Core.eg_rate_selectO((byte) 0),
      YM2612Core.eg_rate_selectO((byte) 2),
      YM2612Core.eg_rate_selectO((byte) 2),
      YM2612Core.eg_rate_selectO((byte) 0),
      YM2612Core.eg_rate_selectO((byte) 1),
      YM2612Core.eg_rate_selectO((byte) 2),
      YM2612Core.eg_rate_selectO((byte) 3),
      YM2612Core.eg_rate_selectO((byte) 0),
      YM2612Core.eg_rate_selectO((byte) 1),
      YM2612Core.eg_rate_selectO((byte) 2),
      YM2612Core.eg_rate_selectO((byte) 3),
      YM2612Core.eg_rate_selectO((byte) 0),
      YM2612Core.eg_rate_selectO((byte) 1),
      YM2612Core.eg_rate_selectO((byte) 2),
      YM2612Core.eg_rate_selectO((byte) 3),
      YM2612Core.eg_rate_selectO((byte) 0),
      YM2612Core.eg_rate_selectO((byte) 1),
      YM2612Core.eg_rate_selectO((byte) 2),
      YM2612Core.eg_rate_selectO((byte) 3),
      YM2612Core.eg_rate_selectO((byte) 0),
      YM2612Core.eg_rate_selectO((byte) 1),
      YM2612Core.eg_rate_selectO((byte) 2),
      YM2612Core.eg_rate_selectO((byte) 3),
      YM2612Core.eg_rate_selectO((byte) 0),
      YM2612Core.eg_rate_selectO((byte) 1),
      YM2612Core.eg_rate_selectO((byte) 2),
      YM2612Core.eg_rate_selectO((byte) 3),
      YM2612Core.eg_rate_selectO((byte) 0),
      YM2612Core.eg_rate_selectO((byte) 1),
      YM2612Core.eg_rate_selectO((byte) 2),
      YM2612Core.eg_rate_selectO((byte) 3),
      YM2612Core.eg_rate_selectO((byte) 0),
      YM2612Core.eg_rate_selectO((byte) 1),
      YM2612Core.eg_rate_selectO((byte) 2),
      YM2612Core.eg_rate_selectO((byte) 3),
      YM2612Core.eg_rate_selectO((byte) 0),
      YM2612Core.eg_rate_selectO((byte) 1),
      YM2612Core.eg_rate_selectO((byte) 2),
      YM2612Core.eg_rate_selectO((byte) 3),
      YM2612Core.eg_rate_selectO((byte) 0),
      YM2612Core.eg_rate_selectO((byte) 1),
      YM2612Core.eg_rate_selectO((byte) 2),
      YM2612Core.eg_rate_selectO((byte) 3),
      YM2612Core.eg_rate_selectO((byte) 4),
      YM2612Core.eg_rate_selectO((byte) 5),
      YM2612Core.eg_rate_selectO((byte) 6),
      YM2612Core.eg_rate_selectO((byte) 7),
      YM2612Core.eg_rate_selectO((byte) 8),
      YM2612Core.eg_rate_selectO((byte) 9),
      YM2612Core.eg_rate_selectO((byte) 10),
      YM2612Core.eg_rate_selectO((byte) 11),
      YM2612Core.eg_rate_selectO((byte) 12),
      YM2612Core.eg_rate_selectO((byte) 13),
      YM2612Core.eg_rate_selectO((byte) 14),
      YM2612Core.eg_rate_selectO((byte) 15),
      YM2612Core.eg_rate_selectO((byte) 16),
      YM2612Core.eg_rate_selectO((byte) 16),
      YM2612Core.eg_rate_selectO((byte) 16),
      YM2612Core.eg_rate_selectO((byte) 16),
      YM2612Core.eg_rate_selectO((byte) 16),
      YM2612Core.eg_rate_selectO((byte) 16),
      YM2612Core.eg_rate_selectO((byte) 16),
      YM2612Core.eg_rate_selectO((byte) 16),
      YM2612Core.eg_rate_selectO((byte) 16),
      YM2612Core.eg_rate_selectO((byte) 16),
      YM2612Core.eg_rate_selectO((byte) 16),
      YM2612Core.eg_rate_selectO((byte) 16),
      YM2612Core.eg_rate_selectO((byte) 16),
      YM2612Core.eg_rate_selectO((byte) 16),
      YM2612Core.eg_rate_selectO((byte) 16),
      YM2612Core.eg_rate_selectO((byte) 16),
      YM2612Core.eg_rate_selectO((byte) 16),
      YM2612Core.eg_rate_selectO((byte) 16),
      YM2612Core.eg_rate_selectO((byte) 16),
      YM2612Core.eg_rate_selectO((byte) 16),
      YM2612Core.eg_rate_selectO((byte) 16),
      YM2612Core.eg_rate_selectO((byte) 16),
      YM2612Core.eg_rate_selectO((byte) 16),
      YM2612Core.eg_rate_selectO((byte) 16),
      YM2612Core.eg_rate_selectO((byte) 16),
      YM2612Core.eg_rate_selectO((byte) 16),
      YM2612Core.eg_rate_selectO((byte) 16),
      YM2612Core.eg_rate_selectO((byte) 16),
      YM2612Core.eg_rate_selectO((byte) 16),
      YM2612Core.eg_rate_selectO((byte) 16),
      YM2612Core.eg_rate_selectO((byte) 16),
      YM2612Core.eg_rate_selectO((byte) 16),
      YM2612Core.eg_rate_selectO((byte) 16),
      YM2612Core.eg_rate_selectO((byte) 16),
      YM2612Core.eg_rate_selectO((byte) 16),
      YM2612Core.eg_rate_selectO((byte) 16)
        };
        private static byte[] eg_rate_shift = new byte[128]
        {
      YM2612Core.eg_rate_shiftO((byte) 11),
      YM2612Core.eg_rate_shiftO((byte) 11),
      YM2612Core.eg_rate_shiftO((byte) 11),
      YM2612Core.eg_rate_shiftO((byte) 11),
      YM2612Core.eg_rate_shiftO((byte) 11),
      YM2612Core.eg_rate_shiftO((byte) 11),
      YM2612Core.eg_rate_shiftO((byte) 11),
      YM2612Core.eg_rate_shiftO((byte) 11),
      YM2612Core.eg_rate_shiftO((byte) 11),
      YM2612Core.eg_rate_shiftO((byte) 11),
      YM2612Core.eg_rate_shiftO((byte) 11),
      YM2612Core.eg_rate_shiftO((byte) 11),
      YM2612Core.eg_rate_shiftO((byte) 11),
      YM2612Core.eg_rate_shiftO((byte) 11),
      YM2612Core.eg_rate_shiftO((byte) 11),
      YM2612Core.eg_rate_shiftO((byte) 11),
      YM2612Core.eg_rate_shiftO((byte) 11),
      YM2612Core.eg_rate_shiftO((byte) 11),
      YM2612Core.eg_rate_shiftO((byte) 11),
      YM2612Core.eg_rate_shiftO((byte) 11),
      YM2612Core.eg_rate_shiftO((byte) 11),
      YM2612Core.eg_rate_shiftO((byte) 11),
      YM2612Core.eg_rate_shiftO((byte) 11),
      YM2612Core.eg_rate_shiftO((byte) 11),
      YM2612Core.eg_rate_shiftO((byte) 11),
      YM2612Core.eg_rate_shiftO((byte) 11),
      YM2612Core.eg_rate_shiftO((byte) 11),
      YM2612Core.eg_rate_shiftO((byte) 11),
      YM2612Core.eg_rate_shiftO((byte) 11),
      YM2612Core.eg_rate_shiftO((byte) 11),
      YM2612Core.eg_rate_shiftO((byte) 11),
      YM2612Core.eg_rate_shiftO((byte) 11),
      YM2612Core.eg_rate_shiftO((byte) 11),
      YM2612Core.eg_rate_shiftO((byte) 11),
      YM2612Core.eg_rate_shiftO((byte) 11),
      YM2612Core.eg_rate_shiftO((byte) 11),
      YM2612Core.eg_rate_shiftO((byte) 10),
      YM2612Core.eg_rate_shiftO((byte) 10),
      YM2612Core.eg_rate_shiftO((byte) 10),
      YM2612Core.eg_rate_shiftO((byte) 10),
      YM2612Core.eg_rate_shiftO((byte) 9),
      YM2612Core.eg_rate_shiftO((byte) 9),
      YM2612Core.eg_rate_shiftO((byte) 9),
      YM2612Core.eg_rate_shiftO((byte) 9),
      YM2612Core.eg_rate_shiftO((byte) 8),
      YM2612Core.eg_rate_shiftO((byte) 8),
      YM2612Core.eg_rate_shiftO((byte) 8),
      YM2612Core.eg_rate_shiftO((byte) 8),
      YM2612Core.eg_rate_shiftO((byte) 7),
      YM2612Core.eg_rate_shiftO((byte) 7),
      YM2612Core.eg_rate_shiftO((byte) 7),
      YM2612Core.eg_rate_shiftO((byte) 7),
      YM2612Core.eg_rate_shiftO((byte) 6),
      YM2612Core.eg_rate_shiftO((byte) 6),
      YM2612Core.eg_rate_shiftO((byte) 6),
      YM2612Core.eg_rate_shiftO((byte) 6),
      YM2612Core.eg_rate_shiftO((byte) 5),
      YM2612Core.eg_rate_shiftO((byte) 5),
      YM2612Core.eg_rate_shiftO((byte) 5),
      YM2612Core.eg_rate_shiftO((byte) 5),
      YM2612Core.eg_rate_shiftO((byte) 4),
      YM2612Core.eg_rate_shiftO((byte) 4),
      YM2612Core.eg_rate_shiftO((byte) 4),
      YM2612Core.eg_rate_shiftO((byte) 4),
      YM2612Core.eg_rate_shiftO((byte) 3),
      YM2612Core.eg_rate_shiftO((byte) 3),
      YM2612Core.eg_rate_shiftO((byte) 3),
      YM2612Core.eg_rate_shiftO((byte) 3),
      YM2612Core.eg_rate_shiftO((byte) 2),
      YM2612Core.eg_rate_shiftO((byte) 2),
      YM2612Core.eg_rate_shiftO((byte) 2),
      YM2612Core.eg_rate_shiftO((byte) 2),
      YM2612Core.eg_rate_shiftO((byte) 1),
      YM2612Core.eg_rate_shiftO((byte) 1),
      YM2612Core.eg_rate_shiftO((byte) 1),
      YM2612Core.eg_rate_shiftO((byte) 1),
      YM2612Core.eg_rate_shiftO((byte) 0),
      YM2612Core.eg_rate_shiftO((byte) 0),
      YM2612Core.eg_rate_shiftO((byte) 0),
      YM2612Core.eg_rate_shiftO((byte) 0),
      YM2612Core.eg_rate_shiftO((byte) 0),
      YM2612Core.eg_rate_shiftO((byte) 0),
      YM2612Core.eg_rate_shiftO((byte) 0),
      YM2612Core.eg_rate_shiftO((byte) 0),
      YM2612Core.eg_rate_shiftO((byte) 0),
      YM2612Core.eg_rate_shiftO((byte) 0),
      YM2612Core.eg_rate_shiftO((byte) 0),
      YM2612Core.eg_rate_shiftO((byte) 0),
      YM2612Core.eg_rate_shiftO((byte) 0),
      YM2612Core.eg_rate_shiftO((byte) 0),
      YM2612Core.eg_rate_shiftO((byte) 0),
      YM2612Core.eg_rate_shiftO((byte) 0),
      YM2612Core.eg_rate_shiftO((byte) 0),
      YM2612Core.eg_rate_shiftO((byte) 0),
      YM2612Core.eg_rate_shiftO((byte) 0),
      YM2612Core.eg_rate_shiftO((byte) 0),
      YM2612Core.eg_rate_shiftO((byte) 0),
      YM2612Core.eg_rate_shiftO((byte) 0),
      YM2612Core.eg_rate_shiftO((byte) 0),
      YM2612Core.eg_rate_shiftO((byte) 0),
      YM2612Core.eg_rate_shiftO((byte) 0),
      YM2612Core.eg_rate_shiftO((byte) 0),
      YM2612Core.eg_rate_shiftO((byte) 0),
      YM2612Core.eg_rate_shiftO((byte) 0),
      YM2612Core.eg_rate_shiftO((byte) 0),
      YM2612Core.eg_rate_shiftO((byte) 0),
      YM2612Core.eg_rate_shiftO((byte) 0),
      YM2612Core.eg_rate_shiftO((byte) 0),
      YM2612Core.eg_rate_shiftO((byte) 0),
      YM2612Core.eg_rate_shiftO((byte) 0),
      YM2612Core.eg_rate_shiftO((byte) 0),
      YM2612Core.eg_rate_shiftO((byte) 0),
      YM2612Core.eg_rate_shiftO((byte) 0),
      YM2612Core.eg_rate_shiftO((byte) 0),
      YM2612Core.eg_rate_shiftO((byte) 0),
      YM2612Core.eg_rate_shiftO((byte) 0),
      YM2612Core.eg_rate_shiftO((byte) 0),
      YM2612Core.eg_rate_shiftO((byte) 0),
      YM2612Core.eg_rate_shiftO((byte) 0),
      YM2612Core.eg_rate_shiftO((byte) 0),
      YM2612Core.eg_rate_shiftO((byte) 0),
      YM2612Core.eg_rate_shiftO((byte) 0),
      YM2612Core.eg_rate_shiftO((byte) 0),
      YM2612Core.eg_rate_shiftO((byte) 0),
      YM2612Core.eg_rate_shiftO((byte) 0),
      YM2612Core.eg_rate_shiftO((byte) 0),
      YM2612Core.eg_rate_shiftO((byte) 0),
      YM2612Core.eg_rate_shiftO((byte) 0)
        };
        private static byte[] dt_tab = new byte[128]
        {
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 1,
      (byte) 1,
      (byte) 1,
      (byte) 1,
      (byte) 1,
      (byte) 1,
      (byte) 1,
      (byte) 1,
      (byte) 2,
      (byte) 2,
      (byte) 2,
      (byte) 2,
      (byte) 2,
      (byte) 3,
      (byte) 3,
      (byte) 3,
      (byte) 4,
      (byte) 4,
      (byte) 4,
      (byte) 5,
      (byte) 5,
      (byte) 6,
      (byte) 6,
      (byte) 7,
      (byte) 8,
      (byte) 8,
      (byte) 8,
      (byte) 8,
      (byte) 1,
      (byte) 1,
      (byte) 1,
      (byte) 1,
      (byte) 2,
      (byte) 2,
      (byte) 2,
      (byte) 2,
      (byte) 2,
      (byte) 3,
      (byte) 3,
      (byte) 3,
      (byte) 4,
      (byte) 4,
      (byte) 4,
      (byte) 5,
      (byte) 5,
      (byte) 6,
      (byte) 6,
      (byte) 7,
      (byte) 8,
      (byte) 8,
      (byte) 9,
      (byte) 10,
      (byte) 11,
      (byte) 12,
      (byte) 13,
      (byte) 14,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 2,
      (byte) 2,
      (byte) 2,
      (byte) 2,
      (byte) 2,
      (byte) 3,
      (byte) 3,
      (byte) 3,
      (byte) 4,
      (byte) 4,
      (byte) 4,
      (byte) 5,
      (byte) 5,
      (byte) 6,
      (byte) 6,
      (byte) 7,
      (byte) 8,
      (byte) 8,
      (byte) 9,
      (byte) 10,
      (byte) 11,
      (byte) 12,
      (byte) 13,
      (byte) 14,
      (byte) 16,
      (byte) 17,
      (byte) 19,
      (byte) 20,
      (byte) 22,
      (byte) 22,
      (byte) 22,
      (byte) 22
        };
        private static byte[] opn_fktable = new byte[16]
        {
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 1,
      (byte) 2,
      (byte) 3,
      (byte) 3,
      (byte) 3,
      (byte) 3,
      (byte) 3,
      (byte) 3,
      (byte) 3
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
      (byte) 8,
      (byte) 3,
      (byte) 1,
      (byte) 0
        };
        private static byte[,] lfo_pm_output = new byte[56, 8]
        {
      {
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0
      },
      {
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0
      },
      {
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0
      },
      {
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0
      },
      {
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0
      },
      {
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0
      },
      {
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0
      },
      {
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 1,
        (byte) 1,
        (byte) 1,
        (byte) 1
      },
      {
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0
      },
      {
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0
      },
      {
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0
      },
      {
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0
      },
      {
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0
      },
      {
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0
      },
      {
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 1,
        (byte) 1,
        (byte) 1,
        (byte) 1
      },
      {
        (byte) 0,
        (byte) 0,
        (byte) 1,
        (byte) 1,
        (byte) 2,
        (byte) 2,
        (byte) 2,
        (byte) 3
      },
      {
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0
      },
      {
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0
      },
      {
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0
      },
      {
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0
      },
      {
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 1
      },
      {
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 1,
        (byte) 1,
        (byte) 1,
        (byte) 1
      },
      {
        (byte) 0,
        (byte) 0,
        (byte) 1,
        (byte) 1,
        (byte) 2,
        (byte) 2,
        (byte) 2,
        (byte) 3
      },
      {
        (byte) 0,
        (byte) 0,
        (byte) 2,
        (byte) 3,
        (byte) 4,
        (byte) 4,
        (byte) 5,
        (byte) 6
      },
      {
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0
      },
      {
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0
      },
      {
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 1,
        (byte) 1
      },
      {
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 1,
        (byte) 1,
        (byte) 1,
        (byte) 1
      },
      {
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 1,
        (byte) 1,
        (byte) 1,
        (byte) 1,
        (byte) 2
      },
      {
        (byte) 0,
        (byte) 0,
        (byte) 1,
        (byte) 1,
        (byte) 2,
        (byte) 2,
        (byte) 2,
        (byte) 3
      },
      {
        (byte) 0,
        (byte) 0,
        (byte) 2,
        (byte) 3,
        (byte) 4,
        (byte) 4,
        (byte) 5,
        (byte) 6
      },
      {
        (byte) 0,
        (byte) 0,
        (byte) 4,
        (byte) 6,
        (byte) 8,
        (byte) 8,
        (byte) 10,
        (byte) 12
      },
      {
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0
      },
      {
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 1,
        (byte) 1,
        (byte) 1,
        (byte) 1
      },
      {
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 1,
        (byte) 1,
        (byte) 1,
        (byte) 2,
        (byte) 2
      },
      {
        (byte) 0,
        (byte) 0,
        (byte) 1,
        (byte) 1,
        (byte) 2,
        (byte) 2,
        (byte) 3,
        (byte) 3
      },
      {
        (byte) 0,
        (byte) 0,
        (byte) 1,
        (byte) 2,
        (byte) 2,
        (byte) 2,
        (byte) 3,
        (byte) 4
      },
      {
        (byte) 0,
        (byte) 0,
        (byte) 2,
        (byte) 3,
        (byte) 4,
        (byte) 4,
        (byte) 5,
        (byte) 6
      },
      {
        (byte) 0,
        (byte) 0,
        (byte) 4,
        (byte) 6,
        (byte) 8,
        (byte) 8,
        (byte) 10,
        (byte) 12
      },
      {
        (byte) 0,
        (byte) 0,
        (byte) 8,
        (byte) 12,
        (byte) 16,
        (byte) 16,
        (byte) 20,
        (byte) 24
      },
      {
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0
      },
      {
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 2,
        (byte) 2,
        (byte) 2,
        (byte) 2
      },
      {
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 2,
        (byte) 2,
        (byte) 2,
        (byte) 4,
        (byte) 4
      },
      {
        (byte) 0,
        (byte) 0,
        (byte) 2,
        (byte) 2,
        (byte) 4,
        (byte) 4,
        (byte) 6,
        (byte) 6
      },
      {
        (byte) 0,
        (byte) 0,
        (byte) 2,
        (byte) 4,
        (byte) 4,
        (byte) 4,
        (byte) 6,
        (byte) 8
      },
      {
        (byte) 0,
        (byte) 0,
        (byte) 4,
        (byte) 6,
        (byte) 8,
        (byte) 8,
        (byte) 10,
        (byte) 12
      },
      {
        (byte) 0,
        (byte) 0,
        (byte) 8,
        (byte) 12,
        (byte) 16,
        (byte) 16,
        (byte) 20,
        (byte) 24
      },
      {
        (byte) 0,
        (byte) 0,
        (byte) 16,
        (byte) 24,
        (byte) 32,
        (byte) 32,
        (byte) 40,
        (byte) 48
      },
      {
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0
      },
      {
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 4,
        (byte) 4,
        (byte) 4,
        (byte) 4
      },
      {
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 4,
        (byte) 4,
        (byte) 4,
        (byte) 8,
        (byte) 8
      },
      {
        (byte) 0,
        (byte) 0,
        (byte) 4,
        (byte) 4,
        (byte) 8,
        (byte) 8,
        (byte) 12,
        (byte) 12
      },
      {
        (byte) 0,
        (byte) 0,
        (byte) 4,
        (byte) 8,
        (byte) 8,
        (byte) 8,
        (byte) 12,
        (byte) 16
      },
      {
        (byte) 0,
        (byte) 0,
        (byte) 8,
        (byte) 12,
        (byte) 16,
        (byte) 16,
        (byte) 20,
        (byte) 24
      },
      {
        (byte) 0,
        (byte) 0,
        (byte) 16,
        (byte) 24,
        (byte) 32,
        (byte) 32,
        (byte) 40,
        (byte) 48
      },
      {
        (byte) 0,
        (byte) 0,
        (byte) 32,
        (byte) 48,
        (byte) 64,
        (byte) 64,
        (byte) 80,
        (byte) 96
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
        private YM2612Core._YM2612_data ym2612 = new YM2612Core._YM2612_data();
        private LongPointer m2 = new LongPointer();
        private LongPointer c1 = new LongPointer();
        private LongPointer c2 = new LongPointer();
        private LongPointer mem = new LongPointer();
        private LongPointer[] out_fm = new LongPointer[8];

        private static uint SC(int db) => (uint)((double)db * 32.0);

        private static byte eg_rate_selectO(byte a) => (byte)((uint)a * 8U);

        private static byte eg_rate_shiftO(byte a) => a;

        public YM2612Core()
        {
            for (int index = 0; index < 8; ++index)
                this.out_fm[index] = new LongPointer();
        }

        private void FM_KEYON(YM2612Core.FM_CH CH, int s)
        {
            YM2612Core.FM_SLOT fmSlot = CH.SLOT[s];
            if (fmSlot.key == (byte)0 && this.ym2612.OPN.SL3.key_csm == (byte)0)
            {
                fmSlot.phase = 0U;
                fmSlot.ssgn = (byte)0;
                if (fmSlot.ar + (uint)fmSlot.ksr < 94U)
                {
                    fmSlot.state = fmSlot.volume <= 0L ? (fmSlot.sl == 0U ? (byte)2 : (byte)3) : (byte)4;
                }
                else
                {
                    fmSlot.volume = 0L;
                    fmSlot.state = fmSlot.sl == 0U ? (byte)2 : (byte)3;
                }
                fmSlot.vol_out = ((int)fmSlot.ssg & 8) == 0 || ((int)fmSlot.ssgn ^ (int)fmSlot.ssg & 4) == 0 ? (uint)fmSlot.volume + fmSlot.tl : ((uint)(512UL - (ulong)fmSlot.volume) & 1023U) + fmSlot.tl;
            }
            fmSlot.key = (byte)1;
        }

        private void FM_KEYOFF(YM2612Core.FM_CH CH, int s)
        {
            YM2612Core.FM_SLOT fmSlot = CH.SLOT[s];
            if (fmSlot.key > (byte)0 && this.ym2612.OPN.SL3.key_csm == (byte)0 && fmSlot.state > (byte)1)
            {
                fmSlot.state = (byte)1;
                if (((int)fmSlot.ssg & 8) > 0)
                {
                    if (((int)fmSlot.ssgn ^ (int)fmSlot.ssg & 4) != 0)
                        fmSlot.volume = 512L - fmSlot.volume;
                    if (fmSlot.volume >= 512L)
                    {
                        fmSlot.volume = 1023L;
                        fmSlot.state = (byte)0;
                    }
                    fmSlot.vol_out = (uint)fmSlot.volume + fmSlot.tl;
                }
            }
            fmSlot.key = (byte)0;
        }

        private void FM_KEYON_CSM(YM2612Core.FM_CH CH, int s)
        {
            YM2612Core.FM_SLOT fmSlot = CH.SLOT[s];
            if (fmSlot.key != (byte)0 || this.ym2612.OPN.SL3.key_csm != (byte)0)
                return;
            fmSlot.phase = 0U;
            fmSlot.ssgn = (byte)0;
            if (fmSlot.ar + (uint)fmSlot.ksr < 94U)
            {
                fmSlot.state = fmSlot.volume <= 0L ? (fmSlot.sl == 0U ? (byte)2 : (byte)3) : (byte)4;
            }
            else
            {
                fmSlot.volume = 0L;
                fmSlot.state = fmSlot.sl == 0U ? (byte)2 : (byte)3;
            }
            if (((int)fmSlot.ssg & 8) != 0 && ((int)fmSlot.ssgn ^ (int)fmSlot.ssg & 4) != 0)
                fmSlot.vol_out = ((uint)(512UL - (ulong)fmSlot.volume) & 1023U) + fmSlot.tl;
            else
                fmSlot.vol_out = (uint)fmSlot.volume + fmSlot.tl;
        }

        private void FM_KEYOFF_CSM(YM2612Core.FM_CH CH, int s)
        {
            YM2612Core.FM_SLOT fmSlot = CH.SLOT[s];
            if (fmSlot.key != (byte)0 || fmSlot.state <= (byte)1)
                return;
            fmSlot.state = (byte)1;
            if (((int)fmSlot.ssg & 8) <= 0)
                return;
            if (((int)fmSlot.ssgn ^ (int)fmSlot.ssg & 4) > 0)
                fmSlot.volume = 512L - fmSlot.volume;
            if (fmSlot.volume >= 512L)
            {
                fmSlot.volume = 1023L;
                fmSlot.state = (byte)0;
            }
            fmSlot.vol_out = (uint)fmSlot.volume + fmSlot.tl;
        }

        private void CSMKeyControll(YM2612Core.FM_CH CH)
        {
            this.FM_KEYON_CSM(CH, 0);
            this.FM_KEYON_CSM(CH, 2);
            this.FM_KEYON_CSM(CH, 1);
            this.FM_KEYON_CSM(CH, 3);
            this.ym2612.OPN.SL3.key_csm = (byte)1;
        }

        private void INTERNAL_TIMER_A()
        {
            if (((int)this.ym2612.OPN.ST.mode & 1) == 0 || (this.ym2612.OPN.ST.TAC -= this.ym2612.OPN.ST.TimerBase) > 0L)
                return;
            if (((int)this.ym2612.OPN.ST.mode & 4) != 0)
                this.ym2612.OPN.ST.status |= (byte)1;
            if (this.ym2612.OPN.ST.TAL != 0L)
                this.ym2612.OPN.ST.TAC += this.ym2612.OPN.ST.TAL;
            else
                this.ym2612.OPN.ST.TAC = this.ym2612.OPN.ST.TAL;
            if (((int)this.ym2612.OPN.ST.mode & 192) != 128)
                return;
            this.CSMKeyControll(this.ym2612.CH[2]);
        }

        private void INTERNAL_TIMER_B(int step)
        {
            if (((int)this.ym2612.OPN.ST.mode & 2) == 0 || (this.ym2612.OPN.ST.TBC -= this.ym2612.OPN.ST.TimerBase * (long)step) > 0L)
                return;
            if (((int)this.ym2612.OPN.ST.mode & 8) != 0)
                this.ym2612.OPN.ST.status |= (byte)2;
            if (this.ym2612.OPN.ST.TBL != 0L)
                this.ym2612.OPN.ST.TBC += this.ym2612.OPN.ST.TBL;
            else
                this.ym2612.OPN.ST.TBC = this.ym2612.OPN.ST.TBL;
        }

        private void set_timers(int v)
        {
            if ((((long)this.ym2612.OPN.ST.mode ^ (long)v) & 192L) != 0L)
            {
                this.ym2612.CH[2].SLOT[0].Incr = -1L;
                if ((v & 192) != 128 && this.ym2612.OPN.SL3.key_csm != (byte)0)
                {
                    this.FM_KEYOFF_CSM(this.ym2612.CH[2], 0);
                    this.FM_KEYOFF_CSM(this.ym2612.CH[2], 2);
                    this.FM_KEYOFF_CSM(this.ym2612.CH[2], 1);
                    this.FM_KEYOFF_CSM(this.ym2612.CH[2], 3);
                    this.ym2612.OPN.SL3.key_csm = (byte)0;
                }
            }
            if ((v & 1) != 0 && ((int)this.ym2612.OPN.ST.mode & 1) == 0)
                this.ym2612.OPN.ST.TAC = this.ym2612.OPN.ST.TAL;
            if ((v & 2) != 0 && ((int)this.ym2612.OPN.ST.mode & 2) == 0)
                this.ym2612.OPN.ST.TBC = this.ym2612.OPN.ST.TBL;
            this.ym2612.OPN.ST.status &= (byte)(~v >> 4);
            this.ym2612.OPN.ST.mode = (uint)v;
        }

        private void setup_connection(YM2612Core.FM_CH CH, int ch)
        {
            LongPointer longPointer = this.out_fm[ch];
            switch (CH.ALGO)
            {
                case 0:
                    CH.connect1 = this.c1;
                    CH.connect2 = this.mem;
                    CH.connect3 = this.c2;
                    CH.mem_connect = this.m2;
                    break;
                case 1:
                    CH.connect1 = this.mem;
                    CH.connect2 = this.mem;
                    CH.connect3 = this.c2;
                    CH.mem_connect = this.m2;
                    break;
                case 2:
                    CH.connect1 = this.c2;
                    CH.connect2 = this.mem;
                    CH.connect3 = this.c2;
                    CH.mem_connect = this.m2;
                    break;
                case 3:
                    CH.connect1 = this.c1;
                    CH.connect2 = this.mem;
                    CH.connect3 = this.c2;
                    CH.mem_connect = this.c2;
                    break;
                case 4:
                    CH.connect1 = this.c1;
                    CH.connect2 = longPointer;
                    CH.connect3 = this.c2;
                    CH.mem_connect = this.mem;
                    break;
                case 5:
                    CH.connect1 = (LongPointer)null;
                    CH.connect2 = longPointer;
                    CH.connect3 = longPointer;
                    CH.mem_connect = this.m2;
                    break;
                case 6:
                    CH.connect1 = this.c1;
                    CH.connect2 = longPointer;
                    CH.connect3 = longPointer;
                    CH.mem_connect = this.mem;
                    break;
                case 7:
                    CH.connect1 = longPointer;
                    CH.connect2 = longPointer;
                    CH.connect3 = longPointer;
                    CH.mem_connect = this.mem;
                    break;
            }
            CH.connect4 = longPointer;
        }

        private void set_det_mul(YM2612Core.FM_CH CH, YM2612Core.FM_SLOT SLOT, int v)
        {
            SLOT.mul = (v & 15) != 0 ? (uint)((v & 15) * 2) : 1U;
            SLOT.DT = this.ym2612.OPN.ST.dt_tab[v >> 4 & 7];
            CH.SLOT[0].Incr = -1L;
        }

        private void set_tl(YM2612Core.FM_SLOT SLOT, int v)
        {
            SLOT.tl = (uint)((v & (int)sbyte.MaxValue) << 3);
            if (((int)SLOT.ssg & 8) != 0 && ((int)SLOT.ssgn ^ (int)SLOT.ssg & 4) != 0 && SLOT.state > (byte)1)
                SLOT.vol_out = ((uint)(512UL - (ulong)SLOT.volume) & 1023U) + SLOT.tl;
            else
                SLOT.vol_out = (uint)SLOT.volume + SLOT.tl;
        }

        private void set_ar_ksr(YM2612Core.FM_CH CH, YM2612Core.FM_SLOT SLOT, int v)
        {
            byte ksr = SLOT.KSR;
            SLOT.ar = (v & 31) != 0 ? (uint)(32 + ((v & 31) << 1)) : 0U;
            SLOT.KSR = (byte)(3 - (v >> 6));
            if ((int)SLOT.KSR != (int)ksr)
                CH.SLOT[0].Incr = -1L;
            if (SLOT.ar + (uint)SLOT.ksr < 94U)
            {
                SLOT.eg_sh_ar = YM2612Core.eg_rate_shift[(int)SLOT.ar + (int)SLOT.ksr];
                SLOT.eg_sel_ar = YM2612Core.eg_rate_select[(int)SLOT.ar + (int)SLOT.ksr];
            }
            else
            {
                SLOT.eg_sh_ar = (byte)0;
                SLOT.eg_sel_ar = (byte)144;
            }
        }

        private void set_dr(YM2612Core.FM_SLOT SLOT, int v)
        {
            SLOT.d1r = (v & 31) != 0 ? (uint)(32 + ((v & 31) << 1)) : 0U;
            SLOT.eg_sh_d1r = YM2612Core.eg_rate_shift[(int)SLOT.d1r + (int)SLOT.ksr];
            SLOT.eg_sel_d1r = YM2612Core.eg_rate_select[(int)SLOT.d1r + (int)SLOT.ksr];
        }

        private void set_sr(YM2612Core.FM_SLOT SLOT, int v)
        {
            SLOT.d2r = (v & 31) != 0 ? (uint)(32 + ((v & 31) << 1)) : 0U;
            SLOT.eg_sh_d2r = YM2612Core.eg_rate_shift[(int)SLOT.d2r + (int)SLOT.ksr];
            SLOT.eg_sel_d2r = YM2612Core.eg_rate_select[(int)SLOT.d2r + (int)SLOT.ksr];
        }

        private void set_sl_rr(YM2612Core.FM_SLOT SLOT, int v)
        {
            SLOT.sl = YM2612Core.sl_table[v >> 4];
            if (SLOT.state == (byte)3 && SLOT.volume >= (long)(int)SLOT.sl)
                SLOT.state = (byte)2;
            SLOT.rr = (uint)(34 + ((v & 15) << 2));
            SLOT.eg_sh_rr = YM2612Core.eg_rate_shift[(int)SLOT.rr + (int)SLOT.ksr];
            SLOT.eg_sel_rr = YM2612Core.eg_rate_select[(int)SLOT.rr + (int)SLOT.ksr];
        }

        private void advance_lfo()
        {
            if (this.ym2612.OPN.lfo_timer_overflow == 0U)
                return;
            this.ym2612.OPN.lfo_timer += this.ym2612.OPN.lfo_timer_add;
            while (this.ym2612.OPN.lfo_timer >= this.ym2612.OPN.lfo_timer_overflow)
            {
                this.ym2612.OPN.lfo_timer -= this.ym2612.OPN.lfo_timer_overflow;
                this.ym2612.OPN.lfo_cnt = (byte)((int)this.ym2612.OPN.lfo_cnt + 1 & (int)sbyte.MaxValue);
                this.ym2612.OPN.LFO_AM = this.ym2612.OPN.lfo_cnt >= (byte)64 ? (uint)(126 - ((int)this.ym2612.OPN.lfo_cnt & 63) * 2) : (uint)this.ym2612.OPN.lfo_cnt * 2U;
                this.ym2612.OPN.LFO_PM = (uint)this.ym2612.OPN.lfo_cnt >> 2;
            }
        }

        private void advance_eg_channels()
        {
            uint egCnt = this.ym2612.OPN.eg_cnt;
            uint index1 = 0;
            do
            {
                int index2 = 0;
                YM2612Core.FM_SLOT fmSlot = this.ym2612.CH[(int)index1].SLOT[index2];
                uint num = 4;
                do
                {
                    switch (fmSlot.state)
                    {
                        case 1:
                            if (((long)egCnt & (long)((1 << (int)fmSlot.eg_sh_rr) - 1)) == 0L)
                            {
                                if (((int)fmSlot.ssg & 8) != 0)
                                {
                                    if (fmSlot.volume < 512L)
                                        fmSlot.volume += (long)(4 * (int)YM2612Core.eg_inc[(int)fmSlot.eg_sel_rr + ((int)(egCnt >> (int)fmSlot.eg_sh_rr) & 7)]);
                                    if (fmSlot.volume >= 512L)
                                    {
                                        fmSlot.volume = 1023L;
                                        fmSlot.state = (byte)0;
                                    }
                                }
                                else
                                {
                                    fmSlot.volume += (long)YM2612Core.eg_inc[(int)fmSlot.eg_sel_rr + ((int)(egCnt >> (int)fmSlot.eg_sh_rr) & 7)];
                                    if (fmSlot.volume >= 1023L)
                                    {
                                        fmSlot.volume = 1023L;
                                        fmSlot.state = (byte)0;
                                    }
                                }
                                fmSlot.vol_out = (uint)fmSlot.volume + fmSlot.tl;
                                break;
                            }
                            break;
                        case 2:
                            if (((long)egCnt & (long)((1 << (int)fmSlot.eg_sh_d2r) - 1)) == 0L)
                            {
                                if (((int)fmSlot.ssg & 8) != 0)
                                {
                                    if (fmSlot.volume < 512L)
                                    {
                                        fmSlot.volume += (long)(4 * (int)YM2612Core.eg_inc[(int)fmSlot.eg_sel_d2r + ((int)(egCnt >> (int)fmSlot.eg_sh_d2r) & 7)]);
                                        fmSlot.vol_out = ((int)fmSlot.ssgn ^ (int)fmSlot.ssg & 4) == 0 ? (uint)fmSlot.volume + fmSlot.tl : ((uint)(512UL - (ulong)fmSlot.volume) & 1023U) + fmSlot.tl;
                                        break;
                                    }
                                    break;
                                }
                                fmSlot.volume += (long)YM2612Core.eg_inc[(int)fmSlot.eg_sel_d2r + ((int)(egCnt >> (int)fmSlot.eg_sh_d2r) & 7)];
                                if (fmSlot.volume >= 1023L)
                                    fmSlot.volume = 1023L;
                                fmSlot.vol_out = (uint)fmSlot.volume + fmSlot.tl;
                                break;
                            }
                            break;
                        case 3:
                            if (((long)egCnt & (long)((1 << (int)fmSlot.eg_sh_d1r) - 1)) == 0L)
                            {
                                if (((int)fmSlot.ssg & 8) != 0)
                                {
                                    if (fmSlot.volume < 512L)
                                    {
                                        fmSlot.volume += (long)(4 * (int)YM2612Core.eg_inc[(int)fmSlot.eg_sel_d1r + ((int)(egCnt >> (int)fmSlot.eg_sh_d1r) & 7)]);
                                        fmSlot.vol_out = ((int)fmSlot.ssgn ^ (int)fmSlot.ssg & 4) == 0 ? (uint)fmSlot.volume + fmSlot.tl : ((uint)(512UL - (ulong)fmSlot.volume) & 1023U) + fmSlot.tl;
                                    }
                                }
                                else
                                {
                                    fmSlot.volume += (long)YM2612Core.eg_inc[(int)fmSlot.eg_sel_d1r + ((int)(egCnt >> (int)fmSlot.eg_sh_d1r) & 7)];
                                    fmSlot.vol_out = (uint)fmSlot.volume + fmSlot.tl;
                                }
                                if (fmSlot.volume >= (long)(int)fmSlot.sl)
                                {
                                    fmSlot.state = (byte)2;
                                    break;
                                }
                                break;
                            }
                            break;
                        case 4:
                            if (((long)egCnt & (long)((1 << (int)fmSlot.eg_sh_ar) - 1)) == 0L)
                            {
                                fmSlot.volume += ~fmSlot.volume * (long)YM2612Core.eg_inc[(int)fmSlot.eg_sel_ar + ((int)(egCnt >> (int)fmSlot.eg_sh_ar) & 7)] >> 4;
                                if (fmSlot.volume <= 0L)
                                {
                                    fmSlot.volume = 0L;
                                    fmSlot.state = fmSlot.sl == 0U ? (byte)2 : (byte)3;
                                }
                                fmSlot.vol_out = ((int)fmSlot.ssg & 8) == 0 || ((int)fmSlot.ssgn ^ (int)fmSlot.ssg & 4) == 0 ? (uint)fmSlot.volume + fmSlot.tl : ((uint)(512UL - (ulong)fmSlot.volume) & 1023U) + fmSlot.tl;
                                break;
                            }
                            break;
                    }
                    ++index2;
                    if (index2 < ((IEnumerable<YM2612Core.FM_SLOT>)this.ym2612.CH[(int)index1].SLOT).Count<YM2612Core.FM_SLOT>())
                        fmSlot = this.ym2612.CH[(int)index1].SLOT[index2];
                    --num;
                }
                while (num != 0U);
                ++index1;
            }
            while (index1 < 6U);
        }

        private void update_ssg_eg_channel(YM2612Core.FM_SLOT[] SLOTS)
        {
            uint num = 4;
            int index = 0;
            YM2612Core.FM_SLOT fmSlot = SLOTS[index];
            do
            {
                if (((int)fmSlot.ssg & 8) != 0 && fmSlot.volume >= 512L && fmSlot.state > (byte)1)
                {
                    if (((int)fmSlot.ssg & 1) != 0)
                    {
                        if (((int)fmSlot.ssg & 2) != 0)
                            fmSlot.ssgn = (byte)4;
                        if (fmSlot.state != (byte)4 && ((int)fmSlot.ssgn ^ (int)fmSlot.ssg & 4) == 0)
                            fmSlot.volume = 1023L;
                    }
                    else
                    {
                        if (((int)fmSlot.ssg & 2) != 0)
                            fmSlot.ssgn ^= (byte)4;
                        else
                            fmSlot.phase = 0U;
                        if (fmSlot.state != (byte)4)
                        {
                            if (fmSlot.ar + (uint)fmSlot.ksr < 94U)
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
                    fmSlot.vol_out = ((int)fmSlot.ssgn ^ (int)fmSlot.ssg & 4) == 0 ? (uint)fmSlot.volume + fmSlot.tl : ((uint)(512UL - (ulong)fmSlot.volume) & 1023U) + fmSlot.tl;
                }
                ++index;
                if (index < ((IEnumerable<YM2612Core.FM_SLOT>)SLOTS).Count<YM2612Core.FM_SLOT>())
                    fmSlot = SLOTS[index];
                --num;
            }
            while (num != 0U);
        }

        private void update_phase_lfo_slot(YM2612Core.FM_SLOT SLOT, long pms, uint block_fnum)
        {
            long num1 = this.lfo_pm_table[(long)((block_fnum & 2032U) >> 4 << 8) + pms + (long)this.ym2612.OPN.LFO_PM];
            if (num1 != 0L)
            {
                block_fnum = (uint)((ulong)(block_fnum * 2U) + (ulong)num1);
                byte num2 = (byte)((block_fnum & 28672U) >> 12);
                block_fnum &= 4095U;
                int index = (int)num2 << 2 | (int)YM2612Core.opn_fktable[(int)(block_fnum >> 8)];
                int num3 = (int)((long)(this.ym2612.OPN.fn_table[(int)block_fnum] >> 7 - (int)num2) + SLOT.DT.value[index]);
                if (num3 < 0)
                    num3 += (int)this.ym2612.OPN.fn_max;
                SLOT.phase += (uint)((long)num3 * (long)SLOT.mul >> 1);
            }
            else
                SLOT.phase += (uint)SLOT.Incr;
        }

        private void update_phase_lfo_channel(YM2612Core.FM_CH CH)
        {
            uint blockFnum = CH.block_fnum;
            long num1 = this.lfo_pm_table[(long)((blockFnum & 2032U) >> 4 << 8) + CH.pms + (long)this.ym2612.OPN.LFO_PM];
            if (num1 != 0L)
            {
                uint num2 = (uint)((ulong)(blockFnum * 2U) + (ulong)num1);
                byte num3 = (byte)((num2 & 28672U) >> 12);
                uint index1 = num2 & 4095U;
                int index2 = (int)num3 << 2 | (int)YM2612Core.opn_fktable[(int)(index1 >> 8)];
                int num4;
                int num5 = (int)((long)(num4 = (int)(this.ym2612.OPN.fn_table[(int)index1] >> 7 - (int)num3)) + CH.SLOT[0].DT.value[index2]);
                if (num5 < 0)
                    num5 += (int)this.ym2612.OPN.fn_max;
                CH.SLOT[0].phase += (uint)((long)num5 * (long)CH.SLOT[0].mul >> 1);
                int num6 = (int)((long)num4 + CH.SLOT[2].DT.value[index2]);
                if (num6 < 0)
                    num6 += (int)this.ym2612.OPN.fn_max;
                CH.SLOT[2].phase += (uint)((long)num6 * (long)CH.SLOT[2].mul >> 1);
                int num7 = (int)((long)num4 + CH.SLOT[1].DT.value[index2]);
                if (num7 < 0)
                    num7 += (int)this.ym2612.OPN.fn_max;
                CH.SLOT[1].phase += (uint)((long)num7 * (long)CH.SLOT[1].mul >> 1);
                int num8 = (int)((long)num4 + CH.SLOT[3].DT.value[index2]);
                if (num8 < 0)
                    num8 += (int)this.ym2612.OPN.fn_max;
                CH.SLOT[3].phase += (uint)((long)num8 * (long)CH.SLOT[3].mul >> 1);
            }
            else
            {
                CH.SLOT[0].phase += (uint)CH.SLOT[0].Incr;
                CH.SLOT[2].phase += (uint)CH.SLOT[2].Incr;
                CH.SLOT[1].phase += (uint)CH.SLOT[1].Incr;
                CH.SLOT[3].phase += (uint)CH.SLOT[3].Incr;
            }
        }

        private void refresh_fc_eg_slot(YM2612Core.FM_SLOT SLOT, int fc, int kc)
        {
            fc += (int)SLOT.DT.value[kc];
            if (fc < 0)
                fc += (int)this.ym2612.OPN.fn_max;
            SLOT.Incr = (long)fc * (long)SLOT.mul >> 1;
            kc >>= (int)SLOT.KSR;
            if ((int)SLOT.ksr == kc)
                return;
            SLOT.ksr = (byte)kc;
            if ((long)SLOT.ar + (long)kc < 94L)
            {
                SLOT.eg_sh_ar = YM2612Core.eg_rate_shift[(long)SLOT.ar + (long)kc];
                SLOT.eg_sel_ar = YM2612Core.eg_rate_select[(long)SLOT.ar + (long)kc];
            }
            else
            {
                SLOT.eg_sh_ar = (byte)0;
                SLOT.eg_sel_ar = (byte)144;
            }
            SLOT.eg_sh_d1r = YM2612Core.eg_rate_shift[(long)SLOT.d1r + (long)kc];
            SLOT.eg_sel_d1r = YM2612Core.eg_rate_select[(long)SLOT.d1r + (long)kc];
            SLOT.eg_sh_d2r = YM2612Core.eg_rate_shift[(long)SLOT.d2r + (long)kc];
            SLOT.eg_sel_d2r = YM2612Core.eg_rate_select[(long)SLOT.d2r + (long)kc];
            SLOT.eg_sh_rr = YM2612Core.eg_rate_shift[(long)SLOT.rr + (long)kc];
            SLOT.eg_sel_rr = YM2612Core.eg_rate_select[(long)SLOT.rr + (long)kc];
        }

        private void refresh_fc_eg_chan(YM2612Core.FM_CH CH)
        {
            if (CH.SLOT[0].Incr != -1L)
                return;
            int fc = (int)CH.fc;
            int kcode = (int)CH.kcode;
            this.refresh_fc_eg_slot(CH.SLOT[0], fc, kcode);
            this.refresh_fc_eg_slot(CH.SLOT[2], fc, kcode);
            this.refresh_fc_eg_slot(CH.SLOT[1], fc, kcode);
            this.refresh_fc_eg_slot(CH.SLOT[3], fc, kcode);
        }

        private uint volume_calc(YM2612Core.FM_SLOT slot, uint AM) => slot.vol_out + (AM & slot.AMmask);

        private int op_calc(uint phase, uint env, int pm)
        {
            uint index = (env << 3) + this.sin_tab[(int)(((long)phase & -65536L) + (long)(pm << 15)) >> 16 & 1023];
            return index >= 6656U ? 0 : this.tl_tab[(int)index];
        }

        private int op_calc1(uint phase, uint env, int pm)
        {
            uint index = (env << 3) + this.sin_tab[(int)(((long)phase & -65536L) + (long)pm) >> 16 & 1023];
            return index >= 6656U ? 0 : this.tl_tab[(int)index];
        }

        private void chan_calc(YM2612Core.FM_CH CH)
        {
            uint AM = this.ym2612.OPN.LFO_AM >> (int)CH.ams;
            uint env1 = this.volume_calc(CH.SLOT[0], AM);
            this.m2.value = this.c1.value = this.c2.value = this.mem.value = 0L;
            CH.mem_connect.value = CH.mem_value;
            long num = CH.op1_out[0] + CH.op1_out[1];
            CH.op1_out[0] = CH.op1_out[1];
            if (CH.connect1 == null)
                this.mem.value = this.c1.value = this.c2.value = CH.op1_out[0];
            else
                CH.connect1.value += CH.op1_out[0];
            CH.op1_out[1] = 0L;
            if (env1 < 832U)
            {
                if (CH.FB == (byte)0)
                    num = 0L;
                CH.op1_out[1] = (long)this.op_calc1(CH.SLOT[0].phase, env1, (int)(num << (int)CH.FB));
            }
            uint env2 = this.volume_calc(CH.SLOT[1], AM);
            if (env2 < 832U)
                CH.connect3.value += (long)this.op_calc(CH.SLOT[1].phase, env2, (int)this.m2.value);
            uint env3 = this.volume_calc(CH.SLOT[2], AM);
            if (env3 < 832U)
                CH.connect2.value += (long)this.op_calc(CH.SLOT[2].phase, env3, (int)this.c1.value);
            uint env4 = this.volume_calc(CH.SLOT[3], AM);
            if (env4 < 832U)
                CH.connect4.value += (long)this.op_calc(CH.SLOT[3].phase, env4, (int)this.c2.value);
            CH.mem_value = this.mem.value;
            if (CH.pms != 0L)
            {
                if ((this.ym2612.OPN.ST.mode & 192U) > 0U && CH == this.ym2612.CH[2])
                {
                    this.update_phase_lfo_slot(CH.SLOT[0], CH.pms, this.ym2612.OPN.SL3.block_fnum[1]);
                    this.update_phase_lfo_slot(CH.SLOT[2], CH.pms, this.ym2612.OPN.SL3.block_fnum[2]);
                    this.update_phase_lfo_slot(CH.SLOT[1], CH.pms, this.ym2612.OPN.SL3.block_fnum[0]);
                    this.update_phase_lfo_slot(CH.SLOT[3], CH.pms, CH.block_fnum);
                }
                else
                    this.update_phase_lfo_channel(CH);
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
                        if (this.ym2612.OPN.lfo_timer_overflow == 0U)
                        {
                            this.ym2612.OPN.lfo_cnt = (byte)0;
                            this.ym2612.OPN.lfo_timer = 0U;
                            this.ym2612.OPN.LFO_AM = 0U;
                            this.ym2612.OPN.LFO_PM = 0U;
                        }
                        this.ym2612.OPN.lfo_timer_overflow = YM2612Core.lfo_samples_per_step[v & 7] << 24;
                        break;
                    }
                    this.ym2612.OPN.lfo_timer_overflow = 0U;
                    break;
                case 36:
                    this.ym2612.OPN.ST.TA = this.ym2612.OPN.ST.TA & 3L | (long)v << 2;
                    this.ym2612.OPN.ST.TAL = 1024L - this.ym2612.OPN.ST.TA << 16;
                    break;
                case 37:
                    this.ym2612.OPN.ST.TA = this.ym2612.OPN.ST.TA & 1020L | (long)v & 3L;
                    this.ym2612.OPN.ST.TAL = 1024L - this.ym2612.OPN.ST.TA << 16;
                    break;
                case 38:
                    this.ym2612.OPN.ST.TB = (long)v;
                    this.ym2612.OPN.ST.TBL = 256L - this.ym2612.OPN.ST.TB << 20;
                    break;
                case 39:
                    this.set_timers(v);
                    break;
                case 40:
                    byte index = (byte)(v & 3);
                    if (index == (byte)3)
                        break;
                    if ((v & 4) != 0)
                        index += (byte)3;
                    YM2612Core.FM_CH CH = this.ym2612.CH[(int)index];
                    if ((v & 16) != 0)
                        this.FM_KEYON(CH, 0);
                    else
                        this.FM_KEYOFF(CH, 0);
                    if ((v & 32) != 0)
                        this.FM_KEYON(CH, 2);
                    else
                        this.FM_KEYOFF(CH, 2);
                    if ((v & 64) != 0)
                        this.FM_KEYON(CH, 1);
                    else
                        this.FM_KEYOFF(CH, 1);
                    if ((v & 128) != 0)
                    {
                        this.FM_KEYON(CH, 3);
                        break;
                    }
                    this.FM_KEYOFF(CH, 3);
                    break;
            }
        }

        private byte OPN_CHAN(int N) => (byte)(N & 3);

        private int OPN_SLOT(int N) => N >> 2 & 3;

        private void OPNWriteReg(int r, int v)
        {
            byte ch = this.OPN_CHAN(r);
            if (ch == (byte)3)
                return;
            if (r >= 256)
                ch += (byte)3;
            YM2612Core.FM_CH CH = this.ym2612.CH[(int)ch];
            YM2612Core.FM_SLOT SLOT = CH.SLOT[this.OPN_SLOT(r)];
            switch (r & 240)
            {
                case 48:
                    this.set_det_mul(CH, SLOT, v);
                    break;
                case 64:
                    this.set_tl(SLOT, v);
                    break;
                case 80:
                    this.set_ar_ksr(CH, SLOT, v);
                    break;
                case 96:
                    this.set_dr(SLOT, v);
                    SLOT.AMmask = (v & 128) != 0 ? uint.MaxValue : 0U;
                    break;
                case 112:
                    this.set_sr(SLOT, v);
                    break;
                case 128:
                    this.set_sl_rr(SLOT, v);
                    break;
                case 144:
                    SLOT.ssg = (byte)(v & 15);
                    if (SLOT.state <= (byte)1)
                        break;
                    if (((int)SLOT.ssg & 8) != 0 && ((int)SLOT.ssgn ^ (int)SLOT.ssg & 4) != 0)
                    {
                        SLOT.vol_out = ((uint)(512UL - (ulong)SLOT.volume) & 1023U) + SLOT.tl;
                        break;
                    }
                    SLOT.vol_out = (uint)SLOT.volume + SLOT.tl;
                    break;
                case 160:
                    switch (this.OPN_SLOT(r))
                    {
                        case 0:
                            uint num1 = (uint)((ulong)(uint)(((int)this.ym2612.OPN.ST.fn_h & 7) << 8) + (ulong)v);
                            byte num2 = (byte)((uint)this.ym2612.OPN.ST.fn_h >> 3);
                            CH.kcode = (byte)((uint)num2 << 2 | (uint)YM2612Core.opn_fktable[(int)(num1 >> 7)]);
                            CH.fc = this.ym2612.OPN.fn_table[(int)num1 * 2] >> 7 - (int)num2;
                            CH.block_fnum = (uint)num2 << 11 | num1;
                            CH.SLOT[0].Incr = -1L;
                            return;
                        case 1:
                            this.ym2612.OPN.ST.fn_h = (byte)(v & 63);
                            return;
                        case 2:
                            if (r >= 256)
                                return;
                            uint num3 = (uint)((ulong)(uint)(((int)this.ym2612.OPN.SL3.fn_h & 7) << 8) + (ulong)v);
                            byte num4 = (byte)((uint)this.ym2612.OPN.SL3.fn_h >> 3);
                            this.ym2612.OPN.SL3.kcode[(int)ch] = (byte)((uint)num4 << 2 | (uint)YM2612Core.opn_fktable[(int)(num3 >> 7)]);
                            this.ym2612.OPN.SL3.fc[(int)ch] = this.ym2612.OPN.fn_table[(int)num3 * 2] >> 7 - (int)num4;
                            this.ym2612.OPN.SL3.block_fnum[(int)ch] = (uint)num4 << 11 | num3;
                            this.ym2612.CH[2].SLOT[0].Incr = -1L;
                            return;
                        case 3:
                            if (r >= 256)
                                return;
                            this.ym2612.OPN.SL3.fn_h = (byte)(v & 63);
                            return;
                        default:
                            return;
                    }
                case 176:
                    switch (this.OPN_SLOT(r))
                    {
                        case 0:
                            int num5 = v >> 3 & 7;
                            CH.ALGO = (byte)(v & 7);
                            CH.FB = num5 != 0 ? (byte)(num5 + 6) : (byte)0;
                            this.setup_connection(CH, (int)ch);
                            return;
                        case 1:
                            CH.pms = (long)((v & 7) * 32);
                            CH.ams = YM2612Core.lfo_ams_depth_shift[v >> 4 & 3];
                            this.ym2612.OPN.pan[(int)ch * 2] = (v & 128) != 0 ? uint.MaxValue : 0U;
                            this.ym2612.OPN.pan[(int)ch * 2 + 1] = (v & 64) != 0 ? uint.MaxValue : 0U;
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
                    double num = (double)YM2612Core.dt_tab[index1 * 32 + index2] * freqbase * 64.0;
                    this.ym2612.OPN.ST.dt_tab[index1].value[index2] = (long)(int)num;
                    this.ym2612.OPN.ST.dt_tab[index1 + 4].value[index2] = -this.ym2612.OPN.ST.dt_tab[index1].value[index2];
                }
            }
            for (int index = 0; index < 4096; ++index)
                this.ym2612.OPN.fn_table[index] = (uint)((double)index * 32.0 * freqbase * 64.0);
            this.ym2612.OPN.fn_max = (uint)(131072.0 * freqbase * 64.0);
        }

        private void OPNSetPres(int pres)
        {
            double freqbase = this.ym2612.OPN.ST.clock / (double)this.ym2612.OPN.ST.rate / (double)pres;
            if (this.config.hq_fm != (byte)0)
                freqbase = 1.0;
            this.ym2612.OPN.eg_timer_add = (uint)(65536.0 * freqbase);
            this.ym2612.OPN.eg_timer_overflow = 196608U;
            this.ym2612.OPN.lfo_timer_add = (uint)(16777216.0 * freqbase);
            this.ym2612.OPN.ST.TimerBase = (long)(int)(65536.0 * freqbase);
            this.init_timetables(freqbase);
        }

        private void reset_channels(YM2612Core.FM_CH[] CH, int num)
        {
            for (int index1 = 0; index1 < num; ++index1)
            {
                CH[index1].mem_value = 0L;
                CH[index1].op1_out[0] = 0L;
                CH[index1].op1_out[1] = 0L;
                for (int index2 = 0; index2 < 4; ++index2)
                {
                    CH[index1].SLOT[index2].Incr = -1L;
                    CH[index1].SLOT[index2].key = (byte)0;
                    CH[index1].SLOT[index2].phase = 0U;
                    CH[index1].SLOT[index2].ssgn = (byte)0;
                    CH[index1].SLOT[index2].state = (byte)0;
                    CH[index1].SLOT[index2].volume = 1023L;
                    CH[index1].SLOT[index2].vol_out = 1023U;
                }
            }
        }

        private void init_tables()
        {
            uint num1 = (uint)~((1 << 14 - (int)this.config.dac_bits) - 1);
            for (int index1 = 0; index1 < 256; ++index1)
            {
                int num2 = (int)Math.Floor(65536.0 / Math.Pow(2.0, (double)(index1 + 1) * (1.0 / 32.0) / 8.0)) >> 4;
                int num3 = ((num2 & 1) == 0 ? num2 >> 1 : (num2 >> 1) + 1) << 2;
                this.tl_tab[index1 * 2] = (int)((long)num3 & (long)num1);
                this.tl_tab[index1 * 2 + 1] = (int)((long)-this.tl_tab[index1 * 2] & (long)num1);
                for (int index2 = 1; index2 < 13; ++index2)
                {
                    this.tl_tab[index1 * 2 + index2 * 2 * 256] = (int)((long)(this.tl_tab[index1 * 2] >> index2) & (long)num1);
                    this.tl_tab[index1 * 2 + 1 + index2 * 2 * 256] = (int)((long)-this.tl_tab[index1 * 2 + index2 * 2 * 256] & (long)num1);
                }
            }
            for (int index = 0; index < 1024; ++index)
            {
                double num4 = Math.Sin((double)(index * 2 + 1) * Math.PI / 1024.0);
                int num5 = (int)(2.0 * ((num4 <= 0.0 ? 8.0 * Math.Log(-1.0 / num4) / Math.Log(2.0) : 8.0 * Math.Log(1.0 / num4) / Math.Log(2.0)) / (1.0 / 32.0)));
                int num6 = (num5 & 1) == 0 ? num5 >> 1 : (num5 >> 1) + 1;
                this.sin_tab[index] = (uint)(num6 * 2 + (num4 >= 0.0 ? 0 : 1));
            }
            for (int index3 = 0; index3 < 8; ++index3)
            {
                for (byte index4 = 0; index4 < (byte)128; ++index4)
                {
                    uint num7 = (uint)index3;
                    for (byte index5 = 0; index5 < (byte)8; ++index5)
                    {
                        byte num8 = 0;
                        for (uint index6 = 0; index6 < 7U; ++index6)
                        {
                            if (((int)index4 & 1 << (int)index6) != 0)
                            {
                                uint num9 = index6 * 8U;
                                num8 += YM2612Core.lfo_pm_output[(int)num9 + (int)num7, (int)index5];
                            }
                        }
                        this.lfo_pm_table[(int)index4 * 32 * 8 + index3 * 32 + (int)index5] = (long)num8;
                        this.lfo_pm_table[(int)index4 * 32 * 8 + index3 * 32 + ((int)index5 ^ 7) + 8] = (long)num8;
                        this.lfo_pm_table[(int)index4 * 32 * 8 + index3 * 32 + (int)index5 + 16] = (long)-num8;
                        this.lfo_pm_table[(int)index4 * 32 * 8 + index3 * 32 + ((int)index5 ^ 7) + 24] = (long)-num8;
                    }
                }
            }
        }

        public void YM2612Init(double clock, int rate)
        {
            this.config.psg_preamp = (short)150;
            this.config.fm_preamp = (short)100;
            this.config.hq_fm = (byte)0;
            this.config.psgBoostNoise = (byte)0;
            this.config.filter = (byte)0;
            this.config.lp_range = (short)50;
            this.config.low_freq = (short)880;
            this.config.high_freq = (short)5000;
            this.config.lg = (short)1;
            this.config.mg = (short)1;
            this.config.hg = (short)1;
            this.config.rolloff = 0.99f;
            this.config.dac_bits = (byte)14;
            this.config.ym2413 = (byte)2;
            this.config.system = (byte)0;
            this.config.region_detect = (byte)0;
            this.config.vdp_mode = (byte)0;
            this.config.master_clock = (byte)0;
            this.config.force_dtack = (byte)0;
            this.config.addr_error = (byte)1;
            this.config.bios = (byte)0;
            this.config.lock_on = (byte)0;
            this.config.hot_swap = (byte)0;
            this.config.xshift = (short)0;
            this.config.yshift = (short)0;
            this.config.xscale = (short)0;
            this.config.yscale = (short)0;
            this.config.aspect = (byte)1;
            this.config.overscan = (byte)3;
            this.config.ntsc = (byte)0;
            this.config.vsync = (byte)1;
            this.config.render = (byte)0;
            this.config.bilinear = (byte)0;
            this.config.tv_mode = (byte)1;
            this.config.gun_cursor[0] = (byte)1;
            this.config.gun_cursor[1] = (byte)1;
            this.config.invert_mouse = (byte)0;
            this.config.autoload = (byte)0;
            this.config.autocheat = (byte)0;
            this.config.s_auto = (byte)0;
            this.config.s_default = (byte)1;
            this.config.s_device = (byte)0;
            this.config.l_device = (byte)0;
            this.config.bg_overlay = (byte)0;
            this.config.screen_w = (short)658;
            this.config.bgm_volume = 100f;
            this.config.sfx_volume = 100f;
            this.config.hot_swap &= (byte)1;
            this.init_tables();
            this.ym2612.OPN.ST.clock = clock;
            this.ym2612.OPN.ST.rate = (uint)rate;
            this.OPNSetPres(144);
        }

        public void YM2612ResetChip()
        {
            this.ym2612.OPN.eg_timer = 0U;
            this.ym2612.OPN.eg_cnt = 0U;
            this.ym2612.OPN.lfo_timer_overflow = 0U;
            this.ym2612.OPN.lfo_timer = 0U;
            this.ym2612.OPN.lfo_cnt = (byte)0;
            this.ym2612.OPN.LFO_AM = 0U;
            this.ym2612.OPN.LFO_PM = 0U;
            this.ym2612.OPN.ST.TAC = 0L;
            this.ym2612.OPN.ST.TBC = 0L;
            this.ym2612.OPN.SL3.key_csm = (byte)0;
            this.ym2612.dacen = (byte)0;
            this.ym2612.dacout = 0L;
            this.set_timers(48);
            this.ym2612.OPN.ST.TB = 0L;
            this.ym2612.OPN.ST.TBL = 268435456L;
            this.ym2612.OPN.ST.TA = 0L;
            this.ym2612.OPN.ST.TAL = 67108864L;
            this.reset_channels(this.ym2612.CH, 6);
            for (int r = 182; r >= 180; --r)
            {
                this.OPNWriteReg(r, 192);
                this.OPNWriteReg(r | 256, 192);
            }
            for (int r = 178; r >= 48; --r)
            {
                this.OPNWriteReg(r, 0);
                this.OPNWriteReg(r | 256, 0);
            }
        }

        public void YM2612Write(uint a, uint v)
        {
            v &= (uint)byte.MaxValue;
            if (a != 0U)
            {
                if (a == 2U)
                {
                    this.ym2612.OPN.ST.address = (ushort)(v | 256U);
                }
                else
                {
                    int address = (int)this.ym2612.OPN.ST.address;
                    if ((address & 496) == 32)
                    {
                        if (address != 42)
                        {
                            if (address == 43)
                                this.ym2612.dacen = (byte)(v & 128U);
                            else
                                this.OPNWriteMode(address, (int)v);
                        }
                        else
                            this.ym2612.dacout = (long)((int)v - 128 << 6);
                    }
                    else
                        this.OPNWriteReg(address, (int)v);
                }
            }
            else
                this.ym2612.OPN.ST.address = (ushort)v;
        }

        public uint YM2612Read() => (uint)this.ym2612.OPN.ST.status & (uint)byte.MaxValue;

        public void YM2612Update(int[] buffer, int length)
        {
            this.refresh_fc_eg_chan(this.ym2612.CH[0]);
            this.refresh_fc_eg_chan(this.ym2612.CH[1]);
            if (((int)this.ym2612.OPN.ST.mode & 192) == 0)
                this.refresh_fc_eg_chan(this.ym2612.CH[2]);
            else if (this.ym2612.CH[2].SLOT[0].Incr == -1L)
            {
                this.refresh_fc_eg_slot(this.ym2612.CH[2].SLOT[0], (int)this.ym2612.OPN.SL3.fc[1], (int)this.ym2612.OPN.SL3.kcode[1]);
                this.refresh_fc_eg_slot(this.ym2612.CH[2].SLOT[2], (int)this.ym2612.OPN.SL3.fc[2], (int)this.ym2612.OPN.SL3.kcode[2]);
                this.refresh_fc_eg_slot(this.ym2612.CH[2].SLOT[1], (int)this.ym2612.OPN.SL3.fc[0], (int)this.ym2612.OPN.SL3.kcode[0]);
                this.refresh_fc_eg_slot(this.ym2612.CH[2].SLOT[3], (int)this.ym2612.CH[2].fc, (int)this.ym2612.CH[2].kcode);
            }
            this.refresh_fc_eg_chan(this.ym2612.CH[3]);
            this.refresh_fc_eg_chan(this.ym2612.CH[4]);
            this.refresh_fc_eg_chan(this.ym2612.CH[5]);
            int index1 = 0;
            for (int index2 = 0; index2 < length; ++index2)
            {
                this.out_fm[0].value = 0L;
                this.out_fm[1].value = 0L;
                this.out_fm[2].value = 0L;
                this.out_fm[3].value = 0L;
                this.out_fm[4].value = 0L;
                this.out_fm[5].value = 0L;
                this.update_ssg_eg_channel(this.ym2612.CH[0].SLOT);
                this.update_ssg_eg_channel(this.ym2612.CH[1].SLOT);
                this.update_ssg_eg_channel(this.ym2612.CH[2].SLOT);
                this.update_ssg_eg_channel(this.ym2612.CH[3].SLOT);
                this.update_ssg_eg_channel(this.ym2612.CH[4].SLOT);
                this.update_ssg_eg_channel(this.ym2612.CH[5].SLOT);
                this.chan_calc(this.ym2612.CH[0]);
                this.chan_calc(this.ym2612.CH[1]);
                this.chan_calc(this.ym2612.CH[2]);
                this.chan_calc(this.ym2612.CH[3]);
                this.chan_calc(this.ym2612.CH[4]);
                if (this.ym2612.dacen == (byte)0)
                    this.chan_calc(this.ym2612.CH[5]);
                else
                    this.out_fm[5].value = this.ym2612.dacout;
                this.advance_lfo();
                this.ym2612.OPN.eg_timer += this.ym2612.OPN.eg_timer_add;
                while (this.ym2612.OPN.eg_timer >= this.ym2612.OPN.eg_timer_overflow)
                {
                    this.ym2612.OPN.eg_timer -= this.ym2612.OPN.eg_timer_overflow;
                    ++this.ym2612.OPN.eg_cnt;
                    this.advance_eg_channels();
                }
                if (this.out_fm[0].value > 8192L)
                    this.out_fm[0].value = 8192L;
                else if (this.out_fm[0].value < -8192L)
                    this.out_fm[0].value = -8192L;
                if (this.out_fm[1].value > 8192L)
                    this.out_fm[1].value = 8192L;
                else if (this.out_fm[1].value < -8192L)
                    this.out_fm[1].value = -8192L;
                if (this.out_fm[2].value > 8192L)
                    this.out_fm[2].value = 8192L;
                else if (this.out_fm[2].value < -8192L)
                    this.out_fm[2].value = -8192L;
                if (this.out_fm[3].value > 8192L)
                    this.out_fm[3].value = 8192L;
                else if (this.out_fm[3].value < -8192L)
                    this.out_fm[3].value = -8192L;
                if (this.out_fm[4].value > 8192L)
                    this.out_fm[4].value = 8192L;
                else if (this.out_fm[4].value < -8192L)
                    this.out_fm[4].value = -8192L;
                if (this.out_fm[5].value > 8192L)
                    this.out_fm[5].value = 8192L;
                else if (this.out_fm[5].value < -8192L)
                    this.out_fm[5].value = -8192L;
                int num1 = (int)(this.out_fm[0].value & (long)this.ym2612.OPN.pan[0]);
                int num2 = (int)(this.out_fm[0].value & (long)this.ym2612.OPN.pan[1]);
                int num3 = num1 + (int)(this.out_fm[1].value & (long)this.ym2612.OPN.pan[2]);
                int num4 = num2 + (int)(this.out_fm[1].value & (long)this.ym2612.OPN.pan[3]);
                int num5 = num3 + (int)(this.out_fm[2].value & (long)this.ym2612.OPN.pan[4]);
                int num6 = num4 + (int)(this.out_fm[2].value & (long)this.ym2612.OPN.pan[5]);
                int num7 = num5 + (int)(this.out_fm[3].value & (long)this.ym2612.OPN.pan[6]);
                int num8 = num6 + (int)(this.out_fm[3].value & (long)this.ym2612.OPN.pan[7]);
                int num9 = num7 + (int)(this.out_fm[4].value & (long)this.ym2612.OPN.pan[8]);
                int num10 = num8 + (int)(this.out_fm[4].value & (long)this.ym2612.OPN.pan[9]);
                int num11 = num9 + (int)(this.out_fm[5].value & (long)this.ym2612.OPN.pan[10]);
                int num12 = num10 + (int)(this.out_fm[5].value & (long)this.ym2612.OPN.pan[11]);
                buffer[index1] = num11;
                int index3 = index1 + 1;
                buffer[index3] = num12;
                index1 = index3 + 1;
                this.ym2612.OPN.SL3.key_csm <<= 1;
                this.INTERNAL_TIMER_A();
                if (((int)this.ym2612.OPN.SL3.key_csm & 2) != 0)
                {
                    this.FM_KEYOFF_CSM(this.ym2612.CH[2], 0);
                    this.FM_KEYOFF_CSM(this.ym2612.CH[2], 2);
                    this.FM_KEYOFF_CSM(this.ym2612.CH[2], 1);
                    this.FM_KEYOFF_CSM(this.ym2612.CH[2], 3);
                    this.ym2612.OPN.SL3.key_csm = (byte)0;
                }
            }
            this.INTERNAL_TIMER_B(length);
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
            public YM2612Core.FM_SLOT[] SLOT = new YM2612Core.FM_SLOT[4];
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
                this.SLOT[0] = new YM2612Core.FM_SLOT();
                this.SLOT[1] = new YM2612Core.FM_SLOT();
                this.SLOT[2] = new YM2612Core.FM_SLOT();
                this.SLOT[3] = new YM2612Core.FM_SLOT();
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
                    this.dt_tab[index] = new LongPointerArray32();
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
            public YM2612Core.FM_ST ST = new YM2612Core.FM_ST();
            public YM2612Core.FM_3SLOT SL3 = new YM2612Core.FM_3SLOT();
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
            public YM2612Core.FM_CH[] CH = new YM2612Core.FM_CH[6];
            public byte dacen;
            public long dacout;
            public YM2612Core.FM_OPN OPN = new YM2612Core.FM_OPN();

            public _YM2612_data()
            {
                for (int index = 0; index < 6; ++index)
                    this.CH[index] = new YM2612Core.FM_CH();
            }
        }
    }
}
