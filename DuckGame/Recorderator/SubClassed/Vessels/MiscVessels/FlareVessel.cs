namespace DuckGame
{
    public class FlareVessel : SomethingSomethingVessel
    {
        public FlareVessel(Thing th) : base(th)
        {
            tatchedTo.Add(typeof(Flare));
            AddSynncl("position", new SomethingSync(typeof(int)));
        }
        public override SomethingSomethingVessel RecDeserialize(BitBuffer b)
        {
            FlareVessel v = new FlareVessel(new Flare(0, 0, null));
            return v;
        }
        public override void PlaybackUpdate()
        {
            Flare f = (Flare)t;
            f.position = CompressedVec2Binding.GetUncompressedVec2((int)valOf("position"), 10000);
            base.PlaybackUpdate();
        }
        public override void RecordUpdate()
        {
            Flare f = (Flare)t;
            addVal("position", CompressedVec2Binding.GetCompressedVec2(f.position, 10000));
            base.RecordUpdate();
        }
    }
}
