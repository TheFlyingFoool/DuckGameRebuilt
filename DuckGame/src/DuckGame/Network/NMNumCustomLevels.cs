namespace DuckGame
{
    public class NMNumCustomLevels : NMDuckNetworkEvent
    {
        public int customLevels;

        public NMNumCustomLevels(int pCustomLevels) => customLevels = pCustomLevels;

        public NMNumCustomLevels()
        {
        }

        public override void Activate()
        {
            foreach (Profile profile in DuckNetwork.profiles)
            {
                if (profile.connection == connection)
                    profile.numClientCustomLevels = customLevels;
            }
            TeamSelect2.UpdateModifierStatus();
        }
    }
}
