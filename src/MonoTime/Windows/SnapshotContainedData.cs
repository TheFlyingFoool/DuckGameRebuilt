// Decompiled with JetBrains decompiler
// Type: DuckGame.SnapshotContainedData
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;

namespace DuckGame
{
    [Flags]
    public enum SnapshotContainedData
    {
        None = 0,
        Position = 1,
        Angle = 2,
        Velocity = 4,
        Frame = 8,
        EndOfData = 255, // 0x000000FF
    }
}
