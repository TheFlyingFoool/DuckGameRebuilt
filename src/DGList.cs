// Decompiled with JetBrains decompiler
// Type: DuckGame.DGList
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections.Generic;

namespace DuckGame
{
    public class DGList
    {
        public static void Sort<T>(List<T> list, Comparison<T> comparison) => list.Sort(comparison);
    }
}
