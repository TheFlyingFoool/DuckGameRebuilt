namespace DuckGame
{
    [FixedNetworkID(29422)]
    public class NMInputDeviceSwitch : NMDuckNetworkEvent
    {
        public byte index;
        public byte inputType;

        public NMInputDeviceSwitch()
        {
        }

        public NMInputDeviceSwitch(byte idx, byte inpType)
        {
            index = idx;
            inputType = inpType;
        }

        public override void Activate()
        {
            if (index < 0 || index > 3)
                return;
            Profile profile = DuckNetwork.profiles[index];
            if (profile != null && profile.inputProfile != null)
            {
                foreach (DeviceInputMapping inputMappingOverride in profile.inputMappingOverrides)
                {
                    if (inputMappingOverride.inputOverrideType == inputType && inputMappingOverride.deviceOverride != null)
                    {
                        profile.inputProfile.lastActiveOverride = inputMappingOverride.deviceOverride;
                        break;
                    }
                }
            }
            base.Activate();
        }
    }
}
