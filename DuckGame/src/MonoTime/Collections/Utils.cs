using System;
using System.Collections.Generic;

namespace DuckGame
{
    internal static class Utils
    {
        public static List<T> Shuffle<T>(List<T> list)
        {
            List<T> objList = new List<T>(list);
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
