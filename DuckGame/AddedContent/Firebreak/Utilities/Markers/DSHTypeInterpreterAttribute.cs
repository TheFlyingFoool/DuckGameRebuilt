using AddedContent.Firebreak.DuckShell.Implementation;
using System;
using System.Reflection;

namespace AddedContent.Firebreak
{
    internal static partial class Marker
    {
        [AttributeUsage(AttributeTargets.Class)]
        internal class DSHTypeInterpreterAttribute : MarkerAttribute
        {
            protected override void Implement()
            {
                DevConsoleDSHWrapper.TypeInterpreterInfos.Add((TypeInfo) Member);
            }
        }
    }
}