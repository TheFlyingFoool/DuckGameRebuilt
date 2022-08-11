using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DuckGame;

public static class FireSerializer
{
    public static string Serialize(object? obj)
    {
        /* Syntax Disclaimer ------------------------------- |
         * ------------------------------------------------- |
         * Using the syntax `if (OBJ is T VARIABLE)` will    |
         * check if OBJ is of type T, if yes, it returns     |
         * true and declares a variable VARIABLE of type T.  |
         * Otherwise it will return false, and VARIABLE will |
         * be unusable because of compiler errors.           |
         * ------------------------------------------------ */

        if (obj is null)
            return "null";

        if (obj is BitBuffer buffer)
            return BitConverter.ToString(buffer.buffer);

        Type type = obj.GetType();

        if (obj is string str)
            return str;

        if (type.IsPrimitive)
            return obj.ToString();

        if (obj is Vec2 vec2)
            return $"{vec2.x},{vec2.y}";

        if (obj is Enum @enum)
            return @enum.ToString();

        // Same as above but deconstructs the Color into r, g, b
        if (obj is Color (var r, var g, var b))
            return $"{r},{g},{b}";

        if (obj is DateTime dateTime)
            return new DateTimeOffset(dateTime).ToUnixTimeSeconds().ToString();

        while (type.InheritsFrom(typeof(IEnumerable)))
        {
            IEnumerable<object> collection = ((IEnumerable) obj).Cast<object>();

            if (!type.IsArray && type.GetGenericArguments().Length != 1)
                break;

            if (!collection.Any())
                return "[]";

            return $"[{string.Join(";", collection.Select(Serialize))}]";
        }

        throw new Exception($"unsupported conversion type: {type}");
    }

    public static object Deserialize(Type type, string str)
    {
        if (str == "null")
            return null;

        if (type == typeof(string))
            return str;

        if (type == typeof(BitBuffer))
            return new BitBuffer(str.Select(b => Convert.ToByte(b)).ToArray());

        if (type.IsPrimitive)
            return Convert.ChangeType(str, type);

        if (type == typeof(Vec2))
            return Vec2.Parse(str);

        if (type.InheritsFrom(typeof(Enum)))
            return Enum.Parse(type, str, true);

        if (type == typeof(Color))
        {
            byte[] split = str.Split(',').Select(byte.Parse).ToArray();
            return new Color(split[0], split[1], split[2]);
        }

        if (type.InheritsFrom(typeof(DateTime)))
            return DateTimeOffset.FromUnixTimeSeconds(long.Parse(str)).DateTime;

        while (type.InheritsFrom(typeof(IEnumerable)))
        {
            var genericArguments = type.GetGenericArguments();
            int genericArgumentLength = genericArguments.Length;
            bool isArray = type.IsArray;
            if (!isArray && genericArgumentLength != 1)
                break;

            Type argType = isArray
                ? type.GetElementType()
                : genericArguments[0];

            if (str.Length >= 2 && (str.First() != '[' || str.Last() != ']'))
                throw new Exception($"Error while parsing type: {type}");

            str = str.Substring(1, str.Length - 2);

            string[] split = str.Split(';');
            var usableCollection = split.Select(x => Deserialize(argType, x));

            var arr = Array.CreateInstance(argType, split.Length);
            for (int i = 0; i < split.Length; i++)
            {
                arr.SetValue(usableCollection.ElementAt(i), i);
            }

            if (isArray)
                return arr;

            if (type.Name == "List`1")
            {
                var listType = typeof(List<>).MakeGenericType(argType!);
                var instance = Activator.CreateInstance(listType);

                var list = (IList) instance;

                for (int i = 0; i < arr.Length; i++)
                {
                    list.Add(arr.GetValue(i));
                }

                return instance;
            }
        }

        throw new Exception($"unsupported conversion type: {type}");
    }

    public static T Deserialize<T>(string str)
    {
        return (T) Deserialize(typeof(T), str);
    }

    public static bool IsSerializable(Type type)
    {
        return type.IsPrimitive
               || type == typeof(Vec2)
               || type.InheritsFrom(typeof(Enum))
               || type == typeof(Color)
               || type.InheritsFrom(typeof(DateTime))
               || type.InheritsFrom(typeof(IEnumerable));
    }
}