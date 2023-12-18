namespace DuckGame
{
    public class NMChatMessage : NMDuckNetwork
    {
        public Profile profile;
        public ushort index;
        public string text = "";

        public NMChatMessage()
        {
        }

        public NMChatMessage(Profile pProfile, string t, ushort idx)
        {
            profile = pProfile;
            text = t;
            index = idx;
        }
    }
}
