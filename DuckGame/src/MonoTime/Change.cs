using System;
using System.Globalization;

namespace DuckGame
{
    public class Change
    {
        public static float ToSingle(object value) => Convert.ToSingle(value, CultureInfo.InvariantCulture);

        public static ulong ToUInt64(object value) => Convert.ToUInt64(value, CultureInfo.InvariantCulture);

        public static int ToInt32(object value) => Convert.ToInt32(value, CultureInfo.InvariantCulture);

        public static bool ToBoolean(object value) => Convert.ToBoolean(value, CultureInfo.InvariantCulture);

        public static string ToString(object value) => Convert.ToString(value, CultureInfo.InvariantCulture);
    }
}
