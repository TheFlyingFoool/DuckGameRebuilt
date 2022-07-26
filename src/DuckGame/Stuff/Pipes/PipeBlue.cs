// Decompiled with JetBrains decompiler
// Type: DuckGame.PipeBlue
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections.Generic;
using System.Linq;

namespace DuckGame
{
    [EditorGroup("Stuff|Pipes")]
    [BaggedProperty("isOnlineCapable", true)]
    public class PipeBlue : PipeTileset
    {
        public PipeBlue(float x, float y)
          : base(x, y, "travelPipesBlue")
        {
            this._editorName = "Blue Pipe";
            this.editorTooltip = "Ducks who travel through Blue pipes sometimes wish they'd never come out the other side.";
            this.pipeDepth = 0.95f;
        }

        protected override Dictionary<PipeTileset.Direction, PipeTileset> GetNeighbors()
        {
            Dictionary<PipeTileset.Direction, PipeTileset> neighbors = new Dictionary<PipeTileset.Direction, PipeTileset>();
            PipeTileset pipeTileset1 = (PipeTileset)Level.CheckPointAll<PipeBlue>(this.x, this.y - 16f).Where<PipeBlue>((Func<PipeBlue, bool>)(x => x.group == this.group)).FirstOrDefault<PipeBlue>();
            if (pipeTileset1 != null)
                neighbors[PipeTileset.Direction.Up] = pipeTileset1;
            PipeTileset pipeTileset2 = (PipeTileset)Level.CheckPointAll<PipeBlue>(this.x, this.y + 16f).Where<PipeBlue>((Func<PipeBlue, bool>)(x => x.group == this.group)).FirstOrDefault<PipeBlue>();
            if (pipeTileset2 != null)
                neighbors[PipeTileset.Direction.Down] = pipeTileset2;
            PipeTileset pipeTileset3 = (PipeTileset)Level.CheckPointAll<PipeBlue>(this.x - 16f, this.y).Where<PipeBlue>((Func<PipeBlue, bool>)(x => x.group == this.group)).FirstOrDefault<PipeBlue>();
            if (pipeTileset3 != null)
                neighbors[PipeTileset.Direction.Left] = pipeTileset3;
            PipeTileset pipeTileset4 = (PipeTileset)Level.CheckPointAll<PipeBlue>(this.x + 16f, this.y).Where<PipeBlue>((Func<PipeBlue, bool>)(x => x.group == this.group)).FirstOrDefault<PipeBlue>();
            if (pipeTileset4 != null)
                neighbors[PipeTileset.Direction.Right] = pipeTileset4;
            return neighbors;
        }
    }
}
