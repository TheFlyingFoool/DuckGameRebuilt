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
