// Decompiled with JetBrains decompiler
// Type: XnaToFna.StubXDK.GamerServices.__LeaderboardIdentity__
// Assembly: XnaToFna, Version=18.5.1.29483, Culture=neutral, PublicKeyToken=null
// MVID: C1D3521D-C7E9-4C43-B430-D28CC69450A3
// Assembly location: C:\Users\daniel\Desktop\Release\XnaToFna.exe

using MonoMod;

namespace XnaToFna.StubXDK.GamerServices
{
  public static class __LeaderboardIdentity__
  {
    [MonoModHook("System.Void Microsoft.Xna.Framework.GamerServices.LeaderboardIdentity::set_Key(System.String)")]
    public static void set_Key(ref object identity, string value)
    {
    }

    [MonoModHook("System.String Microsoft.Xna.Framework.GamerServices.LeaderboardIdentity::get_Key()")]
    public static string get_Key(ref object identity) => "";
  }
}
