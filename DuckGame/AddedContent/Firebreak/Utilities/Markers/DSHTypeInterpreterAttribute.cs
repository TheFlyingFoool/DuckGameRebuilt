using AddedContent.Firebreak.DuckShell.Implementation;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace AddedContent.Firebreak
{
    internal static partial class Marker
    {
        [AttributeUsage(AttributeTargets.Class)]
        internal class DSHTypeInterpreterAttribute : MarkerAttribute
        {
            public static List<TypeInfo> AllTypes = new();

            protected override void Implement()
            {
                AllTypes.Add((TypeInfo) Member);
            }
        }
    }
}