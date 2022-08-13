// Decompiled with JetBrains decompiler
// Type: DuckGame.NMNewDuckNetConnection
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class NMNewDuckNetConnection : NMDuckNetwork
    {
        public string identifier;
        public string name;
        public Team team;
        public byte flippers;
        public bool parentalControlsActive;
        public int flagIndex;
        public Profile profile;
        public ulong profileID;
        public NetIndex16 latestGhostIndex;
        public byte persona;

        public NMNewDuckNetConnection()
        {
        }

        public NMNewDuckNetConnection(
          Profile pProfile,
          string id,
          string duckName,
          Team varTeam,
          byte varFlippers,
          bool parentalControls,
          int varFlagIndex,
          ulong pProfileID,
          byte pPersona)
        {
            identifier = id;
            name = duckName;
            team = varTeam;
            flippers = varFlippers;
            parentalControlsActive = parentalControls;
            flagIndex = varFlagIndex;
            profile = pProfile;
            profileID = pProfileID;
            latestGhostIndex = pProfile.latestGhostIndex;
            persona = pPersona;
        }
    }
}
