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

        public MatchmakingState state => _state;
    }
}
