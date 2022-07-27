// Decompiled with JetBrains decompiler
// Type: DuckGame.NMClientDisconnect
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class NMClientDisconnect : NMDuckNetwork
    {
        public string whom;
        public Profile profile;

        public NMClientDisconnect()
        {
        }

        public NMClientDisconnect(string who, Profile pProfile)
        {
            this.whom = who;
            this.profile = pProfile;
        }

        public NMClientDisconnect(string who, byte pProfile)
        {
            this.whom = who;
            this.profile = DuckNetwork.profiles[pProfile];
        }
    }
}
