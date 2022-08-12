using System;
using System.Linq;
using System.Reflection;

namespace DuckGame;

[AttributeUsage(AttributeTargets.Method)]
public class PostInitializeAttribute : Attribute
{
    public static readonly MethodInfo[] All
        = MemberAttributePair<MethodInfo, PostInitializeAttribute>.GetAll()
            .Select(x => x.MemberInfo)
            .ToArray();
}