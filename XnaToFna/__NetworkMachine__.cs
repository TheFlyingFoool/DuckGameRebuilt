// Decompiled with JetBrains decompiler
// Type: XnaToFna.StubXDK.Net.__NetworkMachine__
// Assembly: XnaToFna, Version=18.5.1.29483, Culture=neutral, PublicKeyToken=null
// MVID: C1D3521D-C7E9-4C43-B430-D28CC69450A3
// Assembly location: C:\Users\daniel\Desktop\Release\XnaToFna.exe

using MonoMod;

namespace XnaToFna.StubXDK.Net
{
  public static class __NetworkMachine__
  {
    [MonoModHook("System.Void Microsoft.Xna.Framework.Net.NetworkMachine::RemoveFromSession()")]
    public static void RemoveFromSession(object machine)
    {
    }
  }
}
