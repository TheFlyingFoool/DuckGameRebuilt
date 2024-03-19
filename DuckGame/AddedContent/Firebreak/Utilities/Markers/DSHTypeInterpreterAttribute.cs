using AddedContent.Firebreak.DuckShell.Implementation;
using System;
using System.Reflection;

namespace AddedContent.Firebreak
{
    public static partial class Marker
    {
        [AttributeUsage(AttributeTargets.Class)]
        public class DSHTypeInterpreterAttribute : MarkerAttribute
        {
            protected override void Implement()
            {
                DevConsoleDSHWrapper.TypeInterpreterInfos.Add((TypeInfo) Member);
            }
        }
    }
}