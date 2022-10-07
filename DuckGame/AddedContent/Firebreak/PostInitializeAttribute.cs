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
        static PostInitializeAttribute()
        {
        }
        public static void OnResults(Dictionary<Type, List<(MemberInfo MemberInfo, Attribute Attribute)>> all)
        {
            foreach ((MemberInfo MemberInfo, Attribute vAttribute) in all[typeof(PostInitializeAttribute)])
            {
                All.Add(MemberInfo as MethodInfo);

            }
        }
    }
}