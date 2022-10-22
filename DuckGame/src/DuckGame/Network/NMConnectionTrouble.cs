// Decompiled with JetBrains decompiler
// Type: DuckGame.NMConnectionTrouble
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class NMConnectionTrouble : NMEvent
    {
        public byte profileIndex;

        public NMConnectionTrouble()
        {
        }

        public NMConnectionTrouble(byte pProfile) => profileIndex = pProfile;

        public override void Activate()
        {
            if (profileIndex < 0 || profileIndex >= DuckNetwork.profiles.Count)
                return;
            Profile profile = DuckNetwork.profiles[profileIndex];
        }
    }
}
