// Decompiled with JetBrains decompiler
// Type: DuckGame.ProfileStatRank
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

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
