namespace DuckGame
{
    public class NMActivateDeathCrate : NMEvent
    {
        public DeathCrate crate;
        public byte setting;

        public NMActivateDeathCrate()
        {
        }

        public NMActivateDeathCrate(byte sett, DeathCrate d)
        {
            setting = sett;
            crate = d;
        }

        public override void Activate()
        {
            if (crate == null)
                return;
            crate.settingIndex = setting;
            crate.ActivateSetting(false);
        }
    }
}
