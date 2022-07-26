// Decompiled with JetBrains decompiler
// Type: DuckGame.MatchmakingBoxCore
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System.Collections.Generic;

namespace DuckGame
{
    public class MatchmakingBoxCore
    {
        public bool pulseLocal;
        public bool pulseNetwork;
        public List<ulong> nonPreferredServers = new List<ulong>();
        public List<ulong> blacklist = new List<ulong>();
        public MatchmakingState _state;
        public List<MatchmakingPlayer> matchmakingProfiles = new List<MatchmakingPlayer>();

        public MatchmakingState state => this._state;
    }
}
