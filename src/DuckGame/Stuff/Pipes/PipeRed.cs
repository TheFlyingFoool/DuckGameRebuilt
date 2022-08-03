// Decompiled with JetBrains decompiler
// Type: DuckGame.PipeRed
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

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

        protected override Dictionary<PipeTileset.Direction, PipeTileset> GetNeighbors()
        {
            Dictionary<PipeTileset.Direction, PipeTileset> neighbors = new Dictionary<PipeTileset.Direction, PipeTileset>();
            PipeTileset pipeTileset1 = Level.CheckPointAll<PipeRed>(x, y - 16f).Where<PipeRed>(x => x.group == group).FirstOrDefault<PipeRed>();
            if (pipeTileset1 != null)
                neighbors[PipeTileset.Direction.Up] = pipeTileset1;
            PipeTileset pipeTileset2 = Level.CheckPointAll<PipeRed>(x, y + 16f).Where<PipeRed>(x => x.group == group).FirstOrDefault<PipeRed>();
            if (pipeTileset2 != null)
                neighbors[PipeTileset.Direction.Down] = pipeTileset2;
            PipeTileset pipeTileset3 = Level.CheckPointAll<PipeRed>(x - 16f, y).Where<PipeRed>(x => x.group == group).FirstOrDefault<PipeRed>();
            if (pipeTileset3 != null)
                neighbors[PipeTileset.Direction.Left] = pipeTileset3;
            PipeTileset pipeTileset4 = Level.CheckPointAll<PipeRed>(x + 16f, y).Where<PipeRed>(x => x.group == group).FirstOrDefault<PipeRed>();
            if (pipeTileset4 != null)
                neighbors[PipeTileset.Direction.Right] = pipeTileset4;
            return neighbors;
        }
    }
}
