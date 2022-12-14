// Decompiled with JetBrains decompiler
// Type: DuckGame.DevNumber
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class DevNumber
    {
        private static float[] _numbers = new float[8];

        public static float one => _numbers[0];

        public static float two => _numbers[1];

        public static float three => _numbers[2];

        public static float four => _numbers[3];

        public static float five => _numbers[4];

        public static float six => _numbers[5];

        public static float seven => _numbers[6];

        public static float eight => _numbers[7];

        public static void Initialize()
        {
        }

        public static void Update()
        {
        }
    }
}
