// Decompiled with JetBrains decompiler
// Type: DuckGame.NMDisarmVertical
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class NMDisarmVertical : NMEvent
    {
        public Duck target;
        public float bump;

        public NMDisarmVertical(Duck pTarget, float pBumpSpeed)
        {
            this.target = pTarget;
            this.bump = pBumpSpeed;
        }

        public NMDisarmVertical()
        {
        }

        public override void Activate()
        {
            if (Level.current == null || this.target == null || !this.target.isServerForObject || this.target.profile == null || (int)this.target.disarmIndex != (int)this.target.profile.networkIndex)
                return;
            if (this.target._disarmWait == 0 && this.target._disarmDisable <= 0)
                this.target.Disarm((Thing)null);
            this.target.vSpeed = this.bump;
            RumbleManager.AddRumbleEvent(this.target.profile, new RumbleEvent(RumbleIntensity.Light, RumbleDuration.Short, RumbleFalloff.None));
        }
    }
}
