namespace DuckGame
{
    public class NMChatDisabledMessage : NMChatMessage
    {
        public NMChatDisabledMessage()
        {
        }

        public NMChatDisabledMessage(Profile pProfile, string t, ushort idx)
        {
            profile = pProfile;
            text = t;
            index = idx;
        }

        public override string ToString() => "NMChatDisabledMessage(\"" + text + "\")";
    }
}
