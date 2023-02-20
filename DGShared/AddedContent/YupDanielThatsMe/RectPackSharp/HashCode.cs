using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace RectpackSharp
{
    internal static class HashCode
    {
        private static void Initialize(out uint v1, out uint v2, out uint v3, out uint v4)
        {
            v1 = s_seed + 2654435761U + 2246822519U;
            v2 = s_seed + 2246822519U;
            v3 = s_seed;
            v4 = s_seed - 2654435761U;
        }
        public static int Combine<T1, T2, T3, T4, T5>(T1 value1, T2 value2, T3 value3, T4 value4, T5 value5)
        {
            uint input = (uint)((value1 != null) ? value1.GetHashCode() : 0);
            uint input2 = (uint)((value2 != null) ? value2.GetHashCode() : 0);
            uint input3 = (uint)((value3 != null) ? value3.GetHashCode() : 0);
            uint input4 = (uint)((value4 != null) ? value4.GetHashCode() : 0);
            uint queuedValue = (uint)((value5 != null) ? value5.GetHashCode() : 0);
            uint num;
            uint num2;
            uint num3;
            uint num4;
            Initialize(out num, out num2, out num3, out num4);
            num = Round(num, input);
            num2 = Round(num2, input2);
            num3 = Round(num3, input3);
            num4 = Round(num4, input4);
            uint num5 = MixState(num, num2, num3, num4);
            num5 += 20U;
            num5 = QueueRound(num5, queuedValue);
            return (int)MixFinal(num5);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static uint Round(uint hash, uint input)
        {
            return RotateLeft(hash + input * 2246822519U, 13) * 2654435761U;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint RotateLeft(uint value, int offset)
        {
            return value << offset | value >> 32 - offset;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong RotateLeft(ulong value, int offset)
        {
            return value << offset | value >> 64 - offset;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static uint MixState(uint v1, uint v2, uint v3, uint v4)
        {
            return RotateLeft(v1, 1) + RotateLeft(v2, 7) + RotateLeft(v3, 12) + RotateLeft(v4, 18);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static uint QueueRound(uint hash, uint queuedValue)
        {
            return RotateLeft(hash + queuedValue * 3266489917U, 17) * 668265263U;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static uint MixFinal(uint hash)
        {
            hash ^= hash >> 15;
            hash *= 2246822519U;
            hash ^= hash >> 13;
            hash *= 3266489917U;
            hash ^= hash >> 16;
            return hash;
        }
        private static unsafe uint GenerateGlobalSeed()
        {
            uint result;
            GetRandomBytes((byte*)(&result), 4);
            return result;
        }
        internal static unsafe void GetRandomBytes(byte* buffer, int length)
        {
            if (!UseNonRandomizedHashSeed)
            {
                using (RandomNumberGenerator randomNumberGenerator = RandomNumberGenerator.Create())
                {
                    byte[] array = new byte[length];
                    randomNumberGenerator.GetBytes(array);
                    Marshal.Copy(array, 0, (IntPtr)((void*)buffer), length);
                }
            }
        }
        private static readonly uint s_seed = GenerateGlobalSeed();

        public static bool UseNonRandomizedHashSeed
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return GetCachedSwitchValue("Switch.System.Data.UseNonRandomizedHashSeed", ref s_useNonRandomizedHashSeed);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool GetCachedSwitchValue(string switchName, ref int cachedSwitchValue)
        {
            return cachedSwitchValue >= 0 && (cachedSwitchValue > 0 || GetCachedSwitchValueInternal(switchName, ref cachedSwitchValue));
        }

        private static bool GetCachedSwitchValueInternal(string switchName, ref int cachedSwitchValue)
        {
            bool switchDefaultValue;
            if (!AppContext.TryGetSwitch(switchName, out switchDefaultValue))
            {
                switchDefaultValue = GetSwitchDefaultValue(switchName);
            }
            bool flag;
            AppContext.TryGetSwitch("TestSwitch.LocalAppContext.DisableCaching", out flag);
            if (!flag)
            {
                cachedSwitchValue = (switchDefaultValue ? 1 : -1);
            }
            return switchDefaultValue;
        }

        private static bool GetSwitchDefaultValue(string switchName)
        {
            return switchName == "Switch.System.Runtime.Serialization.SerializationGuard";
        }

        private static int s_useNonRandomizedHashSeed;
    }
}
