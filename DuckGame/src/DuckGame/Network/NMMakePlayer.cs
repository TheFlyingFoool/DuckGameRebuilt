namespace DuckGame
{
    public class NMMakePlayer : NMEvent
    {
        public Profile profile;
        public Profile replacementSlot;
        public byte specChangeIndex;

        public NMMakePlayer(Profile pProfile, Profile pReplacementSlot, byte pSpecChangeIndex)
        {
            profile = pProfile;
            replacementSlot = pReplacementSlot;
            specChangeIndex = pSpecChangeIndex;
        }

        public NMMakePlayer()
        {
        }

        public override void Activate()
        {
            if (profile != null && replacementSlot != null && profile.slotType == SlotType.Spectator && replacementSlot.slotType != SlotType.Spectator)
            {
                DuckNetwork.MakePlayer_Swap(profile, replacementSlot);
                if (profile.connection == DuckNetwork.localConnection)
                    DuckNetwork.OpenSpectatorInfo(false);
            }
            Send.Message(new NMSpecChangeIndexUpdated(profile, specChangeIndex), NetMessagePriority.ReliableOrdered);
        }
    }
}
