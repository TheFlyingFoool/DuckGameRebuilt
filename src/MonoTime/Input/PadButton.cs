// Decompiled with JetBrains decompiler
// Type: DuckGame.PadButtonComparer
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

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
