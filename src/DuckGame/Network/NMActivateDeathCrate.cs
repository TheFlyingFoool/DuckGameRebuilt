// Decompiled with JetBrains decompiler
// Type: DuckGame.NMActivateDeathCrate
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
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
            this.setting = sett;
            this.crate = d;
        }

        public override void Activate()
        {
            if (this.crate == null)
                return;
            this.crate.settingIndex = this.setting;
            this.crate.ActivateSetting(false);
        }
    }
}
