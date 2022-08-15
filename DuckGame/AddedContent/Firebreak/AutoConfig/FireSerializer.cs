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

        foreach (var serializerModule in FireSerializerModuleAttribute.Serializers)
        {
            if (!serializerModule.CanSerialize(type))
                continue;
            
            return serializerModule.Serialize(obj);
        }

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

    public static string UnescapeString(string str)
    {
        StringBuilder builder = new();
        for (int i = 0; i < str.Length; i++)
        {
            char c = str[i];
            switch (c)
            {
                case '\\':
                {
                    builder.Append(str[++i]);
                    break;
                }
                default:
                {
                    builder.Append(c);
                    break;
                }
            }
        }

        return builder.ToString();
    }

    public static readonly char[] BadChars = new[] 
    {
        '[',
        ']',
        ';',
    };

    public static string EscapeBadStringCharacters(string str)
    {
        StringBuilder builder = new();
        
        for (int i = 0; i < str.Length; i++)
        {
            char c = str[i];
            
            if (BadChars.Contains(c))
                builder.Append('\\');

            builder.Append(c);
        }

        return builder.ToString();
    }

    private static bool HandleIEnumerableSerialize(Type enumerableType, IEnumerable from, out string result)
    {
        result = default;
        
        IEnumerable<object> collection = from.Cast<object>();

        if (!enumerableType.IsArray && enumerableType.GetGenericArguments().Length != 1)
            return false;

        result = collection.Any() 
            ? $"[{string.Join(";", collection.Select(o => $"{EscapeBadStringCharacters(Serialize(o))}"))}]"
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
            throw new Exception($"Invalid IEnumerable format for type: {enumerableType}");

        from = from.Substring(1, from.Length - 2);

        string[] split = SmartIEnumerableSplit(from);
        var usableCollection = split.Select(x => Deserialize(argType, UnescapeString(x)));

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
        bool capturingItem = false;
    
        for (int i = 0; i < str.Length; i++)
        {
            char currentCharacter = str[i];

            switch (currentCharacter)
            {
                case '\\':
                    // stringBuilder.Append(currentCharacter);
                    stringBuilder.Append(str[++i]);
                    break;
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