using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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

    public static bool CaselessEquals(this string str, string str2) => 
        string.Equals(str, str2, StringComparison.CurrentCultureIgnoreCase);
}