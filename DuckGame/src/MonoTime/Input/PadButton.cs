using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace DuckGame
{
    [StructLayout(LayoutKind.Sequential, Size = 1)]
    public struct PadButtonComparer : IEqualityComparer<PadButton>
    {
        public bool Equals(PadButton x, PadButton y) => x == y;

        public int GetHashCode(PadButton obj) => (int)obj;
    }
}
