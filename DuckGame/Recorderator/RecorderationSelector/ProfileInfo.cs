namespace DuckGame
{
    public class ProfileInfo
    {
        public DuckPersona Persona;
        public string Name;
        public ulong SteamID;
        public Team Team;

        public bool IsUsingRebuilt;
        public bool IsSpectator;
        public bool IsHost;
        public bool IsLocalPlayer;

        public SpriteMap ChatBustDuck;
    }
}