// Decompiled with JetBrains decompiler
// Type: XnaToFna.StackOpHelper
// Assembly: XnaToFna, Version=18.5.1.29483, Culture=neutral, PublicKeyToken=null
// MVID: C1D3521D-C7E9-4C43-B430-D28CC69450A3
// Assembly location: C:\Users\daniel\Desktop\Release\XnaToFna.exe

using System;
using System.Collections.Generic;
using System.Reflection;

namespace XnaToFna
{
  public static class StackOpHelper
  {
    [ThreadStatic]
    private static Stack<object> Current;
    public static readonly MethodInfo m_Push = typeof (StackOpHelper).GetMethod("Push");
    public static readonly MethodInfo m_Pop = typeof (StackOpHelper).GetMethod("Pop");

    public static void Push<T>(T value)
    {
      if (StackOpHelper.Current == null)
        StackOpHelper.Current = new Stack<object>();
      StackOpHelper.Current.Push(value);
    }

    public static T Pop<T>() => (T) StackOpHelper.Current.Pop();
  }
}
