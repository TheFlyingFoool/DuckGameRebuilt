using Steamworks;
using System;
using System.Collections.Generic;

// Common helper class that turns out to be used ~ 2 times per function...
public static class SteamHelper {

    public static List<T> GetList<T>(int count, Func<int, T> get) {
        if (count <= 0)
            return new List<T>();
        List<T> list = new List<T>(count);
        for (int i = 0; i < count; i++)
            list.Add(get(i));
        return list;
    }

    public static TOut[] GetArray<TIn, TOut>(IList<TIn> list, Func<TIn, TOut> get) {
        if (list.Count <= 0)
            return new TOut[0];
        TOut[] array = new TOut[list.Count];
        for (int i = 0; i < array.Length; i++)
            array[i] = get(list[i]);
        return array;
    }

    public static byte[] GetImageRGBA(int id) {
        uint w;
        uint h;
        if (!SteamUtils.GetImageSize(id, out w, out h))
            return null;
        byte[] data = new byte[w * h * 4];
        if (!SteamUtils.GetImageRGBA(id, data, data.Length))
            return null;
        return data;
    }
}
