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
