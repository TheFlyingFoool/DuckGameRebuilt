using System;
using System.Collections.Generic;
using System.Reflection;

namespace AddedContent.Firebreak
{
    internal static partial class Marker
    {
        [AttributeUsage(AttributeTargets.Method)]
        internal class UpdateContextAttribute : MarkerAttribute
        {
            public static List<UpdateContextAttribute> All = new();

            protected override void Implement()
            {
                All.Add(this);
            }

            public void Invoke()
            {
                ((MethodInfo)Member).Invoke(null, null);
            }
        }
    }
}