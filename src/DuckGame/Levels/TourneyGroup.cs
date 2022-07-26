// Decompiled with JetBrains decompiler
// Type: DuckGame.TourneyGroup
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System.Collections.Generic;

namespace DuckGame
{
    public class TourneyGroup
    {
        public List<Team> players = new List<Team>();
        public List<bool> assigned = new List<bool>();
        public TourneyGroup next;
        public int groupIndex;
        public int depth;

        public void AddPlayer(Team p, bool ass = false)
        {
            this.players.Add(p);
            this.assigned.Add(ass);
        }
    }
}
