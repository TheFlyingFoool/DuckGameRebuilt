using System;
using System.Collections.Generic;
using System.Reflection;

namespace XnaToFna
{
    public static class StackOpHelper
    {
        [ThreadStatic]
        private static Stack<object> Current;
        public static readonly MethodInfo m_Push = typeof(StackOpHelper).GetMethod("Push");
        public static readonly MethodInfo m_Pop = typeof(StackOpHelper).GetMethod("Pop");

        public static void Push<T>(T value)
        {
            if (Current == null)
                Current = new Stack<object>();
            Current.Push(value);
        }

        public static T Pop<T>() => (T)Current.Pop();
    }
}
