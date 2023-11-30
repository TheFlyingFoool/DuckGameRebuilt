using System.Runtime.CompilerServices;

namespace DuckGame
{
    internal static class SwitchSixAxis
    {
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void Startup();

        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void InitializeSixAxis(int pIndex);

        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern float GetAxis(int pIndex);
    }
}
