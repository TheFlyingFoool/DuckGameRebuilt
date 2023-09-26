namespace DuckGame
{
    public class SuicidePistolVessel : GunVessel
    {
        public SuicidePistolVessel(Thing th) : base(th)
        {
            tatchedTo.Add(typeof(SuicidePistol));
        }
        public override SomethingSomethingVessel RecDeserialize(BitBuffer b)
        {
            SuicidePistolVessel v = new SuicidePistolVessel(new SuicidePistol(0, -2000));
            return v;
        }
        public override BitBuffer RecSerialize(BitBuffer prevBuffer)
        {
            return prevBuffer;
        }
        public override void PlaybackUpdate()
        {
            SuicidePistol sp = (SuicidePistol)t;
            sp.raised = bArray[7];
            base.PlaybackUpdate();
        }
        public override void RecordUpdate()
        {
            SuicidePistol sp = (SuicidePistol)t;
            bArray[7] = sp.raised;
            base.RecordUpdate();
        }
    }
}
