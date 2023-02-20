// Decompiled with JetBrains decompiler
// Type: DuckGame.NMMakeSpectator
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class NMMakeSpectator : NMEvent
    {
        public Profile profile;
        public Profile replacementSlot;
        public byte specChangeIndex;

        public NMMakeSpectator(Profile pProfile, Profile pReplacementSlot, byte pSpecChangeIndex)
        {
            profile = pProfile;
            replacementSlot = pReplacementSlot;
            specChangeIndex = pSpecChangeIndex;
        }

        public NMMakeSpectator()
        {
        }

        public override void Activate()
        {
            if (profile != null && replacementSlot != null && profile.slotType != SlotType.Spectator && replacementSlot.slotType == SlotType.Spectator)
            {
                DuckNetwork.MakeSpectator_Swap(profile, replacementSlot);
                if (profile.connection == DuckNetwork.localConnection)
                    DuckNetwork.OpenSpectatorInfo(true);
            }
            Send.Message(new NMSpecChangeIndexUpdated(profile, specChangeIndex), NetMessagePriority.ReliableOrdered);
        }
    }
}
