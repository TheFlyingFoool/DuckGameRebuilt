using System.Collections.Generic;

namespace DuckGame
{
    public class ProfileStatRank
    {
        public StatInfo stat;
        public float value;
        public List<Profile> profiles = new List<Profile>();

        public ProfileStatRank(StatInfo s, float val, Profile pro = null)
        {
            if (pro != null)
                profiles.Add(pro);
            value = val;
            stat = s;
        }
    }
}
