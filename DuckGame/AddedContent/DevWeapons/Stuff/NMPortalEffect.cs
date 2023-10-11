namespace DuckGame
{
    [ClientOnly]
    public class NMPortalEffect : NMEvent
    {
        public NMPortalEffect(LPortal p)
        {
            lportal = p;
        }
        public NMPortalEffect()
        {
        }
        public LPortal lportal;

        public override void Activate()
        {
            if (lportal != null)
            {
                lportal.TPEffect();
            }
        }
    }
}
