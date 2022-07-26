// Decompiled with JetBrains decompiler
// Type: DuckGame.LobbyFilterComparison
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public enum LobbyFilterComparison
    {
        EqualOrLessThan = -2, // 0xFFFFFFFE
        LessThan = -1, // 0xFFFFFFFF
        Equal = 0,
        GreaterThan = 1,
        EqualToOrGreaterThan = 2,
        NotEqual = 3,
    }
}
