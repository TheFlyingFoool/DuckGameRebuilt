// Decompiled with JetBrains decompiler
// Type: DuckGame.BinaryClassMember
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public struct BinaryClassMember
    {
        public string name;
        public object data;
        public bool extra;
        public static Map<System.Type, byte> typeMap = new Map<System.Type, byte>()
    {
      {
        typeof (int),
         1
      },
      {
        typeof (uint),
         2
      },
      {
        typeof (short),
         3
      },
      {
        typeof (ushort),
         4
      },
      {
        typeof (sbyte),
         5
      },
      {
        typeof (byte),
         6
      },
      {
        typeof (double),
         7
      },
      {
        typeof (float),
         8
      },
      {
        typeof (long),
         9
      },
      {
        typeof (ulong),
         10
      },
      {
        typeof (string),
         11
      },
      {
        typeof (bool),
         12
      }
    };
    }
}
