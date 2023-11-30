namespace DuckGame
{
    public class NMSpecChangeIndexUpdated : NMEvent
    {
        public Profile profile;
        public byte specChangeIndex;

        public NMSpecChangeIndexUpdated(Profile pProfile, byte pSpecChangeIndex)
        {
            profile = pProfile;
            specChangeIndex = pSpecChangeIndex;
        }

        public NMSpecChangeIndexUpdated()
        {
        }

        public override void Activate()
        {
            if (profile == null)
                return;
            profile.remoteSpectatorChangeIndex = specChangeIndex;
        }
    }
}
