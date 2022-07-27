// Decompiled with JetBrains decompiler
// Type: DuckGame.Change
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

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
