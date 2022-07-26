// Decompiled with JetBrains decompiler
// Type: DuckGame.Utils
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections.Generic;

namespace DuckGame
{
    internal class Utils
    {
        public static List<T> Shuffle<T>(List<T> list)
        {
            List<T> objList = new List<T>((IEnumerable<T>)list);
            for (int index1 = 0; index1 < objList.Count - 1; ++index1)
            {
                int index2 = index1 + Rando.Int(objList.Count - 1 - index1);
                T obj = objList[index2];
                objList[index2] = objList[index1];
                objList[index1] = obj;
            }
            return objList;
        }

        public static T FirstOrDefault<T>(List<T> list, Predicate<T> condition)
        {
            foreach (T obj in list)
            {
                if (condition(obj))
                    return obj;
            }
            return default(T);
        }
    }
}
