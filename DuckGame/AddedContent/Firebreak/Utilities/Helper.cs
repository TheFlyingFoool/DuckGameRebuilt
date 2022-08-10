using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace DuckGame;

public static class Helper
{
    public static IEnumerable<(TInfoType Member, IEnumerable<TAttribute> Attributes)>
        GetAllMembersWithAttribute<TInfoType, TAttribute>(Type? inType = null) where TInfoType : MemberInfo 
        where TAttribute : Attribute
    {
        MemberTypes memberType = MemberTypes.All;
    
        if (typeof(TInfoType).IsAssignableFrom(typeof(FieldInfo)))
            memberType = MemberTypes.Field;
        else if (typeof(TInfoType).IsAssignableFrom(typeof(MethodInfo)))
            memberType = MemberTypes.Method;
        else if (typeof(TInfoType).IsAssignableFrom(typeof(PropertyInfo)))
            memberType = MemberTypes.Property;
        else if (typeof(TInfoType).IsAssignableFrom(typeof(ConstructorInfo)))
            memberType = MemberTypes.Constructor;
        
        return GetAllMembersWithAttribute<TAttribute>(memberType, inType)
            .Select<(MemberInfo Member, IEnumerable<TAttribute> Attributes), (TInfoType, IEnumerable<TAttribute>)>
                (x => ((TInfoType) x.Member, x.Attributes));
    }
    
    public static IEnumerable<(MemberInfo Member, IEnumerable<TAttribute> Attributes)>
        GetAllMembersWithAttribute<TAttribute>(MemberTypes filter = MemberTypes.All, Type? inType = null) where TAttribute : Attribute
    {
        if (inType is { })
            return inType.GetMembers(BindingFlags.Public | BindingFlags.Static)
                .Where(x => x.GetCustomAttributes<TAttribute>(false).Any()
                            && x.MemberType.HasFlag(filter))
                .Select(x => (x, x.GetCustomAttributes<TAttribute>(false)));
        
        return Assembly.GetExecutingAssembly()
            .GetTypes()
            .SelectMany(x => x.GetMembers(BindingFlags.Public | BindingFlags.Static))
            .Where(x => x.GetCustomAttributes<TAttribute>(false).Any()
                        && x.MemberType.HasFlag(filter))
            .Select(x => (x, x.GetCustomAttributes<TAttribute>(false)));
    }
    
    public static bool TryFirst<T>(this IEnumerable<T> collection, Func<T, bool> condition, out T? first)
    {
        first = default;
        if (!collection.Any(condition)) return false;
        first = collection.First(condition);
        return true;
    }

    public static string GetFullName(this MemberInfo mi) => $"{mi.DeclaringType}:{mi.Name}";
    
    public static bool InheritsFrom(this Type t1, Type t2) => t2.IsAssignableFrom(t1);

    public static bool CaselessEquals(this string str, string str2) => 
        string.Equals(str, str2, StringComparison.CurrentCultureIgnoreCase);

    public static string ToReadableString(this IEnumerable<object> collection, int indentationLevel = 0, bool doIndent = true)
    {
        object[] arr = collection.ToArray();
        StringBuilder stringBuilder = new();
        
        stringBuilder.Append("[\n");
        indentationLevel++;

        for (int i = 0; i < arr.Length; i++)
        {
            object o = arr[i];
            if (o is IEnumerable<object> subCollection)
            {
                indentedAppend(subCollection.ToReadableString(indentationLevel));
            }
            else indentedAppend(o.ToString());

            if (i != arr.Length)
                stringBuilder.Append(",\n");
        }

        indentationLevel--;
        indentedAppend("]");

        return stringBuilder.ToString();
        
        void indentedAppend(string s) => 
            stringBuilder.Append($"{(doIndent ? new string(' ', indentationLevel * 2) : "")}{s}");
    }

    public static bool Try(Action action)
    {
        try
        {
            action();
        }
        catch
        {
            return false;
        }

        return true;
    }

    public static bool Try<T>(Func<T> action, out T result)
    {
        result = default;
        
        try
        {
            result = action();
        }
        catch
        {
            return false;
        }

        return true;
    }
}