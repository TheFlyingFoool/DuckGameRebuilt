using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DuckGame
{

    [AttributeUsage(AttributeTargets.Method)]
    public class OnProgramStartAttribute : Attribute
    {
        public static IEnumerable<MethodInfo> All;

        static OnProgramStartAttribute()
        {
            MemberAttributePair<MethodInfo, OnProgramStartAttribute>.RequestSearch(all =>
            {
                All = all.Select(x => x.MemberInfo);
            });
        }
    }
}