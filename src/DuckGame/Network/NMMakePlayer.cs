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
            this.profile = pProfile;
            this.replacementSlot = pReplacementSlot;
            this.specChangeIndex = pSpecChangeIndex;
        }

        public NMMakePlayer()
        {
        }

        public override void Activate()
        {
            if (this.profile != null && this.replacementSlot != null && this.profile.slotType == SlotType.Spectator && this.replacementSlot.slotType != SlotType.Spectator)
            {
                DuckNetwork.MakePlayer_Swap(this.profile, this.replacementSlot);
                if (this.profile.connection == DuckNetwork.localConnection)
                    DuckNetwork.OpenSpectatorInfo(false);
            }
            Send.Message((NetMessage)new NMSpecChangeIndexUpdated(this.profile, this.specChangeIndex), NetMessagePriority.ReliableOrdered);
        }
    }
}
