// Decompiled with JetBrains decompiler
// Type: XnaToFna.StubXDK.GamerServices.GamerExtensions
// Assembly: XnaToFna, Version=18.5.1.29483, Culture=neutral, PublicKeyToken=null
// MVID: C1D3521D-C7E9-4C43-B430-D28CC69450A3
// Assembly location: C:\Users\daniel\Desktop\Release\XnaToFna.exe

using MonoMod;

namespace XnaToFna.StubXDK.GamerServices
{
  public static class GamerExtensions
  {
    [MonoModHook("System.UInt64 XnaToFna.StubXDK.GamerServices.GamerExtensions::GetXuid(Microsoft.Xna.Framework.GamerServices.Gamer)")]
    public static ulong GetXuid(object gamer) => 0;
  }
}
