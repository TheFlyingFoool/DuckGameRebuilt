using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DuckGame;

[AttributeUsage(AttributeTargets.Method)]
public class PostInitializeAttribute : Attribute
{
    public static IEnumerable<MethodInfo> All;

    static PostInitializeAttribute()
    {
        MemberAttributePair<MethodInfo, PostInitializeAttribute>.RequestSearch(all =>
        {
            All = all.Select(x => x.MemberInfo);
        });
    }
}