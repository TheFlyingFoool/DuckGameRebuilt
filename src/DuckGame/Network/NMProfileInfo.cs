// Decompiled with JetBrains decompiler
// Type: DuckGame.NMProfileInfo
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System.Collections.Generic;

namespace DuckGame
{
    public class NMProfileInfo : NMDuckNetworkEvent
    {
        public Profile profile;
        public int fans;
        public int loyalFans;
        public bool areParentalControlsActive;
        public int flagIndex;
        public ushort numCustomHats;
        private List<bool> _unlockList;
        private List<Team> _teams;

        public List<bool> GetHatUnlockStatuses()
        {
            if (this._unlockList == null)
                this._unlockList = new List<bool>();
            return this._unlockList;
        }

        public NMProfileInfo()
        {
        }

        public NMProfileInfo(
          Profile pProfile,
          int numFans,
          int numLoyalFans,
          bool pAreParentalControlsActive,
          int pFlagIndex,
          ushort pNumCustomHats,
          List<Team> pTeams)
        {
            this.profile = pProfile;
            this.fans = numFans;
            this.loyalFans = numLoyalFans;
            this.areParentalControlsActive = pAreParentalControlsActive;
            this.flagIndex = pFlagIndex;
            this.numCustomHats = pNumCustomHats;
            this._teams = pTeams;
        }

        protected override void OnSerialize()
        {
            this._serializedData.Write((ushort)this._teams.Count);
            for (int index = 0; index < this._teams.Count; ++index)
                this._serializedData.Write(this._teams[index].locked);
            base.OnSerialize();
        }

        public override void OnDeserialize(BitBuffer msg)
        {
            ushort num = msg.ReadUShort();
            this._unlockList = new List<bool>();
            for (int index = 0; index < (int)num; ++index)
                this._unlockList.Add(msg.ReadBool());
            base.OnDeserialize(msg);
        }

        public override void Activate()
        {
            if (this.profile != null)
            {
                this.profile.stats.unloyalFans = this.fans;
                this.profile.stats.loyalFans = this.loyalFans;
                this.profile.ParentalControlsActive = this.areParentalControlsActive;
                this.profile.flagIndex = this.flagIndex;
                if (this.numCustomHats > (ushort)0)
                    this.profile.GetCustomTeam((ushort)((uint)this.numCustomHats - 1U));
                this.profile.networkHatUnlockStatuses = this._unlockList;
            }
            base.Activate();
        }
    }
}
