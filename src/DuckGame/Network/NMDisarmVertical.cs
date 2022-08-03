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
            target = pTarget;
            bump = pBumpSpeed;
        }

        public NMDisarmVertical()
        {
        }

        public override void Activate()
        {
            if (Level.current == null || target == null || !target.isServerForObject || target.profile == null || target.disarmIndex != target.profile.networkIndex)
                return;
            if (target._disarmWait == 0 && target._disarmDisable <= 0)
                target.Disarm(null);
            target.vSpeed = bump;
            RumbleManager.AddRumbleEvent(target.profile, new RumbleEvent(RumbleIntensity.Light, RumbleDuration.Short, RumbleFalloff.None));
        }
    }
}
