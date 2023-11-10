using System.Collections.Generic;

namespace DuckGame
{
    public class NMAssignKill : NMAssignWin
    {
        public NMAssignKill(List<Profile> pProfiles, Profile pTheRealWinnerHere)
          : base(pProfiles, pTheRealWinnerHere)
        {
            _sound = "scoreDingShort";
        }

        public NMAssignKill() => _sound = "scoreDingShort";
    }
}
