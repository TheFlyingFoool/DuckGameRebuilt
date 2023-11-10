namespace DuckGame
{
    public class MatchmakingPlayer
    {
        public InputProfile inputProfile;
        public Team team;
        public DuckPersona persona;
        public byte[] customData;
        public Profile originallySelectedProfile;
        public Profile masterProfile;
        public bool spectator;
        public bool isMaster;
    }
}
