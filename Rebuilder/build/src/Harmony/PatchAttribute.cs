using System;
using System.Reflection;

namespace DuckGame.Cobalt
{
    [AttributeUsage(AttributeTargets.Method)]
    public class PatchAttribute : Attribute
    {
        public Harmony.Fix PatchType { get; }
        public readonly MethodInfo MethodToPatch;

        public PatchAttribute(Type parentClass, string methodName, Harmony.Fix patchType, BindingFlags methodBindingFlags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public)
        {
            PatchType = patchType;
            MethodToPatch = parentClass.GetMethod(methodName, methodBindingFlags);
        }
    }
}