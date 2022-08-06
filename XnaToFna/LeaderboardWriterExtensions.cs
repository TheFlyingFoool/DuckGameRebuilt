// Decompiled with JetBrains decompiler
// Type: XnaToFna.StubXDK.GamerServices.LeaderboardWriterExtensions
// Assembly: XnaToFna, Version=18.5.1.29483, Culture=neutral, PublicKeyToken=null
// MVID: C1D3521D-C7E9-4C43-B430-D28CC69450A3
// Assembly location: C:\Users\daniel\Desktop\Release\XnaToFna.exe

using MonoMod;

namespace XnaToFna.StubXDK.GamerServices
{
  public static class LeaderboardWriterExtensions
  {
    [MonoModHook("System.Void XnaToFna.StubXDK.GamerServices.LeaderboardWriterExtensions::SetScore(Microsoft.Xna.Framework.GamerServices.LeaderboardWriter,System.Int32)")]
    public static void SetScore(object writer, int score)
    {
    }
  }
}
