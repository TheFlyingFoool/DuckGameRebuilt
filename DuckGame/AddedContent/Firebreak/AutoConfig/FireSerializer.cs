using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
            return string.Empty;

        Type type = obj.GetType();
        
        if (type.IsPrimitive)
            return obj.ToString();
        
        if (obj is Enum @enum)
            return @enum.ToString();

        if (FireSerializerModuleAttribute.Serializers
            .TryFirst(x => x.CanSerialize(type),
                out var serializer))
            return serializer.Serialize(obj);

        if (type.InheritsFrom(typeof(IEnumerable))
            && HandleIEnumerableSerialize(type, (IEnumerable) obj, out string result))
            return result;
        
        throw new Exception($"Unsupported conversion type: {type}");
    }

    public static object Deserialize(Type type, string str)
    {
        if (string.IsNullOrEmpty(str))
            return null;
        
        if (type.IsPrimitive)
            return Convert.ChangeType(str, type);
        
        if (type.InheritsFrom(typeof(Enum)))
            return Enum.Parse(type, str, true);
        
        if (FireSerializerModuleAttribute.Serializers
            .TryFirst(x => x.CanSerialize(type),
                out var serializer))
            return serializer.Deserialize(str);
        
        if (type.InheritsFrom(typeof(IEnumerable))
               && HandleIEnumerableDeserialize(type, str, out var result))
            return result;
        
        throw new Exception($"Unsupported conversion type: {type}");
    }

    public static T Deserialize<T>(string str)
    {
        return (T) Deserialize(typeof(T), str);
    }
    
    public static bool IsSerializable(Type type)
    {
        return FireSerializerModuleAttribute.Serializers.Any(x => x.CanSerialize(type))
               || type.IsPrimitive
               || type.InheritsFrom(typeof(Enum))
               || type.InheritsFrom(typeof(IEnumerable));
    }

    private static bool HandleIEnumerableSerialize(Type enumerableType, IEnumerable from, out string result)
    {
        result = default;
        
        IEnumerable<object> collection = from.Cast<object>();

        if (!enumerableType.IsArray && enumerableType.GetGenericArguments().Length != 1)
            return false;

        result = collection.Any() 
            ? $"[{string.Join(";", collection.Select(Serialize))}]"
            : "[]";
        return true;
    }

    private static bool HandleIEnumerableDeserialize(Type enumerableType, string from, out IEnumerable result)
    {
        result = default;
        
        var genericArguments = enumerableType.GetGenericArguments();
        int genericArgumentLength = genericArguments.Length;
        bool isArray = enumerableType.IsArray;
        if (!isArray && genericArgumentLength != 1)
            return false;

        Type argType = isArray
            ? enumerableType.GetElementType()
            : genericArguments[0];

        if (from.Length >= 2 && (from.First() != '[' || from.Last() != ']'))
            throw new Exception($"Error while parsing type: {enumerableType}");

        from = from.Substring(1, from.Length - 2);

        string[] split = SmartIEnumerableSplit(from);
        var usableCollection = split.Select(x => Deserialize(argType, x));

        var arr = Array.CreateInstance(argType!, split.Length);
        for (int i = 0; i < split.Length; i++)
        {
            arr.SetValue(usableCollection.ElementAt(i), i);
        }
        
        if (isArray)
        {
            result = arr;
            return true;
        }
        
        if (enumerableType.Name == "List`1")
        {
            var listType = typeof(List<>).MakeGenericType(argType!);
            var list = (IList) Activator.CreateInstance(listType);
            
            for (int i = 0; i < arr.Length; i++)
            {
                list.Add(arr.GetValue(i));
            }

            result = list;
            return true;
        }

        return false;
    }
    
    private static string[] SmartIEnumerableSplit(string str)
    {
        List<string> split = new();
        StringBuilder stringBuilder = new();

        int expectedClosingBrackets = 0;
    
        for (int i = 0; i < str.Length; i++)
        {
            char currentCharacter = str[i];

            switch (currentCharacter)
            {
                case ';':
                    if (expectedClosingBrackets > 0)
                        break;
                
                    split.Add(stringBuilder.ToString());
                    stringBuilder.Clear();
                    continue;
                case ']':
                    expectedClosingBrackets--;
                    break;
                case '[':
                    expectedClosingBrackets++;
                    break;
            }

            stringBuilder.Append(currentCharacter);
        }
        
        if (stringBuilder.Length > 0)
            split.Add(stringBuilder.ToString());
        
        return split.ToArray();
    }
}