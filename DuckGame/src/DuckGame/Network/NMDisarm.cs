namespace DuckGame
{
    public class NMDisarm : NMEvent
    {
        public Duck target;
        public float bump;

        public NMDisarm(Duck pTarget, float pBumpSpeed)
        {
            target = pTarget;
            bump = pBumpSpeed;
        }

        public NMDisarm()
        {
        }

        public override void Activate()
        {
            if (Level.current == null || target == null || !target.isServerForObject || target.profile == null || target.disarmIndex != target.profile.networkIndex)
                return;
            if (target._disarmWait == 0 && target._disarmDisable <= 0)
                target.Disarm(null);
            target.hSpeed = bump;
            RumbleManager.AddRumbleEvent(target.profile, new RumbleEvent(RumbleIntensity.Light, RumbleDuration.Short, RumbleFalloff.None));
        }
    }
}
