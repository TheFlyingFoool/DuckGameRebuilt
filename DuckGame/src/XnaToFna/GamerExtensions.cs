using MonoMod;

namespace XnaToFna.StubXDK.GamerServices
{
    public static class GamerExtensions
    {
        [MonoModHook("System.UInt64 XnaToFna.StubXDK.GamerServices.GamerExtensions::GetXuid(Microsoft.Xna.Framework.GamerServices.Gamer)")]
        public static ulong GetXuid(object gamer) => 0;
    }
}
