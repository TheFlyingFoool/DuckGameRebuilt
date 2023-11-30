namespace DuckGame
{
    public class NMOldAngles : NMEvent
    {
        public bool enabled;
        public Profile profile;

        public NMOldAngles()
        {
        }

        public NMOldAngles(Profile pProfile, bool pEnabled)
        {
            enabled = pEnabled;
            profile = pProfile;
        }

        protected override void OnSerialize()
        {
            _serializedData.WriteProfile(profile);
            _serializedData.Write(enabled);
        }

        public override void OnDeserialize(BitBuffer d)
        {
            profile = d.ReadProfile();
            enabled = d.ReadBool();
        }

        public override void Activate()
        {
            if (profile != null && profile.inputProfile != null)
                profile.inputProfile.oldAngles = enabled;
            base.Activate();
        }
    }
}
