using DuckGame;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace AddedContent.Firebreak
{
    internal static partial class Marker
    {
        /// <summary>
        /// like a drawing context... but complex...
        /// </summary>
        [AttributeUsage(AttributeTargets.Method, Inherited = false)]
        internal sealed class ComplexDrawingContextAttribute : MarkerAttribute
        {
            public static List<ComplexDrawingContextAttribute> AllDrawingContexts = new();

            protected override void Implement()
            {
                MethodInfo method = (MethodInfo)Member;
                ParameterInfo[] parameters = method.GetParameters();

                if (parameters.Length != 1 || parameters[0].ParameterType != typeof(Layer))
                    throw new TargetParameterCountException("ComplexDrawingContext method structure should be FUNC(DuckGame.Layer)");
                
                AllDrawingContexts.Add(this);
            }

            public static void ExecuteAll(Layer layer)
            {
                foreach (ComplexDrawingContextAttribute marker in AllDrawingContexts)
                {
                    ((MethodInfo)marker.Member).Invoke(null, new[] {layer});
                }
            }
        }
    }
}