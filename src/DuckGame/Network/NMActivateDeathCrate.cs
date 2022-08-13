// Decompiled with JetBrains decompiler
// Type: DuckGame.NMActivateDeathCrate
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class NMActivateDeathCrate : NMEvent
    {
        public DeathCrate crate;
        public byte setting;

        public NMActivateDeathCrate()
        {
        }

        public NMActivateDeathCrate(byte sett, DeathCrate d)
        {
            setting = sett;
            crate = d;
        }

        public override void Activate()
        {
            if (crate == null)
                return;
            crate.settingIndex = setting;
            crate.ActivateSetting(false);
        }
    }
}
