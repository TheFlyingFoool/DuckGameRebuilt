// Decompiled with JetBrains decompiler
// Type: DuckGame.NMMakePlayer
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

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
