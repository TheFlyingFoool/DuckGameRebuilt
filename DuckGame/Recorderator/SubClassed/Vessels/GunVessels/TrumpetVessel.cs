namespace DuckGame
{
    public class TrumpetVessel : GunVessel
    {
        public TrumpetVessel(Thing th) : base(th)
        {
            tatchedTo.Add(typeof(Trumpet));
            AddSynncl("pitch", new SomethingSync(typeof(byte)));
        }
        public override SomethingSomethingVessel RecDeserialize(BitBuffer b)
        {
            TrumpetVessel v = new TrumpetVessel(new Trumpet(0, -2000));
            return v;
        }
        public override BitBuffer RecSerialize(BitBuffer prevBuffer)
        {
            return prevBuffer;
        }
        public override void PlaybackUpdate()
        {
            Trumpet tb = (Trumpet)t;
            tb.currentPitch = (int)(byte)valOf("pitch") - 2;
            base.PlaybackUpdate();
        }
        public override void RecordUpdate()
        {
            Trumpet tb = (Trumpet)t;
            addVal("pitch", (byte)(tb.currentPitch + 2));
            base.RecordUpdate();
        }
    }
}
