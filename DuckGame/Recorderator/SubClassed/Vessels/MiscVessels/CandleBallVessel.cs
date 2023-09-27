namespace DuckGame
{
    public class CandleBallVessel : SomethingSomethingVessel
    {
        public CandleBallVessel(Thing th) : base(th)
        {
            tatchedTo.Add(typeof(CandleBall));
            AddSynncl("position", new SomethingSync(typeof(int)));
        }
        public override SomethingSomethingVessel RecDeserialize(BitBuffer b)
        {
            CandleBallVessel v = new CandleBallVessel(new CandleBall(0, 0, null));
            return v;
        }
        public override void PlaybackUpdate()
        {
            CandleBall f = (CandleBall)t;
            f.position = CompressedVec2Binding.GetUncompressedVec2((int)valOf("position"), 10000);
            base.PlaybackUpdate();
        }
        public override void RecordUpdate()
        {
            CandleBall f = (CandleBall)t;
            addVal("position", CompressedVec2Binding.GetCompressedVec2(f.position, 10000));
            base.RecordUpdate();
        }
    }
}
