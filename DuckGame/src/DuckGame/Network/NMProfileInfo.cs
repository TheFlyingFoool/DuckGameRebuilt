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
            if (_unlockList == null)
                _unlockList = new List<bool>();
            return _unlockList;
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
            profile = pProfile;
            fans = numFans;
            loyalFans = numLoyalFans;
            areParentalControlsActive = pAreParentalControlsActive;
            flagIndex = pFlagIndex;
            numCustomHats = pNumCustomHats;
            _teams = pTeams;
        }

        protected override void OnSerialize()
        {
            _serializedData.Write((ushort)_teams.Count);
            for (int index = 0; index < _teams.Count; ++index)
                _serializedData.Write(_teams[index].locked);
            base.OnSerialize();
        }

        public override void OnDeserialize(BitBuffer msg)
        {
            ushort num = msg.ReadUShort();
            _unlockList = new List<bool>();
            for (int index = 0; index < num; ++index)
                _unlockList.Add(msg.ReadBool());
            base.OnDeserialize(msg);
        }

        public override void Activate()
        {
            if (profile != null)
            {
                profile.stats.unloyalFans = fans;
                profile.stats.loyalFans = loyalFans;
                profile.ParentalControlsActive = areParentalControlsActive;
                profile.flagIndex = flagIndex;
                if (numCustomHats > 0)
                    profile.GetCustomTeam((ushort)(numCustomHats - 1U));
                profile.networkHatUnlockStatuses = _unlockList;
            }
            base.Activate();
        }
    }
}
