using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DuckGame
{

    [AttributeUsage(AttributeTargets.Method)]
    public class PostInitializeAttribute : Attribute
    {
        public static List<MethodInfo> All = new List<MethodInfo>();
        public MethodInfo MethodInfo;
        /// priority order is Ascending (0, 1, 2, 3, 4, ...)
        public int Priority = 1;
        static PostInitializeAttribute()
        {
        }
        public static void OnResults(Dictionary<Type, List<(MemberInfo MemberInfo, Attribute Attribute)>> all)
        {
            foreach ((MemberInfo MemberInfo, Attribute vAttribute) in all[typeof(PostInitializeAttribute)]
                         .OrderBy(x => ((PostInitializeAttribute) x.Attribute).Priority))
            {
                All.Add(MemberInfo as MethodInfo);
            }
        }
    }
}