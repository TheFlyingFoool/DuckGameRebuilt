using System.Collections.Generic;
using System.Linq;

namespace DuckGame
{
    [EditorGroup("Stuff|Pipes")]
    [BaggedProperty("isOnlineCapable", true)]
    public class PipeRed : PipeTileset
    {
        public PipeRed(float x, float y)
          : base(x, y, "travelPipes")
        {
            _editorName = "Red Pipe";
            editorTooltip = "Ducks who travel through Red pipes are said to have good hearts.";
            pipeDepth = 0.93f;
        }

        protected override Dictionary<Direction, PipeTileset> GetNeighbors()
        {
            Dictionary<Direction, PipeTileset> neighbors = new Dictionary<Direction, PipeTileset>();
            PipeTileset pipeTileset1 = Level.CheckPointAll<PipeRed>(x, y - 16f).Where(x => x.group == group).FirstOrDefault();
            if (pipeTileset1 != null)
                neighbors[Direction.Up] = pipeTileset1;
            PipeTileset pipeTileset2 = Level.CheckPointAll<PipeRed>(x, y + 16f).Where(x => x.group == group).FirstOrDefault();
            if (pipeTileset2 != null)
                neighbors[Direction.Down] = pipeTileset2;
            PipeTileset pipeTileset3 = Level.CheckPointAll<PipeRed>(x - 16f, y).Where(x => x.group == group).FirstOrDefault();
            if (pipeTileset3 != null)
                neighbors[Direction.Left] = pipeTileset3;
            PipeTileset pipeTileset4 = Level.CheckPointAll<PipeRed>(x + 16f, y).Where(x => x.group == group).FirstOrDefault();
            if (pipeTileset4 != null)
                neighbors[Direction.Right] = pipeTileset4;
            return neighbors;
        }
    }
}
