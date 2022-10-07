// Decompiled with JetBrains decompiler
// Type: XnaToFna.FakeMonitor
// Assembly: XnaToFna, Version=18.5.1.29483, Culture=neutral, PublicKeyToken=null
// MVID: C1D3521D-C7E9-4C43-B430-D28CC69450A3
// Assembly location: C:\Users\daniel\Desktop\Release\XnaToFna.exe

namespace XnaToFna
{
    public static class FakeMonitor
    {
        public static void Enter(object o, ref bool b) => b = true;

        public static void Exit(object o)
        {
        }
    }
}
