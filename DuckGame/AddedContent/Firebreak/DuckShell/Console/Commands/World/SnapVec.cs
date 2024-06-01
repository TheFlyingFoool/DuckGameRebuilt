using AddedContent.Firebreak;
using System;

namespace DuckGame.ConsoleEngine
{
    public static partial class Commands
    {
        [Marker.DevConsoleCommand(Description = "Snaps the given vector to a grid of a given size")]
        [return: PrintSerialized]
        public static Vec2 SnapVec(Vec2 vector, float snapSize = 16f)
        {
            // note: Maths.Snap has wonky snapping alignment, while
            // Maths.SnapRound snaps to nearest cell, so i'm using that here
            return Maths.SnapRound(vector, snapSize, snapSize);
        }
    }
}