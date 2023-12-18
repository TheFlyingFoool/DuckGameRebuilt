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
