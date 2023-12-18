namespace DuckGame
{
    public class NMBanned : NMKicked
    {
        public NMBanned()
        {
        }

        public NMBanned(Profile pProfile) => profile = pProfile;
    }
}
