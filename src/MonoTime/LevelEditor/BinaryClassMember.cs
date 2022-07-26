// Decompiled with JetBrains decompiler
// Type: DuckGame.BinaryClassMember
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
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
        (byte) 1
      },
      {
        typeof (uint),
        (byte) 2
      },
      {
        typeof (short),
        (byte) 3
      },
      {
        typeof (ushort),
        (byte) 4
      },
      {
        typeof (sbyte),
        (byte) 5
      },
      {
        typeof (byte),
        (byte) 6
      },
      {
        typeof (double),
        (byte) 7
      },
      {
        typeof (float),
        (byte) 8
      },
      {
        typeof (long),
        (byte) 9
      },
      {
        typeof (ulong),
        (byte) 10
      },
      {
        typeof (string),
        (byte) 11
      },
      {
        typeof (bool),
        (byte) 12
      }
    };
    }
}
