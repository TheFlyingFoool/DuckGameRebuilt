// Decompiled with JetBrains decompiler
// Type: DuckGame.NMSpecChangeIndexUpdated
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class NMSpecChangeIndexUpdated : NMEvent
    {
        public Profile profile;
        public byte specChangeIndex;

        public NMSpecChangeIndexUpdated(Profile pProfile, byte pSpecChangeIndex)
        {
            profile = pProfile;
            specChangeIndex = pSpecChangeIndex;
        }

        public NMSpecChangeIndexUpdated()
        {
        }

        public override void Activate()
        {
            if (profile == null)
                return;
            profile.remoteSpectatorChangeIndex = specChangeIndex;
        }
    }
}
