// Decompiled with JetBrains decompiler
// Type: DuckGame.PipeGreen
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System.Collections.Generic;
using System.Linq;

namespace DuckGame
{
    [EditorGroup("Stuff|Pipes")]
    [BaggedProperty("isOnlineCapable", true)]
    public class PipeGreen : PipeTileset
    {
        public PipeGreen(float x, float y)
          : base(x, y, "travelPipesGreen")
        {
            _editorName = "Green Pipe";
            editorTooltip = "Ducks who travel through Green pipes are said to be industrious and savvy.";
            pipeDepth = 0.91f;
        }

        protected override Dictionary<Direction, PipeTileset> GetNeighbors()
        {
            Dictionary<Direction, PipeTileset> neighbors = new Dictionary<Direction, PipeTileset>();
            PipeTileset pipeTileset1 = Level.CheckPointAll<PipeGreen>(x, y - 16f).Where(x => x.group == group).FirstOrDefault();
            if (pipeTileset1 != null)
                neighbors[Direction.Up] = pipeTileset1;
            PipeTileset pipeTileset2 = Level.CheckPointAll<PipeGreen>(x, y + 16f).Where(x => x.group == group).FirstOrDefault();
            if (pipeTileset2 != null)
                neighbors[Direction.Down] = pipeTileset2;
            PipeTileset pipeTileset3 = Level.CheckPointAll<PipeGreen>(x - 16f, y).Where(x => x.group == group).FirstOrDefault();
            if (pipeTileset3 != null)
                neighbors[Direction.Left] = pipeTileset3;
            PipeTileset pipeTileset4 = Level.CheckPointAll<PipeGreen>(x + 16f, y).Where(x => x.group == group).FirstOrDefault();
            if (pipeTileset4 != null)
                neighbors[Direction.Right] = pipeTileset4;
            return neighbors;
        }
    }
}
