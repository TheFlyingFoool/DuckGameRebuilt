// Decompiled with JetBrains decompiler
// Type: XnaToFna.StubXDK.Net.__LocalNetworkGamer__
// Assembly: XnaToFna, Version=18.5.1.29483, Culture=neutral, PublicKeyToken=null
// MVID: C1D3521D-C7E9-4C43-B430-D28CC69450A3
// Assembly location: C:\Users\daniel\Desktop\Release\XnaToFna.exe

using MonoMod;

namespace XnaToFna.StubXDK.Net
{
    public static class __LocalNetworkGamer__
    {
        [MonoModHook("System.Void Microsoft.Xna.Framework.Net.LocalNetworkGamer::SendPartyInvites()")]
        public static void SendPartyInvites(object gamer)
        {
        }

        [MonoModHook("System.Void Microsoft.Xna.Framework.Net.LocalNetworkGamer::EnableSendVoice(Microsoft.Xna.Framework.Net.NetworkGamer,System.Boolean)")]
        public static void EnableSendVoice(object gamer, object remote, bool flag)
        {
        }
    }
}
