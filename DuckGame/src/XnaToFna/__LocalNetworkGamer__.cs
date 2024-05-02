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
