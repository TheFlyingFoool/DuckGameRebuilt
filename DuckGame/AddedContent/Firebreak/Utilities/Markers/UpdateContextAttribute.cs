using System;
using System.Collections.Generic;
using System.Reflection;

namespace AddedContent.Firebreak
{
    public static partial class Marker
    {
        [AttributeUsage(AttributeTargets.Method)]
        public class UpdateContextAttribute : MarkerAttribute
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