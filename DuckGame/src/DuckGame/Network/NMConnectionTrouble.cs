namespace DuckGame
{
    public class NMConnectionTrouble : NMEvent
    {
        public byte profileIndex;

        public NMConnectionTrouble()
        {
        }

        public NMConnectionTrouble(byte pProfile) => profileIndex = pProfile;

        public override void Activate()
        {
            if (profileIndex < 0 || profileIndex >= DuckNetwork.profiles.Count)
                return;
            Profile profile = DuckNetwork.profiles[profileIndex];
        }
    }
}
